using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace appcfg.compilers
{
    internal class AppCfgConfigfileCompiler
    {
        private readonly string _configFilepath;

        public AppCfgConfigfileCompiler(string configFilepath) { _configFilepath = configFilepath; }
        
        
        public void Compile(Route route, IDictionary<string,object> cfg)
        {
            var cfgFile = Load_config_from_file();
            var paramsFound = Compile_params(cfgFile.Keys);
            Copy_params(paramsFound);


            IEnumerable<(Parameter, string)> Compile_params(IEnumerable<string> configFileFieldnames) {
                foreach (var fieldname in configFileFieldnames) {
                    var param = route.Params.FirstOrDefault(p =>
                        string.Equals(p.Name, fieldname, StringComparison.InvariantCultureIgnoreCase) ||
                        p.Aliases.Any(a => string.Equals(a, fieldname, StringComparison.InvariantCultureIgnoreCase)));
                    if (param != null)
                        yield return (param, fieldname);
                }
            }

            void Copy_params(IEnumerable<(Parameter, string)> paramsToCopy) {
                foreach(var (destinationParam, sourceKey) in paramsToCopy)
                    cfg[destinationParam.Name] = cfgFile[sourceKey].ToString();
            }
        }

        
        private IDictionary<string,object> Load_config_from_file() {
            if (!File.Exists(_configFilepath)) return new Dictionary<string, object>();
            
            var json = new JavaScriptSerializer();
            var cfgText = File.ReadAllText(_configFilepath);
            return (IDictionary<string, object>) json.Deserialize<dynamic>(cfgText);
        }
    }
}