namespace Chip8.Core;

public interface IDisplay
{ 
    void Dispose();
    event EventHandler? WindowClosed;
    void UpdateFromBuffer(byte[] screen);
    void Clear();
}