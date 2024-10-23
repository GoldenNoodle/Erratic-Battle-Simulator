using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class AnimationHandler : MonoBehaviour
    {
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
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

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotate()
        {
            canRotate = false;
        }
    }
}
