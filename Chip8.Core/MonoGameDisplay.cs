namespace Chip8.Core;

public class MonoGameDisplay : IDisposable
{
    private readonly Chip8Game _game;
    private readonly Thread _gameThread;
    private volatile bool _started;
    private volatile bool _disposed;

    public MonoGameDisplay(int scale = 10, string title = "CHIP-8")
    {
        _game = new Chip8Game(64, 32, scale, title);
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
        
        _gameThread.SetApartmentState(ApartmentState.STA); // required on some platforms
        _gameThread.Start();
        _started = true;
    }

    public void UpdateFromBuffer(byte[] screen)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MonoGameDisplay));
        if (!_started) return;
        _game.SetBuffer(screen);
    }

    public bool PollEvents()
    {
        // returns false when window closed
        return _game?.IsRunning ?? false;
    }

    public void Clear()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MonoGameDisplay));
        _game.ClearTexture();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            if (_game.IsRunning)
            {
                _game.Exit();
            }
            if (_gameThread.IsAlive)
            {
                _gameThread.Join(1000);
            }
        }
        catch
        {
            // ignored
        }
    }
}