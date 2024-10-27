using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MySessionComponent : MonoBehaviour
	{
		private float FloorSwitchTimeSeconds;
		private float MaxTileHeightOffset;

		private DateTime LastArenaShiftTime = DateTime.MinValue;

		internal MyFloorShifter FloorShifter { get; private set; }

		private void Awake()
		{
			MyWeaponHandler.Initialize();
			MyDamageOverTimeSystem.Initialize();
		}

		private void Start()
		{
			this.FloorShifter = GameObject.Find("Floor").GetComponent<MyFloorShifter>();
			this.LastArenaShiftTime = DateTime.UtcNow;
			this.FloorSwitchTimeSeconds = Variables.Object(this.gameObject).Get<float>("FloorSwitchTimeSeconds");
			this.MaxTileHeightOffset = Variables.Object(this.gameObject).Get<float>("MaxTileHeightOffset");
		}

		private void OnDisable()
		{
			this.FloorShifter.ResetSubdivisios(0);
		}

		private void FixedUpdate()
		{
			DateTime now = DateTime.UtcNow;
			TimeSpan delta = now - this.LastArenaShiftTime;
			if (delta.TotalSeconds > this.FloorSwitchTimeSeconds) this.ShiftUpdateArena();
		}

		internal void ShiftUpdateArena()
		{
			this.FloorShifter.RandomShiftSubdivisions(50, this.MaxTileHeightOffset);
			this.LastArenaShiftTime = DateTime.UtcNow;
		}
	}
}