using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NQUnit;
using NQUnit.Interfaces;

namespace NQunit.Tests
{
    [TestFixture]
    public class ParserFactoryTests
    {
        [TestCase("watin"), RequiresSTA]
        [TestCase("htmlunit")]
        public void ParserFactoryShouldReturnTheCorrectParser(string type){
            // ARRANGE
            var sut = new NQUnit.QUnitParserFactory();

            // ACT
            IQUnitParser parser = sut.CreateParser(10,type);

            // ASSERT
            Assert.That(parser.ParserType, Is.EqualTo(type),"Parser should be for the correct type");

        }
    }
}
