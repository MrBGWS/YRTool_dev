using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鱼人的数学助手
/// </summary>
public static class YRMath
{

	/// <summary>
	/// 根据角度返回一个二维向量
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
