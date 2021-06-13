/// <summary>
/// UGUI animation Component.
/// 
/// 使用方法：把他和Image组件挂在同一个GameObject上，调整好偏移。
/// 动画不要修改Image组件的sprite，修改本组件sprite即可。
/// 
/// author:CHENGGONG 
/// hanjiasongshu.com
/// </summary>
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UGUIAnimation : MonoBehaviour {

    public Sprite Sprite { get; set; }
    public float xoffset = 0;
    public float yoffset = 0;

    /// <summary>
    /// 启动flipx，需要有一个parent GameObject
    /// </summary>
    public bool enableFlipX = false;
    public bool flipX;

    void OnValidate(){
        Refresh();
        SetColor(Color.white);
    }

    public void Refresh(){
        SetSprite(Sprite);
    }

    void SetSprite(Sprite s){
		
        var image = this.GetComponent<Image>();
		if (image == null)
			return;
		if (s == null) {
			image.enabled = false;
			return;
		} else {
			image.enabled = true;
		}
        image.sprite = s;
        image.rectTransform.sizeDelta = new Vector2( s.rect.width, s.rect.height);
        var pos1x = 0.5f * s.rect.width - s.pivot.x;
        var pos1y = 0.5f * s.rect.height - s.pivot.y;

        image.transform.localPosition = new Vector3( pos1x + xoffset, pos1y + yoffset);

        if(enableFlipX){
            if(flipX){
                this.transform.parent.transform.localRotation = new Quaternion(0, -180f, 0, 0);    
            }else{

                this.transform.parent.transform.localRotation = new Quaternion(0, 0, 0, 0);    
            }    
        }
    }

    public void SetColor(Color color){
        this.GetComponent<Image>().color = color;
    }

    public Color getColor(){
        return this.GetComponent<Image>().color;
    }
}
