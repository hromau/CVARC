using System;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.Rules;

namespace HommClientExample
{
    class Program
    {
        // Вставьте сюда свой личный CvarcTag для того, чтобы учавствовать в онлайн соревнованиях.
        static readonly Guid CvarcTag = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // Пример визуального отображения данных с сенсоров при отладке.
        static void Print(HommSensorData data)
        {
            Console.WriteLine("---------------------------------");

            Console.WriteLine($"You are here: ({data.Location.X},{data.Location.Y})");
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

        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "127.0.0.1", "18700" };
            var ip = args[0];
            var port = int.Parse(args[1]);

            var client = new HommClient<HommSensorData>();

            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.OnSensorDataReceived += Print;
            // Время от времени сервер может отправлять вам информационные сообщения. На каждое сообщение произойдет событие OnInfo
            client.OnInfo += OnInfo;
            // Если случится ошибка -- произойдет событие OnError, а все дальнейшие команды не будут обработаны.
            client.OnError += OnError;

            // У метода Configurate так же есть необязательные аргументы:
            // timeLimit -- время в секундах, сколько будет идти матч (по умолчанию 90)
            // operationalTimeLimit -- время в секундах, отображающее ваш лимит на операции в сумме за всю игру
            // По умолчанию -- 1000. На турнире будет использоваться значение 5. Подробнее про это можно прочитать в правилах.
            // isOnLeftSide -- предпочитаемая сторона. Принимается во внимание во время отладки. По умолчанию true.
            // seed -- источник энтропии для случайного появления рун. По умолчанию -- 0. 
            // При изменении руны будут появляться в другом порядке
            // speedUp -- ускорение отладки в два раза. Может вызывать снижение FPS на слабых машинах
            var sensorData = client.Configurate(ip, port, CvarcTag);
            // Пудж узнает о всех событиях, происходящих в мире, с помощью сенсоров.
            // Для передачи и представления данных с сенсоров служат объекты класса PudgeSensorsData.
            // Каждое действие возвращает новые данные с сенсоров.
            // Мы подписались на обработку событий, поэтому обрабатывать отдельно каждый экземпляр нет необходимости

            while(true)
            {
                var e = Console.ReadKey();
                switch(e.Key)
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