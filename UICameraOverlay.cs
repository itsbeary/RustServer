using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008CC RID: 2252
public class UICameraOverlay : SingletonComponent<UICameraOverlay>
{
	// Token: 0x06003753 RID: 14163 RVA: 0x0014C404 File Offset: 0x0014A604
	public void Show()
	{
		this.CanvasGroup.alpha = 1f;
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0014C416 File Offset: 0x0014A616
	public void Hide()
	{
		this.CanvasGroup.alpha = 0f;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0014C428 File Offset: 0x0014A628
	public void SetFocusMode(CameraFocusMode mode)
	{
		if (mode == CameraFocusMode.Auto)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusAutoText);
			return;
		}
		if (mode != CameraFocusMode.Manual)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusOffText);
			return;
		}
		this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusManualText);
	}

	// Token: 0x040032B3 RID: 12979
	public static readonly Translate.Phrase FocusOffText = new Translate.Phrase("camera.infinite_focus", "Infinite Focus");

	// Token: 0x040032B4 RID: 12980
	public static readonly Translate.Phrase FocusAutoText = new Translate.Phrase("camera.auto_focus", "Auto Focus");

	// Token: 0x040032B5 RID: 12981
	public static readonly Translate.Phrase FocusManualText = new Translate.Phrase("camera.manual_focus", "Manual Focus");

	// Token: 0x040032B6 RID: 12982
	public CanvasGroup CanvasGroup;

	// Token: 0x040032B7 RID: 12983
	public RustText FocusModeLabel;
}
