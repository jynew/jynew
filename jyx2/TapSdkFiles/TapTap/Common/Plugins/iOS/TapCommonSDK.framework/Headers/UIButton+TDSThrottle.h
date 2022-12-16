//
//  UIButton+TDSThrottle.h
//  TDSCommon
//
//  Created by JiangJiahao on 2021/3/22.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSUInteger,TDSButtonThrottleType) {
    TDSButtonThrottleTypeNone = 0,                  // 正常调用
    TDSButtonThrottleTypeThrottleLeading,           // 节流，指定时间间隔内只能调用一次，首次调用时立即触发
    TDSButtonThrottleTypeThrottleTrailing,          // 节流，指定时间间隔内只能调用一次，首次调用超时后触发
    TDSButtonThrottleTypeDebunceLeading,            // 防抖，相邻调用间隔若小于指定时间间隔，则合并成一次调用，首次调用时立即触发
    TDSButtonThrottleTypeDebunceTrailing,           // 防抖，相邻调用间隔若小于指定时间间隔，则合并成一次调用，首次超时后触发
};

@interface UIButton (TDSThrottle)

@property (nonatomic,assign) TDSButtonThrottleType throttleType;

@end

/**
 使用方法：
 #import "UIButton+TDSThrottle.h"
 
 UIButton *testButton = [UIButton buttonWithType:UIButtonTypeCustom];
 [testButton setThrottleType:TDSButtonThrottleTypeDebunceTrailing];

 */

NS_ASSUME_NONNULL_END
