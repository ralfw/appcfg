using System;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgCommandlineScanner
    {
        [Test]
        public void Scan_of_empty_args()
        {
            var route = new Route("r");
            var args = new string[0];
            Assert.AreEqual(0, AppCfgCommandlineScanner.Scan(args,route).Length);
        }
        
        [Test]
        public void Scan_flags_without_values()
        {
            var route = new Route("r");
            var args = new[] {"r", "-a", "--bc", "/d42"};
            var result = AppCfgCommandlineScanner.Scan(args, route);
            Assert.AreEqual(3,result.Length);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[0].Type);
            Assert.AreEqual("a", result[0].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[1].Type);
            Assert.AreEqual("bc", result[1].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[2].Type);
            Assert.AreEqual("d42", result[2].Text);
        }
        
        [Test]
        public void Scan_flags_with_values()
        {
            var route = new Route("r", "rAlias");
            var args = new[] {"rAlias", "-a:1", "--b=234"};
            var result = AppCfgCommandlineScanner.Scan(args, route);
            Assert.AreEqual(4,result.Length);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[0].Type);
            Assert.AreEqual("a", result[0].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Value, result[1].Type);
            Assert.AreEqual("1", result[1].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[2].Type);
            Assert.AreEqual("b", result[2].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Value, result[3].Type);
            Assert.AreEqual("234", result[3].Text);
        }
        
        [Test]
        public void Scan_flags_and_separate_values()
        {
            var route = new Route("r", isDefault:true);
            var args = new[] {"x", "-a", "yz", "--b"};
            var result = AppCfgCommandlineScanner.Scan(args, route);
            Assert.AreEqual(4,result.Length);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Value, result[0].Type);
            Assert.AreEqual("x", result[0].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[1].Type);
            Assert.AreEqual("a", result[1].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Value, result[2].Type);
            Assert.AreEqual("yz", result[2].Text);
            Assert.AreEqual(AppCfgCommandlineScanner.TokenTypes.Name, result[3].Type);
            Assert.AreEqual("b", result[3].Text);
        }
    }
}