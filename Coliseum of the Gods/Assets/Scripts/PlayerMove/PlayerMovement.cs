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
		private PlayerControls InputActions;
		private MyAnimationHandler AnimationHandler;

		internal float MovementSpeedMetersPerSecond = 5;
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
			Vector2 mouse_pos = Mouse.current.position.ReadValue();
			double relative_x = ((double) mouse_pos.x) / Screen.width - 0.5d;
			double relative_y = ((double) mouse_pos.y) / Screen.height - 0.5d;

			if (relative_x > 0.125d || relative_x < -0.125d)
			{
				Vector3 forward = this.transform.forward;
				forward = Quaternion.AngleAxis((float) relative_x, this.transform.up) * forward;
				this.transform.forward = forward;
			}

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
