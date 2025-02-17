using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    //Responsible for all functions that handle animation, updating animations
    //Ex: Walk, Jump, Attack
    public class AnimationHandler : MonoBehaviour
    {
        PlayerManager playerManager;
        InputHandling inputHandler;
        PlayerMovement playerMovement;
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;


        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandling>();
            playerMovement = GetComponentInParent<PlayerMovement>();
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");

        }

        public void UpdateAnimatorValues(float vertMove, float horizMove)
        {
            #region Vertical
            float vert = 0;
            if (vertMove > 0 && vertMove < 0.55f)
            {
                vert = 0.5f;
            }
            else if (vertMove > 0.55f)
            {
                vert = 1;
            }
            else if (vertMove < 0 && vertMove > -0.55f)
            {
                vert = -0.5f;
            }
            else if (vertMove < -0.55f)
            {
                vert = -1;
            }
            else
            {
                vert = 0;
            }
            #endregion

            #region Horizontal
            float horiz = 0;
            if (horizMove > 0 && horizMove < 0.55f)
            {
                horiz = 0.5f;
            }
            else if (horizMove > 0.55f)
            {
                horiz = 1;
            }
            else if (horizMove < 0 && horizMove > -0.55f)
            {
                horiz = -0.5f;
            }
            else if (horizMove < -0.55f)
            {
                horiz = -1;
            }
            else
            {
                horiz = 0;
            }
            #endregion

            anim.SetFloat(vertical, vert, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, horiz, 0.1f, Time.deltaTime);


        }

        
        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }
        

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotate()
        {
            canRotate = false;
        }

        //Commented out for now, but will need for future animation implementations
        
        /*
        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            playerMovement.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerMovement.rigidbody.velocity = velocity;
        }
        */
       
    }
}
