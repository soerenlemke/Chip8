namespace Chip8.Core;

public static class RomLoader
{
    private const int LoadAddress = 0x200;
    
    public static void LoadRom(Chip8 chip8, string romPath)
    {
        ArgumentNullException.ThrowIfNull(chip8);
        if (string.IsNullOrWhiteSpace(romPath)) throw new ArgumentException("ROM path cannot be null or empty.", nameof(romPath));
        if (!File.Exists(romPath)) throw new FileNotFoundException("ROM file not found.", romPath);
        
        var rom = File.ReadAllBytes(romPath);
        if (rom.Length == 0) throw new FileNotFoundException("ROM file not found.", romPath);
        
        if (rom.Length + LoadAddress > chip8.Memory.Length)
            throw new InvalidOperationException("ROM is too large to fit in memory.");
        
        Array.Copy(rom, 0, chip8.Memory, LoadAddress, rom.Length);
        chip8.Pc = LoadAddress;
    }
}