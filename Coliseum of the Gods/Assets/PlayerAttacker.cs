using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class PlayerAttacker : MonoBehaviour
    {


        AnimationHandler animationHandler;

        private void Awake()
        {
            animationHandler = GetComponentInChildren<AnimationHandler>();
        }

        public void HandleAttack(WeaponItem weapon)
        {
            //Attack Animation may not work due to problem with isInteracting variable inside PlayTargetnimation() function inside AnimationHandler script
            animationHandler.PlayTargetAnimation(weapon.attack_melee_right, true);
        }
    }
}

