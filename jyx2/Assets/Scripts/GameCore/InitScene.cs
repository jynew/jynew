using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if DEVELOP_TAPTAP
using LC.Newtonsoft.Json;
using TapTap.Common;
using TapTap.Login;
using TapTap.AntiAddiction;
using TapTap.AntiAddiction.Model;
#endif

public class InitScene : MonoBehaviour
{
    private static string _tapClientId;

    public Button TapLogin_Button;
    
    private async void Awake()
    {
#if DEVELOP_TAPTAP
        GetTapTapParams();
        TapLogin.Init(_tapClientId);
        TapTapAntiAddictionInit();

        TapLogin_Button.gameObject.SetActive(false);
        TapLogin_Button.onClick.AddListener(async () =>
        {
            await TapTapLogin();
        });
        
        // 检查登录状态
        await CheckTapTapLoginStatus();
#else
        SceneManager.LoadScene("0_GameStart");
#endif
    }

#if DEVELOP_TAPTAP
    /// <summary>
    /// 读取TapTap的参数
    /// </summary>
    private static void GetTapTapParams()
    {
        var file = Resources.Load<TextAsset>("TAPTAP_BUILD_PARAMS");
        if (file == null)
        {
            Debug.LogError("there is no configuration file (TAPTAP_BUILD_PARAMS.txt) in resources for taptap");
            return;
        }

        var content = file.text;
        var data = JsonConvert.DeserializeObject<Hashtable>(content);
        if (!data.ContainsKey("ClientId"))
        {
            Debug.LogError("there is no tap ClientId in the TAPTAP_BUILD_PARAMS.txt");
            return;
        }

        _tapClientId = data["ClientId"].ToString();
    }
    
    /// <summary>
    /// 检查用户登录状态
    /// </summary>
    /// <returns></returns>
    private async UniTask CheckTapTapLoginStatus()
    {
        try
        {
            var accessToken = await TapLogin.GetAccessToken();
            
            // 已登录
            await TapTapAntiAddictionStart();
        }
        catch (Exception e)
        {
            // 未登录
            TapLogin_Button.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 用户点了Tap登录按钮
    /// </summary>
    /// <returns></returns>
    private async UniTask TapTapLogin()
    {
        try
        {
            TapLogin_Button.gameObject.SetActive(false);
            // 在 iOS、Android 系统下，会唤起 TapTap 客户端或以 WebView 方式进行登录
            // 在 Windows、macOS 系统下显示二维码（默认）和跳转链接（需配置）
            var accessToken = await TapLogin.Login();
            // Debug.Log($"TapTap 登录成功 accessToken: {accessToken.ToJson()}");

            await TapTapAntiAddictionStart();
        }
        catch (Exception e)
        {
            if (e is TapException tapError)  // using TapTap.Common
            {
                Debug.Log($"encounter exception:{tapError.code} message:{tapError.message}");
                if (tapError.code == (int)TapErrorCode.ERROR_CODE_BIND_CANCEL) // 取消登录
                {
                    Debug.Log("登录取消");
                }
            }
            TapLogin_Button.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 用户登录成功后进行防沉迷认证
    /// </summary>
    private static async UniTask TapTapAntiAddictionStart()
    {
        // 获取 TapTap Profile  可以获得当前用户的一些基本信息，例如名称、头像。
        var profile = await TapLogin.FetchProfile();
        // Debug.Log($"TapTap 登录成功 profile: {profile.ToJson()}");
        
        AntiAddictionUIKit.Startup(profile.openid); // 使用单纯 TapTap 用户认证可以用 openid 或 unionid
    }

    /// <summary>
    /// 防沉迷初始化，配置相关回调
    /// </summary>
    private void TapTapAntiAddictionInit()
    {
        var config = new AntiAddictionConfig()
        {
            gameId = _tapClientId,          // TapTap 开发者中心对应 Client ID
            useTapLogin = true,             // 是否启动 TapTap 快速认证
            showSwitchAccount = false,      // 是否显示切换账号按钮
        };

        Action<int, string> callback = (code, errorMsg) => {
            //code == 500;   //登录成功
            //code = 1000;   //用户登出
            //code = 1001;   //切换账号
            //code = 1030;   //用户当前无法进行游戏
            //code = 1050;   //时长限制
            //code = 9002;   //实名过程中点击了关闭实名窗
            Debug.LogFormat($"TapTapAntiAddiction callback code: {code} error Message: {errorMsg}");
            if (code == 500)
            {
                AntiAddictionUIKit.EnterGame();     // 已登录的玩家，开始游戏时调用此接口，之后 SDK 会自动轮询上报游戏时长
                SceneManager.LoadScene("0_GameStart");
            }
            else
            {
                TapLogin_Button.gameObject.SetActive(true);
            }
        };
        
        AntiAddictionUIKit.Init(config, callback);
    }
        
#endif
}
