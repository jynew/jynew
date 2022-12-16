#import <AntiAddictionUI/AntiAddiction.h>
#import <AntiAddictionService/AntiAddictionService-Swift.h>

char const *GAME_OBJECT = "PluginBridge";


char* MakeStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

@interface Utility : NSObject
@end

@implementation Utility

+ (NSString *)dictonaryToJson:(NSDictionary *) dictionary {
    NSError* error;

    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dictionary options:0 error:&error];
    if (!jsonData) {
        NSLog(@"Dictonary stringify error: %@", error);
        return @"";
    }
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSDictionary *)dictionaryWithJsonString:(NSString *) jsonString {
    if (jsonString == nil) return nil;
    NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *err;
    NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData
                            options:NSJSONReadingMutableContainers
                            error:&err];
    if (err) {
        NSLog(@"json解析失败：%@", err);
        return nil;
    }
    return dic;
}

@end

@interface NativeAntiAddictionKitPlugin : NSObject<AntiAddictionDelegate>
@end

@implementation NativeAntiAddictionKitPlugin

static NativeAntiAddictionKitPlugin *_sharedInstance;

+(NativeAntiAddictionKitPlugin*)sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[NativeAntiAddictionKitPlugin alloc] init];
    });
    return _sharedInstance;
}

-(id)init
{
    self = [super init];
    if (self)
        [self initHelper];
    return self;
}

-(void)initHelper
{
    NSLog(@"Initialized NativeAntiAddictionKitPlugin class");
}

-(NSString *)generateUnityUnifyExtras:(NSDictionary *) extras {
    NSMutableDictionary* result = [[NSMutableDictionary alloc] init];
    if (extras) {
        if (extras[@"title"]) {
            [result setObject:extras[@"title"] forKey:@"title"];
        }
        if (extras[@"description"]) {
            [result setObject:extras[@"description"] forKey:@"description"];
        }
        if (extras[@"restrictType"]) {
            [result setObject:extras[@"userType"] forKey:@"userType"];
        }
        if (extras[@"remainTime"]) {
            [result setObject:[NSString stringWithFormat:@"%@",extras[@"remainTime"]] forKey:@"remaining_time_str"];
        }
        if (extras[@"restrictType"]) {
            [result setObject:[NSString stringWithFormat:@"%@",extras[@"restrictType"]] forKey:@"strict_type"];
        }
    }
    
    return [Utility dictonaryToJson:result];
}


-(NSString *)generateResultMessage:(NSInteger)code extras:(NSDictionary *) extras
{
    NSDictionary* result = [[NSDictionary alloc] initWithObjectsAndKeys:
                            [NSNumber numberWithUnsignedLong:code],@"code"
                            ,[self generateUnityUnifyExtras:extras], @"extras"
                            , nil];
    return [Utility dictonaryToJson:result];
}


-(NSString *)generateCheckPayResult:(NSInteger)status title:(NSString *)title description:(NSString *)description
{
    NSDictionary* result = [[NSDictionary alloc] initWithObjectsAndKeys:
                            [NSNumber numberWithUnsignedLong:status],@"status"
                            ,[NSString stringWithFormat:@"%@",title],@"title"
                            ,[NSString stringWithFormat:@"%@",description], @"description"
                            ,nil];
    return [Utility dictonaryToJson:result];
}

#pragma mark - delegate
- (void)antiAddictionCallbackWithCode:(AntiAddictionResultHandlerCode)code extra:(NSString *)extra {
    NSString *resultString = [NSString stringWithFormat:@"ios callback code:%ld,extra:%@",(long)code,extra];
    NSLog(@"%@", resultString);
    NSDictionary* extraDict = [Utility dictionaryWithJsonString:extra];
    
    if (code == AntiAddictionResultHandlerLoginSuccess) {
        UnitySendMessage(GAME_OBJECT, [@"HandleAntiAddictionCallbackMsg" UTF8String], [[self generateResultMessage:500 extras:extraDict] UTF8String]);
    } else {
        UnitySendMessage(GAME_OBJECT, [@"HandleAntiAddictionCallbackMsg" UTF8String]
                                        , [[self generateResultMessage:code extras:extraDict]
                                            UTF8String]);
    }
}

@end

extern "C"
{
    void initSDK(const char *gameIdentifier
                 , bool useTimeLimit
                 , bool usePaymentLimit
                 , bool showSwitchAccount
                 ) {

        NSString *gameIdentifierParam = [NSString stringWithUTF8String:gameIdentifier];

        NSLog(@"%@", [NSString stringWithFormat:@"initSDK with gameIdentifier: %@ ，useTimeLimit: %@, usePaymentLimit: %@"
        , gameIdentifierParam
        , useTimeLimit?@"YES":@"NO"
        , usePaymentLimit?@"YES":@"NO"]);
        AntiAddictionConfiguration *configuration = AntiAddictionService.configuration;
        configuration.showSwitchAccount = showSwitchAccount;
        [AntiAddiction initGameIdentifier:gameIdentifierParam antiAddictionConfig:configuration antiAddictionCallbackDelegate:[NativeAntiAddictionKitPlugin sharedInstance] completionHandler:^(BOOL) {}];
    }

    void startup(const char *userIdentifier, bool useTapLogin) {
        NSString *userIdentifierParam = [NSString stringWithUTF8String:userIdentifier];
        NSLog(@"%@", [NSString stringWithFormat:@"startup with userIdentifier: %@ ，useTapLogin: %@"
        , userIdentifierParam
        , useTapLogin?@"YES":@"NO"]);
        [AntiAddiction startUpUseTapLogin:useTapLogin userIdentifier:userIdentifierParam];
    }

    void logout() {
        [AntiAddiction logout];
    }

    void enterGame() {
        [AntiAddiction enterGame];
    }

    void leaveGame() {
        [AntiAddiction leaveGame];
    }

    void checkPayLimit(long amount) {
        [AntiAddiction checkPayLimit:(NSInteger)amount callBack:^(BOOL status, NSString * _Nonnull title, NSString * _Nonnull description) {
            UnitySendMessage(GAME_OBJECT, [@"HandleCheckPayLimit" UTF8String], [[Utility dictonaryToJson:[[NSDictionary alloc] initWithObjectsAndKeys:
                                                                                [NSNumber numberWithUnsignedLong:status],@"status"
                                                                                ,[NSString stringWithFormat:@"%@", title], @"title"
                                                                                ,[NSString stringWithFormat:@"%@", description], @"description"
                                                                                , nil]] UTF8String]);
        } failureHandler:^(NSString * _Nonnull exception) {
            UnitySendMessage(GAME_OBJECT, [@"HandleCheckPayLimitException" UTF8String], [exception UTF8String]);
        }];
    }

    void submitPayResult(long amount) {
        [AntiAddiction submitPayResult:(NSInteger)amount callBack:^(BOOL success) {
            UnitySendMessage(GAME_OBJECT, [@"HandleSubmitPayResult" UTF8String], "");
        } failureHandler:^(NSString * _Nonnull exception) {
            UnitySendMessage(GAME_OBJECT, [@"HandleSubmitPayResultException" UTF8String], [exception UTF8String]);
        }];
    }
    
    int getCurrentUserRemainTime() {
        return (int)[AntiAddiction getCurrentUserRemainTime];
    }
    
    int getCurrentUserAgeLimit() {
        return (int)[AntiAddiction getCurrentUserAgeLimite];
    }
    
    const char* getCurrentAntiToken() {
        return MakeStringCopy([[AntiAddiction currentToken] UTF8String]);
    }

    void setUnityVersion(const char * version) {
        [AntiAddiction setUnityVersion:[NSString stringWithUTF8String:version]];
    }

    bool standalone() {
        return [AntiAddiction isStandAlone];

    }
}

