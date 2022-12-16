//
//  Command.h
//  EngineBridge
//
//  Created by xe on 2020/9/28.
//  Copyright © 2020 xe. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN


@interface TDSCommand : NSObject

@property (nonatomic,copy) NSString* service;
@property (nonatomic,copy) NSString* method;
@property (nonatomic,copy) NSString* args;
@property (nonatomic,copy) NSString* callbackId;
@property (nonatomic,assign) BOOL callback;
@property (nonatomic,assign) BOOL onceTime;

+ (TDSCommand*)constructorCommand:(NSString*)commandJSON;

- (NSString*)toJSON;

@end

NS_ASSUME_NONNULL_END
