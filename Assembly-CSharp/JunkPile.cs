using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class JunkPile : BaseEntity
{
	// Token: 0x06000D22 RID: 3362 RVA: 0x00070C20 File Offset: 0x0006EE20
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("JunkPile.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00070C60 File Offset: 0x0006EE60
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.TimeOut), 1800f);
		base.InvokeRepeating(new Action(this.CheckEmpty), 10f, 30f);
		base.Invoke(new Action(this.SpawnInitial), 1f);
		this.isSinking = false;
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00070CC4 File Offset: 0x0006EEC4
	private void SpawnInitial()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00070CF0 File Offset: 0x0006EEF0
	public bool SpawnGroupsEmpty()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].currentPopulation > 0)
			{
				return false;
			}
		}
		return !(this.NPCSpawn != null) || this.NPCSpawn.currentPopulation <= 0;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00070D3E File Offset: 0x0006EF3E
	public void CheckEmpty()
	{
		if (this.SpawnGroupsEmpty() && !this.PlayersNearby())
		{
			base.CancelInvoke(new Action(this.CheckEmpty));
			this.SinkAndDestroy();
		}
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00070D68 File Offset: 0x0006EF68
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && !(basePlayer is HumanNPC))
			{
				flag = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return flag;
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00070DF8 File Offset: 0x0006EFF8
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x00070DFF File Offset: 0x0006EFFF
	public void TimeOut()
	{
		if (this.PlayersNearby())
		{
			base.Invoke(new Action(this.TimeOut), 30f);
			return;
		}
		this.SpawnGroupsEmpty();
		this.SinkAndDestroy();
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00070E30 File Offset: 0x0006F030
	public void SinkAndDestroy()
	{
		base.CancelInvoke(new Action(this.SinkAndDestroy));
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, true, true, true);
		if (this.NPCSpawn != null)
		{
			this.NPCSpawn.Clear();
		}
		base.ClientRPC(null, "CLIENT_StartSink");
		base.transform.position -= new Vector3(0f, 5f, 0f);
		this.isSinking = true;
		base.Invoke(new Action(this.KillMe), 22f);
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00003384 File Offset: 0x00001584
	public void KillMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x04000863 RID: 2147
	public GameObjectRef sinkEffect;

	// Token: 0x04000864 RID: 2148
	public SpawnGroup[] spawngroups;

	// Token: 0x04000865 RID: 2149
	public NPCSpawner NPCSpawn;

	// Token: 0x04000866 RID: 2150
	private const float lifetimeMinutes = 30f;

	// Token: 0x04000867 RID: 2151
	protected bool isSinking;
}
