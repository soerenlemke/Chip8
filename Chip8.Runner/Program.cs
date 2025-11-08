using Chip8.Core;

var chip8 = new Chip8.Core.Chip8();
var emulator = new Emulator(chip8);

var romPath = Path.Combine(AppContext.BaseDirectory, "ROMs", "IBM Logo.ch8");
emulator.Initialize(romPath);
emulator.Run();