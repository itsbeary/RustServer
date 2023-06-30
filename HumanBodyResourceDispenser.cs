using System;

// Token: 0x020003D1 RID: 977
public class HumanBodyResourceDispenser : ResourceDispenser
{
	// Token: 0x060021E9 RID: 8681 RVA: 0x000DC8B8 File Offset: 0x000DAAB8
	public override bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		if (item.info.shortname == "skull.human")
		{
			PlayerCorpse component = base.GetComponent<PlayerCorpse>();
			if (component)
			{
				item.name = HumanBodyResourceDispenser.CreateSkullName(component.playerName);
				item.streamerName = HumanBodyResourceDispenser.CreateSkullName(component.streamerName);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000DC910 File Offset: 0x000DAB10
	public static string CreateSkullName(string playerName)
	{
		return "Skull of \"" + playerName + "\"";
	}
}
