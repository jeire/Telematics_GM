/*
 * Copyright (c) Certicom Corp. 2008-2019.   All rights reserved.
 * 
 * This software cannot be used, reproduced, or distributed in whole or
 * in part by any means without the explicit prior consent of Certicom
 * Corp. Such consent must arise from a separate license agreement from
 * Certicom or its licensees, as appropriate.
 * 
 * ACC, Asset Control Core, Certicom, Certicom AMS, Certicom Bar Code
 * Authentication Agent, Certicom ECC Core, Certicom Security Architecture,
 * Certicom Trust Infrastructure, Certicom CodeSign, Certicom KeyInject,
 * ChipActivate, DieMax, Security Builder, Security Builder API, Security
 * Builder API for .NET, Security Builder BSP, Security Builder Crypto,
 * Security Builder ETS, Security Builder GSE, Security Builder IPSec, Security
 * Builder MCE, Security Builder NSE, Security Builder PKI, Security Builder
 * SSL, and SysActivate are trademarks or registered trademarks of Certicom
 * Corp. All other companies and products listed herein are trademarks or
 * registered trademarks of their respective holders.
 *
 * BlackBerry (R), RIM (R), Research In Motion (R) and related trademarks
 * are owned by Research In Motion Limited. Used under license.
 *
 * This software contains proprietary information of Certicom and
 * distribution is limited to authorized licensees of Certicom. Any
 * unauthorized reproduction or distribution of this document is strictly
 * prohibited.
 */

/*
 * amsagterr.h - AMS agent error header file
 */

#ifndef _AMS_AGTERR_H_
#define _AMS_AGTERR_H_

#ifdef __cplusplus
extern "C" {
#endif

/** @addtogroup amserrors Agent error codes
 *
 * @{
 *
 */
enum {
    AMS_AGT_ERR_NONE                                = 0x0000,
    AMS_AGT_ERR_BASE                                = 0xAA00,
    AMS_AGT_ERR_NULL_PTR                            = 0xAA01,
    AMS_AGT_ERR_BAD_PARAM                           = 0xAA02,
    AMS_AGT_ERR_MEMORY                              = 0xAA03,
    AMS_AGT_ERR_INITIALIZE_SYSTEM_FAIL              = 0xAA04,
    AMS_AGT_ERR_SHUTDOWN_SYSTEM_FAIL                = 0xAA05,
    AMS_AGT_ERR_CONNECT_FAIL                        = 0xAA06,
    AMS_AGT_ERR_INVALID_STORE                       = 0xAA07,
    AMS_AGT_ERR_STORE_EMPTY                         = 0xAA08,
    AMS_AGT_ERR_STORE_FULL                          = 0xAA09,
    AMS_AGT_ERR_UNKNOWN_PRODUCT                     = 0xAA0A,
    AMS_AGT_ERR_REQUEST_TOO_LARGE                   = 0xAA0B,
    AMS_AGT_ERR_UNKNOWN_OID                         = 0xAA0C,
    AMS_AGT_ERR_SYSTEM_NOT_INITIALIZED              = 0xAA0D,
    AMS_AGT_ERR_NO_MORE_APPLIANCE                   = 0xAA0E,
    AMS_AGT_ERR_NAME_INVALID_LENGTH                 = 0xAA0F,
    AMS_AGT_ERR_INVALID_SERVICE                     = 0xAA10,
    AMS_AGT_ERR_ID_INVALID_LENGTH                   = 0xAA11,
    AMS_AGT_ERR_LOG_INVALID_LENGTH                  = 0xAA12,
    AMS_AGT_ERR_STATUS_ALREADY_SET                  = 0xAA13,
    AMS_AGT_ERR_APPLIANCE_GET_ASSETS                = 0xAA14,
    AMS_AGT_ERR_APPLIANCE_PROCESS_LOGS              = 0xAA15,
    AMS_AGT_ERR_INVALID_PRODUCT                     = 0xAA16,
    AMS_AGT_ERR_PRODUCT_REQUIRES_ACC                = 0xAA17,
    AMS_AGT_ERR_INVALID_ACC_DATA                    = 0xAA18,
    AMS_AGT_ERR_MALFORMED_CUSTOM_STRING             = 0xAA19,
    AMS_AGT_ERR_LOGGING_NOT_INITIALIZED             = 0xAA1A,
    AMS_AGT_ERR_APPLIANCE_CONNECT_FAIL              = 0xAA1B,
    AMS_AGT_ERR_PROFILE_NOT_FOUND                   = 0xAA1C,
    AMS_AGT_ERR_PROFILE_NOT_ACTIVE                  = 0xAA1D,
    AMS_AGT_ERR_PROFILE_NOT_RUNNING                 = 0xAA1E,
    AMS_AGT_ERR_PROFILE_BUSY                        = 0xAA1F,
    AMS_AGT_ERR_CLIENT_WAITED_SERVER_TOO_LONG       = 0xAA20, 
    AMS_AGT_ERR_INVALID_IDENTIFIERS                 = 0xAA21,
    AMS_AGT_ERR_APPLIANCE_PROCESS_KEYS_AND_LOGS     = 0xAA22,
    AMS_AGT_ERR_ID_INVALID_FORMAT                   = 0xAA23,
    AMS_AGT_ERR_PARTIAL_SUCCESS                     = 0xAA24,
    AMS_AGT_ERR_UNKNOWN_DATATYPE                    = 0xAA25,
    AMS_AGT_ERR_MALFORMED_IDENTIFIER_STRING         = 0xAA26,
    AMS_AGT_ERR_INVALID_BLOCK                       = 0xAA27,
    AMS_AGT_ERR_INVALID_DATATYPE                    = 0xAA28,
    AMS_AGT_ERR_PUSHED_KEY_EXISTS                   = 0xAA29,
    AMS_AGT_ERR_MISSING_KEY_IDENTIFIER              = 0xAA2A,
    AMS_AGT_ERR_ARRAY_TOO_SMALL                     = 0xAA2B,

    AMS_AGT_ERR_INTERNAL                            = 0xAAFF
};

/** @} */

#ifdef __cplusplus
}
#endif

#endif //_AMS_AGTERR_H_

