--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为Lua侧游戏战斗模块
local battleMgr = {}

function battleMgr.OnBattleStart()
    --print("on battle start")
    Jyx2.Battle.AIManager.Init()
end

function battleMgr.OnBattleEnd()
    --print("on battle end")
    Jyx2.Battle.AIManager.DeInit()
end

return battleMgr
