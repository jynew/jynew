using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Network = TapTap.AntiAddiction.Internal.Network;

namespace TapTap.AntiAddiction 
{
    /// <summary>
    /// 防沉迷轮询器
    /// </summary>
    internal class AntiAddictionPoll : MonoBehaviour 
    {
        static readonly string ANTI_ADDICTION_POLL_NAME = "AntiAddictionPoll";

        static AntiAddictionPoll current;

        /// <summary>
        /// 轮询间隔，单位：秒
        /// </summary>
        private static int pollInterval = 2 * 60;

        private static Coroutine _pollCoroutine;

        private static float? _elpased;

        public static bool StartPoll;

        internal static void StartUp() 
        {
            if (current == null) 
            {
                GameObject pollGo = new GameObject(ANTI_ADDICTION_POLL_NAME);
                DontDestroyOnLoad(pollGo);
                current = pollGo.AddComponent<AntiAddictionPoll>();
                _elpased = null;
            }

            if (_pollCoroutine == null)
            {
                _pollCoroutine = current.StartCoroutine(current.Poll());
                StartPoll = true;
            }
        }
        
        internal static void StartCountdownRemainTime() 
        {
            if (current == null) 
            {
                GameObject pollGo = new GameObject(ANTI_ADDICTION_POLL_NAME);
                DontDestroyOnLoad(pollGo);
                current = pollGo.AddComponent<AntiAddictionPoll>();
                _elpased = null;
            }
            else
            {
                return;
            }

            _elpased = 0;
        }

        internal static void Logout()
        {
            StartPoll = false;
            _elpased = null;
            current?.StopAllCoroutines();
            _pollCoroutine = null;
        }

        private void Update()
        {
            if (_elpased != null)
            {
                _elpased += Time.unscaledDeltaTime;
                if (_elpased >= 1)
                {
                    _elpased = 0;
                    if (TapTapAntiAddictionManager.CurrentRemainSeconds != null)
                        TapTapAntiAddictionManager.CurrentRemainSeconds--;
                }
            }
        }

        IEnumerator Poll() 
        {
            while (true) 
            {
                // 上报/检查可玩
                Task<PlayableResult> checkPlayableTask = TapTapAntiAddictionManager.CheckPlayableOnPolling();
                yield return new WaitUntil(() => checkPlayableTask.IsCompleted);
                Debug.LogFormat($"防沉迷 轮询检查是否可玩: {checkPlayableTask.Result.CanPlay} Time: " +
                                $"{DateTime.Now:hh:mm:ss ddd} 剩余时间(秒): {checkPlayableTask.Result.RemainTime}");
                if (!checkPlayableTask.Result.CanPlay)
                {
                    _elpased = null;
                    break;
                }

                if (_elpased == null)
                    _elpased = 0;
                
                yield return new WaitForSeconds(pollInterval - 1);
            }
        }

        /// <summary>
        /// 离开或者暂停的时候,发送心跳,方便 server 校对时间
        /// </summary>
        /// <param name="pauseStatus"></param>
        private void OnApplicationPause(bool pauseStatus)
        {
            SendPlayableRequest();
        }
        
        /// <summary>
        /// 离开或者暂停的时候,发送心跳,方便 server 校对时间
        /// </summary>
        private void OnApplicationQuit()
        {
            SendPlayableRequest();
        }

        private static void SendPlayableRequest()
        {
#pragma warning disable CS4014
            Network.CheckPlayable();
#pragma warning restore CS4014
        }
    }
}
