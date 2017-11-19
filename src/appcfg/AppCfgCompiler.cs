using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using appcfg.compilers;

namespace appcfg
{
    public static class AppCfgCompilerExt {
        public static dynamic Compile(this AppCfgSchema schema, string[] args) {
            var compiler = new AppCfgCompiler(schema);
            return compiler.Compile(args);
        }
    }
    
    
    public class AppCfgCompiler
    {
        private readonly AppCfgSchema _schema;

        public AppCfgCompiler(AppCfgSchema schema) {
            _schema = schema;
        }

        public dynamic Compile(string[] args)
        {
            var compEnv = new AppCfgEnvironmentCompiler();
            var compConfigfile = new AppCfgConfigfileCompiler(_schema.Filepath);
            var compCommandline = new AppCfgCommandlineCompiler(args);
            
            var cfg = new Dictionary<string, object>();
            
            var route = Determine_route(args);
            cfg["_RoutePath"] = route.Path;

            compEnv.Compile(route, cfg);
            compConfigfile.Compile(route, cfg);
            compCommandline.Compile(route, cfg);
            return Finalize(route, cfg);
        }

        
        private Route Determine_route(string[] args) {
            // empty command line?
            if (args.Length == 0) return Default_route();
            // no route, but flag?
            if (Is_param_flag(args[0])) return Default_route();

            // route explicitly specified?
            var route = _schema.Routes.Where(Match_route).FirstOrDefault();
            if (route != null) return route;

            throw new InvalidOperationException($"No route found matching first command line entry '{args[0]}'!");
            

            bool Is_param_flag(string text) {
                return text.StartsWith("-") || text.StartsWith("/");
            }
            
            Route Default_route() {
                var defaultRoute = _schema.DefaultRoute;
                if (defaultRoute != null) return defaultRoute;
                throw new InvalidOperationException("Missing default route to process empty command line!");
            }
            
            bool Match_route(Route routeCandidate) {
                var alternativePaths = routeCandidate.Aliases.Concat(new[] {routeCandidate.Path});
                return alternativePaths.Any(p => string.Equals(p,args[0],StringComparison.InvariantCultureIgnoreCase));
            }
        }


        private static dynamic Finalize(Route route, IDictionary<string,object> cfg)
        {
            Parse_values_to_type(route, cfg);
            Check_presence_of_required_params(route, cfg);
            Insert_default_values(route, cfg);
            Insert_missing_flags(route, cfg);
            Rename_catchall(cfg);
            return Make_dynamic(cfg);
        }

        private static void Parse_values_to_type(Route route, IDictionary<string, object> cfg) {
            foreach (var k in cfg.Keys.ToArray()) {
                var param = route.Params.FirstOrDefault(p => p.Name == k && p.ValueType != ValueTypes.None);
                if (param == null) continue;

                if (cfg[k] is AppCfgCommandlineParser.MultiValue) {
                    cfg[k] = ((AppCfgCommandlineParser.MultiValue) cfg[k]).Values.Select(v => Parse_value(param.ValueType, v));
                }
                else
                    cfg[k] = Parse_value(param.ValueType, (string) cfg[k]);
            }
        }
        
        private static void Check_presence_of_required_params(Route route, IDictionary<string, object> cfg) {
            var missingParamNames = route.Params.Where(p => p.IsRequired)
                                                .Where(p => !cfg.ContainsKey(p.Name))
                                                .Select(p => p.Name).ToArray();
            
            if (missingParamNames.Length > 0)
                throw new InvalidOperationException($"Missing required parameters: {string.Join(", ", missingParamNames)}");
        }
        
        private static void Insert_default_values(Route route, IDictionary<string, object> cfg) {
            var paramsWithDefaults = route.Params.Where(p => p.ValueType != ValueTypes.None && p.DefaultValue != null);
            var missingParams = paramsWithDefaults.Where(p => !cfg.ContainsKey(p.Name));
            foreach (var mp in missingParams)
                cfg[mp.Name] = mp.DefaultValue;
        }

        private static void Insert_missing_flags(Route route, IDictionary<string, object> cfg) {
            var optionalFlagParams = route.Params.Where(p => p.ValueType == ValueTypes.None);
            var missingParams = optionalFlagParams.Where(p => !cfg.ContainsKey(p.Name));
            foreach (var mp in missingParams)
                cfg[mp.Name] = false;
        }

        private static void Rename_catchall(IDictionary<string, object> cfg) {
            if (cfg.ContainsKey("*")) {
                cfg["_CatchAll"] = cfg["*"];
                cfg.Remove("*");
            }
        }

        private static dynamic Make_dynamic(IDictionary<string, object> cfg) {
            var dynCfg = new ExpandoObject();
            var dynCfgDict = (IDictionary<string, object>) dynCfg;
            foreach (var k in cfg.Keys)
                dynCfgDict[k] = cfg[k];
            return dynCfg;
        }

        
        private static object Parse_value(ValueTypes valueType, string value) {
            switch (valueType) {
                case ValueTypes.Bool:
                    if (bool.TryParse(value, out var b))
                        return b;
                    else if (int.TryParse(value, out var bi))
                        return bi != 0;
                    else
                        throw new InvalidOperationException($"Cannot parse '{value}' to boolean value.");
                case ValueTypes.Number:
                    if (int.TryParse(value, out var i))
                        return i;
                    else if (double.TryParse(value, out var d))
                        return d;
                    else
                        throw new InvalidOperationException($"Cannot parse '{value}' to numeric value.");
                case ValueTypes.DateTime:
                    if (DateTime.TryParse(value, out var dt))
                        return dt;
                    else
                        throw new InvalidOperationException($"Cannot parse '{value}' to date/time value.");
                default:
                    return value;
            }   
        }
    }
}