using UnityEngine;
using System.Collections;
using UnityEditor;

public class HTGuiTools{

	private static Texture2D gradientTexture;

	#region Widget
	public static bool BeginFoldOut(string text,bool foldOut, bool endSpace=true){
	
		text = "<b><size=11>" + text + "</size></b>";
		if (foldOut){
			text = "\u25BC " + text;
		}
		else{
			text = "\u25BA " + text;
		}

		if ( !GUILayout.Toggle(true,text,"dragtab")){
			foldOut=!foldOut;
		}

		if (!foldOut && endSpace)GUILayout.Space(5f);

		return foldOut;
	}

	public static void BeginGroup(int padding=0){


		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		//EditorGUILayout.BeginHorizontal("As TextArea", GUILayout.MinHeight(10f));
        EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
		GUILayout.Space(2f);


	}

	public static void EndGroup(bool endSpace = true){
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(3f);
		GUILayout.EndHorizontal();

		if (endSpace){
			GUILayout.Space(10f);
		}
	}


	public static bool Toggle(string text, bool value,bool leftToggle=false){

		//if (value) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			
		if (leftToggle){
			value = EditorGUILayout.ToggleLeft(text,value); 
		}
		else{
			value = EditorGUILayout.Toggle(text,value); 
		}

		//GUI.backgroundColor = Color.white;

		return value;

	}

	public static bool Toggle (string text, bool value,int width,bool leftToggle=false){

		if (value) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
		
		if (leftToggle){
			value = EditorGUILayout.ToggleLeft(text,value,GUILayout.Width( width)); 
		}
		else{
			value = EditorGUILayout.Toggle(text,value,GUILayout.Width( width)); 
		}
		
		GUI.backgroundColor = Color.white;
		
		return value;
	}

	static public bool Button(string label,Color color,int width, bool leftAligment=false, int height=0){

		GUI.backgroundColor  = color;
		GUIStyle buttonStyle = new GUIStyle("Button");
		
		if (leftAligment)
			buttonStyle.alignment = TextAnchor.MiddleLeft;
		
		if (height==0){
			if (GUILayout.Button( label,buttonStyle,GUILayout.Width(width))){
				GUI.backgroundColor = Color.white;
				return true;	
			}
		}
		else{
			if (GUILayout.Button( label,buttonStyle,GUILayout.Width(width),GUILayout.Height(height))){
				GUI.backgroundColor = Color.white;
				return true;	
			}			
		}
		GUI.backgroundColor = Color.white;		

		return false;
	}
	#endregion

	#region 2d effect
	public static void DrawSeparatorLine(int padding=0){
		
		EditorGUILayout.Space();
		DrawLine(Color.gray, padding);
		EditorGUILayout.Space();
	}
	
	private static void DrawLine(Color color,int padding=0){
		
		GUILayout.Space(10);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		
		GUI.color = color;
		GUI.DrawTexture(new Rect(padding, lastRect.yMax -lastRect.height/2f, Screen.width, 1f), EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
	}
	#endregion

	#region Texture
	private static Rect DrawGradient(int padding, int width, int height=35){

		GUILayout.Space(height);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		lastRect.yMin = lastRect.yMin + 7;
		lastRect.yMax = lastRect.yMax - 7;
		lastRect.width =  Screen.width;

		GUI.DrawTexture(new Rect(padding,lastRect.yMin+1,width, lastRect.yMax- lastRect.yMin), GetGradientTexture());
		
		return lastRect;
	}
	
	private static Texture2D GetGradientTexture(){
		
		if (gradientTexture==null){
			gradientTexture = CreateGradientTexture();
		}
		return gradientTexture;
		
	}
	
	private static Texture2D CreateGradientTexture(){
	
		int height =18;
		
		Texture2D myTexture = new Texture2D(1, height);
		myTexture.hideFlags = HideFlags.HideInInspector;
		myTexture.hideFlags = HideFlags.DontSave;
		myTexture.filterMode = FilterMode.Bilinear;
		
		Color startColor= new Color(0.4f,0.4f,0.4f);
		Color endColor= new Color(0.6f,0.6f,0.6f);
		
		float stepR = (endColor.r - startColor.r)/18f;
		float stepG = (endColor.g - startColor.g)/18f;
		float stepB = (endColor.b - startColor.b)/18f;
		
		Color pixColor = startColor;
		
		for (int i = 1; i < height-1; i++)
		{
			pixColor = new Color(pixColor.r + stepR,pixColor.g + stepG , pixColor.b + stepB);
			myTexture.SetPixel(0, i, pixColor);
		}
		
		myTexture.SetPixel(0, 0, new Color(0,0,0));
		myTexture.SetPixel(0, 17, new Color(1,1,1));
		
		myTexture.Apply();
		
		return myTexture;
	}
	#endregion
}
