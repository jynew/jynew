//
//  TDSAccountProvider.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/3/30.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSAccount.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSAccountProvider <NSObject>

- (nullable TDSAccount *)getAccount;

- (nullable NSDictionary *)getLocalUserInfo;

- (void)getAccountUser:(void (^)(NSDictionary *_Nullable userInfo, NSError *_Nullable error))handler;
@end

NS_ASSUME_NONNULL_END
