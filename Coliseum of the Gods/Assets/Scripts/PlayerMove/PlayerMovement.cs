using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GC
{
	public class MyPlayerMovement : MonoBehaviour
	{
		private Vector2 MovementInput;
		private Vector2 CameraInput;
		private Vector2 VirtualCursor = new Vector2(0, 0);
		private PlayerControls InputActions;
		private MyAnimationHandler AnimationHandler;

		internal float MovementSpeedMetersPerSecond = 5;
		internal float CameraRotationSpeedRotationsPerSecond = 360f;
		internal MyPlayerManager PlayerManager;

		private void Start()
		{
			this.PlayerManager = this.GetComponent<MyPlayerManager>();
			this.AnimationHandler = this.GetComponentInChildren<MyAnimationHandler>();
			MyCameraHandler.Instance.SetFollowedObject(this.gameObject);
			this.AnimationHandler.Initialize();
		}

		private void Update()
		{
			// Look left/right
			float multiplier = this.CameraRotationSpeedRotationsPerSecond * Time.deltaTime * Configuration.LateralMouseMoveMultiplier;
			float angle = this.CameraInput.x * multiplier;
			this.transform.forward = Quaternion.AngleAxis(angle, this.transform.up) * this.transform.forward;
			this.transform.right = Quaternion.AngleAxis(angle, this.transform.up) * this.transform.right;

			// Look up/down
			multiplier = this.CameraRotationSpeedRotationsPerSecond * Time.deltaTime * Configuration.VerticalMouseMoveMultiplier;
			angle = Math.Clamp(this.CameraInput.y * multiplier, -45, 45);
			MyCameraHandler.Instance.SetFollowVectorRotation(0, angle);

			// Move around
			this.AnimationHandler.UpdateAnimatorValues(Mathf.Clamp01(Mathf.Abs(this.MovementInput.x) + Mathf.Abs(this.MovementInput.y)), 0);
			if (this.MovementInput.magnitude == 0) return;
			float offset = this.MovementSpeedMetersPerSecond * Time.deltaTime;
			Vector2 direction = this.MovementInput.normalized * offset;
			Vector3 direction3d = new Vector3(direction.x, 0, direction.y);
			this.transform.position += this.transform.TransformDirection(direction3d);
		}

		public void OnEnable()
		{
			if (this.InputActions == null)
			{
				this.InputActions = new PlayerControls();
				this.InputActions.PlayerMovement.Movement.performed += inputActions => this.MovementInput = inputActions.ReadValue<Vector2>();
				this.InputActions.PlayerMovement.Camera.performed += i => this.CameraInput = i.ReadValue<Vector2>();
			}

			this.InputActions.Enable();
		}

		private void OnDisable()
		{
			this.InputActions.Disable();
		}
	}
}
