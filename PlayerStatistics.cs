using System;
using ConVar;

// Token: 0x02000459 RID: 1113
public class PlayerStatistics
{
	// Token: 0x06002512 RID: 9490 RVA: 0x000EAE85 File Offset: 0x000E9085
	public PlayerStatistics(BasePlayer player)
	{
		this.steam = new SteamStatistics(player);
		this.server = new ServerStatistics(player);
		this.combat = new CombatLog(player);
		this.forPlayer = player;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000EAEB8 File Offset: 0x000E90B8
	public void Init()
	{
		this.steam.Init();
		this.server.Init();
		this.combat.Init();
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000EAEDC File Offset: 0x000E90DC
	public void Save(bool forceSteamSave = false)
	{
		if (Server.official && (forceSteamSave || this.lastSteamSave > 60f))
		{
			this.lastSteamSave = 0f;
			this.steam.Save();
		}
		this.server.Save();
		this.combat.Save();
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000EAF36 File Offset: 0x000E9136
	public void Add(string name, int val, Stats stats = Stats.Steam)
	{
		if ((stats & Stats.Steam) != (Stats)0)
		{
			this.steam.Add(name, val);
		}
		if ((stats & Stats.Server) != (Stats)0)
		{
			this.server.Add(name, val);
		}
		if ((stats & Stats.Life) != (Stats)0)
		{
			this.forPlayer.LifeStoryGenericStat(name, val);
		}
	}

	// Token: 0x04001D58 RID: 7512
	public SteamStatistics steam;

	// Token: 0x04001D59 RID: 7513
	public ServerStatistics server;

	// Token: 0x04001D5A RID: 7514
	public CombatLog combat;

	// Token: 0x04001D5B RID: 7515
	private BasePlayer forPlayer;

	// Token: 0x04001D5C RID: 7516
	private TimeSince lastSteamSave;
}
