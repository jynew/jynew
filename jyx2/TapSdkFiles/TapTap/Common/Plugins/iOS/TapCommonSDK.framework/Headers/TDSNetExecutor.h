//
//  TDSNetExecutor.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/21.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSNetSubscriber.h>

typedef NS_ENUM (NSUInteger, TDSNetMethod) {
    TDSNetMethodGet = 1,
    TDSNetMethodPost
};

NS_ASSUME_NONNULL_BEGIN

@interface TDSNetRequestModel : NSObject

@property (nonatomic, assign) TDSNetMethod method;

@property (nonatomic, copy) NSString *url;

@property (nonatomic, strong) id args;

@property (nonatomic, strong) NSData *data; //protobuf Data数据

@property (nonatomic, assign) BOOL auth;

@property (nonatomic) Class resCls;

@property (nonatomic, copy, nullable) NSDictionary *header;

//是否使用原始网络返回结果
@property (nonatomic, assign) BOOL useOriginResponse;

@end

@interface TDSNetExecutor<__covariant T>: NSObject

+ (TDSNetExecutor *)create:(void(NS_NOESCAPE ^)(id<TDSNetSubscriber> subscriber))didSubscribe;

- (void)success:(void(NS_NOESCAPE ^)(id _Nonnull x))success;

- (void)success:(void(NS_NOESCAPE ^)(id _Nonnull x))success failure:(void (^)(NSError *error))failure;

- (void)success:(void(NS_NOESCAPE ^)(id _Nonnull x))success failure:(void (^)(NSError *error))failure progress:(void(NS_NOESCAPE ^)(id progress))progress;

@end

NS_ASSUME_NONNULL_END
