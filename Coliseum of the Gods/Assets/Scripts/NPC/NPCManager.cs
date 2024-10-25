using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.IotaScripts
{
	public class NPCManager : MonoBehaviour
	{
		private bool Jumped = false;
		private GameObject Player;
		private Rigidbody RigidBody;
		private Vector3 LastPosition;
		private DateTime LastScanTime;


		private void Start()
		{
			this.Player = GameObject.Find("Player");
			this.RigidBody = this.GetComponent<Rigidbody>();
			this.LastPosition = this.transform.position;
		}

		private void Update()
		{
			DateTime now = DateTime.UtcNow;
			double ellapsed_seconds = (now - this.LastScanTime).TotalSeconds;

			if (this.Player == null || this.RigidBody == null || (this.Jumped && ellapsed_seconds < 0.25)) return;
			Vector3 direction = this.Player.transform.position - this.transform.position;
			direction.y = 0;
			float distance = direction.magnitude;
			if (distance == 0) return;
			this.transform.rotation = Quaternion.LookRotation(direction);
			this.Jumped = false;
			bool player_reached = distance <= 1.5f;
			Vector3 velocity = this.transform.forward * 2.5f;
			velocity.y = this.RigidBody.velocity.y;
			velocity = (player_reached) ? new Vector3(0, this.RigidBody.velocity.y, 0) : velocity;

			if (!player_reached && ellapsed_seconds >= 0.25)
			{
				if (Vector3.Distance(this.transform.position, this.LastPosition) <= 1e-3)
				{
					velocity = new Vector3(0, 5, 0);
					this.Jumped = true;
				}

				this.LastScanTime = now;
				this.LastPosition = this.transform.position;
			}

			this.RigidBody.velocity = velocity;
		}
	}
}