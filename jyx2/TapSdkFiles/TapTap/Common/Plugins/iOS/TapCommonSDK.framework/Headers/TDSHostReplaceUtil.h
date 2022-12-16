//
//  TDSHostReplaceUtil.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/8/3.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSHostReplaceUtil : NSObject
+ (instancetype)shareInstance;

- (void)addReplacedHostPair:(NSString *)hostToBeReplaced replacedHost:(NSString *)replacedHost;

- (void)clearReplacedHostPair:(NSString *)hostToBeReplaced;

- (void)clear;

- (NSString *)getReplacedHost:(NSString *)originalHost;

- (BOOL)isTestMode;
@end

NS_ASSUME_NONNULL_END
