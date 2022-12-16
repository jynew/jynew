//
//  AccountGlobalError.h
//  TapTapLoginSource
//
//  Created by Bottle K on 2020/12/15.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
FOUNDATION_EXPORT NSString *const LOGIN_ERROR_ACCESS_DENIED;
FOUNDATION_EXPORT NSString *const LOGIN_ERROR_INVALID_GRANT;
FOUNDATION_EXPORT NSString *const LOGIN_ERROR_PERMISSION_RESULT;

@interface AccountGlobalError : NSObject
@property (nonatomic, assign) NSInteger code;
@property (nonatomic, copy) NSString *msg;
@property (nonatomic, copy) NSString *error;
@property (nonatomic, copy) NSString *errorDescription;

- (instancetype)initWithName:(NSString *)errorName NSError:(NSError *)error;

- (NSString *)toJsonString;
@end

NS_ASSUME_NONNULL_END
