using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

// Cartoon FX Easy Editor - (c) 2013, Jean Moreno

public class CFXEasyEditor : EditorWindow
{
	static private CFXEasyEditor SingleWindow;
	
	[MenuItem("Window/CartoonFX Easy Editor")]
	static void ShowWindow()
	{
		CFXEasyEditor window = EditorWindow.GetWindow<CFXEasyEditor>(EditorPrefs.GetBool("CFX_ShowAsToolbox", true), "CartoonFX Easy Editor", true);
		window.minSize = new Vector2(300, 8);
		window.maxSize = new Vector2(300, 8);
		window.foldoutChanged = true;
	}
	
	//Change Start Color
	private bool AffectAlpha = true;
	private Color ColorValue = Color.white;
	private Color ColorValue2 = Color.white;
	
	//Scale
	private float ScalingValue = 2.0f;
	private float LTScalingValue = 100.0f;
	
	//Delay
	private float DelayValue = 1.0f;
	
	//Duration
	private float DurationValue = 5.0f;
	
	//Tint
	private bool TintStartColor = true;
	private bool TintColorModule = true;
	private bool TintColorSpeedModule = true;
	private Color TintColorValue = Color.white;
	
	//Change Lightness
	private int LightnessStep = 10;
	
	//Module copying
	private ParticleSystem sourceObject;
	private Color ColorSelected = new Color(0.8f,0.95f,1.0f,1.0f);
	private bool[] b_modules = new bool[16];
	
	//Foldouts
	bool basicFoldout = false;
	bool colorFoldout = false;
	bool copyFoldout = false;
	bool foldoutChanged;
	
	//Editor Prefs
	private bool pref_ShowAsToolbox;
	private bool pref_IncludeChildren;
	
	void OnEnable()
	{
		//Load Settings
		pref_ShowAsToolbox = EditorPrefs.GetBool("CFX_ShowAsToolbox", true);
		pref_IncludeChildren = EditorPrefs.GetBool("CFX_IncludeChildren", true);
		basicFoldout = EditorPrefs.GetBool("CFX_BasicFoldout", false);
		colorFoldout = EditorPrefs.GetBool("CFX_ColorFoldout", false);
		copyFoldout = EditorPrefs.GetBool("CFX_CopyFoldout", false);
	}
	
	void OnDisable()
	{
		//Save Settings
		EditorPrefs.SetBool("CFX_BasicFoldout", basicFoldout);
		EditorPrefs.SetBool("CFX_ColorFoldout", colorFoldout);
		EditorPrefs.SetBool("CFX_CopyFoldout", copyFoldout);
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0,0,this.position.width - 8,this.position.height));
		GUILayout.Space(4);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("CARTOON FX Easy Editor", EditorStyles.boldLabel);
		pref_ShowAsToolbox = GUILayout.Toggle(pref_ShowAsToolbox, new GUIContent("Toolbox", "If enabled, the window will be displayed as an external toolbox.\nIf false, it will act as a dockable Unity window."), GUILayout.Width(60));
		if(GUI.changed)
		{
			EditorPrefs.SetBool("CFX_ShowAsToolbox", pref_ShowAsToolbox);
			this.Close();
			CFXEasyEditor.ShowWindow();
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Easily change properties of any Particle System!", EditorStyles.miniLabel);
		
	//----------------------------------------------------------------
		
		pref_IncludeChildren = GUILayout.Toggle(pref_IncludeChildren, new GUIContent("Include Children", "If checked, changes will affect every Particle Systems from each child of the selected GameObject(s)"));
		if(GUI.changed)
		{
			EditorPrefs.SetBool("CFX_IncludeChildren", pref_IncludeChildren);
		}
		
		EditorGUILayout.BeginHorizontal();
		
		GUILayout.Label("Test effect(s):");
		
		if(GUILayout.Button("Play", EditorStyles.miniButtonLeft, GUILayout.Width(50f)))
		{
			foreach(GameObject go in Selection.gameObjects)
			{
				ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
				if(systems.Length == 0) continue;
				foreach(ParticleSystem system in systems)
					system.Play(pref_IncludeChildren);
			}
		}
		if(GUILayout.Button("Pause", EditorStyles.miniButtonMid, GUILayout.Width(50f)))
		{
			foreach(GameObject go in Selection.gameObjects)
			{
				ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
				if(systems.Length == 0) continue;
				foreach(ParticleSystem system in systems)
					system.Pause(pref_IncludeChildren);
			}
		}
		if(GUILayout.Button("Stop", EditorStyles.miniButtonMid, GUILayout.Width(50f)))
		{
			foreach(GameObject go in Selection.gameObjects)
			{
				ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
				if(systems.Length == 0) continue;
				foreach(ParticleSystem system in systems)
					system.Stop(pref_IncludeChildren);
			}
		}
		if(GUILayout.Button("Clear", EditorStyles.miniButtonRight, GUILayout.Width(50f)))
		{
			foreach(GameObject go in Selection.gameObjects)
			{
				ParticleSystem[] systems = go.GetComponents<ParticleSystem>();
				if(systems.Length == 0) continue;
				foreach(ParticleSystem system in systems)
				{
					system.Stop(pref_IncludeChildren);
					system.Clear(pref_IncludeChildren);
				}
			}
		}
		
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
	//----------------------------------------------------------------
		
		//Separator
		GUILayout.Box("",GUILayout.Width(this.position.width - 12), GUILayout.Height(3));
		
		EditorGUI.BeginChangeCheck();
		basicFoldout = EditorGUILayout.Foldout(basicFoldout, "QUICK EDIT");
		if(EditorGUI.EndChangeCheck())
		{
			foldoutChanged = true;
		}
		if(basicFoldout)
		{
		
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Scale Size", "Changes the size of the Particle System(s) and other values accordingly (speed, gravity, etc.)"), GUILayout.Width(120)))
			{
				applyScale();
			}
			GUILayout.Label("Multiplier:",GUILayout.Width(110));
			ScalingValue = EditorGUILayout.FloatField(ScalingValue,GUILayout.Width(50));
			if(ScalingValue <= 0) ScalingValue = 0.1f;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Speed", "Changes the speed of the Particle System(s) (if you want quicker or longer effects, 100% = default speed)"), GUILayout.Width(120)))
			{
				applySpeed();
			}
			GUILayout.Label("Speed (%):",GUILayout.Width(110));
			LTScalingValue = EditorGUILayout.FloatField(LTScalingValue,GUILayout.Width(50));
			if(LTScalingValue < 0.1f) LTScalingValue = 0.1f;
			else if(LTScalingValue > 9999) LTScalingValue = 9999;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Duration", "Changes the duration of the Particle System(s)"), GUILayout.Width(120)))
			{
				applyDuration();
			}
			GUILayout.Label("Duration (sec):",GUILayout.Width(110));
			DurationValue = EditorGUILayout.FloatField(DurationValue,GUILayout.Width(50));
			if(DurationValue < 0.1f) DurationValue = 0.1f;
			else if(DurationValue > 9999) DurationValue = 9999;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Delay", "Changes the delay of the Particle System(s)"), GUILayout.Width(120)))
			{
				applyDelay();
			}
			GUILayout.Label("Delay :",GUILayout.Width(110));
			DelayValue = EditorGUILayout.FloatField(DelayValue,GUILayout.Width(50));
			if(DelayValue < 0.0f) DelayValue = 0.0f;
			else if(DelayValue > 9999f) DelayValue = 9999f;
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.Space(2);
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Loop", "Loop the effect (might not work properly on some effects such as explosions)"), EditorStyles.miniButtonLeft))
			{
				loopEffect(true);
			}
			if(GUILayout.Button(new GUIContent("Unloop", "Remove looping from the effect"), EditorStyles.miniButtonRight))
			{
				loopEffect(false);
			}
			if(GUILayout.Button(new GUIContent("Prewarm On", "Prewarm the effect (if looped)"), EditorStyles.miniButtonLeft))
			{
				prewarmEffect(true);
			}
			if(GUILayout.Button(new GUIContent("Prewarm Off", "Don't prewarm the effect (if looped)"), EditorStyles.miniButtonRight))
			{
				prewarmEffect(false);
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(2);
		
	//----------------------------------------------------------------
		
		}
		
		//Separator
		GUILayout.Box("",GUILayout.Width(this.position.width - 12), GUILayout.Height(3));
		
		EditorGUI.BeginChangeCheck();
		colorFoldout = EditorGUILayout.Foldout(colorFoldout, "COLOR EDIT");
		if(EditorGUI.EndChangeCheck())
		{
			foldoutChanged = true;
		}
		if(colorFoldout)
		{
		
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Set Start Color(s)", "Changes the color(s) of the Particle System(s)\nSecond Color is used when Start Color is 'Random Between Two Colors'."),GUILayout.Width(120)))
			{
				applyColor();
			}
			ColorValue = EditorGUILayout.ColorField(ColorValue);
			ColorValue2 = EditorGUILayout.ColorField(ColorValue2);
			AffectAlpha = GUILayout.Toggle(AffectAlpha, new GUIContent("Alpha", "If checked, the alpha value will also be changed"));
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(new GUIContent("Tint Colors", "Tints the colors of the Particle System(s), including gradients!\n(preserving their saturation and lightness)"),GUILayout.Width(120)))
			{
				tintColor();
			}
			TintColorValue = EditorGUILayout.ColorField(TintColorValue);
			TintColorValue = HSLColor.FromRGBA(TintColorValue).VividColor();
			GUILayout.EndHorizontal();
			
		//----------------------------------------------------------------
			
			/*
			GUILayout.BeginHorizontal();
			GUILayout.Label("Add/Substract Lightness:");
			
			LightnessStep = EditorGUILayout.IntField(LightnessStep, GUILayout.Width(30));
			if(LightnessStep > 99) LightnessStep = 99;
			else if(LightnessStep < 1) LightnessStep = 1;
			GUILayout.Label("%");
			
			if(GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(22)))
			{
				addLightness(true);
			}
			if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(22)))
			{
				addLightness(false);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			*/
			
		//----------------------------------------------------------------
			
			GUILayout.Label("Color Modules to affect:");
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			GUI.color = TintStartColor ? ColorSelected : Color.white; if(GUILayout.Button(new GUIContent("Start Color", "If checked, the \"Start Color\" value(s) will be affected."), EditorStyles.toolbarButton, GUILayout.Width(70))) TintStartColor = !TintStartColor;
			GUI.color = TintColorModule ? ColorSelected : Color.white; if(GUILayout.Button(new GUIContent("Color over Lifetime", "If checked, the \"Color over Lifetime\" value(s) will be affected."), EditorStyles.toolbarButton, GUILayout.Width(110))) TintColorModule = !TintColorModule;
			GUI.color = TintColorSpeedModule ? ColorSelected : Color.white; if(GUILayout.Button(new GUIContent("Color by Speed", "If checked, the \"Color by Speed\" value(s) will be affected."), EditorStyles.toolbarButton, GUILayout.Width(100))) TintColorSpeedModule = !TintColorSpeedModule;
			GUI.color = Color.white;
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(4);
			
		//----------------------------------------------------------------
			
		}
		
		//Separator
		GUILayout.Box("",GUILayout.Width(this.position.width - 12), GUILayout.Height(3));
//		GUILayout.Space(6);
		
	//----------------------------------------------------------------
		
		EditorGUI.BeginChangeCheck();
		copyFoldout = EditorGUILayout.Foldout(copyFoldout, "COPY MODULES");
		if(EditorGUI.EndChangeCheck())
		{
			foldoutChanged = true;
		}
		if(copyFoldout)
		{
		
			GUILayout.Label("Copy properties from a Particle System to others!", EditorStyles.miniLabel);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Source Object:", GUILayout.Width(110));
			sourceObject = (ParticleSystem)EditorGUILayout.ObjectField(sourceObject, typeof(ParticleSystem), true);
			GUILayout.EndHorizontal();
			
			EditorGUILayout.LabelField("Modules to Copy:");
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("ALL", EditorStyles.miniButtonLeft, GUILayout.Width(120)))
			{
				for(int i = 0; i < b_modules.Length; i++) b_modules[i] = true;
			}
			if(GUILayout.Button("NONE", EditorStyles.miniButtonRight, GUILayout.Width(120)))
			{
				for(int i = 0; i < b_modules.Length; i++) b_modules[i] = false;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(4);
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[0] ? ColorSelected : Color.white;	if(GUILayout.Button("Initial", EditorStyles.toolbarButton, GUILayout.Width(70))) b_modules[0] = !b_modules[0];
			GUI.color = b_modules[1] ? ColorSelected : Color.white;	if(GUILayout.Button("Emission", EditorStyles.toolbarButton, GUILayout.Width(70))) b_modules[1] = !b_modules[1];
			GUI.color = b_modules[2] ? ColorSelected : Color.white;	if(GUILayout.Button("Shape", EditorStyles.toolbarButton, GUILayout.Width(70))) b_modules[2] = !b_modules[2];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[3] ? ColorSelected : Color.white;	if(GUILayout.Button("Velocity", EditorStyles.toolbarButton, GUILayout.Width(70))) b_modules[3] = !b_modules[3];
			GUI.color = b_modules[4] ? ColorSelected : Color.white;	if(GUILayout.Button("Limit Velocity", EditorStyles.toolbarButton, GUILayout.Width(100))) b_modules[4] = !b_modules[4];
			GUI.color = b_modules[5] ? ColorSelected : Color.white;	if(GUILayout.Button("Force", EditorStyles.toolbarButton, GUILayout.Width(70))) b_modules[5] = !b_modules[5];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[6] ? ColorSelected : Color.white;	if(GUILayout.Button("Color over Lifetime", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[6] = !b_modules[6];
			GUI.color = b_modules[7] ? ColorSelected : Color.white;	if(GUILayout.Button("Color by Speed", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[7] = !b_modules[7];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[8] ? ColorSelected : Color.white;	if(GUILayout.Button("Size over Lifetime", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[8] = !b_modules[8];
			GUI.color = b_modules[9] ? ColorSelected : Color.white;	if(GUILayout.Button("Size by Speed", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[9] = !b_modules[9];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[10] ? ColorSelected : Color.white;	if(GUILayout.Button("Rotation over Lifetime", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[10] = !b_modules[10];
			GUI.color = b_modules[11] ? ColorSelected : Color.white;	if(GUILayout.Button("Rotation by Speed", EditorStyles.toolbarButton, GUILayout.Width(120))) b_modules[11] = !b_modules[11];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[12] ? ColorSelected : Color.white;	if(GUILayout.Button("Collision", EditorStyles.toolbarButton, GUILayout.Width(100))) b_modules[12] = !b_modules[12];
			GUI.color = b_modules[13] ? ColorSelected : Color.white;	if(GUILayout.Button("Sub Emitters", EditorStyles.toolbarButton, GUILayout.Width(100))) b_modules[13] = !b_modules[13];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.color = b_modules[14] ? ColorSelected : Color.white;	if(GUILayout.Button("Texture Animation", EditorStyles.toolbarButton, GUILayout.Width(110))) b_modules[14] = !b_modules[14];
			GUI.color = b_modules[15] ? ColorSelected : Color.white;	if(GUILayout.Button("Renderer", EditorStyles.toolbarButton, GUILayout.Width(90))) b_modules[15] = !b_modules[15];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUI.color = Color.white;
			
			GUILayout.Space(4);
			if(GUILayout.Button("Copy properties to selected Object(s)"))
			{
				bool foundPs = false;
				foreach(GameObject go in Selection.gameObjects)
				{
					ParticleSystem[] systems;
					if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
					else 					systems = go.GetComponents<ParticleSystem>();
					
					if(systems.Length == 0) continue;
					
					foundPs = true;
					foreach(ParticleSystem system in systems)	CopyModules(sourceObject, system);
				}
				
				if(!foundPs)
				{
					Debug.LogWarning("CartoonFX Easy Editor: No Particle System found in the selected GameObject(s)!");
				}
			}
		}
			
		//----------------------------------------------------------------
		
		GUILayout.Space(8);
		
		//Resize window
		if(foldoutChanged && Event.current.type == EventType.Repaint)
		{
			foldoutChanged = false;
			
			Rect r = GUILayoutUtility.GetLastRect();
			this.minSize = new Vector2(300,r.y + 8);
			this.maxSize = new Vector2(300,r.y + 8);
		}
		
		GUILayout.EndArea();
	}
	
	//Loop effects
	private void loopEffect(bool setLoop)
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("looping").boolValue = setLoop;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Prewarm effects
	private void prewarmEffect(bool setPrewarm)
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("prewarm").boolValue = setPrewarm;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Scale Size
	private void applyScale()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				ScaleParticleValues(ps, go);
			}
			
			//Scale Lights' range
			Light[] lights = go.GetComponentsInChildren<Light>();
			foreach(Light light in lights)
			{
				light.range *= ScalingValue;
				light.transform.localPosition *= ScalingValue;
			}
		}
	}
	
	//Change Color
	private void applyColor()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);
				if(!AffectAlpha)
				{
					psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue = new Color(ColorValue.r, ColorValue.g, ColorValue.b, psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue.a);
					psSerial.FindProperty("InitialModule.startColor.minColor").colorValue = new Color(ColorValue2.r, ColorValue2.g, ColorValue2.b, psSerial.FindProperty("InitialModule.startColor.minColor").colorValue.a);
				}
				else
				{
					psSerial.FindProperty("InitialModule.startColor.maxColor").colorValue = ColorValue;
					psSerial.FindProperty("InitialModule.startColor.minColor").colorValue = ColorValue2;
				}
				psSerial.ApplyModifiedProperties();
			}
		}
	}
	
	//TINT COLORS ================================================================================================================================
	
	private void tintColor()
	{
		if(!TintStartColor && !TintColorModule && !TintColorSpeedModule)
		{
			Debug.LogWarning("CartoonFX Easy Editor: You must toggle at least one of the three Color Modules to be able to tint anything!");
			return;
		}
		
		float hue = HSLColor.FromRGBA(TintColorValue).h;
		
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);
				
				if(TintStartColor)
					GenericTintColorProperty(psSerial.FindProperty("InitialModule.startColor"), hue);
				
				if(TintColorModule)
					GenericTintColorProperty(psSerial.FindProperty("ColorModule.gradient"), hue);
				
				if(TintColorSpeedModule)
					GenericTintColorProperty(psSerial.FindProperty("ColorBySpeedModule.gradient"), hue);
				
				psSerial.ApplyModifiedProperties();
			}
		}
	}
	
	private void GenericTintColorProperty(SerializedProperty colorProperty, float hue)
	{
		int state = colorProperty.FindPropertyRelative("minMaxState").intValue;
		switch(state)
		{
			//Constant Color
		case 0:
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithHue(hue);
			break;
			
			//Gradient
		case 1:
			TintGradient(colorProperty.FindPropertyRelative("maxGradient"), hue);
			break;
			
			//Random between 2 Colors
		case 2:
			colorProperty.FindPropertyRelative("minColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("minColor").colorValue).ColorWithHue(hue);
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithHue(hue);
			break;
			
			//Random between 2 Gradients
		case 3:
			TintGradient(colorProperty.FindPropertyRelative("maxGradient"), hue);
			TintGradient(colorProperty.FindPropertyRelative("minGradient"), hue);
			break;
		}
	}
	
	private void TintGradient(SerializedProperty gradientProperty, float hue)
	{
		gradientProperty.FindPropertyRelative("key0").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key0").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key1").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key1").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key2").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key2").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key3").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key3").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key4").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key4").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key5").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key5").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key6").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key6").colorValue).ColorWithHue(hue);
		gradientProperty.FindPropertyRelative("key7").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key7").colorValue).ColorWithHue(hue);
	}
	
	//LIGHTNESS OFFSET ================================================================================================================================
	
	private void addLightness(bool substract)
	{
		if(!TintStartColor && !TintColorModule && !TintColorSpeedModule)
		{
			Debug.LogWarning("CartoonFX Easy Editor: You must toggle at least one of the three Color Modules to be able to change lightness!");
			return;
		}
		
		float lightness = (float)(LightnessStep/100f);
		if(substract) lightness *= -1f;
		
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject psSerial = new SerializedObject(ps);
				
				if(TintStartColor)
					GenericAddLightness(psSerial.FindProperty("InitialModule.startColor"), lightness);
				
				if(TintColorModule)
					GenericAddLightness(psSerial.FindProperty("ColorModule.gradient"), lightness);
				
				if(TintColorSpeedModule)
					GenericAddLightness(psSerial.FindProperty("ColorBySpeedModule.gradient"), lightness);
				
				psSerial.ApplyModifiedProperties();
				psSerial.Update();
			}
		}
	}
	
	private void GenericAddLightness(SerializedProperty colorProperty, float lightness)
	{
		int state = colorProperty.FindPropertyRelative("minMaxState").intValue;
		switch(state)
		{
			//Constant Color
		case 0:
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithLightnessOffset(lightness);
			break;
			
			//Gradient
		case 1:
			AddLightnessGradient(colorProperty.FindPropertyRelative("maxGradient"), lightness);
			break;
			
			//Random between 2 Colors
		case 2:
			colorProperty.FindPropertyRelative("minColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("minColor").colorValue).ColorWithLightnessOffset(lightness);
			colorProperty.FindPropertyRelative("maxColor").colorValue = HSLColor.FromRGBA(colorProperty.FindPropertyRelative("maxColor").colorValue).ColorWithLightnessOffset(lightness);
			break;
			
			//Random between 2 Gradients
		case 3:
			AddLightnessGradient(colorProperty.FindPropertyRelative("maxGradient"), lightness);
			AddLightnessGradient(colorProperty.FindPropertyRelative("minGradient"), lightness);
			break;
		}
	}
	
	private void AddLightnessGradient(SerializedProperty gradientProperty, float lightness)
	{
		gradientProperty.FindPropertyRelative("key0").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key0").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key1").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key1").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key2").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key2").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key3").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key3").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key4").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key4").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key5").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key5").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key6").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key6").colorValue).ColorWithLightnessOffset(lightness);
		gradientProperty.FindPropertyRelative("key7").colorValue = HSLColor.FromRGBA(gradientProperty.FindPropertyRelative("key7").colorValue).ColorWithLightnessOffset(lightness);
	}
	
	//RGB / HSL Conversions
	private struct HSLColor
	{
		public float h;
		public float s;
		public float l;
		public float a;
		
		public HSLColor(float h, float s, float l, float a)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = a;
		}
		
		public HSLColor(float h, float s, float l)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = 1f;
		}
		
		public HSLColor(Color c)
		{
			HSLColor temp = FromRGBA(c);
			h = temp.h;
			s = temp.s;
			l = temp.l;
			a = temp.a;
		}
		
		public static HSLColor FromRGBA(Color c)
		{
			float h, s, l, a;
			a = c.a;
			
			float cmin = Mathf.Min(Mathf.Min(c.r, c.g), c.b);
			float cmax = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
			
			l = (cmin + cmax) / 2f;
			
			if (cmin == cmax)
			{
				s = 0;
				h = 0;
			}
			else
			{
				float delta = cmax - cmin;
				
				s = (l <= .5f) ? (delta / (cmax + cmin)) : (delta / (2f - (cmax + cmin)));
				
				h = 0;
				
				if (c.r == cmax)
				{
					h = (c.g - c.b) / delta;
				}
				else if (c.g == cmax)
				{
					h = 2f + (c.b - c.r) / delta;
				}
				else if (c.b == cmax)
				{
					h = 4f + (c.r - c.g) / delta;
				}
				
				h = Mathf.Repeat(h * 60f, 360f);
			}
			
			return new HSLColor(h, s, l, a);
		}
		
		public Color ToRGBA()
		{
			float r, g, b, a;
			a = this.a;
			
			float m1, m2;
			
			m2 = (l <= .5f) ? (l * (1f + s)) : (l + s - l * s);
			m1 = 2f * l - m2;
			
			if (s == 0f)
			{
				r = g = b = l;
			}
			else
			{
				r = Value(m1, m2, h + 120f);
				g = Value(m1, m2, h);
				b = Value(m1, m2, h - 120f);
			}
			
			return new Color(r, g, b, a);
		}
		
		static float Value(float n1, float n2, float hue)
		{
			hue = Mathf.Repeat(hue, 360f);
			
			if (hue < 60f)
			{
				return n1 + (n2 - n1) * hue / 60f;
			}
			else if (hue < 180f)
			{
				return n2;
			}
			else if (hue < 240f)
			{
				return n1 + (n2 - n1) * (240f - hue) / 60f;
			}
			else
			{
				return n1;
			}
		}
		
		public Color VividColor()
		{
			this.l = 0.5f;
			this.s = 1.0f;
			return this.ToRGBA();
		}
		
		public Color ColorWithHue(float hue)
		{
			this.h = hue;
			return this.ToRGBA();
		}
		
		public Color ColorWithLightnessOffset(float lightness)
		{
			this.l += lightness;
			if(this.l > 1.0f) this.l = 1.0f;
			else if(this.l < 0.0f) this.l = 0.0f;
			
			return this.ToRGBA();
		}
		
		public static implicit operator HSLColor(Color src)
		{
			return FromRGBA(src);
		}
		
		public static implicit operator Color(HSLColor src)
		{
			return src.ToRGBA();
		}
	}
	
	//Scale Lifetime only
	private void applySpeed()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			//Scale Lifetime
			foreach(ParticleSystem ps in systems)
			{
				ps.playbackSpeed = (100.0f/LTScalingValue);
			}
		}
	}
	
	//Set Duration
	private void applyDuration()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			//Scale Shuriken Particles Values
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			foreach(ParticleSystem ps in systems)
			{
				SerializedObject so = new SerializedObject(ps);
				so.FindProperty("lengthInSec").floatValue = DurationValue;
				so.ApplyModifiedProperties();
			}
		}
	}
	
	//Change delay
	private void applyDelay()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			ParticleSystem[] systems;
			if(pref_IncludeChildren)		systems = go.GetComponentsInChildren<ParticleSystem>(true);
			else 					systems = go.GetComponents<ParticleSystem>();
			
			//Scale Lifetime
			foreach(ParticleSystem ps in systems)
			{
				ps.startDelay = DelayValue;
			}
		}
	}
	
	//Copy Selected Modules
	private void CopyModules(ParticleSystem source, ParticleSystem dest)
	{
		if(source == null)
		{
			Debug.LogWarning("CartoonFX Easy Editor: Select a source Particle System to copy properties from first!");
			return;
		}
		
		SerializedObject psSource = new SerializedObject(source);
		SerializedObject psDest = new SerializedObject(dest);
		
		//Inial Module
		if(b_modules[0])
		{
			psDest.FindProperty("prewarm").boolValue = psSource.FindProperty("prewarm").boolValue;
			psDest.FindProperty("lengthInSec").floatValue = psSource.FindProperty("lengthInSec").floatValue;
			psDest.FindProperty("moveWithTransform").boolValue = psSource.FindProperty("moveWithTransform").boolValue;
			
			GenericModuleCopy(psSource.FindProperty("InitialModule"), psDest.FindProperty("InitialModule"));
			
			dest.startDelay = source.startDelay;
			dest.loop = source.loop;
			dest.playOnAwake = source.playOnAwake;
			dest.playbackSpeed = source.playbackSpeed;
			dest.emissionRate = source.emissionRate;
			dest.startSpeed = source.startSpeed;
			dest.startSize = source.startSize;
			dest.startColor = source.startColor;
			dest.startRotation = source.startRotation;
			dest.startLifetime = source.startLifetime;
			dest.gravityModifier = source.gravityModifier;
		}
		
		//Emission
		if(b_modules[1])	GenericModuleCopy(psSource.FindProperty("EmissionModule"), psDest.FindProperty("EmissionModule"));
		
		//Shape
		if(b_modules[2])	GenericModuleCopy(psSource.FindProperty("ShapeModule"), psDest.FindProperty("ShapeModule"));
		
		//Velocity
		if(b_modules[3])	GenericModuleCopy(psSource.FindProperty("VelocityModule"), psDest.FindProperty("VelocityModule"));
		
		//Velocity Clamp
		if(b_modules[4])	GenericModuleCopy(psSource.FindProperty("ClampVelocityModule"), psDest.FindProperty("ClampVelocityModule"));
		
		//Force
		if(b_modules[5])	GenericModuleCopy(psSource.FindProperty("ForceModule"), psDest.FindProperty("ForceModule"));
		
		//Color
		if(b_modules[6])	GenericModuleCopy(psSource.FindProperty("ColorModule"), psDest.FindProperty("ColorModule"));
		
		//Color Speed
		if(b_modules[7])	GenericModuleCopy(psSource.FindProperty("ColorBySpeedModule"), psDest.FindProperty("ColorBySpeedModule"));
		
		//Size
		if(b_modules[8])	GenericModuleCopy(psSource.FindProperty("SizeModule"), psDest.FindProperty("SizeModule"));
		
		//Size Speed
		if(b_modules[9])	GenericModuleCopy(psSource.FindProperty("SizeBySpeedModule"), psDest.FindProperty("SizeBySpeedModule"));
		
		//Rotation
		if(b_modules[10])	GenericModuleCopy(psSource.FindProperty("RotationModule"), psDest.FindProperty("RotationModule"));
		
		//Rotation Speed
		if(b_modules[11])	GenericModuleCopy(psSource.FindProperty("RotationBySpeedModule"), psDest.FindProperty("RotationBySpeedModule"));
		
		//Collision
		if(b_modules[12])	GenericModuleCopy(psSource.FindProperty("CollisionModule"), psDest.FindProperty("CollisionModule"));
		
		//Sub Emitters
		if(b_modules[13])	SubModuleCopy(psSource, psDest);
		
		//Texture Animation
		if(b_modules[14])	GenericModuleCopy(psSource.FindProperty("UVModule"), psDest.FindProperty("UVModule"));
		
		//Renderer
		if(b_modules[15])
		{
			ParticleSystemRenderer rendSource = source.GetComponent<ParticleSystemRenderer>();
			ParticleSystemRenderer rendDest = dest.GetComponent<ParticleSystemRenderer>();
			
			psSource = new SerializedObject(rendSource);
			psDest = new SerializedObject(rendDest);
			
			SerializedProperty ss = psSource.GetIterator();
			ss.Next(true);
			
			SerializedProperty sd = psDest.GetIterator();
			sd.Next(true);
			
			GenericModuleCopy(ss, sd, false);
		}
	}
	
	//Copy One Module's Values
	private void GenericModuleCopy(SerializedProperty ss, SerializedProperty sd, bool depthBreak = true)
	{
		while(true)
		{
			//Next Property
			if(!ss.NextVisible(true))
			{
				break;
			}
			sd.NextVisible(true);
			
			//If end of module: break
			if(depthBreak && ss.depth == 0)
			{
				break;
			}
			
			bool found = true;
			
			switch(ss.propertyType)
			{
				case SerializedPropertyType.Boolean : 			sd.boolValue = ss.boolValue; break;
				case SerializedPropertyType.Integer : 			sd.intValue = ss.intValue; break;
				case SerializedPropertyType.Float : 			sd.floatValue = ss.floatValue; break;
				case SerializedPropertyType.Color : 			sd.colorValue = ss.colorValue; break;
				case SerializedPropertyType.Bounds : 			sd.boundsValue = ss.boundsValue; break;
				case SerializedPropertyType.Enum : 				sd.enumValueIndex = ss.enumValueIndex; break;
				case SerializedPropertyType.ObjectReference : 	sd.objectReferenceValue = ss.objectReferenceValue; break;
				case SerializedPropertyType.Rect : 				sd.rectValue = ss.rectValue; break;
				case SerializedPropertyType.String : 			sd.stringValue = ss.stringValue; break;
				case SerializedPropertyType.Vector2 : 			sd.vector2Value = ss.vector2Value; break;
				case SerializedPropertyType.Vector3 : 			sd.vector3Value = ss.vector3Value; break;
				case SerializedPropertyType.AnimationCurve : 	sd.animationCurveValue = ss.animationCurveValue; break;
#if !UNITY_3_5
				case SerializedPropertyType.Gradient :			copyGradient(ss,sd); break;
#endif
				
				default: found = false; break;
			}
			
			if(!found)
			{
				found = true;
				
				switch(ss.type)
				{
					default: found = false; break;
				}
			}
		}
		
		//Apply Changes
		sd.serializedObject.ApplyModifiedProperties();
		
		ss.Dispose();
		sd.Dispose();
	}
	
#if !UNITY_3_5
	private void copyGradient(SerializedProperty ss, SerializedProperty sd)
	{
		SerializedProperty gradient = ss.Copy();
		SerializedProperty copyGrad = sd.Copy();
		gradient.Next(true);
		copyGrad.Next(true);
		do
		{
			switch(gradient.propertyType)
			{
				case SerializedPropertyType.Color:		copyGrad.colorValue = gradient.colorValue; break;
				case SerializedPropertyType.Integer:	copyGrad.intValue = gradient.intValue; break;
				default: Debug.Log("CopyGradient: Unrecognized property type:" + gradient.propertyType); break;
			}
			gradient.Next(true);
			copyGrad.Next(true);
		}
		while(gradient.depth > 2);
	}
#endif
	
	//Specific Copy for Sub Emitters Module (duplicate Sub Particle Systems)
	private void SubModuleCopy(SerializedObject source, SerializedObject dest)
	{
		dest.FindProperty("SubModule.enabled").boolValue = source.FindProperty("SubModule.enabled").boolValue;
		
		GameObject copy;
		if(source.FindProperty("SubModule.subEmitterBirth").objectReferenceValue != null)
		{
			//Duplicate sub Particle Emitter
			copy = (GameObject)Instantiate((source.FindProperty("SubModule.subEmitterBirth").objectReferenceValue as ParticleSystem).gameObject);
			
			//Set as child of destination
			Vector3 localPos = copy.transform.localPosition;
			Vector3 localScale = copy.transform.localScale;
			Vector3 localAngles = copy.transform.localEulerAngles;
			copy.transform.parent = (dest.targetObject as ParticleSystem).transform;
			copy.transform.localPosition = localPos;
			copy.transform.localScale = localScale;
			copy.transform.localEulerAngles = localAngles;
			
			//Assign as sub Particle Emitter
			dest.FindProperty("SubModule.subEmitterBirth").objectReferenceValue = copy;
		}
		
		if(source.FindProperty("SubModule.subEmitterDeath").objectReferenceValue != null)
		{
			//Duplicate sub Particle Emitter
			copy = (GameObject)Instantiate((source.FindProperty("SubModule.subEmitterDeath").objectReferenceValue as ParticleSystem).gameObject);
			
			//Set as child of destination
			Vector3 localPos = copy.transform.localPosition;
			Vector3 localScale = copy.transform.localScale;
			Vector3 localAngles = copy.transform.localEulerAngles;
			copy.transform.parent = (dest.targetObject as ParticleSystem).transform;
			copy.transform.localPosition = localPos;
			copy.transform.localScale = localScale;
			copy.transform.localEulerAngles = localAngles;
			
			//Assign as sub Particle Emitter
			dest.FindProperty("SubModule.subEmitterDeath").objectReferenceValue = copy;
		}
		
		if(source.FindProperty("SubModule.subEmitterCollision").objectReferenceValue != null)
		{
			//Duplicate sub Particle Emitter
			copy = (GameObject)Instantiate((source.FindProperty("SubModule.subEmitterCollision").objectReferenceValue as ParticleSystem).gameObject);
			
			//Set as child of destination
			Vector3 localPos = copy.transform.localPosition;
			Vector3 localScale = copy.transform.localScale;
			Vector3 localAngles = copy.transform.localEulerAngles;
			copy.transform.parent = (dest.targetObject as ParticleSystem).transform;
			copy.transform.localPosition = localPos;
			copy.transform.localScale = localScale;
			copy.transform.localEulerAngles = localAngles;
			
			//Assign as sub Particle Emitter
			dest.FindProperty("SubModule.subEmitterCollision").objectReferenceValue = copy;
		}
		
		//Apply Changes
		dest.ApplyModifiedProperties();
	}
	
	//Scale System
	private void ScaleParticleValues(ParticleSystem ps, GameObject parent)
	{
		//Particle System
		ps.startSize *= ScalingValue;
		ps.gravityModifier *= ScalingValue;
		if(ps.startSpeed > 0.01f) ps.startSpeed *= ScalingValue;
		if(ps.gameObject != parent)
			ps.transform.localPosition *= ScalingValue;
		
		SerializedObject psSerial = new SerializedObject(ps);
		
		//Scale Size By Speed Module
		if(psSerial.FindProperty("SizeBySpeedModule.enabled").boolValue)
		{
			psSerial.FindProperty("SizeBySpeedModule.range.x").floatValue *= ScalingValue;
			psSerial.FindProperty("SizeBySpeedModule.range.y").floatValue *= ScalingValue;
		}
		
		//Scale Velocity Module
		if(psSerial.FindProperty("VelocityModule.enabled").boolValue)
		{
			psSerial.FindProperty("VelocityModule.x.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("VelocityModule.y.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("VelocityModule.z.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);
		}
		
		//Scale Limit Velocity Module
		if(psSerial.FindProperty("ClampVelocityModule.enabled").boolValue)
		{
			psSerial.FindProperty("ClampVelocityModule.x.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("ClampVelocityModule.y.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("ClampVelocityModule.z.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.z.maxCurve").animationCurveValue);
			
			psSerial.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.magnitude.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.magnitude.maxCurve").animationCurveValue);
		}
		
		//Scale Force Module
		if(psSerial.FindProperty("ForceModule.enabled").boolValue)
		{
			psSerial.FindProperty("ForceModule.x.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("ForceModule.y.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("ForceModule.z.scalar").floatValue *= ScalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
		}
		
		//Scale Shape Module
		if(psSerial.FindProperty("ShapeModule.enabled").boolValue)
		{
			psSerial.FindProperty("ShapeModule.boxX").floatValue *= ScalingValue;
			psSerial.FindProperty("ShapeModule.boxY").floatValue *= ScalingValue;
			psSerial.FindProperty("ShapeModule.boxZ").floatValue *= ScalingValue;
			psSerial.FindProperty("ShapeModule.radius").floatValue *= ScalingValue;
			
			//Create a new scaled Mesh if there is a Mesh reference
			//(ShapeModule.type 6 == Mesh)
			if(psSerial.FindProperty("ShapeModule.type").intValue == 6)
			{
				Object obj = psSerial.FindProperty("ShapeModule.m_Mesh").objectReferenceValue;
				if(obj != null)
				{
					Mesh mesh = (Mesh)obj;
					string assetPath = AssetDatabase.GetAssetPath(mesh);
					string name = assetPath.Substring(assetPath.LastIndexOf("/")+1);
					
					//Mesh to use
					Mesh meshToUse = null;
					bool createScaledMesh = true;
					float meshScale = ScalingValue;
					
					//Mesh has already been scaled: extract scaling value and re-scale base effect
					if(name.Contains("(scaled)"))
					{
						string scaleStr = name.Substring(name.LastIndexOf("x")+1);
						scaleStr = scaleStr.Remove(scaleStr.IndexOf(" (scaled).asset"));
						
						float oldScale = float.Parse(scaleStr);
						if(oldScale != 0)
						{
							meshScale = oldScale * ScalingValue;
							
							//Check if there's already a mesh with the correct scale
							string unscaledName = assetPath.Substring(0, assetPath.LastIndexOf(" x"));
							assetPath = unscaledName;
							string newPath = assetPath + " x"+meshScale+" (scaled).asset";
							Mesh alreadyScaledMesh = (Mesh)AssetDatabase.LoadAssetAtPath(newPath, typeof(Mesh));
							if(alreadyScaledMesh != null)
							{
								meshToUse = alreadyScaledMesh;
								createScaledMesh = false;
							}
							else
							//Load original unscaled mesh
							{
								Mesh orgMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mesh));
								if(orgMesh != null)
								{
									mesh = orgMesh;
								}
							}
						}
					}
					else
					//Verify if original mesh has already been scaled to that value
					{
						string newPath = assetPath + " x"+meshScale+" (scaled).asset";
						Mesh alreadyScaledMesh = (Mesh)AssetDatabase.LoadAssetAtPath(newPath, typeof(Mesh));
						if(alreadyScaledMesh != null)
						{
							meshToUse = alreadyScaledMesh;
							createScaledMesh = false;
						}
					}
					
					//Duplicate and scale mesh vertices if necessary
					if(createScaledMesh)
					{
						string newMeshPath = assetPath + " x"+meshScale+" (scaled).asset";
						meshToUse = (Mesh)AssetDatabase.LoadAssetAtPath(newMeshPath, typeof(Mesh));
						if(meshToUse == null)
						{
							meshToUse = DuplicateAndScaleMesh(mesh, meshScale);
							AssetDatabase.CreateAsset(meshToUse, newMeshPath);
						}
					}
					
					//Apply new Mesh
					psSerial.FindProperty("ShapeModule.m_Mesh").objectReferenceValue = meshToUse;
				}
			}
		}
		
		//Apply Modified Properties
		psSerial.ApplyModifiedProperties();
	}
	
	//Iterate and Scale Keys (Animation Curve)
	private void IterateKeys(AnimationCurve curve)
	{
		for(int i = 0; i < curve.keys.Length; i++)
		{
			curve.keys[i].value *= ScalingValue;
		}
	}
	
	//Create Scaled Mesh
	private Mesh DuplicateAndScaleMesh(Mesh mesh, float Scale)
	{
		Mesh scaledMesh = new Mesh();
		
		Vector3[] scaledVertices = new Vector3[mesh.vertices.Length];
		for(int i = 0; i < scaledVertices.Length; i++)
		{
			scaledVertices[i] = mesh.vertices[i] * Scale;
		}
		scaledMesh.vertices = scaledVertices;
		
		scaledMesh.normals = mesh.normals;
		scaledMesh.tangents = mesh.tangents;
		scaledMesh.triangles = mesh.triangles;
		scaledMesh.uv = mesh.uv;
		scaledMesh.uv2 = mesh.uv2;
		scaledMesh.colors = mesh.colors;
		
		return scaledMesh;
	}
}
