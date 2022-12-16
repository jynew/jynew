//
//  HttpResult.h
//
//  Created by JiangJiahao on 2018/3/9.
//  Copyright © 2018年 JiangJiahao. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface TDSHttpResult : NSObject

@property (nonatomic,strong) NSData *data;
@property (nonatomic,strong) NSURLResponse *response;
@property (nonatomic,strong) NSError *error;
@property (nonatomic,strong) NSError *localError;              // 本地错误
@property (nonatomic,copy) NSString *originUrl;

@property (nonatomic,copy) NSDictionary *resultDic;

// 多个get同时返回数据时使用
@property (nonatomic) NSArray *dataArr;

@end
