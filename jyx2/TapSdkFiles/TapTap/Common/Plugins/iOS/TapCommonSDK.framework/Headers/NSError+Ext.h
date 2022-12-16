//
//  NSError+Ext.h
//  TapAchievement
//
//  Created by TapTap-David on 2020/9/22.
//  Copyright © 2020 taptap. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSError (Ext)
+ (instancetype)errorWithMessage:(NSString *)errorMsg code:(NSInteger)code;

+ (instancetype)errorWithContent:(NSString *)content message:(NSString *)message code:(NSInteger)code;
@end

NS_ASSUME_NONNULL_END
