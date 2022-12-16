using System;
using System.Runtime.InteropServices;
using UnityEngine;
using TapTap.Common;

namespace Plugins.AntiAddictionUIKit
{
    [Serializable]
    public class CheckPayResult
    {
        public int status;
        public string title;
        public string description;
    }

    [Serializable]
    public class AntiAddictionCallbackOriginData
    {
        public int code;
        public string extras;
    }

    [Serializable]
    public class AntiAddictionCallbackData
    {
        public int code;
        public MsgExtraParams extras;
    }

    [Serializable]
    public class IdetntifyState
    {
        public int authState;

        public string antiAddictionToken;

        public int ageLimit;
    }

    [Serializable]
    public class IdentifyResult
    {
        public int identifyState;
    }

    [Serializable]
    public class MsgExtraParams
    {
        public int userType = -1;
        public string limit_tip_type = "";
        public string strict_type = "";
        public string description = "";
        public string title = "";
    }

    [Serializable]
    public class CheckPayResultParams
    {
        public int status;
        public string title;
        public string description;
    }

    [Serializable]
    public class SubmitPayResultParams
    {
    }


    public static class AntiAddictionUIKit
    {
        // Game object is created to receive async messages
        private const string UNITY_SDK_VERSION = "3.13.0";
        private const string GAME_OBJECT_NAME = "PluginBridge";
        private static GameObject gameObject;

        // Android only variables
        private const string JAVA_OBJECT_NAME = "com.tapsdk.antiaddictionui.NativeAntiAddictionUIKitPlugin";
        private static AndroidJavaObject androidJavaNativeAntiAddiction;

        
        private static Action<AntiAddictionCallbackData> handleAsyncAntiAddictionMsg;
        private static Action<string> handleAsyncAntiAddictionMsgException;

        private static Action<string> handleAsyncAuthIdentityManualException;

        private static Action<CheckPayResult> handleCheckPayLimit;

        private static Action<string> handleCheckPayLimitException;

        private static Action handleSubmitPayResult;

        private static Action<string> handleSubmitPayResultException;

        // iOS only variables
        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void initSDK(string gameIdentifier
            , bool useTimeLimit
            , bool usePaymentLimit
            , bool showSwitchAccount);

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void startup(string userIdentifier, bool useTapLogin);

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void logout();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void enterGame();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void leaveGame();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void checkPayLimit(long amount);

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void submitPayResult(long amount);

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern int getCurrentUserRemainTime();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern int getCurrentUserAgeLimit();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern string getCurrentAntiToken();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern bool standalone();

        #if UNITY_IOS
        [DllImport("__Internal")]
        #endif
        private static extern void setUnityVersion(string version);

        private class PlatformNotSupportedException : Exception
        {
            public PlatformNotSupportedException() : base()
            {

            }
        }

        static AntiAddictionUIKit()
        {
            gameObject = new GameObject();
            gameObject.name = GAME_OBJECT_NAME;

            // attach this class to allow for handling of callbacks from Java or Objective c
            gameObject.AddComponent<NativeAntiAddictionUICallbackHandler>();
            gameObject.AddComponent<NativeCheckPayLimitHandler>();
            gameObject.AddComponent<NativeSubmitPayResultHandler>();

            // Do not destroy when loading a new scene
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            switch (Application.platform)
            {
                case RuntimePlatform.Android:

                    // Initialize native Java object
                    androidJavaNativeAntiAddiction = new AndroidJavaObject(JAVA_OBJECT_NAME);
                    break;

                case RuntimePlatform.IPhonePlayer:
                    break;

                default:
                    throw new PlatformNotSupportedException();

            }
        }

        private class NativeAntiAddictionUICallbackHandler : MonoBehaviour
        {
            private void HandleException(string exception)
            {
                handleAsyncAntiAddictionMsgException?.Invoke(exception);
            }

            private void HandleAntiAddictionCallbackMsg(string antiAddictionCallbackDataJSON)
            {
                Debug.Log("HandleAntiAddictionCallbackMsg antiAddictionCallbackDataJSON:" + antiAddictionCallbackDataJSON);
                var antiAddictionCallbackOriginData = JsonUtility.FromJson<AntiAddictionCallbackOriginData>(antiAddictionCallbackDataJSON);

                Debug.Log("HandleAntiAddictionCallbackMsg resultCode:" + antiAddictionCallbackOriginData.code);

                var result = new AntiAddictionCallbackData();
                result.code = antiAddictionCallbackOriginData.code;

                if (antiAddictionCallbackOriginData.extras != null && antiAddictionCallbackOriginData.extras.Length > 0) {
                    result.extras = JsonUtility.FromJson<MsgExtraParams>(antiAddictionCallbackOriginData.extras);
                    Debug.Log("result.extras title:" + result.extras.title);
                    Debug.Log("result.extras description:" + result.extras.description);
                }
                handleAsyncAntiAddictionMsg?.Invoke(result);
            }
        }

        private class NativeCheckPayLimitHandler : MonoBehaviour
        {
            private void HandleCheckPayLimitException(string exception)
            {
                handleCheckPayLimitException?.Invoke(exception);
            }

            private void HandleCheckPayLimit(string checkPayResultJSON)
            {
                var result = JsonUtility.FromJson<CheckPayResult>(checkPayResultJSON);
                handleCheckPayLimit?.Invoke(result);
            }
        }

        private class NativeSubmitPayResultHandler : MonoBehaviour
        {
            private void HandleSubmitPayResultException(string exception)
            {
                handleSubmitPayResultException?.Invoke(exception);
            }

            private void HandleSubmitPayResult()
            {
                handleSubmitPayResult?.Invoke();
            }
        }

        /*
         * ------------------
         * Interface Metthods
         * ------------------
         */
        public static void Init(string gameIdentifier
            , bool useTimeLimit
            , bool usePaymentLimit
            , Action<AntiAddictionCallbackData> handleAsyncAntiAddictionMsg
            , Action<string> handleAsyncAntiAddictionMsgException)
        {
            AntiAddictionUIKit.handleAsyncAntiAddictionMsg = handleAsyncAntiAddictionMsg;
            AntiAddictionUIKit.handleAsyncAntiAddictionMsgException = handleAsyncAntiAddictionMsgException;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidInit(gameIdentifier, useTimeLimit, usePaymentLimit);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSInit(gameIdentifier, useTimeLimit, usePaymentLimit);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }

            SetUnitySDKVersion(UNITY_SDK_VERSION);
        }

        public static void Init(string gameIdentifier
            , bool useTimeLimit
            , bool usePaymentLimit
            , bool showSwitchAccount
            , Action<AntiAddictionCallbackData> handleAsyncAntiAddictionMsg
            , Action<string> handleAsyncAntiAddictionMsgException)
        {
            AntiAddictionUIKit.handleAsyncAntiAddictionMsg = handleAsyncAntiAddictionMsg;
            AntiAddictionUIKit.handleAsyncAntiAddictionMsgException = handleAsyncAntiAddictionMsgException;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidInit(gameIdentifier, useTimeLimit, usePaymentLimit, showSwitchAccount);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSInit(gameIdentifier, useTimeLimit, usePaymentLimit, showSwitchAccount);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }

            SetUnitySDKVersion(UNITY_SDK_VERSION);
        }

        public static void Startup(bool useTapLogin, string userIdentifier) {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidStartup(useTapLogin, userIdentifier);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSStartup(useTapLogin, userIdentifier);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void EnterGame()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidEnterGame();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSEnterGame();
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void LeaveGame()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidLeaveGame();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSLeaveGame();
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static string CurrentToken()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return PerformAndroidGetCurrentToken();
                case RuntimePlatform.IPhonePlayer:
                    return PerformiOSGetCurrentToken();
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static int CurrentUserAgeLimit()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return PerformAndroidGetCurrentUserAgeLimit();
                case RuntimePlatform.IPhonePlayer:
                    return PerformiOSGetCurrentUserAgeLimit();
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static int CurrentUserRemainTime()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return PerformAndroidGetCurrentUserRemainTime();
                case RuntimePlatform.IPhonePlayer:
                    return PerformiOSGetCurrentUserRemainTime();
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void Logout()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidLogout();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSLogout();
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void CheckPayLimit(long amount
            , Action<CheckPayResult> handleCheckPayLimit
            , Action<string> handleCheckPayLimitException)
        {
            AntiAddictionUIKit.handleCheckPayLimit = handleCheckPayLimit;
            AntiAddictionUIKit.handleCheckPayLimitException = handleCheckPayLimitException;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidCheckPayLimit(amount);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSCheckPayLimit(amount);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void SubmitPayResult(long amount
            , Action handleSubmitPayResult
            , Action<string> handleSubmitPayResultException
            )
        {
            AntiAddictionUIKit.handleSubmitPayResult = handleSubmitPayResult;
            AntiAddictionUIKit.handleSubmitPayResultException = handleSubmitPayResultException;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidSubmitPayResult(amount);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSSubmitPayResult(amount);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void SetUnitySDKVersion(string version)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PerformAndroidSetUnityVersion(version);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    PerformiOSSetUnityVersion(version);
                    break;
                default:
                    throw new PlatformNotSupportedException();

            }
        }

        public static bool isStandalone() {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return (PerformAndroidIsStandalone() == 1) ? true : false;
                case RuntimePlatform.IPhonePlayer:
                    return PerformiOSIsStandalone();
                default:
                    throw new PlatformNotSupportedException();

            }
        }

        public static void SetTestMode(bool testMode)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    setTestMode(testMode);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    setTestMode(testMode);
                    break;
            }
        }

        /*
         * ------------------
         * Internal Metthods(Android)
         * ------------------
         */
        private static void PerformAndroidInit(
            string gameIdentifier
            , bool useTimeLimit
            , bool usePaymentLimit
            )
        {
            Debug.Log("Android Init calling:" + gameIdentifier);
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaNativeAntiAddiction.Call("initSDK"
                , unityActivity
                , gameIdentifier
                , useTimeLimit
                , usePaymentLimit
                );
        }

        private static void PerformAndroidInit(
            string gameIdentifier
            , bool useTimeLimit
            , bool usePaymentLimit
            , bool showSwitchAccount
            )
        {
            Debug.Log("Android Init calling:" + gameIdentifier);
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaNativeAntiAddiction.Call("initSDK"
                , unityActivity
                , gameIdentifier
                , useTimeLimit
                , usePaymentLimit
                , showSwitchAccount
                );
        }

        private static void PerformAndroidStartup(bool useTapLogin, string userIdentifier)
        {
            Debug.Log("Android startup calling");
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaNativeAntiAddiction.Call("startup", unityActivity, useTapLogin, userIdentifier);
        }

        private static string PerformAndroidGetCurrentToken()
        {
            Debug.Log("Android getCurrrentToken calling");
            return androidJavaNativeAntiAddiction.Call<string>("getCurrentToken");
        }

        private static int PerformAndroidGetCurrentUserAgeLimit()
        {
            Debug.Log("Android getCurrrentUserAgeLimit calling");
            return androidJavaNativeAntiAddiction.Call<int>("getCurrentUserAgeLimit");
        }

        private static int PerformAndroidGetCurrentUserRemainTime()
        {
            Debug.Log("Android getCurrrentRemainTime calling");
            return androidJavaNativeAntiAddiction.Call<int>("getCurrentUserRemainTime");
        }

        private static void PerformAndroidLogout()
        {
            Debug.Log("Android logout calling");
            androidJavaNativeAntiAddiction.Call("logout");
        }

        private static void PerformAndroidCheckPayLimit(long amount)
        {
            Debug.Log("Android checkPayLimit calling");
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaNativeAntiAddiction.Call("checkPayLimit", unityActivity, amount);
        }

        private static void PerformAndroidSubmitPayResult(long amount)
        {
            Debug.Log("Android submitPayResult calling");
            androidJavaNativeAntiAddiction.Call("submitPayResult", amount);
        }

        private static void PerformAndroidEnterGame()
        {
            Debug.Log("Android enterGame calling");
            androidJavaNativeAntiAddiction.Call("enterGame");
        }

        private static void PerformAndroidLeaveGame()
        {
            Debug.Log("Android leaveGame calling");
            androidJavaNativeAntiAddiction.Call("leaveGame");
        }

        private static void PerformAndroidSetUnityVersion(string version)
        {
            Debug.Log("Android setUnityVersion calling");
            androidJavaNativeAntiAddiction.Call("setUnityVersion", version);
        }

        private static int PerformAndroidIsStandalone()
        {
            int result = androidJavaNativeAntiAddiction.Call<int>("isStandalone");
            Debug.Log("Android PerformAndroidIsStandalone calling" + result.ToString());
            return result;
        }

        /*
         * ------------------
         * Internal Method(iOS)
         * ------------------
         */
        private static void PerformiOSInit(string gameIdentifier
                , bool useTimeLimit
                , bool usePaymentLimit
            )
        {
            Debug.Log("PerformiOSInit:" + gameIdentifier);
            initSDK(gameIdentifier, useTimeLimit, usePaymentLimit, true);
        }

        private static void PerformiOSInit(string gameIdentifier
                , bool useTimeLimit
                , bool usePaymentLimit
                , bool showSwitchAccount
            )
        {
            Debug.Log("PerformiOSInit:" + gameIdentifier);
            initSDK(gameIdentifier, useTimeLimit, usePaymentLimit, showSwitchAccount);
        }

        private static void PerformiOSStartup(
                bool useTapLogin
                , string userIdentifier
            )
        {
            Debug.Log("PerformiOSStartup:" + userIdentifier);
            startup(userIdentifier, useTapLogin);
        }

        private static string PerformiOSGetCurrentToken()
        {
            // to implement
            Debug.Log("PerformiOSGetCurrentToken");
            return getCurrentAntiToken();
        }

        private static int PerformiOSGetCurrentUserAgeLimit()
        {
            // to implement
            return getCurrentUserAgeLimit();
        }

        private static int PerformiOSGetCurrentUserRemainTime()
        {
            // to implement
            return getCurrentUserRemainTime();
        }

        private static void PerformiOSEnterGame()
        {
            enterGame();
        }

        private static void PerformiOSLeaveGame()
        {
            leaveGame();
        }

        private static void PerformiOSLogout()
        {
            logout();
        }

        private static void PerformiOSCheckPayLimit(long amount)
        {
            checkPayLimit(amount);
        }

        private static void PerformiOSSubmitPayResult(long amount)
        {
            submitPayResult(amount);
        }

        private static void PerformiOSSetUnityVersion(string version)
        {
            setUnityVersion(version);
        }

        private static Boolean PerformiOSIsStandalone() {
            return standalone();
        }

        private static void setTestMode(bool testMode) {
            if (testMode)
            {
                var command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://www.taptap.com")
                    .Args("replacedHost",  "https://www.xdrnd.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("shareInstance")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://tds-tapsdk.cn.tapapis.com")
                    .Args("replacedHost",  "https://tds-api.xdrnd.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1pM6yfulomBTXWKiQT5gK9fY4hq11Kv8D+ewum25oPGReuEn6dez7ogA8bEyQlnYYUoEp5cxYPBbIxJFy7q1qzQhTFphuFzoC1x7DieTvfZbh+b60psEottrCD8M0Pa3h44pzyIp5U5WRpxRcQ9iULolGLHZXJr9nW6bpOsyEIFG5tQ7qCBj8HSFoNBKZH+5Cwh3j5cjmyg55WdJTimg9ysbbwZHYmI+TFPuGo/ckHT6j4TQLCmmxI8Qf5pycn3/qJWFhjx/y8zaxgn2hgxbma8hyyGRCMnhM5tISYQv4zlQF+5RashvKa2zv+FHA5DALzIsGXONeTxk6TSBalX5gQIDAQAB")
                    .Args("replacedHost",     "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAks5GmqBvtcVihXvUorEh3KTHBteK36/4G5e3UOnYKbahspU9+FaJx/GaQxdtnFzkVXoGHVlYkhYokY12YO+OVB9INSgzwfxDGd2ttAsqCsUBl0GCVDzBHnS0agf7hk6YVljG8dRN01yW6q50XQCqyS2L3bfXDuWmUT8upgZ6fJSJRRRGh+vt9AJRBwZb3vQ/d/iejWH/64mGnM154CZGr+28SZ8AAXiCJ0BrfyGZqbhoqeWbFUbI7zv3FXiawuqS5EatH5ZU0ll14MlXdcIK7NzcDKCb/tekkr5zPDdTOPkQ5KDrwOx6oMEYs1sLC37nB0Me6mQWQPZY0lYPQ/GmwwIDAQAB").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://openapi.taptap.com")
                    .Args("replacedHost",  "https://open.api.xdrnd.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
            }
            else
            {
                var command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://www.taptap.com")
                    .Args("replacedHost",  "https://www.taptap.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("shareInstance")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://tds-tapsdk.cn.tapapis.com")
                    .Args("replacedHost",  "https://tds-tapsdk.cn.tapapis.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1pM6yfulomBTXWKiQT5gK9fY4hq11Kv8D+ewum25oPGReuEn6dez7ogA8bEyQlnYYUoEp5cxYPBbIxJFy7q1qzQhTFphuFzoC1x7DieTvfZbh+b60psEottrCD8M0Pa3h44pzyIp5U5WRpxRcQ9iULolGLHZXJr9nW6bpOsyEIFG5tQ7qCBj8HSFoNBKZH+5Cwh3j5cjmyg55WdJTimg9ysbbwZHYmI+TFPuGo/ckHT6j4TQLCmmxI8Qf5pycn3/qJWFhjx/y8zaxgn2hgxbma8hyyGRCMnhM5tISYQv4zlQF+5RashvKa2zv+FHA5DALzIsGXONeTxk6TSBalX5gQIDAQAB")
                    .Args("replacedHost",     "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1pM6yfulomBTXWKiQT5gK9fY4hq11Kv8D+ewum25oPGReuEn6dez7ogA8bEyQlnYYUoEp5cxYPBbIxJFy7q1qzQhTFphuFzoC1x7DieTvfZbh+b60psEottrCD8M0Pa3h44pzyIp5U5WRpxRcQ9iULolGLHZXJr9nW6bpOsyEIFG5tQ7qCBj8HSFoNBKZH+5Cwh3j5cjmyg55WdJTimg9ysbbwZHYmI+TFPuGo/ckHT6j4TQLCmmxI8Qf5pycn3/qJWFhjx/y8zaxgn2hgxbma8hyyGRCMnhM5tISYQv4zlQF+5RashvKa2zv+FHA5DALzIsGXONeTxk6TSBalX5gQIDAQAB").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
                
                command = new Command.Builder()
                    .Service("TDSCommonService")
                    .Method("addReplacedHostPair")
                    .Args("hostToBeReplaced", "https://openapi.taptap.com")
                    .Args("replacedHost",  "https://openapi.taptap.com").CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command);
            }
        }
    }
}