using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class MyCameraHandler : MonoBehaviour
    {
		private static LayerMask IgnoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
		private float M_FollowDistance = -0.25f;
		public static MyCameraHandler Instance;

		private GameObject TargetObject = null;

		public Transform WorldMatrix
		{
			get
			{
				return this.transform;
			}
		}

		public float FollowDistance
		{
			get
			{
				return this.M_FollowDistance;
			}
			set
			{
				this.M_FollowDistance = Mathf.Max(0, value);
			}
		}


		private void Awake()
		{
			MyCameraHandler.Instance = this;
		}

		private void LateUpdate()
		{
			if (this.TargetObject == null) return;
			Transform target = this.TargetObject.transform;
			Vector3 position = target.TransformPoint(new Vector3(0.5f, -1, -this.M_FollowDistance));
			this.transform.position = position;
			this.transform.forward = this.TargetObject.transform.forward;
		}

		internal void SetFollowedObject(GameObject target)
		{
			this.TargetObject = target;
		}
	}
}
