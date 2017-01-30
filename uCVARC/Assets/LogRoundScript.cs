using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;
using System.Linq;
using System.Reflection;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.Core;
using CVARC.V2;
using Pudge;
using UnityCommons;
using ToLogAttribute = Assets.ToLogAttribute;

public static class C
{
    public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this T1[] e1, T2[] e2)
    {
        if (e1.Length != e2.Length) throw new ArgumentException();
        for (int i = 0; i < e1.Length; i++)
            yield return Tuple.Create(e1[i], e2[i]);
    }
}

public class LogRoundScript : PlayScript
{

    int currentLogIndex;
    NewLog log;
    List<IEngine> engines;
    Dictionary<string, Action<string[]>> commandsDict;

    private CommonEngine GetCommonEngine()
    {
        return (CommonEngine)engines.Where(e => e is ICommonEngine).First();
    }

    //private PudgeWorldEngine GetPudgeWorldEngine()
    //{
    //    return (PudgeWorldEngine)engines.Where(e => e is IPudgeWorldEngine).First();
    //}

    // Use this for initialization
    protected override void Initialization()
    {
        log = Dispatcher.LoadedLog;
        currentLogIndex = 0;
        engines = new List<IEngine>();
        engines.Add(new CommonEngine());
        //engines.Add(new PudgeWorldEngine((ICommonEngine)engines[0]));

        commandsDict = new Dictionary<string, Action<string[]>>();
        foreach (var engine in engines)
            foreach (var methodInfo in engine.GetType().GetMethods().Where(x => x.GetCustomAttributes(typeof(ToLogAttribute), false).Any()))
            {
                var m = methodInfo;
                List<Func<string, object>> parsers = new List<Func<string, object>>();
                foreach (var param in m.GetParameters())
                {
                    parsers.Add(CreateParser(param.ParameterType));
                }
                var p = parsers.ToArray();


                commandsDict.Add(methodInfo.Name, args => m.Invoke(engine, p.Zip(args).Select(z => z.Item1(z.Item2)).ToArray()));
            }

        //init commands on 0-stage
        PlayCommandFromLog();
    }

    public Func<string, object> CreateParser(Type type)
    {
        if (type == typeof(string)) return z => z;

        if (type.IsEnum)
        {
            return z => Enum.Parse(type, z);
        }

        if (type == typeof(Frame3D))
            return z => Frame3D.Parse(z);

        var method =
            type.GetMethods()
                .Where(z => z.Name == "Parse" && z.IsStatic && z.GetParameters().Length == 1)
                .FirstOrDefault();
        if (method == null) throw new ArgumentException(type.FullName);
        return z => method.Invoke(null, new object[] { z });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            PlayCommandFromLog();
            GetCommonEngine().UpdateSpeeds();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Dispatcher.LogEnd();
        }

    }

    void PlayCommandFromLog()
    {
        if (currentLogIndex >= log.Segments.Count)
            return;

        var currentSegment = log.Segments[currentLogIndex];
        if (currentSegment.Positions != null)
            foreach (var pairs in currentSegment.Positions)
                GetCommonEngine().SetAbsoluteLocation(pairs.Item1, pairs.Item2);
        if (currentSegment.Scores != null &&
            !string.IsNullOrEmpty(currentSegment.Scores.leftScores) &&
            !string.IsNullOrEmpty(currentSegment.Scores.rightScores))
        {
            scoresTextLeft.text = "Left Scores: " + currentSegment.Scores.leftScores;
            scoresTextRight.text = "Right Scores: " + currentSegment.Scores.rightScores;
        }

        var commands = log.Segments[currentLogIndex].Commands;

        foreach (var command in commands)
            commandsDict[command.CommandName](command.CommandArgs);

        currentLogIndex++;

        if (currentLogIndex >= log.Segments.Count)
            Dispatcher.LogEnd();
    }
}
