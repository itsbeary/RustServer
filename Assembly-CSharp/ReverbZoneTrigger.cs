using System;
using UnityEngine;

// Token: 0x0200023C RID: 572
public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x06001C47 RID: 7239 RVA: 0x000C57D4 File Offset: 0x000C39D4
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.trigger);
		p.RemoveComponent(this.reverbZone);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool IsSyncedToParent()
	{
		return false;
	}

	// Token: 0x04001490 RID: 5264
	public Collider trigger;

	// Token: 0x04001491 RID: 5265
	public AudioReverbZone reverbZone;

	// Token: 0x04001492 RID: 5266
	public float lodDistance = 100f;

	// Token: 0x04001493 RID: 5267
	public bool inRange;

	// Token: 0x04001494 RID: 5268
	public ReverbSettings reverbSettings;
}
