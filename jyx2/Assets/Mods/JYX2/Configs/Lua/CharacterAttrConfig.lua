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
fieldIdx.DisplayName = 2
fieldIdx.MinValue = 3
fieldIdx.MaxValue = 4
fieldIdx.OnStatePage = 5
fieldIdx.OnStartPage = 6
fieldIdx.DirectInit = 7
fieldIdx.InitAttrName = 8
fieldIdx.InitValue = 9
fieldIdx.AttrType = 10
fieldIdx.DisplayOrder = 11
local data = {
{[[Sex]],[[性别]],0,1,false,false,true,[[Sexual]],nil,1,1},
{[[Level]],[[等级]],1,30,true,false,true,[[Level]],nil,3,2},
{[[Exp]],[[经验值]],0,9999999,true,false,true,[[Exp]],nil,3,3},
{[[Hp]],[[生命]],-1,-1,true,false,true,[[MaxHp]],nil,2,4},
{[[PreviousRoundHp]],[[上一回合生命值]],-1,-1,false,false,true,[[MaxHp]],nil,1,5},
{[[MaxHp]],[[生命最大值]],0,999,true,true,true,[[MaxHp]],nil,2,6},
{[[Mp]],[[内力]],-1,-1,true,false,true,[[MaxMp]],nil,2,7},
{[[MaxMp]],[[内力最大值]],0,999,true,true,true,[[MaxMp]],nil,2,8},
{[[MpType]],[[内力性质]],0,2,true,false,true,[[MpType]],nil,1,9},
{[[Tili]],[[体力]],0,100,true,false,true,"",100,1,10},
{[[Hurt]],[[受伤程度]],0,100,false,false,false,"",nil,2,11},
{[[Poison]],[[中毒程度]],0,100,false,false,false,"",nil,2,12},
{[[Attack]],[[攻击力]],0,100,true,true,true,[[Attack]],nil,2,13},
{[[Qinggong]],[[轻功]],0,100,true,true,true,[[Qinggong]],nil,2,14},
{[[Defence]],[[防御力]],0,100,true,true,true,[[Defence]],nil,2,15},
{[[Heal]],[[医疗]],0,100,true,true,true,[[Heal]],nil,2,16},
{[[UsePoison]],[[用毒]],0,100,true,true,true,[[UsePoison]],nil,2,17},
{[[DePoison]],[[解毒]],0,100,true,true,true,[[DePoison]],nil,2,18},
{[[AntiPoison]],[[抗毒]],0,100,false,false,true,[[AntiPoison]],nil,2,19},
{[[Quanzhang]],[[拳掌]],0,100,true,true,true,[[Quanzhang]],nil,2,20},
{[[Yujian]],[[御剑]],0,100,true,true,true,[[Yujian]],nil,2,21},
{[[Shuadao]],[[耍刀]],0,100,true,true,true,[[Shuadao]],nil,2,22},
{[[Qimen]],[[特殊兵器]],0,100,true,true,true,[[Qimen]],nil,2,23},
{[[Anqi]],[[暗器技巧]],0,100,true,true,true,[[Anqi]],nil,2,24},
{[[Wuxuechangshi]],[[武学常识]],0,100,true,true,true,[[Wuxuechangshi]],nil,2,25},
{[[Pinde]],[[品德]],0,100,true,true,true,[[Pinde]],nil,2,26},
{[[AttackPoison]],[[攻击带毒]],0,100,true,true,true,[[AttackPoison]],nil,2,27},
{[[Zuoyouhubo]],[[左右互搏]],0,100,true,true,true,[[Zuoyouhubo]],nil,2,28},
{[[Pinde]],[[声望]],0,200,true,true,true,"",0,2,29},
{[[IQ]],[[资质]],0,100,true,true,true,[[IQ]],nil,2,30},
{[[HpInc]],[[生命增长]],-1,-1,false,false,true,[[HpInc]],nil,2,31},
{[[Weapon]],[[武器]],-1,-1,false,false,true,[[Weapon]],nil,2,32},
{[[Armor]],[[防具]],-1,-1,false,false,true,[[Armor]],nil,2,33},
{[[Xiulianwupin]],[[修练物品]],-1,-1,false,false,true,"",-1,2,34},
{[[ExpForItem]],[[修炼点数]],-1,-1,false,false,false,"",nil,2,35},
{[[ExpForMakeItem]],[[物品炼制点]],-1,-1,false,false,false,"",nil,2,36},
{[[CurrentSkill]],[[当前技能]],-1,-1,false,false,false,"",nil,2,37},
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
configMgr:AddConfigTable([[CharacterAttr]], data)