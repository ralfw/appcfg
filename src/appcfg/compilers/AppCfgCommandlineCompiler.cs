using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace appcfg.compilers
{
    /*
     * CommandLine ::= [routePath] { FlagParam } [ CatchAllParam ]
     * FlagParam ::= Flag [ Value ]
     * Flag ::= ("-" | "--" | "/") flagName [":" | "="]
     * CatchAllParam ::= Value { Value }
     */
    internal class AppCfgCommandlineCompiler
    {
        private readonly string[] _args;

        public AppCfgCommandlineCompiler(string[] args) { _args = args; }

        
        public void Compile(Route route, IDictionary<string,object> cfg) {
            if (_args.Length == 0) return;

            var tokens = AppCfgCommandlineScanner.Scan(_args, route);
            var arguments = AppCfgCommandlineParser.Parse(tokens, route);
            Attach(cfg, arguments);
        }


        private static void Attach(IDictionary<string, object> cfg, IEnumerable<AppCfgCommandlineParser.Argument> arguments) {
            foreach (var a in arguments)
                cfg[a.Name] = a.Value;
        }
    }
}