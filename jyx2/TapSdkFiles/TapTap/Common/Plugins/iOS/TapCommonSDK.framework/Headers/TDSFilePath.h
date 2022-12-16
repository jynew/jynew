//
//  FilePath.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/16.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//  文件路径类

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSFilePath : NSObject
+ (NSString *)homePath;
+ (NSString *)documentsPath;
+ (NSString *)cachesPath;
+ (NSString *)tmpPath;
+ (NSString *)pathForFile:(NSString *)name type:(NSString *)type;
+ (NSString *)imagePath;

+ (NSString *)bundlePath;
@end

NS_ASSUME_NONNULL_END
