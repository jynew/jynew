# TapTap Login PC 登录接入文档

> TapTap.Login PC 支持 Window 以及 Mac 两种平台，提供 Web 浏览器授权以及 TapTap 客户端扫码登录


## 在 Mac 平台使用 TapTap.Login

### 1. 编译配置

* 打开 *BuildSetting* 选择 *PC、Mac & Linux Standalone* Platform，*Target Platform* 选择 MacOS
* 勾选 *Create XCode Project* ，选择输出 XCode 工程进行编译

### 2. 配置 URL Types

* 打开输出的 *XCode Project* , 选择 *Target* ，点击 *Info* ，展开 *URL Types*，请检查是否自动添加以下 URL Scheme，如果未添加，则手动添加进去。
> TapWeb : open-taptap-{clientId}

* 或者修改 *Info.plist* ，添加以下配置

```xml
<key>CFBundleURLTypes</key>
<array>
    <dict>
        <key>CFBundleURLName</key>
        <string>TapWeb</string>
        <key>CFBundleURLSchemes</key>
        <array>
          <string>open-taptap-{client_id}</string>
        </array>
    </dict>
</array>
```

## 在 Window 平台使用 TapTap.Login

#### 给 Window 添加游戏注册表

```
Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\open-taptap-{client_id}]
@="{游戏名称}"
"URL Protocol"="{程序.exe 安装路径}}"

[HKEY_CLASSES_ROOT\open-taptap-{client_id}]
@="{游戏名称}"

[HKEY_CLASSES_ROOT\open-taptap-{client_id}]

[HKEY_CLASSES_ROOT\open-taptap-{client_id}\Shell\Open]

[HKEY_CLASSES_ROOT\open-taptap-{client_id}\Shell\Open\Command]
@="\"{程序.exe 安装路径}\" \"%1\""

```

打开 Window 注册表编辑器，查看 `HKEY_CLASSES_ROOT\open-taptap-{clientId}` 是否存在以及等该目录下是否包含 `DefaultIcon`、`Shell\Open\Command` 是否匹配上文中的参数



