--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为Lua侧游戏战斗模块初始化
local battle = {}

battle.SkillCoverType = {
    POINT = 0,
    LINE = 1,
    CROSS = 2,
    RECT = 3,
    RHOMBUS = 4,
    INVALID = -1
}

battle.DamageCaculator = jy_utils.prequire("Jyx2Battle/DamageCaculator")
battle.RangeLogic = jy_utils.prequire("Jyx2Battle/RangeLogic")
battle.AIManager = jy_utils.prequire("Jyx2Battle/AIManager")
battle.Manager = jy_utils.prequire("Jyx2Battle/BattleMgr")

return battle
