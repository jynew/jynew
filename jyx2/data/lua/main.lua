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

--0=0|1|0|0|0|
--NOTHING TO DO
--1=1|4|0|0|0|Talk(#1, "talk(#0)", "talkname#1", #2); --对话
Talk = CS.Jyx2.Jyx2LuaBridge.Talk
--2=2|3|0|0|0|AddItemWithoutHint(#0, #1); --获得物品,不显示提示
AddItemWithoutHint = CS.Jyx2.Jyx2LuaBridge.AddItemWithoutHint
--3=3|14|0|0|0|ModifyEvent(#0, #1, #2, #3, #4, #5, #6, #7, #8, #9, #a, #b, #c); --修改地图事件
ModifyEvent = CS.Jyx2.Jyx2LuaBridge.ModifyEvent
--4=4|4|1|2|3|if UseItem(#0) == bool then goto label#x end;
UseItem = CS.Jyx2.Jyx2LuaBridge.UseItem
--5=5|3|1|1|2|if AskBattle() == bool then goto label#x end; --询问是否战斗
AskBattle = CS.Jyx2.Jyx2LuaBridge.AskBattle
--6=6|5|1|2|3|if TryBattle(#0) == bool then goto label#x end; --战斗
TryBattle = CS.Jyx2.Jyx2LuaBridge.TryBattle
--7=7|1|0|0|0|do return end;
--NOTHING TO DO
--8=8|2|0|0|0|ChangeMMapMusic(#0); --改变大地图音乐
ChangeMMapMusic = CS.Jyx2.Jyx2LuaBridge.ChangeMMapMusic
--9=9|3|1|1|2|if AskJoin () == bool then goto label#x end; --询问角色是否加入
AskJoin = CS.Jyx2.Jyx2LuaBridge.AskJoin
--10=a|2|0|0|0|Join(#0); --角色加入
Join = CS.Jyx2.Jyx2LuaBridge.Join
--11=b|3|1|1|2|if AskRest() == bool then goto  label#x end; --询问是否进行休息
AskRest = CS.Jyx2.Jyx2LuaBridge.AskRest
--12=c|1|0|0|0|Rest(); --休息
Rest = CS.Jyx2.Jyx2LuaBridge.Rest
--13=d|1|0|0|0|LightScence(); --场景变亮
LightScence = CS.Jyx2.Jyx2LuaBridge.LightScence
--14=e|1|0|0|0|DarkScence(); --场景变暗
DarkScence = CS.Jyx2.Jyx2LuaBridge.DarkScence
--15=f|2|0|0|0|Dead(); --死亡（gameover）
Dead = CS.Jyx2.Jyx2LuaBridge.Dead
--16=10|4|1|2|3|if InTeam(#0) == bool then goto label#x end; --判断是否在队伍里
InTeam = CS.Jyx2.Jyx2LuaBridge.InTeam
--17=11|6|0|0|0|SetScenceMap(#0, #1, #2, #3, #4); --改变地图要素
SetScenceMap = CS.Jyx2.Jyx2LuaBridge.SetScenceMap
--18=12|4|1|2|3|if HaveItem(#0) == bool then goto label#x end; --判断是否拥有道具
HaveItem = CS.Jyx2.Jyx2LuaBridge.HaveItem
--19=13|3|0|0|0|SetScencePosition2(#0, #1);
SetScencePosition2 = CS.Jyx2.Jyx2LuaBridge.SetScencePosition2
--20=14|3|1|1|2|if TeamIsFull() == bool then goto label#x end; --判断队伍是否满员
TeamIsFull = CS.Jyx2.Jyx2LuaBridge.TeamIsFull
--21=15|2|0|0|0|Leave(#0); --离队
Leave = CS.Jyx2.Jyx2LuaBridge.Leave
--22=16|1|0|0|0|ZeroAllMP(); --全队清空内力
ZeroAllMP = CS.Jyx2.Jyx2LuaBridge.ZeroAllMP
--23=17|3|0|0|0|SetOneUsePoi(#0, #1);  --设置用毒能力
SetOneUsePoi = CS.Jyx2.Jyx2LuaBridge.SetOneUsePoi
--24=18|1|0|0|0|
--NOTHING TO DO
--25=19|5|0|0|0|ScenceFromTo(#0, #1, #2, #3); --移动视角
ScenceFromTo = CS.Jyx2.Jyx2LuaBridge.ScenceFromTo
--26=1a|6|0|0|0|Add3EventNum(#0, #1, #2, #3, #5) --修改事件
Add3EventNum = CS.Jyx2.Jyx2LuaBridge.Add3EventNum
--27=1b|4|0|0|0|PlayAnimation(#0, #1, #2); --播放动画
PlayAnimation = CS.Jyx2.Jyx2LuaBridge.PlayAnimation
--28=1c|6|1|4|5|if JudgeEthics(#0, #1, #2) == bool then goto label#x end; --判断道德
JudgeEthics = CS.Jyx2.Jyx2LuaBridge.JudgeEthics
--29=1d|6|1|4|5|if JudgeAttack(#0, #1, #2) == bool then goto label#x end; --判断攻击力
JudgeAttack = CS.Jyx2.Jyx2LuaBridge.JudgeAttack
--30=1e|5|0|0|0|WalkFromTo(#0, #1, #2, #3); --主角行走
WalkFromTo = CS.Jyx2.Jyx2LuaBridge.WalkFromTo
--31=1f|4|1|2|3|if JudgeMoney(#0) == bool then goto label#x end; --判断金钱
JudgeMoney = CS.Jyx2.Jyx2LuaBridge.JudgeMoney
--32=20|3|0|0|0|AddItem(#0, #1); --添加道具，显示提示
AddItem = CS.Jyx2.Jyx2LuaBridge.AddItem
--33=21|4|0|0|0|LearnMagic2(#0, #1, #2); --学会武功
LearnMagic2 = CS.Jyx2.Jyx2LuaBridge.LearnMagic2
--34=22|3|0|0|0|AddAptitude(#0, #1);  --增加资质
AddAptitude = CS.Jyx2.Jyx2LuaBridge.AddAptitude
--35=23|5|0|0|0|SetOneMagic(#0, #1, #2, #3); --设置角色的武功
SetOneMagic = CS.Jyx2.Jyx2LuaBridge.SetOneMagic
--36=24|4|1|2|3|if JudgeSexual(#0) == bool then goto label#x end; --判断性别
JudgeSexual = CS.Jyx2.Jyx2LuaBridge.JudgeSexual
--37=25|2|0|0|0|AddEthics(#0); --增加道德
AddEthics = CS.Jyx2.Jyx2LuaBridge.AddEthics
--38=26|5|0|0|0|ChangeScencePic(#0, #1, #2, #3); --换场景贴图
ChangeScencePic = CS.Jyx2.Jyx2LuaBridge.ChangeScencePic
--39=27|2|0|0|0|OpenScene(#0); 
OpenScene = CS.Jyx2.Jyx2LuaBridge.OpenScene
--40=28|2|0|0|0|SetRoleFace(#0);
SetRoleFace = CS.Jyx2.Jyx2LuaBridge.SetRoleFace
--41=29|4|0|0|0|NPCGetItem(#0, #1, #2); --NPC角色获得道具
NPCAddItem = CS.Jyx2.Jyx2LuaBridge.NPCGetItem
--42=2a|3|1|1|2|if JudgeFemaleInTeam() == bool then goto label#x end; --判断队伍里是否有女性
JudgeFemaleInTeam = CS.Jyx2.Jyx2LuaBridge.JudgeFemaleInTeam
--43=2b|4|1|2|3|if HaveItem(#0) == bool then goto label#x end; --判断是否拥有道具
--ALREADY DEFINED
--44=2c|7|0|0|0|Play2Amination(#0, #1, #2, #3, #4, #5, #6);
Play2Amination = CS.Jyx2.Jyx2LuaBridge.Play2Amination
--45=2d|3|0|0|0|AddSpeed(#0, #1);
AddSpeed = CS.Jyx2.Jyx2LuaBridge.AddSpeed --加速度
--46=2e|3|0|0|0|AddMP(#0, #1);
AddMP = CS.Jyx2.Jyx2LuaBridge.AddMaxMp --加内力
--47=2f|3|0|0|0|AddAttack(#0, #1);
AddAttack = CS.Jyx2.Jyx2LuaBridge.AddAttack --加攻击力
--48=30|3|0|0|0|AddHP(#0, #1);
AddHp = CS.Jyx2.Jyx2LuaBridge.AddHp  --加血
--49=31|3|0|0|0|SetPersonMPPro(#0,#1);
SetPersonMPPro = CS.Jyx2.Jyx2LuaBridge.SetPersonMPPro
--50=32|8|0|0|0|instruct_50(#0, #1, #2, #3, #4, #5, #6);
instruct_50 = CS.Jyx2.Jyx2LuaBridge.instruct_50
--51=33|1|0|0|0|AskSoftStar();
AskSoftStar = CS.Jyx2.Jyx2LuaBridge.AskSoftStar
--52=34|1|0|0|0|ShowEthics();
ShowEthics = CS.Jyx2.Jyx2LuaBridge.ShowEthics
--53=35|1|0|0|0|ShowRepute();
ShowRepute = CS.Jyx2.Jyx2LuaBridge.ShowRepute
--54=36|1|0|0|0|OpenAllScene();
OpenAllScene = CS.Jyx2.Jyx2LuaBridge.OpenAllScene
--55=37|5|1|3|4|if JudgeEventNum(#0, #1) == bool then goto label#x end;
JudgeEventNum = CS.Jyx2.Jyx2LuaBridge.JudgeEventNum
--56=38|2|0|0|0|AddRepute(#0);  --增加声望
AddRepute = CS.Jyx2.Jyx2LuaBridge.AddRepute
--57=39|1|0|0|0|instruct_57();
instruct_57 = CS.Jyx2.Jyx2LuaBridge.instruct_57
--58=3a|1|0|0|0|FightForTop();
FightForTop = CS.Jyx2.Jyx2LuaBridge.FightForTop
--59=3b|1|0|0|0|AllLeave();
AllLeave = CS.Jyx2.Jyx2LuaBridge.AllLeave
--60=3c|6|1|4|5|JudgeScenePic(#0, #1, #2, #3, #4);
JudgeScenePic = CS.Jyx2.Jyx2LuaBridge.JudgeScenePic
--61=3d|3|1|1|2|if Judge14BooksPlaced() == bool then goto label#x end; --这个写法太蠢了，待调整
Judge14BooksPlaced = CS.Jyx2.Jyx2LuaBridge.Judge14BooksPlaced
--62=3e|7|0|0|0|EndAmination(#0, #1, #2, #3, #4, #5, #6); --结局动画
EndAmination = CS.Jyx2.Jyx2LuaBridge.EndAmination
--63=3f|3|0|0|0|SetSexual(#0, #1); --只有一处，用别的方法比较好
SetSexual = CS.Jyx2.Jyx2LuaBridge.SetSexual
--64=40|1|0|0|0|WeiShop();
WeiShop = CS.Jyx2.Jyx2LuaBridge.WeiShop
--65=41|1|0|0|0|
--NOTHING TO DO
--66=42|2|0|0|0|PlayMusic(#0);
PlayMusic = CS.Jyx2.Jyx2LuaBridge.PlayMusic 
--67=43|2|0|0|0|PlayWave(#0);
PlayWave = CS.Jyx2.Jyx2LuaBridge.PlayWave
--68=44|8|0|0|0|Talk(#0, "talk(#1)", "talk(#2)", #3, #4, #5);
--ALREADY DEFINED
--69=45|4|0|0|0|ResetName(#0, #1, #2);
--没有调用
--70=46|3|0|0|0|ShowTitle("talk(#0)", #1);
--没有调用
--71=47|4|0|0|0|JumpScence(#0, #1, #2);
--没有调用
--72=48|6|0|0|0|SetAttribute(#0, #1, #2, #3, #4);
--没有调用


--扩展函数
jyx2_ReplaceSceneObject = CS.Jyx2.Jyx2LuaBridge.jyx2_ReplaceSceneObject
jyx2_MovePlayer = CS.Jyx2.Jyx2LuaBridge.jyx2_MovePlayer
jyx2_CameraFollow = CS.Jyx2.Jyx2LuaBridge.jyx2_CameraFollow
jyx2_CameraFollowPlayer = CS.Jyx2.Jyx2LuaBridge.jyx2_CameraFollowPlayer
jyx2_WalkFromTo = CS.Jyx2.Jyx2LuaBridge.jyx2_WalkFromTo
jyx2_PlayTimeline = CS.Jyx2.Jyx2LuaBridge.jyx2_PlayTimeline
jyx2_StopTimeline = CS.Jyx2.Jyx2LuaBridge.jyx2_StopTimeline
jyx2_Wait = CS.Jyx2.Jyx2LuaBridge.jyx2_Wait
jyx2_SwitchRoleAnimation = CS.Jyx2.Jyx2LuaBridge.jyx2_SwitchRoleAnimation
jyx2_FixMapObject = CS.Jyx2.Jyx2LuaBridge.jyx2_FixMapObject
jyx2_CheckEventCount = CS.Jyx2.Jyx2LuaBridge.jyx2_CheckEventCount
jyx2_CheckBookAndRepute=CS.Jyx2.Jyx2LuaBridge.jyx2_CheckBookAndRepute
jyx2_SetTimelineSpeed = CS.Jyx2.Jyx2LuaBridge.jyx2_SetTimelineSpeed
jyx2_PlayTimelineSimple = CS.Jyx2.Jyx2LuaBridge.jyx2_PlayTimelineSimple
function main_getLuaFiles()
	return {}
end 