//
//  NSDictionary+JSON.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/11.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSDictionary (JSON)
- (NSString *)tds_jsonString;
- (NSString *)tds_jsonStringWithoutOptions:(NSJSONWritingOptions)opt;
@end

NS_ASSUME_NONNULL_END
