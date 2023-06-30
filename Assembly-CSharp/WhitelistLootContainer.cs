using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public class WhitelistLootContainer : LootContainer
{
	// Token: 0x06002451 RID: 9297 RVA: 0x000E7A84 File Offset: 0x000E5C84
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.whitelist = Pool.Get<Whitelist>();
			info.msg.whitelist.users = Pool.GetList<ulong>();
			foreach (ulong num in this.whitelist)
			{
				info.msg.whitelist.users.Add(num);
				Debug.Log("Whitelistcontainer saving user " + num);
			}
		}
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x000E7B30 File Offset: 0x000E5D30
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.fromDisk && info.msg.whitelist != null)
		{
			foreach (ulong num in info.msg.whitelist.users)
			{
				this.whitelist.Add(num);
			}
		}
		base.Load(info);
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000E7BB0 File Offset: 0x000E5DB0
	public void MissionSetupPlayer(global::BasePlayer player)
	{
		this.AddToWhitelist(player.userID);
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000E7BBE File Offset: 0x000E5DBE
	public void AddToWhitelist(ulong userid)
	{
		if (!this.whitelist.Contains(userid))
		{
			this.whitelist.Add(userid);
		}
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000E7BDA File Offset: 0x000E5DDA
	public void RemoveFromWhitelist(ulong userid)
	{
		if (this.whitelist.Contains(userid))
		{
			this.whitelist.Remove(userid);
		}
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000E7BF8 File Offset: 0x000E5DF8
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		ulong userID = player.userID;
		if (!this.whitelist.Contains(userID))
		{
			player.ShowToast(GameTip.Styles.Red_Normal, WhitelistLootContainer.CantLootToast, Array.Empty<string>());
			return false;
		}
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x04001C42 RID: 7234
	public static readonly Translate.Phrase CantLootToast = new Translate.Phrase("whitelistcontainer.noloot", "You are not authorized to access this box");

	// Token: 0x04001C43 RID: 7235
	public List<ulong> whitelist = new List<ulong>();
}
