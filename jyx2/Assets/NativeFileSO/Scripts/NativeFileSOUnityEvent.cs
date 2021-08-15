// 	Copyright (c) 2019 Keiwan Donyagard
// 
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Keiwando.NFSO { 

	public class NativeFileSOUnityEvent : MonoBehaviour {

		public static event Action UnityReceivedControl;

		private static NativeFileSOUnityEvent instance;

		void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}

		private void Start() {
			SendEvent();
		}

		private void OnApplicationFocus(bool focus) {

			if (focus) {
				SendEvent();
			}
		}

		private void OnApplicationPause(bool pause) {
			if (!pause) {
				SendEvent();
			}
		}

		private void SendEvent() {
			if (UnityReceivedControl != null) {
				UnityReceivedControl();
			}
		}
	}
}
