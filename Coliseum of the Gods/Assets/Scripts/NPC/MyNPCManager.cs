using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyNPCManager : MonoBehaviour
	{
		public static double MaxCooldownDeltaSeconds = 1;

		private bool Jumped = false;
		private ulong LocalGameTick = 0;
		private Vector3 LastPosition;
		private DateTime LastScanTime;
		private GameObject TargetHealtyObject = null;
		private Rigidbody RigidBody;
		private MyHealthManager HealthManager;
		private MyWeaponHandler WeaponHandler;
		private System.Random Randomer = new System.Random();


		private void Start()
		{
			this.RigidBody = this.GetComponent<Rigidbody>();
			this.HealthManager = this.GetComponent<MyHealthManager>();
			this.WeaponHandler = this.GetComponent<MyWeaponHandler>();
			this.LastPosition = this.transform.position;
		}

		private void Update()
		{
			if (this.LocalGameTick % 10 == 0) this.UpdateNearestHealthObject();
			DateTime now = DateTime.UtcNow;
			double ellapsed_seconds = (now - this.LastScanTime).TotalSeconds;
			if (this.TargetHealtyObject == null || this.RigidBody == null || (this.Jumped && ellapsed_seconds < 0.25)) return;
			Vector3 direction = this.TargetHealtyObject.transform.position - this.transform.position;
			direction.y = 0;
			float distance = direction.magnitude;
			if (distance == 0) return;
			this.transform.rotation = Quaternion.LookRotation(direction);
			this.Jumped = false;
			bool target_reached = distance <= (this.WeaponHandler?.ActiveWeaponInfo?.WeaponReachDistanceMeters ?? double.PositiveInfinity);
			Vector3 velocity = this.transform.forward * (float) (2.5d * this.HealthManager.AccumulatedMovementSpeedMultiplier);
			velocity.y = this.RigidBody.velocity.y;
			velocity = (target_reached) ? new Vector3(0, this.RigidBody.velocity.y, 0) : velocity;

			if (!target_reached && ellapsed_seconds >= 0.25)
			{
				if (Vector3.Distance(this.transform.position, this.LastPosition) <= 1e-3)
				{
					velocity = new Vector3(0, (float) (5d * this.HealthManager.AccumulatedJumpHeightMultiplier), 0);
					this.Jumped = true;
				}

				this.LastScanTime = now;
				this.LastPosition = this.transform.position;
			}
			else if (target_reached && this.WeaponHandler != null) this.StartCoroutine(this.AttackNearest());

			this.RigidBody.velocity = velocity;
			++this.LocalGameTick;
		}

		private IEnumerator AttackNearest()
		{
			if (this.WeaponHandler?.ActiveWeaponInfo != null)
			{
				double wait = this.Randomer.NextDouble() * MyNPCManager.MaxCooldownDeltaSeconds;
				double cooldown = this.WeaponHandler.ActiveWeaponInfo.CooldownTimeSeconds;
				yield return new WaitForSeconds((float) (cooldown + wait));
				this.WeaponHandler.Attack();
			}
		}

		internal void UpdateNearestHealthObject()
		{
			this.TargetHealtyObject = FindObjectsOfType<MyHealthManager>().Where(comp => comp.gameObject != this.gameObject && comp.enabled && comp.gameObject.activeInHierarchy).Select(obj => obj.gameObject).OrderBy(obj => Vector3.Distance(obj.transform.position, this.gameObject.transform.position)).First();
		}
	}
}