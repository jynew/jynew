//
//  NSMutableArray+Safe.h
//  TapAchievement
//
//  Created by TapTap-David on 2020/9/24.
//  Copyright © 2020 taptap. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSMutableArray (Safe)
- (void)tt_safeAddObject:(id)anObject;

- (void)tt_addNonNullObject:(id)anObject;

- (void)tt_safeInsertObject:(id)anObject atIndex:(NSUInteger)index;

- (void)tt_safeReplaceObjectAtIndex:(NSUInteger)index withObject:(id)anObject;
@end

NS_ASSUME_NONNULL_END
