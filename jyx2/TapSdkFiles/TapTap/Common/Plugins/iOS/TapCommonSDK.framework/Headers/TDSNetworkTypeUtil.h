//
//  TDSNetworkTypeUtil.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/9/7.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSNetworkTypeUtil : NSObject
+ (instancetype)shareInstance;
- (NSString *)getMobileType;
- (NSInteger)getNetworkType;
@end

NS_ASSUME_NONNULL_END
