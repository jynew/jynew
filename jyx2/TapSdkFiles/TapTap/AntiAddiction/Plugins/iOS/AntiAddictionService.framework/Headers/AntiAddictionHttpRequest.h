#import <Foundation/Foundation.h>

@interface AntiAddictionHttpRequest : NSObject

//! GET参数拼接
+ (NSString *)connectUrl:(NSString *)url params:(NSDictionary *)params;
+ (NSString *)connectUrl:(NSString *)url params:(NSDictionary *)params encode:(BOOL)encode;

// POST请求参数拼接
+ (NSString *)postStringWithParams:(NSDictionary *)params;

// cookie
+ (NSString *)cookieStringForUrl:(NSString *)url;

+ (NSString *)urlEncodedString:(NSString *)string;

@end
