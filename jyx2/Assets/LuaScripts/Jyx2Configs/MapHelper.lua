--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本负责为ConfigMgr中的配置表添加专门的功能

local helpers = {}
-- Map 表有一个不做初始化的项 ForceSetLeaveMusicId .此项为-1或nil则表示此项不起作用；如果值为正数则用此数代替OutMusic
function helpers.GetOutMusic(cfg)
    if cfg.ForceSetLeaveMusicId == nil or cfg.ForceSetLeaveMusicId ==-1 then
        return cfg.OutMusic
    else
        return cfg.ForceSetLeaveMusicId
    end
end
-- 获取标签数据，以半角的冒号分割
function helpers.GetTagValue(cfg, str)
    return string.match(cfg.Tags, str..":(%d+)") or ""
end
-- 获取跳转场景数据，以半角的冒号分割
function helpers.GetTransportToMapValue(cfg, str)
    return tonumber(string.match(cfg.TransportToMap, str..":(%d+)"))
end
-- 获取地图显示名称
function helpers.GetShowName(cfg)
    -- 展示主角出生地图名称
    if CS.Jyx2.GlobalAssetConfig.Instance.defaultHomeName == cfg.Name then
        return CS.Jyx2.GameRuntimeData.Instance.Player.Name.."居"
    end
    return cfg.Name
end
-- 获取开场地图
function helpers.GetGameStartMap(cfg)
    local mapTable = Jyx2.ConfigMgr:GetConfig("Map")
    for i,v in pairs(mapTable) do
        if type(v) == "table" and string.find(v.Tags, "START") then
            return v
        end
    end
    return nil
end
-- 使用场景文件名称获取地图
function helpers.GetMapBySceneName(cfg, sceneName)
    local mapTable = Jyx2.ConfigMgr:GetConfig("Map")
    for i,v in pairs(mapTable) do
        if type(v) == "table" and v.MapScene == sceneName then
            return v
        end
    end
    return nil
end
-- 使用场景名称获取地图
function helpers.GetMapByName(cfg, name)
    local mapTable = Jyx2.ConfigMgr:GetConfig("Map")
    for i,v in pairs(mapTable) do
        if type(v) == "table" and v.Name == name then
            return v
        end
    end
    return nil
end

return helpers
