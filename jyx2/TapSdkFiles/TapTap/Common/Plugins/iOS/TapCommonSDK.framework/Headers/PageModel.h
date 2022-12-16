//
//  PageModel.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

//页面唯一id
FOUNDATION_EXPORT NSString *const PAGE_ID_TAPTAP_AUTHORIZE_WEB;
FOUNDATION_EXPORT NSString *const PAGE_ID_TAPTAP_AUTHORIZE_TAPTAPCLIENT;
FOUNDATION_EXPORT NSString *const PAGE_ID_GAME;
//页面别名
FOUNDATION_EXPORT NSString *const PAGE_NAME_TAPTAP_AUTHORIZE_WEB;
FOUNDATION_EXPORT NSString *const PAGE_NAME_TAPTAP_AUTHORIZE_TAPTAPCLIENT;
FOUNDATION_EXPORT NSString *const PAGE_NAME_GAME;

//页面事件名
FOUNDATION_EXPORT NSString *const PAGE_ACTION_APPEAR;
FOUNDATION_EXPORT NSString *const PAGE_ACTION_DISAPPEAR;

@interface PageModel : NSObject
//页面唯一id
@property (nonatomic, copy, nullable) NSString *page_id;

//页面别名
@property (nonatomic, copy, nullable) NSString *page_name;

//页面事件名
@property (nonatomic, copy, nullable) NSString *page_action;

@end

NS_ASSUME_NONNULL_END
