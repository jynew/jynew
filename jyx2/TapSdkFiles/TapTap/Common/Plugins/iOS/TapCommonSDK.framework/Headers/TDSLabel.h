//
//  TDSLabel.h
//  XdComPlatform
//
//  Created by JiangJiahao on 2020/5/14.
//  Copyright © 2020 X.D. Network Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

typedef void(^CopySuccessCallback)(void);

typedef NS_ENUM(NSInteger,TDSLabelVerticalAlignment) {
    TDSLabelVerticalAlignmentTop = 0,
    TDSLabelVerticalAlignmentCenter,
    TDSLabelVerticalAlignmentBottom,
};

@interface TDSLabel : UILabel
@property (nonatomic) UIEdgeInsets edgeInsets;
@property (nonatomic) BOOL canCopy;
@property (nonatomic) CopySuccessCallback copyCallback;
@property (nonatomic) TDSLabelVerticalAlignment verticalAlignment;
@end

NS_ASSUME_NONNULL_END
