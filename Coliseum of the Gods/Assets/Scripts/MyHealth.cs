using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GC
{
	public class MyHealthManager : MonoBehaviour
	{
		private static GameObject Camera;

		private GameObject ThisGameObject;
		private GameObject HealthBar;
		private LineRenderer HealthBarLine;
		private MeshRenderer MeshRenderer;

		internal float CurrentHealth { get; private set; } = 100;
		internal float MaxHealth { get; private set; } = 100;

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
			if (this.HealthBarLine == null || MyHealthManager.Camera == null) return;
			Vector3 direction = (MyHealthManager.Camera.transform.position - this.transform.position).normalized;
			direction.y = 0;
			direction = Quaternion.AngleAxis(90, this.transform.up) * direction;
			this.HealthBar.transform.rotation = Quaternion.LookRotation(direction);
			this.HealthBarLine.SetPosition(1, new Vector3(0, 0, this.CurrentHealth / this.MaxHealth));

			if (this.CurrentHealth == 0)
			{

			}
		}

		internal void Kill()
		{
			this.CurrentHealth = 0;
		}

		internal void SetHealth(float health)
		{
			this.CurrentHealth = Mathf.Clamp(health, 0, this.MaxHealth);
		}

		internal void SetMaxHealth(float max_health)
		{
			this.MaxHealth = Math.Max(max_health, 0);
		}

		internal float Damage(float health)
		{
			this.CurrentHealth = Mathf.Clamp(this.CurrentHealth - health, 0, this.MaxHealth);
			return this.CurrentHealth;
		}
	}
}