//
//  TapLoginHelper.h
//  TapTapLoginSource
//
//  Created by Bottle K on 2020/12/2.
//

#import <Foundation/Foundation.h>
#import <TapLoginSDK/TTSDKConfig.h>
#import <TapLoginSDK/TTSDKAccessToken.h>
#import <TapLoginSDK/TTSDKProfile.h>
#import <TapLoginSDK/TTSDKLoginResult.h>
#import <TapLoginSDK/AccountGlobalError.h>
#import <TapLoginSDK/TapTapLoginResultDelegate.h>

#define TapLoginSDK @"TapLogin"
#define TapLoginSDK_VERSION_NUMBER @"31605001"
#define TapLoginSDK_VERSION        @"3.16.5"

NS_ASSUME_NONNULL_BEGIN

@interface TapLoginHelper : NSObject

/// 初始化
/// @param clientID clientID
+ (void)initWithClientID:(NSString *)clientID;

/// 初始化
/// @param clientID clientID
/// @param config 配置项
+ (void)initWithClientID:(NSString *)clientID config:(TTSDKConfig *_Nullable)config;

/// 修改登录配置
/// @param config 配置项
+ (void)changeTapLoginConfig:(TTSDKConfig *_Nullable)config;

/// 设置登录回调
/// @param delegate 回调
+ (void)registerLoginResultDelegate:(id <TapTapLoginResultDelegate>)delegate;

/// 移除登录回调
+ (void)unregisterLoginResultDelegate;

/// 获取当前设置的登录回调
+ (id <TapTapLoginResultDelegate>)getLoginResultDelegate;

/// 开始登录流程
/// @param permissions 权限列表
+ (void)startTapLogin:(NSArray *)permissions;

/// 获取当前 Token
+ (TTSDKAccessToken *)currentAccessToken;

/// 获取当前 Profile
+ (TTSDKProfile *)currentProfile;

/// 获取当前服务器上最新的 Profile
/// @param callback 回调
+ (void)fetchProfileForCurrentAccessToken:(void (^)(TTSDKProfile *profile, NSError *error))callback;

/// 登出
+ (void)logout;

/// 获取当前用户是否有测试资格
/// @param callback 回调
+ (void)getTestQualification:(void (^)(BOOL isQualified, NSError *_Nullable error))callback;

/// 当前是否有国内客户端支持
+ (BOOL)isTapTapClientSupport;

/// 当前是否有国外客户端支持
+ (BOOL)isTapTapGlobalClientSupport;

/// 监听 url 回调
/// @param url url
+ (BOOL)handleTapTapOpenURL:(NSURL *)url __attribute__((deprecated("Please use [TDSHandleUrl handleOpenURL:]")));
@end

NS_ASSUME_NONNULL_END
