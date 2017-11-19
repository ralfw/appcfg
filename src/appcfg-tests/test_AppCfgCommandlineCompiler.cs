using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgCommandlineCompiler
    {
        [Test]
        public void Compile()
        {
            var args = new[] {"route", "-bool", "--int:42", "/double", "3.14", "-datetime=12.5.17 10:53:17", "1", "2"};
            
            var route = new Route("route")
                            .Param("int", valueType: ValueTypes.Number)
                            .Param("datetime", valueType: ValueTypes.DateTime)
                            .Param("b", "bool")
                            .Param("double", valueType: ValueTypes.Number)
                            .Param("*", valueType: ValueTypes.Number);
            
            var sut = new AppCfgCommandlineCompiler(args);
            
            dynamic result = new ExpandoObject();
            sut.Compile(route, result);
            var cfg = (IDictionary<string, object>) result;

            Assert.AreEqual(5, cfg.Count);
        }
    }
}