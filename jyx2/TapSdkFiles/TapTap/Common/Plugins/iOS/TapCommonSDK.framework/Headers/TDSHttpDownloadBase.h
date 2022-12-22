//
//  HttpDownloadBase.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/16.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//  下载基类

#import <Foundation/Foundation.h>

typedef void(^downloadCallback)(BOOL success);

NS_ASSUME_NONNULL_BEGIN

@interface TDSHttpDownloadBase : NSObject
/// 下载完成以后文件存储路径
+ (NSString *)saveFilePath;

/// 文件存储名
/// @param url 文件url
+ (NSString *)saveFileName:(NSString *)url;

/// 下载文件
/// @param url 文件url
/// @param callback 下载结果回调
+ (void)downloadFile:(NSString *)url callback:(downloadCallback)callback;

@end

NS_ASSUME_NONNULL_END
