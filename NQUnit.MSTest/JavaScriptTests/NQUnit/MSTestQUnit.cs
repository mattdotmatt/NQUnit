using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NQUnit.NUnit.JavaScriptTests.NQUnit
{
    public static class MSTestNQUnitHelpers
    {
        public static void ShouldPass(this QUnitTest theTest)
        {
            if (theTest.InitializationException != null)
                throw new Exception("The QUnit initialization failed.", theTest.InitializationException);

            Assert.AreEqual("pass",theTest.Result, String.Format("Test: {0}", theTest.TestName + Environment.NewLine + theTest.Message));
        }
    }
}
