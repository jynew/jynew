//
//  TDSTrackerManager.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/19.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSTrackerConfig.h>
#import <TapCommonSDK/UserModel.h>
#import <TapCommonSDK/PageModel.h>
#import <TapCommonSDK/ActionModel.h>
#import <TapCommonSDK/NetworkStateModel.h>
#import <TapCommonSDK/TDSTrackerEvent.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSTrackerManager : NSObject

+ (instancetype)sharedInstance;

+ (void)registerTracker:(TDSTrackerConfig *)trackerConfig;

- (void)trackWithEvent:(TDSTrackerEvent *)event;

@end

NS_ASSUME_NONNULL_END
