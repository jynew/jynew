# LuaScripts使用说明
- 在此进行游戏功能模块的Lua化，目前已经实现了`Jyx2Configs`模块。
- 在mod的根目录创建`LuaScripts`目录，并加入mod的ab包中，即可用mod中的Lua文件覆盖游戏框架中对应的Lua文件。
- `LuaTestStarter.asset`文件为一个Lua代码调试器，点击后在右侧`Inspector`面板中按顺序加入需要运行的Lua脚本，点击`测试Lua代码`即可依次运行Lua脚本，方便开发中进行Lua脚本的测试。
## LuaScripts结构
LuaScripts使用模块化结构，总入口是`InitLuaScripts`脚本，该脚本在`LuaManager`初始化时自动调用。`LuaModuleList`脚本储存模块名称和模块主文件的映射表，用来辅助加载模块。在`LuaModuleList`中注册的模块可以直接调用，而没有注册的模块需先进行手动加载才可调用。
### 基础表：Jyx2
全局表`Jyx2`储存所有的模块并提供加载和调用的方法。
- 方法`Jyx2:AddModule(name, path)`，可手动加载主文件为`path`的模块，并指定名称为`name`。如果省略`path`参数，则默认从`LuaModuleList`中查找名称为`name`的模块并加载。
- 方法`Jyx2:GetModule(name)`，可返回名称为`name`的模块。如果模块`name`还没有加载，则根据`LuaModuleList`中的路径自动加载。
- 方法`Jyx2:IsLoaded(name)`，返回布尔型值，如果模块`name`已经加载了，返回值为真。
- 使用`Jyx2.name`来访问模块`name`也是允许的，但没有自动加载功能，推荐在确认模块已经加载时使用。
### 模块注册表：LuaModuleList
使用格式：`moduleName = "PathOfTheMainFile"`来注册一个模块。例如`ConfigMgr`模块主文件为`Assets/LuaScripts/Jyx2Configs/Jyx2ConfigMgr.lua`，则使用`ConfigMgr = "Jyx2Configs/Jyx2ConfigMgr"`来注册该模块。
在Mod中重载`LuaModuleList`文件，即可自行选择哪些模块可以自动加载。注意，如果要自动加载Mod中新增的模块，如名为`MyModuleName`，主文件为`Assets/Mods/MyMod/LuaScripts/MyModule/MyModuleMainFile.lua`的模块，需使用`MyModuleName = "MyModule/MyModuleMainFile"`来注册，即去掉`LuaScripts`以及之前的目录结构。
