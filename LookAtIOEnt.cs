using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086A RID: 2154
public class LookAtIOEnt : MonoBehaviour
{
	// Token: 0x0400309E RID: 12446
	public Text objectTitle;

	// Token: 0x0400309F RID: 12447
	public RectTransform slotToolTip;

	// Token: 0x040030A0 RID: 12448
	public Text slotTitle;

	// Token: 0x040030A1 RID: 12449
	public Text slotConnection;

	// Token: 0x040030A2 RID: 12450
	public Text slotPower;

	// Token: 0x040030A3 RID: 12451
	public Text powerText;

	// Token: 0x040030A4 RID: 12452
	public Text passthroughText;

	// Token: 0x040030A5 RID: 12453
	public Text chargeLeftText;

	// Token: 0x040030A6 RID: 12454
	public Text capacityText;

	// Token: 0x040030A7 RID: 12455
	public Text maxOutputText;

	// Token: 0x040030A8 RID: 12456
	public Text activeOutputText;

	// Token: 0x040030A9 RID: 12457
	public IOEntityUISlotEntry[] inputEntries;

	// Token: 0x040030AA RID: 12458
	public IOEntityUISlotEntry[] outputEntries;

	// Token: 0x040030AB RID: 12459
	public Color NoPowerColor;

	// Token: 0x040030AC RID: 12460
	public GameObject GravityWarning;

	// Token: 0x040030AD RID: 12461
	public GameObject DistanceWarning;

	// Token: 0x040030AE RID: 12462
	public GameObject LineOfSightWarning;

	// Token: 0x040030AF RID: 12463
	public GameObject TooManyInputsWarning;

	// Token: 0x040030B0 RID: 12464
	public GameObject TooManyOutputsWarning;

	// Token: 0x040030B1 RID: 12465
	public GameObject BuildPrivilegeWarning;

	// Token: 0x040030B2 RID: 12466
	public CanvasGroup group;

	// Token: 0x040030B3 RID: 12467
	public LookAtIOEnt.HandleSet[] handleSets;

	// Token: 0x040030B4 RID: 12468
	public RectTransform clearNotification;

	// Token: 0x040030B5 RID: 12469
	public CanvasGroup wireInfoGroup;

	// Token: 0x040030B6 RID: 12470
	public Text wireLengthText;

	// Token: 0x040030B7 RID: 12471
	public Text wireClipsText;

	// Token: 0x040030B8 RID: 12472
	public Text errorReasonTextTooFar;

	// Token: 0x040030B9 RID: 12473
	public Text errorReasonTextNoSurface;

	// Token: 0x040030BA RID: 12474
	public Text errorShortCircuit;

	// Token: 0x040030BB RID: 12475
	public RawImage ConnectionTypeIcon;

	// Token: 0x040030BC RID: 12476
	public Texture ElectricSprite;

	// Token: 0x040030BD RID: 12477
	public Texture FluidSprite;

	// Token: 0x040030BE RID: 12478
	public Texture IndustrialSprite;

	// Token: 0x02000EA1 RID: 3745
	[Serializable]
	public struct HandleSet
	{
		// Token: 0x04004CBF RID: 19647
		public IOEntity.IOType ForIO;

		// Token: 0x04004CC0 RID: 19648
		public GameObjectRef handlePrefab;

		// Token: 0x04004CC1 RID: 19649
		public GameObjectRef handleOccupiedPrefab;

		// Token: 0x04004CC2 RID: 19650
		public GameObjectRef selectedHandlePrefab;

		// Token: 0x04004CC3 RID: 19651
		public GameObjectRef pluggedHandlePrefab;
	}
}
