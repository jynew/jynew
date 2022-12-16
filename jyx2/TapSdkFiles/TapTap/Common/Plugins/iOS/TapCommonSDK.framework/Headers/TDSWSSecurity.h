//////////////////////////////////////////////////////////////////////////////////////////////////
//
//  TDSWSSecurity.h
//
//  Created by Austin and Dalton Cherry on on 9/3/15.
//  Copyright (c) 2014-2017 Austin Cherry.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
//////////////////////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import <Security/Security.h>

@interface TDSWSSSLCert : NSObject

/**
 Designated init for certificates
 
 :param: data is the binary data of the certificate
 
 :returns: a representation security object to be used with
 */
- (instancetype)initWithData:(NSData *)data;


/**
 Designated init for public keys
 
 :param: key is the public key to be used
 
 :returns: a representation security object to be used with
 */
- (instancetype)initWithKey:(SecKeyRef)key;

@end

@interface TDSWSSecurity : NSObject

/**
 Use certs from main app bundle
 
 :param usePublicKeys: is to specific if the publicKeys or certificates should be used for SSL pinning validation
 
 :returns: a representation security object to be used with
 */
- (instancetype)initWithCerts:(NSArray<TDSWSSSLCert*>*)certs publicKeys:(BOOL)publicKeys;

/**
 Designated init
 
 :param keys: is the certificates or public keys to use
 :param usePublicKeys: is to specific if the publicKeys or certificates should be used for SSL pinning validation
 
 :returns: a representation security object to be used with
 */
- (instancetype)initUsingPublicKeys:(BOOL)publicKeys;

/**
 Should the domain name be validated? Default is YES.
 */
@property(nonatomic)BOOL validatedDN;

/**
 Validate if the cert is legit or not.
 :param:  trust is the trust to validate
 :param: domain to validate along with the trust (can be nil)
 :return: YES or NO if valid.
 */
- (BOOL)isValid:(SecTrustRef)trust domain:(NSString*)domain;

@end
