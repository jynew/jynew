--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本负责LuaScripts初始化

-- 避免重复定义
if Jyx2 ~= nil then
    print("重复定义")
    return 
end
local requireList = require "LuaModuleList"
Jyx2 = {} -- 所有的Lua模块都作为Jyx2表的元素添加进去
Jyx2.moduleList = requireList[1]

function Jyx2:AddModule(name, path)
    if path == nil then
        path = self.moduleList[name]
    end
    local tmpMdl = self[name]
    if tmpMdl == nil then
        tmpMdl = require(path)
        if tmpMdl == nil then
            print("Jyx2-Error: 添加模块 "..name.." 失败")
        end
        self[name] = tmpMdl
    end
    if tmpMdl ~= nil then
        Jyx2CSBridge:AddBridge(name)
    end
end

function Jyx2:GetModule(name)
    local tmpMdl = self[name]
    if tmpMdl ~= nil then
        return tmpMdl
    else
        print("增加模块")
        self:AddModule(name)
        return self[name]
    end
    return nil
end

function Jyx2:IsLoaded(name)
    return self[name] ~= nil
end

-- 避免重复定义
if Jyx2CSBridge ~= nil then
    print("重复定义Jyx2CSBridge")
    return 
end
Jyx2CSBridge = {} -- 提供CS侧使用的API
Jyx2CSBridge.bridgeList = requireList[2]

function Jyx2CSBridge:AddBridge(name)
    local tmpCB = self[name]
    if tmpCB == nil then
        tmpCB = require(self.bridgeList[name])
        self[name] = tmpCB
    end
    print("添加bridge "..name)
end
