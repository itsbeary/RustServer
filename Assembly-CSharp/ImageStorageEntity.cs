using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000084 RID: 132
public class ImageStorageEntity : BaseEntity
{
	// Token: 0x06000C69 RID: 3177 RVA: 0x0006B64C File Offset: 0x0006984C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ImageStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 652912521U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ImageRequested ");
				}
				using (TimeWarning.New("ImageRequested", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(652912521U, "ImageRequested", this, player, 3UL))
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
							this.ImageRequested(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ImageRequested");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.jpg;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000C6B RID: 3179 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual uint CrcToLoad
	{
		get
		{
			return 0U;
		}
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0006B7B4 File Offset: 0x000699B4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void ImageRequested(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] array = FileStorage.server.Get(this.CrcToLoad, this.StorageType, this.net.ID, 0U);
		if (array == null)
		{
			Debug.LogWarning("Image entity has no image!");
			return;
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			method = SendMethod.Reliable,
			channel = 2
		};
		base.ClientRPCEx<uint, byte[]>(sendInfo, null, "ReceiveImage", (uint)array.Length, array);
	}

	// Token: 0x04000800 RID: 2048
	private List<ImageStorageEntity.ImageRequest> _requests;

	// Token: 0x02000BE0 RID: 3040
	private struct ImageRequest
	{
		// Token: 0x040041A5 RID: 16805
		public IImageReceiver Receiver;

		// Token: 0x040041A6 RID: 16806
		public float Time;
	}
}
