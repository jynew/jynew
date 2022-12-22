//
//  XDGAutoLayout.h
//  XDG
//
//  Created by JiangJiahao on 2020/8/20.
//  Copyright © 2020 JiangJiahao. All rights reserved.
//  简单自动布局类

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSAutoLayout : NSObject

+ (void)openAutoLayout:(UIView *)targetView;
+ (void)safeAreaLayout:(BOOL)safe;

+ (NSLayoutConstraint *)layoutHeight:(UIView *)targetView height:(CGFloat)height;
+ (NSLayoutConstraint *)layoutWidth:(UIView *)targetView width:(CGFloat)width;

/// 相等约束相等布局
/// @param view1 view1
/// @param view2 view2
+ (void)layoutViewEqual:(UIView *)view1 toView:(UIView *)view2;

+ (NSLayoutConstraint *)layoutViewEqual:(UIView *)view1
                                 toView:(UIView *)view2
                              attribute:(NSLayoutAttribute)attr;

+ (NSLayoutConstraint *)layoutViewEqual:(UIView *)view1
                                 toView:(UIView *)view2
                              attribute:(NSLayoutAttribute)attr
                                 offset:(CGFloat)offset;

+ (NSLayoutConstraint *)layoutViewEqual:(UIView *)view1
                              attribute:(NSLayoutAttribute)attr1
                                 toView:(UIView *)view2
                              attribute:(NSLayoutAttribute)attr2;

/// 约束两个view相等
/// @param view1 view1
/// @param attr1 view1约束
/// @param view2 view2
/// @param attr2 view2约束
/// @param constant 距离
+ (NSLayoutConstraint *)layoutViewEqual:(UIView *)view1
                              attribute:(NSLayoutAttribute)attr1
                                 toView:(UIView *)view2
                              attribute:(NSLayoutAttribute)attr2
                               constant:(CGFloat)constant;

/// 约束两个view，更大
/// @param view1 view1
/// @param attr1 view1约束
/// @param view2 view2
/// @param attr2 view2约束
/// @param constant 距离
+ (NSLayoutConstraint *)layoutViewGreater:(UIView *)view1
                                attribute:(NSLayoutAttribute)attr1
                                   toView:(nullable UIView *)view2
                                attribute:(NSLayoutAttribute)attr2
                                 constant:(CGFloat)constant;

/// 约束两个view，更小
/// @param view1 view1
/// @param attr1 view1约束
/// @param view2 view2
/// @param attr2 view2约束
/// @param constant 距离
+ (NSLayoutConstraint *)layoutViewLesser:(UIView *)view1
                               attribute:(NSLayoutAttribute)attr1
                                  toView:(nullable UIView *)view2
                               attribute:(NSLayoutAttribute)attr2
                                constant:(CGFloat)constant;
@end

NS_ASSUME_NONNULL_END
