using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D6 RID: 214
public class SprayCanSpray : global::DecayEntity, ISplashable
{
	// Token: 0x06001300 RID: 4864 RVA: 0x0009906C File Offset: 0x0009726C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray.OnRpcMessage", 0))
		{
			if (rpc == 2774110739U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestWaterClear ");
				}
				using (TimeWarning.New("Server_RequestWaterClear", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestWaterClear(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_RequestWaterClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x00099190 File Offset: 0x00097390
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.spray == null)
		{
			info.msg.spray = Facepunch.Pool.Get<Spray>();
		}
		info.msg.spray.sprayedBy = this.sprayedByPlayer;
		info.msg.spray.timestamp = this.sprayTimestamp.ToBinary();
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000991F4 File Offset: 0x000973F4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spray != null)
		{
			this.sprayedByPlayer = info.msg.spray.sprayedBy;
			this.sprayTimestamp = DateTime.FromBinary(info.msg.spray.timestamp);
		}
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00099248 File Offset: 0x00097448
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.sprayTimestamp = DateTime.Now;
		this.sprayedByPlayer = deployedBy.userID;
		if (ConVar.Global.MaxSpraysPerPlayer > 0 && this.sprayedByPlayer != 0UL)
		{
			int num = -1;
			DateTime now = DateTime.Now;
			int num2 = 0;
			for (int i = 0; i < SprayCanSpray.AllSprays.Count; i++)
			{
				if (SprayCanSpray.AllSprays[i].sprayedByPlayer == this.sprayedByPlayer)
				{
					num2++;
					if (num == -1 || SprayCanSpray.AllSprays[i].sprayTimestamp < now)
					{
						num = i;
						now = SprayCanSpray.AllSprays[i].sprayTimestamp;
					}
				}
			}
			if (num2 >= ConVar.Global.MaxSpraysPerPlayer && num != -1)
			{
				SprayCanSpray.AllSprays[num].Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		if (deployedBy == null || !deployedBy.IsBuildingAuthed())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x0009933C File Offset: 0x0009753C
	private void ApplyOutOfAuthConditionPenalty()
	{
		if (!base.IsFullySpawned())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
			return;
		}
		float num = this.MaxHealth() * (1f - ConVar.Global.SprayOutOfAuthMultiplier);
		base.Hurt(num, DamageType.Decay, null, true);
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00099388 File Offset: 0x00097588
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RainCheck), 60f, 180f, 30f);
		if (!SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Add(this);
		}
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x000993D4 File Offset: 0x000975D4
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Remove(this);
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x000993F5 File Offset: 0x000975F5
	private void RainCheck()
	{
		if (Climate.GetRain(base.transform.position) > 0f && this.IsOutside())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x0009941D File Offset: 0x0009761D
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return amount > 0;
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00099423 File Offset: 0x00097623
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		return 1;
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00099438 File Offset: 0x00097638
	[global::BaseEntity.RPC_Server]
	private void Server_RequestWaterClear(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.Menu_WaterClear_ShowIf(player))
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x0600130B RID: 4875 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BypassInsideDecayMultiplier
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00099468 File Offset: 0x00097668
	private bool Menu_WaterClear_ShowIf(global::BasePlayer player)
	{
		BaseLiquidVessel baseLiquidVessel;
		return player.GetHeldEntity() != null && (baseLiquidVessel = player.GetHeldEntity() as BaseLiquidVessel) != null && baseLiquidVessel.AmountHeld() > 0;
	}

	// Token: 0x04000BF0 RID: 3056
	public DateTime sprayTimestamp;

	// Token: 0x04000BF1 RID: 3057
	public ulong sprayedByPlayer;

	// Token: 0x04000BF2 RID: 3058
	public static ListHashSet<SprayCanSpray> AllSprays = new ListHashSet<SprayCanSpray>(8);
}
