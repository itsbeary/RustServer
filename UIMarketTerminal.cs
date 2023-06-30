using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class UIMarketTerminal : UIDialog, IVendingMachineInterface
{
	// Token: 0x04001052 RID: 4178
	public static readonly Translate.Phrase PendingDeliveryPluralPhrase = new Translate.Phrase("market.pending_delivery.plural", "Waiting for {n} deliveries...");

	// Token: 0x04001053 RID: 4179
	public static readonly Translate.Phrase PendingDeliverySingularPhrase = new Translate.Phrase("market.pending_delivery.singular", "Waiting for delivery...");

	// Token: 0x04001054 RID: 4180
	public Canvas canvas;

	// Token: 0x04001055 RID: 4181
	public MapView mapView;

	// Token: 0x04001056 RID: 4182
	public RectTransform shopDetailsPanel;

	// Token: 0x04001057 RID: 4183
	public float shopDetailsMargin = 16f;

	// Token: 0x04001058 RID: 4184
	public float easeDuration = 0.2f;

	// Token: 0x04001059 RID: 4185
	public LeanTweenType easeType = LeanTweenType.linear;

	// Token: 0x0400105A RID: 4186
	public RustText shopName;

	// Token: 0x0400105B RID: 4187
	public GameObject shopOrderingPanel;

	// Token: 0x0400105C RID: 4188
	public RectTransform sellOrderContainer;

	// Token: 0x0400105D RID: 4189
	public GameObjectRef sellOrderPrefab;

	// Token: 0x0400105E RID: 4190
	public VirtualItemIcon deliveryFeeIcon;

	// Token: 0x0400105F RID: 4191
	public GameObject deliveryFeeCantAffordIndicator;

	// Token: 0x04001060 RID: 4192
	public GameObject inventoryFullIndicator;

	// Token: 0x04001061 RID: 4193
	public GameObject notEligiblePanel;

	// Token: 0x04001062 RID: 4194
	public GameObject pendingDeliveryPanel;

	// Token: 0x04001063 RID: 4195
	public RustText pendingDeliveryLabel;

	// Token: 0x04001064 RID: 4196
	public RectTransform itemNoticesContainer;

	// Token: 0x04001065 RID: 4197
	public GameObjectRef itemRemovedPrefab;

	// Token: 0x04001066 RID: 4198
	public GameObjectRef itemPendingPrefab;

	// Token: 0x04001067 RID: 4199
	public GameObjectRef itemAddedPrefab;

	// Token: 0x04001068 RID: 4200
	public CanvasGroup gettingStartedTip;

	// Token: 0x04001069 RID: 4201
	public SoundDefinition buyItemSoundDef;

	// Token: 0x0400106A RID: 4202
	public SoundDefinition buttonPressSoundDef;
}
