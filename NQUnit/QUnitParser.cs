﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using WatiN.Core;
using WatiN.Core.Native.Windows;
using NQUnit.Interfaces;

namespace NQUnit
{
    /// <summary>
    /// The class that takes care of firing an IE session using WatiN and parsing the DOM of the page to extract the QUnit information.
    /// </summary>
    public class QUnitParser : IQUnitParser
    {
        private readonly int _maxWaitInMs;
        private readonly IE _ie;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="maxWaitInMs">The maximum number of milliseconds before the tests should timeout after page load; -1 for infinity, 0 to not support asynchronous tests</param>
        public QUnitParser(int maxWaitInMs)
        {
            _maxWaitInMs = maxWaitInMs < 0 ? Int32.MaxValue : maxWaitInMs;
            _ie = new IE();
            if (NQUnit.HideBrowserWindow)
            {
                _ie.ShowWindow(NativeMethods.WindowShowStyle.Hide);
            }
            if (NQUnit.ClearCacheBeforeRunningTests)
            {
                _ie.ClearCache();
            }
        }

        /// <summary>
        /// Returns an array of QUnitTest objects given a test page URI.
        /// </summary>
        /// <param name="testPage">The URI of the test page; either a URL or a file path</param>
        /// <returns>An array of QUnitTest objects</returns>
        public IEnumerable<QUnitTest> GetQUnitTestResults(string testPage)
        {
            _ie.GoTo(testPage);
            _ie.WaitForComplete(5);
            return GrabTestResultsFromWebPage(testPage);
        }

        private IEnumerable<QUnitTest> GrabTestResultsFromWebPage(string testPage)
        {
            var stillRunning = true;
            var testOl = default(Element);
            var documentRoot = default(XElement);
            var wait = 0;

            // BEWARE: This logic is tightly coupled to the structure of the HTML generated by the QUnit test runner
            // Also, this could probably be greatly simplified with a couple well-crafted XPath expressions

            while (stillRunning && wait <= _maxWaitInMs)
            {
                var elementCollection = _ie.Elements.Filter(Find.ById("qunit-tests"));
                if (elementCollection.Count != 0)
                {
                    testOl = elementCollection[0];
                    if (testOl == null) yield break;
                    documentRoot = XDocument.Load(new StringReader(MakeXHtml(testOl.OuterHtml))).Root;
                    if (documentRoot == null) yield break;

                    stillRunning = documentRoot.Elements().Any(e => e.Attributes().First(x => x.Name.Is("class")).Value == "running");
                }
                if (stillRunning && wait < _maxWaitInMs) Thread.Sleep(wait + 100 > _maxWaitInMs ? _maxWaitInMs - wait : 100);
                wait += 100;
            }

            foreach (var listItem in documentRoot.Elements())
            {
                var testName = listItem.Elements().First(x => x.Name.Is("strong")).Value;
                var resultClass = listItem.Attributes().First(x => x.Name.Is("class")).Value;
                var failedAssert = String.Empty;
                if (resultClass == "fail")
                {
                    var specificAssertFailureListItem = listItem.Elements()
                        .First(x => x.Name.Is("ol")).Elements()
                        .First(x => x.Name.Is("li") && x.Attributes().First(a => a.Name.Is("class")).Value == "fail");
                    if (specificAssertFailureListItem != null)
                    {
                        failedAssert = specificAssertFailureListItem.Value;
                    }
                }

                yield return new QUnitTest
                {
                    FileName = testPage,
                    TestName = RemoveAssertCounts(testName),
                    Result = resultClass,
                    Message = failedAssert
                };
            }

        }

        private static string MakeXHtml(string html)
        {
            var replacer = new Regex(@"<([^ >]+)(.*?)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var innerReplacer = new Regex(@"(\s+.+?=)([^\s$]+)", RegexOptions.IgnoreCase);
            var h = replacer.Replace(html, match =>
                "<" + match.Groups[1] + innerReplacer.Replace(match.Groups[2].Value, innerMatch =>
                    innerMatch.Groups[2].Value.Contains("\"") ? innerMatch.Groups[1].Value + innerMatch.Groups[2].Value : innerMatch.Groups[1].Value + "\"" + innerMatch.Groups[2].Value + "\""
                ) + ">"
            );
            return h;
        }


        private static string RemoveAssertCounts(string fullTagText)
        {
            if (fullTagText == null) return String.Empty;
            int parenPosition = fullTagText.IndexOf('(');
            if (parenPosition > 0)
            {
                return fullTagText.Substring(0, parenPosition);
            }
            return fullTagText;
        }

        /// <summary>
        /// Closes the IE instance.
        /// </summary>
        public void Dispose()
        {
            _ie.Close();
        }

        #region IQUnitParser Members


        public string ParserType
        {
            get { return "watin"; }
        }

        #endregion
    }
}
