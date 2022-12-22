//
//  TapDBDynamicPropertiesDelegate.h
//  TapDB
//
//  Created by xe on 2021/4/13.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef const char* _Nullable (*TapPropertiesDelegate)(const char*);

@interface TapPropertiesDelegateProxy : NSObject

- (instancetype)initWithDelegate: (TapPropertiesDelegate)delegate key:(NSString*)key;

- (NSString*) getProperties;

@end

NS_ASSUME_NONNULL_END
