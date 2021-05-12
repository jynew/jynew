/***********************************************
				EasyTouch Controls
	Copyright © 2016 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
public class ETCArea : MonoBehaviour {

	public enum AreaPreset { Choose, TopLeft, TopRight, BottomLeft, BottomRight};

	public bool show;

	#region Constructeur
	public ETCArea(){
		show = true;
	}
	#endregion

	#region MonoBehaviour Callback
	public void Awake(){
		GetComponent<Image>().enabled = show;
	}
	#endregion

	public void ApplyPreset(AreaPreset preset){

		RectTransform parent = transform.parent.GetComponent<RectTransform>();
	
		switch (preset) {
			case AreaPreset.TopRight:
				this.rectTransform().anchoredPosition = new Vector2(parent.rect.width/4f,parent.rect.height/4f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,parent.rect.width/2f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,parent.rect.height/2f);

				this.rectTransform().anchorMin = new Vector2(1,1);
				this.rectTransform().anchorMax = new Vector2(1,1);
				this.rectTransform().anchoredPosition = new Vector2( -this.rectTransform().sizeDelta.x/2f , -this.rectTransform().sizeDelta.y/2f );

				break;

			case AreaPreset.TopLeft:
				this.rectTransform().anchoredPosition = new Vector2(-parent.rect.width/4f,parent.rect.height/4f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,parent.rect.width/2f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,parent.rect.height/2f);

				this.rectTransform().anchorMin = new Vector2(0,1);
				this.rectTransform().anchorMax = new Vector2(0,1);
				this.rectTransform().anchoredPosition = new Vector2( this.rectTransform().sizeDelta.x/2f, -this.rectTransform().sizeDelta.y/2f );

				break;

			case AreaPreset.BottomRight:
				this.rectTransform().anchoredPosition = new Vector2(parent.rect.width/4f,-parent.rect.height/4f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,parent.rect.width/2f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,parent.rect.height/2f);

				this.rectTransform().anchorMin = new Vector2(1,0);
				this.rectTransform().anchorMax = new Vector2(1,0);
				this.rectTransform().anchoredPosition = new Vector2( -this.rectTransform().sizeDelta.x/2f , this.rectTransform().sizeDelta.y/2f );

				break;
				
			case AreaPreset.BottomLeft:
				this.rectTransform().anchoredPosition = new Vector2(-parent.rect.width/4f,-parent.rect.height/4f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,parent.rect.width/2f);
				this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,parent.rect.height/2f);

				this.rectTransform().anchorMin = new Vector2(0,0);
				this.rectTransform().anchorMax = new Vector2(0,0);
				this.rectTransform().anchoredPosition = new Vector2( this.rectTransform().sizeDelta.x/2f , this.rectTransform().sizeDelta.y/2f );

				break;
		}

		 

	}
}

