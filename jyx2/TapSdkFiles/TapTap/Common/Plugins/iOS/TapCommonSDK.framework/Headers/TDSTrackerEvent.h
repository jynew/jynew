//
//  TDSTrackerEvent.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/6/21.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSTrackerConfig.h>
#import <TapCommonSDK/UserModel.h>
#import <TapCommonSDK/PageModel.h>
#import <TapCommonSDK/ActionModel.h>
#import <TapCommonSDK/NetworkStateModel.h>
#import <TapCommonSDK/LoginModel.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSTrackerEvent : NSObject
//事件类型
@property (nonatomic, assign) TDSTrackerType trackerType;

//用户模型
@property (nonatomic, strong, nullable) UserModel *userModel;

//页面模型
@property (nonatomic, strong, nullable) PageModel *pageModel;

//行为模型
@property (nonatomic, strong, nullable) ActionModel *actionModel;

//网络模型
@property (nonatomic, strong, nullable) NetworkStateModel *networkModel;

//登录模型
@property (nonatomic, strong, nullable) LoginModel *loginModel;
@end

NS_ASSUME_NONNULL_END
