using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000411 RID: 1041
public class CardTable : BaseCardGameEntity
{
	// Token: 0x06002373 RID: 9075 RVA: 0x000E244C File Offset: 0x000E064C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06002374 RID: 9076 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x04001B55 RID: 6997
	[Header("Card Table")]
	[SerializeField]
	private ViewModel viewModel;

	// Token: 0x04001B56 RID: 6998
	[SerializeField]
	private CardGameUI.PlayingCardImage[] tableCards;

	// Token: 0x04001B57 RID: 6999
	[SerializeField]
	private Renderer[] tableCardBackings;

	// Token: 0x04001B58 RID: 7000
	[SerializeField]
	private Canvas cardUICanvas;

	// Token: 0x04001B59 RID: 7001
	[SerializeField]
	private Image[] tableCardImages;

	// Token: 0x04001B5A RID: 7002
	[SerializeField]
	private Sprite blankCard;

	// Token: 0x04001B5B RID: 7003
	[SerializeField]
	private CardTable.ChipStack[] chipStacks;

	// Token: 0x04001B5C RID: 7004
	[SerializeField]
	private CardTable.ChipStack[] fillerStacks;

	// Token: 0x02000CF2 RID: 3314
	[Serializable]
	public class ChipStack : IComparable<CardTable.ChipStack>
	{
		// Token: 0x0600501C RID: 20508 RVA: 0x001A8249 File Offset: 0x001A6449
		public int CompareTo(CardTable.ChipStack other)
		{
			if (other == null)
			{
				return 1;
			}
			return this.chipValue.CompareTo(other.chipValue);
		}

		// Token: 0x04004608 RID: 17928
		public int chipValue;

		// Token: 0x04004609 RID: 17929
		public GameObject[] chips;
	}
}
