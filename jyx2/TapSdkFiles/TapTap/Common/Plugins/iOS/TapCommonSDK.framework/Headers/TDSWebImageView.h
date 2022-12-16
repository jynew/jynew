//
//  XDWebImageView.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/16.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSWebImageView : UIImageView

- (void)setImageWithUrl:(NSString *)imageUrl;

- (void)setImageWithUrl:(nullable NSString *)imageUrl placeholderImage:(nullable UIImage *)placeholder;

@end

NS_ASSUME_NONNULL_END
