var mapFile = "map.txt";
var map = ParseFile(mapFile);

while (true)
{
    Thread.Sleep(500);
    PrintMap(map);
    Dictionary<Point, CellType> nextMap = new(map);

    foreach (var cell in map)
    {
        var point = cell.Key;
        var type = cell.Value;

        switch (type)
        {
            case CellType.Signal:
                nextMap[point] = CellType.SignalTail;
                break;

            case CellType.SignalTail:
                nextMap[point] = CellType.Wire;
                break;

            case CellType.Wire:
                var x = point.X;
                var y = point.Y;

                var count =
                    CheckSignalInWire(x - 1, y - 1, map)
                    + CheckSignalInWire(x, y - 1, map)
                    + CheckSignalInWire(x + 1, y - 1, map)
                    + CheckSignalInWire(x - 1, y, map)
                    + CheckSignalInWire(x + 1, y, map)
                    + CheckSignalInWire(x - 1, y + 1, map)
                    + CheckSignalInWire(x, y + 1, map)
                    + CheckSignalInWire(x + 1, y + 1, map);

                if (count is 1 or 2)
                    nextMap[point] = CellType.Signal;

                break;
        }
    }

    map = nextMap;
}

int CheckSignalInWire(int x, int y, Dictionary<Point, CellType> map)
{
    return map.TryGetValue(new(x, y), out var type) && type == CellType.Signal ? 1 : 0;
}

void PrintMap(Dictionary<Point, CellType> map)
{
    Console.SetCursorPosition(0, 0);
    Console.Clear();
#pragma warning disable CA1416 // Validate platform compatibility
    // Console.WindowHeight = 100;
    // Console.WindowWidth = 100;
#pragma warning restore CA1416 // Validate platform compatibility

    foreach (var cell in map)
    {
        var x = cell.Key.X;
        var y = cell.Key.Y;
        var type = cell.Value;

        Console.SetCursorPosition(x, y);
        Console.BackgroundColor = type switch
        {
            CellType.Signal => ConsoleColor.Blue,
            CellType.SignalTail => ConsoleColor.Red,
            CellType.Wire => ConsoleColor.Green,
            _ => throw new NotImplementedException(),
        };

        Console.Write(' ');
        Console.ResetColor();
    }
}

Dictionary<Point, CellType> ParseFile(string file)
{
    var mapLines = File.ReadAllLines(file);
    var result = new Dictionary<Point, CellType>();

    for (int y = 0; y < mapLines.Length; y++)
    {
        var temp = mapLines[y];
        for (int x = 0; x < temp.Length; x++)
        {
            var symbol = temp[x];
            Point point = new(x, y);

            switch (symbol)
            {
                case '1':
                    result.Add(point, CellType.Signal);
                    break;
                case '2':
                    result.Add(point, CellType.SignalTail);
                    break;
                case '3':
                    result.Add(point, CellType.Wire);
                    break;
            }
        }
    }

    return result;
}

record Point(int X, int Y);

enum CellType
{
    Signal,
    SignalTail,
    Wire,
}
