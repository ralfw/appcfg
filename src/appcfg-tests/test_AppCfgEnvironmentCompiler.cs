using System;
using System.Collections.Generic;
using System.Dynamic;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgEnvironmentCompiler
    {
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("pOne", "1");
            Environment.SetEnvironmentVariable("Param2", "2");
            Environment.SetEnvironmentVariable("withDefault", "");
            Environment.SetEnvironmentVariable("PREFIX_PARAMNAME", "withprefix");
            Environment.SetEnvironmentVariable("NOPREFIXPARAMNAME", "noprefix");
        }
        
        
        [Test]
        public void Compile()
        {
            var route = new Route("")
                .Param("param1", "p1 pOne")
                .Param("param2");
            
            var schema = new AppCfgSchema(
                "",
                new Route[0]
            );
            var sut = new AppCfgEnvironmentCompiler();
            
            dynamic cfg = new ExpandoObject();
            sut.Compile(route, cfg);
            
            Assert.AreEqual("1", cfg.param1);
            Assert.AreEqual("2", cfg.param2);
        }
        
        
        [Test]
        public void Compile_skips_missing()
        {
            var route = new Route("")
                .Param("param1", "p1 pOne")
                .Param("missing");
            
            var schema = new AppCfgSchema(
                "",
                new Route[0]
            );
            var sut = new AppCfgEnvironmentCompiler();
            
            var cfg = new Dictionary<string, object>();
            sut.Compile(route, cfg);

            Assert.AreEqual(1, cfg.Count);
            Assert.AreEqual("1", cfg["param1"]);
        }
        
        
        [Test]
        public void Compile_uses_special_environment_varname_with_route_prefix()
        {
            var route = new Route("", environmentPrefix: "PREFIX_")
                            .Param("param1", environmentVarName: "PARAMNAME");
            
            var schema = new AppCfgSchema(
                "",
                new Route[0]
            );
            var sut = new AppCfgEnvironmentCompiler();
            
            var cfg = new Dictionary<string, object>();
            sut.Compile(route, cfg);

            Assert.AreEqual("withprefix", cfg["param1"]);
        }
        
        
        [Test]
        public void Compile_uses_special_environment_varname_without_route_prefix()
        {
            var route = new Route("")
                .Param("pOne", environmentVarName: "NOPREFIXPARAMNAME");
            
            var schema = new AppCfgSchema(
                "",
                new Route[0]
            );
            var sut = new AppCfgEnvironmentCompiler();
            
            var cfg = new Dictionary<string, object>();
            sut.Compile(route, cfg);

            Assert.AreEqual("noprefix", cfg["pOne"]);
        }
    }
}