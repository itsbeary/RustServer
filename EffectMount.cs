using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
	// Token: 0x040011E4 RID: 4580
	public bool firstPerson;

	// Token: 0x040011E5 RID: 4581
	public GameObject effectPrefab;

	// Token: 0x040011E6 RID: 4582
	public GameObject spawnedEffect;

	// Token: 0x040011E7 RID: 4583
	public GameObject mountBone;

	// Token: 0x040011E8 RID: 4584
	public SoundDefinition onSoundDef;

	// Token: 0x040011E9 RID: 4585
	public SoundDefinition offSoundDef;

	// Token: 0x040011EA RID: 4586
	public bool blockOffSoundWhenGettingDisabled;
}
