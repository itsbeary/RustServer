using System;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public struct SpawnIndividual
{
	// Token: 0x06002AC6 RID: 10950 RVA: 0x001050C4 File Offset: 0x001032C4
	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		this.PrefabID = prefabID;
		this.Position = position;
		this.Rotation = rotation;
	}

	// Token: 0x040022F1 RID: 8945
	public uint PrefabID;

	// Token: 0x040022F2 RID: 8946
	public Vector3 Position;

	// Token: 0x040022F3 RID: 8947
	public Quaternion Rotation;
}
