//
//  Result.h
//  EngineBridge
//
//  Created by xe on 2020/9/28.
//  Copyright © 2020 xe. All rights reserved.
//


#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSResult : NSObject

@property (nonatomic,assign) int code;
@property (nonatomic,copy) NSString* message;
@property (nonatomic,copy) NSString* content;
@property (nonatomic,copy) NSString* callbackId;
@property (nonatomic,assign) BOOL onceTime;

+ (TDSResult*)code:(int)code
           content:(NSString*)content
        callbackId:(NSString*)callbackId
           message:(NSString*)message;

+ (TDSResult*)code:(int)code
           content:(NSString*)content
        callbackId:(NSString*)callbackId
           message:(NSString*)message
          onceTime:(BOOL) onceTime;

- (NSString*)toJSON;

@end

NS_ASSUME_NONNULL_END
