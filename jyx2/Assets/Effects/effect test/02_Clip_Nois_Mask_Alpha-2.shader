// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33337,y:32703,varname:node_9361,prsc:2|custl-8073-OUT,alpha-1650-OUT,clip-7089-OUT;n:type:ShaderForge.SFN_Tex2d,id:7175,x:29822,y:32456,ptovrint:False,ptlb:1_TEX,ptin:_1_TEX,varname:node_510,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7650-OUT;n:type:ShaderForge.SFN_Multiply,id:6055,x:30751,y:32407,varname:node_6055,prsc:2|A-9853-OUT,B-3634-RGB,C-7992-OUT,D-3634-A,E-7102-RGB;n:type:ShaderForge.SFN_Color,id:3634,x:30182,y:32300,ptovrint:False,ptlb:1_TEX_Color,ptin:_1_TEX_Color,varname:node_1516,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Desaturate,id:1603,x:30015,y:32492,varname:node_1603,prsc:2|COL-7175-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:7992,x:30182,y:32456,ptovrint:False,ptlb:1_TEX_QS,ptin:_1_TEX_QS,varname:node_1251,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7175-RGB,B-1603-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9853,x:30182,y:32227,ptovrint:False,ptlb:1_TEX_LD,ptin:_1_TEX_LD,varname:node_1953,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:7650,x:29598,y:32456,varname:node_7650,prsc:2|A-4318-UVOUT,B-2796-OUT;n:type:ShaderForge.SFN_Tex2d,id:1122,x:28823,y:33025,ptovrint:False,ptlb:1_Nois,ptin:_1_Nois,varname:node_9959,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6652-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2442,x:29017,y:33099,varname:node_2442,prsc:2|A-1122-R,B-1498-OUT;n:type:ShaderForge.SFN_Slider,id:1498,x:28666,y:33195,ptovrint:False,ptlb:1_Nois_QD,ptin:_1_Nois_QD,varname:node_7557,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:3906,x:28313,y:32527,varname:node_3906,prsc:2|A-9465-OUT,B-3924-OUT;n:type:ShaderForge.SFN_RemapRange,id:9465,x:28099,y:32432,varname:node_9465,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-8884-OUT;n:type:ShaderForge.SFN_Slider,id:8884,x:27751,y:32434,ptovrint:False,ptlb:1_TEX_UVrotator,ptin:_1_TEX_UVrotator,varname:node_2140,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_TexCoord,id:3157,x:28301,y:32088,varname:node_3157,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:4318,x:28779,y:32475,varname:node_4318,prsc:2|UVIN-7494-OUT,ANG-5676-OUT;n:type:ShaderForge.SFN_Append,id:5520,x:27930,y:33037,varname:node_5520,prsc:2|A-6881-X,B-6881-Y;n:type:ShaderForge.SFN_Multiply,id:2203,x:28115,y:32989,varname:node_2203,prsc:2|A-1193-T,B-5520-OUT;n:type:ShaderForge.SFN_Add,id:3576,x:28312,y:32917,varname:node_3576,prsc:2|A-561-UVOUT,B-2203-OUT;n:type:ShaderForge.SFN_TexCoord,id:561,x:28115,y:32848,varname:node_561,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector4Property,id:6881,x:27750,y:33017,ptovrint:False,ptlb:1_Nois_UVspeed,ptin:_1_Nois_UVspeed,varname:node_859,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Add,id:7494,x:28493,y:32203,varname:node_7494,prsc:2|A-3157-UVOUT,B-7910-OUT;n:type:ShaderForge.SFN_Multiply,id:7910,x:28301,y:32232,varname:node_7910,prsc:2|A-1193-T,B-5321-OUT;n:type:ShaderForge.SFN_Append,id:5321,x:28089,y:32279,varname:node_5321,prsc:2|A-9666-X,B-9666-Y;n:type:ShaderForge.SFN_Vector4Property,id:9666,x:27910,y:32257,ptovrint:False,ptlb:1_TEX_UVspeed,ptin:_1_TEX_UVspeed,varname:node_8854,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Rotator,id:6652,x:28587,y:33025,varname:node_6652,prsc:2|UVIN-3576-OUT,ANG-9970-OUT;n:type:ShaderForge.SFN_Multiply,id:9970,x:28316,y:33285,varname:node_9970,prsc:2|A-2457-OUT,B-3924-OUT;n:type:ShaderForge.SFN_RemapRange,id:2457,x:28149,y:33213,varname:node_2457,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-7331-OUT;n:type:ShaderForge.SFN_Slider,id:7331,x:27807,y:33212,ptovrint:False,ptlb:1_Nois_UVrotator,ptin:_1_Nois_UVrotator,varname:_1_UVrotator_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_SwitchProperty,id:2796,x:29220,y:33153,ptovrint:False,ptlb:1_Nois_switch,ptin:_1_Nois_switch,varname:node_4635,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2074-OUT,B-2442-OUT;n:type:ShaderForge.SFN_Vector1,id:2074,x:29017,y:33042,varname:node_2074,prsc:2,v1:0;n:type:ShaderForge.SFN_Time,id:1193,x:26776,y:33672,varname:node_1193,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:4924,x:31093,y:33214,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_6261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3600-UVOUT;n:type:ShaderForge.SFN_Rotator,id:3600,x:30909,y:33214,varname:node_3600,prsc:2|UVIN-9543-UVOUT,ANG-1638-OUT;n:type:ShaderForge.SFN_TexCoord,id:9543,x:30683,y:33120,varname:node_9543,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1638,x:30683,y:33278,varname:node_1638,prsc:2|A-5937-OUT,B-3485-OUT;n:type:ShaderForge.SFN_Pi,id:3485,x:30492,y:33361,varname:node_3485,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:5937,x:30459,y:33204,varname:node_5937,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-1253-OUT;n:type:ShaderForge.SFN_Slider,id:1253,x:30126,y:33204,ptovrint:False,ptlb:Mask_UVrotator,ptin:_Mask_UVrotator,varname:node_7574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_ValueProperty,id:3727,x:31093,y:33407,ptovrint:False,ptlb:Mask_QD,ptin:_Mask_QD,varname:node_6823,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_SwitchProperty,id:1650,x:31853,y:33047,ptovrint:False,ptlb:Mask_switch,ptin:_Mask_switch,varname:node_2765,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-1568-OUT,B-8262-OUT;n:type:ShaderForge.SFN_Multiply,id:8262,x:31587,y:33268,varname:node_8262,prsc:2|A-1568-OUT,B-7575-OUT;n:type:ShaderForge.SFN_Power,id:7575,x:31290,y:33293,varname:node_7575,prsc:2|VAL-4924-R,EXP-3727-OUT;n:type:ShaderForge.SFN_If,id:7089,x:29457,y:33938,varname:node_7089,prsc:2|A-3476-OUT,B-9526-R,GT-2435-OUT,EQ-2435-OUT,LT-2492-OUT;n:type:ShaderForge.SFN_If,id:6596,x:29457,y:34064,varname:node_6596,prsc:2|A-3476-OUT,B-3593-OUT,GT-2435-OUT,EQ-2435-OUT,LT-2492-OUT;n:type:ShaderForge.SFN_Tex2d,id:9526,x:28981,y:33949,ptovrint:False,ptlb:Clip,ptin:_Clip,varname:node_6224,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-130-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3593,x:29181,y:34037,varname:node_3593,prsc:2|A-9526-R,B-1940-OUT;n:type:ShaderForge.SFN_VertexColor,id:9145,x:28782,y:33664,varname:node_9145,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9049,x:28782,y:33812,ptovrint:False,ptlb:Clip_QD,ptin:_Clip_QD,varname:node_3675,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:7025,x:28986,y:33794,varname:node_7025,prsc:2|A-9145-A,B-9049-OUT;n:type:ShaderForge.SFN_RemapRange,id:1940,x:28981,y:34115,varname:node_1940,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-8315-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8315,x:28797,y:34115,ptovrint:False,ptlb:IF_fw,ptin:_IF_fw,varname:node_1849,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:2435,x:29181,y:34166,varname:node_2435,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:2492,x:29181,y:34225,varname:node_2492,prsc:2,v1:0;n:type:ShaderForge.SFN_Subtract,id:6127,x:29638,y:33991,varname:node_6127,prsc:2|A-7089-OUT,B-6596-OUT;n:type:ShaderForge.SFN_Multiply,id:4511,x:29846,y:33991,varname:node_4511,prsc:2|A-6127-OUT,B-9757-RGB,C-9757-A;n:type:ShaderForge.SFN_Color,id:9757,x:29638,y:34143,ptovrint:False,ptlb:IF_Color,ptin:_IF_Color,varname:node_6154,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:6839,x:30043,y:33991,varname:node_6839,prsc:2|A-4511-OUT,B-3530-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3530,x:29846,y:34158,ptovrint:False,ptlb:IF_qd,ptin:_IF_qd,varname:node_6313,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Multiply,id:6689,x:31839,y:33713,varname:node_6689,prsc:2|A-7575-OUT,B-6839-OUT;n:type:ShaderForge.SFN_RemapRange,id:3476,x:29181,y:33794,varname:node_3476,prsc:2,frmn:1,frmx:0,tomn:0.1,tomx:-1|IN-7025-OUT;n:type:ShaderForge.SFN_VertexColor,id:7102,x:29816,y:32822,varname:node_7102,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1568,x:30487,y:32815,varname:node_1568,prsc:2|A-3634-A,B-7175-A,C-7102-R;n:type:ShaderForge.SFN_Add,id:8073,x:32891,y:32860,varname:node_8073,prsc:2|A-6055-OUT,B-6689-OUT,C-4483-OUT;n:type:ShaderForge.SFN_Add,id:5676,x:28526,y:32527,varname:node_5676,prsc:2|A-3906-OUT,B-433-OUT;n:type:ShaderForge.SFN_Multiply,id:433,x:28115,y:32715,varname:node_433,prsc:2|A-1193-T,B-4098-OUT;n:type:ShaderForge.SFN_Slider,id:4098,x:27762,y:32735,ptovrint:False,ptlb:1_TEX_UVrotator_speed,ptin:_1_TEX_UVrotator_speed,varname:_1_TEX_UVrotator_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-30,cur:0,max:30;n:type:ShaderForge.SFN_Rotator,id:130,x:28767,y:33949,varname:node_130,prsc:2|UVIN-216-OUT,ANG-5337-OUT;n:type:ShaderForge.SFN_Add,id:5337,x:28497,y:33996,varname:node_5337,prsc:2|A-5224-OUT,B-1296-OUT;n:type:ShaderForge.SFN_Multiply,id:5224,x:28186,y:34005,varname:node_5224,prsc:2|A-2889-OUT,B-3924-OUT;n:type:ShaderForge.SFN_Pi,id:3924,x:27209,y:33636,varname:node_3924,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2889,x:27950,y:33868,varname:node_2889,prsc:2,frmn:0,frmx:360,tomn:0,tomx:2|IN-6474-OUT;n:type:ShaderForge.SFN_Slider,id:6474,x:27598,y:33866,ptovrint:False,ptlb:Clip_rotator,ptin:_Clip_rotator,varname:node_4943,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_Slider,id:1380,x:27806,y:34195,ptovrint:False,ptlb:Clip_UVrotator_speed,ptin:_Clip_UVrotator_speed,varname:_2_TEX_UVrotator_speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-30,cur:0,max:30;n:type:ShaderForge.SFN_Multiply,id:1296,x:28186,y:34175,varname:node_1296,prsc:2|A-1193-T,B-1380-OUT;n:type:ShaderForge.SFN_Add,id:216,x:28446,y:33665,varname:node_216,prsc:2|A-7465-UVOUT,B-639-OUT;n:type:ShaderForge.SFN_TexCoord,id:7465,x:28183,y:33548,varname:node_7465,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:639,x:28183,y:33692,varname:node_639,prsc:2|A-1193-T,B-5603-OUT;n:type:ShaderForge.SFN_Append,id:5603,x:28001,y:33715,varname:node_5603,prsc:2|A-6757-X,B-6757-Y;n:type:ShaderForge.SFN_Vector4Property,id:6757,x:27812,y:33697,ptovrint:False,ptlb:Clip_UVspeed,ptin:_Clip_UVspeed,varname:_2_Nois_UVspeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Fresnel,id:8324,x:31655,y:32339,varname:node_8324,prsc:2|EXP-989-OUT;n:type:ShaderForge.SFN_Multiply,id:9292,x:32068,y:32339,varname:node_9292,prsc:2|A-8660-OUT,B-655-OUT;n:type:ShaderForge.SFN_Multiply,id:8660,x:31859,y:32339,varname:node_8660,prsc:2|A-8324-OUT,B-2755-RGB;n:type:ShaderForge.SFN_Color,id:2755,x:31655,y:32487,ptovrint:False,ptlb:FRE_Color,ptin:_FRE_Color,varname:node_5707,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:989,x:31474,y:32362,ptovrint:False,ptlb:FRE_fw,ptin:_FRE_fw,varname:node_5278,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:655,x:31859,y:32487,ptovrint:False,ptlb:FRE_qd,ptin:_FRE_qd,varname:_FRE_fw_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:4483,x:32282,y:32339,ptovrint:False,ptlb:FRE_switch,ptin:_FRE_switch,varname:node_1657,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5905-OUT,B-9292-OUT;n:type:ShaderForge.SFN_Vector1,id:5905,x:32068,y:32275,varname:node_5905,prsc:2,v1:0;proporder:7992-3634-9853-7175-8884-4098-9666-2796-1122-1498-7331-6881-1650-4924-3727-1253-9526-9049-6474-1380-6757-9757-8315-3530-4483-2755-989-655;pass:END;sub:END;*/

Shader "Skilleffect/02_Clip_Nois_Mask_Alpha-2" {
    Properties {
        [MaterialToggle] _1_TEX_QS ("1_TEX_QS", Float ) = 0
        _1_TEX_Color ("1_TEX_Color", Color) = (1,1,1,1)
        _1_TEX_LD ("1_TEX_LD", Float ) = 2
        _1_TEX ("1_TEX", 2D) = "white" {}
        _1_TEX_UVrotator ("1_TEX_UVrotator", Range(0, 360)) = 0
        _1_TEX_UVrotator_speed ("1_TEX_UVrotator_speed", Range(-30, 30)) = 0
        _1_TEX_UVspeed ("1_TEX_UVspeed", Vector) = (0,0,0,0)
        [MaterialToggle] _1_Nois_switch ("1_Nois_switch", Float ) = 0
        _1_Nois ("1_Nois", 2D) = "white" {}
        _1_Nois_QD ("1_Nois_QD", Range(0, 10)) = 0
        _1_Nois_UVrotator ("1_Nois_UVrotator", Range(0, 360)) = 0
        _1_Nois_UVspeed ("1_Nois_UVspeed", Vector) = (0,0,0,0)
        [MaterialToggle] _Mask_switch ("Mask_switch", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
        _Mask_QD ("Mask_QD", Float ) = 3
        _Mask_UVrotator ("Mask_UVrotator", Range(0, 360)) = 0
        _Clip ("Clip", 2D) = "white" {}
        _Clip_QD ("Clip_QD", Float ) = 1
        _Clip_rotator ("Clip_rotator", Range(0, 360)) = 0
        _Clip_UVrotator_speed ("Clip_UVrotator_speed", Range(-30, 30)) = 0
        _Clip_UVspeed ("Clip_UVspeed", Vector) = (0,0,0,0)
        _IF_Color ("IF_Color", Color) = (1,1,1,1)
        _IF_fw ("IF_fw", Float ) = 0.1
        _IF_qd ("IF_qd", Float ) = 5
        [MaterialToggle] _FRE_switch ("FRE_switch", Float ) = 0
        _FRE_Color ("FRE_Color", Color) = (1,1,1,1)
        _FRE_fw ("FRE_fw", Float ) = 1
        _FRE_qd ("FRE_qd", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 2.0
            uniform sampler2D _1_TEX; uniform float4 _1_TEX_ST;
            uniform float4 _1_TEX_Color;
            uniform fixed _1_TEX_QS;
            uniform float _1_TEX_LD;
            uniform sampler2D _1_Nois; uniform float4 _1_Nois_ST;
            uniform float _1_Nois_QD;
            uniform float _1_TEX_UVrotator;
            uniform float4 _1_Nois_UVspeed;
            uniform float4 _1_TEX_UVspeed;
            uniform float _1_Nois_UVrotator;
            uniform fixed _1_Nois_switch;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Mask_UVrotator;
            uniform float _Mask_QD;
            uniform fixed _Mask_switch;
            uniform sampler2D _Clip; uniform float4 _Clip_ST;
            uniform float _Clip_QD;
            uniform float _IF_fw;
            uniform float4 _IF_Color;
            uniform float _IF_qd;
            uniform float _1_TEX_UVrotator_speed;
            uniform float _Clip_rotator;
            uniform float _Clip_UVrotator_speed;
            uniform float4 _Clip_UVspeed;
            uniform float4 _FRE_Color;
            uniform float _FRE_fw;
            uniform float _FRE_qd;
            uniform fixed _FRE_switch;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_3476 = ((i.vertexColor.a+_Clip_QD)*1.1+-1.0);
                float node_3924 = 3.141592654;
                float4 node_1193 = _Time;
                float node_130_ang = (((_Clip_rotator*0.005555556+0.0)*node_3924)+(node_1193.g*_Clip_UVrotator_speed));
                float node_130_spd = 1.0;
                float node_130_cos = cos(node_130_spd*node_130_ang);
                float node_130_sin = sin(node_130_spd*node_130_ang);
                float2 node_130_piv = float2(0.5,0.5);
                float2 node_130 = (mul((i.uv0+(node_1193.g*float2(_Clip_UVspeed.r,_Clip_UVspeed.g)))-node_130_piv,float2x2( node_130_cos, -node_130_sin, node_130_sin, node_130_cos))+node_130_piv);
                float4 _Clip_var = tex2D(_Clip,TRANSFORM_TEX(node_130, _Clip));
                float node_7089_if_leA = step(node_3476,_Clip_var.r);
                float node_7089_if_leB = step(_Clip_var.r,node_3476);
                float node_2492 = 0.0;
                float node_2435 = 1.0;
                float node_7089 = lerp((node_7089_if_leA*node_2492)+(node_7089_if_leB*node_2435),node_2435,node_7089_if_leA*node_7089_if_leB);
                clip(node_7089 - 0.5);
////// Lighting:
                float node_4318_ang = (((_1_TEX_UVrotator*0.005555556+0.0)*node_3924)+(node_1193.g*_1_TEX_UVrotator_speed));
                float node_4318_spd = 1.0;
                float node_4318_cos = cos(node_4318_spd*node_4318_ang);
                float node_4318_sin = sin(node_4318_spd*node_4318_ang);
                float2 node_4318_piv = float2(0.5,0.5);
                float2 node_4318 = (mul((i.uv0+(node_1193.g*float2(_1_TEX_UVspeed.r,_1_TEX_UVspeed.g)))-node_4318_piv,float2x2( node_4318_cos, -node_4318_sin, node_4318_sin, node_4318_cos))+node_4318_piv);
                float node_6652_ang = ((_1_Nois_UVrotator*0.005555556+0.0)*node_3924);
                float node_6652_spd = 1.0;
                float node_6652_cos = cos(node_6652_spd*node_6652_ang);
                float node_6652_sin = sin(node_6652_spd*node_6652_ang);
                float2 node_6652_piv = float2(0.5,0.5);
                float2 node_6652 = (mul((i.uv0+(node_1193.g*float2(_1_Nois_UVspeed.r,_1_Nois_UVspeed.g)))-node_6652_piv,float2x2( node_6652_cos, -node_6652_sin, node_6652_sin, node_6652_cos))+node_6652_piv);
                float4 _1_Nois_var = tex2D(_1_Nois,TRANSFORM_TEX(node_6652, _1_Nois));
                float2 node_7650 = (node_4318+lerp( 0.0, (_1_Nois_var.r*_1_Nois_QD), _1_Nois_switch ));
                float4 _1_TEX_var = tex2D(_1_TEX,TRANSFORM_TEX(node_7650, _1_TEX));
                float node_3600_ang = ((_Mask_UVrotator*0.005555556+0.0)*3.141592654);
                float node_3600_spd = 1.0;
                float node_3600_cos = cos(node_3600_spd*node_3600_ang);
                float node_3600_sin = sin(node_3600_spd*node_3600_ang);
                float2 node_3600_piv = float2(0.5,0.5);
                float2 node_3600 = (mul(i.uv0-node_3600_piv,float2x2( node_3600_cos, -node_3600_sin, node_3600_sin, node_3600_cos))+node_3600_piv);
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(node_3600, _Mask));
                float node_7575 = pow(_Mask_var.r,_Mask_QD);
                float node_6596_if_leA = step(node_3476,(_Clip_var.r*(_IF_fw*10.0+0.0)));
                float node_6596_if_leB = step((_Clip_var.r*(_IF_fw*10.0+0.0)),node_3476);
                float3 finalColor = ((_1_TEX_LD*_1_TEX_Color.rgb*lerp( _1_TEX_var.rgb, dot(_1_TEX_var.rgb,float3(0.3,0.59,0.11)), _1_TEX_QS )*_1_TEX_Color.a*i.vertexColor.rgb)+(node_7575*(((node_7089-lerp((node_6596_if_leA*node_2492)+(node_6596_if_leB*node_2435),node_2435,node_6596_if_leA*node_6596_if_leB))*_IF_Color.rgb*_IF_Color.a)*_IF_qd))+lerp( 0.0, ((pow(1.0-max(0,dot(normalDirection, viewDirection)),_FRE_fw)*_FRE_Color.rgb)*_FRE_qd), _FRE_switch ));
                float node_1568 = (_1_TEX_Color.a*_1_TEX_var.a*i.vertexColor.r);
                return fixed4(finalColor,lerp( node_1568, (node_1568*node_7575), _Mask_switch ));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
