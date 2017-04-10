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

        const int verifyPort = 88312;

        public TournamentVerifier()
        {

            listener = new TcpListener(verifyPort);
            listener.Start();
        }

        public void Dispose()
        {
            listener.Stop();
        }

        public void VerifyAndSave()
        {
            var extraction = Json.Read<List<TournamentParticipantExtraction>>("extract.json");
            var list = extraction.Where(z => z.Status == ExtractionStatus.OK).Select(z => z.Participant).ToList();
            var result = Verify(list);
            Json.Write("verify.json", result);
            var participants = result.Where(z => z.Status == TournamentVerificationStatus.OK).Select(z => z.Participant).ToList();
            Json.Write("participants.json",participants);
        }

        public List<TournamentVerificationResult> Verify(List<TournamentParticipant> list)
        {

            var result = new List<TournamentVerificationResult>();
            foreach (var e in list)
            {
                try
                {
                    var r = Verify(e);
                    result.Add(new TournamentVerificationResult { Participant = e, Status = r });
                }
                catch (VerificationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    result.Add(new TournamentVerificationResult { Participant = e, Status = TournamentVerificationStatus.UnknownError, AdditinalInformation = ex.Message });
                }
            }
            return result;
        }

        private TournamentVerificationStatus Verify(TournamentParticipant e)
        {
            if (!File.Exists(e.PathToExe))
                return TournamentVerificationStatus.FileNotFound;

            if (listener.Pending())
                throw new VerificationException("Something is wrong: pending clients before client starts. Possible interference");

            var process = new Process();
            process.StartInfo.FileName = e.PathToExe;
            process.StartInfo.Arguments = "127.0.0.1 " + verifyPort;
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
                        client.ReadJson<GameSettings>();
                        client.ReadJson<JObject>();
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
                listener.AcceptTcpClient();
                result = TournamentVerificationStatus.SecondConnectionDetected;
            }

            process.Kill();
            return result;
        }
    }
}
