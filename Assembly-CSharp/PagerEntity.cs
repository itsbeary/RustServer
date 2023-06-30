using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A9 RID: 169
public class PagerEntity : global::BaseEntity, IRFObject
{
	// Token: 0x06000F78 RID: 3960 RVA: 0x00081ED8 File Offset: 0x000800D8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PagerEntity.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFrequency ");
				}
				using (TimeWarning.New("ServerSetFrequency", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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
							this.ServerSetFrequency(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerSetFrequency");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x00082040 File Offset: 0x00080240
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x00082048 File Offset: 0x00080248
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, false, true);
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x00082053 File Offset: 0x00080253
	public override void ServerInit()
	{
		base.ServerInit();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x00082067 File Offset: 0x00080267
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0008207B File Offset: 0x0008027B
	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x00082084 File Offset: 0x00080284
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		bool flag = base.IsOn();
		if (on != flag)
		{
			base.SetFlag(global::BaseEntity.Flags.On, on, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000820B6 File Offset: 0x000802B6
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(PagerEntity.Flag_Silent, wantsSilent, false, true);
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x00062B09 File Offset: 0x00060D09
	public void SetOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x000820C6 File Offset: 0x000802C6
	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.frequency, newFreq, this, true, true);
		this.frequency = newFreq;
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x000820E0 File Offset: 0x000802E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int num = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, num, this, true, true);
		this.frequency = num;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x00082151 File Offset: 0x00080351
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x00082180 File Offset: 0x00080380
	internal override void OnParentRemoved()
	{
		base.SetParent(null, false, true);
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0008218B File Offset: 0x0008038B
	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x000821A4 File Offset: 0x000803A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
		if (base.isServer && info.fromDisk)
		{
			this.ChangeFrequency(this.frequency);
		}
	}

	// Token: 0x04000A2B RID: 2603
	public static global::BaseEntity.Flags Flag_Silent = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000A2C RID: 2604
	private int frequency = 55;

	// Token: 0x04000A2D RID: 2605
	public float beepRepeat = 2f;

	// Token: 0x04000A2E RID: 2606
	public GameObjectRef pagerEffect;

	// Token: 0x04000A2F RID: 2607
	public GameObjectRef silentEffect;

	// Token: 0x04000A30 RID: 2608
	private float nextChangeTime;
}
