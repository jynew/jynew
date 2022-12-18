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
-- 注意，不可以在Lua中修改配置表中的值，例如
--item1.Name = "xxx"
print(roleName)
print(Jyx2:IsLoaded("ConfigMgr"))
print(Jyx2:IsLoaded("RuntimeData"))
