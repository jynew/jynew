using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeEyedGames.DecaliciousExample
{
    public class PickupWeapon : IInteract
    {
        public override void Interact()
        {
            Camera.main.GetComponent<ShootDecal>().enabled = true;
            Destroy(gameObject);
        }
    }
}