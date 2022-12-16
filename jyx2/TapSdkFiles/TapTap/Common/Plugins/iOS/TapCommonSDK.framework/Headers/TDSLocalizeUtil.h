//
//  TDSLocalizeUtil.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/3/4.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSLocalizeManager.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSLocalizeUtil : NSObject
+ (NSString *)getCurrentLangString;
+ (TapLanguageType)getCurrentLangType;
@end

NS_ASSUME_NONNULL_END
