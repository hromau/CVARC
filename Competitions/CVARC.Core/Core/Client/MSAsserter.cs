using System;

namespace CVARC.V2
{
    public class MSAsserter : IAsserter
    {
        // здесь была использована библиотека:
        // <Reference Include="Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL" />
        public void IsEqual(double expected, double actual, double delta)
        {
            // Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual, delta);
            if (Math.Abs(expected - actual) > delta)
                throw new Exception("Assert failed");
        }
        public void IsEqual(bool expected, bool actual)
        {
            // Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
            if (expected ^ actual)
                throw new Exception("Assert failed");
        }
    }
}
