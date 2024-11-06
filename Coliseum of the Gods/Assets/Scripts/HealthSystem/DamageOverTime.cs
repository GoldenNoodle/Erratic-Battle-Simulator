using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using static GC.IotaScripts.MyWeaponHandler;
using static GC.IotaScripts.MyWeaponHandler.MyWeaponInfo;

namespace GC.IotaScripts
{
	internal class MyDamageOverTimeSystem
	{
		[Serializable]
		public struct SerializedDamageOverTimeSystem
		{
			public uint DurationGameTicks;
			public double HealthLossPerTick;
			public double CurrentHealth;
			public double MovementSpeedMultiplier;
			public double JumpHeightMultiplier;
		}

		private static Dictionary<string, MyDamageOverTimeSystem> DamageOverTimeSystems = new Dictionary<string, MyDamageOverTimeSystem>();

		private bool Applicable = false;
		private readonly string Name;
		private readonly uint DurationGameTicks;
		private readonly double HealthLossPerTick;
		private readonly double CurrentHealth;
		private readonly double MovementSpeedMultiplier;
		private readonly double JumpHeightMultiplier;
		private readonly Func<MyHealthManager, bool> TickCallback;

		private uint RemainingTimeTicks;

		public static MyDamageOverTimeSystem FromJSON(string name, string json)
		{
			SerializedDamageOverTimeSystem weapon_info = JsonUtility.FromJson<SerializedDamageOverTimeSystem>(json);
			return new MyDamageOverTimeSystem(name, weapon_info);
		}

		public static void Initialize()
		{
			TextAsset file = Resources.Load("DamageOverTimeSystems", typeof(TextAsset)) as TextAsset;
			Dictionary<string, Dictionary<string, object>> _json = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(file.text);
			Dictionary<string, string> json = new Dictionary<string, string>();
			foreach (KeyValuePair<string, Dictionary<string, object>> pair in _json) json.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
			foreach (KeyValuePair<string, string> dot_info in json) MyDamageOverTimeSystem.DamageOverTimeSystems.Add(dot_info.Key, MyDamageOverTimeSystem.FromJSON(dot_info.Key, dot_info.Value));
		}

		public static MyDamageOverTimeSystem GetDoTSystem(string name)
		{
			return MyDamageOverTimeSystem.DamageOverTimeSystems.GetValueOrDefault(name, null);
		}

		private MyDamageOverTimeSystem(string name, SerializedDamageOverTimeSystem dot_info)
		{
			this.Name = name;
			this.DurationGameTicks = dot_info.DurationGameTicks;
			this.HealthLossPerTick = dot_info.HealthLossPerTick;
			this.CurrentHealth = dot_info.CurrentHealth;
			this.MovementSpeedMultiplier = dot_info.MovementSpeedMultiplier;
			this.JumpHeightMultiplier = dot_info.JumpHeightMultiplier;
			//this.TickCallback = tick_callback;
		}

		private MyDamageOverTimeSystem(MyDamageOverTimeSystem original)
		{
			this.Name = original.Name;
			this.DurationGameTicks = original.DurationGameTicks;
			this.HealthLossPerTick = original.HealthLossPerTick;
			this.CurrentHealth = original.CurrentHealth;
			this.MovementSpeedMultiplier = original.MovementSpeedMultiplier;
			this.JumpHeightMultiplier= original.JumpHeightMultiplier;
			this.TickCallback = original.TickCallback;
		}

		public bool Tick(MyHealthManager health_manager)
		{
			if (!this.Applicable) throw new InvalidOperationException("This DoT system is not applicable, try using \"MyDamageOverTimeSystem.Applyable\"");
			if (health_manager == null) return false;
			if (this.CurrentHealth < 0) health_manager.Damage(this.HealthLossPerTick);
			else health_manager.SetHealth(this.CurrentHealth);
			health_manager.AccumulateMovementSpeedMultiplier(this.MovementSpeedMultiplier);
			health_manager.AccumulateJumpHeightMultiplier(this.JumpHeightMultiplier);
			--this.RemainingTimeTicks;
			return this.RemainingTimeTicks == 0 || (this.TickCallback?.Invoke(health_manager) ?? false);
		}

		public MyDamageOverTimeSystem Applyable()
		{
			if (this.Applicable) throw new InvalidOperationException("This DoT system is already applied");
			MyDamageOverTimeSystem clone = new MyDamageOverTimeSystem(this);
			clone.RemainingTimeTicks = this.DurationGameTicks;
			clone.Applicable = true;
			return clone;
		}
	}
}
