//
//  BridgeCallback.h
//  Bridge
//
//  Created by xe on 2020/10/16.
//  Copyright © 2020 xe. All rights reserved.
//
#import <Foundation/Foundation.h>

@protocol TDSBridgeCallback <NSObject>

@optional

- (void)onResult:(NSString *)msg;

@end
