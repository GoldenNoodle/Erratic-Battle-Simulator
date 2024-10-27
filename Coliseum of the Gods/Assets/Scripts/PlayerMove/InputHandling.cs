using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GC
{
    //Responsible for all functions dealing with appropriate actions due to inputs in-game
    public class InputHandling : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool jumpInput;
        public bool attackInput;

        PlayerControls inputActions;
        CameraHandler cameraHandler;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;

        Vector2 movementInput;
        Vector2 cameraInput;

        
        private void Awake()
        {
            //cameraHandler = CameraHandler.singleton; PlayerManager takes care of this so we dont need it but keep in case something goes wrong
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();

        }
        
        //Player Manager Handles this function: Keeping this here for personal use
        /*
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }
        */

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.Jump.performed += context => jumpInput = true;
                inputActions.PlayerActions.Attack.performed += context => attackInput = true;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleAttackInput(delta);
            
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
        
        private void HandleAttackInput(float delta)
        {
            inputActions.PlayerActions.Attack.performed += inputActions => attackInput = true;

            if (attackInput)
            {
                playerAttacker.HandleAttack(playerInventory.rightWeapon);
            }
        }
        
        
    }

}

