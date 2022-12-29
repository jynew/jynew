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
print("Jyx2 Init")
local requireList = require "LuaModuleList"
Jyx2 = {} -- 所有的Lua模块都作为Jyx2表的元素添加进去
Jyx2.moduleList = requireList

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

function Jyx2:DeInit()
    print("Jyx2 DeInit")
    Jyx2.ConfigMgr:ClearAllConfig()
end

-- 辅助函数
-- prequire 用于载入模块，如果模块不存在，返回空表
function prequire(name)
    local rst = {}
    if pcall(function(x) rst = require(x) end, name) then
        return rst
    else
        return rst
    end
end
