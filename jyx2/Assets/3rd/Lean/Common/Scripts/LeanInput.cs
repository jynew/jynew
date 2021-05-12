using KeyCode = UnityEngine.KeyCode;
#if ENABLE_INPUT_SYSTEM
using NewCode = Unity​Engine.InputSystem.Key;
#endif

namespace Lean.Common
{
	/// <summary>This class wraps <b>UnityEngine.Input</b> and <b>UnityEngine.InputSystem</b> so they can both be used from the same interface.</summary>
	public static class LeanInput
	{
#if ENABLE_INPUT_SYSTEM
		private static System.Collections.Generic.Dictionary<KeyCode, NewCode> keyMapping = new System.Collections.Generic.Dictionary<KeyCode, NewCode>()
		{
			{ KeyCode.None, NewCode.None },
			{ KeyCode.Backspace, NewCode.Backspace },
			{ KeyCode.Tab, NewCode.Tab },
			{ KeyCode.Clear, NewCode.None },
			{ KeyCode.Return, NewCode.Enter },
			{ KeyCode.Pause, NewCode.Pause },
			{ KeyCode.Escape, NewCode.Escape },
			{ KeyCode.Space, NewCode.Space },
			{ KeyCode.Exclaim, NewCode.None },
			{ KeyCode.DoubleQuote, NewCode.None },
			{ KeyCode.Hash, NewCode.None },
			{ KeyCode.Dollar, NewCode.None },
			{ KeyCode.Percent, NewCode.None },
			{ KeyCode.Ampersand, NewCode.None },
			{ KeyCode.Quote, NewCode.Quote },
			{ KeyCode.LeftParen, NewCode.None },
			{ KeyCode.RightParen, NewCode.None },
			{ KeyCode.Asterisk, NewCode.None },
			{ KeyCode.Plus, NewCode.None },
			{ KeyCode.Comma, NewCode.Comma },
			{ KeyCode.Minus, NewCode.Minus },
			{ KeyCode.Period, NewCode.Period },
			{ KeyCode.Slash, NewCode.Slash },
			{ KeyCode.Alpha1, NewCode.Digit1 },
			{ KeyCode.Alpha2, NewCode.Digit2 },
			{ KeyCode.Alpha3, NewCode.Digit3 },
			{ KeyCode.Alpha4, NewCode.Digit4 },
			{ KeyCode.Alpha5, NewCode.Digit5 },
			{ KeyCode.Alpha6, NewCode.Digit6 },
			{ KeyCode.Alpha7, NewCode.Digit7 },
			{ KeyCode.Alpha8, NewCode.Digit8 },
			{ KeyCode.Alpha9, NewCode.Digit9 },
			{ KeyCode.Alpha0, NewCode.Digit0 },
			{ KeyCode.Colon, NewCode.None },
			{ KeyCode.Semicolon, NewCode.Semicolon },
			{ KeyCode.Less, NewCode.None },
			{ KeyCode.Equals, NewCode.Equals },
			{ KeyCode.Greater, NewCode.None },
			{ KeyCode.Question, NewCode.None },
			{ KeyCode.At, NewCode.None },
			{ KeyCode.LeftBracket, NewCode.LeftBracket },
			{ KeyCode.Backslash, NewCode.Backslash },
			{ KeyCode.RightBracket, NewCode.RightBracket },
			{ KeyCode.Caret, NewCode.None },
			{ KeyCode.Underscore, NewCode.None },
			{ KeyCode.BackQuote, NewCode.Backquote },
			{ KeyCode.A, NewCode.A },
			{ KeyCode.B, NewCode.B },
			{ KeyCode.C, NewCode.C },
			{ KeyCode.D, NewCode.D },
			{ KeyCode.E, NewCode.E },
			{ KeyCode.F, NewCode.F },
			{ KeyCode.G, NewCode.G },
			{ KeyCode.H, NewCode.H },
			{ KeyCode.I, NewCode.I },
			{ KeyCode.J, NewCode.J },
			{ KeyCode.K, NewCode.K },
			{ KeyCode.L, NewCode.L },
			{ KeyCode.M, NewCode.M },
			{ KeyCode.N, NewCode.N },
			{ KeyCode.O, NewCode.O },
			{ KeyCode.P, NewCode.P },
			{ KeyCode.Q, NewCode.Q },
			{ KeyCode.R, NewCode.R },
			{ KeyCode.S, NewCode.S },
			{ KeyCode.T, NewCode.T },
			{ KeyCode.U, NewCode.U },
			{ KeyCode.V, NewCode.V },
			{ KeyCode.W, NewCode.W },
			{ KeyCode.X, NewCode.X },
			{ KeyCode.Y, NewCode.Y },
			{ KeyCode.Z, NewCode.Z },
			{ KeyCode.LeftCurlyBracket, NewCode.None },
			{ KeyCode.Pipe, NewCode.None },
			{ KeyCode.RightCurlyBracket, NewCode.None },
			{ KeyCode.Tilde, NewCode.None },
			{ KeyCode.Delete, NewCode.Delete },
			{ KeyCode.Keypad0, NewCode.Numpad0 },
			{ KeyCode.Keypad1, NewCode.Numpad1 },
			{ KeyCode.Keypad2, NewCode.Numpad2 },
			{ KeyCode.Keypad3, NewCode.Numpad3 },
			{ KeyCode.Keypad4, NewCode.Numpad4 },
			{ KeyCode.Keypad5, NewCode.Numpad5 },
			{ KeyCode.Keypad6, NewCode.Numpad6 },
			{ KeyCode.Keypad7, NewCode.Numpad7 },
			{ KeyCode.Keypad8, NewCode.Numpad8 },
			{ KeyCode.Keypad9, NewCode.Numpad9 },
			{ KeyCode.KeypadPeriod, NewCode.NumpadPeriod },
			{ KeyCode.KeypadDivide, NewCode.NumpadDivide },
			{ KeyCode.KeypadMultiply, NewCode.NumpadMultiply },
			{ KeyCode.KeypadMinus, NewCode.NumpadMinus },
			{ KeyCode.KeypadPlus, NewCode.NumpadPlus },
			{ KeyCode.KeypadEnter, NewCode.NumpadEnter },
			{ KeyCode.KeypadEquals, NewCode.NumpadEquals },
			{ KeyCode.UpArrow, NewCode.UpArrow },
			{ KeyCode.DownArrow, NewCode.DownArrow },
			{ KeyCode.RightArrow, NewCode.RightArrow },
			{ KeyCode.LeftArrow, NewCode.LeftArrow },
			{ KeyCode.Insert, NewCode.Insert },
			{ KeyCode.Home, NewCode.Home },
			{ KeyCode.End, NewCode.End },
			{ KeyCode.PageUp, NewCode.PageUp },
			{ KeyCode.PageDown, NewCode.PageDown },
			{ KeyCode.F1, NewCode.F1 },
			{ KeyCode.F2, NewCode.F2 },
			{ KeyCode.F3, NewCode.F3 },
			{ KeyCode.F4, NewCode.F4 },
			{ KeyCode.F5, NewCode.F5 },
			{ KeyCode.F6, NewCode.F6 },
			{ KeyCode.F7, NewCode.F7 },
			{ KeyCode.F8, NewCode.F8 },
			{ KeyCode.F9, NewCode.F9 },
			{ KeyCode.F10, NewCode.F10 },
			{ KeyCode.F11, NewCode.F11 },
			{ KeyCode.F12, NewCode.F12 },
			{ KeyCode.F13, NewCode.None },
			{ KeyCode.F14, NewCode.None },
			{ KeyCode.F15, NewCode.None },
			{ KeyCode.Numlock, NewCode.NumLock },
			{ KeyCode.CapsLock, NewCode.CapsLock },
			{ KeyCode.ScrollLock, NewCode.ScrollLock },
			{ KeyCode.RightShift, NewCode.RightShift },
			{ KeyCode.LeftShift, NewCode.LeftShift },
			{ KeyCode.RightControl, NewCode.RightCtrl },
			{ KeyCode.LeftControl, NewCode.LeftCtrl },
			{ KeyCode.RightAlt, NewCode.RightAlt },
			{ KeyCode.LeftAlt, NewCode.LeftAlt },
			{ KeyCode.RightCommand, NewCode.RightCommand },
			//{ KeyCode.RightApple, NewCode.RightApple },
			{ KeyCode.LeftCommand, NewCode.LeftCommand },
			//{ KeyCode.LeftApple, NewCode.LeftApple },
			{ KeyCode.LeftWindows, NewCode.LeftWindows },
			{ KeyCode.RightWindows, NewCode.RightWindows },
			{ KeyCode.AltGr, NewCode.AltGr },
			{ KeyCode.Help, NewCode.None },
			{ KeyCode.Print, NewCode.PrintScreen },
			{ KeyCode.SysReq, NewCode.None },
			{ KeyCode.Break, NewCode.None },
			{ KeyCode.Menu, NewCode.ContextMenu },
		};

		private static UnityEngine.InputSystem.Controls.ButtonControl GetMouseButtonControl(int index)
		{
			if (UnityEngine.InputSystem.Mouse.current != null)
			{
				switch (index)
				{
					case 0: return UnityEngine.InputSystem.Mouse.current.leftButton;
					case 1: return UnityEngine.InputSystem.Mouse.current.rightButton;
					case 2: return UnityEngine.InputSystem.Mouse.current.middleButton;
					case 3: return UnityEngine.InputSystem.Mouse.current.forwardButton;
					case 4: return UnityEngine.InputSystem.Mouse.current.backButton;
				}
			}
			
			return null;
		}

		private static UnityEngine.InputSystem.Controls.ButtonControl GetButtonControl(KeyCode oldKey)
		{
			if (UnityEngine.InputSystem.Mouse.current != null)
			{
				switch (oldKey)
				{
					case KeyCode.Mouse0: return UnityEngine.InputSystem.Mouse.current.leftButton;
					case KeyCode.Mouse1: return UnityEngine.InputSystem.Mouse.current.rightButton;
					case KeyCode.Mouse2: return UnityEngine.InputSystem.Mouse.current.middleButton;
					case KeyCode.Mouse3: return UnityEngine.InputSystem.Mouse.current.forwardButton;
					case KeyCode.Mouse4: return UnityEngine.InputSystem.Mouse.current.backButton;
				}
			}

			if (UnityEngine.InputSystem.Keyboard.current != null)
			{
				var newKey = default(UnityEngine.InputSystem.Key);

				if (keyMapping.TryGetValue(oldKey, out newKey) == true)
				{
					return UnityEngine.InputSystem.Keyboard.current[newKey];
				}
			}

			return null;
		}
#endif
		public static int GetTouchCount()
		{
#if ENABLE_INPUT_SYSTEM
			if (UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled == false)
			{
				UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
			}

			return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
#else
			return UnityEngine.Input.touchCount;
#endif
		}

		public static void GetTouch(int index, out int id, out UnityEngine.Vector2 position, out float pressure, out bool set)
		{
#if ENABLE_INPUT_SYSTEM
			var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[index];

			id       = touch.finger.index;
			position = touch.screenPosition;
			pressure = touch.pressure;
			set      =
				touch.phase == UnityEngine.InputSystem.TouchPhase.Began ||
				touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
				touch.phase == UnityEngine.InputSystem.TouchPhase.Moved;
#else
			var touch = UnityEngine.Input.GetTouch(index);

			id       = touch.fingerId;
			position = touch.position;
			pressure = touch.pressure;
			set      =
				touch.phase == UnityEngine.TouchPhase.Began ||
				touch.phase == UnityEngine.TouchPhase.Stationary ||
				touch.phase == UnityEngine.TouchPhase.Moved;
#endif
		}

		public static UnityEngine.Vector2 GetMousePosition()
		{
#if ENABLE_INPUT_SYSTEM
			return UnityEngine.InputSystem.Mouse.current != null ? UnityEngine.InputSystem.Mouse.current.position.ReadValue() : default(UnityEngine.Vector2);
#else
			return UnityEngine.Input.mousePosition;
#endif
		}

		public static bool GetDown(KeyCode oldKey)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetButtonControl(oldKey); return buttonControl != null ? buttonControl.wasPressedThisFrame : false;
#else
			return UnityEngine.Input.GetKeyDown(oldKey);
#endif
		}

		public static bool GetPressed(KeyCode oldKey)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetButtonControl(oldKey); return buttonControl != null ? buttonControl.isPressed : false;
#else
			return UnityEngine.Input.GetKey(oldKey);
#endif
		}

		public static bool GetUp(KeyCode oldKey)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetButtonControl(oldKey); return buttonControl != null ? buttonControl.wasReleasedThisFrame : false;
#else
			return UnityEngine.Input.GetKeyUp(oldKey);
#endif
		}

		public static bool GetMouseDown(int index)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetMouseButtonControl(index); return buttonControl != null ? buttonControl.wasPressedThisFrame : false;
#else
			return UnityEngine.Input.GetMouseButtonDown(index);
#endif
		}

		public static bool GetMousePressed(int index)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetMouseButtonControl(index); return buttonControl != null ? buttonControl.isPressed : false;
#else
			return UnityEngine.Input.GetMouseButton(index);
#endif
		}

		public static bool GetMouseUp(int index)
		{
#if ENABLE_INPUT_SYSTEM
			var buttonControl = GetMouseButtonControl(index); return buttonControl != null ? buttonControl.wasReleasedThisFrame : false;
#else
			return UnityEngine.Input.GetMouseButtonUp(index);
#endif
		}

		public static float GetMouseWheelDelta()
		{
#if ENABLE_INPUT_SYSTEM
			return UnityEngine.InputSystem.Mouse.current.scroll != null ? UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y : 0.0f;
#else
			return UnityEngine.Input.mouseScrollDelta.y;
#endif
		}

		public static bool GetMouseExists()
		{
#if ENABLE_INPUT_SYSTEM
			return UnityEngine.InputSystem.Mouse.current != null;
#else
			return UnityEngine.Input.mousePresent;
#endif
		}

		public static bool GetKeyboardExists()
		{
#if ENABLE_INPUT_SYSTEM
			return UnityEngine.InputSystem.Keyboard.current != null;
#else
			return true;
#endif
		}
	}
}