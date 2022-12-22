//
//  TDSRegionApi.h
//  TDSCommon
//
//  Created by TapTap-David on 2020/11/18.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSRegionNetClient.h>
NS_ASSUME_NONNULL_BEGIN

@interface TDSRegionApi : NSObject
+ (TDSNetExecutor *)getDeviceRegion:(NSInteger)carrier bundleId:(NSString *)bundleId;
@end

NS_ASSUME_NONNULL_END
