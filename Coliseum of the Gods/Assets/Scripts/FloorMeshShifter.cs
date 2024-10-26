using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.IotaScripts
{
	public class MyFloorShifter : MonoBehaviour
	{
		private sealed class MySubdivisionShifter
		{
			private uint CurrentGameTick = 0;
			private uint TotalGameTicks = 0;
			private Vector3 FromPosition;
			private Vector3 ToPosition;
			private Vector3 CurrentPosition;
			public readonly Vector3 InitialPosition;
			public readonly GameObject Target;

			public MySubdivisionShifter(GameObject target)
			{
				this.Target = target;
				this.FromPosition = target.transform.localPosition;
				this.ToPosition = target.transform.localPosition;
				this.InitialPosition = target.transform.localPosition;
				this.CurrentPosition = target.transform.localPosition;
			}

			internal void Tick()
			{
				if (this.CurrentGameTick == 0) return;
				float ratio = 1f - ((float) this.CurrentGameTick-- / (float) this.TotalGameTicks);
				this.CurrentPosition = Vector3.LerpUnclamped(this.FromPosition, this.ToPosition, ratio);
				this.Target.transform.localPosition = this.CurrentPosition;
			}

			public void ShiftTo(Vector3 position, uint game_ticks)
			{
				this.TotalGameTicks = game_ticks;
				this.CurrentGameTick = game_ticks;
				this.ToPosition = position;
				this.FromPosition = this.CurrentPosition;
			}
		}

		private static uint Subdivisions = 5u;

		private List<MySubdivisionShifter> FloorSubdivisions = new List<MySubdivisionShifter>();

		private void Start()
		{
			MeshRenderer mesh_renderer = this.GetComponent<MeshRenderer>();
			MeshFilter mesh_filter = this.GetComponent<MeshFilter>();
			Vector3 plane_size = mesh_filter.mesh.bounds.size;
			Vector3 subdivision_size = plane_size / MyFloorShifter.Subdivisions;
			Vector3 tl_corner = mesh_filter.mesh.bounds.min;
			Vector3 lateral_offset = new Vector3(subdivision_size.x, 0, 0);
			Vector3 vertical_offset = new Vector3(0, 0, subdivision_size.z);

			Vector3 tl_corner_l = new Vector3(0, 0, 0);
			Vector3 tr_corner_l = new Vector3(subdivision_size.x, 0, 0);
			Vector3 bl_corner_l = new Vector3(0, 0, subdivision_size.z);
			Vector3 br_corner_l = new Vector3(subdivision_size.x, 0, subdivision_size.z);

			uint index = 0;

			for (uint y = 0; y < MyFloorShifter.Subdivisions; ++y)
			{
				for (uint x = 0; x < MyFloorShifter.Subdivisions; ++x)
				{
					GameObject sub_division = new GameObject($"FloorDivision{index++}");
					MeshRenderer renderer = sub_division.AddComponent<MeshRenderer>();
					MeshFilter filter = sub_division.AddComponent<MeshFilter>();
					MeshCollider collider = sub_division.AddComponent<MeshCollider>();
					Mesh mesh = new Mesh();

					mesh.vertices = new Vector3[4] {
						tl_corner_l,
						tr_corner_l,
						bl_corner_l,
						br_corner_l
					};

					mesh.triangles = new int[6] {
						0, 2, 1,
						2, 3, 1
					};

					mesh.normals = new Vector3[4] {
						-Vector3.forward,
						-Vector3.forward,
						-Vector3.forward,
						-Vector3.forward
					};

					mesh.uv = new Vector2[4] {
						new Vector2(0, 0),
						new Vector2(1, 0),
						new Vector2(0, 1),
						new Vector2(1, 1)
					};

					filter.mesh = mesh;
					renderer.sharedMaterial = mesh_renderer.sharedMaterial;
					collider.cookingOptions = (MeshColliderCookingOptions) 0b11111;
					collider.sharedMesh = mesh;
					sub_division.transform.SetParent(this.transform);
					sub_division.transform.localPosition = (lateral_offset * x) + (vertical_offset * y) - mesh_filter.mesh.bounds.extents;
					sub_division.transform.localScale = new Vector3(1, 1, 1);
					this.FloorSubdivisions.Add(new MySubdivisionShifter(sub_division));
				}
			}

			MeshCollider mesh_collider = this.GetComponent<MeshCollider>();
			if (mesh_collider != null) mesh_collider.enabled = false;
			mesh_renderer.enabled = false;
		}

		private void LateUpdate()
		{
			foreach (MySubdivisionShifter shifter in this.FloorSubdivisions) shifter.Tick();
		}

		internal void RandomShiftSubdivisions(uint game_ticks, float max_height_offset)
		{
			this.RandomShiftSubdivisions(game_ticks, max_height_offset, (int) (DateTime.UtcNow - DateTime.MinValue).Ticks);
		}

		internal void RandomShiftSubdivisions(uint game_ticks, float max_height_offset, int seed)
		{
			for (int i = 0; i < this.FloorSubdivisions.Count; ++i)
			{
				MySubdivisionShifter shifter = this.FloorSubdivisions[i];
				UnityEngine.Random.InitState(seed + (i << 1));
				Vector3 offset = new Vector3(0, UnityEngine.Random.value * max_height_offset, 0);
				shifter.ShiftTo(shifter.InitialPosition + offset, game_ticks);
			}
		}

		internal void ResetSubdivisios(uint game_ticks)
		{
			foreach (MySubdivisionShifter shifter in this.FloorSubdivisions) shifter.ShiftTo(shifter.InitialPosition, game_ticks);
		}
	}
}
