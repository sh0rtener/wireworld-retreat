var mapFile = "map.txt";
var map = ParseFile(mapFile);

var directions = new Point[] {
    new(-1, -1),
    new(0, -1),
    new(+1, -1),
    new(-1, 0),
    new(+1, 0),
    new(-1, +1),
    new(0, +1),
    new(+1, +1),
};

var cursorPoint = new Point(0, 0);

var task = Task.Run(() =>
{
    while (true)
    {
        var key = Console.ReadKey();

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                cursorPoint = new(cursorPoint.X, cursorPoint.Y - 1);
                break;
            case ConsoleKey.DownArrow:
                cursorPoint = new(cursorPoint.X, cursorPoint.Y + 1);
                break;
            case ConsoleKey.LeftArrow:
                cursorPoint = new(cursorPoint.X - 1, cursorPoint.Y);
                break;
            case ConsoleKey.RightArrow:
                cursorPoint = new(cursorPoint.X + 1, cursorPoint.Y);
                break;
            case ConsoleKey.W:
                cursorPoint = new(0, 0);
                break;
            case ConsoleKey.Spacebar:
                if (!map.ContainsKey(cursorPoint))
                    map[cursorPoint] = CellType.Wire;
                break;
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(key.KeyChar);
    }
});

while (true)
{
    Thread.Sleep(500);
    PrintUI(map);
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

                var count = directions
                    .Select(dir => new Point(x + dir.X, y + dir.Y))
                    .Select(neighbor => CheckSignalInWire(neighbor.X, neighbor.Y, map))
                    .Sum();

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

void PrintUI(Dictionary<Point, CellType> map)
{
    Console.SetCursorPosition(0, 0);
    Console.Clear();
    
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

    Console.SetCursorPosition(cursorPoint.X, cursorPoint.Y);
    Console.Write("*");
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
