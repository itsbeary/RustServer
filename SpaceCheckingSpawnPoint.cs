using System;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class SpaceCheckingSpawnPoint : GenericSpawnPoint
{
	// Token: 0x06002B16 RID: 11030 RVA: 0x0010617C File Offset: 0x0010437C
	public override bool IsAvailableTo(GameObjectRef prefabRef)
	{
		if (!base.IsAvailableTo(prefabRef))
		{
			return false;
		}
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(prefabRef.Get(), base.transform.position, base.transform.rotation, Vector3.one * this.customBoundsCheckScale);
	}

	// Token: 0x04002330 RID: 9008
	public bool useCustomBoundsCheckMask;

	// Token: 0x04002331 RID: 9009
	public LayerMask customBoundsCheckMask;

	// Token: 0x04002332 RID: 9010
	public float customBoundsCheckScale = 1f;
}
