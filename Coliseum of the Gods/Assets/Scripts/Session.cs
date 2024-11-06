using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MySessionComponent : MonoBehaviour
	{
		private DateTime LastArenaShiftTime = DateTime.MinValue;
		private DateTime LastWeaponShiftTime = DateTime.MinValue;
		private GameObject TextCounter;

		internal MyFloorShifter FloorShifter { get; private set; }
		internal TMP_Text TextCounterText { get; private set; }

		public uint FloorSubdivisionLevel = 10;
		public float FloorSwitchTimeSeconds = 30;
		public float WeaponSwitchTimeSeconds = 30;
		public float MaxTileHeightOffset = 0;
		public double DeathmatchTimeSeconds = 120;

		internal double RemainingDeathmatchTimeSeconds;

		private void Awake()
		{
			MyWeaponHandler.Initialize();
			MyDamageOverTimeSystem.Initialize();
		}

		private void Start()
		{
			this.FloorShifter = GameObject.Find("Floor").GetComponent<MyFloorShifter>();
			this.FloorShifter.Initialize(this.FloorSubdivisionLevel);
			this.TextCounter = GameObject.Find("CounterText");
			this.TextCounterText = this.TextCounter.GetComponent<TMP_Text>();
			this.RemainingDeathmatchTimeSeconds = this.DeathmatchTimeSeconds;

			this.ShiftUpdateArena();
			this.ShiftRandomizeWeapons();
		}

		private void OnDisable()
		{
			this.FloorShifter.ResetSubdivisios(0);
		}

		private void FixedUpdate()
		{
			DateTime now = DateTime.UtcNow;
			if ((now - this.LastArenaShiftTime).TotalSeconds > this.FloorSwitchTimeSeconds) this.ShiftUpdateArena();
			if ((now - this.LastWeaponShiftTime).TotalSeconds > this.WeaponSwitchTimeSeconds) this.ShiftRandomizeWeapons();
			this.RemainingDeathmatchTimeSeconds -= Time.deltaTime;
			
			if (this.RemainingDeathmatchTimeSeconds >= 0) this.TextCounterText.text = $"{(uint) (this.RemainingDeathmatchTimeSeconds / 60d):D2}:{(uint) (this.RemainingDeathmatchTimeSeconds % 60d):D2}";
			else
			{
				double deathmatch_time = -this.RemainingDeathmatchTimeSeconds;
				this.TextCounterText.text = $"{(uint) (deathmatch_time / 60d):D2}:{(uint) (deathmatch_time % 60d):D2}";
				this.TextCounterText.color = Color.red;
			}
		}

		internal void ShiftUpdateArena()
		{
			this.FloorShifter.RandomShiftSubdivisions(50, this.MaxTileHeightOffset);
			this.LastArenaShiftTime = DateTime.UtcNow;
		}

		internal void ShiftRandomizeWeapons()
		{
			foreach (MyWeaponHandler handler in MyWeaponHandler.Instances)
			{
				UnityEngine.Random.InitState((int) DateTime.UtcNow.Ticks);
				int index = (int) Math.Round(UnityEngine.Random.value * MyWeaponHandler.WeaponInfos.Count);
				handler.ActiveWeapon = (index == MyWeaponHandler.WeaponInfos.Count) ? null : MyWeaponHandler.WeaponInfos.Keys.ElementAt(index);
			}

			this.LastWeaponShiftTime = DateTime.UtcNow;
		}
	}
}