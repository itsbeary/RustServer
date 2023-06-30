using System;
using Facepunch.CardGames;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class BlackjackMachine : BaseCardGameEntity
{
	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06002362 RID: 9058 RVA: 0x000E2418 File Offset: 0x000E0618
	// (set) Token: 0x06002363 RID: 9059 RVA: 0x000E241F File Offset: 0x000E061F
	[ServerVar(Help = "Maximum initial bet per round")]
	public static int maxbet
	{
		get
		{
			return BlackjackMachine._maxbet;
		}
		set
		{
			BlackjackMachine._maxbet = Mathf.Clamp(value, 25, 1000000);
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06002364 RID: 9060 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000E2433 File Offset: 0x000E0633
	public override void InitShared()
	{
		base.InitShared();
		this.controller = (BlackjackController)base.GameController;
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000E244C File Offset: 0x000E064C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000E2455 File Offset: 0x000E0655
	public override void PlayerStorageChanged()
	{
		base.PlayerStorageChanged();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04001B38 RID: 6968
	[Header("Blackjack Machine")]
	[SerializeField]
	private GameObjectRef mainScreenPrefab;

	// Token: 0x04001B39 RID: 6969
	[SerializeField]
	private GameObjectRef smallScreenPrefab;

	// Token: 0x04001B3A RID: 6970
	[SerializeField]
	private Transform mainScreenParent;

	// Token: 0x04001B3B RID: 6971
	[SerializeField]
	private Transform[] smallScreenParents;

	// Token: 0x04001B3C RID: 6972
	private static int _maxbet = 500;

	// Token: 0x04001B3D RID: 6973
	private BlackjackController controller;

	// Token: 0x04001B3E RID: 6974
	private BlackjackMainScreenUI mainScreenUI;

	// Token: 0x04001B3F RID: 6975
	private BlackjackSmallScreenUI[] smallScreenUIs = new BlackjackSmallScreenUI[3];
}
