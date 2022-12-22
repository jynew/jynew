//
//  TapAuthManager.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/1/27.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TapAuthResult.h>
#import <TapCommonSDK/TapAuthConfig.h>

NS_ASSUME_NONNULL_BEGIN

typedef void (^TapAuthManagerRequestHandler)(TapAuthResult *result);

FOUNDATION_EXPORT NSString * const TapTapAuthGameTag;
FOUNDATION_EXPORT NSString * const TapTap_SDK_VERSION;

@interface TapAuthManager : NSObject

@property (nonatomic, strong, nullable) TapAuthConfig * config;
@property (nonatomic, strong, nullable) TapAuthAccessToken * currentAccessToken;



+ (instancetype)sharedManager;

- (void)requestTapTapPermissions:(NSArray <NSString *>*)permissions handler:(TapAuthManagerRequestHandler)handler;

/// requestTapTapPermissions
/// @param permissions permissions needed
/// @param isInternal is or not in China
/// @param source source
/// @param clientID Your clientID
/// @param handler handler
+ (void)requestTapTapPermissions:(NSArray <NSString *>*)permissions
                          isInternal:(BOOL)isInternal
                          source:(NSString *)source
                        clientID:(NSString *)clientID
                         handler:(TapAuthManagerRequestHandler)handler;

- (void)createToken:(NSString *)code verifier:(NSString *)verifier handler:(void (^)(TapAuthAccessToken *, NSError *))handler;

//- (BOOL)handleURL:(NSURL *)url;


@end

NS_ASSUME_NONNULL_END
