//
//  NSDictionary+TDSSafe.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSDictionary (TDSSafe)

- (BOOL)tds_boolForKey:(NSString *)key;
- (NSInteger)tds_integerForKey:(NSString *)key;
- (NSDictionary *)tds_dicForKey:(NSString *)key;
- (NSString *)tds_stringForKey:(NSString *)key;
- (NSArray *)tds_arrayForKey:(NSString *)key;
- (NSSet *)tds_setForKey:(NSString *)key;
- (NSNumber *)tds_numberForKey:(NSString *)key;

@end

NS_ASSUME_NONNULL_END
