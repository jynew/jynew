//
//  WKCookieWebview.h
//  NativeApp
//
//  Created by JiangJiahao on 2019/4/3.
//  Copyright © 2019 JiangJiahao. All rights reserved.
//  处理wkwebview的cookie问题

#import <Foundation/Foundation.h>
#import <WebKit/WebKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSWKCookieWebview : WKWebView
- (id)initWithFrame:(CGRect)frame configuration:(WKWebViewConfiguration *)configuration useRedirectCookie:(BOOL)useRedirectCookie;

@end

NS_ASSUME_NONNULL_END
