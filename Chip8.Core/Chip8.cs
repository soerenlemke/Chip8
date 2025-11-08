namespace Chip8.Core;

public class Chip8
{
    public const int ScreenWidth = 64;
    public const int ScreenHeight = 32;

    private readonly byte[] _fontSet =
    [
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80 // F
    ];

    private int _stackPointer;

    public Chip8()
    {
        // Load fontset into memory starting at 0x50
        for (var i = 0; i < _fontSet.Length; i++) Memory[0x50 + i] = _fontSet[i];

        // initialize display and screen buffer
        Screen = new byte[ScreenWidth * ScreenHeight];
        Display = new MonoGameDisplay();
    }

    public byte[] Memory { get; set; } = new byte[4096];
    public MonoGameDisplay Display { get; set; } // MonoGame-based display
    public byte[] Screen { get; set; } // 0/1 per pixel, row-major
    public ushort Pc { get; set; } = 0x200; // Program counter
    public ushort I { get; set; } = 0; // Index register
    public byte DelayTimer { get; set; }
    public byte SoundTimer { get; set; }
    public byte[] Vx { get; set; } = new byte[16];
    public byte[] Vf { get; set; } = new byte[16];

    public Keyboard Keyboard { get; set; } =
        new(KeyboardMappings.AvailableMappings.Qwertz); // TODO: allow configuration

    private ushort[] Stack { get; } = new ushort[16];

    public void PushStack(ushort value)
    {
        if (_stackPointer >= Stack.Length) throw new StackOverflowException("Stack overflow");
        Stack[_stackPointer++] = value;
    }

    public ushort PopStack()
    {
        if (_stackPointer == 0) throw new InvalidOperationException("Stack underflow");
        return Stack[--_stackPointer];
    }

    public void CheckTimers()
    {
        DecrementTimers();
        PlaySound();
    }

    private void DecrementTimers()
    {
        // TODO: should be done at 60Hz
        if (DelayTimer > 0) DelayTimer--;
        if (SoundTimer > 0) SoundTimer--;
    }

    private void PlaySound()
    {
        if (SoundTimer > 0) Console.Write("\a");
    }

    public void ClearDisplay()
    {
        Array.Clear(Screen, 0, Screen.Length);
        Display.Clear();
        Display.UpdateFromBuffer(Screen);
    }
}