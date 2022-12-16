//
//  ComponentMessageDelegate.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/5/11.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
@protocol ComponentMessageDelegate <NSObject>

- (void)onMessageWithCode:(NSInteger)code extras:(NSDictionary *)extras;
@end

NS_ASSUME_NONNULL_END
