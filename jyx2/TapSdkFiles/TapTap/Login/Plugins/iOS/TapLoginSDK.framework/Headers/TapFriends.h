//
//  TapFriends.h
//  TapLoginSDK
//
//  Created by pzheng on 2022/02/23.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/// Tap Friend Info
@interface TapFriendInfo : NSObject

/// Nickname.
@property (nonatomic, readonly) NSString *name;

/// Avatar.
@property (nonatomic, readonly) NSString *avatar;

/// Open ID.
@property (nonatomic, readonly) NSString *openid;

@end

/// Query Result
@interface TapFriendResult : NSObject

/// List of `TapFriendInfo`.
@property (nonatomic, nullable, readonly) NSArray<TapFriendInfo *> *data;

/// The start index of the next query.
@property (nonatomic, nullable, readonly) NSString *cursor;

@end

/// Query Option
@interface TapFriendQueryOption : NSObject

/// The limit of the result.
@property (nonatomic) NSUInteger size;

/// The start index of this query.
@property (nonatomic, nullable) NSString *cursor;

@end

/// Tap Friends
@interface TapFriends : NSObject

/// Query mutual list.
/// @param option See `TapFriendQueryOption`.
/// @param callback Result callback.
+ (void)queryMutualListWithOption:(TapFriendQueryOption * _Nullable)option
                         callback:(void (^)(TapFriendResult * _Nullable result, NSError * _Nullable error))callback;

@end

NS_ASSUME_NONNULL_END
