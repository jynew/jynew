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
fieldIdx.ShopItems = 2
fieldIdx.Trigger = 3
local data = {
{12,{{120,1000,50},{121,1000,80}},1},
{4,{{42,1,200},{43,1,200},{44,1,200},{45,1,200},{46,1,400},{47,1,450}},3},
{11,{{110,100,10}},0},
{17,{{171,100,10}},0},}
local mt = {}
mt.__index = function(a,b)
	if fieldIdx[b] then
		return a[fieldIdx[b]]
	end
	return nil
end
mt.__newindex = function(t,k,v)
	error('do not edit config')
end
mt.__metatable = false
for _,v in ipairs(data) do
	setmetatable(v,mt)
end
require 'Jyx2Configs/Jyx2ConfigMgr'
local configMgr = Jyx2ConfigMgr:Instance()
configMgr:AddConfigTable([[Shop]], data)