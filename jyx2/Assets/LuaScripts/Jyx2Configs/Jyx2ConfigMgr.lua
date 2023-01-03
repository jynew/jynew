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

-- 当前的配置表列表
Jyx2ConfigMgr["ConfigList"] = {}

-- 添加一个配置表
function Jyx2ConfigMgr:AddConfigTable(cfgName, cfgData)
    local tmpCfg = self[cfgName]
    -- 如果存在这个配置表那就不再重复添加
    if tmpCfg == nil then
        local length = #cfgData
        -- print("len: "..length)
        -- 长度小于1说明表是空的
        if length < 1 then
            self[cfgName] = cfgData
        -- 如果元素中有Id这个字段，就改造成字典
        elseif cfgData[1].Id then
            self[cfgName] = {}
            for _,v in pairs(cfgData) do
                self[cfgName][v.Id] = v
            end
        else
            self[cfgName] = cfgData
        end
        -- 记录Config列表
        table.insert(self.ConfigList, cfgName)
    end
    self[cfgName]["ItemNum"] = length
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

-- 清空所有配置表
function Jyx2ConfigMgr:DeInit()
    for i,cfgName in pairs(self.ConfigList) do
        self[cfgName] = nil
    end
    self.ConfigList = {}
end

return Jyx2ConfigMgr
