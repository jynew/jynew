//
//  TTSDKLoginResult.h
//  TapTapSDK
//
//  Created by TapTap on 2017/10/17.
//  Copyright © 2017年 易玩. All rights reserved.
//

#import <Foundation/Foundation.h>

@class TTSDKAccessToken;

/**
 *  @brief 登入结果
 *
 *  该类封装了登入的响应结果（非NSError情况下）
 */
@interface TTSDKLoginResult : NSObject

/// 授权Token
@property (nonatomic, copy) TTSDKAccessToken *token;

/// 用户是否选择取消授权（非拒绝授权，拒绝授权将在NSError中进行返回）
@property (nonatomic, readonly) BOOL isCancelled;

- (instancetype)initWithToken:(TTSDKAccessToken *)token
                  isCancelled:(BOOL)isCancelled;

@end
