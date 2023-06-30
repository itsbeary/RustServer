using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200081F RID: 2079
public class ChangelogButton : MonoBehaviour
{
	// Token: 0x060035D6 RID: 13782 RVA: 0x001475A8 File Offset: 0x001457A8
	private void Update()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(false);
		if (activeGameMode != null)
		{
			if (this.CanvasGroup.alpha != 1f)
			{
				this.CanvasGroup.alpha = 1f;
				this.CanvasGroup.blocksRaycasts = true;
				this.Button.Text.SetPhrase(new Translate.Phrase(activeGameMode.shortname, activeGameMode.shortname));
				return;
			}
		}
		else if (this.CanvasGroup.alpha != 0f)
		{
			this.CanvasGroup.alpha = 0f;
			this.CanvasGroup.blocksRaycasts = false;
		}
	}

	// Token: 0x04002EE7 RID: 12007
	public RustButton Button;

	// Token: 0x04002EE8 RID: 12008
	public CanvasGroup CanvasGroup;
}
