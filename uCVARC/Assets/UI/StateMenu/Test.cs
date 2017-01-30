//using Newtonsoft.Json;
using Assets;

public class Test
{
    public string NameTest;
    public int State;

    //[JsonConstructor]
    public Test(string NameTest, int State)
    {
        this.NameTest = NameTest;
        this.State = State;
        if (TestDispatcher.LastTestExecution.ContainsKey(NameTest))
            TestDispatcher.LastTestExecution[NameTest] = State != 1;
    }

    public Test()
    {
        NameTest = "";
        State = -1;
    }
}
