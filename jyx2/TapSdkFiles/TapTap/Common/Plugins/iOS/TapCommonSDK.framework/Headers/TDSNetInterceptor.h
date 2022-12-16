//
//  TDSNetInterceptor.h
//  TDSCommon
//
//  Created by Bottle K on 2021/2/25.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSNetInterceptDelegate <NSObject>
@optional
- (void)receiveNetContent:(NSDictionary *)content;
@end

@interface TDSNetInterceptor : NSObject

+ (instancetype)new NS_UNAVAILABLE;

- (instancetype)init NS_UNAVAILABLE;

+ (instancetype)sharedInstance;

- (void)registerNetInterceptor:(NSString *)from delegate:(id<TDSNetInterceptDelegate>)delegate;

- (void)unRegisterNetInterceptor:(NSString *)from;

- (void)interceptWithContent:(NSDictionary *)content;

+ (void)checkAuthErrorAccessDenied:(NSDictionary *)params handler:(void (^)(NSDictionary *dataDic))handler;

+ (void)checkAuthError:(NSDictionary *)params errorList:(NSArray *)errorList handler:(void (^)(NSDictionary *dataDic))handler;
@end

NS_ASSUME_NONNULL_END
