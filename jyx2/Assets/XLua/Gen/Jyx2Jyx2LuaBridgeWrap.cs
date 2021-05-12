#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class Jyx2Jyx2LuaBridgeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Jyx2.Jyx2LuaBridge);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 68, 1, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Talk", _m_Talk_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetItem", _m_GetItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ModifyEvent", _m_ModifyEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AskBattle", _m_AskBattle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TryBattle", _m_TryBattle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ChangeMMapMusic", _m_ChangeMMapMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AskJoin", _m_AskJoin_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Join", _m_Join_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Dead", _m_Dead_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HaveItem", _m_HaveItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UseItem", _m_UseItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Leave", _m_Leave_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ZeroAllMP", _m_ZeroAllMP_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetOneUsePoi", _m_SetOneUsePoi_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ScenceFromTo", _m_ScenceFromTo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Add3EventNum", _m_Add3EventNum_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "InTeam", _m_InTeam_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TeamIsFull", _m_TeamIsFull_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetScenceMap", _m_SetScenceMap_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddEthics", _m_AddEthics_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ChangeScencePic", _m_ChangeScencePic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PlayAnimation", _m_PlayAnimation_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeEthics", _m_JudgeEthics_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeAttack", _m_JudgeAttack_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WalkFromTo", _m_WalkFromTo_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LearnMagic2", _m_LearnMagic2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddAptitude", _m_AddAptitude_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetOneMagic", _m_SetOneMagic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeSexual", _m_JudgeSexual_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeFemaleInTeam", _m_JudgeFemaleInTeam_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Play2Amination", _m_Play2Amination_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddSpeed", _m_AddSpeed_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddMaxMp", _m_AddMaxMp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddAttack", _m_AddAttack_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddHp", _m_AddHp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetPersonMPPro", _m_SetPersonMPPro_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "instruct_50", _m_instruct_50_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShowEthics", _m_ShowEthics_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShowRepute", _m_ShowRepute_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeEventNum", _m_JudgeEventNum_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "OpenAllScence", _m_OpenAllScence_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FightForTop", _m_FightForTop_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AllLeave", _m_AllLeave_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeScencePic", _m_JudgeScencePic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Judge14BooksPlaced", _m_Judge14BooksPlaced_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EndAmination", _m_EndAmination_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetSexual", _m_SetSexual_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PlayMusic", _m_PlayMusic_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "OpenScence", _m_OpenScence_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetRoleFace", _m_SetRoleFace_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "NPCGetItem", _m_NPCGetItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "PlayWave", _m_PlayWave_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AskRest", _m_AskRest_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DarkScence", _m_DarkScence_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Rest", _m_Rest_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LightScence", _m_LightScence_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "JudgeMoney", _m_JudgeMoney_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddItem", _m_AddItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetScencePosition2", _m_SetScencePosition2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddRepute", _m_AddRepute_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "WeiShop", _m_WeiShop_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AskSoftStar", _m_AskSoftStar_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "instruct_57", _m_instruct_57_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "jyx2_ReplaceSceneObject", _m_jyx2_ReplaceSceneObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "jyx2_CameraFollow", _m_jyx2_CameraFollow_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "jyx2_CameraFollowPlayer", _m_jyx2_CameraFollowPlayer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "jyx2_WalkFromTo", _m_jyx2_WalkFromTo_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "isQuickBattle", _g_get_isQuickBattle);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "isQuickBattle", _s_set_isQuickBattle);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Jyx2.Jyx2LuaBridge does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Talk_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    string _content = LuaAPI.lua_tostring(L, 2);
                    string _talkName = LuaAPI.lua_tostring(L, 3);
                    int _type = LuaAPI.xlua_tointeger(L, 4);
                    
                    Jyx2.Jyx2LuaBridge.Talk( _roleId, _content, _talkName, _type );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _itemId = LuaAPI.xlua_tointeger(L, 1);
                    int _count = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.GetItem( _itemId, _count );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ModifyEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _scene = LuaAPI.xlua_tointeger(L, 1);
                    int _eventId = LuaAPI.xlua_tointeger(L, 2);
                    int _canPass = LuaAPI.xlua_tointeger(L, 3);
                    int _changeToEventId = LuaAPI.xlua_tointeger(L, 4);
                    int _interactiveEventId = LuaAPI.xlua_tointeger(L, 5);
                    int _useItemEventId = LuaAPI.xlua_tointeger(L, 6);
                    int _enterEventId = LuaAPI.xlua_tointeger(L, 7);
                    int _p7 = LuaAPI.xlua_tointeger(L, 8);
                    int _p8 = LuaAPI.xlua_tointeger(L, 9);
                    int _p9 = LuaAPI.xlua_tointeger(L, 10);
                    int _p10 = LuaAPI.xlua_tointeger(L, 11);
                    int _p11 = LuaAPI.xlua_tointeger(L, 12);
                    int _p12 = LuaAPI.xlua_tointeger(L, 13);
                    
                    Jyx2.Jyx2LuaBridge.ModifyEvent( _scene, _eventId, _canPass, _changeToEventId, _interactiveEventId, _useItemEventId, _enterEventId, _p7, _p8, _p9, _p10, _p11, _p12 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AskBattle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.AskBattle(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TryBattle_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _battleId = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.TryBattle( _battleId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ChangeMMapMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _musicId = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.ChangeMMapMusic( _musicId );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AskJoin_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.AskJoin(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Join_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.Join( _roleId );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Dead_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.Dead(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HaveItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _itemId = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.HaveItem( _itemId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UseItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _itemId = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.UseItem( _itemId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Leave_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.Leave( _roleId );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ZeroAllMP_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.ZeroAllMP(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetOneUsePoi_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _v = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.SetOneUsePoi( _roleId, _v );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ScenceFromTo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _x = LuaAPI.xlua_tointeger(L, 1);
                    int _y = LuaAPI.xlua_tointeger(L, 2);
                    int _x2 = LuaAPI.xlua_tointeger(L, 3);
                    int _y2 = LuaAPI.xlua_tointeger(L, 4);
                    
                    Jyx2.Jyx2LuaBridge.ScenceFromTo( _x, _y, _x2, _y2 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Add3EventNum_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _scene = LuaAPI.xlua_tointeger(L, 1);
                    int _eventId = LuaAPI.xlua_tointeger(L, 2);
                    int _v1 = LuaAPI.xlua_tointeger(L, 3);
                    int _v2 = LuaAPI.xlua_tointeger(L, 4);
                    int _v3 = LuaAPI.xlua_tointeger(L, 5);
                    
                    Jyx2.Jyx2LuaBridge.Add3EventNum( _scene, _eventId, _v1, _v2, _v3 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InTeam_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.InTeam( _roleId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TeamIsFull_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.TeamIsFull(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetScenceMap_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sceneId = LuaAPI.xlua_tointeger(L, 1);
                    int _layer = LuaAPI.xlua_tointeger(L, 2);
                    int _x = LuaAPI.xlua_tointeger(L, 3);
                    int _y = LuaAPI.xlua_tointeger(L, 4);
                    int _v = LuaAPI.xlua_tointeger(L, 5);
                    
                    Jyx2.Jyx2LuaBridge.SetScenceMap( _sceneId, _layer, _x, _y, _v );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddEthics_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _add = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.AddEthics( _add );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ChangeScencePic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _p1 = LuaAPI.xlua_tointeger(L, 1);
                    int _p2 = LuaAPI.xlua_tointeger(L, 2);
                    int _p3 = LuaAPI.xlua_tointeger(L, 3);
                    int _p4 = LuaAPI.xlua_tointeger(L, 4);
                    
                    Jyx2.Jyx2LuaBridge.ChangeScencePic( _p1, _p2, _p3, _p4 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayAnimation_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _p1 = LuaAPI.xlua_tointeger(L, 1);
                    int _p2 = LuaAPI.xlua_tointeger(L, 2);
                    int _p3 = LuaAPI.xlua_tointeger(L, 3);
                    
                    Jyx2.Jyx2LuaBridge.PlayAnimation( _p1, _p2, _p3 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeEthics_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _low = LuaAPI.xlua_tointeger(L, 2);
                    int _high = LuaAPI.xlua_tointeger(L, 3);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeEthics( _roleId, _low, _high );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeAttack_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _low = LuaAPI.xlua_tointeger(L, 2);
                    int _high = LuaAPI.xlua_tointeger(L, 3);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeAttack( _roleId, _low, _high );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WalkFromTo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _x1 = LuaAPI.xlua_tointeger(L, 1);
                    int _y1 = LuaAPI.xlua_tointeger(L, 2);
                    int _x2 = LuaAPI.xlua_tointeger(L, 3);
                    int _y2 = LuaAPI.xlua_tointeger(L, 4);
                    
                    Jyx2.Jyx2LuaBridge.WalkFromTo( _x1, _y1, _x2, _y2 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LearnMagic2_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _magicId = LuaAPI.xlua_tointeger(L, 2);
                    int _noDisplay = LuaAPI.xlua_tointeger(L, 3);
                    
                    Jyx2.Jyx2LuaBridge.LearnMagic2( _roleId, _magicId, _noDisplay );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddAptitude_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _v = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddAptitude( _roleId, _v );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetOneMagic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _magicIndexRole = LuaAPI.xlua_tointeger(L, 2);
                    int _magicId = LuaAPI.xlua_tointeger(L, 3);
                    int _level = LuaAPI.xlua_tointeger(L, 4);
                    
                    Jyx2.Jyx2LuaBridge.SetOneMagic( _roleId, _magicIndexRole, _magicId, _level );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeSexual_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sexual = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeSexual( _sexual );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeFemaleInTeam_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeFemaleInTeam(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Play2Amination_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _eventIndex1 = LuaAPI.xlua_tointeger(L, 1);
                    int _beginPic1 = LuaAPI.xlua_tointeger(L, 2);
                    int _endPic1 = LuaAPI.xlua_tointeger(L, 3);
                    int _eventIndex2 = LuaAPI.xlua_tointeger(L, 4);
                    int _beginPic2 = LuaAPI.xlua_tointeger(L, 5);
                    int _endPic2 = LuaAPI.xlua_tointeger(L, 6);
                    
                    Jyx2.Jyx2LuaBridge.Play2Amination( _eventIndex1, _beginPic1, _endPic1, _eventIndex2, _beginPic2, _endPic2 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddSpeed_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddSpeed( _roleId, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddMaxMp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddMaxMp( _roleId, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddAttack_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddAttack( _roleId, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddHp_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddHp( _roleId, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetPersonMPPro_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.SetPersonMPPro( _roleId, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_instruct_50_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _p1 = LuaAPI.xlua_tointeger(L, 1);
                    int _p2 = LuaAPI.xlua_tointeger(L, 2);
                    int _p3 = LuaAPI.xlua_tointeger(L, 3);
                    int _p4 = LuaAPI.xlua_tointeger(L, 4);
                    int _p5 = LuaAPI.xlua_tointeger(L, 5);
                    int _p6 = LuaAPI.xlua_tointeger(L, 6);
                    int _p7 = LuaAPI.xlua_tointeger(L, 7);
                    
                    Jyx2.Jyx2LuaBridge.instruct_50( _p1, _p2, _p3, _p4, _p5, _p6, _p7 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowEthics_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.ShowEthics(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowRepute_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.ShowRepute(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeEventNum_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _eventIndex = LuaAPI.xlua_tointeger(L, 1);
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeEventNum( _eventIndex, _value );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenAllScence_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.OpenAllScence(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FightForTop_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.FightForTop(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AllLeave_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.AllLeave(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeScencePic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sceneId = LuaAPI.xlua_tointeger(L, 1);
                    int _eventId = LuaAPI.xlua_tointeger(L, 2);
                    int _pic = LuaAPI.xlua_tointeger(L, 3);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeScencePic( _sceneId, _eventId, _pic );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Judge14BooksPlaced_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.Judge14BooksPlaced(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EndAmination_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _p1 = LuaAPI.xlua_tointeger(L, 1);
                    int _p2 = LuaAPI.xlua_tointeger(L, 2);
                    int _p3 = LuaAPI.xlua_tointeger(L, 3);
                    int _p4 = LuaAPI.xlua_tointeger(L, 4);
                    int _p5 = LuaAPI.xlua_tointeger(L, 5);
                    int _p6 = LuaAPI.xlua_tointeger(L, 6);
                    int _p7 = LuaAPI.xlua_tointeger(L, 7);
                    
                    Jyx2.Jyx2LuaBridge.EndAmination( _p1, _p2, _p3, _p4, _p5, _p6, _p7 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetSexual_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _sexual = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.SetSexual( _roleId, _sexual );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayMusic_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.PlayMusic( _id );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenScence_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _sceneId = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.OpenScence( _sceneId );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetRoleFace_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _dir = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.SetRoleFace( _dir );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_NPCGetItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _roleId = LuaAPI.xlua_tointeger(L, 1);
                    int _itemId = LuaAPI.xlua_tointeger(L, 2);
                    int _count = LuaAPI.xlua_tointeger(L, 3);
                    
                    Jyx2.Jyx2LuaBridge.NPCGetItem( _roleId, _itemId, _count );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayWave_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _waveIndex = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.PlayWave( _waveIndex );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AskRest_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.AskRest(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DarkScence_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.DarkScence(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Rest_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.Rest(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LightScence_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.LightScence(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_JudgeMoney_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _money = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool gen_ret = Jyx2.Jyx2LuaBridge.JudgeMoney( _money );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _itemId = LuaAPI.xlua_tointeger(L, 1);
                    int _count = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.AddItem( _itemId, _count );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetScencePosition2_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _x = LuaAPI.xlua_tointeger(L, 1);
                    int _y = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.SetScencePosition2( _x, _y );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddRepute_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _daode = LuaAPI.xlua_tointeger(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.AddRepute( _daode );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WeiShop_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.WeiShop(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AskSoftStar_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.AskSoftStar(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_instruct_57_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.instruct_57(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_jyx2_ReplaceSceneObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _scene = LuaAPI.lua_tostring(L, 1);
                    string _path = LuaAPI.lua_tostring(L, 2);
                    string _replace = LuaAPI.lua_tostring(L, 3);
                    
                    Jyx2.Jyx2LuaBridge.jyx2_ReplaceSceneObject( _scene, _path, _replace );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_jyx2_CameraFollow_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                    Jyx2.Jyx2LuaBridge.jyx2_CameraFollow( _path );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_jyx2_CameraFollowPlayer_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Jyx2.Jyx2LuaBridge.jyx2_CameraFollowPlayer(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_jyx2_WalkFromTo_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _fromName = LuaAPI.xlua_tointeger(L, 1);
                    int _toName = LuaAPI.xlua_tointeger(L, 2);
                    
                    Jyx2.Jyx2LuaBridge.jyx2_WalkFromTo( _fromName, _toName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isQuickBattle(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, Jyx2.Jyx2LuaBridge.isQuickBattle);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isQuickBattle(RealStatePtr L)
        {
		    try {
                
			    Jyx2.Jyx2LuaBridge.isQuickBattle = LuaAPI.lua_toboolean(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
