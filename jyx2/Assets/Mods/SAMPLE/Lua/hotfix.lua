-- 这里是热修复C#逻辑代码的地方
local util = require 'xlua.util'

--这里是展示给大家如何通过lua的热更新进行函数逻辑重写
util.hotfix_ex(CS.Jyx2.RoleInstance, "InitData", function(self)
    print("lua hot fix called!") --打印调试信息
    self:InitData()  --先执行原函数++++

    --再补执行mod中的附加逻辑
    local function eszy()
        self.MaxHp = self.MaxHp + 100
    end

    local function normal()
        self.MaxHp = self.MaxHp + 200
    end

    local function hard()
        self.MaxHp = self.MaxHp + 300
    end

    local switch = {
        [0] = eszy,
        [1] = normal,
        [2] = hard,
    }

    local difficulty = CS.GameSettingManager.settings[CS.GameSettingManager.Catalog.Difficulty]

    print("Difficulty: ", difficulty)

    if (switch[difficulty]) then
        switch[difficulty]()
        print("switch difficulty called!")
    else
        print("case default")
    end

end)

--[[TODO：
例子

 直接定义函数的例子
 修改UI的例子
 新建UI的例子
 修改战斗参数的例子
]]