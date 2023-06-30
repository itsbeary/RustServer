using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008CD RID: 2253
public class UIConversationScreen : SingletonComponent<UIConversationScreen>, IUIScreen
{
	// Token: 0x040032B8 RID: 12984
	public NeedsCursor needsCursor;

	// Token: 0x040032B9 RID: 12985
	public RectTransform conversationPanel;

	// Token: 0x040032BA RID: 12986
	public RustText conversationSpeechBody;

	// Token: 0x040032BB RID: 12987
	public RustText conversationProviderName;

	// Token: 0x040032BC RID: 12988
	public RustButton[] responseButtons;

	// Token: 0x040032BD RID: 12989
	public RectTransform letterBoxTop;

	// Token: 0x040032BE RID: 12990
	public RectTransform letterBoxBottom;

	// Token: 0x040032BF RID: 12991
	protected CanvasGroup canvasGroup;
}
