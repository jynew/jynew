//
//  PlatformXUA.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/6/21.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface PlatformXUA : NSObject
@property (nonatomic, copy) NSDictionary *xuaMap;
+ (instancetype)shareInstance;
@end

NS_ASSUME_NONNULL_END
