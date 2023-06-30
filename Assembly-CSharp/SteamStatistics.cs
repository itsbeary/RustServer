using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConVar;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class SteamStatistics
{
	// Token: 0x0600251C RID: 9500 RVA: 0x000EAFE9 File Offset: 0x000E91E9
	public SteamStatistics(BasePlayer p)
	{
		this.player = p;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000EB003 File Offset: 0x000E9203
	public void Init()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		this.refresh = PlatformService.Instance.LoadPlayerStats(this.player.userID);
		this.intStats.Clear();
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000EB038 File Offset: 0x000E9238
	public void Save()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		PlatformService.Instance.SavePlayerStats(this.player.userID);
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000EB060 File Offset: 0x000E9260
	public void Add(string name, int var)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return;
		}
		using (TimeWarning.New("PlayerStats.Add", 0))
		{
			int num = 0;
			if (this.intStats.TryGetValue(name, out num))
			{
				Dictionary<string, int> dictionary = this.intStats;
				dictionary[name] += var;
				PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)this.intStats[name]);
			}
			else
			{
				num = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
				if (!PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)(num + var)))
				{
					if (Global.developer > 0)
					{
						Debug.LogWarning("[STEAMWORKS] Couldn't SetUserStat: " + name);
					}
				}
				else
				{
					this.intStats.Add(name, num + var);
				}
			}
		}
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000EB168 File Offset: 0x000E9368
	public int Get(string name)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return 0;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return 0;
		}
		int num2;
		using (TimeWarning.New("PlayerStats.Get", 0))
		{
			int num;
			if (this.intStats.TryGetValue(name, out num))
			{
				num2 = num;
			}
			else
			{
				num2 = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
			}
		}
		return num2;
	}

	// Token: 0x04001D60 RID: 7520
	private BasePlayer player;

	// Token: 0x04001D61 RID: 7521
	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	// Token: 0x04001D62 RID: 7522
	private Task refresh;
}
