using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E7 RID: 2023
public class ExpandedLifeStats : MonoBehaviour
{
	// Token: 0x04002D7A RID: 11642
	public GameObject DisplayRoot;

	// Token: 0x04002D7B RID: 11643
	public GameObjectRef GenericStatRow;

	// Token: 0x04002D7C RID: 11644
	[Header("Resources")]
	public Transform ResourcesStatRoot;

	// Token: 0x04002D7D RID: 11645
	public List<ExpandedLifeStats.GenericStatDisplay> ResourceStats;

	// Token: 0x04002D7E RID: 11646
	[Header("Weapons")]
	public GameObjectRef WeaponStatRow;

	// Token: 0x04002D7F RID: 11647
	public Transform WeaponsRoot;

	// Token: 0x04002D80 RID: 11648
	[Header("Misc")]
	public Transform MiscRoot;

	// Token: 0x04002D81 RID: 11649
	public List<ExpandedLifeStats.GenericStatDisplay> MiscStats;

	// Token: 0x04002D82 RID: 11650
	public LifeInfographic Infographic;

	// Token: 0x04002D83 RID: 11651
	public RectTransform MoveRoot;

	// Token: 0x04002D84 RID: 11652
	public Vector2 OpenPosition;

	// Token: 0x04002D85 RID: 11653
	public Vector2 ClosedPosition;

	// Token: 0x04002D86 RID: 11654
	public GameObject OpenButtonRoot;

	// Token: 0x04002D87 RID: 11655
	public GameObject CloseButtonRoot;

	// Token: 0x04002D88 RID: 11656
	public GameObject ScrollGradient;

	// Token: 0x04002D89 RID: 11657
	public ScrollRect Scroller;

	// Token: 0x02000E8D RID: 3725
	[Serializable]
	public struct GenericStatDisplay
	{
		// Token: 0x04004C7C RID: 19580
		public string statKey;

		// Token: 0x04004C7D RID: 19581
		public Sprite statSprite;

		// Token: 0x04004C7E RID: 19582
		public Translate.Phrase displayPhrase;
	}
}
