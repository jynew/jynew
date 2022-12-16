//
//  TDSNetSubscriber.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/22.
//

#import <Foundation/Foundation.h>

@protocol TDSNetSubscriber <NSObject>
@optional

- (void)sendSuccess:(nullable id)value;

- (void)sendFailure:(nullable NSError *)error;

- (void)sendProgress:(nullable id)progress;

@end

NS_ASSUME_NONNULL_BEGIN

@interface TDSNetSubscriber : NSObject<TDSNetSubscriber>

+ (instancetype)subscriberWithSuccess:(void (^)(id x))success failure:(nullable void (^)(NSError *error))error progress:(nullable void (^)(id progress))progress;

@end

NS_ASSUME_NONNULL_END
