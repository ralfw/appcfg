using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace appcfg.compilers
{
    /*
     * Transforms a list of command line args into a stream of tokens, e.g.
     *     source: "routename -a:123 /b 987 3.14"
     *     tokens:
     *         1. type: name, text: "a"
     *         2. type: value, text: 123
     *         3. type: name, text: "b"
     *         4. type: value, text: 987
     *         5. type: value, text: 3.14
     *
     * Tokenization does not care about the validity of name/value combinations or values w/o names.
     * A route path at the start of args is skipped (if present at all).
     */
    internal static class AppCfgCommandlineScanner
    {
        public enum TokenTypes {
            Name,
            Value
        }
        public class Token {
            public TokenTypes Type;
            public string Text;
        }
        
        
        public static Token[] Scan(IEnumerable<string> args, Route route) {
            args = Skip_route_path(args, route);
            return Parse_args(args).ToArray();
        }
        
        
        private static IEnumerable<string> Skip_route_path(IEnumerable<string> args, Route route)
        {
            if (!args.Any()) return args;
            if (string.Equals(args.First(), route.Path, StringComparison.InvariantCultureIgnoreCase) ||
                route.Aliases.Any(a => string.Equals(args.First(), a, StringComparison.InvariantCultureIgnoreCase)))
                return args.Skip(1);
            return args;
        }


        private static IEnumerable<Token> Parse_args(IEnumerable<string> args) {
            foreach(var arg in args) {
                if (Try_parse_arg(arg, out var name, out var value)) {
                    yield return new Token {Type = TokenTypes.Name, Text = name};
                    if (value != null) yield return new Token {Type = TokenTypes.Value, Text = value};
                }
                else
                    yield return new Token{Type = TokenTypes.Value, Text = arg};
            }
        }
        
        private static bool Try_parse_arg(string text, out string name, out string value) {
            name = "";
            value = null;
                
            // hone regex at http://regexstorm.net/tester
            const string REGEX_PATTERN = @"^(-|--|/)(?<name>(\w|\d)+)((:|=)(?<value>.+))*";
            var m = Regex.Match(text, REGEX_PATTERN, RegexOptions.IgnoreCase);
            if (!m.Success) return false;
                
            name = m.Groups["name"].Value;
            if (m.Groups["value"].Success) value = m.Groups["value"].Value;
            return true;
        }
    }
}