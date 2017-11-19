using System;
using System.Collections.Generic;
using System.Linq;
using appcfg;
using Microsoft.SqlServer.Server;

namespace exploration
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            foreach(var a in args)
                Console.WriteLine($"<{a}>");


            var cfg = new AppCfgSchema("my.cfg", // env var name prefix
                new Route("verb1", "verb1Alias", "MYROUTE_",
                        true) // is default, env var name prefix (added to app prefix)
                    .Param("add", "a", isRequired: true) // required
                    .Param("subtract", "s")
                    .Param("upload", "u", ValueTypes.String)
                    .Param("db", "database", ValueTypes.String, "CONFIG_STRING") // with env var name
                    .Param("*"),
                new Route("verb2")
                    .Param("b,below")
            ).Compile(new[] {"a", "b"});

        }
    }

    


}