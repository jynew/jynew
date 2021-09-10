/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Jyx2
{
    [RequireComponent(typeof(Animator))]
    public class MapChestInteract : MonoBehaviour
    {

        public AnimationClip clip;

        private AnimancerComponent _animancerComponent;

        private void Awake()
        {
            var animator=this.GetComponent<Animator>() ?? this.gameObject.AddComponent<Animator>();
            _animancerComponent = this.GetComponent<AnimancerComponent>() ??
                                  this.gameObject.AddComponent<AnimancerComponent>();
            _animancerComponent.Animator = animator;
        }

        /// <summary>
        /// 设置为打开状态
        /// </summary>
        public void SetOpened()
        {
            var state = _animancerComponent.Play(clip);
            state.Events.OnEnd = _animancerComponent.Playable.PauseGraph;
            state.NormalizedTime = 1;
        }
        public IEnumerator Open()
        {
            _animancerComponent.Playable.UnpauseGraph();
            var state = _animancerComponent.Play(clip);
            state.Speed = 1;
            state.Events.OnEnd = () =>
            {
                _animancerComponent.Playable.PauseGraph();
            };
            yield return new WaitForSeconds(clip.length);
        }

        public void Close(Action action = null)
        {
            _animancerComponent.Playable.UnpauseGraph();
            var state = _animancerComponent.Play(clip);
            state.Speed = -1;
            state.Events.OnEnd = () =>
            {
                action?.Invoke();
                _animancerComponent.Playable.PauseGraph();
            };
        }
    }
}