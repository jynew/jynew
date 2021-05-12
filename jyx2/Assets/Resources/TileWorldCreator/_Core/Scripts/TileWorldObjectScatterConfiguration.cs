using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TileWorld
{
    [System.Serializable]
    public class TileWorldObjectScatterConfiguration : ScriptableObject
    {

        public void OnEnable() {  }
        /// <summary>
        /// Create a new configuration. 
        /// _currentConfiguration = assign the current configuration to copy the presets from if _preserveObjects is true
        /// _preserveObjects = copy presets from current configuration to the new configuration.
        /// </summary>
        /// <param name="_currentConfig"></param>
        /// <param name="_preserveObjects"></param>
        /// <returns></returns>
        public static TileWorldObjectScatterConfiguration NewConfiguration(TileWorldObjectScatterConfiguration _currentConfig, bool _preserveObjects)
        {
            var _so = ScriptableObject.CreateInstance<TileWorldObjectScatterConfiguration>();

            if (_preserveObjects)
            {
                _so.paintObjects = _currentConfig.paintObjects;
                _so.positionObjects = _currentConfig.positionObjects;
                _so.proceduralObjects = _currentConfig.proceduralObjects;
            }

            return _so;
        }


        // Base class for object scattering
        [System.Serializable]
        public class DefaultObjectConfiguration
        {
            public GameObject go;
            public bool active;
            public bool isChild;
            public bool placeOnOccupiedCell;
            public bool useTileRotation;
            public Vector3 offsetRotation;
            public Vector3 offsetPosition;
            public int selectedLayer;

            // If current object is a child, use child spawn properties
            public float radius;
            public int spawnCount;
            // random rotation
            public bool useRandomRotation;
            public Vector3 randomRotationMin;
            public Vector3 randomRotationMax;
            // random scaling
            public bool useRandomScaling;
            public bool uniformScaling;
            public Vector3 randomScalingMax;
            public Vector3 randomScalingMin;


            // Only for ui
            public bool showPanel;

            // Default constructor
            public DefaultObjectConfiguration()
            {
                isChild = false;
                active = true;
                useTileRotation = false;
                offsetPosition = Vector3.zero;
                offsetRotation = Vector3.zero;

                selectedLayer = 0;
                radius = 5;
                spawnCount = 5;
                useRandomScaling = false;
                uniformScaling = false;
                useRandomRotation = false;
                randomRotationMin = Vector3.zero;
                randomRotationMax = Vector3.zero;
                randomScalingMax = new Vector3(1, 1, 1);
                randomScalingMin = new Vector3(1, 1, 1);
            }
        }

        [System.Serializable]
        public class PositionObjectConfiguration : DefaultObjectConfiguration
        {
            public enum PlacementType
            {
                bestGuess,
                mapBased
            }

            public PlacementType placementType;

            public enum MapBasedSpawnPosition
            {
                startPosition,
                endPosition
            }

            public MapBasedSpawnPosition mapBasedSpawnPosition;

            public enum BestGuessSpawnPosition
            {
                topLeft,
                topMiddle,
                topRight,
                centerLeft,
                centerMiddle,
                centerRight,
                bottomLeft,
                bottomMiddle,
                bottomRight
            }

            public BestGuessSpawnPosition bestGuessSpawnPosition;

            public enum BlockType
            {
                block,
                ground
            }

            public BlockType blockType;

            public PositionObjectConfiguration(GameObject _go)
            {
                this.go = _go;
            }

            public PositionObjectConfiguration(PositionObjectConfiguration _config)
            {
                go = _config.go;
                active = _config.active;
                placeOnOccupiedCell = _config.placeOnOccupiedCell;
                useTileRotation = _config.useTileRotation;
                offsetRotation = _config.offsetRotation;
                offsetPosition = _config.offsetPosition;
                selectedLayer = _config.selectedLayer;

                // If current object is a child, use child spawn properties
                radius = _config.radius;
                spawnCount = _config.spawnCount;
                // random rotation
                useRandomRotation = _config.useRandomRotation;
                randomRotationMin = _config.randomRotationMin;
                randomRotationMax = _config.randomRotationMax;
                // random scaling
                useRandomScaling = _config.useRandomScaling;
                uniformScaling = _config.uniformScaling;
                randomScalingMax = _config.randomScalingMax;
                randomScalingMin = _config.randomScalingMin;

                placementType = _config.placementType;
                mapBasedSpawnPosition = _config.mapBasedSpawnPosition;
                bestGuessSpawnPosition = _config.bestGuessSpawnPosition;
            }
        }

        [System.Serializable]
        public class PaintObjectConfiguration : DefaultObjectConfiguration
        {
            public bool paintThisObject;


            public PaintObjectConfiguration(GameObject _go)
            {
                this.go = _go;
                paintThisObject = false;
                selectedLayer = 0;
            }

            public PaintObjectConfiguration(PaintObjectConfiguration _config)
            {
                go = _config.go;
                active = _config.active;
                placeOnOccupiedCell = _config.placeOnOccupiedCell;
                useTileRotation = _config.useTileRotation;
                offsetRotation = _config.offsetRotation;
                offsetPosition = _config.offsetPosition;
                selectedLayer = _config.selectedLayer;

                // If current object is a child, use child spawn properties
                radius = _config.radius;
                spawnCount = _config.spawnCount;
                // random rotation
                useRandomRotation = _config.useRandomRotation;
                randomRotationMin = _config.randomRotationMin;
                randomRotationMax = _config.randomRotationMax;
                // random scaling
                useRandomScaling = _config.useRandomScaling;
                uniformScaling = _config.uniformScaling;
                randomScalingMax = _config.randomScalingMax;
                randomScalingMin = _config.randomScalingMin;

                paintThisObject = _config.paintThisObject;
            }
        }


        [System.Serializable]
        public class ProceduralObjectConfiguration : DefaultObjectConfiguration
        {
            public enum RuleTypes
            {
                random,
                pattern
            }

            public RuleTypes ruleType;

            // random properties
            public float weight;
            public enum BlockType
            {
                block,
                ground
            }

            public BlockType blockType;

            // pattern properties
            public enum TileTypes
            {
                edge,
                corner,
                invertedCorner,
                block,
                ground
            }

            public TileTypes tileTypes;

            public int everyNTile;
            public int inset;


            public ProceduralObjectConfiguration(GameObject _go)
            {
                this.go = _go;
                everyNTile = 1;
                inset = 0;
            }

            public ProceduralObjectConfiguration(ProceduralObjectConfiguration _config)
            {
                go = _config.go;
                active = _config.active;
                placeOnOccupiedCell = _config.placeOnOccupiedCell;
                useTileRotation = _config.useTileRotation;
                offsetRotation = _config.offsetRotation;
                offsetPosition = _config.offsetPosition;
                selectedLayer = _config.selectedLayer;

                // If current object is a child, use child spawn properties
                radius = _config.radius;
                spawnCount = _config.spawnCount;
                // random rotation
                useRandomRotation = _config.useRandomRotation;
                randomRotationMin = _config.randomRotationMin;
                randomRotationMax = _config.randomRotationMax;
                // random scaling
                useRandomScaling = _config.useRandomScaling;
                uniformScaling = _config.uniformScaling;
                randomScalingMax = _config.randomScalingMax;
                randomScalingMin = _config.randomScalingMin;

                ruleType = _config.ruleType;
                weight = _config.weight;
                blockType = _config.blockType;
                tileTypes = _config.tileTypes;
                everyNTile = _config.everyNTile;
                inset = _config.inset;
                
            }
        }
        
        public List<PaintObjectConfiguration> paintObjects = new List<PaintObjectConfiguration>();
        public List<PositionObjectConfiguration> positionObjects = new List<PositionObjectConfiguration>();
        public List<ProceduralObjectConfiguration> proceduralObjects = new List<ProceduralObjectConfiguration>();

    }
}
