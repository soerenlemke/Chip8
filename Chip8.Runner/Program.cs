using Chip8.Core;
using Chip8.Core.Display;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<IDisplay, MonoGameDisplay>();
services.AddSingleton<Chip8.Core.Chip8>();
services.AddSingleton<Emulator>();

using var provider = services.BuildServiceProvider();

var emulator = provider.GetRequiredService<Emulator>();
var romPath = Path.Combine(AppContext.BaseDirectory, "ROMs", "IBM Logo.ch8");
emulator.Initialize(romPath);
emulator.Run(); 