using System;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000866 RID: 2150
public class LifeInfographic : MonoBehaviour
{
	// Token: 0x04003085 RID: 12421
	[NonSerialized]
	public PlayerLifeStory life;

	// Token: 0x04003086 RID: 12422
	public GameObject container;

	// Token: 0x04003087 RID: 12423
	public RawImage AttackerAvatarImage;

	// Token: 0x04003088 RID: 12424
	public Image DamageSourceImage;

	// Token: 0x04003089 RID: 12425
	public LifeInfographicStat[] Stats;

	// Token: 0x0400308A RID: 12426
	public Animator[] AllAnimators;

	// Token: 0x0400308B RID: 12427
	public GameObject WeaponRoot;

	// Token: 0x0400308C RID: 12428
	public GameObject DistanceRoot;

	// Token: 0x0400308D RID: 12429
	public GameObject DistanceDivider;

	// Token: 0x0400308E RID: 12430
	public Image WeaponImage;

	// Token: 0x0400308F RID: 12431
	public LifeInfographic.DamageSetting[] DamageDisplays;

	// Token: 0x04003090 RID: 12432
	public Texture2D defaultAvatarTexture;

	// Token: 0x04003091 RID: 12433
	public bool ShowDebugData;

	// Token: 0x02000E9E RID: 3742
	[Serializable]
	public struct DamageSetting
	{
		// Token: 0x04004C9F RID: 19615
		public DamageType ForType;

		// Token: 0x04004CA0 RID: 19616
		public string Display;

		// Token: 0x04004CA1 RID: 19617
		public Sprite DamageSprite;
	}
}
