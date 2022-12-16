//
//  TDSUrlSafe.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/3/25.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSUrlSafe : NSObject

// base64 url 编码
+ (nullable NSString *)base64UrlEncoder:(NSData *)data;

// base64 url 解码
+ (nullable NSData *)base64UrlDecoder:(NSString *)str;

@end

NS_ASSUME_NONNULL_END
