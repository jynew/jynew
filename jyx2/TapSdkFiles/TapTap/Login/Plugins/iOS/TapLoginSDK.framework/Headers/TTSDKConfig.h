//
//  TTSDKConfig.h
//  TapTapSDK
//
//  Created by wzb on 2020/5/13.
//  Copyright © 2020 易玩. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

#define TapLoginSDKErrorDomain @"TapLoginSDKErrorDomain"

typedef NS_ENUM (NSInteger, RegionType) {
    RegionTypeCN,
    RegionTypeIO
};

@interface TTSDKConfig : NSObject

/// 是否为圆角，默认为圆角
@property (nonatomic, assign) BOOL roundCorner;
/// 限定登录客户端
@property (nonatomic, assign) RegionType regionType;
/// Server URL
@property (nonatomic, copy) NSString *serverURL;

@end

NS_ASSUME_NONNULL_END
