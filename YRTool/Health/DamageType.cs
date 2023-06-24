using UnityEngine;

namespace YRTool
{
	public enum DamageTypeModes { BaseDamage, TypedDamage }
	/// <summary>
	/// A scriptable object you can create assets from, to identify damage types
	/// </summary>
	[CreateAssetMenu(menuName = "YRTool/Health/DamageType", fileName = "DamageType")]
	public class DamageType : ScriptableObject
	{
	}    
}