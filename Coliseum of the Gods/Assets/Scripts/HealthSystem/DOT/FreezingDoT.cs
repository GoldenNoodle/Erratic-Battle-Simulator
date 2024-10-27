using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.IotaScripts
{ 
	internal partial class DamageOverTimeDefinitions
	{
		MyDamageOverTimeSystem FreezingDoT = MyDamageOverTimeSystem.RegisterNewDoTSystem("FreezingDoT", 300, 1, movement_speed_multiplier: 0, jump_height_multiplier: 0);
	}
}