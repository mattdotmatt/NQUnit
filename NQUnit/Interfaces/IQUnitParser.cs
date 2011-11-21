using System;
namespace NQUnit.Interfaces
{
    public interface IQUnitParser : IDisposable
    {
        System.Collections.Generic.IEnumerable<QUnitTest> GetQUnitTestResults(string testPage);
        string ParserType{get;}
    }
}
