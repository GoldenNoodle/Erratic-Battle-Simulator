using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

namespace GC.IotaScripts
{
	internal class MyDamageOverTimeSystem
	{
		private static Dictionary<string, MyDamageOverTimeSystem> RegisteredDoTSystems = new Dictionary<string, MyDamageOverTimeSystem>();

		private bool Applicable = false;
		private readonly string Name;
		private readonly uint DurationGameTicks;
		private readonly double HealthLossPerTick;
		private readonly double CurrentHealth;
		private readonly double MovementSpeedMultiplier;
		private readonly double JumpHeightMultiplier;
		private readonly Func<MyHealthManager, bool> TickCallback;

		private uint RemainingTimeTicks;

		public static MyDamageOverTimeSystem RegisterNewDoTSystem(string name, uint duration_game_ticks = 0, double health_loss_per_tick = 0, double current_health = -1, double movement_speed_multiplier = 1, double jump_height_multiplier = 1, Func<MyHealthManager, bool> tick_callback = null)
		{
			if (MyDamageOverTimeSystem.RegisteredDoTSystems.ContainsKey(name)) throw new AmbiguousImplementationException("Specified DoT name is alreay implemented");
			MyDamageOverTimeSystem dot = new MyDamageOverTimeSystem(name, duration_game_ticks, health_loss_per_tick, current_health, movement_speed_multiplier, jump_height_multiplier, tick_callback);
			MyDamageOverTimeSystem.RegisteredDoTSystems.Add(name, dot);
			return dot;
		}

		public static MyDamageOverTimeSystem GetDoTSystem(string name)
		{
			return MyDamageOverTimeSystem.RegisteredDoTSystems.GetValueOrDefault(name, null);
		}

		private MyDamageOverTimeSystem(string name, uint duration_game_ticks, double health_loss_per_tick, double current_health, double movement_speed_multiplier, double jump_height_multiplier, Func<MyHealthManager, bool> tick_callback)
		{
			this.Name = name;
			this.DurationGameTicks = duration_game_ticks;
			this.HealthLossPerTick = health_loss_per_tick;
			this.CurrentHealth = current_health;
			this.MovementSpeedMultiplier = movement_speed_multiplier;
			this.JumpHeightMultiplier = jump_height_multiplier;
			this.TickCallback = tick_callback;
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
			MyDamageOverTimeSystem clone = new MyDamageOverTimeSystem(this.Name, this.DurationGameTicks, this.HealthLossPerTick, this.CurrentHealth, this.MovementSpeedMultiplier, this.JumpHeightMultiplier, this.TickCallback);
			clone.RemainingTimeTicks = this.DurationGameTicks;
			clone.Applicable = true;
			return clone;
		}
	}

	internal partial class DamageOverTimeDefinitions
	{ 
		
	}
}
