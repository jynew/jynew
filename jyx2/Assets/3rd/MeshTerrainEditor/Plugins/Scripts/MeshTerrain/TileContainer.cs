using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Tiled mesh terrain container, this component can be saved removed when building.
    /// </summary>
    public class TileContainer : MonoBehaviour
    {
        public void OnEnable()
        {
            if (!Application.isEditor)
            {
                // remove this component
                Destroy(this);
            }
        }
    }
}