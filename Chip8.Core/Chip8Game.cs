using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chip8.Core;

public class Chip8Game : Game
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _scale;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;
    private Color[] _pixels;
    private readonly object _bufLock = new();
    private byte[]? _latestBuffer;
    private volatile bool _running;

    public bool IsRunning => _running;

    public Chip8Game(int width = 64, int height = 32, int scale = 10, string title = "CHIP-8")
    {
        _width = width;
        _height = height;
        _scale = Math.Max(1, scale);
        Window.Title = title;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferMultiSampling = false
        };
        _graphics.PreferredBackBufferWidth = _width * _scale;
        _graphics.PreferredBackBufferHeight = _height * _scale;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _texture = new Texture2D(GraphicsDevice, _width, _height, false, SurfaceFormat.Color);
        _pixels = new Color[_width * _height];
        ClearTexture();
        base.LoadContent();
        _running = true;
    }

    protected override void Draw(GameTime gameTime)
    {
        lock (_bufLock)
        {
            if (_latestBuffer != null)
            {
                // convert byte[] (0/1 per pixel) to Color[]
                for (var i = 0; i < _latestBuffer.Length && i < _pixels.Length; i++)
                {
                    var on = _latestBuffer[i] != 0;
                    _pixels[i] = on ? Color.White : Color.Black;
                }
                _texture.SetData(_pixels);
                _latestBuffer = null;
            }
        }

        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        var dest = new Rectangle(0, 0, _width * _scale, _height * _scale);
        _spriteBatch.Draw(_texture, dest, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void SetBuffer(byte[] screen)
    {
        ArgumentNullException.ThrowIfNull(screen);
        if (screen.Length != _width * _height) throw new ArgumentException("screen buffer size mismatch");
        
        lock (_bufLock)
        {
            // copy to avoid threading issues
            _latestBuffer = (byte[])screen.Clone();
        }
    }

    public void ClearTexture()
    {
        if (_pixels == null)
            _pixels = new Color[_width * _height];

        for (int i = 0; i < _pixels.Length; i++) _pixels[i] = Color.Black;

        if (_texture != null)
            _texture.SetData(_pixels);
    }

}