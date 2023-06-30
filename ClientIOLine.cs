using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class ClientIOLine : BaseMonoBehaviour
{
	// Token: 0x040011DB RID: 4571
	public RendererLOD _lod;

	// Token: 0x040011DC RID: 4572
	public LineRenderer _line;

	// Token: 0x040011DD RID: 4573
	public Material directionalMaterial;

	// Token: 0x040011DE RID: 4574
	public Material defaultMaterial;

	// Token: 0x040011DF RID: 4575
	public IOEntity.IOType lineType;

	// Token: 0x040011E0 RID: 4576
	public static List<ClientIOLine> _allLines = new List<ClientIOLine>();

	// Token: 0x040011E1 RID: 4577
	public WireTool.WireColour colour;

	// Token: 0x040011E2 RID: 4578
	public IOEntity ownerIOEnt;
}
