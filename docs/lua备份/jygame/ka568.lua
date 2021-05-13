Talk(44, "你這小子，挺有兩下子的，過的了我的萬鱷潭，很好．咦！我看你手長足長，腦骨後禿，腰脅柔軟，跟我非常像，真是學武的奇才．快磕頭，求我收你為弟子．", "talkname44", 0);
Talk(0, "有沒有搞錯，看你呆頭呆腦的，還要我拜你為師．我看你拜我為師還差不多．", "talkname0", 1);
Talk(44, "你這小子，還真倔強．好，你若打的過我，我岳老二就拜你為師．否則，你就拜我為師．", "talkname44", 0);
Talk(0, "奇怪，我怎麼聽人家說南海鱷神叫”岳老三”呢？", "talkname0", 1);
Talk(44, "是”岳老二”！", "talkname44", 0);
if TryBattle(90) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(44, "你這小子，資質果真不錯，我沒看錯眼．來，快拜我岳老二為師．", "talkname44", 0);
    Talk(0, "岳老三，你自己說過的話都忘了嗎？你說打輸我要拜我為師的，怎麼忘了呢？", "talkname0", 1);
    Talk(44, "拜就拜，我岳老二向來說話算話．你是我師父了．", "talkname44", 0);
    ModifyEvent(-2, -2, -2, -2, 569, -1, -1, -2, -2, -2, -2, -2, -2);
    AddRepute(3);
do return end;
