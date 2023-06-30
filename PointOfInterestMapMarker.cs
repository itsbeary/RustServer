using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FF RID: 2047
public class PointOfInterestMapMarker : MonoBehaviour
{
	// Token: 0x04002E25 RID: 11813
	public Image MapIcon;

	// Token: 0x04002E26 RID: 11814
	public Image MapIconOuter;

	// Token: 0x04002E27 RID: 11815
	public GameObject LeaderRoot;

	// Token: 0x04002E28 RID: 11816
	public GameObject EditPopup;

	// Token: 0x04002E29 RID: 11817
	public Tooltip Tooltip;

	// Token: 0x04002E2A RID: 11818
	public GameObject MarkerLabelRoot;

	// Token: 0x04002E2B RID: 11819
	public RustText MarkerLabel;

	// Token: 0x04002E2C RID: 11820
	public RustText NoMarkerLabel;

	// Token: 0x04002E2D RID: 11821
	public RustInput MarkerLabelModify;

	// Token: 0x04002E2E RID: 11822
	public MapMarkerIconSelector[] IconSelectors;

	// Token: 0x04002E2F RID: 11823
	public MapMarkerIconSelector[] ColourSelectors;

	// Token: 0x04002E30 RID: 11824
	public bool IsListWidget;

	// Token: 0x04002E31 RID: 11825
	public GameObject DeleteButton;
}
