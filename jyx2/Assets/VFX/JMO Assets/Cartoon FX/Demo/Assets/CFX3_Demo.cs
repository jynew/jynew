using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Script handling the Demo scene of CartoonFX 3 particles.

public class CFX3_Demo : MonoBehaviour
{
	public bool orderedSpawns = true;
	public float step = 1.0f;
	public float range = 5.0f;
	private float order = -5.0f;
	
	public Renderer groundRenderer;
	public Collider groundCollider;
	
	private GameObject[] ParticleExamples;
	private int exampleIndex;
	private string randomSpawnsDelay = "0.5";
	private bool randomSpawns;
	
	private bool slowMo;
	
	private List<GameObject> onScreenParticles = new List<GameObject>();
	
	void Awake()
	{
		List<GameObject> particleExampleList = new List<GameObject>();
		int nbChild = this.transform.childCount;
		for(int i = 0; i < nbChild; i++)
		{
			GameObject child = this.transform.GetChild(i).gameObject;
			particleExampleList.Add(child);
		}
		ParticleExamples = particleExampleList.ToArray();
		
		StartCoroutine("CheckForDeletedParticles");
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			prevParticle();
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextParticle();
		}
		else if(Input.GetKeyDown(KeyCode.Delete))
		{
			destroyParticles();
		}
		
		if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			RaycastHit hit = new RaycastHit();
			if(groundCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 9999f))
			{
				GameObject particle = spawnParticle();
				particle.transform.position = hit.point + particle.transform.position;
			}
		}
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(5,20,Screen.width-10,60));
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Effect", GUILayout.Width(50));
		if(GUILayout.Button("<", GUILayout.Width(25)))
		{
			prevParticle();
		}
		GUILayout.Label(ParticleExamples[exampleIndex].name, GUILayout.Width(265));
		if(GUILayout.Button(">", GUILayout.Width(25)))
		{
			nextParticle();
		}
		
		GUILayout.Space(80);
		
		if(GUILayout.Button(CFX_Demo_RotateCamera.rotating ? "Pause Camera" : "Rotate Camera"))
		{
			CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;
		}
		
		if(GUILayout.Button(randomSpawns ? "Stop Random Spawns" : "Start Random Spawns", GUILayout.Width(140)))
		{
			randomSpawns = !randomSpawns;
			if(randomSpawns)	StartCoroutine("RandomSpawnsCoroutine");
			else 				StopCoroutine("RandomSpawnsCoroutine");
		}
		
		randomSpawnsDelay = GUILayout.TextField(randomSpawnsDelay, 10, GUILayout.Width(42));
		randomSpawnsDelay = Regex.Replace(randomSpawnsDelay, @"[^0-9.]", "");
		
		if(GUILayout.Button(groundRenderer.enabled ? "Hide Ground" : "Show Ground", GUILayout.Width(90)))
		{
			groundRenderer.enabled = !groundRenderer.enabled;
		}
		
		if(GUILayout.Button(slowMo ? "Normal Speed" : "Slow Motion", GUILayout.Width(100)))
		{
			slowMo = !slowMo;
			if(slowMo)	Time.timeScale = 0.33f;
			else  		Time.timeScale = 1.0f;
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		//TEXT
		GUILayout.BeginArea(new Rect(5,50,Screen.width-10,60));
		GUILayout.BeginHorizontal();
		
		GUILayout.Label("Click on the ground to spawn selected particles");
		GUILayout.FlexibleSpace();
		GUILayout.Label("Use the LEFT and RIGHT keys to switch effects; Press DEL to delete all effects on screen");
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private GameObject spawnParticle()
	{
		GameObject particles = (GameObject)Instantiate(ParticleExamples[exampleIndex]);
		particles.transform.position = new Vector3(0,particles.transform.position.y,0);
		#if UNITY_3_5
			particles.SetActiveRecursively(true);
		#else
			particles.SetActive(true);
			for(int i = 0; i < particles.transform.childCount; i++)
				particles.transform.GetChild(i).gameObject.SetActive(true);
		#endif
		
		ParticleSystem ps = particles.GetComponent<ParticleSystem>();
		if(ps != null && ps.loop)
		{
			ps.gameObject.AddComponent<CFX3_AutoStopLoopedEffect>();
			ps.gameObject.AddComponent<CFX_AutoDestructShuriken>();
		}
		
		onScreenParticles.Add(particles);
		
		return particles;
	}
	
	IEnumerator CheckForDeletedParticles()
	{
		while(true)
		{
			yield return new WaitForSeconds(5.0f);
			for(int i = onScreenParticles.Count - 1; i >= 0; i--)
			{
				if(onScreenParticles[i] == null)
				{
					onScreenParticles.RemoveAt(i);
				}
			}
		}
	}
	
	IEnumerator RandomSpawnsCoroutine()
	{
		
	LOOP:
		GameObject particles = spawnParticle();
		
		if(orderedSpawns)
		{
			particles.transform.position = this.transform.position + new Vector3(order,particles.transform.position.y,0);
			order -= step;
			if(order < -range) order = range;
		}
		else 				particles.transform.position = this.transform.position + new Vector3(Random.Range(-range,range),0,Random.Range(-range,range)) + new Vector3(0,particles.transform.position.y,0);
		
		yield return new WaitForSeconds(float.Parse(randomSpawnsDelay));
		
		goto LOOP;
	}
	
	private void prevParticle()
	{
		exampleIndex--;
		if(exampleIndex < 0) exampleIndex = ParticleExamples.Length - 1;
	}
	private void nextParticle()
	{
		exampleIndex++;
		if(exampleIndex >= ParticleExamples.Length) exampleIndex = 0;
	}
	
	private void destroyParticles()
	{
		for(int i = onScreenParticles.Count - 1; i >= 0; i--)
		{
			if(onScreenParticles[i] != null)
			{
				GameObject.Destroy(onScreenParticles[i]);
			}
			
			onScreenParticles.RemoveAt(i);
		}
	}
}
