using UnityEngine;
using System.Collections.Generic;

public static class StaticMethods {

	public static bool IsInLayerMask(this LayerMask mask, GameObject obj) {
	 return ((mask.value & (1 << obj.layer)) > 0);
	}
}
