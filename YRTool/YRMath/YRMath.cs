using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���˵���ѧ����
/// </summary>
public static class YRMath
{

	/// <summary>
	/// ���ݽǶȷ���һ����ά����
	/// </summary>
	/// <param name="angle"></param>
	/// <returns></returns>
	public static Vector3 DirectionFromAngle2D(float angle)
	{
		Vector3 direction = Vector3.zero;
		direction.x = Mathf.Cos(angle * Mathf.Deg2Rad);
		direction.y = Mathf.Sin(angle * Mathf.Deg2Rad);
		direction.z = 0f;
		return direction;
	}
}
