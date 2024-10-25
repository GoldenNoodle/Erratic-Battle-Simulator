using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC
{
	internal static class Configuration
	{
		public static Vector2 ScreenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		public static Vector2 ScreenCenter = Configuration.ScreenSize / 2f;

		public static float PlayerMovementSpeed = 1f;
		public static float NPCMovementSpeed = 1f;

		public static float LateralMouseMoveMultiplier = 1f;
		public static float VerticalMouseMoveMultiplier = 0.25f;
	}
}