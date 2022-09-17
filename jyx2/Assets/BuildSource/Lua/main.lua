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
Talk = util.async_to_sync(luaBridge.Talk)--对话
TryBattle = util.async_to_sync(luaBridge.TryBattle)--战斗
AskBattle = util.async_to_sync(luaBridge.AskBattle)--询问是否战斗
AskJoin = util.async_to_sync(luaBridge.AskJoin)--询问角色是否加入
AskRest = util.async_to_sync(luaBridge.AskRest)--询问是否进行休息
LightScence = util.async_to_sync(luaBridge.LightScence)--场景变亮
DarkScence = util.async_to_sync(luaBridge.DarkScence)--场景变暗
ShowEthics = util.async_to_sync(luaBridge.ShowEthics)--显示道德
ShowRepute = util.async_to_sync(luaBridge.ShowRepute)--显示声望
ShowMessage = util.async_to_sync(luaBridge.ShowMessage)--显示信息确认框
ShowMessageSelectPanel = util.async_to_sync(luaBridge.ShowMessageSelectPanel)--显示二选一框
ShowYesOrNoSelectPanel = util.async_to_sync(luaBridge.ShowYesOrNoSelectPanel)--显示选择框
WeiShop = util.async_to_sync(luaBridge.WeiShop)--小宝商店
AskSoftStar = util.async_to_sync(luaBridge.AskSoftStar)--软体宝宝对话
ShowSelectPanel = util.async_to_sync(luaBridge.ShowSelectPanel)--选择函数
jyx2_WalkFromTo = util.async_to_sync(luaBridge.jyx2_WalkFromTo)--主角走路
jyx2_Wait = util.async_to_sync(luaBridge.jyx2_Wait)--等待（秒）
TryBattlePlus = util.async_to_sync(luaBridge.TryBattlePlus)--战斗（含计数器）

--以下是不带延迟回调的函数
AddItemWithoutHint = luaBridge.AddItemWithoutHint--获得物品,不显示提示
ModifyEvent = luaBridge.ModifyEvent--修改地图事件
UseItem = luaBridge.UseItem--使用物品
ChangeMMapMusic = luaBridge.ChangeMMapMusic--改变大地图音乐
Join = luaBridge.Join--角色加入
Rest = luaBridge.Rest--休息
Dead = luaBridge.Dead--死亡（gameover）
InTeam = luaBridge.InTeam--判断是否在队伍里
SetScenceMap = luaBridge.SetScenceMap--改变地图要素
HaveItem = luaBridge.HaveItem--判断是否拥有道具
SetScencePosition2 = luaBridge.SetScencePosition2
TeamIsFull = luaBridge.TeamIsFull--判断队伍是否满员
Leave = luaBridge.Leave--离队
ZeroAllMP = luaBridge.ZeroAllMP--全队清空内力
SetOneUsePoi = luaBridge.SetOneUsePoi--设置用毒能力
ScenceFromTo = luaBridge.ScenceFromTo--移动视角
Add3EventNum = luaBridge.Add3EventNum--修改事件
PlayAnimation = luaBridge.PlayAnimation--播放动画
JudgeEthics = luaBridge.JudgeEthics--判断道德
JudgeAttack = luaBridge.JudgeAttack--判断攻击力
WalkFromTo = luaBridge.WalkFromTo--主角行走
JudgeMoney = luaBridge.JudgeMoney--判断金钱
AddItem = luaBridge.AddItem--添加道具，显示提示
LearnMagic2 = luaBridge.LearnMagic2--学会武功
AddAptitude = luaBridge.AddAptitude--增加资质
SetOneMagic = luaBridge.SetOneMagic--设置角色的武功
JudgeSexual = luaBridge.JudgeSexual--判断性别
AddEthics = luaBridge.AddEthics--增加道德
ChangeScencePic = luaBridge.ChangeScencePic--换场景贴图
OpenScene = luaBridge.OpenScene--打开场景
SetRoleFace = luaBridge.SetRoleFace--设定主角面对方向
NPCAddItem = luaBridge.NPCGetItem--NPC角色获得道具
JudgeFemaleInTeam = luaBridge.JudgeFemaleInTeam--判断队伍里是否有女性
Play2Amination = luaBridge.Play2Amination--播放动画
AddSpeed = luaBridge.AddSpeed--加速度
AddMp = luaBridge.AddMp--加内力
AddAttack = luaBridge.AddAttack--加攻击力
AddHp = luaBridge.AddHp--加血
SetPersonMPPro = luaBridge.SetPersonMPPro--设置角色内功适性
instruct_50 = luaBridge.instruct_50

OpenAllScene = luaBridge.OpenAllScene--打开所有场景
JudgeEventNum = luaBridge.JudgeEventNum--判断触发器的交互事件
AddRepute = luaBridge.AddRepute--增加声望
instruct_57 = luaBridge.instruct_57
FightForTop = luaBridge.FightForTop--武林大会
AllLeave = luaBridge.AllLeave--全体离队
JudgeScenePic = luaBridge.JudgeScenePic--判断场景的图片
Judge14BooksPlaced = luaBridge.Judge14BooksPlaced--判断14天书摆放
EndAmination = luaBridge.EndAmination--结局动画
SetSexual = luaBridge.SetSexual--设定性别
PlayMusic = luaBridge.PlayMusic--播放音乐
PlayWave = luaBridge.PlayWave--播放音效


jyx2_ReplaceSceneObject = luaBridge.jyx2_ReplaceSceneObject--替换场景内容（同步存档）
jyx2_MovePlayer = luaBridge.jyx2_MovePlayer--移动主角
jyx2_CameraFollow = luaBridge.jyx2_CameraFollow--摄像机锁定到
jyx2_CameraFollowPlayer = luaBridge.jyx2_CameraFollowPlayer--摄像机归为到主角

jyx2_PlayTimeline = luaBridge.jyx2_PlayTimeline--播放动画
jyx2_StopTimeline = luaBridge.jyx2_StopTimeline--停止播放动画

jyx2_SwitchRoleAnimation = luaBridge.jyx2_SwitchRoleAnimation--切换地图角色动态（同步存档）
jyx2_FixMapObject = luaBridge.jyx2_FixMapObject--修改场景内物体位置（同步存档）
jyx2_CheckEventCount = luaBridge.jyx2_CheckEventCount
jyx2_CheckBookAndRepute=luaBridge.jyx2_CheckBookAndRepute--判断14天书和声望
jyx2_SetTimelineSpeed = luaBridge.jyx2_SetTimelineSpeed--设置动画播放速率，将影响jyx2_PlayTimeline和jyx2_Wait两个函数
jyx2_PlayTimelineSimple = luaBridge.jyx2_PlayTimelineSimple--播放动画（简单模式）播放完毕后将自动停止动画
jyx2_ShowEndScene = luaBridge.jyx2_ShowEndScene--游戏结束动画

SetFlag = luaBridge.jyx2_SetFlag
GetFlag = luaBridge.jyx2_GetFlag
SetFlagInt = luaBridge.jyx2_SetFlagInt
GetFlagInt = luaBridge.jyx2_GetFlagInt

RestFight = luaBridge.RestFight--野外休息
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


AddAntiPoison = luaBridge.AddAntiPoison--加抗毒
BackToMainMenu =luaBridge.BackToMainMenu--返回主菜单
ScreenVignette = luaBridge.ScreenVignette--屏幕边缘阴影
JoinWithoutHint = luaBridge.JoinWithoutHint--入队不提示
LeaveWithoutHint = luaBridge.LeaveWithoutHint--离队不提示
EnterPond = luaBridge.EnterPond--进黑龙潭
LeavePond = luaBridge.LeavePond--出黑龙潭
AddExpWithoutHint = luaBridge.AddExpWithoutHint--增加经验不提示
PreloadLua = luaBridge.PreloadLua --重新加载预加载的lua文件，一般用于调试hotfix使用

RestTeamPlus = luaBridge.RestTeamPlus--全队恢复（含计数器）
RestFightPlus = luaBridge.RestFightPlus--野外休息（含计数器）
RestPlus = luaBridge.RestPlus--休息（含计数器）

GetCurrentGameMapid = luaBridge.GetCurrentGameMapid--获取当前地图编号
RestTeam = luaBridge.RestTeam--全队恢复
GetTeamTotalLevel = luaBridge.GetTeamTotalLevel--获取队伍等级总和
AddHpWithoutHint = luaBridge.AddHpWithoutHint  --加血不提示
AddMpWithoutHint = luaBridge.AddMpWithoutHint --加内力不提示
AddLevelreturnUper = luaBridge.AddLevelreturnUper --加等级并返回实际增加的值
GetTeamMaxLevel = luaBridge.GetTeamMaxLevel--获取队伍最大等级
GetCurrentEventID = luaBridge.GetCurrentEventID--获取当前事件ID
JudgePointEventNum = luaBridge.JudgePointEventNum--判断指定触发器的交互事件
function main_getLuaFiles()
	return {}
end 