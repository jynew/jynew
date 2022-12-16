//
//  TapTapLoginResultDelegate.h
//  Pods-TDSLoginSource_Example
//
//  Created by Bottle K on 2020/12/15.
//

#import <Foundation/Foundation.h>
#import <TapLoginSDK/AccountGlobalError.h>
#import <TapLoginSDK/TTSDKAccessToken.h>
NS_ASSUME_NONNULL_BEGIN

@protocol TapTapLoginResultDelegate <NSObject>

/// 登录成功回调
/// @param token token对象
- (void)onLoginSuccess:(TTSDKAccessToken *)token;

/// 登录取消
- (void)onLoginCancel;

/// 登录失败
/// @param error 失败原因
- (void)onLoginError:(AccountGlobalError *)error;

@end

NS_ASSUME_NONNULL_END
