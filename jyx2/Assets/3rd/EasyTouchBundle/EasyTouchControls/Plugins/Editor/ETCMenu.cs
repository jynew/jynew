using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using UnityEngine.EventSystems;

public static class ETCMenu{


	[MenuItem ("GameObject/EasyTouch Controls/InputManager", false, 0)]
	static void AddInputManager(){
		ETCInput.instance.Create();
	}

	[MenuItem ("GameObject/EasyTouch Controls/Joystick", false, 0)]
	static void  AddJoystick(){
	
		ETCInput.instance.Create();
		GameObject canvas = SetupUI();

		Object[] sprites = Resources.LoadAll("ETCDefaultSprite");

		GameObject joystick = new GameObject("New Joystick", typeof(ETCJoystick),typeof(RectTransform), typeof( CanvasGroup), typeof(Image) );
		joystick.transform.SetParent( canvas.transform,false);
		joystick.GetComponent<Image>().preserveAspect = true;
		joystick.GetComponent<Image>().sprite = GetSpriteByName("ETCArea",sprites);
		joystick.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,130);
		joystick.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,130);
		joystick.GetComponent<CanvasGroup>().hideFlags = HideFlags.HideInInspector;
		joystick.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;


		GameObject thumb = new GameObject("Thumb",typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		thumb.transform.SetParent(joystick.transform,false);
		thumb.GetComponent<Image>().preserveAspect = true;
		thumb.GetComponent<Image>().sprite = GetSpriteByName("ETCThumb",sprites);
		thumb.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,80);
		thumb.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,80);

		joystick.GetComponent<ETCJoystick>().thumb = thumb.transform as RectTransform;

		joystick.GetComponent<ETCJoystick>().InitCurve();


		Selection.activeGameObject = joystick;
	}

	[MenuItem ("GameObject/EasyTouch Controls/D-Pad", false, 0)]
	static void  AddDPad(){

		ETCInput.instance.Create();
		GameObject canvas = SetupUI();
		
		Object[] sprites = Resources.LoadAll("ETCDefaultSprite"); 
		
		GameObject button = new GameObject("New DPad", typeof(ETCDPad),typeof(RectTransform),typeof(Image));
		button.transform.SetParent( canvas.transform,false);
		
		button.GetComponent<Image>().preserveAspect = true;
		button.GetComponent<Image>().sprite = GetSpriteByName("ETCDPad",sprites);
		
		button.GetComponent<ETCDPad>().normalSprite = GetSpriteByName("ETCButtonNormal",sprites);
		button.GetComponent<ETCDPad>().normalColor = Color.white;
		button.GetComponent<ETCDPad>().pressedSprite = GetSpriteByName("ETCButtonPressed",sprites);
		button.GetComponent<ETCDPad>().pressedColor = Color.white;
		
		button.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
		
		Selection.activeGameObject = button;
	}

	[MenuItem ("GameObject/EasyTouch Controls/Button", false, 0)]
	static void  AddButton(){

		ETCInput.instance.Create();

		GameObject canvas = SetupUI();

		Object[] sprites = Resources.LoadAll("ETCDefaultSprite");

		GameObject button = new GameObject("New Button", typeof(ETCButton),typeof(RectTransform),typeof(Image));
		button.transform.SetParent( canvas.transform,false);

		button.GetComponent<Image>().preserveAspect = true;
		button.GetComponent<Image>().sprite = GetSpriteByName("ETCButtonNormal",sprites);

		button.GetComponent<ETCButton>().normalSprite = GetSpriteByName("ETCButtonNormal",sprites);
		button.GetComponent<ETCButton>().normalColor = Color.white;
		button.GetComponent<ETCButton>().pressedSprite = GetSpriteByName("ETCButtonPressed",sprites);
		button.GetComponent<ETCButton>().pressedColor = Color.white;

		button.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;

		Selection.activeGameObject = button;
	}

	[MenuItem ("GameObject/EasyTouch Controls/TouchPad", false, 0)]
	static void  AddTouchPad(){
		ETCInput.instance.Create();

		GameObject canvas = SetupUI();

		Object[] sprites = Resources.LoadAll("ETCDefaultSprite");

		GameObject touchPad = new GameObject("New TouchPad", typeof(ETCTouchPad),typeof(RectTransform),typeof(Image));
		touchPad.transform.SetParent( canvas.transform,false);

		touchPad.GetComponent<Image>().sprite = GetSpriteByName("ETCFrame",sprites);

		touchPad.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;

		Selection.activeGameObject = touchPad;
	}


	[MenuItem ("GameObject/EasyTouch Controls/Area", false, 0)]
	public static ETCArea AddJoystickArea(){

		GameObject canvas = SetupUI();
		
		Object[] sprites = Resources.LoadAll("ETCDefaultSprite");
		
		GameObject area = new GameObject("Joystick area", typeof(RectTransform),  typeof(ETCArea), typeof(Image));
		area.GetComponent<Image>().sprite = GetSpriteByName("ETCFrame",sprites);
		area.GetComponent<Image>().type = Image.Type.Sliced;
		area.transform.SetParent( canvas.transform,false);
		
		area.transform.SetAsFirstSibling();
		
		area.GetComponent<ETCArea>().ApplyPreset(ETCArea.AreaPreset.BottomLeft);
		return area.GetComponent<ETCArea>();
	
	}

	static GameObject SetupUI(){
		// Canvas
		GameObject canvas = GameObject.Find("EasyTouchControlsCanvas");
		if (canvas == null){
			canvas = AddCanvas();
		}
		
		// Event system
		if (GameObject.FindObjectOfType(typeof(EventSystem))==null){
			AddEventSystem();
		}

		// TouchInput
		/*
		#if !UNITY_5_3_OR_NEWER
		if ( GameObject.FindObjectOfType(typeof(TouchInputModule)) ){
			TouchInputModule tm = (TouchInputModule)GameObject.FindObjectOfType(typeof(TouchInputModule));
			//tm.allowActivationOnStandalone = true;
			tm.forceModuleActive = true;
		}
		#endif*/
		return canvas;

	}
	
	static void AddEventSystem(){

		//#if UNITY_5_3_OR_NEWER
		new GameObject("EventSystem",typeof(EventSystem), typeof(StandaloneInputModule));
		//#else
		//new GameObject("EventSystem",typeof(EventSystem), typeof(TouchInputModule), typeof(StandaloneInputModule));
		//#endif
	}

	static GameObject  AddCanvas(bool isSpaceCamera=false){

		GameObject canvas = new GameObject("EasyTouchControlsCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		if (isSpaceCamera){
			canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
			canvas.GetComponent<Canvas>().worldCamera = Camera.main;
		}
		else{
			canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		}

		canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

		return canvas;
		
	}

	static Sprite GetSpriteByName(string name, Object[] sprites){

		Sprite sprite = null;
		for (int i=0;i<sprites.Length;i++){
			if (sprites[i].name == name){
				sprite = (Sprite)sprites[i];
			}
		}

		return sprite;
	}
}

