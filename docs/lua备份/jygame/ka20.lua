Talk(0, "奇怪，頭怎麼這麼重．．莫非．．．", "talkname0", 1);
PlayAnimation(-1, 5994, 6012);
DarkScence();
ZeroAllMP();
ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
ModifyEvent(-2, 1, 1, -2, 21, -1, -1, 5168, 5168, 5168, 0, -2, -2);
jyx2_ReplaceSceneObject("","NPC/yanji","1") --刷出阎基

LightScence();
Talk(4, "原本預備對付苗人鳳的悲酥清風，居然先讓你受用了！", "talkname4", 0);
PlayAnimation(-1, 6026, 6036);
Talk(0, "你．．你．．", "talkname0", 1);
Talk(4, "江湖上最近盛傳有個年輕小毛頭到處找”十四天書”，想必就是你吧？找到幾本書啦？快交出來．", "talkname4", 0);
Talk(0, "你真卑鄙．", "talkname0", 1);
Talk(4, "我閻基做事向來只求結果，不問方法．怎麼？還不拿出來，要大爺我親自動手嗎？要知道書對死人是沒有意義的．", "talkname4", 0);
Talk(0, "倒要看看死的是你還是我．", "talkname0", 1);
if TryBattle(1) == false then goto label0 end;
    LightScence();
    Talk(4, "想不到少俠武功如此蓋世，連西夏的悲酥清風都對你沒有作用，”十四天書”的確該是少俠所有．", "talkname4", 0);
    Talk(0, "轉的倒是挺快的嘛，毛頭小子馬上就變成少俠，難怪這一點點武藝可以混到現在．", "talkname0", 1);
    Talk(4, "小的這兩下那能和少俠相比呢，混口飯吃罷了．對了，小的略通點醫術，少俠是否有什麼病痛，小的幫你看看．", "talkname4", 0);
    Talk(0, "我看是算了吧，給你看病，那豈不是讓黃鼠狼給雞把脈一樣的道理．", "talkname0", 1);
    Talk(4, "那兒的話，那少俠就隨便看看，若不嫌棄的話，有什麼喜歡的就拿去吧．", "talkname4", 0);
    ModifyEvent(-2, 7, -2, -2, -2, -2, 25, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -2, -2, -2, -1, -1, -1, -2, -2, -2);
    AddRepute(1);
    do return end;
::label0::
    Dead();
do return end;
