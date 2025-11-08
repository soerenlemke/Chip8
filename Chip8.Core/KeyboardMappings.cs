namespace Chip8.Core;

public static class KeyboardMappings
{
    public enum AvailableMappings
    {
        Qwertz,
        Qwerty
    }

    public static Dictionary<ConsoleKey, int> GetQwertzMapping()
    {
        return new Dictionary<ConsoleKey, int>
        {
            [ConsoleKey.D1] = 0x1,
            [ConsoleKey.D2] = 0x2,
            [ConsoleKey.D3] = 0x3,
            [ConsoleKey.D4] = 0xC,

            [ConsoleKey.Q] = 0x4,
            [ConsoleKey.W] = 0x5,
            [ConsoleKey.E] = 0x6,
            [ConsoleKey.R] = 0xD,

            [ConsoleKey.A] = 0x7,
            [ConsoleKey.S] = 0x8,
            [ConsoleKey.D] = 0x9,
            [ConsoleKey.F] = 0xE,

            [ConsoleKey.Y] = 0xA,
            [ConsoleKey.X] = 0x0,
            [ConsoleKey.C] = 0xB,
            [ConsoleKey.V] = 0xF
        };
    }

    public static Dictionary<ConsoleKey, int> GetQwertyMapping()
    {
        return new Dictionary<ConsoleKey, int>
        {
            [ConsoleKey.D1] = 0x1,
            [ConsoleKey.D2] = 0x2,
            [ConsoleKey.D3] = 0x3,
            [ConsoleKey.D4] = 0xC,

            [ConsoleKey.Q] = 0x4,
            [ConsoleKey.W] = 0x5,
            [ConsoleKey.E] = 0x6,
            [ConsoleKey.R] = 0xD,

            [ConsoleKey.A] = 0x7,
            [ConsoleKey.S] = 0x8,
            [ConsoleKey.D] = 0x9,
            [ConsoleKey.F] = 0xE,

            [ConsoleKey.Z] = 0xA,
            [ConsoleKey.X] = 0x0,
            [ConsoleKey.C] = 0xB,
            [ConsoleKey.V] = 0xF
        };
    }
}