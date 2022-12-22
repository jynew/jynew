//
//  TDSCommonConfirmDialog.h
//  TDSCommon
//
//  Created by Bottle K on 2021/3/2.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

typedef void (^TDSCommonConfirmDialogHandler) (BOOL confirm);

@interface TDSCommonConfirmDialog : UIView
- (void)setupWithTitle:(NSString *)title
               content:(NSString *)content
            cancelText:(NSString *)cancelText
           confirmText:(NSString *)confirmText
               handler:(TDSCommonConfirmDialogHandler)handler;
@end

NS_ASSUME_NONNULL_END
