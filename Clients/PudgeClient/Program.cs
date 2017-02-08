using System;
using System.Linq;
using Pudge;
using Pudge.Player;
using Pudge.World;
using Infrastructure;

namespace PudgeClientExample
{
    class Program
    {
        // Вставьте сюда свой личный CvarcTag для того, чтобы учавствовать в онлайн соревнованиях.
        static readonly Guid CvarcTag = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // Пример визуального отображения данных с сенсоров при отладке.
        static void Print(PudgeSensorsData data)
        {
            Console.WriteLine("---------------------------------");
            if (data.IsDead)
            {
                // Правильное обращение со смертью.
                Console.WriteLine("Ooops, i'm dead :(");
                return;
            }
            Console.WriteLine("I'm here: " + data.SelfLocation);
            Console.WriteLine("My score now: {0}", data.SelfScores);
            Console.WriteLine("Current time: {0:F}", data.WorldTime);
            foreach (var rune in data.Map.Runes)
                Console.WriteLine("Rune! Type: {0}, Size = {1}, Location: {2}", rune.Type, rune.Size, rune.Location);
            foreach (var heroData in data.Map.Heroes)
                Console.WriteLine("Enemy! Type: {0}, Location: {1}, Angle: {2:F}", heroData.Type, heroData.Location, heroData.Angle);
            foreach (var eventData in data.Events)
                Console.WriteLine("I'm under effect: {0}, Duration: {1}", eventData.Event,
                    eventData.Duration - (data.WorldTime - eventData.Start));
            Console.WriteLine("---------------------------------");
            Console.WriteLine();
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

            Debugger.Config = new DebuggerConfig { AlwaysOn = true };
            Debugger.Logger += Console.WriteLine;

            if (args.Length == 0)
                args = new[] { "127.0.0.1", "18700" };
            var ip = args[0];
            var port = int.Parse(args[1]);

            var client = new PudgeClient();

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

            // Угол поворота указывается в градусах, против часовой стрелки.
            // Для поворота по часовой стрелке используйте отрицательные значения.
            client.Rotate(-45);

            client.Move(60);
            client.Wait(0.1);

            // Так можно хукать.
            sensorData = client.Hook();

            // Так дожидаемся, пока хук вернется к нам
            while (true)
            {
                if (sensorData == null)
                    Console.WriteLine("Null reference");
                if (!sensorData.Events.Any(s => s.Event == PudgeEvent.HookThrown))
                    break;
               sensorData = client.Wait(0.1);
            }

            // Пример длинного движения. Move(100) лучше не писать. Мало ли что произойдет за это время ;) 
            for (int i = 0; i < 5; i++)
                client.Move(15);
            client.Wait(1);

            // вот так телепортироваться
            client.Blink(0, 0);
            // вот так ставить варды :)
            client.CreateWard();

            client.Rotate(180);
            client.Move(10);
            client.Wait(2);

            // Корректно завершаем работу
            client.Exit();
            Console.ReadKey();
        }
    }
}