如果发现BUG，请联系作者 guanvvv@qq.com

这个目录的源码设计之初是跨项目的。所以请勿污染。

# 学习路径

- readme.md
- 打包时：
    - ModMenuBK.cs
- 运行时：
    - BasicModRuntime.cs

# 设计原则

- MOD只根据目录名作为全局唯一标识符。
- 如果多人制作的MOD的目录相同，那么只能引入一个。
- 在Unity里面，每个MOD都是独立的根目录和独立的组。
- 在打包的时候，会将MOD分别打包到 ModOutput目录下。
- 游戏里面是固定的目录作为MOD的根目录，所有MOD都直接下载到这个目录下面。
- 如果MOD需要引用原始工程的资源，则只能在原始工程中建立。

# 实现思路

- 灵活动态修改Addressable的配置

# 如何标记MOD

- 在Inspector窗口中 勾选Asserts/Mod/XXX 的Addressable
- 打开 Window/AssetManagement/Addressable/Groups
- 创建一个Group，名称是XXX
- 在该Group的Inspector中
    - [AddSchema] BasicModSchema
    - 去除选择框：Context Packing & vLoading/Advanced Options/Include In Build

# 如何将反标记MOD

- 在该Group的Inspector中
    - 移除BasicModSchema

# 如何添加MOD相关菜单

- 直接将ModMenuBK.cs中的代码拷贝到项目中，自行修改菜单内容

# 如何修复设置

- 因为Addressable功能和配置相当多，为了MOD能够正常工作，需要将许多属性设置为特别的值。
- 使用项目的菜单进行相关的Addressable打包操作的时候，会自动修复设置。
- 如果需要手工修复，也可以点击菜单 */修复配置

# 如何打包Addressable

- 使用项目的菜单项进行操作。
- 尽量不要使用Unity原生菜单或者原生函数。
- 代码里面如果需要操作这些功能，直接调用或者参考 ModMenu的相关函数即可。

# 如何发布

- 所有Mod在打包后都放在ModOutput目录下
- 运行时所有MOD的根目录
    - UnityEditor下：ModOutput
    - 在WindowsExe下：游戏根目录下的ModDeploy
    - 其他平台：Application.persistentDataPath + "/ModDeploy"
- 项目组需要自行拷贝缺省MOD到安装目录
- 在Mod根目录下的MOD不需要注册，可以直接初始化，这种Mod发布称作《标准发布》
- 如果需要注册非Mod根目录下的MOD，则调用函数 BasicModRuntime.RegisterMod
- 可以用非标注册来覆盖内部MOD

# 如何初始化

- 注册非标准发布的MOD：BasicModRuntime.RegisterMod
- 初始化Addressable：await Addressables.InitializeAsync().Task
- 选择MOD并初始化：await BasicModRuntime.InitModsAsync(...)

# 局限性

- 目前只支持用户自行下载到本地某个目录或者Mod根目录，不支持网络下载。项目可以自行去整。
- 如果在运行时需要动态调整MOD，则需要修改Addressable源码。所以本BasicMod不支持动态调整。
    - 所以如果游戏用户选择了新的Mods，那么需要重启游戏。
- 目前仅仅在 Addressable 1.17.13 下进行过测试

# 内置资源的引用

- 因为MOD开发者必须使用主干工程进行MOD开发，因此引用内置资源是自然的。
- 这些内置资源在打包的时候，不会打包到ModOutput中。
- 如果某个内置资源1被MODA的资源2引用，然而后来从主干中删除了，那么最新版本的游戏在加载ModA.2的时候当然会错了。
- 每个Mod所引用的UnityBuiltinShader都会独立打包。
    - 因为主工程无法保证所有Mod用到的Shader都会在缺省资源中引用。

# 待完成

- Mod规则的检测和报错，比如：
    - MOD的组里面只能有一条目录记录。
    - MOD的根目录是Assets/Mods/*
    - MOD的组名是目录名
- 支持增加快速启动