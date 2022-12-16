//
//  NetworkStateModel.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/3/23.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface NetworkStateModel : NSObject
@property (nonatomic, copy) NSString *session_id;   //每次启动唯一ID
@property (nonatomic, copy) NSString *host;         //服务器
@property (nonatomic, assign) NSInteger code;       //返回码200成功
@property (nonatomic, assign) CGFloat delay;        //网络延迟时间（毫秒)
@end

NS_ASSUME_NONNULL_END
