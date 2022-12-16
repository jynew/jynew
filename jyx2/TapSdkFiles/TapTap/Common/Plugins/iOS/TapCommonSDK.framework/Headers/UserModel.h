//
//  UserModel.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface UserModel : NSObject
//用户id
@property (nonatomic, copy) NSString *user_id;
//用户名
@property (nonatomic, copy) NSString *user_name;
//taptap 授权的open_id
@property (nonatomic, copy) NSString *taptap_open_id;

@end

NS_ASSUME_NONNULL_END
