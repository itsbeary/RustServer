using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C5 RID: 1989
public class CardGamePlayerWidget : MonoBehaviour
{
	// Token: 0x04002C67 RID: 11367
	[SerializeField]
	private GameObjectRef cardImageSmallPrefab;

	// Token: 0x04002C68 RID: 11368
	[SerializeField]
	private RawImage avatar;

	// Token: 0x04002C69 RID: 11369
	[SerializeField]
	private RustText playerName;

	// Token: 0x04002C6A RID: 11370
	[SerializeField]
	private RustText scrapTotal;

	// Token: 0x04002C6B RID: 11371
	[SerializeField]
	private RustText betText;

	// Token: 0x04002C6C RID: 11372
	[SerializeField]
	private Image background;

	// Token: 0x04002C6D RID: 11373
	[SerializeField]
	private Color inactiveBackground;

	// Token: 0x04002C6E RID: 11374
	[SerializeField]
	private Color activeBackground;

	// Token: 0x04002C6F RID: 11375
	[SerializeField]
	private Color foldedBackground;

	// Token: 0x04002C70 RID: 11376
	[SerializeField]
	private Color winnerBackground;

	// Token: 0x04002C71 RID: 11377
	[SerializeField]
	private Animation actionShowAnimation;

	// Token: 0x04002C72 RID: 11378
	[SerializeField]
	private RustText actionText;

	// Token: 0x04002C73 RID: 11379
	[SerializeField]
	private Sprite canSeeIcon;

	// Token: 0x04002C74 RID: 11380
	[SerializeField]
	private Sprite cannotSeeIcon;

	// Token: 0x04002C75 RID: 11381
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002C76 RID: 11382
	[SerializeField]
	private Image cornerIcon;

	// Token: 0x04002C77 RID: 11383
	[SerializeField]
	private Transform cardDisplayParent;

	// Token: 0x04002C78 RID: 11384
	[SerializeField]
	private GridLayoutGroup cardDisplayGridLayout;

	// Token: 0x04002C79 RID: 11385
	[SerializeField]
	private GameObject circle;

	// Token: 0x04002C7A RID: 11386
	[SerializeField]
	private RustText circleText;
}
