using GC.IotaScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        MyHealthManager healthManager;
        Transform cameraObject;
        InputHandling inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimationHandler animationHandler;


        public new Rigidbody rigidbody;
        public GameObject camera; //code referenced as a normalCamera and a lockOnCamera but this game will only need a normal camera

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        public bool isInteracting;

        public bool isInAir;

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            healthManager = GetComponent<MyHealthManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandling>();
            animationHandler = GetComponentInChildren<AnimationHandler>(); //children because its going on player model under player game object
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animationHandler.Initialize();
            Application.targetFrameRate = 60;

        }
        
        public void Update()
        {
            float delta = Time.deltaTime;
            

            inputHandler.TickInput(delta);
            HandleMovement(delta);
            
        }
        

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical; // moves up and down
            targetDir += cameraObject.right * inputHandler.horizontal; //moves left and right


            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rotSpeed = (float) (rotationSpeed * healthManager.AccumulatedMovementSpeedMultiplier);

            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, targetRot, rotSpeed * delta);

            myTransform.rotation = targetRotation;

        }

        public void HandleMovement(float delta)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical; // moves up and down
            moveDirection += cameraObject.right * inputHandler.horizontal; //moves left and right
            moveDirection.Normalize();
            moveDirection.y = 0; //freezes movement in y direction so we dont randomy levitate off the ground


            float speed = (float) (movementSpeed * healthManager.AccumulatedMovementSpeedMultiplier);
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            bool jump = this.inputHandler.jumpInput && Physics.Raycast(rigidbody.transform.position, Vector3.down, 1f);
            projectedVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = (jump) ? new Vector3(0, (float) (5d * healthManager.AccumulatedJumpHeightMultiplier), 0) : projectedVelocity;
            this.inputHandler.jumpInput = false;
            
            animationHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);


            if (animationHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        
        #endregion



    }
}
