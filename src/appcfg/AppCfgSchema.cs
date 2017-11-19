using System;
using System.Collections.Generic;
using System.Linq;

namespace appcfg
{
    //Feature: global params spanning all routes
    //Feature: enum param value type only allows certain strings as values
    //    The default value could be set to an enum type. It defines which strings are allowed.
    //    The first value of the enum type then would be the default value.
    //Feature: filename/path and URL value types
    //Feature: JSON value type
    //Feature: Validate that param names are valid identifiers
    //Feature: A description for routes and params to be shown when help is requested
    public class AppCfgSchema
    {
        public string Filepath { get; }
        public Route[] Routes { get; }

        
        public AppCfgSchema(string filepath, params Route[] routes) {
            this.Filepath = filepath;
            this.Routes = routes;
            Validate_routes();
        }

        public Route DefaultRoute => this.Routes.FirstOrDefault(r => r.IsDefault);


        private void Validate_routes() {
            var routeSignatures = this.Routes.SelectMany(r => new[]{r.Path}.Concat(r.Aliases)).ToArray();
            var uniqueSignatures = routeSignatures.Distinct().ToArray();
            if (routeSignatures.Length > uniqueSignatures.Length)
                throw new InvalidOperationException($"Routes in config schema are not unique!");            
        }
    }

    
    public class Route
    {
        public string Path { get;}
        public string[] Aliases { get; }
        public bool IsDefault { get; }
        public string EnvironmentPrefix { get; }
        public Parameter[] Params => _params.ToArray();
        
        
        private readonly List<Parameter> _params = new List<Parameter>();

        
        public Route(string path, string aliases = "", string environmentPrefix = "", bool isDefault = false) {
            this.Path = path.Trim();
            this.Aliases = Parse_aliases(aliases);
            this.EnvironmentPrefix = environmentPrefix;
            this.IsDefault = isDefault;
        }
        
        public Route Param(string name,
                           string aliases="", 
                           ValueTypes valueType = ValueTypes.None,
                           string environmentVarName = "",
                           bool isRequired=false,
                           object defaultValue=null)
        {
            _params.Add(new Parameter(name, Parse_aliases(aliases), valueType, this.EnvironmentPrefix, environmentVarName, isRequired, defaultValue));
            Validate_params();
            return this;
        }

        private static string[] Parse_aliases(string text) {
            var aliases = text.Split(new[] {',', ';', ' '}, StringSplitOptions.RemoveEmptyEntries);
            return aliases.Select(a => a.Trim()).ToArray();
        }

        private void Validate_params() {
            var routeSignatures = _params.SelectMany(p => new[]{p.Name}.Concat(p.Aliases)).ToArray();
            var uniqueSignatures = routeSignatures.Distinct().ToArray();
            if (routeSignatures.Length > uniqueSignatures.Length)
                throw new InvalidOperationException($"Parameters in route '{this.Path}' are not unique!");

            var catchallParam = _params.FirstOrDefault(p => p.Name == "*");
            if (catchallParam != null && catchallParam != _params.Last())
                throw new InvalidOperationException($"Catch-all parameter '*' needs to be the last one.");
        }
    }

    
    public class Parameter
    {
        public string Name { get; }
        public string[] Aliases { get; }
        public ValueTypes ValueType { get; }

        public string[] EnvironmentVarNames {
            get {
                return this._environmentVarName != "" ? 
                            new[] {_environmentPrefix + _environmentVarName} : 
                            new[]{this.Name}.Concat(this.Aliases).Select(a => _environmentPrefix + a).ToArray();
            }
        }

        public bool IsRequired { get; }
        public object DefaultValue { get; }

        
        private readonly string _environmentPrefix;
        private readonly string _environmentVarName;
        
        
        internal Parameter(string name, 
                           string[] aliases, 
                           ValueTypes valueType, 
                           string environmentPrefix, 
                           string environmentVarName, 
                           bool isRequired,
                           object defaultValue)
        {
            this.Name = name.Trim();
            this.Aliases = aliases;
            this.ValueType = valueType;
            _environmentPrefix = environmentPrefix;
            _environmentVarName = environmentVarName;
            this.IsRequired = isRequired;
            this.DefaultValue = defaultValue;
            
            if (this.Name == "*" && this.ValueType == ValueTypes.None)
                throw new InvalidOperationException("Catch-all parameter '*' must specify a value type!");
        }
    }
        
    
    public enum ValueTypes
    {
        None,
        Bool,
        String,
        Number,
        DateTime
    }
}