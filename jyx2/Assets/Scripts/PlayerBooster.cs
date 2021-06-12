using HanSquirrel.ResourceManager;
using UnityEngine;

public class PlayerBooster : MonoBehaviour
{
    private void Awake()
    {
        //实例化LevelMaster
        MapRole player = Jyx2ResourceHelper.CreatePrefabInstance(ConStr.Player).GetComponent<MapRole>();
        player.name = "Player";
        player.transform.SetParent(transform, false);
        player.transform.localPosition = Vector3.zero;
    }
}