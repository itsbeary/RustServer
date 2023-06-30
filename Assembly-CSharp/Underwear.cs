using System;
using UnityEngine;

// Token: 0x0200076A RID: 1898
[CreateAssetMenu(menuName = "Rust/Underwear")]
public class Underwear : ScriptableObject
{
	// Token: 0x060034AE RID: 13486 RVA: 0x0014510A File Offset: 0x0014330A
	public uint GetID()
	{
		return StringPool.Get(this.shortname);
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x00145117 File Offset: 0x00143317
	public bool HasMaleParts()
	{
		return this.replacementsMale.Length != 0;
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x00145123 File Offset: 0x00143323
	public bool HasFemaleParts()
	{
		return this.replacementsFemale.Length != 0;
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x00145130 File Offset: 0x00143330
	public bool ValidForPlayer(BasePlayer player)
	{
		if (this.HasMaleParts() && this.HasFemaleParts())
		{
			return true;
		}
		bool flag = Underwear.IsFemale(player);
		return (flag && this.HasFemaleParts()) || (!flag && this.HasMaleParts());
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x00145174 File Offset: 0x00143374
	public static bool IsFemale(BasePlayer player)
	{
		ulong userID = player.userID;
		ulong num = 4332UL;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(num + userID));
		float num2 = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return num2 > 0.5f;
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x001451BC File Offset: 0x001433BC
	public static bool Validate(Underwear underwear, BasePlayer player)
	{
		if (underwear == null)
		{
			return true;
		}
		if (!underwear.ValidForPlayer(player))
		{
			return false;
		}
		if (underwear.adminOnly && (!player.IsAdmin || !player.IsDeveloper))
		{
			return false;
		}
		bool flag = underwear.steamItem == null || player.blueprints.steamInventory.HasItem(underwear.steamItem.id);
		bool flag2 = false;
		if (player.isServer && (underwear.steamDLC == null || underwear.steamDLC.HasLicense(player.userID)))
		{
			flag2 = true;
		}
		return flag && flag2;
	}

	// Token: 0x04002B4A RID: 11082
	public string shortname = "";

	// Token: 0x04002B4B RID: 11083
	public Translate.Phrase displayName;

	// Token: 0x04002B4C RID: 11084
	public Sprite icon;

	// Token: 0x04002B4D RID: 11085
	public Sprite iconFemale;

	// Token: 0x04002B4E RID: 11086
	public SkinReplacement[] replacementsMale;

	// Token: 0x04002B4F RID: 11087
	public SkinReplacement[] replacementsFemale;

	// Token: 0x04002B50 RID: 11088
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x04002B51 RID: 11089
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDLC;

	// Token: 0x04002B52 RID: 11090
	public bool adminOnly;
}
