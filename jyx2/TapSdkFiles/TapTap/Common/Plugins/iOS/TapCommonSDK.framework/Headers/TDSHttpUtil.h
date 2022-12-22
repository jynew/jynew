//
//  TTAchievementUtil.h
//  TapAchievement
//
//  Created by TapTap-David on 2020/9/15.
//  Copyright © 2020 taptap. All rights reserved.
//

#import <Foundation/Foundation.h>


NS_ASSUME_NONNULL_BEGIN

@interface TDSHttpUtil : NSObject
+ (NSString *)URLEncodeString:(NSString *)str;

+ (NSString *)URLDecodeString:(NSString *)str;

+ (NSString *)urlEncode:(NSString *)str;

+ (NSString *)SHA256:(NSString *)key;

+ (NSString *)md5String:(NSString *)str;

+ (NSString *)getCurrentTime;

+ (NSString *)randomString:(int)length;

+ (NSString *)base64HMacSha1WithSecret:(NSString *)secret
                            signString:(NSString *)signString;

+ (NSString *)getLcSignWithClientKey:(NSString *)clientKey;

+ (NSString *)getMacToken:(NSString *)url
     method:(NSString *)method
    oauthID:(NSString *)oauthID
oauthMacKey:(NSString *)oauthMacKey;

+ (NSString *)getDeviceId;

+ (NSString *)createACodeVerifier:(NSUInteger)length;

+ (NSDictionary *)queryDictionaryFromURL:(NSURL *)url;

@end

NS_ASSUME_NONNULL_END
