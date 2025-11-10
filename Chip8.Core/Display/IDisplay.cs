namespace Chip8.Core.Display;

public interface IDisplay : IDisposable
{
    new void Dispose();
    event EventHandler? WindowClosed;
    void UpdateFromBuffer(byte[] screen);
    void Clear();
}