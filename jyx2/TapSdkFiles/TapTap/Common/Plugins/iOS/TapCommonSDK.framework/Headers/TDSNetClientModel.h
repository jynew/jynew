//
//  TDSNetClientConfig.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSCommonHeader.h>
#import <TapCommonSDK/TDSAccount.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSNetConfigModel : NSObject
/// 可扩展头
@property (nonatomic, copy, nullable) NSString *baseUrl;
@property (nonatomic, copy, nullable) NSDictionary *extensionHeader;
@property (nonatomic, strong, nullable) TDSCommonHeader *commonHeader;
@property (nonatomic, strong, nullable) TDSAccount *account;
@end

NS_ASSUME_NONNULL_END
