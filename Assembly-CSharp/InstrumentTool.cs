using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000088 RID: 136
public class InstrumentTool : HeldEntity
{
	// Token: 0x06000CBD RID: 3261 RVA: 0x0006E3E0 File Offset: 0x0006C5E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("InstrumentTool.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_PlayNote(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopNote(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x0006E640 File Offset: 0x0006C840
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		float num4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetOwnerPlayer());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", num, num2, num3, num4);
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0006E6A0 File Offset: 0x0006C8A0
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", num, num2, num3);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0006E6E0 File Offset: 0x0006C8E0
	public override void ServerUse()
	{
		base.ServerUse();
		if (base.IsInvoking(new Action(this.StopAfterTime)))
		{
			return;
		}
		this.lastPlayedTurretData = this.KeyController.Bindings.BaseBindings[UnityEngine.Random.Range(0, this.KeyController.Bindings.BaseBindings.Length)];
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave, 1f);
		base.Invoke(new Action(this.StopAfterTime), 0.2f);
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0006E784 File Offset: 0x0006C984
	private void StopAfterTime()
	{
		base.ClientRPC<int, int, int>(null, "Client_StopNote", (int)this.lastPlayedTurretData.Note, (int)this.lastPlayedTurretData.Type, this.lastPlayedTurretData.NoteOctave);
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x0006E7B3 File Offset: 0x0006C9B3
	public override bool IsUsableByTurret
	{
		get
		{
			return this.UsableByAutoTurrets;
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000CC3 RID: 3267 RVA: 0x0006E7BB File Offset: 0x0006C9BB
	public override Transform MuzzleTransform
	{
		get
		{
			return this.MuzzleT;
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsInstrument()
	{
		return true;
	}

	// Token: 0x04000838 RID: 2104
	public InstrumentKeyController KeyController;

	// Token: 0x04000839 RID: 2105
	public SoundDefinition DeploySound;

	// Token: 0x0400083A RID: 2106
	public Vector2 PitchClamp = new Vector2(-90f, 90f);

	// Token: 0x0400083B RID: 2107
	public bool UseAnimationSlotEvents;

	// Token: 0x0400083C RID: 2108
	public Transform MuzzleT;

	// Token: 0x0400083D RID: 2109
	public bool UsableByAutoTurrets;

	// Token: 0x0400083E RID: 2110
	private NoteBindingCollection.NoteData lastPlayedTurretData;
}
