using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007CF RID: 1999
public class KeyCodeEntry : UIDialog
{
	// Token: 0x04002CDB RID: 11483
	public Text textDisplay;

	// Token: 0x04002CDC RID: 11484
	public Action<string> onCodeEntered;

	// Token: 0x04002CDD RID: 11485
	public Action onClosed;

	// Token: 0x04002CDE RID: 11486
	public Text typeDisplay;

	// Token: 0x04002CDF RID: 11487
	public Translate.Phrase masterCodePhrase;

	// Token: 0x04002CE0 RID: 11488
	public Translate.Phrase guestCodePhrase;

	// Token: 0x04002CE1 RID: 11489
	public GameObject memoryKeycodeButton;
}
