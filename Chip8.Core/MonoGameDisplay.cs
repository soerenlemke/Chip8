namespace Chip8.Core;

public class MonoGameDisplay : IDisposable
{
    private readonly Chip8Game _game;
    private readonly Thread _gameThread;
    private volatile bool _disposed;
    private volatile bool _started;

    public MonoGameDisplay(int scale = 10, string title = "CHIP-8")
    {
        _game = new Chip8Game(64, 32, scale, title);

        _game.Exiting += OnGameExiting;

        _gameThread = new Thread(() =>
        {
            try
            {
                _game.Run();
            }
            catch (Exception)
            {
                // swallow to avoid crashing host thread; inspect logs if needed
            }
            finally
            {
                _started = false;
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
            // Plattform unterstützt STA/COM nicht — ignorieren
        }
        catch (NotSupportedException)
        {
            // ältere Laufzeiten können NotSupportedException werfen
        }

        _gameThread.Start();
        _started = true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _game.Exiting -= OnGameExiting;

        try
        {
            if (_game.IsRunning) _game.Exit();
            if (_gameThread.IsAlive) _gameThread.Join(1000);
        }
        catch
        {
            // ignored
        }
    }

    public event EventHandler? WindowClosed;

    public void UpdateFromBuffer(byte[] screen)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MonoGameDisplay));
        if (!_started) return;
        _game.SetBuffer(screen);
    }

    public void Clear()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MonoGameDisplay));
        _game.ClearTexture();
    }

    private void OnGameExiting(object? sender, EventArgs e)
    {
        _started = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }
}