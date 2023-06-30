using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class CeilingLight : IOEntity
{
	// Token: 0x06000991 RID: 2449 RVA: 0x0005A120 File Offset: 0x00058320
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CeilingLight.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0005A160 File Offset: 0x00058360
	public override int ConsumptionAmount()
	{
		if (base.IsOn())
		{
			return 2;
		}
		return base.ConsumptionAmount();
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0005A174 File Offset: 0x00058374
	public override void Hurt(HitInfo info)
	{
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				base.ClientRPC<int, Vector3, Vector3>(null, "ClientPhysPush", 0, info.attackNormal * 3f * (info.damageTypes.Total() / 50f), info.HitPositionWorld);
			}
			base.Hurt(info);
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0005A1D8 File Offset: 0x000583D8
	public void RefreshGrowables()
	{
		List<GrowableEntity> list = Facepunch.Pool.GetList<GrowableEntity>();
		global::Vis.Entities<GrowableEntity>(base.transform.position + new Vector3(0f, -ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 524288, QueryTriggerInteraction.Collide);
		List<PlanterBox> list2 = Facepunch.Pool.GetList<PlanterBox>();
		foreach (GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter = growableEntity.GetPlanter();
				if (planter != null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceLightUpdate();
				}
				growableEntity.CalculateQualities(false, true, false);
				growableEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		Facepunch.Pool.FreeList<PlanterBox>(ref list2);
		Facepunch.Pool.FreeList<GrowableEntity>(ref list);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0005A2B4 File Offset: 0x000584B4
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
		if (flag != base.IsOn())
		{
			if (base.IsOn())
			{
				this.LightsOn();
				return;
			}
			this.LightsOff();
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0005A300 File Offset: 0x00058500
	public void LightsOn()
	{
		this.RefreshGrowables();
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0005A300 File Offset: 0x00058500
	public void LightsOff()
	{
		this.RefreshGrowables();
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0005A308 File Offset: 0x00058508
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.RefreshGrowables();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0005A318 File Offset: 0x00058518
	public override void OnAttacked(HitInfo info)
	{
		float num = 3f * (info.damageTypes.Total() / 50f);
		base.ClientRPC<NetworkableId, Vector3, Vector3>(null, "ClientPhysPush", (info.Initiator != null && info.Initiator is BasePlayer && !info.IsPredicting) ? info.Initiator.net.ID : default(NetworkableId), info.attackNormal * num, info.HitPositionWorld);
		base.OnAttacked(info);
	}

	// Token: 0x04000653 RID: 1619
	public float pushScale = 2f;
}
