using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007CC RID: 1996
public class UIFireworkDesignItem : MonoBehaviour
{
	// Token: 0x04002CCC RID: 11468
	public static readonly Translate.Phrase EmptyPhrase = new Translate.Phrase("firework.pattern.design.empty", "Empty");

	// Token: 0x04002CCD RID: 11469
	public static readonly Translate.Phrase UntitledPhrase = new Translate.Phrase("firework.pattern.design.untitled", "Untitled");

	// Token: 0x04002CCE RID: 11470
	public RustText Title;

	// Token: 0x04002CCF RID: 11471
	public RustButton LoadButton;

	// Token: 0x04002CD0 RID: 11472
	public RustButton SaveButton;

	// Token: 0x04002CD1 RID: 11473
	public RustButton EraseButton;

	// Token: 0x04002CD2 RID: 11474
	public UIFireworkDesigner Designer;

	// Token: 0x04002CD3 RID: 11475
	public int Index;
}
