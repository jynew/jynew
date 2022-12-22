//
//  TDSThrottle.h
//  TDSCommon
//
//  Created by JiangJiahao on 2021/3/22.
//

#import <Foundation/Foundation.h>

/** 自定管理执行与释放时，标记调用来源： 对象/类地址_方法名_调用行数 */
#define TDSThrottleKey [NSString stringWithFormat:@"%p_%s_%d",self,__func__,__LINE__]
#define TDSThrottleKeyAppendCustom(customKey) [NSString stringWithFormat:@"%p_%s_%d%@",self,__func__,__LINE__,customKey]

NS_ASSUME_NONNULL_BEGIN

typedef void(^TDSThrottleTaskBlock)(void);
extern double const THROTTLE_INTERVAL;  // 默认频率间隔， 0.5s

/**
 相邻调用必须间隔超时时间以上才会触发
 */
@interface TDSThrottle : NSObject
#pragma mark - 自动管理
+ (TDSThrottle *)throttleWithThrottleKey:(NSString *)throttleKey taskBlock:(TDSThrottleTaskBlock)taskBlock;

+ (TDSThrottle *)throttleWithInterval:(NSTimeInterval)interval
                         throttleKey:(NSString *)throttleKey
                           taskBlock:(TDSThrottleTaskBlock)taskBlock;

/// 自动执行一个Throttle(节流)任务。
/// 注意：适用调用不是异常频繁的任务，如用户按钮频繁点击限制
/// @param interval 防抖间隔，默认0.5s
/// @param queue 任务执行队列，默认主队列
/// @param throttleKey 任务来源标识，可使用默认宏 TDSThrottleKey 或 TDSThrottleKeyAppendCustom
/// @param taskBlock 需要执行的任务
+ (TDSThrottle *)throttleWithInterval:(NSTimeInterval)interval
                             onQueue:(dispatch_queue_t)queue
                         throttleKey:(NSString *)throttleKey
                           taskBlock:(TDSThrottleTaskBlock)taskBlock;
#pragma mark - 手动管理
+ (TDSThrottle *)manualThrottleWithTaskBlock:(TDSThrottleTaskBlock)taskBlock;

+ (TDSThrottle *)manualThrottleWithInterval:(NSTimeInterval)interval
                               taskBlock:(TDSThrottleTaskBlock)taskBlock;

/// 手动获取一个Throttle(节流)任务,需要在不再使用时手动调用 dispose 释放
/// 注意：适合在任务会异常频繁执行时进行限制，如滑动列表时在频繁系统回调中处理的任务
/// @param interval 抖间隔，默认0.5s
/// @param queue 任务执行队列，默认主队列
/// @param taskBlock 需要执行的任务
+ (TDSThrottle *)manualThrottleWithInterval:(NSTimeInterval)interval
                                 onQueue:(dispatch_queue_t)queue
                               taskBlock:(TDSThrottleTaskBlock)taskBlock;

#pragma mark - 执行与销毁
/// 触发任务执行，手动管理时调用
- (void)invoke;

/// 销毁任务，手动管理时调用
- (void)dispose;

@end

#pragma mark - private classes
/**
 调用后立即触发，间隔时间未超时无法再次触发，每次调用不会重新计时
 */
@interface TDSThrottleLeading : TDSThrottle

@end

/**
 调用后等待间隔时间超时以后触发，每次调用不会重新计时
 */
@interface TDSThrottleTrailing : TDSThrottle

@end

/**
 使用方法：
 1.自动管理（自动执行/释放）
 // 按钮事件或需要执行的函数，任务使用 Throttle 包裹
 - (void)testThrottle {
     [TDSThrottleLeading throttleWithInterval:0.8 throttleKey:TDSThrottleKey taskBlock:^{
         // TODO 想要执行的任务
     }];
 }
 
 2.手动管理 (创建时调用 manual 开头函数)
 
 @property (nonatomic, strong) TDSThrottle *testThrottler;

 // 按钮事件或需要执行的函数，任务使用 Throttle 包裹
 - (void)testThrottle {
     if (!self.testThrottler) {
         self.testThrottler = [TDSThrottleLeading manualThrottleWithTaskBlock:^{
             // TODO 想要执行的任务
         }];
     }
     [self.testThrottler invoke];
 }
 
 // 在适当时机，如退出页面时释放
 [self.testThrottler dispose];
 */
NS_ASSUME_NONNULL_END

