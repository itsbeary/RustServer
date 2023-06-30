using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C9 RID: 201
public class SearchLight : global::IOEntity
{
	// Token: 0x060011F1 RID: 4593 RVA: 0x00091774 File Offset: 0x0008F974
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SearchLight.OnRpcMessage", 0))
		{
			if (rpc == 3611615802U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLight ");
				}
				using (TimeWarning.New("RPC_UseLight", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3611615802U, "RPC_UseLight", this, player, 3f))
						{
							return true;
						}
					}
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
							this.RPC_UseLight(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UseLight");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x000918DC File Offset: 0x0008FADC
	public override void ResetState()
	{
		this.aimDir = Vector3.zero;
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x00023862 File Offset: 0x00021A62
	public override int ConsumptionAmount()
	{
		return 10;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x000918E9 File Offset: 0x0008FAE9
	public bool IsMounted()
	{
		return this.mountedPlayer != null;
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x000918F7 File Offset: 0x0008FAF7
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Facepunch.Pool.Get<ProtoBuf.AutoTurret>();
		info.msg.autoturret.aimDir = this.aimDir;
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x00091926 File Offset: 0x0008FB26
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.aimDir = info.msg.autoturret.aimDir;
		}
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x00091952 File Offset: 0x0008FB52
	public void PlayerEnter(global::BasePlayer player)
	{
		if (this.IsMounted() && player != this.mountedPlayer)
		{
			return;
		}
		this.PlayerExit();
		if (player != null)
		{
			this.mountedPlayer = player;
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0009198F File Offset: 0x0008FB8F
	public void PlayerExit()
	{
		if (this.mountedPlayer)
		{
			this.mountedPlayer = null;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000919B4 File Offset: 0x0008FBB4
	public void MountedUpdate()
	{
		if (this.mountedPlayer == null || this.mountedPlayer.IsSleeping() || !this.mountedPlayer.IsAlive() || this.mountedPlayer.IsWounded() || Vector3.Distance(this.mountedPlayer.transform.position, base.transform.position) > 2f)
		{
			this.PlayerExit();
			return;
		}
		Vector3 vector = this.eyePoint.transform.position + this.mountedPlayer.eyes.BodyForward() * 100f;
		this.SetTargetAimpoint(vector);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x00091A64 File Offset: 0x0008FC64
	public void SetTargetAimpoint(Vector3 worldPos)
	{
		this.aimDir = (worldPos - this.eyePoint.transform.position).normalized;
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x00091A95 File Offset: 0x0008FC95
	public override int GetCurrentEnergy()
	{
		if (this.currentEnergy >= this.ConsumptionAmount())
		{
			return base.GetCurrentEnergy();
		}
		return Mathf.Clamp(this.currentEnergy - base.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x00091AC8 File Offset: 0x0008FCC8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_UseLight(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if (flag && this.IsMounted())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.PlayerEnter(player);
			return;
		}
		this.PlayerExit();
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x00091B1B File Offset: 0x0008FD1B
	public override void OnKilled(HitInfo info)
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.OnKilled(info);
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x00091B2E File Offset: 0x0008FD2E
	public void Update()
	{
		if (base.isServer && this.IsMounted())
		{
			this.MountedUpdate();
		}
	}

	// Token: 0x04000B41 RID: 2881
	public GameObject pitchObject;

	// Token: 0x04000B42 RID: 2882
	public GameObject yawObject;

	// Token: 0x04000B43 RID: 2883
	public GameObject eyePoint;

	// Token: 0x04000B44 RID: 2884
	public SoundPlayer turnLoop;

	// Token: 0x04000B45 RID: 2885
	public bool needsBuildingPrivilegeToUse = true;

	// Token: 0x04000B46 RID: 2886
	private Vector3 aimDir = Vector3.zero;

	// Token: 0x04000B47 RID: 2887
	private global::BasePlayer mountedPlayer;

	// Token: 0x02000C06 RID: 3078
	public static class SearchLightFlags
	{
		// Token: 0x04004228 RID: 16936
		public const global::BaseEntity.Flags PlayerUsing = global::BaseEntity.Flags.Reserved5;
	}
}
