using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class MyCameraHandler : MonoBehaviour
    {
		private static LayerMask IgnoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);

		public static MyCameraHandler Instance;

		private float FollowDistanceMeters = 10;
		private GameObject TargetObject = null;


		private void Awake()
		{
			MyCameraHandler.Instance = this;
		}

		private void LateUpdate()
		{
			if (this.TargetObject == null) return;
			Transform target = this.TargetObject.transform;
			Vector3 position = target.TransformPoint(new Vector3(this.FollowDistanceMeters, 0, 0));
			this.transform.position = position;
		}

		internal void SetFollowedObject(GameObject target)
		{
			this.TargetObject = target;
		}
	}
}
