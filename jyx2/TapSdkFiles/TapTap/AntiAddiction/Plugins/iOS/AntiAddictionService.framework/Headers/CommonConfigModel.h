//
//  CommonConfigModel.h
//  AntiAddictionService
//
//  Created by jessy on 2021/9/22.
//  Copyright © 2021 JiangJiahao. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface CommonConfigModel : NSObject

+ (instancetype)sharedInstance;

@property (nonatomic,strong) NSArray *auth_identify_words;

@property (nonatomic,strong) NSDictionary * ui_config;

@property (nonatomic,strong) NSString *unKnowTitle;

@property (nonatomic,strong) NSString *unKnowDesc;

@property (nonatomic,strong) NSString *name;

//  实名是否有记录 /public-department/v1/clients/{clientId}/users/{userId}/real-name 接口返回
@property (nonatomic,assign) BOOL hasAuthRecord;

@end

NS_ASSUME_NONNULL_END
