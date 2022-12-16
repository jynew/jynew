//
//  TapAuthAccessToken.h
//  TapCommonSDK
//
//  Created by 黄驿峰 on 2022/1/26.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/// TapTap authorization token
@interface TapAuthAccessToken : NSObject

/// A unique identifier
@property (nonatomic, copy, nullable) NSString * kid;

/// access token
@property (nonatomic, copy, nullable) NSString * accessToken;

/// token Type
@property (nonatomic, copy, nullable) NSString * tokenType;

/// mac key
@property (nonatomic, copy, nullable) NSString * macKey;

/// mac key algorithm
@property (nonatomic, copy, nullable) NSString * macAlgorithm;

/// Multiple user permissions are separated by commas (,)
@property (nonatomic, copy, nullable) NSString * scope;


+ (TapAuthAccessToken *)buildWithKid:(NSString *)kid accessToken:(NSString *)accessToken tokenType:(NSString *)tokenType macKey:(NSString *)macKey macAlgorithm:(NSString *)macAlgorithm;

+ (nullable TapAuthAccessToken *)buildWithJsonString:(NSString *)jsonString;

+ (nullable TapAuthAccessToken *)buildWithQueryString:(NSString *)queryString;

+ (nullable TapAuthAccessToken *)buildWithDic:(NSDictionary *)dic;

- (NSString *)tokenToQueryString;

- (NSString *)tokenToJsonString;


//+ (void)clearLocalAccessToken;
//- (void)saveToLocal;
//+ (nullable TapAuthAccessToken *)loadLocalAccessToken;


- (BOOL)containCompliancePermission;

@end

NS_ASSUME_NONNULL_END
