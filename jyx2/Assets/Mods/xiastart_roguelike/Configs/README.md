# 配置文件说明
配置文件使用Excel文件编写，文件名无要求，但建议使用容易理解的名字。编写好后，在编辑器中启动游戏的时候会自动读取并生成游戏内使用的Lua配置表。如果想要手动刷新，需到Unity编辑器中点击Mod根目录中的`ModSetting`文件，并在右侧`Inspector`面板中点击“生成配置表”按钮，即可重新生成Lua配置表。
Lua配置表为实际上在游戏中使用的配置表，但请不要手动修改。任何对Lua配置表的修改都会在下次启动游戏的时候被刷新覆盖，而且Lua配置表无法上传到项目的GitHub中。想要修改配置时修改对应的Excel即可。
## Excel配置表格式
示例如下表：
Excel|A|B|C
--|--|--|--
1|LuaConfigGen | ConfigName|
2|代号|名称|更多字段
3|L_number|L_string|L_number[]
4|Id|Name|Other
5|1|胡斐|1,3,5
- `A1`单元格是固定的，用来标明这个表格是一个配置表。如果`A1`中的值不是`LuaConfigGen`，这个Excel将被直接忽略。
- `B2`单元格中填写这个配置表的名称。可以在游戏中使用这个名称来访问该配置表。例如，`B2`的值为`Character`，则可以在Lua脚本中使用`Jyx2.ConfigMgr.Character`访问该配置表。
- 第二行到第四行是字段设置。其中第二行为每个字段的中文解释，第三行指定字段中的值是什么类型，第四行为字段的名字。
- 第五行开始是具体的数据，每一行表示一条数据。
### 字段的解释
第二行目前只是为了让excel表容易理解，不会对配置表的功能产生影响。
### 字段数据类型
数据类型分为基础型、复合型、列表型三种。
- 基础型：
  * 数字：`L_number` 
  * 字符串：`L_string`
  * 布尔：`L_boolean`
 - 复合型：将基础型使用`,`隔开，例如
   * `L_number,L_string`
   * `L_number,L_boolean,L_number`
   * 单独使用复合型是没有意义的，不如拆开变成几个基础型用起来方便。复合型主要是用来配合列表使用。
 - 列表型：基础型或复合型加上`[]`，用来表示由多个该类型数据组成的列表，例如
   * `L_number[]`表示一个数字列表
   * `L_number,L_string[]`表示一个复合型列表，列表中每个元素都是由一个数字和一个字符串组成的复合型数据
### 字段名称
- 使用拉丁字符表示，可以在游戏中使用该名称访问这个字段。例如在Lua中，使用`Jyx2.ConfigMgr.Character[0].Name`可获取配置表`Character`中第1条数据的`Name`字段的值。
- 如果一个字段的名称是空白的，或者使用`#`开头，那这个字段就会被忽略，游戏中将无法获取这个字段的任何值。
#### 复合型列表字段（嵌套表格）
如果一个字段的数据类型是复合型列表，为访问方便，应当为字段中复合型数据的每一个元素起一个名字。例如配置表`武功`中的
`Levels-Attack,SelectRange,AttackRange,AddMp,KillMp`
是一个复合型列表字段的名称，对应的数据类型为：`L_number,L_number,L_number,L_number,L_number[]`。
其中`Levels`为字段的实际名称，`Attack,SelectRange,AttackRange,AddMp,KillMp`分别为`Levels`中元素的五个属性的名称。
也就是说，`Levels`实际上是一个嵌套进来的子表。其下有名为`Attack`,`SelectRange`等五个子字段。使用`level0 = Jyx2.ConfigMgr.Skill[0].Levels`即可将表中`Id`为0的数据的`Levels`字段存储到`level0`中。`level0`则是一个新的表，使用`level0[1].Attack`即可访问`level0`表中第一个元素的`Attack`字段的值。
- 注意，上述例子中，如果只使用`Levels`来作为字段名称，那么例子中的`level0[1].Attack`可以使用`level0[1][1]`来访问，但会缺乏可读性。
### 数据
- 每一行为一条数据，`Id`字段的值可以用来访问这条数据，例如使用`Jyx2.configMgr.Character[233]`可以读取配置表`Character`中`Id`为`233`的那条数据。
- 如果一条数据的`Id`字段是空白的，或者使用`#`开头，那这条数据就会被忽略。
- 复合型字段的值，使用`,`分隔复合型值的每一个元素。例如`13,abc`为一个`L_number,L_string`类型的值。
- 列表型字段的值，使用`|`分隔列表中的每一个元素。例如`13|14|16`为一个`L_number[]`类型的值，`13,abc|14,ef`为一个`L_number,L_string[]`类型的值。

## 如何在游戏中读取配置表

### Lua侧的读取方法
生成的配置表会在游戏初始化时被读入`Jyx2.ConfigMgr`这个Lua 表中。在Lua脚本中使用如下代码可以访问Lua配置表，具体写法可以自由组合。
```Lua
-- 可以直接调用
local roleName = Jyx2.ConfigMgr.Character[2].Name
-- 但推荐先获取配置表模块，以后用起来会比较方便
local configMgr = Jyx2:GetModule("ConfigMgr")
-- 获取名为 Character 的配置表，下面几种方式都可以
local cfg1 = configMgr:GetConfig("Character")
local cfg2 = configMgr["Character"]
local cfg3 = configMgr.Character
-- 读取名为 Character 的配置表中 Id 为 123 的一条数据
local item1 = cfg1[123]
local item2 = configMgr.Character[123]
local item3 = configMgr:GetItem("Character", 123)
-- 读取名为`Name`的字段
local value1 = item1["Name"]
local value2 = item1.Name
local value3 = configMgr.Character[123].Name
-- 注意，尽量不要修改Lua配置表中的值，容易引起其他读取该配置的模块的异常。
```
### 为Lua配置表增加功能性函数
例如，想要为名称为`Map`的配置表中的每条数据增加一些功能性的函数，让他看起来像c#中的对象，可以建立`Jyx2ConfigMgr/MapHelper`模块，在其中定义相关的函数。例如其中的`GetShowName`函数，在Lua脚本中可以像对象的成员函数一样使用：
`Jyx2.ConfigMgr.Map[1]:GetShowName()`访问`Map`表中`Id`为1的那条数据的`GetShowName`函数。
具体格式参考`MapHelper`和`ItemHelper`文件即可。

### C#侧的读取方法
C#侧可以使用接口来访问Lua配置表。按照Lua侧的数据结果写一个接口，按照Xlua的说明添加标签，重新生成代码即可。具体定义参考`Assets/Scripts/LuaCore/LuaToCsBridge.cs`文件。
目前已经为各个配置表定义了相应接口。例如Item表映射到了字典
`Jyx2.LuaToCsBridge.ItemTable`，
可以通过`Jyx2.LuaToCsBridge.ItemTable[1]`访问Item表中的Id为1的那条数据。
- 注意，修改C#侧的接口需要重新编译游戏框架，因此在mod中是难以实现的。将来会将一部分游戏功能改为Lua侧实现，这样mod可以在Lua侧对配置表进行更大的修改。
