//
//  TTSDKAccessToken.h
//  TapTapSDK
//
//  Created by TapTap on 2017/10/17.
//  Copyright © 2017年 易玩. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  @brief TapTap登录授权数据封装类
 *
 *  该类封装了所有授权提供的返回数据
 */
@interface TTSDKAccessToken : NSObject

/// 唯一标志
@property (nonatomic, copy) NSString * kid;

/// 认证码
@property (nonatomic, copy) NSString * accessToken;

/// 认证码类型
@property (nonatomic, copy) NSString * tokenType;

/// mac密钥
@property (nonatomic, copy) NSString * macKey;

/// mac密钥计算方式
@property (nonatomic, copy) NSString * macAlgorithm;

/// 用户授权的权限，多个时以逗号隔开
@property (nonatomic, copy) NSString * scope;

/// 用户授权的权限 Array 形式
@property (nonatomic, copy) NSArray<NSString *> * scopeArray;

/// 根据JSON生成 TTSDKAccessToken
/// @param accessTokenString json字符串类型的AccessToken
+ (TTSDKAccessToken *)build:(NSString *)accessTokenString;

/// 通过参数生成实例
+ (TTSDKAccessToken *)build:(NSString *)kid accessToken:(NSString *)accessToken tokenType:(NSString *)tokenType macKey:(NSString *)macKey macAlgorithm:(NSString *)macAlgorithm;

+ (TTSDKAccessToken *)build:(NSString *)kid accessToken:(NSString *)accessToken tokenType:(NSString *)tokenType macKey:(NSString *)macKey macAlgorithm:(NSString *)macAlgorithm scope:(NSArray *)scope;

/// 转换成json字符串
- (NSString *)toJsonString;

+ (NSArray *)scopeStringToArray:(NSString *)scopeString;

/**
 *  @brief 获取当前认证
 *
 *  该认证会优先读取本地缓存，不存在时将会返回nil
 */
+ (TTSDKAccessToken *)currentAccessToken;

+ (void)setCurrentToken:(TTSDKAccessToken *)token;

@end
