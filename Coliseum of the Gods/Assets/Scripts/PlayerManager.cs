using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class MyPlayerManager : MonoBehaviour
    {
		void Start()
		{
			MyCameraHandler.Instance.SetFollowedObject(this.gameObject);
		}

		void Update()
		{
			
		}
	}
}

