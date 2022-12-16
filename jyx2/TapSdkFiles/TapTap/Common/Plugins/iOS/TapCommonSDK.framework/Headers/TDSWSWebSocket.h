//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  TDSWSWebSocket.h
//
//  Created by Austin and Dalton Cherry on on 5/13/14.
//  Copyright (c) 2014-2017 Austin Cherry.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
//////////////////////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSWSSecurity.h>

@class TDSWSWebSocket;

/**
 It is important to note that all the delegate methods are put back on the main thread.
 This means if you want to do some major process of the data, you need to create a background thread.
 */
@protocol TDSWSWebSocketDelegate <NSObject>

@optional
/**
 The websocket connected to its host.
 @param socket is the current socket object.
 */
-(void)websocketDidConnect:(nonnull TDSWSWebSocket*)socket;

/**
 The websocket was disconnected from its host.
 @param socket is the current socket object.
 @param error  is return an error occured to trigger the disconnect.
 */
-(void)websocketDidDisconnect:(nonnull TDSWSWebSocket*)socket error:(nullable NSError*)error;

/**
 The websocket got a text based message.
 @param socket is the current socket object.
 @param string is the text based data that has been returned.
 */
-(void)websocket:(nonnull TDSWSWebSocket*)socket didReceiveMessage:(nonnull NSString*)string;

/**
 The websocket got a binary based message.
 @param socket is the current socket object.
 @param data   is the binary based data that has been returned.
 */
-(void)websocket:(nonnull TDSWSWebSocket*)socket didReceiveData:(nullable NSData*)data;

/**
 The websocket got a pong.
 @param socket is the current socket object.
 @param data   is the binary based data that has been returned.
 */
-(void)websocket:(nonnull TDSWSWebSocket*)socket didReceivePong:(nullable NSData*)data;

@end

@interface TDSWSWebSocket : NSObject

@property(nonatomic,weak, nullable)id<TDSWSWebSocketDelegate>delegate;
@property(nonatomic, readonly, nonnull) NSURL *url;

/**
 constructor to create a new websocket with QOS_CLASS_UTILITY dispatch queue
 @param url       the host you want to connect to.
 @param protocols the websocket protocols you want to use (e.g. chat,superchat).
 @return a newly initalized websocket.
 */
- (nonnull instancetype)initWithURL:(nonnull NSURL *)url protocols:(nullable NSArray*)protocols;

/**
 constructor to create a new websocket
 @param url       the host you want to connect to.
 @param protocols the websocket protocols you want to use (e.g. chat,superchat).
 @param callbackQueue the dispatch queue for handling callbacks
 @return a newly initalized websocket.
 */
- (nonnull instancetype)initWithURL:(nonnull NSURL *)url protocols:(nullable NSArray*)protocols callbackQueue:(nonnull dispatch_queue_t)callbackQueue;

/**
 constructor to create a new websocket
 @param url       the host you want to connect to.
 @param protocols the websocket protocols you want to use (e.g. chat,superchat).
 @param callbackQueue the dispatch queue for handling callbacks
 @param connectTimeout timeout for blocking connect
 @return a newly initalized websocket.
 */
- (nonnull instancetype)initWithURL:(nonnull NSURL *)url protocols:(nullable NSArray*)protocols callbackQueue:(nonnull dispatch_queue_t)callbackQueue connectTimeout:(NSTimeInterval)connectTimeout;

/**
 connect to the host
 */
- (void)connect;

/**
 disconnect to the host. This sends the close Connection opcode to terminate cleanly.
 */
- (void)disconnect;

/**
 write binary based data to the socket.
 @param data the binary data to write.
 */
- (void)writeData:(nonnull NSData*)data;

/**
 write text based data to the socket.
 @param string the string to write.
 */
- (void)writeString:(nonnull NSString*)string;

/**
 write ping to the socket.
 @param data the binary data to write (if desired).
 */
- (void)writePing:(nonnull NSData*)data;

/**
 Add a header to send along on the the HTTP connect.
 @param value the string to send
 @param key   the HTTP key name to send
 */
- (void)addHeader:(nonnull NSString*)value forKey:(nonnull NSString*)key;

/**
 returns if the socket is conneted or not.
 */
@property(nonatomic, assign, readonly)BOOL isConnected;

/**
 Enable VOIP support on the socket, so it can be used in the background for VOIP calls.
 Default setting is No.
 */
@property(nonatomic, assign)BOOL voipEnabled;

/**
 Allows connection to self signed or untrusted WebSocket connection. Useful for development.
 Default setting is No.
 */
@property(nonatomic, assign)BOOL selfSignedSSL;

/**
 Use for SSL pinning.
 */
@property(nonatomic, strong, nullable)TDSWSSecurity *security;

/**
 Set your own custom queue.
 Default setting is dispatch_get_main_queue.
 */
@property(nonatomic, strong, nullable)dispatch_queue_t queue;

/**
 Block property to use on connect.
 */
@property(nonatomic, strong, nullable)void (^onConnect)(void);

/**
 Block property to use on disconnect.
 */
@property(nonatomic, strong, nullable)void (^onDisconnect)(NSError*_Nullable);

/**
 Block property to use on receiving data.
 */
@property(nonatomic, strong, nullable)void (^onData)(NSData*_Nullable);

/**
 Block property to use on receiving text.
 */
@property(nonatomic, strong, nullable)void (^onText)(NSString*_Nullable);

/**
 Block property to use on receiving pong.
 */
@property(nonatomic, strong, nullable)void (^onPong)(NSData*_Nullable);
@end
