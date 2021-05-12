/*
    TileWorldEvents
    
    See the runtime demo scene to know how to use them
     
    OnBuildComplete -> Gets called after building a map is complete
    OnScatterProceduralComplete -> Gets called after instantiating all procedural objects
    OnScatterPositionBasedComplete -> Gets called after instantiating all position based objects
    BuildProgress -> Returns current build progress from 0 - 100
    MergeProgress -> Returns current merge progress from 0 - 100

*/
using UnityEngine;
using System.Collections;

namespace TileWorld.Events
{
    public class TileWorldEvents : MonoBehaviour
    {

        // Events 
        public delegate void TileWorldMessages();
        public static event TileWorldMessages OnBuildComplete;
        public static event TileWorldMessages OnScatterProceduralComplete;
        public static event TileWorldMessages OnScatterPositionBasedComplete;
        public static event TileWorldMessages OnMergeComplete;

        public delegate void TileWorldProgress(float _progress);
        public static event TileWorldProgress BuildProgress;
        public static event TileWorldProgress MergeProgress;

        public static void CallOnBuildComplete()
        {
            if (OnBuildComplete != null)
            {
                OnBuildComplete();
            }
        }

        public static void CallOnScatterProceduralComplete()
        {
            if (OnScatterProceduralComplete != null)
            {
                OnScatterProceduralComplete();
            }
        }

        public static void CallOnScatterPositionBasedComplete()
        {
            if (OnScatterPositionBasedComplete != null)
            {
                OnScatterPositionBasedComplete();
            }
        }

        public static void CallOnMergeComplete()
        {
            if (OnMergeComplete != null)
            {
                OnMergeComplete();
            }
        }

        public static void CallBuildProgress(float _progress)
        {
            if (BuildProgress != null)
            {
                BuildProgress(_progress);
            }
        }

        public static void CallMergeProgress(float _progress)
        {
            if (MergeProgress != null)
            {
                MergeProgress(_progress);
            }
        }

    }
}
