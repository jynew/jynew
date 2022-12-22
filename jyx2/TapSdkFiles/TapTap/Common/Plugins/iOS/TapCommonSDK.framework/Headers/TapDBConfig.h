//
//  TapDBConfig.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/4/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TapDBConfig : NSObject
@property (nonatomic, assign) BOOL enable;
@property (nonatomic, copy) NSString *channel;
@property (nonatomic, copy) NSString *gameVersion;
@property (nonatomic, copy) NSDictionary *deviceLoginProperties;
@property (nonatomic, assign, getter=isAdvertiserIDCollectionEnabled) BOOL advertiserIDCollectionEnabled;

@end

NS_ASSUME_NONNULL_END
