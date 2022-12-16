//
//  TapBillboardConfig.h
//  TapCommonSDK
//
//  Created by TapTap on 2022/7/18.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

#define TEMPLATE_NAVIGATE @"navigate"
#define TEMPLATE_IMAGE @"image"

@interface TapBillboardConfig : NSObject
@property (nonatomic, copy) NSSet<NSArray *> *diemensionSet;
@property (nonatomic, copy) NSString *templateType;
@property (nonatomic, copy) NSString *serverUrl;

@end

NS_ASSUME_NONNULL_END
