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
fieldIdx.MapScene = 3
fieldIdx.TransportToMap = 4
fieldIdx.InMusic = 5
fieldIdx.OutMusic = 6
fieldIdx.EnterCondition = 7
fieldIdx.Tags = 8
local data = {
{0,[[胡斐居]],[[00_hufeiju]],[[Leave:1000]],-1,0,1,""},
{1,[[河洛客栈]],[[01_heluokezhan]],[[Leave:1000]],2,19,0,""},
{2,[[云鹤崖]],[[02_yunheya]],[[Leave:1000]],-1,19,1,""},
{3,[[有间客栈]],[[03_youjiankezhan]],[[Leave:1000]],2,19,0,""},
{4,[[昆仑仙境]],[[04_kunlunxianjing]],[[Leave:67]],8,12,1,[[POINTLIGHT]]},
{5,[[闯王山洞]],[[05_shandong]],[[Leave:1000]],12,0,1,[[POINTLIGHT]]},
{6,[[北丑居]],[[06_beichouju]],[[Leave:1000]],11,19,1,""},
{7,[[神雕山洞]],[[07_shandong]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{8,[[大轮寺]],[[08_dalunsi]],[[Leave:1000]],4,0,1,""},
{9,[[成昆居]],[[09_chengkunju]],[[Leave:1000]],-1,0,1,""},
{10,[[蜘蛛山洞]],[[10_shandong]],[[Leave:1000]],12,19,1,[[POINTLIGHT]]},
{11,[[光明顶]],[[11_guangmingding]],[[Leave:1000]],10,0,1,""},
{12,[[明教分舵]],[[12_mingjiaofenduo]],[[Leave:1000,Leave2:13]],-1,0,1,""},
{13,[[明教地道]],[[13_mingjiaodidao]],[[Leave:12]],12,16,1,""},
{14,[[高昌迷宫]],[[14_gaochangmigong]],[[Leave:15]],12,16,1,[[POINTLIGHT]]},
{15,[[沙漠废墟]],[[15_shamofeixu]],[[Leave:1000,Leave2:14]],12,16,1,""},
{16,[[金轮寺]],[[16_jinlunsi]],[[Leave:1000]],4,0,1,""},
{17,[[回族部落]],[[17_huizubuluo]],[[Leave:1000]],4,16,1,""},
{18,[[古墓]],[[18_gumu]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{19,[[重阳宫]],[[19_chongyanggong]],[[Leave:1000]],22,0,1,""},
{20,[[百花谷]],[[20_baihuagu]],[[Leave:1000]],13,19,1,""},
{21,[[黑龙潭]],[[21_heilongtan]],[[Leave:1000]],8,16,1,[[NONAVAGENT]]},
{22,[[绝情谷]],[[22_jueqinggu]],[[Leave:1000]],-1,16,1,""},
{23,[[洪七公居]],[[23_hongqigongju]],[[Leave:1000]],-1,0,1,""},
{24,[[苗人凤居]],[[24_miaorenfengju]],[[Leave:1000]],-1,16,1,""},
{25,[[武道大会]],[[25_wudaodahui]],[[Leave:1000]],15,19,1,""},
{26,[[黑木崖]],[[26_heimuya]],[[Leave:1000]],-1,16,1,""},
{27,[[嵩山派]],[[27_songshanpai]],[[Leave:1000]],-1,0,1,""},
{28,[[少林寺]],[[28_shaolinsi]],[[Leave:1000]],20,0,1,""},
{29,[[泰山派]],[[29_taishanpai]],[[Leave:1000]],22,0,1,""},
{30,[[平一指居]],[[30_pingyizhiju]],[[Leave:1000]],-1,0,1,""},
{31,[[恒山派]],[[31_hengshanpai]],[[Leave:1000]],21,0,1,""},
{32,[[海边小屋]],[[32_haibianxiaowu]],[[Leave:1000]],1,19,1,""},
{33,[[峨嵋派]],[[33_emeipai]],[[Leave:1000]],23,0,1,""},
{34,[[崆峒派]],[[34_kongtongpai]],[[Leave:1000]],-1,0,1,""},
{35,[[星宿海]],[[35_xingxiuhai]],[[Leave:1000]],4,0,1,""},
{36,[[青城派]],[[36_qingchengpai]],[[Leave:1000]],-1,16,1,""},
{37,[[五毒教]],[[37_wudujiao]],[[Leave:1000]],-1,19,1,""},
{38,[[摩天崖]],[[38_motianya]],[[Leave:1000]],-1,16,1,""},
{39,[[凌霄城]],[[39_lingxiaocheng]],[[Leave:1000]],4,0,1,""},
{40,[[悦来客栈]],[[40_yuelaikezhan]],[[Leave:1000]],2,19,0,""},
{41,[[神秘山洞]],[[41_shandong]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{42,[[无量山洞]],[[42_wuliangshandong]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{43,[[武当派]],[[43_wudangpai]],[[Leave:1000]],22,0,1,""},
{44,[[蝴蝶谷]],[[44_hudiegu]],[[Leave:1000]],-1,16,1,""},
{45,[[程英居]],[[45_chengyingju]],[[Leave:1000]],8,19,1,""},
{46,[[金蛇山洞]],[[46_jinsheshandong]],[[Leave:1000]],12,0,1,[[POINTLIGHT]]},
{47,[[一灯居]],[[47_yidengju]],[[Leave:1000]],8,16,1,""},
{48,[[铁掌山]],[[48_tiezhangshan]],[[Leave:1000]],-1,0,1,""},
{49,[[药王庄]],[[49_yaowangzhuang]],[[Leave:1000]],1,16,1,""},
{50,[[阎基居]],[[50_yanjiju]],[[Leave:1000]],1,19,1,""},
{51,[[丐帮]],[[51_gaibang]],[[Leave:1000]],-1,16,1,""},
{52,[[燕子坞]],[[52_yanziwu]],[[Leave:1000]],1,0,1,""},
{53,[[擂鼓山]],[[53_leigushan]],[[Leave:1000]],-1,16,1,""},
{54,[[薛慕华居]],[[54_xuemuhuaju]],[[Leave:1000]],-1,0,1,""},
{55,[[梅庄]],[[55_meizhuang]],[[Leave:1000,Leave2:82]],1,0,1,""},
{56,[[福威镖局]],[[56_fuweibiaoju]],[[Leave:1000]],9,16,1,""},
{57,[[华山派]],[[57_huashanpai]],[[Leave:1000]],-1,0,1,""},
{58,[[衡山派]],[[58_hengshanpai]],[[Leave:1000]],-1,0,1,""},
{59,[[田伯光居]],[[59_tianboguangju]],[[Leave:1000]],-1,0,1,""},
{60,[[龙门客栈]],[[60_longmenkezhan]],[[Leave:1000]],2,19,0,""},
{61,[[高升客栈]],[[61_gaoshengkezhan]],[[Leave:1000]],2,19,0,""},
{62,[[破庙]],[[62_pomiao]],[[Leave:1000]],-1,0,1,""},
{63,[[天宁寺]],[[63_tianningsi]],[[Leave:1000]],-1,0,1,""},
{64,[[南贤居]],[[64_nanxianju]],[[Leave:1000]],11,0,0,""},
{65,[[唐诗山洞]],[[65_shandong]],[[Leave:1000]],12,19,1,[[POINTLIGHT]]},
{66,[[冰蚕山洞]],[[66_shandong]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{67,[[昆仑山洞]],[[67_shandong]],[[Leave:1000,Leave2:4]],12,16,1,[[POINTLIGHT]]},
{68,[[昆仑派]],[[68_kunlunpai]],[[Leave:1000]],4,0,1,""},
{69,[[白驼山]],[[69_baituoshan]],[[Leave:1000]],4,0,1,""},
{70,[[小虾米居]],[[70_xiaoxiamiju]],[[Leave:1000]],-1,19,0,[[START:0]]},
{71,[[神龙教]],[[71_shenlongjiao]],[[Leave:1000]],-1,0,1,""},
{72,[[冰火岛]],[[72_binghuodao]],[[Leave:1000]],12,16,1,[[POINTLIGHT]]},
{73,[[灵蛇岛]],[[73_lingshedao]],[[Leave:1000]],-1,0,1,""},
{74,[[侠客岛]],[[74_xiakedao]],[[Leave:1000]],12,16,1,""},
{75,[[桃花岛]],[[75_taohuadao]],[[Leave:1000]],-1,0,1,""},
{76,[[霹雳堂]],[[76_pilitang]],[[Leave:1000,Leave2:83]],-1,19,1,""},
{77,[[万鳄岛]],[[77_wanedao]],[[Leave:1000]],-1,19,1,""},
{78,[[渤泥岛]],[[78_bonidao]],[[Leave:1000]],-1,0,1,""},
{79,[[鸳鸯山洞]],[[79_shandong]],[[Leave:1000]],12,19,1,""},
{80,[[绝情谷底]],[[80_jueqinggudi]],[[Leave:1000]],9,16,1,""},
{81,[[思过崖]],[[81_siguoya]],[[Leave:1000]],16,19,1,""},
{82,[[梅庄地牢]],[[82_meizhuangdilao]],[[Leave:55]],12,1,1,""},
{83,[[圣堂]],[[83_shengtang]],[[Leave:76]],17,19,1,[[POINTLIGHT]]},
{1000,[[大地图]],[[1000_daditu]],[[-1]],19,-1,0,[[WORLDMAP]]},}
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
configMgr:AddConfigTable([[Map]], data)