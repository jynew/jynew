//
//  XDLightWebImageView.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/12/18.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//  轻量,没有点击事件

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSLightWebImageView : UIView

- (void)setImageWithUrl:(NSString *)imageUrl;
- (void)setImageWithUrl:(NSString *)imageUrl size:(CGSize)imageSize;

@end

NS_ASSUME_NONNULL_END
