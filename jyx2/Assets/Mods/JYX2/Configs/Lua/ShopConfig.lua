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
{1,{{7,1000,80},{104,1000,100},{16,1000,150},{111,1,400},{59,1,700}},16},
{3,{{1,1000,20},{8,1000,150},{36,1,500},{118,1,400},{85,1,600}},14},
{40,{{26,1000,100},{5,1000,60},{35,1,500},{120,1,800},{71,1,300}},20},
{60,{{9,1000,200},{20,1000,250},{97,1000,10},{114,1,300},{55,1,250}},12},
{61,{{19,1000,250},{13,1000,50},{28,1,400},{119,1,500},{51,1,300}},9},}
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
local configMgr = Jyx2:GetModule('ConfigMgr')
configMgr:AddConfigTable([[Shop]], data)