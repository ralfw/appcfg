using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgConfigfileCompiler
    {
        [Test]
        public void Compile()
        {
            var route = new Route("")
                            .Param("param1", " p1 ; pOne")
                            .Param("param2");
            
            var sut = new AppCfgConfigfileCompiler("config1.json");
            
            dynamic cfg = new ExpandoObject();
            sut.Compile(route, cfg);
            
            Assert.AreEqual("1", cfg.param1);
            Assert.AreEqual("2", cfg.param2);
        }
        
        
        [Test]
        public void Compile_with_missing_config_file()
        {
            var route = new Route("").Param("x");
            
            var sut = new AppCfgConfigfileCompiler("config missing");
            
            dynamic cfg = new ExpandoObject();
            sut.Compile(route, cfg);
            
            Assert.IsFalse(File.Exists("config missing"));
            Assert.AreEqual(0, ((IDictionary<string,object>)cfg).Count);
        }
        
        
        [Test]
        public void Compile_skips_missing_optional_param()
        {
            var route = new Route("")
                .Param("param1", " p1 ; pOne")
                .Param("missing");
            
            var sut = new AppCfgConfigfileCompiler("config1.json");
            
            IDictionary<string,object> cfg = new Dictionary<string, object>();
            sut.Compile(route, cfg);
            
            Assert.AreEqual(1,cfg.Count);
            Assert.AreEqual("1", cfg["param1"]);
        }
    }
}