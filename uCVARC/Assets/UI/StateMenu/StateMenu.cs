using UnityEngine;
using System.Collections;
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using CVARC.V2;
using System;

public class StateMenu {
    public bool IsOpenTestsWindow { get; set; }
    public Dictionary<string, Dictionary<string, Folder>> StateFolder;
    public string CurrentAssembly;
    public string CurrentLevel;

    //[JsonConstructor]
    public StateMenu(string currentAssembly, string currentLevel, Dictionary<string, Dictionary<string, Folder>> stateFolder, bool isOpenTestsWindow)
    {
        CurrentAssembly = currentAssembly;
        CurrentLevel = currentLevel;
        StateFolder = stateFolder;
        IsOpenTestsWindow = isOpenTestsWindow;
    }

    public StateMenu(Dictionary<string, Dictionary<string, Func<Competitions>>> assembly)
    {
        IsOpenTestsWindow = false;
        StateFolder = new Dictionary<string, Dictionary<string, Folder>>();
        foreach (var e in assembly.Keys)
        {
            StateFolder.Add(e, new Dictionary<string, Folder>());
            StateFolder[e] = new Dictionary<string, Folder>();
            foreach (var i in assembly[e].Keys)
                StateFolder[e].Add(i, Folder.MakeFolder(e, assembly[e][i]().Logic.Tests.Keys.OrderBy(x => x).ToArray()));
        }
        
        CurrentAssembly = assembly.Keys.First();
        CurrentLevel = assembly[CurrentAssembly].Keys.First();
    }

    public void CheckContaints(Dictionary<string, Dictionary<string, Func<Competitions>>> assembly)
    {
        foreach (var e in assembly.Keys)
        {
            if (!StateFolder.ContainsKey(e))
            {
                StateFolder.Add(e, new Dictionary<string, Folder>());
                foreach (var i in assembly[e].Keys)
                    StateFolder[e].Add(i, Folder.MakeFolder(e, assembly[e][i]().Logic.Tests.Keys.OrderBy(x => x).ToArray()));
            }
            else
            {
                foreach (var i in assembly[e].Keys)
                    if (!StateFolder[e].ContainsKey(i))
                        StateFolder[e].Add(i, Folder.MakeFolder(e, assembly[e][i]().Logic.Tests.Keys.OrderBy(x => x).ToArray()));
            }
        }
    }
    public bool ContainsAssembly(string name)
    {
        return StateFolder.ContainsKey(name);
    }
}
