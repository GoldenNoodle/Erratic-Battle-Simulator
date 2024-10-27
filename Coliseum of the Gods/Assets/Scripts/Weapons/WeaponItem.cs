using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Attack Animation")]
        public string attack_melee_right;



    }
}