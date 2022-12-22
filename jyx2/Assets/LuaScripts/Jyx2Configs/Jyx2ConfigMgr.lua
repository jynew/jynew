--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
 -- 本脚本负责配置文件的载入和访问

local Jyx2ConfigMgr = {}

-- 添加一个配置表
function Jyx2ConfigMgr:AddConfigTable(cfgName, cfgData)
    local tmpCfg = self[cfgName]
    if tmpCfg == nil then
        local _id = cfgData[1].Id
        if _id then
            self[cfgName] = {}
            for _,v in ipairs(cfgData) do
                self[cfgName][v.Id] = v
            end
        else
            self[cfgName] = cfgData
        end
    end
    self[cfgName]["ItemNum"] = self:GetLength(cfgName)
end

-- 获取某表格的长度
function Jyx2ConfigMgr:GetLength(name)
    local tmpCfg = self[name]
    if tmpCfg == nil then
        return 0
    end
    if tmpCfg.ItemNum ~= nil then
        return tmpCfg.ItemNum
    end
    local res = 0
    for k,v in pairs(tmpCfg) do
        res = res + 1
    end
    return res
end

-- 获取对应的表格数据
function Jyx2ConfigMgr:GetConfig(name)
    if name == nil then
        CS.UnityEngine.Debug.LogError("GetConfig 参数错误，使用:GetConfig")
        return nil
    end
    local tmpCfg = self[name]
    if nil ~= tmpCfg then
        return tmpCfg
    else
        CS.UnityEngine.Debug.LogError("配置表 "..name.." 不存在")
    end
    return nil
end

-- 获取表格中指定的ID项
function Jyx2ConfigMgr:GetItem(name,id)
    local tmpCfg = self:GetConfig(name)
    if tmpCfg ~= nil then
        if tmpCfg[id] ~= nil then
            return tmpCfg[id]
        else
            CS.UnityEngine.Debug.LogError("该ID "..id.." 不存在")
        end
    end
    return nil
end

return Jyx2ConfigMgr
