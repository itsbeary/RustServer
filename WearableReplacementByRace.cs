using System;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class WearableReplacementByRace : MonoBehaviour
{
	// Token: 0x06002C48 RID: 11336 RVA: 0x0010C2C8 File Offset: 0x0010A4C8
	public GameObjectRef GetReplacement(int meshIndex)
	{
		int num = Mathf.Clamp(meshIndex, 0, this.replacements.Length - 1);
		return this.replacements[num];
	}

	// Token: 0x040023FF RID: 9215
	public GameObjectRef[] replacements;
}
