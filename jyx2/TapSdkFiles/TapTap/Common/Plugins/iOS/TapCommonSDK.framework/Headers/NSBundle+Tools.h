//
//  NSBundle+Tools.h
//  TDSAchievement
//
//  Created by TapTap-David on 2020/8/26.
//  Copyright © 2020 taptap. All rights reserved.
//
#import <UIKit/UIKit.h>

@interface NSBundle (Tools)
+ (instancetype)tds_bundleName:(NSString *)bundleName aClass:(Class)aClass;
- (NSString *)tds_localizedStringForKey:(NSString *)key value:(NSString *)value;
- (NSString *)tds_localizedStringForKey:(NSString *)key;

- (UIImage *)tds_imageName:(NSString *)imageName;
@end
