# TapCommon 用于支持 TapSDK 其他模块中 Android、iOS 与 Unity 的通信

> 目前 TapSDK 业务中，Unity 业务层实现的功能不多，主要作为桥接来调用 Android、iOS SDK 中的方法。

## 一、实现 Unity 调用 Android、iOS 方法

> 为了方便 Unity 一套代码可以调用双端方法，所以定义原生接口时，需要考虑 Android、iOS 语言之间的差异。

### Android 接口定义

- 使用 `@BridgeService` 、`@BridgeMethod` 、`@BridgeParam` 注解修饰 `类`、`方法`、`参数`。
- `Activity`、`BridgeCallback` 这两个类型参数不需要 `@BridgeParam` 注解。
- `@BridgeParam` 仅支持基本数据类型（包含 `String`）。

```java
@BridgeService("TestService")
public class TestService implementation IBridgeService {
    
    @BridgeMethod("testMethodWithCallback")
    void testMethodWithCallback(BridgeCallback callback);
    
    @BridgeMethod("testMethodWithArgsAndCallback")
    void testMethodWithArgsAndCallback(Activity activity, @BridgeParam("args1") String appId, @BridgeParam("args2") int args2, BridgeCallback callback);
    
}

```

### Android 接口实现

```java
public class TestService implementation TestService {
    
    @Override
    public void testMethodWithCallback(BridgeCallback callback){
        callback.onResult("testMethodWithCallback 回调给 Unity 的参数");        
    }
    
    @Override
    public void testMethodWithArgsAndCallback(Activity activity, String appId,int args2, BridgeCallback callback){
        callback.onResult("testMethodWithArgsAndCallback 回调给 Unity 的参数");        
    }

}

```

### iOS 接口定义

> iOS 方法名通过反射获取到的为 `args1:args2:bridgeCallback` ，所以 iOS 的方法定义与 Android 略有不同。

- 类名必须同 Android `@BridgeService` 所修饰的类名一致。
- 参数名例如 `args1`、`args2` 必须同 Android `@BridgeParam` 修饰的一致，用于回调的 `iOS` 闭包的参数名必须为 `bridgeCallback`。
- 当参数个数仅为 0 或者 仅为 闭包时，方法名必须同 Android `@BridgeMethod` 修饰的方法一致。

```objectivec
@interface TestService

// 匹配的是 Android 中 testMethodWithCallback 方法
+(void) testMethodWithCallback:(void (^)(NSString *result))callback;

// 匹配的是 Android 中 testMethodWithArgsAndCallback 方法
+(void) args1:(NSString*)args1 args2:(NSNumber*)args2 bridgeCallback:(void (^)(NSString *result))callback;

@end
```

### iOS 接口实现

```objectivec
@implementation TestService

+(void) testMethodWithCallback:(void (^)(NSString *result)) callback{
    callback(@"testMethodWithCallback 回调给 Unity 的参数");
}

+(void) args1:(NSString*)args1 args2:(NSNumber*)args2 bridgeCallback:(void (^)(NSString *result))callback{
    callback(@"testMethodWithArgsAndCallback 回调给 Unity 的参数");
}

@end
```

### Unity 调用上文中定义的 Android、iOS 接口

#### 1.初始化

```c#
// Android 初始化
//CLZ_NAME 和IMP_NAME为 接口以及实现类的全路径包名 例如：com.tds.bridge.TestService，com.tds.bridge.TestServiceImpl
EngineBridge.GetInstance().Register(CLZ_NAME, IMP_NAME);

// iOS 无需初始化
```
#### 2.调用方法

`Bridge.CallHandler` 为异步方法，执行线程的流程为：

Unity Thread -> Native MainThread -> Execute Function -> Unity Thread

所以执行 `CallHandler` 的 Thread 和 `Action<Result>` 的 Thread 都为 Unity 当前 Thread。

```c#
 var command = new Command.Builder()
            .Service("TestService") // @BridgeService 值以及 iOS 类名
            .Method("testMethodWithArgsAndCallback")  // @BridgeMethod 值 以及 iOS 方法名
            .Args("args1","value") // @BridgeParam 值 以及 iOS 参数名
            .Args("args2",1) // 同上
            .Callback(true) // 是否需要添加 BridgeCallback
            .OnceTime(true) // 当前 BridgeCallback 是否常驻内存
            .CommandBuilder();
 
 // 需要回调
 EngineBridge.GetInstance().CallHandler(command,result=>{
     if(EngineBridge.CheckResult(result)){
        // 桥接调用成功
        // 当前 Content 则为 Android、iOS 通过 BridgeCallback 传给 Unity 的值
        var content = result.content;
     }
 });
 
 // 不需要回调
 EngineBridge.GetInstance().CallHandler(command);
```

## 二、Android 、iOS 调用 Unity

鉴于 TapSDK 3.1.+ 之后，Android 与 iOS 需要同步 `TapBootstrap` 中 `TDSUser` 的部分参数，所以 `TapCommon` 在当前版本支持了原生简单的调用 Unity 接口。

以下以 Android、iOS 需要 Unity 提供 `sessionToken` 以及 `objectId` 为例

### Unity 实现 ITapPropertiesProxy 接口并注册

```c#
public class SessionTokenProxy:ITapPropertiesProxy{

    public string GetProperties(){
       return "sessionToken-kafjaskldfjasjdhfajkdfajdfas";
    }

}

public class ObjectIdProxy:ITapPropertiesProxy {
    
    public string GetProperties(){
        return "objectId-dafasdfad";
    }
    
}

// 通过 TapCommon 注册 Native 需要调用的接口
TapCommon.RegisterProperties("sessionToken",new SessionTokenProxy());

TapCommon.RegisterProperties("objectid",new ObjectIdProxy());
```

### Android、iOS 调用 Unity 实现的 ITapPropertiesProxy 来获取所需要的值

Android 获取 `sessionToken` 以及 `objectId`

```java
String sessionToken = TapPropertiesHolder.INSTANCE.getProperty("sessionToken");
String objectId = TapPropertiesHolder.INSTANCE.getProperty("objectId");
```

iOS 获取 `sessionToken` 以及 `objectId`

```objectivec
NSString* sessionToken = [[TapPropertiesHolder shareInstance] getProperty:@"sessionToken"];
NSString* objectId = [[TapPropertiesHolder shareInstance] getProperty:@"objectId"];
```







