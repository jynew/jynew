# ChangeLog

## 3.13.0
### Fixed Bugs
- Android & iOS: 支持新的 scope (basic_info & email)
- Android: 修复一些情况下的错误崩溃
- iOS: 支持更安全的有端授权协议

## 3.11.1

### Fixed Bugs
- iOS: 修复登录模块对系统 URL 回调的使用方式

### Dependencies

- TapTap.Common v3.11.1

## 3.11.0

### Dependencies

- TapTap.Common v3.11.0

## 3.10.0

### Dependencies

- TapTap.Common v3.10.0

## 3.9.0

### Dependencies

- TapTap.Common v3.9.0

## 3.8.0

### Dependencies

- TapTap.Common v3.8.0

## 3.7.1

### Dependencies

- TapTap.Common v3.7.1

## 3.7.0

### Optimization and fixed bugs
- 添加获取互关列表接口

### Dependencies

- TapTap.Common v3.7.0


## 3.6.3

### Optimization and fixed bugs
- Android 尝试修复静态变量丢失的问题

### Dependencies

- TapTap.Common v3.6.3

## 3.6.1

### Optimization and fixed bugs
- UI 优化

### Dependencies

- TapTap.Common v3.6.1

## 3.6.0

### Optimization and fixed bugs

- 优化未安装 iOS Support 时 Editor 编译问题

### Dependencies

- TapTap.Common v3.6.0

## 3.5.2

### New Feature

- 新增 PC Web 授权登录

### Dependencies

- TapTap.Common v3.5.2

## 3.5.0

### Optimization and fixed bugs
- 内嵌 web 登录页面支持异形刘海屏的正常展示

### Dependencies

- TapTap.Common v3.5.0

## 3.4.0

### Dependencies

- TapTap.Common v3.4.0

## 3.3.0

### Optimization and fixed bugs

- 优化 iOS Plist.info 修改方式，防止覆盖工程原有配置。

### Dependencies

- TapTap.Common v3.3.0

## 3.2.1

### New Feature 

- 新增 TapTap 登录权限配置范围

### Dependencies

- TapTap.Common v3.2.0

## 3.2.0

### New Feature

- 支持 PC 端使用 TapTap 登录

### Dependencies

- TapTap.Common v3.2.0

## 3.1.0

### Dependencies

- TapTap.Common v3.1.0

## 3.0.0

### New Feature

- 新增 TapTap OAuth 相关接口
  ```
  // 登陆  
  TapLogin.Login();
  // 登出
  TapLogin.Logout();
  ```
- 新增篝火测试资格
  ```
  var boolean = await TapLogin.GetTestQualification();
  ```

### Dependencies

- TapTap.Common v3.0.0

## 2.1.7

### Optimization

- 更新 iOS 拦截 openUrl 方式

### Dependencies

- TapTap.Common v2.1.7

### 2.1.6

### Optimization and fixed bugs

- 修复 TapTap 授权登录的一些 UI 问题

### Dependencies

- TapTap.Common v2.1.6

### 2.1.5

### New Feature

- 云玩内唤起 TapTap 客户端登陆

### 2.1.4

### Dependencies

- TapTap.Common v2.1.4

### 2.1.3

### Dependencies

- TapTap.Common v2.1.3

### 2.1.2

### Dependencies

- TapTap.Common v2.1.2

## 2.1.1

### Dependencies

- TapTap.Common v2.1.1

## 2.0.0

### Feature

* TapTap Login

