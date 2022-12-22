//
//  TDSCommonHeader.h
//  TDSCommon
//
//  Created by Bottle K on 2020/9/29.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSCommonHeader : NSObject

- (instancetype)init:(NSString *)sdkName
      sdkVersionCode:(NSString *)sdkVersionCode
      sdkVersionName:(NSString *)sdkVersionName;

- (NSString *)getXUAValue;
@end

NS_ASSUME_NONNULL_END
