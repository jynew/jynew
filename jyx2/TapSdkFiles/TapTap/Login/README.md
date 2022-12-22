## [TapTap.Login](./Documentation/README.md)

## 使用前提

使用 TapTap.Login 前提是必须依赖以下库:

* [TapTap.Common](https://github.com/TapTap/TapCommon-Unity.git)

> 如果游戏需要支持 PC Platform ，则参考[ PC配置文档 ](./PC.md)

### 1.初始化

#### 如果配合 `TapBoostrap` 使用，则不需要调用初始化接口

```c#
TapLogin.Init(string clientID);
```

### 2.唤起 TapTap 网页 或者 TapTap 客户端进行登陆

登陆成功之后，会返回 `AccessToken`

```c#
var accessToken = await TapLogin.Login();
```

配置权限范围

```c#
// 默认使用 public_profile

var accessToken = await TapLogin.Login(new []{"public_profile"})
;
```

### 3. 获取 TapTap AccessToken

```c#
var accessToken = await TapLogin.GetAccessToken();
```

### 4. 获取 TapTap Profile

```c#
var profile = await TapLogin.FetchProfile();
```

### 5. 获取篝火测试资格

```c#
var boolean = await TapLogin.GetTestQualification();
```

### 6. 退出登陆

```c#
TapLogin.Logout();
```