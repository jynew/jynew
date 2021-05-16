Talk(0, "看你的樣子，妳大概就是金花婆婆吧．快放了王前輩，否則要你吃不完兜著走．", "talkname0", 1);
Talk(15, "胡青牛老眼昏花了是不是，叫了個小子來送死．小子，我看你年紀輕輕的，快走吧，別自討苦吃了．", "talkname15", 0);
Talk(0, "你活生生的拆散人家夫妻，我就是看不慣，這檔事我管定了．", "talkname0", 1);
Talk(15, "本來想這不關你的事，你非要管，我就讓你這不知天高地厚的小子吃點苦頭．", "talkname15", 0);
if TryBattle(14) == false then goto label0 end;
    ModifyEvent(-2, -2, -2, -2, 100, -1, -1, -2, -2, -2, -2, -2, -2);
    SetScenceMap(-2, 1, 21, 17, 0);
    LightScence();
    Talk(0, "老婆婆，我想請問你天有多高，地有多厚啊？我真的不知道耶！", "talkname0", 1);
    Talk(15, "哼！", "talkname15", 0);
    AddRepute(3);
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 102, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(15, "看你資質挺好的，老婆婆我不想殺你，你走吧．", "talkname15", 0);
do return end;
