//
//  NSObject+TDSCoding.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSObject (TDSCoding)
/**
 *  解码（从文件中解析对象）
 */
- (void)tds_decode:(NSCoder *)decoder;
/**
 *  编码（将对象写入文件中）
 */
- (void)tds_encode:(NSCoder *)encoder;
@end

NS_ASSUME_NONNULL_END
