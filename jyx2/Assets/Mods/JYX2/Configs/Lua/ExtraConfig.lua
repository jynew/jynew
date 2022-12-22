--[[
本文件由编辑器自动生成，如需修改请先修改Excel表格后再使用Unity生成本文件

金庸群侠传3D重制版
https://github.com/jynew/jynew

这是本开源项目文件头，所有代码均使用MIT协议。
但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。

金庸老先生千古！
]]
local fieldIdx = {}
fieldIdx.Id = 1
fieldIdx.Weapon = 2
fieldIdx.Wugong = 3
fieldIdx.ExtraAttack = 4
local data = {
{106,106,57,100},
{107,107,49,50},
{108,108,49,50},
{115,115,63,50},
{116,116,67,70},
{110,110,54,80},
{119,119,68,100},
}
local mt = {}
mt.__index = function(a,b)
	if fieldIdx[b] then
		return a[fieldIdx[b]]
	end
	return nil
end
mt.__metatable = false
for _,v in pairs(data) do
	setmetatable(v,mt)
end
local configMgr = Jyx2:GetModule('ConfigMgr')
configMgr:AddConfigTable([[Extra]], data)