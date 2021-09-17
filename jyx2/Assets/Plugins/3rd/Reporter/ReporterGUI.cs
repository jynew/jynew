using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
}
