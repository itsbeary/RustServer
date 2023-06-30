using System;
using System.Linq;
using System.Threading.Tasks;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DB RID: 219
public class SteamInventory : EntityComponent<BasePlayer>
{
	// Token: 0x06001340 RID: 4928 RVA: 0x0009A94C File Offset: 0x00098B4C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SteamInventory.OnRpcMessage", 0))
		{
			if (rpc == 643458331U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateSteamInventory ");
				}
				using (TimeWarning.New("UpdateSteamInventory", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(643458331U, "UpdateSteamInventory", this.GetBaseEntity(), player))
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
							this.UpdateSteamInventory(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UpdateSteamInventory");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x0009AAB8 File Offset: 0x00098CB8
	public bool HasItem(int itemid)
	{
		if (base.baseEntity.UnlockAllSkins)
		{
			return true;
		}
		if (this.Items == null)
		{
			return false;
		}
		IPlayerItem[] items = this.Items;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].DefinitionId == itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x0009AB04 File Offset: 0x00098D04
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private async Task UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			Debug.LogWarning("UpdateSteamInventory: Data is null");
		}
		else
		{
			IPlayerInventory playerInventory = await PlatformService.Instance.DeserializeInventory(array);
			if (playerInventory == null)
			{
				Debug.LogWarning("UpdateSteamInventory: result is null");
			}
			else if (base.baseEntity == null)
			{
				Debug.LogWarning("UpdateSteamInventory: player is null");
			}
			else if (!playerInventory.BelongsTo(base.baseEntity.userID))
			{
				Debug.LogWarning(string.Format("UpdateSteamPlayer: inventory belongs to someone else (userID={0})", base.baseEntity.userID));
			}
			else if (base.gameObject)
			{
				this.Items = playerInventory.Items.ToArray<IPlayerItem>();
				playerInventory.Dispose();
			}
		}
	}

	// Token: 0x04000C0A RID: 3082
	private IPlayerItem[] Items;
}
