namespace Chip8.Core;

public class Keyboard(KeyboardMappings.AvailableMappings mapping)
{
    private readonly bool[] _keys = new bool[16];
    private readonly Lock _lock = new();

    private Dictionary<ConsoleKey, int> _mapping = mapping switch
    {
        KeyboardMappings.AvailableMappings.Qwertz => KeyboardMappings.GetQwertzMapping(),
        KeyboardMappings.AvailableMappings.Qwerty => KeyboardMappings.GetQwertyMapping(),
        _ => throw new ArgumentOutOfRangeException(nameof(mapping), mapping, null)
    };

    public void SetKeyDown(int key)
    {
        if ((uint)key >= _keys.Length) throw new ArgumentOutOfRangeException(nameof(key));
        lock (_lock)
        {
            _keys[key] = true;
        }
    }

    public void SetKeyUp(int key)
    {
        if ((uint)key >= _keys.Length) throw new ArgumentOutOfRangeException(nameof(key));
        lock (_lock)
        {
            _keys[key] = false;
        }
    }

    public void SetPhysicalKeyDown(ConsoleKey physicalKey)
    {
        if (TryMap(physicalKey, out var chipKey)) SetKeyDown(chipKey);
    }

    public void SetPhysicalKeyUp(ConsoleKey physicalKey)
    {
        if (TryMap(physicalKey, out var chipKey)) SetKeyUp(chipKey);
    }

    private bool TryMap(ConsoleKey physicalKey, out int chipKey)
    {
        lock (_lock)
        {
            return _mapping.TryGetValue(physicalKey, out chipKey);
        }
    }

    public void SetMapping(ConsoleKey physicalKey, int chipKey)
    {
        if ((uint)chipKey >= _keys.Length) throw new ArgumentOutOfRangeException(nameof(chipKey));
        lock (_lock)
        {
            _mapping[physicalKey] = chipKey;
        }
    }

    public void SetMapping(Dictionary<ConsoleKey, int> mapping)
    {
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));
        foreach (var v in mapping.Values)
            if ((uint)v >= _keys.Length)
                throw new ArgumentOutOfRangeException(nameof(mapping));

        lock (_lock)
        {
            _mapping = new Dictionary<ConsoleKey, int>(mapping);
        }
    }

    public IReadOnlyDictionary<ConsoleKey, int> GetMapping()
    {
        lock (_lock)
        {
            return new Dictionary<ConsoleKey, int>(_mapping);
        }
    }

    public bool IsPressed(int key)
    {
        if ((uint)key >= _keys.Length) throw new ArgumentOutOfRangeException(nameof(key));
        lock (_lock)
        {
            return _keys[key];
        }
    }

    public void ClearKeys()
    {
        lock (_lock)
        {
            for (var i = 0; i < _keys.Length; i++) _keys[i] = false;
        }
    }
}