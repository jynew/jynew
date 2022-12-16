//
//  TDSCommonDialogView.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/4/29.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSCommonDialogProtocol <NSObject>

- (void)onReloadData;
- (void)onClose;
@end

@interface TDSCommonDialogView : UIView
@property (nonatomic, weak) id<TDSCommonDialogProtocol> delegate;

@property (nonatomic, strong) UIView *dialogView;

//loading
@property (nonatomic, strong) UIView *loadingView;
//reload
@property (nonatomic, strong) UIView *reloadView;

- (void)closeDialog;
@end

NS_ASSUME_NONNULL_END
