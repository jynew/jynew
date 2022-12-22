//
//  TDSCommonUtils.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/18.
//

#import <Foundation/Foundation.h>
#import "TDSTrackerConfig.h"

NS_ASSUME_NONNULL_BEGIN

@interface TDSCommonUtils : NSObject
+ (NSData *)lz4Compress:(NSData *)rawData;

+ (uint32_t)transformTime;

+ (NSString *)md5HexDigest:(NSData *)data;

+ (NSString *)getDeviceIdentifier;

+ (NSString *)getHardParam;

+ (NSString *)getNetworkType;

+ (NSString *)getNetWorkStatus:(NSString *)hostName;

+ (NSString *)getTotalMemorySize:(unsigned long long)fileSize;

+ (NSString *)getTotalDiskSize;

+ (NSString *)localeIdentifier;

+ (NSString *)getCpuInfo;

+ (NSString *)topic:(TDSTrackerType)type;
@end

NS_ASSUME_NONNULL_END
