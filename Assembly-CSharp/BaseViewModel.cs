using System;
using UnityEngine;

// Token: 0x0200096F RID: 2415
public class BaseViewModel : MonoBehaviour
{
	// Token: 0x0400342E RID: 13358
	[Header("BaseViewModel")]
	public LazyAimProperties lazyaimRegular;

	// Token: 0x0400342F RID: 13359
	public LazyAimProperties lazyaimIronsights;

	// Token: 0x04003430 RID: 13360
	public Transform pivot;

	// Token: 0x04003431 RID: 13361
	public bool useViewModelCamera = true;

	// Token: 0x04003432 RID: 13362
	public bool wantsHeldItemFlags;

	// Token: 0x04003433 RID: 13363
	public GameObject[] hideSightMeshes;

	// Token: 0x04003434 RID: 13364
	public bool isGestureViewModel;

	// Token: 0x04003435 RID: 13365
	public Transform MuzzlePoint;

	// Token: 0x04003436 RID: 13366
	[Header("Skin")]
	public SubsurfaceProfile subsurfaceProfile;
}
