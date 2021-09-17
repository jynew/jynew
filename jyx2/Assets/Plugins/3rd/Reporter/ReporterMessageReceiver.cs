using UnityEngine;
using System.Collections;

public class ReporterMessageReceiver : MonoBehaviour
{
	Reporter reporter;
	void Start()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnPreStart()
	{
		//To Do : this method is called before initializing reporter, 
		//we can for example check the resultion of our device ,then change the size of reporter
		if (reporter == null)
			reporter = gameObject.GetComponent<Reporter>();

		if (Screen.width < 1000)
			reporter.size = new Vector2(32, 32);
		else
			reporter.size = new Vector2(48, 48);

		reporter.UserData = "Put user date here like his account to know which user is playing on this device";
	}

	void OnHideReporter()
	{
		//TO DO : resume your game
	}

	void OnShowReporter()
	{
		//TO DO : pause your game and disable its GUI
	}

	void OnLog(Reporter.Log log)
	{
		//TO DO : put you custom code 
	}

}
