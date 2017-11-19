using System;
using System.Collections.Generic;
using System.Linq;

namespace appcfg.compilers
{
    internal class AppCfgEnvironmentCompiler
    {
        public void Compile(Route route, IDictionary<string,object> cfg) {
            foreach (var p in route.Params) {
                if (Is_in_environment(p, out var envVarname))
                    cfg[p.Name] = Environment.GetEnvironmentVariable(envVarname);
            }

            
            bool Is_in_environment(Parameter candidateParam, out string envVarname) {
                envVarname = null;
                foreach (var envKey in Environment.GetEnvironmentVariables().Keys) {
                    if (candidateParam.EnvironmentVarNames.Any(e =>string.Equals(e, envKey.ToString(), StringComparison.InvariantCultureIgnoreCase))) {
                        envVarname = envKey.ToString();
                        return true;
                    }
                }
                return false;
            }
        }
    }
}