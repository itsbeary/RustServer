using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D3 RID: 211
public class SpinnerWheel : Signage
{
	// Token: 0x060012DA RID: 4826 RVA: 0x000973C0 File Offset: 0x000955C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpinnerWheel.OnRpcMessage", 0))
		{
			if (rpc == 3019675107U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AnyoneSpin ");
				}
				using (TimeWarning.New("RPC_AnyoneSpin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3019675107U, "RPC_AnyoneSpin", this, player, 3f))
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
							this.RPC_AnyoneSpin(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_AnyoneSpin");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455840454U, "RPC_Spin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Spin(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000976C0 File Offset: 0x000958C0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.spinnerWheel = Facepunch.Pool.Get<ProtoBuf.SpinnerWheel>();
		info.msg.spinnerWheel.spin = this.wheel.rotation.eulerAngles;
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00097708 File Offset: 0x00095908
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion quaternion = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isServer)
			{
				this.wheel.transform.rotation = quaternion;
			}
		}
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00097758 File Offset: 0x00095958
	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x00097760 File Offset: 0x00095960
	public virtual void Update_Server()
	{
		if (this.velocity > 0f)
		{
			float num = Mathf.Clamp(this.GetMaxSpinSpeed() * this.velocity, 0f, this.GetMaxSpinSpeed());
			this.velocity -= UnityEngine.Time.deltaTime * Mathf.Clamp(this.velocity / 2f, 0.1f, 1f);
			if (this.velocity < 0f)
			{
				this.velocity = 0f;
			}
			this.wheel.Rotate(Vector3.up, num * UnityEngine.Time.deltaTime, Space.Self);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Update_Client()
	{
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x00097800 File Offset: 0x00095A00
	public void Update()
	{
		if (base.isClient)
		{
			this.Update_Client();
		}
		if (base.isServer)
		{
			this.Update_Server();
		}
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00097820 File Offset: 0x00095A20
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.AllowPlayerSpins())
		{
			return;
		}
		if (this.AnyoneSpin() || rpc.player.CanBuild())
		{
			if (this.velocity > 15f)
			{
				return;
			}
			this.velocity += UnityEngine.Random.Range(4f, 7f);
		}
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00097883 File Offset: 0x00095A83
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_AnyoneSpin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, rpc.read.Bit(), false, true);
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x0003018A File Offset: 0x0002E38A
	public bool AnyoneSpin()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x04000BC7 RID: 3015
	public Transform wheel;

	// Token: 0x04000BC8 RID: 3016
	public float velocity;

	// Token: 0x04000BC9 RID: 3017
	public Quaternion targetRotation = Quaternion.identity;

	// Token: 0x04000BCA RID: 3018
	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	// Token: 0x04000BCB RID: 3019
	public SoundDefinition spinStartSoundDef;

	// Token: 0x04000BCC RID: 3020
	public SoundDefinition spinAccentSoundDef;

	// Token: 0x04000BCD RID: 3021
	public SoundDefinition spinStopSoundDef;

	// Token: 0x04000BCE RID: 3022
	public float minTimeBetweenSpinAccentSounds = 0.3f;

	// Token: 0x04000BCF RID: 3023
	public float spinAccentAngleDelta = 180f;

	// Token: 0x04000BD0 RID: 3024
	private Sound spinSound;

	// Token: 0x04000BD1 RID: 3025
	private SoundModulation.Modulator spinSoundGain;
}
