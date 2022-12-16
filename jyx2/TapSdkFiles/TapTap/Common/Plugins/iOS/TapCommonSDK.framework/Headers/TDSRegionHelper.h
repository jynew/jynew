//
//  TDSRegionHelper.h
//  TDSCommon
//
//  Created by TapTap-David on 2020/11/18.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSRegionHelper : NSObject

+ (void)getRegionCode:(void(^)(BOOL isMainland))complete;

@end

NS_ASSUME_NONNULL_END
