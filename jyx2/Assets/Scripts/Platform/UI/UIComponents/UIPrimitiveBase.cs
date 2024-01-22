using System;

namespace UnityEngine.UI {  
  
    public class UIPrimitiveBase :MaskableGraphic, ILayoutElement, ICanvasRaycastFilter {  

        [SerializeField]  
        private Sprite m_Sprite;  
  
        public Sprite sprite {
			get { 
				return m_Sprite; 
			}
			set { 
//				if (SetPropertyUtility.SetClass(ref m_Sprite, value))
				SetAllDirty(); 
			}
		}  
  
        [NonSerialized]  
        private Sprite m_OverrideSprite;  
  
        public Sprite overrideSprite {
			get {
				return m_OverrideSprite == null ? sprite : m_OverrideSprite; 
			} 
			set { 
//				if(SetPropertyUtility.SetClass(ref m_OverrideSprite, value)) 
				SetAllDirty(); 
			} 
		}
  
   
  
        // Not serialized until we supportread-enabled sprites better.  
  
        internal float m_EventAlphaThreshold =1;  
  
        public float eventAlphaThreshold { get{ return m_EventAlphaThreshold; } set { m_EventAlphaThreshold = value; } }  
  
        /// <summary>  
  
        /// Image's texture comes from theUnityEngine.Image.  
  
        /// </summary>  
  
        public override Texture mainTexture  
  
        {  
  
            get  
  
            {  
  
                if (overrideSprite == null)  
  
                {  
  
                    if (material != null&& material.mainTexture != null)  
  
                    {  
  
                        return material.mainTexture;  
  
                    }  
  
                    return s_WhiteTexture;  
  
                }  

                return overrideSprite.texture;  
  
            }  
  
        }  
  
        public float pixelsPerUnit  
        {  
            get  
            {  
  
                float spritePixelsPerUnit =100;  
  
                if (sprite)  
  
                    spritePixelsPerUnit =sprite.pixelsPerUnit;  
  
   
  
                float referencePixelsPerUnit =100;  
  
                if (canvas)  
  
                    referencePixelsPerUnit =canvas.referencePixelsPerUnit;  

                return spritePixelsPerUnit /referencePixelsPerUnit;  
  
            }  
  
        }  
  

        protected UIVertex[] SetVbo(Vector2[]vertices, Vector2[] uvs)  
  
        {  
  
            UIVertex[] vbo = new UIVertex[4];  
  
            for (int i = 0; i <vertices.Length; i++)  
  
            {  
  
                var vert = UIVertex.simpleVert;  
  
                vert.color = color;  
  
                vert.position = vertices[i];  
  
                vert.uv0 = uvs[i];  
  
                vbo[i] = vert;  
  
            }  
  
            return vbo;  
  
        }  
 
  
 
  
 
        #region ILayoutElement Interface  
  
   
  
        public virtual void CalculateLayoutInputHorizontal() { }  
  
        public virtual void CalculateLayoutInputVertical() { }  
  
   
  
        public virtual float minWidth { get {return 0; } }  
  
   
  
        public virtual float preferredWidth  
  
        {  
  
            get  
  
            {  
  
                if (overrideSprite == null)  
  
                    return 0;  
  
                return overrideSprite.rect.size.x / pixelsPerUnit;  
  
            }  
  
        }  
  
   
  
        public virtual float flexibleWidth {get { return -1; } }  
  
   
  
        public virtual float minHeight { get {return 0; } }  
  
   
  
        public virtual float preferredHeight  
  
        {  
  
            get  
  
            {  
  
                if (overrideSprite == null)  
  
                    return 0;  
  
                return overrideSprite.rect.size.y / pixelsPerUnit;  
  
            }  
  
        }  
  
   
  
        public virtual float flexibleHeight {get { return -1; } }  
  
   
  
        public virtual int layoutPriority { get{ return 0; } }  
 
  
 
        #endregion  
 
  
 
        #region ICanvasRaycastFilter Interface  
  
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)  
  
        {  
  
            if (m_EventAlphaThreshold >= 1)  
  
                return true;  
  
   
  
            Sprite sprite = overrideSprite;  
  
            if (sprite == null)  
  
                return true;  
  
   
  
            Vector2 local;  
  
           RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,screenPoint, eventCamera, out local);  
  
   
  
            Rect rect = GetPixelAdjustedRect();  
  
   
  
            // Convert to have lower leftcorner as reference point.  
  
            local.x += rectTransform.pivot.x *rect.width;  
  
            local.y += rectTransform.pivot.y *rect.height;  
  
   
  
            local = MapCoordinate(local, rect);  
  
   
  
            // Normalize local coordinates.  
  
            Rect spriteRect =sprite.textureRect;  
  
            Vector2 normalized = new Vector2(local.x / spriteRect.width, local.y / spriteRect.height);  
  
   
  
            // Convert to texture space.  
  
            float x = Mathf.Lerp(spriteRect.x,spriteRect.xMax, normalized.x) / sprite.texture.width;  
  
            float y = Mathf.Lerp(spriteRect.y,spriteRect.yMax, normalized.y) / sprite.texture.height;  
  
   
  
            try  
  
            {  
  
                return sprite.texture.GetPixelBilinear(x, y).a >= m_EventAlphaThreshold;  
  
            }  
  
            catch (UnityException e)  
  
            {  
  
                Debug.LogError("UsingclickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read." + e.Message + " Also make sure to disable sprite packing for thissprite.", this);  
  
                return true;  
  
            }  
  
        }  
  
   
  
        /// <summary>  
  
        /// Return image adjusted position  
  
        /// **Copied from Unity's Imagecomponent for now and simplified for UI Extensions primatives  
  
        /// </summary>  
  
        /// <paramname="local"></param>  
  
        /// <paramname="rect"></param>  
  
        /// <returns></returns>  
  
        private Vector2 MapCoordinate(Vector2 local, Rect rect)  
  
        {  
  
            Rect spriteRect = sprite.rect;  
  
                return new Vector2(local.x *spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);  
  
        }  
  
   
  
        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)  
  
        {  
  
            for (int axis = 0; axis <= 1;axis++)  
  
            {  
  
                float combinedBorders =border[axis] + border[axis + 2];  
  
                if (rect.size[axis] <combinedBorders && combinedBorders != 0)  
  
                {  
  
                    float borderScaleRatio =rect.size[axis] / combinedBorders;  
  
                    border[axis] *=borderScaleRatio;  
  
                    border[axis + 2] *=borderScaleRatio;  
  
                }  
  
            }  
  
            return border;  
  
        }  
 
  
 
        #endregion  
  
   
  
   
  
    }  
  
}  