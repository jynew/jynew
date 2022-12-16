//
//  IscTapLoginService.h
//  Pods-TDSLoginSource_Example
//
//  Created by Bottle K on 2020/12/15.
//

#import <Foundation/Foundation.h>
#import <TapLoginSDK/TapLoginSDK.h>
NS_ASSUME_NONNULL_BEGIN

@interface IscTapLoginService : NSObject
+ (void)addSDKLoginResultDelegate:(NSString *)sdkName delegate:(id <TapTapLoginResultDelegate>)delegate;

+ (void)removeSDKLoginResultDelegate:(NSString *)sdkName;

+ (void)startSDKLogin:(NSString *)sdkName permission:(NSArray *)permissions;

+ (void)handleLoginError:(NSDictionary *)params;

+ (void)changeConfigWithClientId:(NSString *)clientId
                      regionType:(RegionType)region
                           token:(TTSDKAccessToken *)token;
@end

NS_ASSUME_NONNULL_END
