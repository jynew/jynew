//
//  TapAuthResult.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/1/27.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TapAuthAccessToken.h>

typedef NS_ENUM(NSInteger, TapAuthResultType) {
    TapAuthResultTypeSuccess,
    TapAuthResultTypeCancel,
    TapAuthResultTypeError,
};

NS_ASSUME_NONNULL_BEGIN

@interface TapAuthResult : NSObject

@property (nonatomic, assign, readonly) TapAuthResultType type;

@property (nonatomic, strong, nullable, readonly) TapAuthAccessToken *token;

@property (nonatomic, strong, nullable, readonly) NSError *error;

+ (instancetype)successWithToken:(TapAuthAccessToken *)token;
+ (instancetype)failWithError:(NSError *)error;
+ (instancetype)cancel;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END
