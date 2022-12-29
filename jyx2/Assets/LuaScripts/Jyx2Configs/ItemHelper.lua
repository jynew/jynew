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
-- 
function helpers.GetPic(cfg)
    return string.format("BuildSource/Items/%d.png", cfg.Id)
end

function helpers.IsWeapon(cfg)
    return (cfg.EquipmentType == 0)
end

function helpers.IsArmor(cfg)
    return (cfg.EquipmentType == 1)
end

function helpers.IsBook(cfg)
    return (cfg.ItemType == 2)
end

function helpers.NoItemUser(cfg)
    return (CS.Jyx2.GameRuntimeData.Instance:GetItemUser(cfg.Id) == -1)
end

function helpers.IsBeingUsedBy(cfg, roleId)
    local data = CS.Jyx2.GameRuntimeData.Instance
    if type(roleId) == 'number' then
        return (data:GetItemUser(cfg.Id) == roleId)
    end
    if roleId:GetType() == typeof(CS.Jyx2.RoleInstance) then
        return (data:GetItemUser(cfg.Id) == roleId.Key)
    end
end

function helpers.GetItemType(cfg)
    return cfg.ItemType
end

return helpers
