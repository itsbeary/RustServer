using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000795 RID: 1941
public class UIAIDesignerScreen : SingletonComponent<UIAIDesignerScreen>, IUIScreen
{
	// Token: 0x04002B64 RID: 11108
	public GameObject SaveEntityButton;

	// Token: 0x04002B65 RID: 11109
	public GameObject SaveServerButton;

	// Token: 0x04002B66 RID: 11110
	public GameObject SaveDefaultButton;

	// Token: 0x04002B67 RID: 11111
	public RustInput InputAIDescription;

	// Token: 0x04002B68 RID: 11112
	public RustText TextDefaultStateContainer;

	// Token: 0x04002B69 RID: 11113
	public Transform PrefabAddNewStateButton;

	// Token: 0x04002B6A RID: 11114
	public Transform StateContainer;

	// Token: 0x04002B6B RID: 11115
	public Transform PrefabState;

	// Token: 0x04002B6C RID: 11116
	public EnumListUI PopupList;

	// Token: 0x04002B6D RID: 11117
	public static EnumListUI EnumList;

	// Token: 0x04002B6E RID: 11118
	public NeedsCursor needsCursor;

	// Token: 0x04002B6F RID: 11119
	protected CanvasGroup canvasGroup;

	// Token: 0x04002B70 RID: 11120
	public GameObject RootPanel;
}
