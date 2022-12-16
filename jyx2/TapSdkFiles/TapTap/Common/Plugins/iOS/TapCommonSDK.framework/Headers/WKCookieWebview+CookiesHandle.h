//
//  WKCookieWebview+CookiesHandle.h
//  NativeApp
//
//  Created by JiangJiahao on 2019/4/3.
//  Copyright © 2019 JiangJiahao. All rights reserved.
//

#import <TapCommonSDK/TDSWKCookieWebview.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSWKCookieWebview (CookiesHandle)

- (void)syncCookies:(NSURLRequest *)request task:(nullable NSURLSessionTask *)task complitionHandle:(void(^)(NSURLRequest *newRequest))complitionHandle;

- (void)syncCookiesInJS:(nullable NSURLRequest *)request;

@end

NS_ASSUME_NONNULL_END
