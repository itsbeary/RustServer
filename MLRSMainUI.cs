using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

// Token: 0x0200048E RID: 1166
public class MLRSMainUI : MonoBehaviour
{
	// Token: 0x04001EDE RID: 7902
	[SerializeField]
	private bool isFullscreen;

	// Token: 0x04001EDF RID: 7903
	[SerializeField]
	private GameObject noAimingModuleModeGO;

	// Token: 0x04001EE0 RID: 7904
	[SerializeField]
	private GameObject activeModeGO;

	// Token: 0x04001EE1 RID: 7905
	[SerializeField]
	private MLRSAmmoUI noAimingModuleAmmoUI;

	// Token: 0x04001EE2 RID: 7906
	[SerializeField]
	private MLRSAmmoUI activeAmmoUI;

	// Token: 0x04001EE3 RID: 7907
	[SerializeField]
	private MLRSVelocityUI velocityUI;

	// Token: 0x04001EE4 RID: 7908
	[SerializeField]
	private RustText titleText;

	// Token: 0x04001EE5 RID: 7909
	[SerializeField]
	private RustText usernameText;

	// Token: 0x04001EE6 RID: 7910
	[SerializeField]
	private TokenisedPhrase readyStatus;

	// Token: 0x04001EE7 RID: 7911
	[SerializeField]
	private TokenisedPhrase realigningStatus;

	// Token: 0x04001EE8 RID: 7912
	[SerializeField]
	private TokenisedPhrase firingStatus;

	// Token: 0x04001EE9 RID: 7913
	[SerializeField]
	private RustText statusText;

	// Token: 0x04001EEA RID: 7914
	[SerializeField]
	private MapView mapView;

	// Token: 0x04001EEB RID: 7915
	[SerializeField]
	private ScrollRectEx mapScrollRect;

	// Token: 0x04001EEC RID: 7916
	[SerializeField]
	private ScrollRectZoom mapScrollRectZoom;

	// Token: 0x04001EED RID: 7917
	[SerializeField]
	private RectTransform mapBaseRect;

	// Token: 0x04001EEE RID: 7918
	[SerializeField]
	private RectTransform minRangeCircle;

	// Token: 0x04001EEF RID: 7919
	[SerializeField]
	private RectTransform targetAimRect;

	// Token: 0x04001EF0 RID: 7920
	[SerializeField]
	private RectTransform trueAimRect;

	// Token: 0x04001EF1 RID: 7921
	[SerializeField]
	private UILineRenderer connectingLine;

	// Token: 0x04001EF2 RID: 7922
	[SerializeField]
	private GameObject noTargetCirclePrefab;

	// Token: 0x04001EF3 RID: 7923
	[SerializeField]
	private Transform noTargetCircleParent;

	// Token: 0x04001EF4 RID: 7924
	[SerializeField]
	private SoundDefinition changeTargetSoundDef;

	// Token: 0x04001EF5 RID: 7925
	[SerializeField]
	private SoundDefinition readyToFireSoundDef;
}
