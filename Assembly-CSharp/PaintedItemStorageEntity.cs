using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AA RID: 170
public class PaintedItemStorageEntity : global::BaseEntity, IServerFileReceiver, IUGCBrowserEntity
{
	// Token: 0x06000F8A RID: 3978 RVA: 0x00082220 File Offset: 0x00080420
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PaintedItemStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 2439017595U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateImage ");
				}
				using (TimeWarning.New("Server_UpdateImage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2439017595U, "Server_UpdateImage", this, player, 3UL))
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
							this.Server_UpdateImage(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogException(ex);
						player.Kick("RPC Error in Server_UpdateImage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x00082388 File Offset: 0x00080588
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintedItem != null)
		{
			this._currentImageCrc = info.msg.paintedItem.imageCrc;
			if (base.isServer)
			{
				this.lastEditedBy = info.msg.paintedItem.editedBy;
			}
		}
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x000823E0 File Offset: 0x000805E0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.paintedItem = Facepunch.Pool.Get<PaintedItem>();
		info.msg.paintedItem.imageCrc = this._currentImageCrc;
		info.msg.paintedItem.editedBy = this.lastEditedBy;
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x00082430 File Offset: 0x00080630
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_UpdateImage(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player.userID != base.OwnerID)
		{
			return;
		}
		foreach (global::Item item in msg.player.inventory.containerWear.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == this.net.ID)
			{
				return;
			}
		}
		global::Item item2 = msg.player.inventory.FindBySubEntityID(this.net.ID);
		if (item2 == null || item2.isBroken)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = 0U;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 512, 512))
			{
				return;
			}
			uint currentImageCrc = this._currentImageCrc;
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, 0U);
			if (this._currentImageCrc != currentImageCrc)
			{
				item2.LoseCondition(0.25f);
			}
			this.lastEditedBy = msg.player.userID;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x000825D0 File Offset: 0x000807D0
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000F8F RID: 3983 RVA: 0x000825FC File Offset: 0x000807FC
	public uint[] GetContentCRCs
	{
		get
		{
			if (this._currentImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[] { this._currentImageCrc };
		}
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0008261C File Offset: 0x0008081C
	public void ClearContent()
	{
		this._currentImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00007A44 File Offset: 0x00005C44
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0008262C File Offset: 0x0008082C
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.lastEditedBy <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong> { this.lastEditedBy };
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000F93 RID: 3987 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0008264F File Offset: 0x0008084F
	[Conditional("PAINTED_ITEM_DEBUG")]
	private void DebugOnlyLog(string str)
	{
		UnityEngine.Debug.Log(str, this);
	}

	// Token: 0x04000A31 RID: 2609
	private uint _currentImageCrc;

	// Token: 0x04000A32 RID: 2610
	private ulong lastEditedBy;
}
