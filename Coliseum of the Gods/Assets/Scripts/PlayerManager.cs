using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GC.IotaScripts;


//Player Manager is reponsible for all updating code.
//This allows for more readibility as PlayerMovement, AnimationHandler, and InputHandling will only show code for their respective purposes

namespace GC
{
    public class PlayerManager : MonoBehaviour
    {

        InputHandling inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerMovement playerMovement;
        MyHealthManager HealthManager;
		MyWeaponHandler weaponHandler;

		//public bool isInteracting;


		private void Awake()
        {
            cameraHandler = CameraHandler.singleton;

        }

        void Start()
        {
            inputHandler = GetComponent<InputHandling>();
            anim = GetComponentInChildren<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
			cameraHandler = CameraHandler.singleton;
        }


        void Update()
        {
            //isInteracting = anim.GetBool("IsInteracting");
            
            float delta = Time.deltaTime;
            inputHandler.TickInput(delta);
            playerMovement.HandleMovement(delta);
		}
        
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
            
			if (this.inputHandler != null && this.inputHandler.attackInput)
			{
				this.weaponHandler?.Attack();
				this.inputHandler.attackInput = false;
			}
		}
        
        public void BindWeapon(GameObject weapon)
        {
            this.weaponHandler = weapon.GetComponent<MyWeaponHandler>();
        }
    }
}

