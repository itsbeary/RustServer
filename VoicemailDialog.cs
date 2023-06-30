using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007D2 RID: 2002
public class VoicemailDialog : MonoBehaviour
{
	// Token: 0x04002D07 RID: 11527
	public GameObject RecordingRoot;

	// Token: 0x04002D08 RID: 11528
	public RustSlider RecordingProgress;

	// Token: 0x04002D09 RID: 11529
	public GameObject BrowsingRoot;

	// Token: 0x04002D0A RID: 11530
	public PhoneDialler ParentDialler;

	// Token: 0x04002D0B RID: 11531
	public GameObjectRef VoicemailEntry;

	// Token: 0x04002D0C RID: 11532
	public Transform VoicemailEntriesRoot;

	// Token: 0x04002D0D RID: 11533
	public GameObject NoVoicemailRoot;

	// Token: 0x04002D0E RID: 11534
	public GameObject NoCassetteRoot;
}
