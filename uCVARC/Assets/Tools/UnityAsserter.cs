using CVARC.V2;
using System;


class UnityAsserter : IAsserter
{
    private string testName;

    public UnityAsserter(string TestName)
    {
        testName = TestName;
    }

    public bool Failed { get; private set;  }

    public void IsEqual(bool expected, bool actual)
    {
        if (expected != actual)
        {
            Debugger.Log(DebuggerMessageType.UnityTest, testName+": Expected " + expected + ", but was " + actual);
            Failed = true;
        }
    }

    public void Fail(string str)
    {
        Debugger.Log(DebuggerMessageType.UnityTest, testName + " failed:" + str);
        Failed = true;
    }

    public void IsEqual(double expected, double actual, double delta)
    {
        var difference = Math.Abs(expected - actual);
        if (difference > delta)
        {
            Debugger.Log(DebuggerMessageType.UnityTest, testName+": Expected " + expected + "+- " + delta + ", but was " + actual);
            Failed = true;
        }
    }


    public void DebugOkMessage()
    {
        if (!Failed) Debugger.Log(DebuggerMessageType.UnityTest, testName + " PASSED");
    }
}