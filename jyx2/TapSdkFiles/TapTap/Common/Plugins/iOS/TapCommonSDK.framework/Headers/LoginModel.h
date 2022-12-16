//
//  LoginModel.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/6/21.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

//登录流程中触发的事件
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_AUTHORIZE_START;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_OPEN;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_BACK;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_TOKEN;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_PROFILE;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_SUCCESS;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_FAIL;
FOUNDATION_EXPORT NSString *const LOGIN_ACTION_TAPTAP_AUTHORIZE_CANCEL;

//登录类型
FOUNDATION_EXPORT NSString *const LOGIN_TYPE_TAPTAP;
FOUNDATION_EXPORT NSString *const LOGIN_TYPE_WEBVIEW;
FOUNDATION_EXPORT NSString *const LOGIN_TYPE_AUTO;

@interface LoginModel : NSObject
//每次登录流程唯一ID
@property (nonatomic, copy) NSString *login_session_id;

//登录流程中触发的事件
@property (nonatomic, copy) NSString *login_action;

//登录类型
@property (nonatomic, copy, nullable) NSString *login_type;

//错误code
@property (nonatomic, copy, nullable) NSString *login_error_code;

//错误message
@property (nonatomic, copy, nullable) NSString *login_error_msg;
@end

NS_ASSUME_NONNULL_END
