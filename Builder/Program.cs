using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Builder
{
    class Program
    {
        const string UnityEditorPath = @"C:\Program Files\Unity\Editor\Unity.exe";
        const string SevenZ = @"C:\Program Files\7-Zip\7z.exe";

        const string ucvarc_zip = "ucvarc.zip";
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

        static void DeleteAndXCopy(string from, string to, params string[] subdirectoriesToDelete)
        {
            foreach (var d in subdirectoriesToDelete)
            {
                var name = from + "\\" + d;
                if (Directory.Exists(name))
                    Directory.Delete(name,true);
            }
            XCopy(from, to);
        }

        static void CopyProjectAndCorrectReferences(string path)
        {
            Console.Write("Copying " + path+"... ");
            var directory = new DirectoryInfo(path);
            var dest = Release + "\\" + directory.Name;
            if (Directory.Exists(dest))
                Directory.Delete(dest, true);
            Directory.CreateDirectory(dest);
            DeleteAndXCopy(path, dest, "bin", "obj");

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
            if (File.Exists(ucvarc_zip)) File.Delete(ucvarc_zip);
            if (Directory.Exists(BinariesPath)) Directory.Delete(BinariesPath, true);
            Directory.CreateDirectory("Release");
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


            XCopy(@"uCvarc\Dlc", BinariesPath + "\\Dlc");
            XCopy(@"Commons\SingleplayerProxy\bin\Debug", BinariesPath);
            

            CopyProjectAndCorrectReferences(@"Competitions\Pudge\PudgeClient");

            Console.Write("Copying common files... ");
            File.Copy("Builder\\Start.bat", Release + "\\Start.bat", true);
            File.Copy("Builder\\settings.json", BinariesPath+"\\settings.json", true);

            Console.WriteLine("OK");

            Console.Write("Zipping... ");
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
