//
//  TDSLoggerService.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/30.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSLoggerService : NSObject

+ (void)log:(NSString *)config tag:(NSString *)tag message:(NSString *)message;
@end

NS_ASSUME_NONNULL_END
