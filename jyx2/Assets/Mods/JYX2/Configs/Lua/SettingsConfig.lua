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
fieldIdx.Name = 2
fieldIdx.Value = 3
local data = {
{0,[[MAX_ROLE_LEVEL]],30},
{1,[[MAX_ROLE_TILI]],100},
{2,[[MAX_POISON]],100},
{3,[[MAX_USE_POISON]],100},
{4,[[MAX_HEAL]],100},
{5,[[MAX_DEPOISON]],100},
{6,[[MAX_ANTIPOISON]],100},
{7,[[MAX_HURT]],100},
{8,[[GAME_START_MUSIC_ID]],16},
{9,[[MAX_ROLE_WEAPON_ATTR]],100},
{10,[[MAX_ROLE_HP]],999},
{11,[[MAX_ROLE_MP]],999},
{12,[[MAX_ROLE_ATTACK]],100},
{13,[[MAX_ROLE_DEFENCE]],100},
{14,[[MAX_ROLE_QINGGONG]],100},
{15,[[MAX_ROLE_ATK_POISON]],100},
{16,[[MAX_ROLE_SHENGWANG]],200},
{17,[[MAX_ROLE_PINDE]],100},
{18,[[MAX_ROLE_ZIZHI]],100},
{19,[[MAX_WUGONG_LEVEL]],10},
{20,[[MONEY_ID]],174},
{21,[[MAX_TEAMCOUNT]],6},
{22,[[MAX_SKILL_COUNT]],10},
{23,[[MAX_ROLE_ATTRIBUTE]],100},
{24,[[MAX_BATTLE_TEAMMATE_COUNT]],6},
{25,[[WORLD_MAP_ID]],1000},
{26,[[LEVEL_UP_EXP]],50, 150, 300, 500, 750, 1050, 1400, 1800, 2250, 2750, 3850, 5050, 6350, 7750, 9250, 10850, 12550, 14350, 16750, 18250, 21400, 24700, 28150, 31750, 35500, 39400, 43450, 47650, 52000, 60000},
{27,[[PLAYER_MOVE_SPEED]],6},
{28,[[PLAYER_MOVE_SPEED_WORLD_MAP]],10},
{29,[[SAVE_COUNT]],3},
{30,[[CAM_SMOOTHING]],3},
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
configMgr:AddConfigTable([[Settings]], data)