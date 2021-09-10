// HTExplosion v1.0 (July 2012)
// HTExplosion.cs library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com

using UnityEngine;
using System.Collections;

/// <summary>
/// HThis allows the creation of a particle and play an animated sprite from spritesheet.
/// </summary>
public class HTExplosion : MonoBehaviour {
	
	#region enumeration
	/// <summary>
	/// The rendering mode for particles..
	/// </summary>
	public enum CameraFacingMode{ 
		/// <summary>
		/// Render the particles as billboards facing the camera with tag "MainCamera". (Default)
		/// </summary>
		BillBoard, 
		/// <summary>
		/// Render the particles as billboards always facing up along the y-Axis.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Render the particles as billboards always facing up along the X-Axis.
		/// </summary>
		Vertical,
		/// <summary>
		/// The particle never facinc up the camera.
		/// </summary>
		Never 
	};
	#endregion
	
	#region public properties
	
	/// <summary>
	/// The sprite sheet material.
	/// </summary>
	public Material spriteSheetMaterial;	
	/// <summary>
	/// The number of sprtie on the spritesheet.
	/// </summary>
	public int spriteCount;
	/// <summary>
	/// The uv animation tile x.
	/// </summary>
	public int uvAnimationTileX;
	/// <summary>
	/// The uv animation tile y.
	/// </summary>
	public int uvAnimationTileY;
	/// <summary>
	/// The number of images per second to play animation
	/// </summary>
	public int framesPerSecond;
	/// <summary>
	/// The initial size of the explosion
	/// </summary>
	public Vector3 size = new Vector3(1,1,1);
	/// <summary>
	/// The speed growing.
	/// </summary>
	public float speedGrowing;
	/// <summary>
	/// Applied a rondom rotation on z-Axis.
	/// </summary>
	public bool randomRotation;
	/// <summary>
	/// The is one shot animation.
	/// </summary>
	public bool isOneShot=true;
	/// <summary>
	/// The billboarding mode
	/// </summary>
	public CameraFacingMode billboarding;  // Bilboardin mode
	/// <summary>
	/// The add light effect.
	/// </summary>
	public bool addLightEffect=false;
	/// <summary>
	/// The light range.
	/// </summary>
	public float lightRange;
	/// <summary>
	/// The color of the light.
	/// </summary>
	public Color lightColor;
	/// <summary>
	/// The light fade speed.
	/// </summary>
	public float lightFadeSpeed=1;
	
	#endregion
	
	#region private properties
	/// <summary>
	/// The material with the sprite speed.
	/// </summary>
	private Material mat;
	/// <summary>
	/// The mesh.
	/// </summary>
	private Mesh mesh;
	/// <summary>
	/// The mesh render.
	/// </summary>
	private MeshRenderer meshRender;
	/// <summary>
	/// The audio source.
	/// </summary>
	private AudioSource soundEffect;
	/// <summary>
	/// The start time of the explosion
	/// </summary>
	private float startTime;
	/// <summary>
	/// The main camera.
	/// </summary>
	private Camera mainCam;
	/// <summary>
	/// The effect end.
	/// </summary>
	private bool effectEnd=false;
	/// <summary>
	/// The random Z angle.
	/// </summary>
	private float randomZAngle;
	
	#endregion
	
	#region MonoBehaviour methods
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake(){
		
		// Creation of the particle
		CreateParticle();
		
		// We search the main camera
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		
		// do we have sound effect ?
		soundEffect = GetComponent("AudioSource") as AudioSource;
		
			// Add light
		if (addLightEffect){
			gameObject.AddComponent<Light>();
			gameObject.GetComponent<Light>().color = lightColor;
			gameObject.GetComponent<Light>().range = lightRange;
		}
		
		GetComponent<Renderer>().enabled = false;
	}
	
	// Use this for initialization
	void Start () {
	
		startTime = Time.time;
		transform.localScale = size;
		
		if (randomRotation){
			randomZAngle = Random.Range(-180.0f,180.0f);
		}
		else{
			randomZAngle = 0;
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		
		bool end=false;
		
		Camera_BillboardingMode();
		
		// Calculate index
    	float index = (Time.time-startTime) * framesPerSecond;
		
		if ((index<=spriteCount || !isOneShot) && !effectEnd ){
		     // repeat when exhausting all frames
		    index = index % (uvAnimationTileX * uvAnimationTileY);
		   		
			if (index== spriteCount){
				startTime = Time.time;	
				index=0;
			}
			
		    // Size of every tile
		    Vector2 size = new Vector2 (1.0f / uvAnimationTileX, 1.0f / uvAnimationTileY);
		   
		    // split into horizontal and vertical index
		    float uIndex = Mathf.Floor(index % uvAnimationTileX);
		    float vIndex = Mathf.Floor(index / uvAnimationTileX);
		
		    // build offset
		    Vector2 offset = new Vector2 (uIndex * size.x , 1.0f - size.y - vIndex * size.y);
			
		   	GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
		   	GetComponent<Renderer>().material.SetTextureScale ("_MainTex", size);
		    
		    // growing
		    transform.localScale += new Vector3(speedGrowing,speedGrowing,speedGrowing) * Time.deltaTime ;
		    GetComponent<Renderer>().enabled = true;
		}			
		else{
	 		effectEnd = true;
			GetComponent<Renderer>().enabled = false;
			end = true;		

			if (soundEffect){
				if (soundEffect.isPlaying){
					end = false;
				}
			}
		
			if (addLightEffect && end){
				if (gameObject.GetComponent<Light>().intensity>0){
					end = false;
				}
			}
			
			if (end){
				Destroy(gameObject);	
 			}
		}
		
		// Light effect
	 	if (addLightEffect && lightFadeSpeed!=0){
			gameObject.GetComponent<Light>().intensity -= lightFadeSpeed*Time.deltaTime;
		}
	}
	
	#endregion
	
	#region private methods
	
	/// <summary>
	/// Creates the particle.
	/// </summary>
	void CreateParticle(){
		
		mesh = gameObject.AddComponent<MeshFilter>().mesh; 
		meshRender = gameObject.AddComponent<MeshRenderer>(); 		
		
		mesh.vertices = new Vector3[] { new Vector3(-0.5f,-0.5f,0f),new Vector3(-0.5f,0.5f,0f), new Vector3(0.5f,0.5f,0f), new Vector3(0.5f,-0.5f,0f) };
		mesh.triangles = new int[] {0,1,2, 2,3,0 };
		mesh.uv = new Vector2[] { new Vector2 (0f, 0f), new Vector2 (0f, 1f), new Vector2 (1f, 1f), new Vector2(1f,0f)};

		meshRender.castShadows = false;
		meshRender.receiveShadows = false;
		mesh.RecalculateNormals();		
		
		GetComponent<Renderer>().material= spriteSheetMaterial;
	}
	
	/// <summary>
	/// Camera_s the billboarding mode.
	/// </summary>
	void Camera_BillboardingMode(){
		
		Vector3 lookAtVector = mainCam.transform.position - transform.position;
         
		switch (billboarding){
			case CameraFacingMode.BillBoard:
				transform.LookAt( lookAtVector); 
				break;
			case CameraFacingMode.Horizontal:
				lookAtVector.x = lookAtVector.z =0 ;
				transform.LookAt(mainCam.transform.position- lookAtVector);
				break;
			case CameraFacingMode.Vertical:
				lookAtVector.y=lookAtVector.z =0;
				transform.LookAt(mainCam.transform.position- lookAtVector);
				break;
		}
		transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,randomZAngle);
	}
	
	#endregion
	
}
