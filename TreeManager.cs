using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E7 RID: 231
public class TreeManager : global::BaseEntity
{
	// Token: 0x0600146C RID: 5228 RVA: 0x000A13C8 File Offset: 0x0009F5C8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeManager.OnRpcMessage", 0))
		{
			if (rpc == 1907121457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_RequestTrees ");
				}
				using (TimeWarning.New("SERVER_RequestTrees", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1907121457U, "SERVER_RequestTrees", this, player, 0UL))
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
							this.SERVER_RequestTrees(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_RequestTrees");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000A1530 File Offset: 0x0009F730
	public static Vector3 ProtoHalf3ToVec3(ProtoBuf.Half3 half3)
	{
		return new Vector3
		{
			x = Mathf.HalfToFloat((ushort)half3.x),
			y = Mathf.HalfToFloat((ushort)half3.y),
			z = Mathf.HalfToFloat((ushort)half3.z)
		};
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x000A1580 File Offset: 0x0009F780
	public static ProtoBuf.Half3 Vec3ToProtoHalf3(Vector3 vec3)
	{
		return new ProtoBuf.Half3
		{
			x = (uint)Mathf.FloatToHalf(vec3.x),
			y = (uint)Mathf.FloatToHalf(vec3.y),
			z = (uint)Mathf.FloatToHalf(vec3.z)
		};
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000A15CC File Offset: 0x0009F7CC
	public override void ServerInit()
	{
		base.ServerInit();
		TreeManager.server = this;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x000A15DA File Offset: 0x0009F7DA
	public static void OnTreeDestroyed(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Remove(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		TreeManager.server.ClientRPC<NetworkableId>(null, "CLIENT_TreeDestroyed", billboardEntity.net.ID);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x000A1614 File Offset: 0x0009F814
	public static void OnTreeSpawned(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Add(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>())
		{
			TreeManager.ExtractTreeNetworkData(billboardEntity, tree);
			TreeManager.server.ClientRPC<ProtoBuf.Tree>(null, "CLIENT_TreeSpawned", tree);
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000A1678 File Offset: 0x0009F878
	private static void ExtractTreeNetworkData(global::BaseEntity billboardEntity, ProtoBuf.Tree tree)
	{
		tree.netId = billboardEntity.net.ID;
		tree.prefabId = billboardEntity.prefabID;
		tree.position = TreeManager.Vec3ToProtoHalf3(billboardEntity.transform.position);
		tree.scale = billboardEntity.transform.lossyScale.y;
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000A16D0 File Offset: 0x0009F8D0
	public static void SendSnapshot(global::BasePlayer player)
	{
		BufferList<global::BaseEntity> values = TreeManager.entities.Values;
		TreeList treeList = null;
		for (int i = 0; i < values.Count; i++)
		{
			global::BaseEntity baseEntity = values[i];
			ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>();
			TreeManager.ExtractTreeNetworkData(baseEntity, tree);
			if (treeList == null)
			{
				treeList = Facepunch.Pool.Get<TreeList>();
				treeList.trees = Facepunch.Pool.GetList<ProtoBuf.Tree>();
			}
			treeList.trees.Add(tree);
			if (treeList.trees.Count >= 100)
			{
				TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
				treeList.Dispose();
				treeList = null;
			}
		}
		if (treeList != null)
		{
			TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
			treeList.Dispose();
		}
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000A1774 File Offset: 0x0009F974
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(0UL)]
	private void SERVER_RequestTrees(global::BaseEntity.RPCMessage msg)
	{
		TreeManager.SendSnapshot(msg.player);
	}

	// Token: 0x04000CDC RID: 3292
	public static ListHashSet<global::BaseEntity> entities = new ListHashSet<global::BaseEntity>(8);

	// Token: 0x04000CDD RID: 3293
	public static TreeManager server;

	// Token: 0x04000CDE RID: 3294
	private const int maxTreesPerPacket = 100;
}
