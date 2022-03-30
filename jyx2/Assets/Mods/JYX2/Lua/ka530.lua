if InTeam(51) == false then goto label0 end;
    Talk(51, "等一下！", "talkname51", 0);--对话显示在上方
::label0::
    if InTeam(51) == true then goto label1 end;
        Talk(51, "等一下！", "talkname51", 1);--对话显示在下方
::label1::
        DarkScence();
        ModifyEvent(-2, 20, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 21, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 22, 1, 1, -1, -1, -1, 6306, 6306, 6306, -2, -2, -2);--by fanyu 改变贴图 场景51-22
        jyx2_ReplaceSceneObject("", "NPC/慕容复", "1");--慕容复
        if InTeam(51) == true then goto label2 end;--慕容复不是队员，出现王语嫣
            ModifyEvent(-2, 23, 1, 1, -1, -1, -1, 6298, 6298, 6298, -2, -2, -2);--by fanyu 改变贴图 场景51-23
            jyx2_ReplaceSceneObject("", "NPC/王语嫣", "1");--王语嫣
			if JudgeScenePic(52, 3, 6310, 0, 14)==false then goto label3 end;--如果之前带段誉拜访过燕子坞，同时出现段誉. 否则跳转label3
                ModifyEvent(-2, 24, 1, 1, -1, -1, -1, 6314, 6314, 6314, -2, -2, -2);--by fanyu 改变贴图 场景51-24
				jyx2_ReplaceSceneObject("", "NPC/段誉", "1");--段誉出现
::label2::
::label3::
                if InTeam(76) == false then goto label4 end;--如果慕容复是队员，王语嫣和段誉根据是否是队员决定是否出现
                    ModifyEvent(-2, 23, 1, 1, -1, -1, -1, 6298, 6298, 6298, -2, -2, -2);--by fanyu 改变贴图 场景51-23
                    jyx2_ReplaceSceneObject("", "NPC/王语嫣", "1");--王语嫣
					if InTeam(53) == false then goto label5 end;
                        ModifyEvent(-2, 24, 1, 1, -1, -1, -1, 6314, 6314, 6314, -2, -2, -2);--by fanyu 改变贴图 场景51-24
                        jyx2_ReplaceSceneObject("", "NPC/段誉", "1");--段誉出现
::label4::
::label5::
                        LightScence();
                        Talk(0, "慕容公子，你要做什么？", "talkname0", 1);
                        Talk(51, "我决定今天要在武林同道面前揭发乔峰真实的身份……", "talkname51", 0);
                        Talk(0, "慕容公子，得饶人处且饶人。", "talkname0", 1);
                        Talk(51, "哼！你得到书了，而我呢？大燕复国的希望都在我身上……", "talkname51", 0);
                        Talk(0, "那，恕在下得罪了。", "talkname0", 1);
                        if TryBattle(85) == true then goto label6 end;
                            Dead();
                            do return end;
::label6::
                            LightScence();
                            Talk(0, "慕容公子，我不杀你，这件事还请你忘记，否则……", "talkname0", 1);
                            Talk(51, "哼！", "talkname51", 0);
                            AddRepute(3);
                            AddEthics(3);
                            DarkScence();
                            if InTeam(51) == false then goto label7 end;
                                Leave(51);
::label7::
                                ModifyEvent(-2, 22, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
								jyx2_ReplaceSceneObject("", "NPC/慕容复", "");--慕容复离开
                                ModifyEvent(52, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
								jyx2_ReplaceSceneObject("52", "NPC/慕容复", "");
                                LightScence();
                                if JudgeScenePic(-2, 23, 6298, 1, 0) then goto label8 end;--如果前面显示王语嫣，跳转label8
                                    do return end;
::label8::
                                    Talk(0, "王姑娘，你怎么还在这，你表哥已经走了。", "talkname0", 1);
                                    Talk(109, "唉！我表哥为了大燕复国之事，已经发疯了。在他一生之中，便是梦想要做大燕皇帝。这也难怪，因为他慕容氏世世代代，做的便是这个梦。他祖宗几十代做下来的梦，传到他身上，怎又能盼他觉醒呢？我表哥他本性并不坏，只不过为了想做大燕皇帝，行事才会变得如此不择手段……", "talkname109", 0);
                                    Talk(0, "可是你不是一直都喜欢着他吗……", "talkname0", 1);
                                    Talk(109, "在我表哥心中，复兴大业一直都是他心中最重要的事，儿女私情只不过……", "talkname109", 0);
                                    if JudgeScenePic(-2, 24, 6314, 46, 0) then goto label9 end;--如果上面显示段誉跳转label9
                                        Talk(0, "王姑娘，你别烦恼，或许过阵子你表哥就会想通了。", "talkname0", 1);
                                        Talk(109, "希望如此。那我先回燕子坞了。公子，告辞！", "talkname109", 0);
                                        DarkScence();
                                        if InTeam(76) == false then goto label10 end;
                                            Leave(76);
::label10::
                                            ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
											jyx2_ReplaceSceneObject("", "NPC/王语嫣", "");--王语嫣离开
                                            ModifyEvent(52, 2, 1, 1, 495, -1, -1, 6298, 6298, 6298, -2, -2, -2);--by fanyu 启动495脚本，改变贴图(王语嫣) 场景52-2
                                            jyx2_ReplaceSceneObject("52", "NPC/王语嫣", "1");--王语嫣出现
                                            LightScence();
                                            do return end;
::label9::
                                            Talk(53, "王姑娘，你别烦恼，我去劝劝你表哥，让他对你好一点……", "talkname53", 0);
                                            Talk(109, "段公子，我真是糊涂透顶，你一直待我这么好，我……我却……直到此刻我方才明白，这世上谁才是真的爱我，怜我的人…………", "talkname109", 0);
                                            Talk(0, "恭喜段兄，真心终于打动了美人芳心。不知二位今后有何打算？", "talkname0", 1);
                                            Talk(109, "我曾听段郎说，无量山洞中有一玉像，像极了我。我想先和段郎去那一游。", "talkname109", 0);
                                            Talk(0, "那，祝你们一路顺风了。", "talkname0", 1);
                                            Talk(53, "兄弟，你也保重。", "talkname53", 0);
                                            DarkScence();
                                            if InTeam(53) == false then goto label11 end;
                                                Leave(53);
::label11::
                                                if InTeam(76) == false then goto label12 end;
                                                    Leave(76);
::label12::
                                                    ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                                                    ModifyEvent(-2, 24, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
													jyx2_ReplaceSceneObject("", "NPC/王语嫣", "");--王语嫣离开
													jyx2_ReplaceSceneObject("", "NPC/段誉", "");--段誉离开
                                                    ModifyEvent(52, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                                                    ModifyEvent(52, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
													jyx2_ReplaceSceneObject("52", "NPC/王语嫣", "");
													jyx2_ReplaceSceneObject("52", "NPC/段誉", "");
                                                    ModifyEvent(42, 6, 1, 1, 594, -1, -1, 6296, 6296, 6296, -2, -2, -2);
                                                    ModifyEvent(42, 7, 1, 1, 593, -1, -1, 6308, 6308, 6308, -2, -2, -2);
                                                    jyx2_ReplaceSceneObject("42", "NPC/王语嫣", "1");--王语嫣出现
                                                    jyx2_ReplaceSceneObject("42", "NPC/段誉", "1");--段誉出现

                                                    LightScence();
                                                    Talk(0, "别人都已有情人终成眷属，而我呢？唉！别想这么多了，走吧！", "talkname0", 1);
                                                    AddEthics(5);
                                                    do return end;
do return end;
