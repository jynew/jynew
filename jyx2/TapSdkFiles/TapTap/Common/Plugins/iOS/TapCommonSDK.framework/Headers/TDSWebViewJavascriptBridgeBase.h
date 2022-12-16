//
//  TapTapForumWebViewJavascriptBridgeBase.h
//
//  Created by @LokiMeyburg on 10/15/14.
//  Copyright (c) 2014 @LokiMeyburg. All rights reserved.
//

#import <Foundation/Foundation.h>

#define kOldProtocolScheme @"wvjbscheme"
#define kNewProtocolScheme @"https"
#define kQueueHasMessage   @"__wvjb_queue_message__"
#define kBridgeLoaded      @"__bridge_loaded__"

typedef void (^TDSWVJBResponseCallback)(id responseData);
typedef void (^TDSWVJBHandler)(id data, TDSWVJBResponseCallback responseCallback);
typedef NSDictionary ForumWVJBMessage;

@protocol TDSWebViewJavascriptBridgeBaseDelegate <NSObject>
- (NSString*) _evaluateJavascript:(NSString*)javascriptCommand;
@end

@interface TDSWebViewJavascriptBridgeBase : NSObject


@property (weak, nonatomic) id <TDSWebViewJavascriptBridgeBaseDelegate> delegate;
@property (strong, nonatomic) NSMutableArray* startupMessageQueue;
@property (strong, nonatomic) NSMutableDictionary* responseCallbacks;
@property (strong, nonatomic) NSMutableDictionary* messageHandlers;
@property (strong, nonatomic) TDSWVJBHandler messageHandler;

+ (void)enableLogging;
+ (void)setLogMaxLength:(int)length;
- (void)reset;
- (void)sendData:(id)data responseCallback:(TDSWVJBResponseCallback)responseCallback handlerName:(NSString*)handlerName;
- (void)flushMessageQueue:(NSString *)messageQueueString;
- (void)injectJavascriptFile;
- (BOOL)isWebViewJavascriptBridgeURL:(NSURL*)url;
- (BOOL)isQueueMessageURL:(NSURL*)urll;
- (BOOL)isBridgeLoadedURL:(NSURL*)urll;
- (void)logUnkownMessage:(NSURL*)url;
- (NSString *)webViewJavascriptCheckCommand;
- (NSString *)webViewJavascriptFetchQueyCommand;
- (void)disableJavscriptAlertBoxSafetyTimeout;

@end
