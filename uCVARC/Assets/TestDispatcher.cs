using System;
using System.Collections.Generic;
using System.Threading;
using CVARC.V2;

namespace Assets
{
    public static class TestDispatcher
    {
        public static readonly Dictionary<string, bool> LastTestExecution = new Dictionary<string, bool>();

        public static void RunOneTest(LoadingData data, string testName)
        {
            Debugger.Log(DebuggerMessageType.UnityTest, "Starting test " + testName);
            var test = Dispatcher.Loader.GetTest(data, testName);
            var asserter = new UnityAsserter(testName);

            Action testAction = () => ExecuteTest(testName, test, asserter, MakeServerInfo(data));
            testAction.BeginInvoke(null, null);
        }

        public static void RunAllTests(LoadingData data)
        {
            var competitions = Dispatcher.Loader.GetCompetitions(data);
            var testsNames = competitions.Logic.Tests.Keys;

            Action testAction = () => ExecuteTests(testsNames, data);
            testAction.BeginInvoke(null, null);
        }

        static void ExecuteTests(IEnumerable<string> testNames, LoadingData data)
        {
            Debugger.Log(DebuggerMessageType.UnityTest, "staring tests");
            foreach (var testName in testNames)
            {
                if (Dispatcher.UnityShutdown)
                {
                    Debugger.Log(DebuggerMessageType.UnityTest, "unity shutdown! stop testing");
                    break;
                }
                var asserter = new UnityAsserter(testName);
                Debugger.Log(DebuggerMessageType.UnityTest, "Test is ready");
                var test = Dispatcher.Loader.GetTest(data, testName);
                ExecuteTest(testName, test, asserter, MakeServerInfo(data));
            }
        }

        static void ExecuteTest(string testName, ICvarcTest test, UnityAsserter asserter, NetworkData networkInfo)
        {
            try
            {
                test.Run(networkInfo, asserter);
                asserter.DebugOkMessage();
            }
            catch (Exception e)
            {
                asserter.Fail(e.GetType().Name + " " + e.Message);
            }
            lock (LastTestExecution)
            {
                LastTestExecution[testName] = !asserter.Failed;
            }
            Dispatcher.SetGameOver();
        }

        static void WaitWorld(NetworkData data)
        {
            var myName = data.LoadingData.AssemblyName + data.LoadingData.Level;
            while (Dispatcher.CurrentRunner == null ||
                   Dispatcher.CurrentRunner.Name != myName ||
                   Dispatcher.CurrentRunner.World == null)
                Thread.Sleep(5);
            data.World = Dispatcher.CurrentRunner.World;

        }

        static NetworkData MakeServerInfo(LoadingData data)
        {
            var networkInfo = new NetworkData
            {
                Port = UnityConstants.SoloNetworkPort,
                LoadingData = data,
                WaitWorld = WaitWorld
            };
            return networkInfo;
        }
    }
}
