//
//  TDSHandleUrl.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/3/30.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSHandleUrl : NSObject

/// 在application:openURL:中调用
/// @param url 传入openURL的url
+ (BOOL)handleOpenURL:(nullable NSURL *)url;

/// 各模块注册handleUrl的事件
/// @param event 在block中调用handleUrl的事件
/// @param tag 唯一标识符，防止重复添加
+ (void)addHandleEvent:(BOOL (^)(NSURL *url))event withTag:(NSString *)tag;


@end

NS_ASSUME_NONNULL_END
