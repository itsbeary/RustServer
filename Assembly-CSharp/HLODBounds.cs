using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
public class HLODBounds : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002259 RID: 8793
	[Tooltip("The bounds that this HLOD will cover. This should not overlap with any other HLODs")]
	public Bounds MeshBounds = new Bounds(Vector3.zero, new Vector3(50f, 25f, 50f));

	// Token: 0x0400225A RID: 8794
	[Tooltip("Assets created will use this prefix. Make sure multiple HLODS in a scene have different prefixes")]
	public string MeshPrefix = "root";

	// Token: 0x0400225B RID: 8795
	[Tooltip("The point from which to calculate the HLOD. Any RendererLODs that are visible at this distance will baked into the HLOD mesh")]
	public float CullDistance = 100f;

	// Token: 0x0400225C RID: 8796
	[Tooltip("If set, the lod will take over at this distance instead of the CullDistance (eg. we make a model based on what this area looks like at 200m but we actually want it take over rendering at 300m)")]
	public float OverrideLodDistance;

	// Token: 0x0400225D RID: 8797
	[Tooltip("Any renderers below this height will considered culled even if they are visible from a distance. Good for underground areas")]
	public float CullBelowHeight;

	// Token: 0x0400225E RID: 8798
	[Tooltip("Optimises the mesh produced by removing non-visible and small faces. Can turn it off during dev but should be on for final builds")]
	public bool ApplyMeshTrimming = true;

	// Token: 0x0400225F RID: 8799
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x04002260 RID: 8800
	public RendererLOD DebugComponent;

	// Token: 0x04002261 RID: 8801
	public bool ShowTrimSettings;
}
