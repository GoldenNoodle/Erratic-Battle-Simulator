using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyWeaponHandler : MonoBehaviour
	{
		internal sealed class MyWeaponParentInfo
		{
			public string ParentName { get; private set; }
			public string ParentPath { get; private set; }
			public Vector3 ModelOffset { get; private set; }
			public Vector3 ModelRotation { get; private set; }
			public Vector3 ModelScale { get; private set; }

			public MyWeaponParentInfo(string parent_name, string parent_path, Vector3 model_offset, Vector3 model_rotation, Vector3 model_scale)
			{
				this.ParentName = parent_name;
				this.ParentPath = parent_path;
				this.ModelOffset = model_offset;
				this.ModelRotation = model_rotation;
				this.ModelScale = model_scale;
			}
		}

		internal sealed class MyWeaponInfo
		{

			public float WeaponReachDistanceMeters { get; private set; }
			public double BaseDamage { get; private set; }
			public double CooldownTimeSeconds { get; private set; }
			public string ProjectileSpawner { get; private set; }
			public string DamageOverTimeSystem { get; private set; }
			public string ModelName { get; private set; }
			public Dictionary<string, List<MyWeaponParentInfo>> ModelParents { get; private set; } = new Dictionary<string, List<MyWeaponParentInfo>>();
			public string Name { get; private set; }

			public static MyWeaponInfo FromJSON(string name, JObject json)
			{
				if (json == null) throw new NullReferenceException("JSON input was null");
				MyWeaponInfo weapon_info = new MyWeaponInfo();
				weapon_info.Name = name;
				weapon_info.WeaponReachDistanceMeters = json.GetValue("WeaponReachDistanceMeters").Value<float>();
				weapon_info.BaseDamage = json.GetValue("BaseDamage").Value<double>();
				weapon_info.CooldownTimeSeconds = json.GetValue("CooldownTimeSeconds").Value<double>();
				weapon_info.ProjectileSpawner = json.GetValue("ProjectileSpawner").Value<string>();
				weapon_info.DamageOverTimeSystem = json.GetValue("DamageOverTimeSystem").Value<string>();
				weapon_info.ModelName = json.GetValue("ModelName").Value<string>();
				JObject model_parents = (JObject) json.GetValue("ModelParents");
				
				foreach (KeyValuePair<string, JToken> pair in model_parents)
				{
					JArray parents = (JArray) pair.Value;
					List<MyWeaponParentInfo> parent_infos = new List<MyWeaponParentInfo>();

					foreach (JToken parent in parents)
					{
						JObject model_parent = (JObject) parent;
						string parent_path = model_parent.GetValue("Parent").Value<string>();
						JArray j_model_offset = (JArray) model_parent.GetValue("ModelOffset");
						JArray j_model_rotation = (JArray) model_parent.GetValue("ModelRotation");
						JArray j_model_scale = (JArray) model_parent.GetValue("ModelScale");
						float[] model_offset = j_model_offset.Select(token => token.Value<float>()).ToArray();
						float[] model_rotation = j_model_rotation.Select(token => token.Value<float>()).ToArray();
						float[] model_scale = j_model_scale.Select(token => token.Value<float>()).ToArray();
						if (model_offset.Length != 3) throw new ArgumentException("The specified model offset is invalid");
						if (model_rotation.Length != 3) throw new ArgumentException("The specified model rotation is invalid");
						if (model_scale.Length != 3) throw new ArgumentException("The specified model scale is invalid");
						parent_infos.Add(new MyWeaponParentInfo(pair.Key, parent_path, new Vector3(model_offset[0], model_offset[1], model_offset[2]), new Vector3(model_rotation[0], model_rotation[1], model_rotation[2]), new Vector3(model_scale[0], model_scale[1], model_scale[2])));
					}

					weapon_info.ModelParents.Add(pair.Key, parent_infos);
				}
				
				return weapon_info;
			}
		}

		internal readonly static Dictionary<string, MyWeaponInfo> WeaponInfos = new Dictionary<string, MyWeaponInfo>();
		internal readonly static List<MyWeaponHandler> Instances = new List<MyWeaponHandler>();

		private DateTime LastAttackTime = DateTime.MinValue;

		public string InitialActiveWeapon = null;
		public string PrefabObjectName = null;

		internal MyWeaponInfo ActiveWeaponInfo { get; private set; } = null;

		public string ActiveWeapon
		{
			get
			{
				return this.ActiveWeaponInfo?.Name;
			}

			set
			{
				string prefab_name;
				GameObject prefab = this.gameObject.GetChildByName(this.PrefabObjectName);
				List<MyWeaponParentInfo> parent_infos;

				if (this.ActiveWeaponInfo != null)
				{
					prefab_name = this.ActiveWeaponInfo.ModelName;
					parent_infos = this.ActiveWeaponInfo.ModelParents.GetValueOrDefault(this.PrefabObjectName);

					if (parent_infos != null)
					{
						foreach (string prefab_path in parent_infos.Select(parent => parent.ParentPath))
						{
							GameObject parent = prefab.GetChildByPath(prefab_path.Split('/', '\\'));
							foreach (GameObject child in parent.GetAllChildren()) Destroy(child);
						}
					}
				}

				this.ActiveWeaponInfo = MyWeaponHandler.GetWeaponInfo(value);
				if (this.ActiveWeaponInfo == null) return;
				prefab_name = this.ActiveWeaponInfo.ModelName;
				parent_infos = this.ActiveWeaponInfo.ModelParents.GetValueOrDefault(this.PrefabObjectName);
				if (parent_infos == null) return;

				foreach (MyWeaponParentInfo parent_info in parent_infos)
				{
					GameObject parent = prefab.GetChildByPath(parent_info.ParentPath.Split("/", '\\'));
					if (parent ==  null) throw new NullReferenceException($"The parent path specified by \"{parent_info.ParentPath}\" is NULL");
					GameObject source = Resources.Load<GameObject>(this.ActiveWeaponInfo.ModelName);
					GameObject model = Instantiate(source, parent.transform);
					model.transform.SetLocalPositionAndRotation(parent_info.ModelOffset, Quaternion.Euler(parent_info.ModelRotation));
					model.transform.localScale = parent_info.ModelScale;
				}
			}
		}
		
		internal static void Initialize()
		{
			TextAsset file = Resources.Load("WeaponInfo", typeof(TextAsset)) as TextAsset;
			Dictionary<string, JObject> json = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(file.text);
			foreach (KeyValuePair<string, JObject> weapon_info in json) MyWeaponHandler.WeaponInfos.Add(weapon_info.Key, MyWeaponInfo.FromJSON(weapon_info.Key, weapon_info.Value));
		}

		internal static MyWeaponInfo GetWeaponInfo(string name)
		{
			return (name == null) ? null : MyWeaponHandler.WeaponInfos.GetValueOrDefault(name, null);
		}

		private void Awake()
		{
			MyWeaponHandler.Instances.Add(this);
		}

		private void Start()
		{
			this.ActiveWeapon = this.InitialActiveWeapon;
		}

		private void OnDisable()
		{
			MyWeaponHandler.Instances.Remove(this);
		}

		private void OnEnable()
		{
			MyWeaponHandler.Instances.Add(this);
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