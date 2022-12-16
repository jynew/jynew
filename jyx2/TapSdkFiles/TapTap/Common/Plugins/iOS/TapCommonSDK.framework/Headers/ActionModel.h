//
//  ActionModel.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface ActionModel : NSObject
@property (nonatomic, copy) NSString *click;
@property (nonatomic, copy) NSString *like;
@property (nonatomic, copy) NSString *comment;
@property (nonatomic, copy) NSString *collect;
@property (nonatomic, copy) NSString *impression;
@end

NS_ASSUME_NONNULL_END
