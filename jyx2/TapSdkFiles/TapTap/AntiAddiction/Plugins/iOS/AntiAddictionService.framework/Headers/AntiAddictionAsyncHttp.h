#import <Foundation/Foundation.h>
#import "AntiAddictionHttpResult.h"

extern NSString *const ANTI_TIMEOUTKEY;
extern NSString *const ANTI_HTTPMETHODKEY;
extern NSString *const ANTI_HTTPBODYKEY;
extern NSString *const ANTI_DATAFORMAT;
extern NSString *const ANTI_CACHE_POLICY_KEY;

/**
 header
 */
extern NSString *const ANTI_AUTH_KEY;


typedef void(^AntiCallBackBlock)(AntiAddictionHttpResult *result);
typedef void(^GetAllCallBack)(NSArray *resultArr,BOOL successAll);


@interface AntiAddictionAsyncHttp : NSObject
@property (nonatomic,copy) AntiCallBackBlock callBackBlock;
@property (nonatomic,copy) AntiCallBackBlock failedCallback;

- (void)startTask;
- (void)stopTask;
- (void)retryTask;
- (NSInteger)httpTaskIdentify;

- (void)handleSuccessResult:(AntiAddictionHttpResult *)result;
- (void)handleFailResult:(AntiAddictionHttpResult *)result;

/// GET请求
/// @param urlStr url
/// @param requestParams 网络请求参数，如超时、格式等
/// @param customHeaderParams 自定义请求头参数
/// @param params 本次请求参数
/// @param callBackBlock 成功回调
/// @param failedCallback 失败回调
+ (void)httpGet:(NSString *)urlStr
            requestParams:(NSDictionary *)requestParams
             customHeader:(NSDictionary *)customHeaderParams
                   params:(NSDictionary *)params
                 callBack:(AntiCallBackBlock)callBackBlock failedCallback:(AntiCallBackBlock)failedCallback;

/**
 多个get请求并发，同时返回

 @param urlStrArr URL数组
 @param requestParamsArr 请求参数数组
 @param customHeaderParamsArr 自定义请求头数组
 @param paramsDicArr 参数数组
 @param callback 回掉
 */
+ (void)httpGetAll:(NSArray *)urlStrArr
  requestParamsArr:(NSArray *)requestParamsArr
  customHeadersArr:(NSArray *)customHeaderParamsArr
            params:(NSArray *)paramsDicArr
          callback:(GetAllCallBack)callback;

/// POST请求
/// @param urlStr URL
/// @param requestParams 网络请求参数，如超时、数据格式、请求头等
/// @param customHeaderParams 自定义请求头参数
/// @param params 本次请求参数
/// @param paramsJson 本次请求参数的 json 字符串，若有，优先使用 json
/// @param callBackBlock 成功回调
/// @param failedCallback 失败回调
+ (void)httpPost:(NSString *)urlStr
             requestParams:(NSDictionary *)requestParams
              customHeader:(NSDictionary *)customHeaderParams
                    params:(NSDictionary *)params
                paramsJson:(NSString *)paramsJson
                  callBack:(AntiCallBackBlock)callBackBlock
            failedCallback:(AntiCallBackBlock)failedCallback;

+ (void)httpPost:(NSString *)urlStr
             requestParams:(NSDictionary *)requestParams
              customHeader:(NSDictionary *)customHeaderParams
                    params:(NSDictionary *)params
                  callBack:(AntiCallBackBlock)callBackBlock
          failedCallback:(AntiCallBackBlock)failedCallback;

@end
