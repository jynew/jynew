//
//  TDSButton.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/4/27.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSButton : UIButton

@property (nonatomic, assign) NSInteger mode;

- (void)confirmMode;

- (void)cancelMode;
@end

NS_ASSUME_NONNULL_END
