using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace appcfg.compilers
{
    /*
     * The parser checks a list of argument tokens against the "syntax" of a route (defined by its params).
     */
    internal static class AppCfgCommandlineParser
    {
        public class Argument {
            public string Name;
            public object Value;
        }

        public class MultiValue {
            public string[] Values;
        }

        
        public static Argument[] Parse(AppCfgCommandlineScanner.Token[] tokens, Route route)
        {
            var argList = new List<Argument>();
            var tokenList = new List<AppCfgCommandlineScanner.Token>(tokens);
            
            foreach (var param in route.Params)
                Find_in_tokens(param, tokenList,
                    i => Build_argument_for_param_from_tokens(param, tokenList, i, argList));
            
            return argList.ToArray();
        }

        
        private static void Find_in_tokens(Parameter param, List<AppCfgCommandlineScanner.Token> tokenList, Action<int> onTokenFound) {
            for(var i=0; i<tokenList.Count; i++)
                if (Token_matches_param(tokenList[i])) {
                    onTokenFound(i);
                    return;
                }

            if (param.Name == "*")
                onTokenFound(0);

            
            bool Token_matches_param(AppCfgCommandlineScanner.Token token) {
                if (token.Type == AppCfgCommandlineScanner.TokenTypes.Value) return false;

                return string.Equals(token.Text, param.Name, StringComparison.InvariantCultureIgnoreCase) ||
                       param.Aliases.Any(a => string.Equals(token.Text, a, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        
        private static void Build_argument_for_param_from_tokens(Parameter param,
                                                                 IList<AppCfgCommandlineScanner.Token> tokens, 
                                                                 int paramIndex, 
                                                                 ICollection<Argument> arguments)
        {
            var arg = new Argument {Name = param.Name, Value = true};
            arguments.Add(arg);
            
            if (param.Name != "*")
            {
                // Single value?
                tokens.RemoveAt(paramIndex);

                if (param.ValueType == ValueTypes.None) return;
                if (paramIndex >= tokens.Count) throw new InvalidOperationException($"Missing value for parameter '{param.Name}'!");
                
                arg.Value = tokens[paramIndex].Text;
                tokens.RemoveAt(paramIndex);
            }
            else
            {
                // Catch all values!
                arg.Value = new MultiValue {Values = tokens.Select(t => t.Text).ToArray()};
                tokens.Clear();
            }

        }
    }
}