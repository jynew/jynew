//
//  AntiAddiction.h
//  AntiAddictionUI
//
//  Created by jessy on 2021/9/22.
//

#import <Foundation/Foundation.h>
#import <AntiAddictionService/AntiAddictionService-Swift.h>
#import <AntiAddictionUI/AntiAddictionUI.h>

#define AntiAddictionSDK @"AntiAddiction"


NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, AntiAddictionResultHandlerCode) {
    AntiAddictionResultHandlerLoginSuccess          = 500,   //登录成功
    AntiAddictionResultHandlerExited                = 1000,   //登录成功
    AntiAddictionResultHandlerSwitchAccount         = 1001,  // 切换账号
    AntiAddictionResultHandlerPeriodRestrict        = 1030,  // 达到宵禁时间，不可玩游戏
    AntiAddictionResultHandlerDurationLimit         = 1050,  // 可玩时长耗尽，不可玩游戏
    AntiAddictionResultHandlerRealNameStop          = 9002,  // 实名过程中点击了关闭实名窗
    
    AntiAddictionResultHandlerLoginLogout __attribute__((deprecated("Please use AntiAddictionResultHandlerExited"))) = 1000,  //用户登出
    AntiAddictionResultHandlerTimeLimit __attribute__((deprecated("Please use AntiAddictionResultHandlerPeriodRestrict"))) = 1030,  // 用户当前无法进行游戏
    AntiAddictionResultHandlerOpenAlert __attribute__((deprecated("Not supported in future versions"))) = 1095,  // 未成年允许游戏弹窗
};

typedef NS_ENUM(NSInteger, AntiAddictionAgeLimit) {
    AntiAddictionAgeLimitUnknown            = -1,
    AntiAddictionAgeLimitChild              = 0,
    AntiAddictionAgeLimitTeen               = 8,
    AntiAddictionAgeLimitYoung              = 16,
    AntiAddictionAgeLimitAdult              = 18,
};

@protocol AntiAddictionDelegate <NSObject>

- (void)antiAddictionCallbackWithCode:(AntiAddictionResultHandlerCode)code extra:(NSString * _Nullable)extra;

@end


@interface AntiAddiction : NSObject

/// 初始化
/// @param config 防沉迷配置
/// @param delegate 回调代理
+ (void)initWithConfig:(AntiAddictionConfig *)config delegate:(id<AntiAddictionDelegate>)delegate;

/// 启动防沉迷&实名系统
/// @param userID 游戏维度的用户唯一标识
+ (void)startupWithUserID:(NSString *)userID;

/// 退出防沉迷&实名系统
+ (void)exit;

/// 进入游戏
+ (void)enterGame;

/// 离开游戏
+ (void)leaveGame;

/// 获取用户年龄段
+ (AntiAddictionAgeLimit)getAgeRange;

/// 获取用户剩余时长（单位：秒）
+ (NSInteger)getRemainingTime;

/// 获取用户剩余时长（单位：分钟）
+ (NSInteger)getRemainingTimeInMinutes;


/// 查询能否支付
/// @param amount 支付金额，单位分
/// @param resultBlock 能否成功
/// @param failureHandler 查询能否支付失败（一般网络错误）
+ (void)checkPayLimit:(NSInteger)amount resultBlock:(void(^ _Nullable)(BOOL status))resultBlock failureHandler:(void (^ _Nullable)(NSString * _Nonnull error))failureHandler;

/// 上报消费结果
/// @param amount 支付金额，单位分
/// @param callBack 上报是否成功
/// @param failureHandler 上报消费结果失败（一般网络错误）
+ (void)submitPayResult:(NSInteger)amount callBack:(void(^ _Nullable)(BOOL success))callBack failureHandler:(void (^ _Nullable)(NSString * _Nonnull error))failureHandler;

///获取当前防沉迷Token
+ (NSString *)currentToken;



+ (void)initGameIdentifier:(NSString *)gameIdentifier antiAddictionConfig:(AntiAddictionConfiguration *)config  antiAddictionCallbackDelegate:(id<AntiAddictionDelegate>)delegate completionHandler:(void(^)(BOOL))com __attribute__((deprecated("Please use [AntiAddiction initWithConfig:delegate:]")));
+ (void)startUpUseTapLogin:(BOOL)useTapLogin userIdentifier:(NSString *)userIdentifier __attribute__((deprecated("Please use [AntiAddiction startupWithUserID:]")));
+ (NSInteger)getCurrentUserAgeLimite __attribute__((deprecated("Please use [AntiAddiction getAgeRange]")));
+ (NSInteger)getCurrentUserRemainTime __attribute__((deprecated("Please use [AntiAddiction getRemainingTime]")));
+ (void)checkPayLimit:(NSInteger)amount callBack:(void (^ _Nullable)(BOOL status, NSString * _Nonnull title, NSString *  _Nonnull description))callBack failureHandler:(void (^ _Nullable)(NSString * _Nonnull))failureHandler __attribute__((deprecated("Please use [AntiAddiction checkPayLimit:resultBlock:failureHandler:]")));
+ (BOOL)isStandAlone __attribute__((deprecated("Not supported in future versions")));
+ (void)logout __attribute__((deprecated("Please use [AntiAddiction exit]")));
+ (NSString *)getSDKVersion __attribute__((deprecated("Please use AntiAddictionSDK_VERSION")));

///   获取实名信息
/// @param gameIdentifier  游戏唯一标识
/// @param userIdentifier  游戏维度的用户唯一标识
/// @param successHandelr AntiAddictionRealNameAuthState 用户状态，antiAddictionToken 防沉迷Token，AntiAddictionRealNameAgeLimit 年龄区分
/// @param failureHandler msg 错误信息
+ (void)fetchUserIndentifyInfoGameIdentifier:(NSString *)gameIdentifier userIdentifier:(NSString *)userIdentifier
                              successHandler:(void(^)(AntiAddictionRealNameAuthState state, NSString *antiAddictionToken, AntiAddictionRealNameAgeLimit ageLimit))successHandelr
                              failureHandler:(void(^)(NSString *msg))failureHandler __attribute__((deprecated("Not supported in future versions")));


+ (void)setUnityVersion:(NSString *)version;

@end

NS_ASSUME_NONNULL_END
