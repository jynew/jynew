//
//  AsyncHttp.h
//
//  Created by JiangJiahao on 2018/3/9.
//  Copyright © 2018年 JiangJiahao. All rights reserved.
//  简单HTTP请求

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSHttpResult.h>

extern NSString *const TDS_TIMEOUTKEY;
extern NSString *const TDS_HTTPMETHODKEY;
extern NSString *const TDS_HTTPBODYKEY;
extern NSString *const TDS_DATAFORMAT;
extern NSString *const TDS_CACHE_POLICY_KEY;
extern NSString *const TDS_PROTOBUF_KEY;

/**
 header
 */
extern NSString *const TDS_AUTH_KEY;


typedef void(^CallBackBlock)(TDSHttpResult *result);
typedef void(^GetAllCallBack)(NSArray *resultArr,BOOL successAll);


@interface TDSAsyncHttp : NSObject
@property (nonatomic,copy) CallBackBlock callBackBlock;
@property (nonatomic,copy) CallBackBlock failedCallback;

- (void)stopTask;
- (void)retryTask;

- (void)handleSuccessResult:(TDSHttpResult *)result;
- (void)handleFailResult:(TDSHttpResult *)result;

/// GET请求
/// @param urlStr url
/// @param requestParams 网络请求参数，如超时、格式等
/// @param customHeaderParams 自定义请求头参数
/// @param params 本次请求参数
/// @param callBackBlock 成功回调
/// @param failedCallback 失败回调
- (TDSAsyncHttp *)httpGet:(NSString *)urlStr
            requestParams:(NSDictionary *)requestParams
             customHeader:(NSDictionary *)customHeaderParams
                   params:(NSDictionary *)params
                 callBack:(CallBackBlock)callBackBlock failedCallback:(CallBackBlock)failedCallback;

/**
 多个get请求并发，同时返回

 @param urlStrArr URL数组
 @param requestParamsArr 请求参数数组
 @param customHeaderParamsArr 自定义请求头数组
 @param paramsDicArr 参数数组
 @param callback 回掉
 */
- (void)httpGetAll:(NSArray *)urlStrArr
  requestParamsArr:(NSArray *)requestParamsArr
  customHeadersArr:(NSArray *)customHeaderParamsArr
            params:(NSArray *)paramsDicArr
          callback:(GetAllCallBack)callback;

/// POST请求
/// @param urlStr URL
/// @param requestParams 网络请求参数，如超时、数据格式、请求头等
/// @param customHeaderParams 自定义请求头参数
/// @param params 本次请求参数
/// @param callBackBlock 成功回调
/// @param failedCallback 失败回调
- (TDSAsyncHttp *)httpPost:(NSString *)urlStr
             requestParams:(NSDictionary *)requestParams
              customHeader:(NSDictionary *)customHeaderParams
                    params:(NSDictionary *)params
                  callBack:(CallBackBlock)callBackBlock
            failedCallback:(CallBackBlock)failedCallback;

@end
