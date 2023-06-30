using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200080C RID: 2060
public class UICompass : MonoBehaviour
{
	// Token: 0x04002E6F RID: 11887
	public RawImage compassStrip;

	// Token: 0x04002E70 RID: 11888
	public CanvasGroup compassGroup;

	// Token: 0x04002E71 RID: 11889
	public List<CompassMapMarker> CompassMarkers;

	// Token: 0x04002E72 RID: 11890
	public List<CompassMapMarker> TeamCompassMarkers;

	// Token: 0x04002E73 RID: 11891
	public List<CompassMissionMarker> MissionMarkers;

	// Token: 0x04002E74 RID: 11892
	public List<CompassMapMarker> LocalPings;

	// Token: 0x04002E75 RID: 11893
	public List<CompassMapMarker> TeamPings;

	// Token: 0x04002E76 RID: 11894
	public Image LeftPingPulse;

	// Token: 0x04002E77 RID: 11895
	public Image RightPingPulse;
}
