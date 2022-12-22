--[[
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 ]]--
-- 本脚本为RoleInstance类定义
local class = require('LuaClass')
Jyx2:AddModule("ConfigMgr")

-- 创建角色类
local Role = class("RoleInstance")
function Role:ctor(key)
    self.Key = key or 0
    -- 读取角色配置
    self:BindKey()
    self:InitData()
end

function Role:BindKey()
    self._data = assert(Jyx2.ConfigMgr:GetItem("Character", self.Key), "Id 为 "..self.Key.." 的角色配置不存在")
    -- 初始化武功列表
end

function Role:InitData()
    self.Name = self._data.Name
    local attr = assert(Jyx2.ConfigMgr:GetConfig("CharacterAttr"), "属性表不存在")
    for i,a in pairs(attr) do
        if i ~= "ItemNum" and a.DirectInit then
            if a.InitAttrName ~= "" then
                self[i] = self._data[a.InitAttrName]
            else
                self[i] = a.InitValue
            end
        end
    end
end

function Role:printSelf()
    print("Key: "..self.Key)
    print("Name: "..self.Name)
end

local r = Role.new(2)
r:printSelf()
