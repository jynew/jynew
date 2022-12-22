//
//  NSObject+TDSProperty.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSModelHelper.h>
#import <TapCommonSDK/TDSMacros.h>

NS_ASSUME_NONNULL_BEGIN

typedef void (^TDSClassesEnumerator) (Class cls, BOOL *stop);

typedef void (^TDSPropertiesEnumerator) (TDSProperty *property, BOOL *stop);

@interface NSObject (TDSProperty)
+ (void)tds_enumerateProperties:(TDSPropertiesEnumerator)enumerator;

+ (void)tds_enumerateClasses:(TDSClassesEnumerator)enumerator;

@end

NS_ASSUME_NONNULL_END
