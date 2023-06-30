using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
[CreateAssetMenu(menuName = "Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
	// Token: 0x06002A8D RID: 10893 RVA: 0x00103699 File Offset: 0x00101899
	public bool Contains(HitArea hitArea)
	{
		return (this.area & hitArea) > (HitArea)0;
	}

	// Token: 0x040022CA RID: 8906
	[InspectorFlags]
	public HitArea area;
}
