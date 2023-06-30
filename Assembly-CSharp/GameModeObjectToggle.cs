using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000518 RID: 1304
public class GameModeObjectToggle : BaseMonoBehaviour
{
	// Token: 0x060029BA RID: 10682 RVA: 0x000FFFFE File Offset: 0x000FE1FE
	public void Awake()
	{
		this.SetToggle(this.defaultState);
		BaseGameMode.GameModeChanged += this.OnGameModeChanged;
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x0010001D File Offset: 0x000FE21D
	public void OnDestroy()
	{
		BaseGameMode.GameModeChanged -= this.OnGameModeChanged;
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x00100030 File Offset: 0x000FE230
	public void OnGameModeChanged(BaseGameMode newGameMode)
	{
		bool flag = this.ShouldBeVisible(newGameMode);
		this.SetToggle(flag);
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x0010004C File Offset: 0x000FE24C
	public void SetToggle(bool wantsOn)
	{
		foreach (GameObject gameObject in this.toToggle)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(wantsOn);
			}
		}
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x00100084 File Offset: 0x000FE284
	public bool ShouldBeVisible(BaseGameMode newGameMode)
	{
		if (newGameMode == null)
		{
			return this.defaultState;
		}
		return (this.tagsToDisable.Length == 0 || (!newGameMode.HasAnyGameModeTag(this.tagsToDisable) && !this.tagsToDisable.Contains("*"))) && ((this.gameModeTags.Length != 0 && (newGameMode.HasAnyGameModeTag(this.gameModeTags) || this.gameModeTags.Contains("*"))) || this.defaultState);
	}

	// Token: 0x040021D7 RID: 8663
	public string[] gameModeTags;

	// Token: 0x040021D8 RID: 8664
	public string[] tagsToDisable;

	// Token: 0x040021D9 RID: 8665
	public GameObject[] toToggle;

	// Token: 0x040021DA RID: 8666
	public bool defaultState;
}
