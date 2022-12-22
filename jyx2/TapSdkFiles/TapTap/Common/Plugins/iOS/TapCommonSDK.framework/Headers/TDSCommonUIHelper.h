//
//  TDSCommonUIHelper.h
//  TDSCommon
//
//  Created by Bottle K on 2021/3/2.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSCommonUIHelper : NSObject

/// 从common bundle获取image
+ (UIImage *)getImageFromCommonWithImageName:(NSString *)name;

/// 从bundle获取image
+ (UIImage *)getImageFromBundle:(NSString *)bundleName withImage:(NSString *)imageName;

/// 从bundle获取翻译后的文案
+ (NSString *)getTranslatedStringFromCommonWithKey:(NSString *)key;

/// rgb转color
+ (UIColor *)rgbToColorWithRed:(CGFloat)red
                         green:(CGFloat)green
                          blue:(CGFloat)blue;

/// rgba转color
+ (UIColor *)rgbToColorWithRed:(CGFloat)red
                         green:(CGFloat)green
                          blue:(CGFloat)blue
                         aplha:(CGFloat)alpha;

/// hex转color
+ (UIColor *)hexToColor:(int)hexValue;

/// hex转color
+ (UIColor *)hexToColor:(int)hexValue alpha:(CGFloat)alpha;

/// 屏幕宽度，会根据横竖屏的变化而变化
+ (CGFloat)screenWidth;

/// 屏幕高度，会根据横竖屏的变化而变化
+ (CGFloat)screenHeight;

/// 屏幕宽度，跟横竖屏无关
+ (CGFloat)deviceWidth;

/// 屏幕高度，跟横竖屏无关
+ (CGFloat)deviceHeight;

/// 用户界面横屏了才会返回YES
+ (BOOL)isUILandscape;

/// 无论支不支持横屏，只要设备横屏了，就会返回YES
+ (BOOL)isDeviceLandscape;

/// 是否有刘海
+ (BOOL)hasNotch;

/// 安全区域（上左下右）
+ (UIEdgeInsets)safeAreaInsets;

+ (UIViewController *)findTopViewController;

+ (UIViewController *)findUIViewController:(UIViewController *)controller;
@end

NS_ASSUME_NONNULL_END
