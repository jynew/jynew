## TapTap.Common

### 接口描述

#### 1.获取地区

```c#
TapCommon.GetRegionCode(isMainland =>
{
    // true 中国大陆 false 非中国大陆
});
```

#### 2. TapTap 是否安装

```c#
TapCommon.IsTapTapInstalled(installed =>
{
    // true 安装 false 未安装
});
```

#### 3. TapTap IO 是否安装

```c#
TapCommon.IsTapTapGlobalInstalled(installed =>
{
    // true 安装  false 未安装
});
```

### Android 独占方法

#### 4. 在 TapTap 更新游戏

```c#
TapCommon.UpdateGameInTapTap(appId,updateSuccess =>
{
    // true 更新成功 false 更新失败
});
```

#### 5. 在 TapTap IO 更新游戏

```c#
TapCommon.UpdateGameInTapGlobal(appId,updateSuccess =>
{
    // true 更新成功 false 更新失败
});
```

#### 6. 在 TapTap 打开当前游戏的评论区

```c#
TapCommon.OpenReviewInTapTap(appId,openSuccess =>
{
    // true 打开评论区 false 打开失败
});
```

#### 6. 在 TapTap IO 打开当前游戏的评论区

```c#
TapCommon.OpenReviewInTapGlobal(appId,openSuccess =>
{
    // true 打开评论区 false 打开失败
});
```

#### 7. 唤起 TapTap 客户端更新游戏

appId: 游戏在 TapTap 商店的唯一身份标识

例如：https://www.taptap.com/app/187168 ，其中 187168 是 appid.

```c#
// 在 TapTap 客户端更新游戏，失败时通过浏览器打开 Tap 网站对应的游戏页面
// 当你在国内区上架时使用
bool isSuccess = await TapCommon.UpdateGameAndFailToWebInTapTap(string appId);
// 当你在海外区上架时使用
bool isSuccess = await TapCommon.UpdateGameAndFailToWebInTapGlobal(string appId):
```

如果你需要在唤起 Tap 客户端失败时跳转到自己的网页

```c#
bool isSuccess = await TapCommon.UpdateGameAndFailToWebInTapTap(string appId, string webUrl)
bool isSuccess = await TapCommon.UpdateGameAndFailToWebInTapGlobal(string appId, string webUrl)
```