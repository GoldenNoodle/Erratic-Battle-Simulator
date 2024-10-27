using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyHealthManager : MonoBehaviour
	{
		private static GameObject Camera;

		private GameObject HealthBar;
		private LineRenderer HealthBarLine;
		private MeshRenderer MeshRenderer;
		private List<MyDamageOverTimeSystem> DamageOverTimeSystems = new List<MyDamageOverTimeSystem>();

		internal double CurrentHealth { get; private set; } = 100;
		internal double MaxHealth { get; private set; } = 100;
		internal double AccumulatedMovementSpeedMultiplier { get; private set; } = 1;
		internal double AccumulatedJumpHeightMultiplier { get; private set; } = 1;

		private void Start()
		{
			this.HealthBar = Enumerable.Range(0, this.transform.childCount).Select(index => this.transform.GetChild(index).gameObject).FirstOrDefault(child => child.name == "HealthBar");
			this.HealthBarLine = this.HealthBar?.GetComponent<LineRenderer>();
			this.MeshRenderer = this.GetComponent<MeshRenderer>();
			if (this.HealthBarLine == null) throw new NullReferenceException("Failed to find the child or Line object with the expected name \"HealthBar\"");
			MyHealthManager.Camera = MyHealthManager.Camera ?? GameObject.Find("MainCamera");
		}

		private void Update()
		{
			this.AccumulatedMovementSpeedMultiplier = 1;
			this.AccumulatedJumpHeightMultiplier = 1;
			this.DamageOverTimeSystems.RemoveAll(dot => dot.Tick(this));

			if (this.HealthBarLine != null && MyHealthManager.Camera != null)
			{
				Vector3 direction = (MyHealthManager.Camera.transform.position - this.transform.position).normalized;
				direction.y = 0;
				direction = Quaternion.AngleAxis(90, this.transform.up) * direction;
				this.HealthBar.transform.rotation = Quaternion.LookRotation(direction);
				this.HealthBarLine.SetPosition(1, new Vector3(0, 0, (float) (this.CurrentHealth / this.MaxHealth)));
			}

			if (this.CurrentHealth == 0)
			{
				this.gameObject.SetActive(false);
			}
		}

		internal void Kill()
		{
			this.CurrentHealth = 0;
		}

		internal void SetHealth(double health)
		{
			this.CurrentHealth = Math.Clamp(health, 0, this.MaxHealth);
		}

		internal void SetMaxHealth(double max_health)
		{
			this.MaxHealth = Math.Max(max_health, 0);
		}

		internal void AccumulateMovementSpeedMultiplier(double multiplier)
		{
			this.AccumulatedMovementSpeedMultiplier *= Math.Max(0, multiplier);
		}

		internal void AccumulateJumpHeightMultiplier(double multiplier)
		{
			this.AccumulatedJumpHeightMultiplier *= Math.Max(0, multiplier);
		}

		internal void ApplyDamageOverTime(string DoT_system_name)
		{
			MyDamageOverTimeSystem dot = MyDamageOverTimeSystem.GetDoTSystem(DoT_system_name);
			if (dot == null) this.DamageOverTimeSystems.Add(dot.Applyable());
		}

		internal double Damage(double health)
		{
			this.CurrentHealth = Math.Clamp(this.CurrentHealth - health, 0, this.MaxHealth);
			return this.CurrentHealth;
		}
	}
}