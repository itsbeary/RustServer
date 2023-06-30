using System;
using UnityEngine;

// Token: 0x02000440 RID: 1088
public class LightListener : BaseEntity
{
	// Token: 0x060024B1 RID: 9393 RVA: 0x000E938D File Offset: 0x000E758D
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == this.onMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		if (msg == this.offMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04001C9C RID: 7324
	public string onMessage = "";

	// Token: 0x04001C9D RID: 7325
	public string offMessage = "";

	// Token: 0x04001C9E RID: 7326
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime onLights;

	// Token: 0x04001C9F RID: 7327
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime offLights;
}
