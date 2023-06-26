using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Obsolete("使用Rewired代替，参加Jyx2_Input.cs")]
public class GamepadHelper
{
	public const string CONFIRM_BUTTON = "JFire2";
	public const string CANCEL_BUTTON = "JFire3";
	public const string ACTION_BUTTON = "JFire1";
	public const string JUMP_BUTTON = "JJump";
	public const string DPAD_X_AXIS = "DPadX";
	public const string DPAD_Y_AXIS = "DPadY";
	public const string ANALOG_LEFT_X_AXIS = "Horizontal";
	public const string ANALOG_LEFT_Y_AXIS = "Vertical";
	public const string ANALOG_RIGHT_X_AXIS = "JHorizontalR";
	public const string ANALOG_RIGHT_Y_AXIS = "JVerticalR";
	public const string TAB_L1 = "JL1";
	public const string TAB_R1 = "JR1";
	public const string START_BUTTON = "JOptions";
	public const string PAD_BUTTON = "PadPress";

	private const int PollGamepadPeriod = 5;
	private static DateTime _lastPollTime = DateTime.MinValue;
	private static bool _gamepadConnected = false;

	public static bool GamepadConnected
	{
		get
		{
			if (_lastPollTime.AddSeconds(PollGamepadPeriod) < DateTime.Now)
			{
				_gamepadConnected = Input.GetJoystickNames()
					.Where(n => !string.IsNullOrWhiteSpace(n))
					.Count() > 0;
			}

			return _gamepadConnected;
		}
	}


	public static bool IsButtonPressed(string buttonName)
	{
		return Input.GetButtonDown(buttonName);
	}

	public static bool IsConfirm()
	{
		return IsButtonPressed(CONFIRM_BUTTON);
	}

	public static bool IsCancel()
	{
		return IsButtonPressed(CANCEL_BUTTON);
	}

	public static bool IsAction()
	{
		return IsButtonPressed(ACTION_BUTTON);
	}

	public static bool IsJump()
	{
		return IsButtonPressed(JUMP_BUTTON);
	}

	public static bool IsStart()
	{
		return IsButtonPressed(START_BUTTON);
	}

	public static bool IsPadPress()
	{
		return IsButtonPressed(PAD_BUTTON);
	}

	public static bool IsTabLeft()
	{
		return IsButtonPressed(TAB_L1);
	}

	public static bool IsTabRight()
	{
		return IsButtonPressed(TAB_R1);
	}

	public static bool IsDadXMove(bool right)
	{
		var input = Input.GetAxis(DPAD_X_AXIS);
		return (right) ? input == 1 : input == -1;
	}

	public static bool IsDadYMove(bool down)
	{
		var input = Input.GetAxis(DPAD_Y_AXIS);
		return (down) ? input == -1 : input == 1;
	}

	public static AnalogAxisMove GetLeftAnalogMove()
	{
		return new AnalogAxisMove
		{
			X = Input.GetAxis(ANALOG_LEFT_X_AXIS),
			Y = Input.GetAxis(ANALOG_LEFT_Y_AXIS)
		};
	}

	public static AnalogAxisMove GetRightAnalogMove()
	{
		return new AnalogAxisMove
		{
			X = Input.GetAxis(ANALOG_RIGHT_X_AXIS),
			Y = Input.GetAxis(ANALOG_RIGHT_Y_AXIS)
		};
	}
}

public class AnalogAxisMove
{
	public float X;
	public float Y;
}

