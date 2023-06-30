using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class FlashlightBeam : MonoBehaviour, IClientComponent
{
	// Token: 0x040011EC RID: 4588
	public Vector2 scrollDir;

	// Token: 0x040011ED RID: 4589
	public Vector3 localEndPoint = new Vector3(0f, 0f, 2f);

	// Token: 0x040011EE RID: 4590
	public LineRenderer beamRenderer;
}
