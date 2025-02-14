﻿using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CVARC.V2
{
    public class Competitions
    {
        public readonly LogicPart Logic;
        public readonly Func<IKeyboard> KeyboardFactory;
        public readonly Func<GameSettings, List<IEngine>> EnginesFactory;
        public readonly string LevelName;
        public readonly string CompetitionsName;

        public Competitions(string competitionName, string levelName, LogicPartHelper logicPartHelper, Func<IKeyboard> keyboardFactory, Func<GameSettings, List<IEngine>> enginesFactory)
        {
            CompetitionsName = competitionName;
            LevelName = levelName;
            Logic = logicPartHelper.Create();
            EnginesFactory = enginesFactory;
            KeyboardFactory = keyboardFactory;
        }


        //public IWorld Create(Configuration arguments, IRunMode environment)
        //{
        //    environment.InitializeCompetitions(this);
        //    Logic.World.Initialize(this, environment);
        //    return Logic.World;
        //}

        //public static IWorld Create(string[] commandLineArguments)
        //{
        //    var arguments = Configuration.Analyze(commandLineArguments);
        //    var environment = RunModeFactory.Create(arguments.Mode);
        //    environment.CheckArguments(arguments);

        //    var assemblyName = arguments.Assembly + ".dll";

        //    if (!File.Exists(assemblyName))
        //    {
        //        throw new Exception(string.Format("The competitions assembly {0} was not found", assemblyName));
        //    }

        //    var ass = Assembly.LoadFrom(assemblyName);
        //    var list = ass.GetExportedTypes().ToList();
        //    var competitionsClass = list
        //        .SingleOrDefault(a => a.IsSubclassOf(typeof(Competitions)) && a.Name == arguments.Level);
        //    if (competitionsClass == null)
        //        throw new Exception(string.Format("The level {0} was not found in {1}", arguments.Level, arguments.Assembly));
        //    var ctor = competitionsClass.GetConstructor(new Type[] { });
        //    var competitions = ctor.Invoke(new object[] { }) as Competitions;

            
            
        //    return competitions.Create(arguments, environment);
        //}
    }
}