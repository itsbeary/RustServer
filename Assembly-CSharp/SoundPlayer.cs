using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class SoundPlayer : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x040014E6 RID: 5350
	public SoundDefinition soundDefinition;

	// Token: 0x040014E7 RID: 5351
	public bool playImmediately = true;

	// Token: 0x040014E8 RID: 5352
	public float minStartDelay;

	// Token: 0x040014E9 RID: 5353
	public float maxStartDelay;

	// Token: 0x040014EA RID: 5354
	public bool debugRepeat;

	// Token: 0x040014EB RID: 5355
	public bool pending;

	// Token: 0x040014EC RID: 5356
	public Vector3 soundOffset = Vector3.zero;
}
