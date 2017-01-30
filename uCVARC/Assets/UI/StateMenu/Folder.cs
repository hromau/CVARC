using System.Collections.Generic;
//using Newtonsoft.Json;

public class Folder
{
    public string Name;
    public bool IsOpen;
    public List<Folder> Files;
    public List<Test> Tests;

    //[JsonConstructor]
    public Folder(string name, bool isOpen, List<Folder> files, List<Test> tests)
    {
        this.Name = name;
        this.IsOpen = isOpen;
        this.Files = files;
        this.Tests = tests;
    }

    public Folder(string name)
    {
        Name = name;
        IsOpen = false;
        Files = new List<Folder>();
        Tests = new List<Test>();
    }

    public void Show(bool ParentIsOpen)
    {
        if (ParentIsOpen)
        {
            foreach (var test in Files)
            {
                if (test is Folder)
                    ((Folder)test).Show(ParentIsOpen && IsOpen);
                else
                {

                }
            }
        }
    }

    public static Folder MakeFolder(string nameHead, string[] tests)
    {
        var folder = new Folder(nameHead);
        foreach (var test in tests)
        {
            var names = test.Split('_');
            Folder last = folder;
            for (int i = 0; i < names.Length - 1; i++)
            {
                var f = last.Contains(names[i]);
                if (f == null)
                {
                    var newFolder = new Folder(names[i]);
                    last.Files.Add(newFolder);
                    last = newFolder;
                }
                else
                    last = (Folder)f;
            }
            last.Tests.Add(new Test(test, 0));
        }
        return folder;

    }

    public IEnumerable<object> GetChildren()
    {
        foreach (var e in Files)
            yield return e;
        foreach (var e in Tests)
            yield return e;
    }
    public IEnumerable<Test> GetChildrenTests()
    {
        foreach (var e in Tests)
            yield return e;
        foreach (var e in Files)
            foreach (var k in e.GetChildrenTests())
                yield return k;

    }

    public object Contains(string name)
    {
        foreach (var e in Files)
        {
            if (e is Folder)
                if (name == ((Folder)e).Name)
                    return e;
        }
        return null;
    }
}