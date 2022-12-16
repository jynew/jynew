//
//  XDCache.h
//  XDCollectionView
//
//  Created by JiangJiahao on 2019/5/22.
//  Copyright © 2019 tapdb. All rights reserved.
//  simple image cache

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSMemoryCache : NSObject
+ (TDSMemoryCache *)shareInstance;

/// 设置最大缓存数量，默认50
/// @param countLimit 缓存数量
+ (void)setCacheCountLimit:(NSUInteger)countLimit;

/// 设置缓存项目
/// @param obj 缓存对象
/// @param key key
+ (void)setObject:(id)obj forKey:(id)key;

/// 获取缓存
/// @param key Key
+ (id)objectForKey:(id)key;

@end

NS_ASSUME_NONNULL_END
