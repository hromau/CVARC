using AIRLab.Mathematics;

namespace CVARC.V2
{
    //abstract class AbstractLocationAsserter<TSensorData> : ISensorAsserter<TSensorData>
    //{
    //    public Frame2D ExpextedLocation { get; set; }
    //    public double Delta { get; set; }
        
    //    protected AbstractLocationAsserter(Frame2D expectedLocation, double delta)
    //    {
    //        ExpextedLocation = expectedLocation;
    //        Delta = delta;
    //    } 
        
    //    public abstract Frame2D ExtractLocationData(TSensorData data);

    //    public void Assert(TSensorData data, IAsserter asserter)
    //    {
    //        var actualLocation = ExtractLocationData(data);
    //        asserter.IsEqual(ExpextedLocation.X, actualLocation.X, Delta);
    //        asserter.IsEqual(ExpextedLocation.Y, actualLocation.Y, Delta);            
    //        asserter.IsEqual(ExpextedLocation.Angle.Grad, actualLocation.Angle.Grad, Delta);
    //    }
    //}
}
