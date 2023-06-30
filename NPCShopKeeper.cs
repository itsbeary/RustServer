using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class NPCShopKeeper : NPCPlayer
{
	// Token: 0x0600187C RID: 6268 RVA: 0x000B6D5A File Offset: 0x000B4F5A
	public InvisibleVendingMachine GetVendingMachine()
	{
		if (!this.invisibleVendingMachineRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.invisibleVendingMachineRef.Get(base.isServer).GetComponent<InvisibleVendingMachine>();
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x000B6D88 File Offset: 0x000B4F88
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void UpdateProtectionFromClothing()
	{
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Hurt(HitInfo info)
	{
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x000B6DD8 File Offset: 0x000B4FD8
	public override void ServerInit()
	{
		base.ServerInit();
		this.initialFacingDir = base.transform.rotation * Vector3.forward;
		base.Invoke(new Action(this.DelayedSleepEnd), 3f);
		this.SetAimDirection(base.transform.rotation * Vector3.forward);
		base.InvokeRandomized(new Action(this.Greeting), UnityEngine.Random.Range(5f, 10f), 5f, UnityEngine.Random.Range(0f, 2f));
		if (this.invisibleVendingMachineRef.IsValid(true) && this.machine == null)
		{
			this.machine = this.GetVendingMachine();
			return;
		}
		if (this.machine != null && !this.invisibleVendingMachineRef.IsValid(true))
		{
			this.invisibleVendingMachineRef.Set(this.machine);
		}
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x000B6EC3 File Offset: 0x000B50C3
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.shopKeeper = Pool.Get<ShopKeeper>();
		info.msg.shopKeeper.vendingRef = this.invisibleVendingMachineRef.uid;
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x000B6EF7 File Offset: 0x000B50F7
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.shopKeeper != null)
		{
			this.invisibleVendingMachineRef.uid = info.msg.shopKeeper.vendingRef;
		}
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x000B6F28 File Offset: 0x000B5128
	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x000B6F30 File Offset: 0x000B5130
	public void DelayedSleepEnd()
	{
		this.EndSleeping();
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x000B6F38 File Offset: 0x000B5138
	public void GreetPlayer(global::BasePlayer player)
	{
		if (player != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(player.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = player;
			return;
		}
		this.SetAimDirection(this.initialFacingDir);
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x000B6F94 File Offset: 0x000B5194
	public void Greeting()
	{
		List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
		Vis.Entities<global::BasePlayer>(base.transform.position, 10f, list, 131072, QueryTriggerInteraction.Collide);
		Vector3 position = base.transform.position;
		global::BasePlayer basePlayer = null;
		foreach (global::BasePlayer basePlayer2 in list)
		{
			if (!basePlayer2.isClient && !basePlayer2.IsNpc && !(basePlayer2 == this) && basePlayer2.IsVisible(this.eyes.position, float.PositiveInfinity) && !(basePlayer2 == this.lastWavedAtPlayer) && Vector3.Dot(Vector3Ex.Direction2D(basePlayer2.eyes.position, this.eyes.position), this.initialFacingDir) >= 0.2f)
			{
				basePlayer = basePlayer2;
				break;
			}
		}
		if (basePlayer == null && !list.Contains(this.lastWavedAtPlayer))
		{
			this.lastWavedAtPlayer = null;
		}
		if (basePlayer != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = basePlayer;
		}
		else
		{
			this.SetAimDirection(this.initialFacingDir);
		}
		Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x0400112F RID: 4399
	public EntityRef invisibleVendingMachineRef;

	// Token: 0x04001130 RID: 4400
	public InvisibleVendingMachine machine;

	// Token: 0x04001131 RID: 4401
	private float greetDir;

	// Token: 0x04001132 RID: 4402
	private Vector3 initialFacingDir;

	// Token: 0x04001133 RID: 4403
	private global::BasePlayer lastWavedAtPlayer;
}
