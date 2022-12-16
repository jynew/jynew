#import <Foundation/Foundation.h>
#import "AntiAddictionAsyncHttp.h"

NS_ASSUME_NONNULL_BEGIN

extern NSString *ANTI_ADDICTION_HTTP_FINISH_NOTIFICATION;

@interface AntiAddictionHttpManager : NSObject

@property (nonatomic) NSString *unityVersion;

+ (NSInteger)httpTaskIndex;

+ (void)addHttpTask:(AntiAddictionAsyncHttp *)httpTask index:(NSInteger)taskIndex;

+ (AntiAddictionHttpManager *)shareInstance;

@end

NS_ASSUME_NONNULL_END
