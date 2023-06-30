using System;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class LandmarkInfo : MonoBehaviour
{
	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06002A36 RID: 10806 RVA: 0x0004485D File Offset: 0x00042A5D
	public virtual MapLayer MapLayer
	{
		get
		{
			return MapLayer.Overworld;
		}
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x00101E95 File Offset: 0x00100095
	protected virtual void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Landmarks.Add(this);
		}
	}

	// Token: 0x04002288 RID: 8840
	[Header("LandmarkInfo")]
	public bool shouldDisplayOnMap;

	// Token: 0x04002289 RID: 8841
	public bool isLayerSpecific;

	// Token: 0x0400228A RID: 8842
	public Translate.Phrase displayPhrase;

	// Token: 0x0400228B RID: 8843
	public Sprite mapIcon;
}
