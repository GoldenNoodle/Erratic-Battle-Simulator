using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GC
{
	public class MyHealthManager : MonoBehaviour
	{
		private GameObject ThisGameObject;
		private GameObject HealthBar;
		private LineRenderer HealthBarLine;

		internal float CurrentHealth = 100;
		internal float MaxHealth = 100;

		private void Start()
		{
			this.HealthBar = Enumerable.Range(0, this.transform.childCount).Select(index => this.transform.GetChild(index).gameObject).FirstOrDefault(child => child.name == "HealthBar");
			this.HealthBarLine = this.HealthBar?.GetComponent<LineRenderer>();
		}

		private void Update()
		{
			GameObject camera;
			if (this.HealthBarLine == null || (camera = GameObject.Find("MainCamera")) == null) return;
			Vector3 direction = (camera.transform.position - this.transform.position).normalized;
			direction.y = 0;
			direction = Quaternion.AngleAxis(90, this.transform.up) * direction;
			this.HealthBar.transform.rotation = Quaternion.LookRotation(direction);
			this.HealthBarLine.SetPosition(1, new Vector3(0, 0, this.CurrentHealth / this.MaxHealth));
		}
	}
}