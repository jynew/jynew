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
 
 
--所有的指令文档可以参阅以下链接
--https://github.com/jynew/jynew/wiki/%E6%B8%B8%E6%88%8F%E4%BA%8B%E4%BB%B6%E6%8C%87%E4%BB%A4


--注释后面标记的是对应的指令号

util = require 'xlua.util'

luaBridge = CS.Jyx2.Jyx2LuaBridge

function combine(func1, func2)
	return function()
		local rst = func1()
		func2(rst)
	end
end

--系统预留函数，不要在lua中调用
LuaExecFinished = luaBridge.LuaExecFinished

--带延迟回调的函数
Talk = util.async_to_sync(luaBridge.Talk)
TryBattle = util.async_to_sync(luaBridge.TryBattle)
AskBattle = util.async_to_sync(luaBridge.AskBattle)
AskJoin = util.async_to_sync(luaBridge.AskJoin)
AskRest = util.async_to_sync(luaBridge.AskRest)
LightScence = util.async_to_sync(luaBridge.LightScence)
DarkScence = util.async_to_sync(luaBridge.DarkScence)
ShowEthics = util.async_to_sync(luaBridge.ShowEthics)
ShowRepute = util.async_to_sync(luaBridge.ShowRepute)

ShowMessage = util.async_to_sync(luaBridge.ShowMessage)
ShowYesOrNoSelectPanel = util.async_to_sync(luaBridge.ShowYesOrNoSelectPanel)
WeiShop = util.async_to_sync(luaBridge.WeiShop)
AskSoftStar = util.async_to_sync(luaBridge.AskSoftStar)

jyx2_WalkFromTo = util.async_to_sync(luaBridge.jyx2_WalkFromTo)
jyx2_Wait = util.async_to_sync(luaBridge.jyx2_Wait)


--以下是不带延迟回调的函数
AddItemWithoutHint = luaBridge.AddItemWithoutHint
ModifyEvent = luaBridge.ModifyEvent
UseItem = luaBridge.UseItem
ChangeMMapMusic = luaBridge.ChangeMMapMusic
Join = luaBridge.Join
Rest = luaBridge.Rest
Dead = luaBridge.Dead
InTeam = luaBridge.InTeam
SetScenceMap = luaBridge.SetScenceMap
HaveItem = luaBridge.HaveItem
SetScencePosition2 = luaBridge.SetScencePosition2
TeamIsFull = luaBridge.TeamIsFull
Leave = luaBridge.Leave
ZeroAllMP = luaBridge.ZeroAllMP
SetOneUsePoi = luaBridge.SetOneUsePoi
ScenceFromTo = luaBridge.ScenceFromTo
Add3EventNum = luaBridge.Add3EventNum
PlayAnimation = luaBridge.PlayAnimation
JudgeEthics = luaBridge.JudgeEthics
JudgeAttack = luaBridge.JudgeAttack
WalkFromTo = luaBridge.WalkFromTo
JudgeMoney = luaBridge.JudgeMoney
AddItem = luaBridge.AddItem
LearnMagic2 = luaBridge.LearnMagic2
AddAptitude = luaBridge.AddAptitude
SetOneMagic = luaBridge.SetOneMagic
JudgeSexual = luaBridge.JudgeSexual
AddEthics = luaBridge.AddEthics
ChangeScencePic = luaBridge.ChangeScencePic
OpenScene = luaBridge.OpenScene
SetRoleFace = luaBridge.SetRoleFace
NPCAddItem = luaBridge.NPCGetItem
JudgeFemaleInTeam = luaBridge.JudgeFemaleInTeam
Play2Amination = luaBridge.Play2Amination
AddSpeed = luaBridge.AddSpeed
AddMp = luaBridge.AddMp
AddAttack = luaBridge.AddAttack
AddHp = luaBridge.AddHp
SetPersonMPPro = luaBridge.SetPersonMPPro
instruct_50 = luaBridge.instruct_50

OpenAllScene = luaBridge.OpenAllScene
JudgeEventNum = luaBridge.JudgeEventNum
AddRepute = luaBridge.AddRepute
instruct_57 = luaBridge.instruct_57
FightForTop = luaBridge.FightForTop
AllLeave = luaBridge.AllLeave
JudgeScenePic = luaBridge.JudgeScenePic
Judge14BooksPlaced = luaBridge.Judge14BooksPlaced
EndAmination = luaBridge.EndAmination
SetSexual = luaBridge.SetSexual
PlayMusic = luaBridge.PlayMusic
PlayWave = luaBridge.PlayWave


jyx2_ReplaceSceneObject = luaBridge.jyx2_ReplaceSceneObject
jyx2_MovePlayer = luaBridge.jyx2_MovePlayer
jyx2_CameraFollow = luaBridge.jyx2_CameraFollow
jyx2_CameraFollowPlayer = luaBridge.jyx2_CameraFollowPlayer

jyx2_PlayTimeline = luaBridge.jyx2_PlayTimeline
jyx2_StopTimeline = luaBridge.jyx2_StopTimeline

jyx2_SwitchRoleAnimation = luaBridge.jyx2_SwitchRoleAnimation
jyx2_FixMapObject = luaBridge.jyx2_FixMapObject
jyx2_CheckEventCount = luaBridge.jyx2_CheckEventCount
jyx2_CheckBookAndRepute=luaBridge.jyx2_CheckBookAndRepute
jyx2_SetTimelineSpeed = luaBridge.jyx2_SetTimelineSpeed
jyx2_PlayTimelineSimple = luaBridge.jyx2_PlayTimelineSimple
jyx2_ShowEndScene = luaBridge.jyx2_ShowEndScene

SetFlag = luaBridge.jyx2_SetFlag
GetFlag = luaBridge.jyx2_GetFlag
SetFlagInt = luaBridge.jyx2_SetFlagInt
GetFlagInt = luaBridge.jyx2_GetFlagInt

RestFight = luaBridge.RestFight--战斗中休息
JudgeIQ = luaBridge.JudgeIQ--判断IQ
AddHeal = luaBridge.AddHeal--增加医疗
AddDefence = luaBridge.AddDefence--增加防御
AddQuanzhang = luaBridge.AddQuanzhang--增加拳掌
AddShuadao = luaBridge.AddShuadao--增加耍刀
AddYujian = luaBridge.AddYujian--增加御剑
AddAnqi = luaBridge.AddAnqi--增加暗器
AddQimen = luaBridge.AddQimen--增加奇门
AddWuchang = luaBridge.AddWuchang--增加武学常识
AddAttackPoison = luaBridge.AddAttackPoison--增加功夫带毒
AddExp = luaBridge.AddExp--增加经验
JudgeWCH = luaBridge.JudgeWCH--判断武学常识
JudgeHeal = luaBridge.JudgeHeal--判断医疗
JudgeQuanzhang = luaBridge.JudgeQuanzhang--判断拳掌
JudgeYujian = luaBridge.JudgeYujian--判断御剑
JudgeAttackPoison = luaBridge.JudgeAttackPoison--判断攻击带毒
JudgeQimen = luaBridge.JudgeQimen--判断奇门
JudgeDefence = luaBridge.JudgeDefence--判断防御
GetTeamMembersCount = luaBridge.GetTeamMembersCount--获取队伍人数
GetRoleLevel = luaBridge.GetRoleLevel--获取指定角色等级
GetTeamTotalHp = luaBridge.GetTeamTotalHp--获取队伍生命总和
GetTeamId = luaBridge.GetTeamId--获取队伍角色Id列表
RoleUseItem = luaBridge.RoleUseItem--指定角色使用物品
RoleUnequipItem = luaBridge.RoleUnequipItem--指定角色卸下物品（装备）

ShowSelectPanel = luaBridge.ShowSelectPanel
AddAntiPoison = luaBridge.AddAntiPoison
BackToMainMenu =luaBridge.BackToMainMenu
ScreenVignette = luaBridge.ScreenVignette

PreloadLua = luaBridge.PreloadLua --重新加载预加载的lua文件，一般用于调试hotfix使用

RestTeamPlus = luaBridge.RestTeamPlus--全队恢复（含计数器）
RestFightPlus = luaBridge.RestFightPlus--野外休息（含计数器）
RestPlus = luaBridge.RestPlus--休息（含计数器）
TryBattlePlus = luaBridge.TryBattlePlus--战斗（含计数器）

function main_getLuaFiles()
	return {}
end 