using System;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x06001C1C RID: 7196 RVA: 0x000C4EB9 File Offset: 0x000C30B9
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.collider);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x04001446 RID: 5190
	public SoundDefinition softSound;

	// Token: 0x04001447 RID: 5191
	public SoundDefinition medSound;

	// Token: 0x04001448 RID: 5192
	public SoundDefinition hardSound;

	// Token: 0x04001449 RID: 5193
	public Collider collider;
}
