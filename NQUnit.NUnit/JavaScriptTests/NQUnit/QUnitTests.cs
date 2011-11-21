using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NQUnit.NUnit.JavaScriptTests.NQUnit
{
    [TestFixture]
    public class QUnitTests
    {
        [Test, TestCaseSource("GetQUnitTests")]
        public void Test(QUnitTest test)
        {
            test.ShouldPass();
        }

        public IEnumerable<QUnitTest> GetQUnitTests()
        {
            var testsDirectory = Path.Combine(Environment.CurrentDirectory, "JavaScriptTests");
            var watinTestRunner = new QUnitParserFactory().CreateParser(500, "watin");
            return global::NQUnit.NQUnit.GetTests(watinTestRunner,Directory.GetFiles(testsDirectory, "*.html"));
        }
    }
}