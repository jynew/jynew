# 游戏防沉迷 AntiAddiction (Unity) 对接文档
AntiAddictionSDK 是为了遵循最新防沉迷政策而编写的一个集实名登记、防沉迷时长限制、付费限制三部分功能的组件，方便国内游戏团队快速接入游戏实现防沉迷功能从而符合政策规定。

# 说明
Unity 模块是通过引入 iOS 和 Android 模块后增加桥接文件打包出的 `.unitypackage`，方便以 Unity 开发的游戏直接引入。其他引擎/平台的游戏可以通过 iOS/Android 原生的方式接入，详见 iOS/Android 各模块接入文档。

## 1.接入SDK
Unity 开发环境:2018.4.36f1

导入 `AntiAddictionForUnity.unitypackage`

### 1.1 iOS
- iOS Deployment Target 最低支持 iOS 10.0
- Xcode 13.0 beta 5 编译 

>注意:  
>`unitypackge`中默认 iOS 平台 `AntiAddictionService.framework、AntiAddictionUI.framework` 同时支持真机和模拟器架构。

**检查 Unity 输出的 Xcode 工程**

1. 请确保设置 `Xcode` - `General` - `Frameworks, Libraries, and Embedded Content` 中的 `AntiAddictionService.framework`  和 AntiAddictionUI.framework 为 `Do Not Embed`。
2. 如果编译报错找不到头文件或者模块，请确保 `Xcode`-`Build Settings` - `Framework Search Paths` 中的路径以保证 Xcode 正常编译。
3. 确保 Xcode 工程的 `Build Settings` 的 `Swift Compile Language/Swfit Language Version` 为 `Swift5`。
4. 添加依赖库 `libz.tbd` `libc++.tdb`
5. 开始代码接入
6. 将 AntiAddiction-Unity/Assets/Plugins/iOS/Resource/AntiAdictionResources.bundle 拷贝到游戏项目下 (如果unity项目没有正确导入 AntiAddictionResources.bundle)

> 请确保以上步骤正确执行。

### 1.2 Android
最低支持安卓版本 5.0。

## 2.接口文档
防沉迷需要游戏提供用于授权防沉迷的游戏唯一id（需要保证唯一即可，建议不要使用游戏中的用户id，如果一定要使用可以进行hash处理，客户端对长度无限制，服务端支持最长32位的字符）。

**以下使用需要SDK命名空间下**
```
using Plugins.AntiAddictionUIKit
```

### 2.1 初始化
初始化SDK并设置回调，初始化方法接收Action作为回调
- 参数介绍
- gameIdentifier 游戏名称标识（游戏自行定义）
- useTimeLimit 启用时长限制功能
- usePaymentLimit 启用付费限制功能
- antiServerUrl 防沉迷服务域名
- identifyServerUrl 实名服务域名
- departmentWebSocketUrl 中宣部长连服务域名
- antiSecretKey 防沉迷服务密钥
示例如下：
```
string gameIdentifier = "游戏的 Client ID";
// 是否启用时长限制功能
bool useTimeLimit = true;
// 是否启用消费限制功能
bool usePaymentLimit = true;
AntiAddictionUIKit.Init(gameIdentifier, useTimeLimit, usePaymentLimit,
    (antiAddictionCallbackData) => {
        int code = antiAddictionCallbackData.code;
        MsgExtraParams extras = antiAddictionCallbackData.extras;
        // 根据 code 不同提示玩家不同信息，详见下面的说明
    },
    (exception) => {
        // 处理异常
    },
);
```
回调中会返回对应的回调类型码 resultCode 和相应信息 message：

回调类型 | 参数值 |  触发条件 | 附带信息
--- | --- | --- | ---
CALLBACK\_CODE\_ENTER\_SUCCESS | 500 | 用户登录后判断当前用户可以进行游戏(未成年用户在可玩时间登录也会收到该消息) | 无
CALLBACK\_CODE\_SWITCH_ACCOUNT | 1000 | 切换账号，当用户因防沉迷机制受限时，选择切换账号时会触发 | 无
CALLBACK\_CODE\_TIME\_LIMIT | 1030 | 用户当前无法进行游戏 | 给用户返回提示信息
CALLBACK\_CODE\_OPEN\_ALERT | 1095 | 未成年允许游戏弹窗
CALLBACK\_CODE\_REAL\_NAME\_STOP | 9002 | 实名过程中点击了关闭实名窗

### 2.2 防沉迷授权

SDK 支持两种防沉迷授权方式：

1. 使用 TapTap 快速认证，传入玩家的唯一标识和 TapTap 的鉴权信息，TDS 云端会根据相应玩家在 TapTap 的实名信息判断玩家是否可以进行游戏。
2. 不使用 TapTap 快速认证，玩家在 SDK 提供的界面中手动输入身份证号等实名信息，TDS 云端会将相应信息上报至中宣部防沉迷实名认证系统。

这两种方式都需要传入的玩家唯一标识，该标识由游戏自己定义。
如果使用 TDS 内建账户系统，可以使用玩家的 `objectId`。

```cs
bool useTapLogin = true;
string userIdentifier = "玩家的唯一标识";
string tapTapAccessToken = "TapTap 第三方登录的 access token";

AntiAddictionUIKit.Startup(useTapLogin, userIdentifier, tapTapAccessToken);
```

### 手动输入实名信息

```cs
string userIdentifier = "玩家的唯一标识";
AntiAddictionUIKit.Startup(false, userIdentifier, "");
```

### 获取 TapTap Access Token

初始化时需要传入 TapTap 的 `access token`，以便从 TapTap 获取玩家的实名信息。

无论游戏使用[TDS 内建账户系统](/sdk/taptap-login/guide/start/#用-taptap-oauth-授权结果直接登录账户系统)，还是使用[单纯 TapTap 用户认证](/sdk/taptap-login/guide/tap-login/#taptap-登录并获取登录结果)的方式接入 TapTap 登录，在玩家已登录 TapTap 的情况下，都可以通过如下接口获取 TapTap 的 `access token`：

```cs
AccessToken accessToken = TapLogin.GetAccessToken();
string tapTapAccessToken = JsonUtility.ToJson(accessToken);
```

### 2.3 登出

玩家在游戏内退出账号时调用，重置防沉迷状态。

```cs
AntiAddictionUIKit.Logout();
```

### 2.4 获取玩家年龄段

调用该接口可获取玩家所处年龄段：
```cs
int ageRange = AntiAddictionUIKit.CurrentUserAgeLimit();
```

### 2.5 检查消费上限

根据年龄段的不同，未成年玩家的消费金额有不同的上限。
如果启用消费限制功能，开发者需要在未成年玩家消费前检查是否受限，并在成功消费后上报消费金额。

游戏在收到玩家的付费请求后，调用以下接口当前玩家的付费行为是否被限制：

```cs
long amount = 100;
AntiAddictionUIKit.CheckPayLimit(amount,
    (result) => {
        // status 为 1 时可以支付
        int status = result.status;
        if (status != 1) {
            // 限制消费提示标题
            string title = result.title;
            // 限制消费提示描述（例如法规说明）
            string description = result.description; 
        }
    },
    (exception) => {
        // 处理异常
    }
);
```

### 2.6 上报消费金额
消费金额的单位为分。

检查消费上限需要游戏事先上报未成年玩家的消费金额。
建议开发者在服务端上报。
开发者也可以调用 SDK 提供的接口，当未成年玩家消费成功后，在客户端上报消费金额，在客户端上报的可靠性低于在服务端上报，主要适用于无服务端的单机游戏。

```cs
long amount = 100;
AntiAddictionUIKit.SubmitPayResult(amount,
    () => {
        // 成功
    }, (exception) => {
        // 处理异常
    }
);

### 2.7 上报游戏时长

如果启用时长限制功能，需要上报游戏时长。

已登录的玩家，开始游戏时调用此接口，之后 SDK 会自动轮询上报游戏时长。

```cs
AntiAddictionUIKit.EnterGame();
```

```cs
AntiAddictionUIKit.LeaveGame();
```

### 2.8 获取用户防沉迷token
```cs
string token = AntiAddictionUIKit.CurrentToken();
```
