//
//  TapDBDynamicPropertiesProxy.h
//  TapDB
//
//  Created by xe on 2021/4/13.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TapPropertiesDelegateProxy.h>

NS_ASSUME_NONNULL_BEGIN

@interface TapPropertiesHolder : NSObject

@property NSMutableDictionary<NSString*,TapPropertiesDelegateProxy*> *dic;

+ (TapPropertiesHolder*)shareInstance;

- (NSString*)getProperty:(NSString*)key;

@end

NS_ASSUME_NONNULL_END
