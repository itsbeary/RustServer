using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BA RID: 186
public class RecorderTool : ThrownWeapon, ICassettePlayer
{
	// Token: 0x060010AE RID: 4270 RVA: 0x00089A18 File Offset: 0x00087C18
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RecorderTool.OnRpcMessage", 0))
		{
			if (rpc == 3075830603U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TogglePlaying ");
				}
				using (TimeWarning.New("Server_TogglePlaying", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3075830603U, "Server_TogglePlaying", this, player))
						{
							return true;
						}
					}
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
							this.Server_TogglePlaying(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_TogglePlaying");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x060010AF RID: 4271 RVA: 0x00089B7C File Offset: 0x00087D7C
	// (set) Token: 0x060010B0 RID: 4272 RVA: 0x00089B84 File Offset: 0x00087D84
	public Cassette cachedCassette { get; private set; }

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x060010B1 RID: 4273 RVA: 0x00089B8D File Offset: 0x00087D8D
	public Sprite LoadedCassetteIcon
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return null;
			}
			return this.cachedCassette.HudSprite;
		}
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00089BAA File Offset: 0x00087DAA
	private bool HasCassette()
	{
		return this.cachedCassette != null;
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x060010B3 RID: 4275 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x00089BB8 File Offset: 0x00087DB8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	public void Server_TogglePlaying(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.ReadByte() == 1;
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x00089BDE File Offset: 0x00087DDE
	public void OnCassetteInserted(Cassette c)
	{
		this.cachedCassette = c;
		base.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x00089BFE File Offset: 0x00087DFE
	public void OnCassetteRemoved(Cassette c)
	{
		this.cachedCassette = null;
		base.ClientRPC(null, "Client_OnCassetteRemoved");
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00089C14 File Offset: 0x00087E14
	protected override void SetUpThrownWeapon(BaseEntity ent)
	{
		base.SetUpThrownWeapon(ent);
		if (base.GetOwnerPlayer() != null)
		{
			ent.OwnerID = base.GetOwnerPlayer().userID;
		}
		DeployedRecorder deployedRecorder;
		if (this.cachedCassette != null && (deployedRecorder = ent as DeployedRecorder) != null)
		{
			this.GetItem().contents.itemList[0].SetParent(deployedRecorder.inventory);
		}
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x00089C80 File Offset: 0x00087E80
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04000A9A RID: 2714
	[ClientVar(Saved = true)]
	public static bool debugRecording;

	// Token: 0x04000A9B RID: 2715
	public AudioSource RecorderAudioSource;

	// Token: 0x04000A9C RID: 2716
	public SoundDefinition RecordStartSfx;

	// Token: 0x04000A9D RID: 2717
	public SoundDefinition RewindSfx;

	// Token: 0x04000A9E RID: 2718
	public SoundDefinition RecordFinishedSfx;

	// Token: 0x04000A9F RID: 2719
	public SoundDefinition PlayTapeSfx;

	// Token: 0x04000AA0 RID: 2720
	public SoundDefinition StopTapeSfx;

	// Token: 0x04000AA1 RID: 2721
	public float ThrowScale = 3f;
}
