using System;
using UnityEngine;

// Token: 0x02000346 RID: 838
public class FirstPersonEffect : MonoBehaviour, IEffect
{
	// Token: 0x0400187A RID: 6266
	public bool isGunShot;

	// Token: 0x0400187B RID: 6267
	[HideInInspector]
	public EffectParentToWeaponBone parentToWeaponComponent;
}
