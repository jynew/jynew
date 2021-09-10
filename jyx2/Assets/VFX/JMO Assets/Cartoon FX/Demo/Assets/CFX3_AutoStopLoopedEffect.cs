using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Script handling looped effect in the Demo Scene, so that they eventually stop

[RequireComponent(typeof(ParticleSystem))]
public class CFX3_AutoStopLoopedEffect : MonoBehaviour
{
	public float effectDuration = 2.5f;
	private float d;
	
	void OnEnable()
	{
		d = effectDuration;
	}
	
	void Update()
	{
		if(d > 0)
		{
			d -= Time.deltaTime;
			if(d <= 0)
			{
				this.GetComponent<ParticleSystem>().Stop(true);
				
				CFX3_Demo_Translate translation = this.gameObject.GetComponent<CFX3_Demo_Translate>();
				if(translation != null)
				{
					translation.enabled = false;
				}
			}
		}
	}
}
