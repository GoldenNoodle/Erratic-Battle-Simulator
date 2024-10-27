using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyWeaponHandler : MonoBehaviour
	{
		private DateTime LastAttackTime = DateTime.MinValue;
		private GameObject PlayerObject;

		internal float WeaponReachDistanceMeters;

		internal double BaseDamage;
		internal double CooldownTimeSeconds;
		internal string DamageOverTimeSystem { get; private set; }

		private void Start()
		{
			this.DamageOverTimeSystem = Variables.Object(this.gameObject).Get<string>("DamageOverTimeSystem");
			this.BaseDamage = Variables.Object(this.gameObject).Get<float>("BaseDamage");
			this.CooldownTimeSeconds = Variables.Object(this.gameObject).Get<float>("CooldownTimeSeconds");
			this.WeaponReachDistanceMeters = Variables.Object(this.gameObject).Get<float>("WeaponReachDistanceMeters");

			Transform src = this.transform;

			while (src != null && this.PlayerObject == null)
			{
				if (src.gameObject.GetComponent<MyHealthManager>() != null) this.PlayerObject = src.gameObject;
				src = src.parent;
			}

			this.PlayerObject?.GetComponent<PlayerManager>()?.BindWeapon(this.gameObject);
		}

		public void Attack()
		{
			if (this.PlayerObject == null) return;
			RaycastHit hit;
			bool attack_landed = Physics.CapsuleCast(this.PlayerObject.transform.position, this.PlayerObject.transform.TransformPoint(new Vector3(0.1f, 0, 0)), 0.25f, this.PlayerObject.transform.forward, out hit, this.WeaponReachDistanceMeters);
			Debug.Log($"ATTACK LANDED={attack_landed}");
			if (!attack_landed) return;
			GameObject target = hit.rigidbody?.gameObject;
			MyHealthManager target_health = target?.GetComponent<MyHealthManager>();
			if (target_health == null) return;
			target_health.Damage(this.BaseDamage);
			if (this.DamageOverTimeSystem != null && this.DamageOverTimeSystem.Length > 0) target_health.ApplyDamageOverTime(this.DamageOverTimeSystem);
		}
	}
}