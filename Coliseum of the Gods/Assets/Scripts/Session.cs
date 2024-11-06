using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace GC.IotaScripts
{
	public class MySessionComponent : MonoBehaviour
	{
		internal static Camera MainCamera { get; private set; }

		private DateTime LastArenaShiftTime = DateTime.MinValue;
		private DateTime LastWeaponShiftTime = DateTime.MinValue;

		private GameObject[] PlayerTextTL = new GameObject[2];

		internal MyFloorShifter FloorShifter { get; private set; }

		internal double RemainingDeathmatchTimeSeconds;

		public uint FloorSubdivisionLevel = 10;
		public float FloorSwitchTimeSeconds = 30;
		public float WeaponSwitchTimeSeconds = 30;
		public float MaxTileHeightOffset = 0;
		public double DeathmatchTimeSeconds = 120;

		internal static string TimeSecondsToHHMMSS(double time_seconds)
		{
			uint hours = (uint) time_seconds / 3600;
			uint minutes = (uint) time_seconds / 60;
			uint seconds = (uint) time_seconds % 60;
			return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
		}

		private void Awake()
		{
			MyWeaponHandler.Initialize();
			MyDamageOverTimeSystem.Initialize();
		}

		private void Start()
		{
			this.FloorShifter = GameObject.Find("Floor").GetComponent<MyFloorShifter>();
			this.FloorShifter.Initialize(this.FloorSubdivisionLevel);
			this.RemainingDeathmatchTimeSeconds = this.DeathmatchTimeSeconds;

			GameObject camera = GameObject.Find("MainCamera");
			MySessionComponent.MainCamera = camera.GetComponent<Camera>();

			for (int i = 0; i < this.PlayerTextTL.Length; ++i)
			{
				GameObject text = new GameObject($"Text{i}");

				MeshRenderer renderer = text.AddComponent<MeshRenderer>();
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				renderer.receiveGI = 0;
				renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
				renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
				renderer.probeAnchor = null;
				renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
				renderer.allowOcclusionWhenDynamic = true;

				TextMeshPro text_comp = text.AddComponent<TextMeshPro>();
				text_comp.text = "";
				text_comp.fontSize = 18;
				text_comp.alignment = TextAlignmentOptions.Left;
				text_comp.enableWordWrapping = false;
				text_comp.overflowMode = TextOverflowModes.Ellipsis;
				text_comp.color = Color.white;
				text_comp.outlineColor = Color.black;
				text_comp.outlineWidth = 0.25f;
				text_comp.fontMaterial.SetColor("_OutlineColor", Color.black);
				//text_comp.fontMaterial.SetFloat("_OutlineWidth", 0.25f);
				text_comp.autoSizeTextContainer = true;

				renderer.materials = new Material[1] { text_comp.fontMaterial };
				text.transform.SetParent(camera.transform);
				text.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
				this.PlayerTextTL[i] = text;
			}

			this.ShiftRandomizeWeapons();
			this.LastArenaShiftTime = DateTime.UtcNow;
		}

		private void OnDisable()
		{
			this.FloorShifter.ResetSubdivisios(0);
		}

		private void FixedUpdate()
		{
			// Shift/randomize session things
			DateTime now = DateTime.UtcNow;
			if ((now - this.LastArenaShiftTime).TotalSeconds > this.FloorSwitchTimeSeconds) this.ShiftUpdateArena();
			if ((now - this.LastWeaponShiftTime).TotalSeconds > this.WeaponSwitchTimeSeconds) this.ShiftRandomizeWeapons();

			// Update text headers
			this.RemainingDeathmatchTimeSeconds -= Time.deltaTime;
			if (this.RemainingDeathmatchTimeSeconds >= 0) this.SetHeader(0, $"Deathmatch In: {MySessionComponent.TimeSecondsToHHMMSS(this.RemainingDeathmatchTimeSeconds)}");
			else this.SetHeader(0, $"Deathmatch Time: {MySessionComponent.TimeSecondsToHHMMSS(-this.RemainingDeathmatchTimeSeconds)}", Color.red);
			this.SetHeader(1, $"Arena Shift In: {MySessionComponent.TimeSecondsToHHMMSS(this.RemainingDeathmatchTimeSeconds % 30)}");

			// Update text
			float by = MySessionComponent.MainCamera.pixelHeight - 25;

			for (int i = 0; i < this.PlayerTextTL.Length; ++i)
			{
				GameObject text = this.PlayerTextTL[i];
				TextMeshPro tmp = text?.GetComponent<TextMeshPro>();
				if ((tmp?.text.Length ?? 0) == 0) continue;

				Vector2 sprite_size = new Vector2(tmp.renderedWidth, tmp.renderedHeight);
				Vector2 local_sprite_size = sprite_size / tmp.pixelsPerUnit;
				Vector3 world_size = local_sprite_size;
				world_size.x *= text.transform.lossyScale.x;
				world_size.y *= text.transform.lossyScale.y;

				//convert to screen space size
				Vector3 screen_size = 0.5f * world_size / MySessionComponent.MainCamera.orthographicSize;
				screen_size.y *= MySessionComponent.MainCamera.aspect;

				//size in pixels
				Vector2 in_pixels = new Vector2(screen_size.x * MySessionComponent.MainCamera.pixelWidth, screen_size.y * MySessionComponent.MainCamera.pixelHeight) * 0.5f;
				text.transform.position = MySessionComponent.MainCamera.ScreenToWorldPoint(new Vector3(in_pixels.x * 6f, by, 1));
				text.transform.rotation = MySessionComponent.MainCamera.transform.rotation;
				by -= in_pixels.y + 20;
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

		internal void SetHeader(int index, string header, Color? color = null)
		{
			if (index > this.PlayerTextTL.Length) throw new InvalidOperationException("The specified header index is out of bounds");
			GameObject text = this.PlayerTextTL[index];
			if (text == null) return;
			TextMeshPro tmp = text.GetComponent<TextMeshPro>();
			tmp.text = header ?? "";
			tmp.color = color ?? tmp.color;
		}
	}
}