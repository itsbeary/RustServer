using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x020006A1 RID: 1697
[RequireComponent(typeof(TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
	// Token: 0x06003048 RID: 12360 RVA: 0x00122319 File Offset: 0x00120519
	public void Init(Terrain terrain, TerrainConfig config)
	{
		this.terrain = terrain;
		this.config = config;
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Setup()
	{
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostSetup()
	{
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x00122329 File Offset: 0x00120529
	public void LogSize(object obj, ulong size)
	{
		Debug.Log(obj.GetType() + " allocated: " + size.FormatBytes(false));
	}

	// Token: 0x040027FA RID: 10234
	[NonSerialized]
	public bool isInitialized;

	// Token: 0x040027FB RID: 10235
	internal Terrain terrain;

	// Token: 0x040027FC RID: 10236
	internal TerrainConfig config;
}
