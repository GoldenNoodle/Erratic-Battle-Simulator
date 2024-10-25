using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
	public class MyPlayerMovement : MonoBehaviour
	{
		private Vector2 MovementInput;
		private Vector2 CameraInput;
		private PlayerControls InputActions;

		internal float MovementSpeedMetersPerSecond = 5;
		internal MyPlayerManager PlayerManager;

		private void Start()
		{
			this.PlayerManager = this.GetComponent<MyPlayerManager>();
		}

		private void Update()
		{
			if (this.MovementInput.magnitude == 0) return;
			float offset = this.MovementSpeedMetersPerSecond * Time.deltaTime;
			Vector2 direction = this.MovementInput.normalized * offset;
			Vector3 direction3d = new Vector3(direction.x, 0, direction.y);
			this.transform.position += direction3d;
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
