using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GC
{
    public class MyCameraHandler : MonoBehaviour
    {
		private static LayerMask IgnoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
		private static Vector3 CameraOffset = new Vector3(5f, 1, 0);
		private static Vector3 BaseFollowVector = new Vector3(0, 0, -1);
		public static MyCameraHandler Instance;

		private float M_FollowDistance = -0.25f;
		private Vector2 FollowVectorAngleOffset = new Vector2(0, 0);
		private Vector3 FollowVector = MyCameraHandler.BaseFollowVector;
		private GameObject TargetObject = null;
		private GameObject MainCamera = null;

		public Transform WorldMatrix
		{
			get
			{
				return this.MainCamera?.transform;
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
				this.M_FollowDistance = -Mathf.Max(0, value);
			}
		}


		private void Awake()
		{
			this.MainCamera = this.transform.GetChild(0).GetChild(0).gameObject;
			MyCameraHandler.Instance = this;
		}

		private void LateUpdate()
		{
			if (this.TargetObject == null) return;
			Transform target = this.TargetObject.transform;
			Vector3 offset = target.TransformDirection(MyCameraHandler.CameraOffset);
			this.WorldMatrix.rotation = target.rotation;
			this.WorldMatrix.position = target.TransformPoint(this.FollowVector * this.M_FollowDistance + offset);
		}

		internal void SetFollowedObject(GameObject target)
		{
			this.TargetObject = target;
		}

		internal void SetFollowVectorRotation(float lateral, float vertical)
		{
			//this.FollowVector = Quaternion.AngleAxis(lateral, new Vector3(0, 1, 0)) * MyCameraHandler.BaseFollowVector;
			//Debug.Log(vertical);
			//this.FollowVector = Quaternion.AngleAxis(10, new Vector3(1, 0, 0)) * this.FollowVector;
		}
	}
}
