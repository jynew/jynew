--[[
/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
 ]]--

require "LuaClass"

Jyx2ConfigMgr = {
    --实例对象
    _instance = nil,
    --缓存表格数据
    _cacheConfig = {},
    --具有id的表的快速索引缓存，结构__fastIndexConfig["LanguageCfg"][100]
    _quickIndexConfig = {},
}
Jyx2ConfigMgr.__index = Jyx2ConfigMgr
setmetatable(Jyx2ConfigMgr, LuaClass)

-- 构造器
function Jyx2ConfigMgr:new()
    local self = {}
    self = LuaClass:new()
    setmetatable(self,Jyx2ConfigMgr)
    return self
end

-- 获取单例
function Jyx2ConfigMgr:Instance()
    if Jyx2ConfigMgr._instance == nil then
        Jyx2ConfigMgr._instance = Jyx2ConfigMgr:new()
    end
    return Jyx2ConfigMgr._instance
end

-- 添加一个配置表
function Jyx2ConfigMgr:AddConfigTable(cfgName, cfgData)
    local tmpCfg = self._cacheConfig[cfgName]
    if tmpCfg == nil then
        local _id = cfgData[1].Id
        if _id then
            self._cacheConfig[cfgName] = {}
            for _,v in ipairs(cfgData) do
                self._cacheConfig[cfgName][v.Id] = v
            end
        else
            self._cacheConfig[cfgName] = cfgData
        end
    end
end

-- 获取对应的表格数据
function Jyx2ConfigMgr:GetConfig(name)
    local tmpCfg = self._cacheConfig[name]
    if nil ~= tmpCfg then
        return tmpCfg
    else
        CS.UnityEngine.Debug.LogError("配置表 "..name.." 不存在")
    end
    return nil
end

-- 获取表格中指定的ID项
function Jyx2ConfigMgr:GetItem(name,id)
    if nil == self._quickIndexConfig[name] then
        local cfgData = self:GetConfig(name)
        if cfgData and cfgData.items and cfgData.items[1] then
            -- 如果是空表的话不做处理
            local _id = cfgData.items[1].id
            if _id then
                -- 数据填充
                self._quickIndexConfig[name] = {}
                for _,v in ipairs(cfgData.items) do
                    self._quickIndexConfig[name][v.id]= v
                    print("---->"..v.id)
                end
            else
                print(string.format("Config: %s don't contain id: %d!",name,id))
            end
        end
    end
    if self._quickIndexConfig[name] then
        return self._quickIndexConfig[name][id]
    end
    return nil
end
