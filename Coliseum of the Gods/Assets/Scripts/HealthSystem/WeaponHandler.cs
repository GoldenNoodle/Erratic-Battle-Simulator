using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyWeaponHandler : MonoBehaviour
	{
		internal sealed class MyWeaponInfo
		{
			[Serializable]
			public struct SerializedWeaponInfo
			{
				public float WeaponReachDistanceMeters;
				public double BaseDamage;
				public double CooldownTimeSeconds;
				public string ProjectileSpawner;
				public string DamageOverTimeSystem;
			}

			public float WeaponReachDistanceMeters { get; private set; }
			public double BaseDamage { get; private set; }
			public double CooldownTimeSeconds { get; private set; }
			public string ProjectileSpawner { get; private set; }
			public string DamageOverTimeSystem { get; private set; }
			public string Name { get; private set; }

			public static MyWeaponInfo FromJSON(string name, string json)
			{
				SerializedWeaponInfo weapon_info = JsonUtility.FromJson<SerializedWeaponInfo>(json);
				return new MyWeaponInfo(name, weapon_info);
			}

			private MyWeaponInfo(string name, SerializedWeaponInfo weapon_info)
			{
				this.WeaponReachDistanceMeters = weapon_info.WeaponReachDistanceMeters;
				this.BaseDamage = weapon_info.BaseDamage;
				this.CooldownTimeSeconds = weapon_info.CooldownTimeSeconds;
				this.ProjectileSpawner = weapon_info.ProjectileSpawner;
				this.DamageOverTimeSystem = weapon_info.DamageOverTimeSystem;
				this.Name = name;
			}
		}

		internal readonly static Dictionary<string, MyWeaponInfo> WeaponInfos = new Dictionary<string, MyWeaponInfo>();

		private DateTime LastAttackTime = DateTime.MinValue;
		private MyWeaponInfo ActiveWeaponInfo = null;

		public string InitialActiveWeapon = null;

		public string ActiveWeapon
		{
			get
			{
				return this.ActiveWeaponInfo?.Name;
			}

			set
			{
				this.ActiveWeaponInfo = MyWeaponHandler.GetWeaponInfo(value);
			}
		}
		
		internal static void Initialize()
		{
			TextAsset file = Resources.Load("WeaponInfo", typeof(TextAsset)) as TextAsset;
			Dictionary<string, Dictionary<string, object>> _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(file.text);
			Dictionary<string, string> json = new Dictionary<string, string>();
			foreach (KeyValuePair<string, Dictionary<string, object>> pair in _json) json.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
			foreach (KeyValuePair<string, string> weapon_info in json) MyWeaponHandler.WeaponInfos.Add(weapon_info.Key, MyWeaponInfo.FromJSON(weapon_info.Key, weapon_info.Value));
		}

		internal static MyWeaponInfo GetWeaponInfo(string name)
		{
			return MyWeaponHandler.WeaponInfos.GetValueOrDefault(name, null);
		}

		private void Start()
		{
			this.ActiveWeapon = this.InitialActiveWeapon;
		}

		public void Attack()
		{
			if (this.ActiveWeaponInfo == null) return;
			DateTime now = DateTime.UtcNow;
			double ellapsed_seconds = (now - this.LastAttackTime).TotalSeconds;
			if (ellapsed_seconds < this.ActiveWeaponInfo.CooldownTimeSeconds) return;
			RaycastHit hit;
			bool attack_landed = Physics.CapsuleCast(this.gameObject.transform.position, this.gameObject.transform.TransformPoint(new Vector3(0.1f, 0, 0)), 0.25f, this.gameObject.transform.forward, out hit, this.ActiveWeaponInfo.WeaponReachDistanceMeters);
			if (!attack_landed) return;
			this.LastAttackTime = now;
			GameObject target = hit.rigidbody?.gameObject;
			MyHealthManager target_health = target?.GetComponent<MyHealthManager>();
			if (target_health == null) return;
			target_health.Damage(this.ActiveWeaponInfo.BaseDamage);
			if (this.ActiveWeaponInfo.DamageOverTimeSystem != null && this.ActiveWeaponInfo.DamageOverTimeSystem.Length > 0) target_health.ApplyDamageOverTime(this.ActiveWeaponInfo.DamageOverTimeSystem);
		}
	}
}