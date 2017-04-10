using Infrastructure;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        NoBinaries,
        NoSource,
        NoPathFile,
        NoExecutable,
        WrongExecutable
    }

    class SolutionsExtracter
    {
        public void Extract(DirectoryInfo directory)
        {
            var unzippedDirectory = directory.CreateSubdirectory("Unzipped");
            if (unzippedDirectory.Exists)
                unzippedDirectory.Delete();

            var list = new List<TournamentParticipantExtraction>();

            foreach (var file in directory.GetFiles("*.zip"))
            {
                var name = file.Name.Substring(file.Name.IndexOf("."));
                var zip = ZipFile.Read(file.FullName);
                var outputDirectory = new DirectoryInfo(Path.Combine(unzippedDirectory.FullName, name));
                if (!outputDirectory.Exists)
                    outputDirectory.Create();
                zip.ExtractAll(outputDirectory.FullName);

                var subs = outputDirectory.GetDirectories().Select(z => z.Name).ToList();
                ExtractionStatus status = ExtractionStatus.OK;
                if (!subs.Contains("binaries"))
                    status = ExtractionStatus.NoBinaries;
                if (!subs.Contains("sources"))
                    status = ExtractionStatus.NoSource;
                if (!outputDirectory.GetFiles().Select(z => z.Name).Contains("path.txt"))
                    status = ExtractionStatus.NoPathFile;

                string exePath = null;
                try
                {
                    var pathFile = outputDirectory.GetFiles("path.txt").Single();
                    exePath = File.ReadAllText(pathFile.FullName).Trim();
                    var exeFile = new FileInfo(outputDirectory.FullName + "\\binaries\\" + exePath);
                    if (!File.Exists(exePath))
                        status = ExtractionStatus.NoBinaries;
                    if (exeFile.Extension != "exe")
                        status = ExtractionStatus.WrongExecutable;
                    exePath = exeFile.FullName;
                }
                catch
                {
                    exePath = null;
                }
                list.Add(new TournamentParticipantExtraction
                {
                    Participant = new TournamentParticipant
                    {
                        Id = int.Parse(name),
                        PathToExe = exePath
                    },
                    Status = status
                });
            }

            Json.Write("extract.json", list);
        }
    }
}
