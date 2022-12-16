//
//  BridgeTool.h
//  EngineBridge
//
//  Created by xe on 2020/10/9.
//  Copyright © 2020 xe. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface TDSBridgeTool : NSObject

+ (BOOL)isEmpty:(NSString *)str;

+ (NSString *)jsonStringWithString:(NSString *)string;

+ (NSString *)jsonStringWithArray:(NSArray *)array;

+ (NSString *)jsonStringWithDictionary:(NSDictionary *)dictionary;

+ (NSString *)jsonStringWithObject:(id)model;

+ (NSString *)jsonStringWithMutaDic:(NSDictionary *)dic;

+ (NSDictionary *)dictionaryWithModel:(id)model;

@end

