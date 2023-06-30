using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class ArcadeEntityController : BaseMonoBehaviour
{
	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x0600172D RID: 5933 RVA: 0x000B0E26 File Offset: 0x000AF026
	// (set) Token: 0x0600172E RID: 5934 RVA: 0x000B0E33 File Offset: 0x000AF033
	public Vector3 heading
	{
		get
		{
			return this.arcadeEntity.heading;
		}
		set
		{
			this.arcadeEntity.heading = value;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x0600172F RID: 5935 RVA: 0x000B0E41 File Offset: 0x000AF041
	// (set) Token: 0x06001730 RID: 5936 RVA: 0x000B0E53 File Offset: 0x000AF053
	public Vector3 positionLocal
	{
		get
		{
			return this.arcadeEntity.transform.localPosition;
		}
		set
		{
			this.arcadeEntity.transform.localPosition = value;
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x06001731 RID: 5937 RVA: 0x000B0E66 File Offset: 0x000AF066
	// (set) Token: 0x06001732 RID: 5938 RVA: 0x000B0E78 File Offset: 0x000AF078
	public Vector3 positionWorld
	{
		get
		{
			return this.arcadeEntity.transform.position;
		}
		set
		{
			this.arcadeEntity.transform.position = value;
		}
	}

	// Token: 0x04000F8F RID: 3983
	public BaseArcadeGame parentGame;

	// Token: 0x04000F90 RID: 3984
	public ArcadeEntity arcadeEntity;

	// Token: 0x04000F91 RID: 3985
	public ArcadeEntity sourceEntity;
}
