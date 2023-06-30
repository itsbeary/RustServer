using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class SkinnedMultiMesh : MonoBehaviour
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06001E39 RID: 7737 RVA: 0x000CE0AD File Offset: 0x000CC2AD
	public List<Renderer> Renderers { get; } = new List<Renderer>(32);

	// Token: 0x04001767 RID: 5991
	public bool shadowOnly;

	// Token: 0x04001768 RID: 5992
	internal bool IsVisible = true;

	// Token: 0x04001769 RID: 5993
	public bool eyesView;

	// Token: 0x0400176A RID: 5994
	public Skeleton skeleton;

	// Token: 0x0400176B RID: 5995
	public SkeletonSkinLod skeletonSkinLod;

	// Token: 0x0400176C RID: 5996
	public List<SkinnedMultiMesh.Part> parts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x0400176D RID: 5997
	[NonSerialized]
	public List<SkinnedMultiMesh.Part> createdParts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x0400176E RID: 5998
	[NonSerialized]
	public long lastBuildHash;

	// Token: 0x0400176F RID: 5999
	[NonSerialized]
	public MaterialPropertyBlock sharedPropertyBlock;

	// Token: 0x04001770 RID: 6000
	[NonSerialized]
	public MaterialPropertyBlock hairPropertyBlock;

	// Token: 0x04001771 RID: 6001
	public float skinNumber;

	// Token: 0x04001772 RID: 6002
	public float meshNumber;

	// Token: 0x04001773 RID: 6003
	public float hairNumber;

	// Token: 0x04001774 RID: 6004
	public int skinType;

	// Token: 0x04001775 RID: 6005
	public SkinSetCollection SkinCollection;

	// Token: 0x02000CB4 RID: 3252
	public struct Part
	{
		// Token: 0x040044EF RID: 17647
		public Wearable wearable;

		// Token: 0x040044F0 RID: 17648
		public GameObject gameObject;

		// Token: 0x040044F1 RID: 17649
		public string name;

		// Token: 0x040044F2 RID: 17650
		public Item item;
	}
}
