//
//  HttpDownloadImage.h
//  NativeApp
//
//  Created by JiangJiahao on 2018/10/16.
//  Copyright © 2018 JiangJiahao. All rights reserved.
//

#import <TapCommonSDK/TDSHttpDownloadBase.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSHttpDownloadImage : TDSHttpDownloadBase

+ (void)downloadImage:(NSString *)url callback:(downloadCallback)callback;

@end

NS_ASSUME_NONNULL_END
