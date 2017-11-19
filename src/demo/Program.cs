using System;
using System.Collections.Generic;
using appcfg;

namespace demo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var cfgSchema = new appcfg.AppCfgSchema("democonfig.json",
                new Route("add", "addition", isDefault:true)
                    .Param("a", "x",valueType:ValueTypes.Number,isRequired:true)
                    .Param("b", "y",valueType:ValueTypes.Number,isRequired:true),
                new Route("sum", "summation")
                    .Param("*",valueType:ValueTypes.Number),
                new Route("div", "division")
                    .Param("a", "x,divident",valueType:ValueTypes.Number,isRequired:true)
                    .Param("b", "y,divisor",valueType:ValueTypes.Number,isRequired:true)
                    .Param("round", "r",valueType:ValueTypes.Bool)
                    .Param("suppressDivBy0",valueType:ValueTypes.Bool,defaultValue:false,environmentVarName:"DEMO_SUPPRESSDIVBY0")
                );
            
            var cfgcomp = new AppCfgCompiler(cfgSchema);
            
            var cfg = cfgcomp.Compile(args);

            switch (cfg._RoutePath) {
                case "add":
                    var addResult = cfg.a + cfg.b;
                    Console.WriteLine($"{cfg.a} + {cfg.b} = {addResult}");
                    break;
                    
                case "sum":
                    var sumResult = 0;
                    foreach (var x in cfg._CatchAll)
                        sumResult += x;
                    Console.WriteLine($"Sum of all params: {sumResult}");
                    break;
                    
                case "div":
                    double divResult = 0;
                    if (cfg.suppressDivBy0 && cfg.b == 0)
                        divResult = double.MaxValue;
                    else
                        divResult = cfg.a / cfg.b;
            
                    if (cfg.round)
                        divResult = Math.Round(divResult);
                    
                    Console.WriteLine($"{cfg.a} / {cfg.b} = {divResult}");
                    break;
            }
        }
    }
}