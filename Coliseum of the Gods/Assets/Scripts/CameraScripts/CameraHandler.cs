using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class CameraHandler : MonoBehaviour

        //Note that Transforms in Unity describes a game object component that we can determine its position, rotation, and scale
    {
        public Transform targetTransform; //target transform that camerea will go to
        public Transform cameraTransform; //transform that controls camera movement
        public Transform cameraPivotTransform; //transform that controls camera pivoting (turning on a swivel/rotate around pivot)
        private Transform myTransform; //transform of game object
        private Vector3 cameraTransformPosition; //Vector position of camera position
        private LayerMask ignoreLayers; //used for camera collision
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minPivot = -35;
        public float maxPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minCollisionOffset = 0.2f;



        private void Awake()
        {
            singleton = this;
            myTransform = transform; //my transform variable is equal to the transform of the game object
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            
        }

        //This function assigns targetPosition the value of the Lerp (linear interpolation) between myTransform.position and targetTransform.position
        //This function is called in Update() every frame which will cause the camera to follow the target transform.position (the player transform)
        public void FollowTarget(float delta) 
        {
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);

            //Using SmoothDamp instead of Lerp will make the camera less jittery
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);
        }


        //Allows camera to move using values recorded from the mouse
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            //Mathf.Clamp() will return the value,sontrained within the min and max
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot); //Clamp between the pivot angle and min/max pivot angles. Allows the camera to be stuck between two points of pivot so it cant go higher or lower than the defined angles
       
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        
        }

        private void HandleCameraCollision(float delta)
        {
            targetPosition = defaultPosition;
            //RayCast is a function in the Physics library that projects a ray into the scene and returns true or false if the targetwas hit
            //It is used for camera collisiton detection by using that ray to detect if any object is in its path
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            //SphereCast is similar to RayCast but is a sphere instead of a ray that comes around the camera. If an object collides with the sphere than it returns true, otherwise it returns false
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point); //returns the distance between point cameraPivotTransform and the hit point of the SphereCast
                targetPosition = -(distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minCollisionOffset)
            {
                targetPosition = -minCollisionOffset;
            }

            //could also use SmoothDamp instead like in the FollowTarget Function
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
