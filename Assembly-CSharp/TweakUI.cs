using System;
using UnityEngine;

// Token: 0x0200088B RID: 2187
public class TweakUI : SingletonComponent<TweakUI>
{
	// Token: 0x0600369C RID: 13980 RVA: 0x0014973F File Offset: 0x0014793F
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2) && this.CanToggle())
		{
			this.SetVisible(!TweakUI.isOpen);
		}
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x00149763 File Offset: 0x00147963
	protected bool CanToggle()
	{
		return LevelManager.isLoaded;
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x0014976F File Offset: 0x0014796F
	public void SetVisible(bool b)
	{
		if (b)
		{
			TweakUI.isOpen = true;
			return;
		}
		TweakUI.isOpen = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
	}

	// Token: 0x04003160 RID: 12640
	public static bool isOpen;
}
