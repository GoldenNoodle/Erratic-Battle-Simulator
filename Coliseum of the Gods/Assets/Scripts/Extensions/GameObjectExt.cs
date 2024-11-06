using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GC.IotaScripts
{
	public static partial class Extensions
	{ 
		public static GameObject GetChildByName(this GameObject self, string name)
		{
			return Enumerable.Range(0, self.transform.childCount).Select(index => self.transform.GetChild(index).gameObject).FirstOrDefault(child => child.name == name);
		}

		public static GameObject GetChildByPath(this GameObject self, IEnumerable<string> path)
		{
			GameObject src = self;
			foreach (string name in path) src = src.GetChildByName(name);
			return src;
		}

		public static IEnumerable<GameObject> GetAllChildren(this GameObject self)
		{
			return Enumerable.Range(0, self.transform.childCount).Select(index => self.transform.GetChild(index).gameObject);
		}
	}
}
