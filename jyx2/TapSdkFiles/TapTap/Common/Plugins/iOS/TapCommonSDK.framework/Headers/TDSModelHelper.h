//
//  TDSModelHelper.h
//  TDSCommon
//
//  Created by Insomnia on 2020/10/20.
//

#import <Foundation/Foundation.h>
#import <objc/runtime.h>

#ifndef __TDSConst__
#define __TDSConst__
extern NSString *const TDSTypeInt;
extern NSString *const TDSTypeFloat;
extern NSString *const TDSTypeDouble;
extern NSString *const TDSTypeLong;
extern NSString *const TDSTypeLongLong;
extern NSString *const TDSTypeChar;
extern NSString *const TDSTypeBOOL;
extern NSString *const TDSTypePointer;

extern NSString *const TDSTypeIvar;
extern NSString *const TDSTypeMethod;
extern NSString *const TDSTypeBlock;
extern NSString *const TDSTypeClass;
extern NSString *const TDSTypeSEL;
extern NSString *const TDSTypeId;

#endif



@interface TDSPropertyType : NSObject
/** 类型标识符 */
@property (nonatomic, copy) NSString *code;

/** 是否为id类型 */
@property (nonatomic, readonly, getter=isIdType) BOOL idType;

/** 对象类型（如果是基本数据类型，此值为nil） */
@property (nonatomic, readonly) Class typeClass;

/** 类型是否来自于Foundation框架，比如NSString、NSArray */
@property (nonatomic, readonly, getter = isFromFoundation) BOOL fromFoundation;
/** 类型是否不支持KVC */
@property (nonatomic, readonly, getter = isKVCDisabled) BOOL KVCDisabled;

/**
 *  获得缓存的类型对象
 */
+ (instancetype)cachedTypeWithCode:(NSString *)code;
@end

@interface TDSProperty : NSObject
/** 成员属性 */
@property (nonatomic, assign) objc_property_t property;
/** 成员属性名 */
@property (nonatomic, readonly) NSString *name;

/** 成员变量的类型 */
@property (nonatomic, readonly) TDSPropertyType *type;
/** 成员来源于哪个类（可能是父类） */
@property (nonatomic, assign) Class srcClass;

/**** 同一个成员变量 - 父类和子类的行为可能不一致（key、keys、objectClassInArray） ****/
/** 对应着字典中的key */
- (void)setKey:(NSString *)key forClass:(Class)c;
- (NSString *)keyFromClass:(Class)c;

/** 对应着字典中的多级key */
- (NSArray *)keysFromClass:(Class)c;

/** 模型数组中的模型类型 */
- (void)setObjectClassInArray:(Class)objectClass forClass:(Class)c;
- (Class)objectClassInArrayFromClass:(Class)c;
/**** 同一个成员变量 - 父类和子类的行为可能不一致（key、keys、objectClassInArray） ****/

/**
 * 设置成员变量的值
 */
- (void)setValue:(id)value forObject:(id)object;
/**
 * 得到成员变量的值
 */
- (id)valueFromObject:(id)object;

/**
 *  初始化
 */
+ (instancetype)cachedPropertyWithProperty:(objc_property_t)property;
@end
