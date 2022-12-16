//
//  XDReachability.h
//  TDS
//
//  Created by JiangJiahao on 2019/7/25.
//  Copyright © 2019 X.D. Network Inc. All rights reserved.
//  直接用的YYReachability

#import <Foundation/Foundation.h>
#import <SystemConfiguration/SystemConfiguration.h>
#import <netinet/in.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSUInteger, TDSReachabilityStatus) {
    TDSReachabilityStatusNone  = 0, ///< Not Reachable
    TDSReachabilityStatusWWAN  = 1, ///< Reachable via WWAN (2G/3G/4G)
    TDSReachabilityStatusWiFi  = 2, ///< Reachable via WiFi
};

typedef NS_ENUM(NSUInteger, TDSReachabilityWWANStatus) {
    TDSReachabilityWWANStatusNone  = 0, ///< Not Reachable vis WWAN
    TDSReachabilityWWANStatus2G = 2, ///< Reachable via 2G (GPRS/EDGE)       10~100Kbps
    TDSReachabilityWWANStatus3G = 3, ///< Reachable via 3G (WCDMA/HSDPA/...) 1~10Mbps
    TDSReachabilityWWANStatus4G = 4, ///< Reachable via 4G (eHRPD/LTE)       100Mbps
    TDSReachabilityWWANStatus5G = 5, ///< Reachable via 5G (sa/nsa)          500Mbps
};

@interface TDSReachability : NSObject
@property (nonatomic, readonly) SCNetworkReachabilityFlags flags;                           ///< Current flags.
@property (nonatomic, readonly) TDSReachabilityStatus status;                                ///< Current status.
@property (nonatomic, readonly) TDSReachabilityWWANStatus wwanStatus NS_AVAILABLE_IOS(7_0);  ///< Current WWAN status.
@property (nonatomic, readonly, getter=isReachable) BOOL reachable;                         ///< Current reachable status.

/// Notify block which will be called on main thread when network changed.
@property (nullable, nonatomic, copy) void (^TDSReachabilityNotifyBlock)(TDSReachability *reachability);

/// Create an object to check the reachability of the default route.
+ (instancetype)reachability;

/// Create an object to check the reachability of the local WI-FI.
+ (instancetype)reachabilityForLocalWifi DEPRECATED_MSG_ATTRIBUTE("unnecessary and potentially harmful");

/// Create an object to check the reachability of a given host name.
+ (nullable instancetype)reachabilityWithHostname:(NSString *)hostname;

/// Create an object to check the reachability of a given IP address
/// @param hostAddress You may pass `struct sockaddr_in` for IPv4 address or `struct sockaddr_in6` for IPv6 address.
+ (nullable instancetype)reachabilityWithAddress:(const struct sockaddr *)hostAddress;
@end

NS_ASSUME_NONNULL_END
