//
//  TDSTrackerConfig.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/15.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
typedef enum : NSUInteger {
    TDSTrackerForTapsdk = 1,
    TDSTrackerForFriends,
    TDSTrackerForNetwork,
    TDSTrackerForTapSDKNetwork
} TDSTrackerType;

@interface TDSTrackerConfig : NSObject
@property (nonatomic, copy) NSString *accessKeyId;
@property (nonatomic, copy) NSString *accessKeySecret;
@property (nonatomic, copy) NSString *project;
@property (nonatomic, copy) NSString *endPoint;
@property (nonatomic, copy) NSString *logStore;
@property (nonatomic, copy) NSString *sdkVersionName;
@property (nonatomic, assign) TDSTrackerType trackerType;
@property (nonatomic, assign) NSInteger groupSize;
@end

NS_ASSUME_NONNULL_END
