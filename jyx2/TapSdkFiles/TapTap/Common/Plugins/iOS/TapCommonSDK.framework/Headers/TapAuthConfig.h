//
//  TapAuthConfig.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/2/7.
//

#import <Foundation/Foundation.h>

typedef NS_ENUM (NSInteger, TapAuthRegionType) {
    TapAuthRegionTypeCN,
    TapAuthRegionTypeIO
};

NS_ASSUME_NONNULL_BEGIN

@interface TapAuthConfig : NSObject

@property (nonatomic, strong, readonly) NSString * source;
@property (nonatomic, strong, readonly) NSString * clientID;
@property (nonatomic, assign, readonly) TapAuthRegionType regionType;
/// Whether it is rounded. The default value is rounded
@property (nonatomic, assign) BOOL roundCorner;

@property (nonatomic, strong, readonly) NSString * TapTapAppScheme;
@property (nonatomic, strong, readonly) NSString * mainUrl;



+ (instancetype)configWithSource:(NSString *)source
                        clientID:(NSString *)clientID
                      regionType:(TapAuthRegionType)regionType;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END
