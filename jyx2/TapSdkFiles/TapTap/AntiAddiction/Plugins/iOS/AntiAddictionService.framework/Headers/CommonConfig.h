//
//  CommonConfig.h
//  AntiAddictionService
//
//  Created by jessy on 2021/9/22.
//  Copyright © 2021 JiangJiahao. All rights reserved.
//

#import <Foundation/Foundation.h>
//#import <AntiAddictionService/AntiAddictionService-Swift.h>

NS_ASSUME_NONNULL_BEGIN

@interface CommonConfig : NSObject

//+ (void)dataFromJson;

// 老逻辑，暂时不动
+ (void)dataFromJsonWithConfig:(NSDictionary *)data_dict;

+ (void)saveConfigToJsonFile:(NSDictionary *)dic;
+ (nullable NSDictionary *)loadConfigFromJsonFile;

@end

NS_ASSUME_NONNULL_END
