namespace Chip8.Core;

public class Emulator(Chip8 chip8)
{
    private readonly Chip8 _chip8 = chip8;
    private ushort _instruction;

    public void Initialize()
    {
        // TODO: RomLoader to load a ROM into memory
        // TODO: setup initial state (PC, SP, registers, etc.)
        // TODO: load display

        throw new NotImplementedException();
    }

    public void RunCycle()
    {
        // TODO: how can we make cycle time consistent?
        // TODO: how can we make cycle time configurable?

        // Note: In practice, a standard speed of around 700 CHIP-8 instructions per second fits well enough for most CHIP-8 programs you’ll find

        Fetch();
        DecodeAndExecute();
    }

    // the instruction from memory at the current PC (program counter)
    private void Fetch()
    {
        if (_chip8.Pc + 1 >= _chip8.Memory.Length)
            throw new InvalidOperationException("Program counter out of bounds");

        var high = _chip8.Memory[_chip8.Pc];
        var low = _chip8.Memory[_chip8.Pc + 1];
        _instruction = (ushort)((high << 8) | low);

        _chip8.Pc += 2;
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
                _chip8.ClearDisplay();
                return;
            case 0x00EE:
                _chip8.Pc = _chip8.PopStack();
                return;
        }

        switch (firstNibble)
        {
            case 0x1: // 1NNN - JP addr
                _chip8.Pc = nnn;
                return;

            case 0x6: // 6XKK - LD Vx, byte
                _chip8.Vx[x] = kk;
                return;

            case 0x7: // 7XKK - ADD Vx, byte (ohne Carry Flag)
                _chip8.Vx[x] = (byte)(_chip8.Vx[x] + kk);
                return;

            case 0xA: // ANNN - LD I, addr
                _chip8.I = nnn;
                return;

            case 0xD: // DXYN - DRW Vx, Vy, nibble
                var collision = DrawSprite(x, y, n);
                // VF = 1 if any pixel was unset (collision), otherwise 0
                _chip8.Vx[0xF] = collision ? (byte)1 : (byte)0;
                return;

            default:
                throw new NotImplementedException($"unknown opcode: 0x{opcode:X4}");
        }
    }
    
    private bool DrawSprite(int xReg, int yReg, int n)
    {
        var vx = _chip8.Vx[xReg];
        var vy = _chip8.Vx[yReg];
        var collision = false;

        for (var row = 0; row < n; row++)
        {
            var spriteByte = _chip8.Memory[_chip8.I + row];
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
                if (_chip8.Screen[idx] == 1) collision = true;

                _chip8.Screen[idx] ^= 1;
            }
        }

        _chip8.Display.UpdateFromBuffer(_chip8.Screen);
        return collision;
    }
}