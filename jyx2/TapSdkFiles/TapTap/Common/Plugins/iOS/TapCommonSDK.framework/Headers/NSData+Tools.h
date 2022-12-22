//
//  NSData+JSON.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/3/9.
//  Copyright © 2018年 JiangJiahao. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSData (JSON)

- (NSArray *)tds_arrayFromJsonData;

- (NSDictionary *)tds_dictionaryFromJsonData;

- (NSString *)tds_stringFromData;

- (NSData *)tds_aes256Encrypt:(NSString *)key;
- (NSData *)tds_aes256Decrypt:(NSString *)key;

@end
