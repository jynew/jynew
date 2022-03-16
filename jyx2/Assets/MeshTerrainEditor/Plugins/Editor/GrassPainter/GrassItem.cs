using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Grass instance wrapper for editing
    /// </summary>
    public class GrassItem : IQuadObject
    {
        private readonly GrassStar star;
        private readonly GrassQuad quad;
        public GameObject gameObject;

        public GrassItem(GrassStar star, GameObject gameObject)
        {
            this.star = star;
            this.gameObject = gameObject;
        }

        public GrassItem(GrassQuad quad, GameObject gameObject)
        {
            this.quad = quad;
            this.gameObject = gameObject;
        }

        public GrassStar Star
        {
            get { return this.star; }
        }

        public GrassQuad Quad
        {
            get { return this.quad; }
        }

        public bool Destroyed
        {
            get { return !this.gameObject; }
        }

        public Vector2 Position2D
        {
            get
            {
                if(this.quad != null)
                {
                    return new Vector2(this.quad.Position.x, this.quad.Position.z);
                }
                if (this.star != null)
                {
                    return new Vector2(this.star.Position.x, this.star.Position.z);
                }
                return Vector2.zero;
            }

            set
            {
                if (this.quad != null)
                {
                    this.quad.Position = value;
                }
                else
                {
                    this.star.Position = value;
                }
 
                if (!this.gameObject)
                {//ignore missing GameObject
                    return;
                }
                this.gameObject.transform.position = value;
            }
        }

        public float Size
        {
            get
            {
                if (this.quad != null)
                {
                    return this.quad.Width;
                }
                if (this.star != null)
                {
                    return this.star.Width;
                }

                return 0;
            }
        }

        public float Height
        {
            get
            {
                if (this.quad != null)
                {
                    return this.quad.Position.y;
                }
                if (this.star != null)
                {
                    return this.star.Position.y;
                }

                return 0;
            }
            set
            {
                if (this.quad != null)
                {
                    var p = this.quad.Position;
                    this.quad.Position = new Vector3(p.x, value, p.z);
                }
                if (this.star != null)
                {
                    var p = this.star.Position;
                    this.star.Position = new Vector3(p.x, value, p.z);
                }

                {
                    if (!this.gameObject)
                    {//ignore missing GameObject
                        return;
                    }
                    var p = this.gameObject.transform.position;
                    this.gameObject.transform.position = new Vector3(p.x, value, p.z);
                }
            }
        }

        public float RotationY
        {
            get
            {
                if (this.quad != null)
                {
                    return this.quad.RotationY;
                }

                if (this.star != null)
                {
                    return this.star.RotationY;
                }

                return 0;
            }
            set
            {
                if (this.quad != null)
                {
                    var p = this.quad.RotationY;
                    this.quad.RotationY = value;
                }

                if (this.star != null)
                {
                    var p = this.star.Position;
                    this.star.RotationY = value;
                }

                if (!this.gameObject)
                {//ignore missing GameObject
                    return;
                }
                this.gameObject.transform.rotation = Quaternion.Euler(0, value, 0);
            }
        }

        public Rect Bounds
        {
            get
            {
                if (this.quad != null)
                {
                    return new Rect(this.quad.Position.x, this.quad.Position.z,
                        0.00001f, 0.00001f);
                }
                if (this.star != null)
                {
                    return new Rect(this.star.Position.x, this.star.Position.z,
                        0.00001f, 0.00001f);
                }
                return Rect.zero;
            }
        }
    }
}