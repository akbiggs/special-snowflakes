using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MathHelper {
	public static Vector3 SetX(this Vector3 v, float x) {
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 SetY(this Vector3 v, float y) {
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 SetZ(this Vector3 v, float z) {
		return new Vector3(v.x, v.y, z);
	}
}
