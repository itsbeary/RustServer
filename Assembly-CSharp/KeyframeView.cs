using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BB RID: 1979
public class KeyframeView : MonoBehaviour
{
	// Token: 0x04002C02 RID: 11266
	public ScrollRect Scroller;

	// Token: 0x04002C03 RID: 11267
	public GameObjectRef KeyframePrefab;

	// Token: 0x04002C04 RID: 11268
	public RectTransform KeyframeRoot;

	// Token: 0x04002C05 RID: 11269
	public Transform CurrentPositionIndicator;

	// Token: 0x04002C06 RID: 11270
	public bool LockScrollToCurrentPosition;

	// Token: 0x04002C07 RID: 11271
	public RustText TrackName;
}
