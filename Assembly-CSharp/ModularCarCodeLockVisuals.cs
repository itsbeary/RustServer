using System;
using UnityEngine;

// Token: 0x020004A1 RID: 1185
[Serializable]
public class ModularCarCodeLockVisuals : MonoBehaviour
{
	// Token: 0x04001F5C RID: 8028
	[SerializeField]
	private GameObject lockedVisuals;

	// Token: 0x04001F5D RID: 8029
	[SerializeField]
	private GameObject unlockedVisuals;

	// Token: 0x04001F5E RID: 8030
	[SerializeField]
	private GameObject blockedVisuals;

	// Token: 0x04001F5F RID: 8031
	[SerializeField]
	private GameObjectRef codelockEffectDenied;

	// Token: 0x04001F60 RID: 8032
	[SerializeField]
	private GameObjectRef codelockEffectShock;

	// Token: 0x04001F61 RID: 8033
	[SerializeField]
	private float xOffset = 0.91f;

	// Token: 0x04001F62 RID: 8034
	[SerializeField]
	private ParticleSystemContainer keycodeDestroyableFX;
}
