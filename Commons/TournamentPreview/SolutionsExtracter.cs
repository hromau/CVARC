using Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TournamentPreview
{
    class TournamentParticipantExtraction
    {
        public ExtractionStatus Status { get; set; }
        public TournamentParticipant Participant
        {
            get; set;
        }
    }

    enum ExtractionStatus
    {
        OK,
        CantUnzip,
        NoBinaries,
        NoSource,
        NoPathFile,
        NoExecutable,
        WrongExtension,
        CantFindExecutableFromPathTxt
    }

    class SolutionsExtracter
    {

        public void Print()
        {
            var data = Json.Read<List<TournamentParticipantExtraction>>("extract.json");
            foreach(var e in 
            data.GroupBy(z => z.Status).Select(z => new { status = z.Key, count = z.Count() }).OrderBy(z => z.status)
                )
            {
                Console.WriteLine(e.status + "\t" + e.count);
            }

            foreach (var e in data.Where(z => z.Status != ExtractionStatus.OK))
                Console.WriteLine(e.Participant.Id + "\t" + e.Status);
        }

        public void Extract(DirectoryInfo directory)
        {
            var unzippedDirectory = directory.CreateSubdirectory("Unzipped");
            if (unzippedDirectory.Exists)
                unzippedDirectory.Delete(true);

            var list = new List<TournamentParticipantExtraction>();

            foreach (var file in directory.GetFiles("*.zip"))
            {
                Console.Write("Extracting " + file.Name);
                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                var outputDirectory = new DirectoryInfo(Path.Combine(unzippedDirectory.FullName, name));
                if (!outputDirectory.Exists)
                    outputDirectory.Create();
            

                var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "\"C:\\Program Files\\7-Zip\\7z.exe\"",
                    Arguments = "x \"" + file.FullName + "\" -o\"" + outputDirectory.FullName + "\"",
                    CreateNoWindow = true
                });
                p.WaitForExit();
            }
        }

        public void Control(DirectoryInfo directory)
        {

            var unzippedDirectory = directory.CreateSubdirectory("Unzipped");
           
            var list = new List<TournamentParticipantExtraction>();

            string exePath;
            ExtractionStatus status = ExtractionStatus.OK;

            foreach (var file in directory.GetFiles("*.zip"))
            {
                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                var outputDirectory = new DirectoryInfo(Path.Combine(unzippedDirectory.FullName, name));

                TournamentParticipantExtraction res = null;
                if (status != ExtractionStatus.OK)
                {
                    res = new TournamentParticipantExtraction
                    {
                        Participant = new TournamentParticipant
                        {
                            Id = int.Parse(name),
                            PathToExe = null
                        },
                        Status = status
                    };
                }
                else res=CheckFolder(outputDirectory);

                Console.WriteLine(res.Status);

                list.Add(res);
            }

            Json.Write("extract.json", list);
        }

        private static TournamentParticipantExtraction CheckFolder(DirectoryInfo outputDirectory)
        {
            
            string resultPath= null;
            var status = ExtractionStatus.OK;


            var name = outputDirectory.Name;


            if (outputDirectory.GetDirectories().Count() == 1)
                outputDirectory = outputDirectory.GetDirectories().First();


            if (status == ExtractionStatus.OK)
            {

                var subs = outputDirectory.GetDirectories().Select(z => z.Name.ToLower()).ToList();
                if (!subs.Contains("binaries"))
                    status = ExtractionStatus.NoBinaries;
                if (!subs.Contains("source"))
                    status = ExtractionStatus.NoSource;
                if (!outputDirectory.GetFiles().Select(z => z.Name).Contains("path.txt"))
                    status = ExtractionStatus.NoPathFile;
            }

            string exePath = null;
            if (status == ExtractionStatus.OK)
            {
                try
                {
                    var pathFile = outputDirectory.GetFiles("path.txt").Single();
                    exePath = File.ReadAllText(pathFile.FullName).Trim();
                }
                catch
                {
                    status = ExtractionStatus.NoPathFile;
                }
            }

            if (status == ExtractionStatus.OK)
            {
                bool found = false;
                foreach (var folder in new[] { "", "binaries" })
                {
                    foreach (var ending in new[] { "", ".exe" })
                    {
                        var path = Path.Combine(outputDirectory.FullName, folder, exePath + ending);
                        var exeFile = new FileInfo(path);
                        if (exeFile.Exists)
                        {
                            found = true;
                            if (exeFile.Extension != ".exe")
                                status = ExtractionStatus.WrongExtension;
                            resultPath = path;
                            break;
                        }
                    }
                    if (found) break;
                }

                if (!found)
                    status = ExtractionStatus.CantFindExecutableFromPathTxt;
            }

            return new TournamentParticipantExtraction
            {
                Participant = new TournamentParticipant
                {
                    Id = int.Parse(name),
                    PathToExe = resultPath
                },
                Status = status
            };
        }
    }
}
