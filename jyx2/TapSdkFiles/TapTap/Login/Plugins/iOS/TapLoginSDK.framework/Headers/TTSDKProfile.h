//
//  TTSDKProfile.h
//  TapTapSDK
//
//  Created by TapTap on 2017/10/27.
//  Copyright © 2017年 易玩. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  @brief TapTap用户信息封装类
 *
 *  该类封装了所有用户信息提供的返回数据
 */
@interface TTSDKProfile : NSObject

/// 用户名
@property (nonatomic, readonly, strong) NSString *name;

/// 用户头像
@property (nonatomic, readonly, strong) NSString *avatar;

/// open id
@property (nonatomic, readonly, strong) NSString *openid;

/// union id
@property (nonatomic, readonly, strong) NSString *unionid;

@property (nonatomic, readonly, strong) NSString *email;

@property (nonatomic, readonly, assign, getter = isEmailVerified) BOOL emailVerified;

- (instancetype)initWithJSON:(NSDictionary *)json;

/**
 *  @brief 获取当前用户信息
 *
 *  该用户信息会优先读取本地缓存，不存在时将会返回nil
 */
+ (TTSDKProfile *)currentProfile;

+ (void)fetchProfileForCurrentAccessToken:(void (^)(TTSDKProfile *profile, NSError *error))handler;

- (NSString *)toJsonString;
@end
