# 配置文件说明
配置文件使用Excel文件编写，文件名无要求，但建议使用容易理解的名字。编写好后，需到Unity编辑器中点击Mod根目录中的`ModSetting`文件，并在右侧`Inspector`面板中点击“生成配置表”按钮，以生成Lua配置表。
Lua配置表为实际上在游戏中使用的配置表，但请不要手动修改。想要修改时先修改对应的Excel，再到Unity编辑器中重新生成Lua配置表。
## Excel配置表格式
示例如下表：
Excel|A|B|C
--|--|--|--
1|LuaConfigGen | ConfigName|
2|代号|名称|更多字段
3|L_number|L_string|L_number[]
4|Id|Name|Other
5|1|胡斐|1,3,5
- `A1`单元格是固定的，用来标明这个表格是一个配置表。
- `A2`单元格中填写这个配置表的名称，生成Lua配置表之后，可以在游戏中使用这个名称来访问该配置表，例如，用`Jyx2.ConfigMgr.Character`访问名为`Character`的配置表。
- 第二行到第四行是字段设置。其中第二行为每个字段的中文解释，第三行指定字段中的值是什么类型，第四行为字段的名字。
- 第五行开始是具体的数据
### 字段的解释
第二行目前只是为了让excel表容易理解，不会在Lua表中出现
### 字段数据类型
数据类型分为基础型、复合型、列表型三种。
- 基础型：
  * 数字：`L_number` 
  * 字符串：`L_string`
  * 布尔：`L_boolean`
 - 复合型：将基础型使用`,`隔开，例如
   * `L_number,L_string`
   * `L_number,L_boolean,L_number`
 - 列表型：基础型或复合型加上`[]`，用来表示多个该类型的表，例如
   * `L_number[]`表示一个数字序列
   * `L_number,L_string[]`表示一个复合型序列，序列中每个元素是由一个数字和一个字符串组成的复合型数据
### 字段名称
使用拉丁字符表示，生成Lua配置表后，可以在游戏中使用该名称访问这个字段。例如`Jyx2.ConfigMgr.Character[0].Name`为配置表`Character`中第1条数据的`Name`字段的值。
### 数据
每一行为一条数据，`Id`字段的值可以用来访问这条数据，例如使用`Jyx2.configMgr.Character[233]`可以读取配置表`Character`中`Id`为`233`的那条数据。
- 复合型字段的值，使用`,`分隔复合型值的每一个元素。例如`13,abc`为一个`L_number,L_string`类型的值。
- 列表型字段的值，使用`|`分隔列表中的每一个元素。例如`13|14|16`为一个`L_number[]`类型的值，`13,abc|14,ef`为一个`L_number,L_string[]`类型的值。
## Lua配置表使用方法
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
-- 注意，不可以在Lua中修改配置表中的值，例如下面的代码会直接触发异常。
-- 修改具体数据请在RuntimeData中进行。
item1.Name = "xxx"
```

