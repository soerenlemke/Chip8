using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chip8.Core.Display;

public sealed class MonoGameDisplay : Game, IDisplay
{
    private const int Width = 64;
    private const int Height = 32;
    private readonly Lock _bufLock = new();

    private readonly Thread _gameThread;
    private readonly int _scale;
    private volatile bool _disposed;
    private volatile bool _isRunning;
    private byte[]? _latestBuffer;
    private Color[]? _pixels;
    private SpriteBatch? _spriteBatch;
    private Texture2D? _texture;

    public MonoGameDisplay(int scale = 10, string title = "CHIP-8")
    {
        _scale = Math.Max(1, scale);

        var graphics = new GraphicsDeviceManager(this)
        {
            PreferMultiSampling = false
        };
        graphics.PreferredBackBufferWidth = Width * _scale;
        graphics.PreferredBackBufferHeight = Height * _scale;

        Window.Title = title;

        _gameThread = new Thread(() =>
        {
            try
            {
                Run();
            }
            catch (Exception)
            {
                // swallow to avoid crashing host thread; inspect logs if needed
            }
        })
        {
            IsBackground = true
        };

        try
        {
            _gameThread.SetApartmentState(ApartmentState.STA);
        }
        catch (PlatformNotSupportedException)
        {
        }
        catch (NotSupportedException)
        {
        }

        _gameThread.Start();
    }

    public bool IsRunning => _isRunning;

    public event EventHandler? WindowClosed;

    public void UpdateFromBuffer(byte[] screen)
    {
        ArgumentNullException.ThrowIfNull(screen);
        if (screen.Length != Width * Height) throw new ArgumentException("screen buffer size mismatch");

        lock (_bufLock)
        {
            _latestBuffer = (byte[])screen.Clone();
        }
    }

    public void Clear()
    {
        ClearTexture();
    }

    // Public Dispose: recommended pattern to satisfy CA1816
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _texture = new Texture2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color);
        _pixels = new Color[Width * Height];
        ClearTexture();
        base.LoadContent();
        _isRunning = true;
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        _isRunning = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        base.OnExiting(sender, args);
    }

    protected override void Draw(GameTime gameTime)
    {
        lock (_bufLock)
        {
            if (_latestBuffer != null && _pixels != null && _texture != null)
            {
                for (var i = 0; i < _latestBuffer.Length && i < _pixels.Length; i++)
                    _pixels[i] = _latestBuffer[i] != 0 ? Color.White : Color.Black;
                _texture.SetData(_pixels);
                _latestBuffer = null;
            }
        }

        GraphicsDevice.Clear(Color.Black);

        if (_spriteBatch != null && _texture != null)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            var dest = new Rectangle(0, 0, Width * _scale, Height * _scale);
            _spriteBatch.Draw(_texture, dest, Color.White);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    private void ClearTexture()
    {
        _pixels ??= new Color[Width * Height];
        for (var i = 0; i < _pixels.Length; i++) _pixels[i] = Color.Black;
        _texture?.SetData(_pixels);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            try
            {
                if (_isRunning) Exit();
                if (_gameThread.IsAlive) _gameThread.Join(1000);
                // Free managed resources created here if any
                _texture?.Dispose();
                _spriteBatch?.Dispose();
            }
            catch
            {
                // ignored
            }

        _disposed = true;
    }
}