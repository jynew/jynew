//
//  TDSDomainManager.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/4/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSDomainManagerDelegate <NSObject>

@optional
- (void)checkDomainsDone:(NSDictionary *)resultInfo;

@end

FOUNDATION_EXTERN NSString *const DOMAIN_CHECK_HOST_KEY;
FOUNDATION_EXTERN NSString *const DOMAIN_CHECK_CODE_KEY;
FOUNDATION_EXTERN NSString *const DOMAIN_CHECK_DELAY_KEY;
FOUNDATION_EXTERN NSString *const DOMAIN_CHECK_REACHABLE_KEY;

@interface TDSDomainManager : NSObject
/// 获取一个域名管理实例
/// @param mainDomains 主域名
/// @param backupDomains 备用域名
+ (TDSDomainManager *)managerForDomains:(NSArray *)mainDomains backupDomains:(NSArray *)backupDomains;

- (void)setupDelegate:(id<TDSDomainManagerDelegate>)delegate;

/// 指定域名检查接口
/// @param checkAPI 检查接口
- (void)setupCheckAPI:(NSString *)checkAPI;

/// 获取一个当前可用域名
- (NSString *)getActiveDomain;

/// 标记一个域名为可用
/// @param domain 域名
- (void)activeDomain:(NSString *)domain;
+ (void)activeDomain:(NSString *)domain;

/// 标记一个域名不可用
/// @param domain 域名
- (void)deactiveDomain:(NSString *)domain;
+ (void)deactiveDomain:(NSString *)domain;

/// 开始检测域名，并定期检测
- (void)startCheckDomains;

/// 开始检测域名
/// @param api 若传入api则会请求该api数据,若传入空则直接检查域名连通
/// @param repeat 是否定时检测
- (void)startCheckDomains:(nullable NSString *)api repeat:(BOOL)repeat;

/// 停止检测域名
- (void)stopCheckDomains;
@end

NS_ASSUME_NONNULL_END
