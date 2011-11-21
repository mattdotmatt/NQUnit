using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NQUnit.Interfaces;

namespace NQUnit
{
    public class QUnitParserFactory
    {
        public IQUnitParser CreateParser(int msWait, string parserType)
        {
            switch (parserType){
                case "watin":
                    return new QUnitParser(msWait);
                case "htmlunit":
                    return new QUnitParserHtmlUnit(msWait,new HtmlUnitBrowser());
            }

            throw new NotSupportedException();
        }
    }
}
