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
    print("Jyx2 重复定义")
    return 
end

local requireList = require "LuaModuleList"
Jyx2 = {} -- 所有的Lua模块都作为Jyx2表的元素添加进去
Jyx2.moduleList = {} -- 模块列表

function Jyx2:AddModule(name, path)
    -- 如果没指定模块地址，则从表中获取
    if path == nil then
        path = requireList[name]
    end
    local tmpMdl = self[name]
    if tmpMdl == nil then
        tmpMdl = require(path)
        if tmpMdl == nil then
            print("Jyx2-Error: 添加模块 "..name.." 失败")
        end
        print("Jyx2 增加模块: "..name)
        self[name] = tmpMdl
        table.insert(self.moduleList, name)
    end
end

function Jyx2:GetModule(name)
    local tmpMdl = self[name]
    if tmpMdl ~= nil then
        return tmpMdl
    else
        self:AddModule(name)
        return self[name]
    end
    return nil
end

function Jyx2:IsLoaded(name)
    return self[name] ~= nil
end

-- 初始化
function Jyx2:Init()
    print("Jyx2 Init")
    -- 根据模块列表添加模块
    for mod,path in pairs(requireList) do
        self:AddModule(mod,path)
    end
end

-- 反初始化
function Jyx2:DeInit()
    print("Jyx2 DeInit")
    for i,mod in pairs(self.moduleList) do
        if self[mod] ~= nil then
            -- 如果模块有自己的DeInit函数就运行一下
            if self[mod].DeInit ~= nil then
                self[mod]:DeInit()
            end
            -- 注销这个模块
            self[mod] = nil
        end
    end
    self.moduleList = {}
end

-- 辅助函数
jy_utils = require("Jyx2Utils")

Jyx2:Init()
