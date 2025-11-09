namespace Chip8.Core;

public class Emulator(Chip8 chip8)
{
    private ushort _instruction;
    private bool _windowClosed;

    public void Initialize(string romPath)
    {
        chip8.Display.WindowClosed += OnWindowClosed;
        RomLoader.LoadRom(chip8, romPath);
    }

    public void Run()
    {
        // TODO: how can we make cycle time consistent?
        // TODO: how can we make cycle time configurable?

        while (!_windowClosed)
        {
            Fetch();
            DecodeAndExecute();
        }

        chip8.Display.WindowClosed -= OnWindowClosed;
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _windowClosed = true;
    }

    // the instruction from memory at the current PC (program counter)
    private void Fetch()
    {
        if (chip8.Pc + 1 >= chip8.Memory.Length)
            throw new InvalidOperationException("Program counter out of bounds");

        var high = chip8.Memory[chip8.Pc];
        var low = chip8.Memory[chip8.Pc + 1];
        _instruction = (ushort)((high << 8) | low);

        chip8.Pc += 2;
    }

    // the instruction to find out what the emulator should do
    private void DecodeAndExecute()
    {
        var opcode = _instruction;
        var nnn = (ushort)(opcode & 0x0FFF);
        var n = (byte)(opcode & 0x000F);
        var x = (byte)((opcode & 0x0F00) >> 8);
        var y = (byte)((opcode & 0x00F0) >> 4);
        var kk = (byte)(opcode & 0x00FF);
        var firstNibble = (byte)((opcode & 0xF000) >> 12);

        switch (opcode)
        {
            case 0x00E0:
                chip8.ClearDisplay();
                return;
            case 0x00EE:
                chip8.Pc = chip8.PopStack();
                return;
        }

        // TODO: implement all opcodes

        switch (firstNibble)
        {
            case 0x1: // 1NNN - JP addr
                chip8.Pc = nnn;
                return;

            case 0x6: // 6XKK - LD Vx, byte
                chip8.Vx[x] = kk;
                return;

            case 0x7: // 7XKK - ADD Vx, byte (ohne Carry Flag)
                chip8.Vx[x] = (byte)(chip8.Vx[x] + kk);
                return;

            case 0xA: // ANNN - LD I, addr
                chip8.I = nnn;
                return;

            case 0xD: // DXYN - DRW Vx, Vy, nibble
                var collision = DrawSprite(x, y, n);
                // VF = 1 if any pixel was unset (collision), otherwise 0
                chip8.Vx[0xF] = collision ? (byte)1 : (byte)0;
                return;

            default:
                throw new NotImplementedException($"unknown opcode: 0x{opcode:X4}");
        }
    }

    private bool DrawSprite(int xReg, int yReg, int n)
    {
        var vx = chip8.Vx[xReg];
        var vy = chip8.Vx[yReg];
        var collision = false;

        for (var row = 0; row < n; row++)
        {
            var spriteByte = chip8.Memory[chip8.I + row];
            for (var bit = 0; bit < 8; bit++)
            {
                var px = (vx + (7 - bit)) % Chip8.ScreenWidth;
                if (px < 0) px += Chip8.ScreenWidth;
                var py = (vy + row) % Chip8.ScreenHeight;
                if (py < 0) py += Chip8.ScreenHeight;

                var idx = py * Chip8.ScreenWidth + px;
                var spriteBit = (spriteByte >> bit) & 0x1;

                if (spriteBit == 0) continue;

                // if pixel was set and will be toggled off => collision
                if (chip8.Screen[idx] == 1) collision = true;

                chip8.Screen[idx] ^= 1;
            }
        }

        chip8.Display.UpdateFromBuffer(chip8.Screen);
        return collision;
    }
}