<img align="right" width="100" height="100" src="https://user-images.githubusercontent.com/37786733/41541140-71602302-731a-11e8-9434-79b3a57292b6.png">

[![Discord](https://img.shields.io/discord/361769369404964864.svg)](https://discord.gg/qgPrHv4)
[![GitHub issues](https://img.shields.io/github/issues/Siccity/xNode.svg)](https://github.com/Siccity/xNode/issues)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/Siccity/xNode/master/LICENSE.md)
[![GitHub Wiki](https://img.shields.io/badge/wiki-available-brightgreen.svg)](https://github.com/Siccity/xNode/wiki)

[Downloads](https://github.com/Siccity/xNode/releases) / [Asset Store](http://u3d.as/108S) / [Documentation](https://github.com/Siccity/xNode/wiki)

Support xNode on [Ko-fi](https://ko-fi.com/Z8Z5DYWA) or [Patreon](https://www.patreon.com/thorbrigsted)

### xNode
Thinking of developing a node-based plugin? Then this is for you. You can download it as an archive and unpack to a new unity project, or connect it as git submodule.

xNode is super userfriendly, intuitive and will help you reap the benefits of node graphs in no time.
With a minimal footprint, it is ideal as a base for custom state machines, dialogue systems, decision makers etc.

<p align="center">
  <img src="https://user-images.githubusercontent.com/6402525/53689100-3821e680-3d4e-11e9-8440-e68bd802bfd9.png">
</p>

### Key features
* Lightweight in runtime
* Very little boilerplate code
* Strong separation of editor and runtime code
* No runtime reflection (unless you need to edit/build node graphs at runtime. In this case, all reflection is cached.)
* Does not rely on any 3rd party plugins
* Custom node inspector code is very similar to regular custom inspector code
* Supported from Unity 5.3 and up

### Wiki
* [Getting started](https://github.com/Siccity/xNode/wiki/Getting%20Started) - create your very first node node and graph
* [Examples branch](https://github.com/Siccity/xNode/tree/examples) - look at other small projects

### Installing with Unity Package Manager
*(Requires Unity version 2018.3.0b7  or above)*

To install this project as a [Git dependency](https://docs.unity3d.com/Manual/upm-git.html) using the Unity Package Manager,
add the following line to your project's `manifest.json`:

```
"com.github.siccity.xnode": "https://github.com/siccity/xNode.git"
```

You will need to have Git installed and available in your system's PATH.

If you are using [Assembly Definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) in your project, you will need to add `XNode` and/or `XNodeEditor` as Assembly Definition References.

### Node example:
```csharp
// public classes deriving from Node are registered as nodes for use within a graph
public class MathNode : Node {
    // Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 
    [Input] public float a;
    [Input] public float b;
    // The value of an output node field is not used for anything, but could be used for caching output results
    [Output] public float result;
    [Output] public float sum;

    // The value of 'mathType' will be displayed on the node in an editable format, similar to the inspector
    public MathType mathType = MathType.Add;
    public enum MathType { Add, Subtract, Multiply, Divide}
    
    // GetValue should be overridden to return a value for any specified output port
    public override object GetValue(NodePort port) {

        // Get new a and b values from input connections. Fallback to field values if input is not connected
        float a = GetInputValue<float>("a", this.a);
        float b = GetInputValue<float>("b", this.b);

        // After you've gotten your input values, you can perform your calculations and return a value
        if (port.fieldName == "result")
            switch(mathType) {
                case MathType.Add: default: return a + b;
                case MathType.Subtract: return a - b;
                case MathType.Multiply: return a * b;
                case MathType.Divide: return a / b;
            }
        else if (port.fieldName == "sum") return a + b;
        else return 0f;
    }
}
```

Join the [Discord](https://discord.gg/qgPrHv4 "Join Discord server") server to leave feedback or get support.
Feel free to also leave suggestions/requests in the [issues](https://github.com/Siccity/xNode/issues "Go to Issues") page.
