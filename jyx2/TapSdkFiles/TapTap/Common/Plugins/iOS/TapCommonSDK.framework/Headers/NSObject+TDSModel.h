//
//  NSObject+TDSModel.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSModel <NSObject>
@optional

+ (NSDictionary *)replacedKeyFromPropertyName;
+ (NSString *)replacedKeyFromPropertyName:(NSString *)propertyName;
+ (NSDictionary *)objectClassInArray;
+ (Class)objectClassInArray:(NSString *)propertyName;

@end

@interface NSObject (TDSModel) <TDSModel>

+ (instancetype)tds_modelWithKeyValues:(id)keyValues;

- (NSDictionary *)tds_keyValues;

@end

NS_ASSUME_NONNULL_END
