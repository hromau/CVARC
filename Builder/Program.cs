﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Builder
{
    class Program
    {
        const string UnityEditorPath = @"C:\Program Files\Unity\Hub\Editor\2021.2.9f1\Editor\Unity.exe";
        const string SevenZ = @"C:\Program Files\7-Zip\7z.exe";
        const string I18Source = @"C:\Program Files\Unity\Hub\Editor\2021.2.9f1\Editor\Data\MonoBleedingEdge\lib\mono\unity";

        const string Release = "Release";
        const string BinariesName = "Binaries";

        static string BinariesPath => Release + "\\" + BinariesName;

        static void XCopyRecursice(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);
            foreach (var file in new DirectoryInfo(from).GetFiles())
                File.Copy(from + "\\" + file.Name, to + "\\" + file.Name, true);
            foreach (var directory in new DirectoryInfo(from).GetDirectories())
                XCopyRecursice(directory.FullName, to + "\\" + directory.Name);
        }

        static void XCopy(string from, string to)
        {
            Console.Write("Copying " + from + "... ");
            XCopyRecursice(from, to);
            Console.WriteLine("OK");
        }

        static void XCopyThenDelete(string from, string to, params string[] subdirectoriesToDelete)
        {
            XCopy(from, to);
            foreach (var d in subdirectoriesToDelete)
            {
                var name = to + "\\" + d;
                if (Directory.Exists(name))
                    Directory.Delete(name, true);
            }
        }

        static void CopyProjectAndCorrectReferences(string path)
        {
            Console.Write("Copying " + path + "... ");
            var directory = new DirectoryInfo(path);
            var dest = Release + "\\" + directory.Name;
            if (Directory.Exists(dest))
                Directory.Delete(dest, true);
            Directory.CreateDirectory(dest);
            XCopyThenDelete(path, dest, "bin", "obj");

            var projFile = new DirectoryInfo(dest).GetFiles("*.csproj").Single();

            XDocument xmlDoc = XDocument.Load(projFile.FullName);


            var test = xmlDoc.Elements().Single().Elements().ToList();
            var test1 = test.Where(z => z.Name.LocalName == "ItemGroup").ToList();

            var projectReferencesItemGroup = xmlDoc.Elements().Single().Elements()
                .Where(z => z.Name.LocalName == "ItemGroup")
                .Where(z => z.Elements().All(x => x.Name.LocalName == "ProjectReference"))
                .FirstOrDefault();
            var referencesItemGroup = xmlDoc.Elements().Single().Elements()
                .Where(z => z.Name.LocalName == "ItemGroup")
                .Where(z => z.Elements().All(x => x.Name.LocalName == "Reference"))
                .FirstOrDefault();


            foreach (var project in projectReferencesItemGroup.Elements().ToList())
            {
                var name = project.Elements().Where(z => z.Name.LocalName == "Name").Single().Value;
                referencesItemGroup.Add(
                    new XElement("Reference",
                        new XAttribute("Include", name),
                        new XElement("HintPath", "..\\" + BinariesName + "\\Dlc\\Assemblies\\" + name + ".dll")));
                project.Remove();
            }

            xmlDoc.Save(projFile.FullName);
            var text = File.ReadAllText(projFile.FullName);
            text = text.Replace(" xmlns=\"\"", "");
            File.WriteAllText(projFile.FullName, text);
        }


        static void Main(string[] args)
        {
            Environment.CurrentDirectory = "..\\..\\..\\";
            Console.Write("Cleaning up folders... ");

            foreach (var file in Directory.GetFiles(".", "ucvarc*.zip"))
                File.Delete(file);

            if (Directory.Exists(Release)) Directory.Delete(Release, true);

            Directory.CreateDirectory(Release);
            Directory.CreateDirectory(BinariesPath);

            Console.WriteLine("Done");

            Console.Write("Starting unity builder... ");
            var unityProcess = Process.Start(new ProcessStartInfo
            {
                FileName = UnityEditorPath,
                Arguments = @"-batchmode -executeMethod BuildScript.PerformBuild -quit",
                WorkingDirectory = "uCvarc"
            });
            unityProcess.WaitForExit();
            Console.WriteLine("Done");


            Console.Write("Copying I18* libraries... ");
            foreach (var file in new DirectoryInfo(I18Source).GetFiles("i18*.dll"))
                File.Copy(file.FullName, BinariesPath + @"\ucvarc_Data\Managed\" + file.Name, true);
            Console.WriteLine("OK");

            XCopy(@"uCvarc\Dlc", BinariesPath + "\\Dlc");
            XCopy(@"Commons\SingleplayerProxy\bin\Debug", BinariesPath);
            XCopy(@"Commons\ReplayRunner\bin\Debug", BinariesPath);

            Console.Write("Deleting log.txt ... ");
            var logFileName = BinariesPath + "\\log.txt";
            if (File.Exists(logFileName))
                File.Delete(logFileName);
            Console.WriteLine("OK");


            CopyProjectAndCorrectReferences(@"Competitions\Pudge\PudgeClient");
            CopyProjectAndCorrectReferences(@"Competitions\homm\homm.client");

            Console.Write("Copying common files... ");
            File.Copy("Builder\\Start.bat", Release + "\\Start.bat", true);
            File.Copy("Builder\\RegisterCvarcReplay.bat", Release + "\\RegisterCvarcReplay.bat", true);
            File.Copy("Builder\\settings.json", BinariesPath + "\\settings.json", true);

            Console.WriteLine("OK");

            Console.Write("Zipping... ");

            var ucvarc_zip = string.Format("ucvarc-{0:yy-MM-dd-hh-mm}.zip", DateTime.Now);

            var zipProcess = Process.Start(new ProcessStartInfo
            {
                FileName = SevenZ,
                Arguments = @"a -tzip " + ucvarc_zip + " .\\Release\\"
            });
            zipProcess.WaitForExit();
            Console.WriteLine("OK");
        }
    }
}