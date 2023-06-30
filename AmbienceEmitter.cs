using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x000C47CF File Offset: 0x000C29CF
	// (set) Token: 0x06001BFA RID: 7162 RVA: 0x000C47D7 File Offset: 0x000C29D7
	public TerrainTopology.Enum currentTopology { get; private set; }

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001BFB RID: 7163 RVA: 0x000C47E0 File Offset: 0x000C29E0
	// (set) Token: 0x06001BFC RID: 7164 RVA: 0x000C47E8 File Offset: 0x000C29E8
	public TerrainBiome.Enum currentBiome { get; private set; }

	// Token: 0x06001BFD RID: 7165 RVA: 0x000C47F1 File Offset: 0x000C29F1
	public int CompareTo(AmbienceEmitter other)
	{
		return this.cameraDistanceSq.CompareTo(other.cameraDistanceSq);
	}

	// Token: 0x040013C9 RID: 5065
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040013CA RID: 5066
	public AmbienceDefinitionList stings;

	// Token: 0x040013CB RID: 5067
	public bool isStatic = true;

	// Token: 0x040013CC RID: 5068
	public bool followCamera;

	// Token: 0x040013CD RID: 5069
	public bool isBaseEmitter;

	// Token: 0x040013CE RID: 5070
	public bool active;

	// Token: 0x040013CF RID: 5071
	public float cameraDistanceSq = float.PositiveInfinity;

	// Token: 0x040013D0 RID: 5072
	public BoundingSphere boundingSphere;

	// Token: 0x040013D1 RID: 5073
	public float crossfadeTime = 2f;

	// Token: 0x040013D4 RID: 5076
	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	// Token: 0x040013D5 RID: 5077
	public float deactivateTime = float.PositiveInfinity;

	// Token: 0x040013D6 RID: 5078
	public bool playUnderwater = true;

	// Token: 0x040013D7 RID: 5079
	public bool playAbovewater = true;
}
