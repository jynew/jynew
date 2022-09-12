-- 这里是热修复C#逻辑代码的地方
local util = require 'xlua.util'

--这里是展示给大家如何通过lua的热更新进行函数逻辑重写
util.hotfix_ex(CS.Jyx2.RoleInstance, "LevelUp", function(self)
    print("lua hot fix called!") --打印调试信息
    self:Recover()  --先执行原函数

    --再补执行mod中的附加逻辑
    if(self.IQ > 50 and self.Wuxuechangshi < 50 ) then
        self.Wuxuechangshi = self.Wuxuechangshi + 1
    end
end)