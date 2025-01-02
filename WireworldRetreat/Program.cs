var mapFile = "map.txt";
var map = ParseFile(mapFile);

foreach (var kv in map)
{
    Console.WriteLine("x: {0}; y: {1}, v: {2}", kv.Key.X, kv.Key.Y, kv.Value);
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
