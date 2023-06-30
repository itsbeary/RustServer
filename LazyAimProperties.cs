using System;
using UnityEngine;

// Token: 0x02000563 RID: 1379
[CreateAssetMenu(menuName = "Rust/LazyAim Properties")]
public class LazyAimProperties : ScriptableObject
{
	// Token: 0x040022D0 RID: 8912
	[Range(0f, 10f)]
	public float snapStrength = 6f;

	// Token: 0x040022D1 RID: 8913
	[Range(0f, 45f)]
	public float deadzoneAngle = 1f;
}
