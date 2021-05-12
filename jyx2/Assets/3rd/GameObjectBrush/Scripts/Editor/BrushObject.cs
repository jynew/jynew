using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GameObjectBrush
{

    /// <summary>
    /// Class that is responsible for holding information about a brush, such as the prefab/gameobject, size, density, etc.
    /// </summary>
    [System.Serializable]
    public class BrushObject
    {
        public GameObject brushObject;

        [Tooltip("Use prefab instantiation instead of cloning.")] public bool usePrefabs = true;
        public bool allowIntercollision = false;
        public bool alignToSurface = false;
        [Tooltip("Should the rotation be randomized on the x axis?")] public bool randomizeXRotation = false;
        [Tooltip("Should the rotation be randomized on the y axis?")] public bool randomizeYRotation = true;
        [Tooltip("Should the rotation be randomized on the z axis?")] public bool randomizeZRotation = false;

        public Transform parentContainer;
        [Range(0, 1)] public float density = 1f;
        [Range(0, 100)] public float brushSize = 5f;
        [Range(0, 10)] public float minScale = 0.5f;
        [Range(0, 10)] public float maxScale = 1.5f;
        [Tooltip("The offset applied to the pivot of the brushObject. This is usefull if you find that the placed GameObjects are floating/sticking in the ground too much.")] public Vector3 offsetFromPivot = Vector3.zero;
        [Tooltip("The offset applied to the rotation of the brushObject.")] public Vector3 rotOffsetFromPivot = Vector3.zero;

        /* filters */
        [Range(0, 360)] public float minSlope = 0f;
        [Range(0, 360)] public float maxSlope = 360f;
        public LayerMask layerFilter = ~0;

        public bool isTagFilteringEnabled = false;
        public string tagFilter = "";


        public BrushObject(GameObject obj)
        {
            this.brushObject = obj;
        }

        /// <summary>
        /// Pastes the details from another brush
        /// </summary>
        public void PasteDetails(BrushObject brush)
        {
            if (brush != null)
            {
                parentContainer = brush.parentContainer;
                allowIntercollision = brush.allowIntercollision;
                alignToSurface = brush.alignToSurface;
                randomizeXRotation = brush.randomizeXRotation;
                randomizeYRotation = brush.randomizeYRotation;
                randomizeZRotation = brush.randomizeZRotation;
                density = brush.density;
                brushSize = brush.brushSize;
                minScale = brush.minScale;
                maxScale = brush.maxScale;
                offsetFromPivot = brush.offsetFromPivot;
                rotOffsetFromPivot = brush.rotOffsetFromPivot;
            }
        }
        /// <summary>
        /// Pastes the filters from another brush
        /// </summary>
        public void PasteFilters(BrushObject brush)
        {
            if (brush != null)
            {
                minSlope = brush.minSlope;
                maxSlope = brush.maxSlope;
                layerFilter = brush.layerFilter;
                isTagFilteringEnabled = brush.isTagFilteringEnabled;
                tagFilter = brush.tagFilter;
            }
        }

        /// <summary>
        /// Resets the filters on this bursh
        /// </summary>
        public void ResetFilters()
        {
            minSlope = 0f;
            maxSlope = 360f;
            layerFilter = ~0;
            isTagFilteringEnabled = false;
            tagFilter = "";
        }
        /// <summary>
        /// Resets the details of this brush
        /// </summary>
        public void ResetDetails()
        {
            parentContainer = null;
            allowIntercollision = false;
            alignToSurface = false;
            randomizeXRotation = false;
            randomizeYRotation = true;
            randomizeZRotation = false;
            density = 1f;
            brushSize = 5f;
            minScale = 0.5f;
            maxScale = 1.5f;
            offsetFromPivot = Vector3.zero;
            rotOffsetFromPivot = Vector3.zero;
        }
    }
}