using System;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgCommandlineParser
    {        
        [Test]
        public void Parse_without_values()
        {
            var args = new[] {
                  new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "p1" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "p2Alias" }
            };
            var route = new Route("").Param("p1").Param("p2", "p2Alias");

            var result = AppCfgCommandlineParser.Parse(args, route);
            
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("p1", result[0].Name);
            Assert.AreEqual("p2", result[1].Name);
        }
        
        [Test]
        public void Parse_with_values()
        {
            var args = new[] {
                new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "p1" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Value, Text = "v1" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "p2" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Value, Text = "v2" }
            };
            var route = new Route("").Param("p1", valueType:ValueTypes.String).Param("p2", "p2Alias", valueType:ValueTypes.String);

            var result = AppCfgCommandlineParser.Parse(args, route);
            
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("p1", result[0].Name);
            Assert.AreEqual("v1", result[0].Value);
            Assert.AreEqual("p2", result[1].Name);
            Assert.AreEqual("v2", result[1].Value);
        }
        
        
        [Test]
        public void Parse_with_missing_value()
        {
            var args = new[] {
                new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "int" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Value, Text = "42" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "double" }
            };
            var route = new Route("")
                .Param("int", valueType: ValueTypes.Number)
                .Param("double", valueType: ValueTypes.Number);

            Assert.Throws<InvalidOperationException>(() => AppCfgCommandlineParser.Parse(args, route));
        }
        
        
        [Test]
        public void Parse_with_catchall()
        {
            var args = new[] {
                new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "int" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Value, Text = "42" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "a" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "1" }
                , new AppCfgCommandlineScanner.Token { Type = AppCfgCommandlineScanner.TokenTypes.Name, Text = "2" }
            };
            var route = new Route("")
                .Param("int", valueType: ValueTypes.Number)
                .Param("a")
                .Param("*", valueType: ValueTypes.Number);
            
            
            var result = AppCfgCommandlineParser.Parse(args, route);
            
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("42", result[0].Value);
            Assert.AreEqual("a", result[1].Name);
            Assert.AreEqual("*", result[2].Name);
            Assert.AreEqual(new[]{"1","2"}, (result[2].Value as AppCfgCommandlineParser.MultiValue).Values);
        }
    }
}