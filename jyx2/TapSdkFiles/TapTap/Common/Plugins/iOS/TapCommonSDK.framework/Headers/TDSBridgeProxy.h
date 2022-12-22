//
//  EngineBridgeProxy.h
//  Bridge
//
//  Created by xe on 2020/10/15.
//  Copyright © 2020 xe. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSBridgeCallback.h>
#import <TapCommonSDK/TDSBridge.h>

@interface TDSBridgeProxy : NSObject<TDSBridgeCallback>

+ (TDSBridgeProxy *)shareInstance;

- (void)onResult:(NSString*) result;

@end
