//
//  NSArray+Safe.h
//  TapAchievement
//
//  Created by TapTap-David on 2020/9/15.
//  Copyright © 2020 taptap. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSArray (Safe)

- (void)tds_each:(void (^)(id object, NSUInteger index))block;

- (void)tds_apply:(void (^)(id object, NSUInteger index))block;

- (NSArray *)tds_map:(id (^)(id object, NSUInteger index))block;

- (id)tds_reduce:(id (^)(id accumulated, id object))block;

- (NSArray *)tds_filter:(BOOL (^)(id object, NSUInteger index))block;

- (id)tds_safeObjectAtIndex:(NSUInteger)index;

@end
