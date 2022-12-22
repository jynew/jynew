//
//  TDSAccount.h
//  TDSCommon
//
//  Created by Bottle K on 2020/9/29.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSInteger, TDSAccountType) {
    TAP,
    XD,
    XDG,
    TYPE_TDS,
LC
};

@interface TDSAccount : NSObject

/// xd token
@property (nonatomic, copy, readonly) NSString *token;
/// tap/tds
@property (nonatomic, copy, readonly) NSString *kid;
@property (nonatomic, copy, readonly) NSString *accessToken;
@property (nonatomic, copy, readonly) NSString *tokenType;
@property (nonatomic, copy, readonly) NSString *macKey;
@property (nonatomic, copy, readonly) NSString *macAlgorithm;
/// tds
@property (nonatomic, assign, readonly) long expireIn;
/// lc
@property (nonatomic, copy, readonly) NSString *clientId;
@property (nonatomic, copy, readonly) NSString *clientToken;
@property (nonatomic, copy, readonly) NSString *sessionToken;

- (instancetype)initWithToken:(NSString *)token type:(TDSAccountType)type;

- (instancetype)initWithLC:(NSString *)token type:(TDSAccountType)type;

- (TDSAccountType)getAccountType;

@end

NS_ASSUME_NONNULL_END
