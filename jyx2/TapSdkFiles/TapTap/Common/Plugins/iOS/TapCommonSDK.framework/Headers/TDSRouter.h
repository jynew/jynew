//
//  TDSRouter.h
//  TDSCommon
//
//  Created by Insomnia on 2020/11/27.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

extern NSString *const TDSRouterURL;
extern NSString *const TDSRouterResp;
extern NSString *const TDSRouterParams;

typedef void (^TDSRouterHandler)(NSDictionary *params);

typedef void (^TDSRouterResponse)(NSDictionary *response);


/// TDS路由
@interface TDSRouter : NSObject

/// 注册路由
/// @param url 路由路径，例如tds://service，并支持带参数路由tds://service/:user/:age
/// @param handler 接收参数，包含了注册的 URL 中对应的变量， 如注册的 URL 为ds://service/:user那么，就会传一个 @{@"user": "tds_name"} 这样的字典过来
+ (void)registerServiceWithURL:(NSString *)url handler:(TDSRouterHandler)handler;


///  反注册
/// @param url 路由路径
+ (void)unregisterServiceWithUrl:(NSString *)url;

/// 请求路由
/// @param url 路由路径
+ (void)requestWithURL:(NSString *)url;


/// 请求路由
/// @param url 路由路径
/// @param params 请求参数
+ (void)requestWithURL:(NSString *)url params:(NSDictionary * _Nullable)params;

/// 请求路由
/// @param url 路由路径
/// @param params 请求参数
/// @param response 返回值字典
+ (void)requestWithURL:(NSString *)url params:(NSDictionary * _Nullable)params response:(TDSRouterResponse _Nullable)response;

/// 是否存在服务
/// @param url 路由路径
+ (BOOL)hasServiceWithURL:(NSString *)url;


/// 自动拼接路由参数
/// @param url 路由路径，例如tds://service/:user/:age
/// @param params 数组顺序要与路由参数顺序对应
/// @return 生成URL 字符串
+ (NSString *)generateWithURL:(NSString *)url params:(NSArray *)params;

@end

NS_ASSUME_NONNULL_END
