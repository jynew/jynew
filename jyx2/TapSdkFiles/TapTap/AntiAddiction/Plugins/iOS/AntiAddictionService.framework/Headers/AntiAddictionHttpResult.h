
#import <Foundation/Foundation.h>

@interface AntiAddictionHttpResult : NSObject

@property (nonatomic,strong) NSData *data;
@property (nonatomic,strong) NSURLResponse *response;
@property (nonatomic) NSError *error;
@property (nonatomic) NSError *localError;              // 本地错误
@property (nonatomic) NSString *originUrl;
@property (nonatomic, assign) NSInteger statusCode;

@property (nonatomic) NSDictionary *resultDic;

// 多个get同时返回数据时使用
@property (nonatomic) NSArray *dataArr;

@end
