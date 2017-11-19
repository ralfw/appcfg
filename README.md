# appcfg - .NET Application Configuration Made Easy
With *appcfg* you get one source for all you configuration data stored in environment variables, defined in a configuration file, or passed in on the command line.

> Also available as NuGet package [appcfg](https://www.nuget.org/packages/appcfg/).

## Overview Example
> You want to write a program to do simple math calculations. It should add two numbers, sum up a list of numbers, or divide two numbers. Upon division you want to switch between whether division by zero should throw an error or lead to a very large number. Also you want to specify if division results should be rounded.

### Using Configuration Parameters
This is how the business logic part of your program would look with *appcfg*. `cfg` contains all the parameters to the operations of the program.

```
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
```

The point of *appcfg* is that you don't have to care were those parameters got specified or how they were passed to the program.

#### Routes and Named Parameters
Here's one way to run the program (see demo project in `/src`):

`demo.exe add -a:2 -b:3`

`add` is called the *route path* to tell the program what to do. Think of it as a command or query. But it's called a *route path* to allude to web applications where parts of URLs are called routes, e.g. `/api/products/{id}`, and are used to trigger the execution of certain business logic.

After *appcfg* has processed all configuration parameters regardless of source the *route path* is accessible as `_RoutePath` on the resulting config data structure (here: `cfg`).

Likewise the parameters to the route are accessible as fields on that (dynamic) data structure, e.g. `cfg.a` for the `-a` parameter.

Another way to run the program would be:

`demo.exe /a 2 --b=3`

There are several changes to note:

* The *route path* is missing. But that does not matter since `add` was defined as the default route (see below).
* The parameters are started with different characters. *appcfg* looks for `-` and `--` and `/` to signify parameter names.
* The parameter values are either attached to the parameter name or follow as the next argument on the command line. When attached the delimiter can be `:` or `=`.

Also the program coule be started like this:

`demo.exe addition /a 2 --b=3`

The *route path* is back, but it's named differently. *appcfg* allows aliases for routes and parameter names:

`demo.exe addition /x 2 --y=3`

#### Unnamed Parameters (Catch All)
Now on to another mathematical operation offered by the program. To sum up a couple of numbers the program can be called like this:

`demo.exe sum 1 2 3`

No parameter name has been specified. Instead all numbers given are treated the same. The belong to a *catch all* parameter defined for the route (see below).

In the code the values are accessible through the field `_CatchAll`.

#### Reading from Different Source
Parameters can be defined on the command line as shown so far. For values changing from call to call this is the easiest way. But sometimes there are parameters which rarely change or you want to predefine. In that case you can define them in a config file or in the environment.

Take the `div` *route path* for example:

`demo.exe div -a 10 -b 3`

The result shown is *3*. Why not *3.33333*? Because the business logic found the parameter `cfg.round` to be set to `true`.

This value has not been given on the command line, though. Instead it's predefined in a JSON config file like so:

```
{
  "round":"true"
}
```

*appcfg* makes this transparent to you.

If you want to change this global definition you can override it by passing in a value for `round` on the command line:

`demo.exe div -a 10 -b 3 -round:false`

Now the result has full precision.

Or what about a division by zero?

`./demo.sh div -a 10 -b 0`

The result is *1,79769313486232E+308* Why is there no exception or an output of *+infinite*?

Again, *appcfg* read a parameter value from a different source. This time it's the environment variable `DEMO_SUPPRESSDIVBY0` which is set in the script:

```
#!/usr/bin/env bash
export DEMO_SUPPRESSDIVBY0="true"
mono demo.exe $*
```

Its value is visible in the business logic as `cfg.suppressDivBy0`.

But if your want to overrule such a setting in the environment you can pass in a different value on the command line:

`./demo.sh div -a 10 -b 0 /suppressDivBy0=false`

*appcfg* views the command line, a config file, and environment variables as a hierarchy of sources. Definitions in lower levels can be overridden on higher levels with the command line being the top level.

### Defining the Configuration Schema
Which routes and parameters an application configuration consists of is defined by a schema:

```
using appcfg;
...
var cfgSchema = new AppCfgSchema("democonfig.json",
    new Route("add", "addition", isDefault:true)
        .Param("a","x",valueType:ValueTypes.Number,isRequired:true)
        .Param("b","y",valueType:ValueTypes.Number,isRequired:true),
    new Route("sum","summation")
        .Param("*",valueType:ValueTypes.Number),
    new Route("div", "division")
        .Param("a","x,divident",valueType:ValueTypes.Number,isRequired:true)
        .Param("b","y,divisor",valueType:ValueTypes.Number,isRequired:true)
        .Param("round","r",valueType:ValueTypes.Bool)
        .Param("suppressDivBy0",valueType:ValueTypes.Bool,defaultValue:false,environmentVarName:"DEMO_SUPPRESSDIVBY0")
    );

var cfgcomp = new AppCfgCompiler(cfgSchema);

var cfg = cfgcomp.Compile(args);
```

The schema is a hierarchy:

* there can be many *routes* (with one being the default route)
  * each route can have many *parameters*

What you need to know about configuration schemas in short:

* Routes as well as parameters have a primary name and possibly several aliases.
* If parameters have a value its type needs to be specified (e.g. numeric, date/time, boolean). They can have a default value.
* Parameters without a value are flags. Their type by default is boolean. If present their value is `true`, otherwise `false`.
* Parameters can be made required. If they are missing in all sources an exception is thrown.
* The order in which parameters are defined on the command line is not important.
* The catch-all parameter has the name `*` and needs to be the last parameter in a route.

After a schema has been defined your initialize a compiler (`AppCfgCompiler`) with it and then compile `args` (or any `string` array). The result is a `dynamic` variable with the primary parameter names as its fields.

All parameters of the route found in the `args` are present on this result, whether they were present or missing. Either they have a specified value found in one of the sources or they have their default value or are set to `true`/`false´ as flags.



