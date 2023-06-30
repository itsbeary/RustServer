using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class PowerlineNode : MonoBehaviour
{
	// Token: 0x06003009 RID: 12297 RVA: 0x00120D79 File Offset: 0x0011EF79
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.AddWire(this);
		}
	}

	// Token: 0x040027A1 RID: 10145
	public GameObjectRef WirePrefab;

	// Token: 0x040027A2 RID: 10146
	public float MaxDistance = 50f;
}
