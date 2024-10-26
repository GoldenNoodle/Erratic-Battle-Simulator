using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
	public class NPCManager : MonoBehaviour
	{
		private GameObject Player;

		private void Start()
		{
			this.Player = GameObject.Find("Player");
		}

		private void Update()
		{
			if (this.Player == null) return;
			Vector3 direction = this.Player.transform.position - this.transform.position;
			direction.y = 0;
			float distance = direction.magnitude;
			if (distance == 0) return;
			this.transform.rotation = Quaternion.LookRotation(direction);
			if (distance > 1.5f) this.transform.position += (this.transform.forward * 2.5f * Time.deltaTime);
		}
	}
}