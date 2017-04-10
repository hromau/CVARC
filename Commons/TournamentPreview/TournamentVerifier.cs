using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TournamentPreview
{
    class TournamentVerifier : IDisposable
    {
        TcpListener listener;

        const int verifyPort = 8832;

        public static void Print()
        {
            var data = Json.Read<List<TournamentVerificationResult>>("verify.json");
            foreach (var e in
            data.GroupBy(z => z.Status).Select(z => new { status = z.Key, count = z.Count() }).OrderBy(z => z.status)
                )
            {
                Console.WriteLine(e.status + "\t" + e.count);
            }

            foreach (var e in data.Where(z => z.Status != TournamentVerificationStatus.OK))
                Console.WriteLine(e.Participant.Id + "\t" + e.Status);
        }

        public TournamentVerifier()
        {

            listener = new TcpListener(verifyPort);
            listener.Start();
        }

        public void Dispose()
        {
            listener.Stop();
        }

        public void VerifyAndSave(string expectedLevel)
        {
            var extraction = Json.Read<List<TournamentParticipantExtraction>>("extract.json");
            var list = extraction.Where(z => z.Status == ExtractionStatus.OK).Select(z => z.Participant).ToList();
            var result = Verify(list,expectedLevel);
            Json.Write("verify.json", result);
            var participants = result.Where(z => z.Status == TournamentVerificationStatus.OK).Select(z => z.Participant).ToList();
            Json.Write("participants.json",participants);
        }

        public List<TournamentVerificationResult> Verify(List<TournamentParticipant> list, string expectedLevel)
        {

            var result = new List<TournamentVerificationResult>();
            foreach (var e in list)
            {
                Console.Write(e.Id);
                TournamentVerificationStatus  status =  TournamentVerificationStatus.OK;
                string addInfo = null;
                try
                {
                    status = Verify(e, expectedLevel);
                }
                catch (VerificationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    status = TournamentVerificationStatus.UnknownError;
                    addInfo = ex.Message;
                }

                result.Add(new TournamentVerificationResult { Participant = e, Status = status, AdditinalInformation=addInfo });
                Console.WriteLine(status);
            }
            return result;
        }

        private TournamentVerificationStatus Verify(TournamentParticipant e, string expectedLevel)
        {
            if (!File.Exists(e.PathToExe))
                return TournamentVerificationStatus.FileNotFound;

            if (listener.Pending())
                throw new VerificationException("Something is wrong: pending clients before client starts. Possible interference");

            var process = new Process();
            process.StartInfo.FileName = e.PathToExe;
            process.StartInfo.Arguments = "127.0.0.1 " + verifyPort;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            var watch = new Stopwatch();
            watch.Start();
            var result = TournamentVerificationStatus.OK;
            while (true)
            {
                if (listener.Pending())
                {

                    var client = listener.AcceptTcpClient();
                    try
                    {
                        var settings = client.ReadJson<GameSettings>();
                        Console.WriteLine(settings.LoadingData.Level);
                        var state = client.ReadJson<JObject>();
                    }
                    catch
                    {
                        result = TournamentVerificationStatus.WrongFormat;
                    }
                    break;
                }
                else if (watch.ElapsedMilliseconds > 1000)
                {
                    result = TournamentVerificationStatus.ConnectionFailed;
                    break;
                }
            }
            Thread.Sleep(500);
            while (listener.Pending())
            {
                var client = listener.AcceptTcpClient();
                var trash = client.ReadJson<JObject>();
                if (result != TournamentVerificationStatus.ConnectionFailed)
                    result = TournamentVerificationStatus.SecondConnectionDetected;
            }

            process.Kill();
            return result;
        }
    }
}
