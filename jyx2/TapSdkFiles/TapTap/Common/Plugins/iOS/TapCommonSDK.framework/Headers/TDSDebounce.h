//
//  TDSDebounce.h
//  TDSCommon
//
//  Created by JiangJiahao on 2021/3/22.
//


#import <Foundation/Foundation.h>

/** 自定管理执行与释放时，标记调用来源： 对象/类地址_方法名_调用行数 */
#define TDSDebunceKey [NSString stringWithFormat:@"%p_%s_%d",self,__func__,__LINE__]
#define TDSDebunceKeyAppendCustom(customKey) [NSString stringWithFormat:@"%p_%s_%d%@",self,__func__,__LINE__,customKey]

NS_ASSUME_NONNULL_BEGIN

typedef void(^TDSDebounceTaskBlock)(void);
extern double const DEBOUNCE_INTERVAL;  // 默认频率间隔，0.5s

/**
 相邻调用若都在间隔时间内，则合并成一次调用
 */
@interface TDSDebounce : NSObject
#pragma mark - 自动管理
+ (TDSDebounce *)debounceWithDebounceKey:(NSString *)debounceKey taskBlock:(TDSDebounceTaskBlock)taskBlock;

+ (TDSDebounce *)debounceWithInterval:(NSTimeInterval)interval
                         debounceKey:(NSString *)debounceKey
                           taskBlock:(TDSDebounceTaskBlock)taskBlock;

/// 自动执行一个Debounce(防抖)任务。
/// 注意：适用调用不是异常频繁的任务，如用户按钮频繁点击限制
/// @param interval 防抖间隔，默认0.5s
/// @param queue 任务执行队列，默认主队列
/// @param debounceKey 任务来源标识，可使用默认宏 TDSDebunceKey 或 TDSDebunceKeyAppendCustom
/// @param taskBlock 需要执行的任务
+ (TDSDebounce *)debounceWithInterval:(NSTimeInterval)interval
                             onQueue:(dispatch_queue_t)queue
                         debounceKey:(NSString *)debounceKey
                           taskBlock:(TDSDebounceTaskBlock)taskBlock;
#pragma mark - 手动管理
+ (TDSDebounce *)manualDebounceWithTaskBlock:(TDSDebounceTaskBlock)taskBlock;

+ (TDSDebounce *)manualDebounceWithInterval:(NSTimeInterval)interval
                           taskBlock:(TDSDebounceTaskBlock)taskBlock;

/// 手动获取一个Debounce(防抖)任务,需要在不再使用时手动调用 dispose 释放
/// 注意：适合在任务会异常频繁执行时进行限制
/// @param interval 抖间隔，默认0.5s
/// @param queue 任务执行队列，默认主队列
/// @param taskBlock 需要执行的任务
+ (TDSDebounce *)manualDebounceWithInterval:(NSTimeInterval)interval
                             onQueue:(dispatch_queue_t)queue
                           taskBlock:(TDSDebounceTaskBlock)taskBlock;
#pragma mark - 执行与释放
/// 触发任务执行，手动管理时调用
- (void)invoke;

/// 销毁任务，手动管理时调用
- (void)dispose;

@end

#pragma mark - private classes
/**
 调用后等待间隔时间超时以后触发，每次触发后重新计时
 */
@interface TDSDebounceTrailing : TDSDebounce

@end

/**
 调用后立即触发，间隔时间未超时无法再次触发，每次触发后重新计时
 */
@interface TDSDebounceLeading : TDSDebounce

@end

/**
 使用方法：
 1.自动管理（自动执行/释放）
 // 按钮事件或需要执行的函数，任务使用 Debounce 包裹
 - (void)testDebounce {
     [TDSDebounceLeading debounceWithInterval:0.8 debounceKey:TDSDebunceKey taskBlock:^{
         // TODO 想要执行的任务
     }];
 }
 
 2.手动管理 (创建时调用 manual 开头函数)
 
 @property (nonatomic, strong) TDSDebounce *testDebouncer;

 // 按钮事件或需要执行的函数，任务使用 Debounce 包裹
 - (void)testDebounce {
     if (!self.testDebouncer) {
         self.testDebouncer = [TDSDebounceLeading manualDebounceWithTaskBlock:^{
             // TODO 想要执行的任务
         }];
     }
     [self.testDebouncer invoke];
 }
 
 // 在适当时机，如退出页面时释放
 [self.testDebouncer dispose];
 */

NS_ASSUME_NONNULL_END

