using HoMM;
using HoMM.ClientClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homm.Client
{
    class TextGame
    {
        // Пример визуального отображения данных с сенсоров при отладке.
        static void Print(HommSensorData data)
        {
            Console.WriteLine("---------------------------------");

            Console.WriteLine($"You are here: ({data.Location.X},{data.Location.Y})");

            Console.WriteLine($"You have {data.MyTreasury.Select(z => z.Value + " " + z.Key).Aggregate((a, b) => a + ", " + b)}");

            var location = data.Location.ToLocation();

            Console.Write("W: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.Up)));

            Console.Write("E: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.RightUp)));

            Console.Write("D: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.RightDown)));

            Console.Write("S: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.Down)));

            Console.Write("A: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.LeftDown)));

            Console.Write("Q: ");
            Console.WriteLine(GetObjectAt(data.Map, location.NeighborAt(Direction.LeftUp)));
        }

        static string GetObjectAt(MapData map, Location location)
        {
            if (location.X < 0 || location.X >= map.Width || location.Y < 0 || location.Y >= map.Height)
                return "Outside";
            return map.Objects.
                Where(x => x.Location.X == location.X && x.Location.Y == location.Y)
                .FirstOrDefault()?.ToString() ?? "Nothing";
        }

        static void OnError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        static void OnInfo(string infoMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(infoMessage);
            Console.ResetColor();
        }

        public static void Start()
        {
            var ip = "127.0.0.1";
            var port = 18700;

            var client = new HommClient<HommSensorData>();

            client.OnSensorDataReceived += Print;
            client.OnInfo += OnInfo;
            client.OnError += OnError;

            var sensorData = client.Configurate(ip, port, Program.CvarcTag, operationalTimeLimit: 1000);

            while (true)
            {
                var e = Console.ReadKey();
                switch (e.Key)
                {
                    case ConsoleKey.Escape:
                        client.Exit();
                        return;
                    case ConsoleKey.Q:
                        client.Move(Direction.LeftUp);
                        break;
                    case ConsoleKey.W:
                        client.Move(Direction.Up);
                        break;
                    case ConsoleKey.E:
                        client.Move(Direction.RightUp);
                        break;
                    case ConsoleKey.A:
                        client.Move(Direction.LeftDown);
                        break;
                    case ConsoleKey.S:
                        client.Move(Direction.Down);
                        break;
                    case ConsoleKey.D:
                        client.Move(Direction.RightDown);
                        break;

                }
            }
        }
    }
}
