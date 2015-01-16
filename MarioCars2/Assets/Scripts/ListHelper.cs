using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListHelper {
	public static T Pop<T>(this IList<T> list, int index = 0) {
		T value = list[index];
		list.RemoveAt(index);
		return value;
	}
}
