Talk(44, "你这小子，挺有两下子的，过得了我的万鳄潭，很好。咦！我看你手长足长，脑骨后秃，腰胁柔软，跟我非常像，真是学武的奇才。快磕头，求我收你为弟子。", "talkname44", 0);
Talk(0, "有没有搞错，看你呆头呆脑的，还要我拜你为师。我看你拜我为师还差不多。", "talkname0", 1);
Talk(44, "你这小子，还真倔强。好，你若打得过我，我岳老二就拜你为师。否则，你就拜我为师。", "talkname44", 0);
Talk(0, "奇怪，我怎么听人家说南海鳄神叫“岳老三”呢？", "talkname0", 1);
Talk(44, "是“岳老二”！", "talkname44", 0);
if TryBattle(90) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(44, "你这小子，资质果真不错，我没看错眼。来，快拜我岳老二为师。", "talkname44", 0);
    Talk(0, "岳老三，你自己说过的话都忘了吗？你说打输我要拜我为师的，怎么忘了呢？", "talkname0", 1);
    Talk(44, "拜就拜，我岳老二向来说话算话．你是我师父了。", "talkname44", 0);
    ModifyEvent(-2, -2, -2, -2, 569, -1, -1, -2, -2, -2, -2, -2, -2);
    AddRepute(3);
do return end;
