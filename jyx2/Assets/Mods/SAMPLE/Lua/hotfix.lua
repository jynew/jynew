-- 这里是热修复C#逻辑代码的地方
local util = require 'xlua.util'

--这里是展示给大家如何通过lua的热更新进行函数逻辑重写
util.hotfix_ex(CS.Jyx2.RoleInstance, "LevelUp", function(self)
    print("lua hot fix called!") --打印调试信息
    self:LevelUp()  --先执行原函数

    --再补执行mod中的附加逻辑
    --例子明显一点
    self.MaxHP = 999
end)

--[[TODO：
例子

 直接定义函数的例子
 修改UI的例子
 新建UI的例子
 修改战斗参数的例子
]]