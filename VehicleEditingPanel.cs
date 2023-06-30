using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000860 RID: 2144
public class VehicleEditingPanel : LootPanel
{
	// Token: 0x04003045 RID: 12357
	[SerializeField]
	[Range(0f, 1f)]
	private float disabledAlpha = 0.25f;

	// Token: 0x04003046 RID: 12358
	[Header("Edit Vehicle")]
	[SerializeField]
	private CanvasGroup editGroup;

	// Token: 0x04003047 RID: 12359
	[SerializeField]
	private GameObject moduleInternalItemsGroup;

	// Token: 0x04003048 RID: 12360
	[SerializeField]
	private GameObject moduleInternalLiquidsGroup;

	// Token: 0x04003049 RID: 12361
	[SerializeField]
	private GameObject destroyChassisGroup;

	// Token: 0x0400304A RID: 12362
	[SerializeField]
	private Button itemTakeButton;

	// Token: 0x0400304B RID: 12363
	[SerializeField]
	private Button liquidTakeButton;

	// Token: 0x0400304C RID: 12364
	[SerializeField]
	private GameObject liquidHelp;

	// Token: 0x0400304D RID: 12365
	[SerializeField]
	private GameObject liquidButton;

	// Token: 0x0400304E RID: 12366
	[SerializeField]
	private Color gotColor;

	// Token: 0x0400304F RID: 12367
	[SerializeField]
	private Color notGotColor;

	// Token: 0x04003050 RID: 12368
	[SerializeField]
	private Text generalInfoText;

	// Token: 0x04003051 RID: 12369
	[SerializeField]
	private Text generalWarningText;

	// Token: 0x04003052 RID: 12370
	[SerializeField]
	private Image generalWarningImage;

	// Token: 0x04003053 RID: 12371
	[SerializeField]
	private Text repairInfoText;

	// Token: 0x04003054 RID: 12372
	[SerializeField]
	private Button repairButton;

	// Token: 0x04003055 RID: 12373
	[SerializeField]
	private Text destroyChassisButtonText;

	// Token: 0x04003056 RID: 12374
	[SerializeField]
	private Text destroyChassisCountdown;

	// Token: 0x04003057 RID: 12375
	[SerializeField]
	private Translate.Phrase phraseEditingInfo;

	// Token: 0x04003058 RID: 12376
	[SerializeField]
	private Translate.Phrase phraseNoOccupant;

	// Token: 0x04003059 RID: 12377
	[SerializeField]
	private Translate.Phrase phraseBadOccupant;

	// Token: 0x0400305A RID: 12378
	[SerializeField]
	private Translate.Phrase phrasePlayerObstructing;

	// Token: 0x0400305B RID: 12379
	[SerializeField]
	private Translate.Phrase phraseNotDriveable;

	// Token: 0x0400305C RID: 12380
	[SerializeField]
	private Translate.Phrase phraseNotRepairable;

	// Token: 0x0400305D RID: 12381
	[SerializeField]
	private Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x0400305E RID: 12382
	[SerializeField]
	private Translate.Phrase phraseRepairSelectInfo;

	// Token: 0x0400305F RID: 12383
	[SerializeField]
	private Translate.Phrase phraseRepairEnactInfo;

	// Token: 0x04003060 RID: 12384
	[SerializeField]
	private Translate.Phrase phraseHasLock;

	// Token: 0x04003061 RID: 12385
	[SerializeField]
	private Translate.Phrase phraseHasNoLock;

	// Token: 0x04003062 RID: 12386
	[SerializeField]
	private Translate.Phrase phraseAddLock;

	// Token: 0x04003063 RID: 12387
	[SerializeField]
	private Translate.Phrase phraseAddLockButton;

	// Token: 0x04003064 RID: 12388
	[SerializeField]
	private Translate.Phrase phraseChangeLockCodeButton;

	// Token: 0x04003065 RID: 12389
	[SerializeField]
	private Text carLockInfoText;

	// Token: 0x04003066 RID: 12390
	[SerializeField]
	private RustText carLockButtonText;

	// Token: 0x04003067 RID: 12391
	[SerializeField]
	private Button actionLockButton;

	// Token: 0x04003068 RID: 12392
	[SerializeField]
	private Button removeLockButton;

	// Token: 0x04003069 RID: 12393
	[SerializeField]
	private GameObjectRef keyEnterDialog;

	// Token: 0x0400306A RID: 12394
	[SerializeField]
	private Translate.Phrase phraseEmptyStorage;

	// Token: 0x0400306B RID: 12395
	[Header("Create Chassis")]
	[SerializeField]
	private VehicleEditingPanel.CreateChassisEntry[] chassisOptions;

	// Token: 0x02000E9B RID: 3739
	[Serializable]
	private class CreateChassisEntry
	{
		// Token: 0x06005304 RID: 21252 RVA: 0x001B198D File Offset: 0x001AFB8D
		public ItemDefinition GetChassisItemDef(ModularCarGarage garage)
		{
			return garage.chassisBuildOptions[(int)this.garageChassisIndex].itemDef;
		}

		// Token: 0x04004C99 RID: 19609
		public byte garageChassisIndex;

		// Token: 0x04004C9A RID: 19610
		public Button craftButton;

		// Token: 0x04004C9B RID: 19611
		public Text craftButtonText;

		// Token: 0x04004C9C RID: 19612
		public Text requirementsText;
	}
}
