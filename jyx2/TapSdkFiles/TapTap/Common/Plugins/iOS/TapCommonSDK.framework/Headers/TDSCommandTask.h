//
//  CommandTask.h
//  EngineBridge
//
//  Created by xe on 2020/9/29.
//  Copyright © 2020 xe. All rights reserved.
//
#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSCommand.h>
NS_ASSUME_NONNULL_BEGIN

@interface TDSCommandTask : NSObject

- (void)execute:(TDSCommand *)command brigeCallback:(void (^)(NSString * resultJSON))result;

@end

NS_ASSUME_NONNULL_END

