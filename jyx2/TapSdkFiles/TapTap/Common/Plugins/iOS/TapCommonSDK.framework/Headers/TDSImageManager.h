//
//  ImageManager.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/16.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//  图片辅助类，有些方法可以用catogary实现

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef void(^resultBlockWithName)(UIImage *_Nullable resultImage,NSString * _Nonnull imageName);
typedef void(^resultBlock)(UIImage *_Nullable resultImage);


NS_ASSUME_NONNULL_BEGIN

@interface TDSImageManager : NSObject
/// 图片缩放
/// @param img 图片
/// @param size 尺寸
+ (UIImage *)scaleToSize:(UIImage *)img size:(CGSize)size;

/// 滤镜，高斯模糊
/// @param image 图片
/// @param blur 模糊程度
+ (UIImage *)blurryImage:(UIImage *)image withBlurLevel:(CGFloat)blur;

/// URL获取图片名称
/// @param url 图片URL
+ (NSString *)imageNameWithUrl:(NSString *)url;

+ (void)loadImage:(NSString *)imageName needDecode:(BOOL)needDecode resultBlock:(resultBlockWithName)block;
+ (void)loadImage:(NSString *)imageName resultBlock:(resultBlockWithName)block;
+ (void)loadImage:(NSString *)imageName size:(CGSize)size resultBlock:(resultBlockWithName)block;

// 只需要传入"xxx.png"
+ (UIImage *)getBundleImage:(NSString *)imageName resultBlock:(resultBlockWithName)block;
+ (UIImage *)getBundleImage:(NSString *)imageName size:(CGSize)size resultBlock:(resultBlockWithName)block;

//UIColor 转UIImage
+ (UIImage*)createImageWithColor: (UIColor *)color;
@end

NS_ASSUME_NONNULL_END
