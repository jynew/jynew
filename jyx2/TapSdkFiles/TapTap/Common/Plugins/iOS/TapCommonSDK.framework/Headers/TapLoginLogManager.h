//
//  TapLoginLogManager.h
//  TapLoginSDK
//
//  Created by Bottle K on 2021/6/22.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TdsAccountProvider.h>
#import <TapCommonSDK/TDSTrackerEvent.h>

NS_ASSUME_NONNULL_BEGIN

@interface TapLoginLogManager : NSObject

+ (instancetype)sharedInstance;

- (TDSTrackerEvent *)generateNewEvent;

- (TDSTrackerEvent *)event;

+ (void)logStart;

+ (void)logTapOpenWithType:(NSString *)type;

+ (void)logTapBack;

+ (void)logTapToken;

+ (void)logTapProfileWithOpenId:(NSString *_Nullable)openId;

+ (void)logTapSuccess;

+ (void)logTapFailWithError:(NSError *)error;

+ (void)logTapCancel;

@end

NS_ASSUME_NONNULL_END
