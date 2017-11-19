using System;
using System.Collections.Generic;
using appcfg;
using appcfg.compilers;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgCompiler
    {
        [Test]
        public void Compile_from_all_sources()
        {
            Environment.SetEnvironmentVariable("fromEnv", "42");
            Environment.SetEnvironmentVariable("cfgfileOverridesEnv", "env2");
            Environment.SetEnvironmentVariable("cmdlineOverridesEnv", "env3");
            
            var args = new[] { "route2", 
                               "-fromCmdline:cmdline1", 
                               "/cmdlineOverridesCfgfile=cmdline2", 
                               "-cmdlineOverridesEnv", "cmdline3",
                               "-onCmdline",
                               "cl*1", "cl*2"};
            var schema = new AppCfgSchema("config2.json", 
                new Route("route1", "")
                    .Param("x")
                    .Param("y"),
                new Route("route2")
                    .Param("onCmdline")
                    .Param("missingFromCmdline")
                    .Param("missingWithDefault",valueType:ValueTypes.Number,defaultValue:42)
                    .Param("fromEnv",valueType:ValueTypes.Number)
                    .Param("fromCfgfile",valueType:ValueTypes.DateTime)
                    .Param("fromCmdline",valueType:ValueTypes.String)
                    .Param("cfgfileOverridesEnv",valueType:ValueTypes.String)
                    .Param("cmdlineOverridesCfgfile",valueType:ValueTypes.String)
                    .Param("cmdlineOverridesEnv",valueType:ValueTypes.String)
                    .Param("*",valueType:ValueTypes.String)
            );
            var sut = new AppCfgCompiler(schema);
            
            var cfg = sut.Compile(args);
            
            Assert.AreEqual("route2", cfg._RoutePath);
            Assert.AreEqual(true, cfg.onCmdline);
            Assert.AreEqual(false, cfg.missingFromCmdline);
            Assert.AreEqual(42, cfg.missingWithDefault);
            Assert.AreEqual(42, cfg.fromEnv);
            Assert.AreEqual(new DateTime(2017,5,12,10,53,17), cfg.fromCfgfile);
            Assert.AreEqual("cmdline1", cfg.fromCmdline);
            Assert.AreEqual("cfgfile2", cfg.cfgfileOverridesEnv);
            Assert.AreEqual("cmdline2", cfg.cmdlineOverridesCfgfile);
            Assert.AreEqual("cmdline3", cfg.cmdlineOverridesEnv);
            Assert.AreEqual(new[]{"cl*1", "cl*2"}, cfg._CatchAll);
        }
    }
}