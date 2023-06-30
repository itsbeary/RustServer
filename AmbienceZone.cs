using System;

// Token: 0x02000227 RID: 551
public class AmbienceZone : TriggerBase, IClientComponentEx
{
	// Token: 0x06001C04 RID: 7172 RVA: 0x000C4929 File Offset: 0x000C2B29
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x040013EB RID: 5099
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040013EC RID: 5100
	public AmbienceDefinitionList stings;

	// Token: 0x040013ED RID: 5101
	public float priority;

	// Token: 0x040013EE RID: 5102
	public bool overrideCrossfadeTime;

	// Token: 0x040013EF RID: 5103
	public float crossfadeTime = 1f;

	// Token: 0x040013F0 RID: 5104
	public float ambienceGain = 1f;
}
