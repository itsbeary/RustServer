using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// Token: 0x0200003B RID: 59
public class BaseEntity : global::BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
	// Token: 0x0600029C RID: 668 RVA: 0x0002A254 File Offset: 0x00028454
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseEntity.OnRpcMessage", 0))
		{
			if (rpc == 1552640099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BroadcastSignalFromClient ");
				}
				using (TimeWarning.New("BroadcastSignalFromClient", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1552640099U, "BroadcastSignalFromClient", this, player))
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
							this.BroadcastSignalFromClient(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BroadcastSignalFromClient");
					}
				}
				return true;
			}
			if (rpc == 3645147041U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_RequestFile ");
				}
				using (TimeWarning.New("SV_RequestFile", 0))
				{
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
							this.SV_RequestFile(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SV_RequestFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600029D RID: 669 RVA: 0x0002A500 File Offset: 0x00028700
	public virtual float RealisticMass
	{
		get
		{
			return 100f;
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0002A507 File Offset: 0x00028707
	public virtual void OnCollision(Collision collision, global::BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0002A50E File Offset: 0x0002870E
	protected void ReceiveCollisionMessages(bool b)
	{
		if (b)
		{
			base.gameObject.transform.GetOrAddComponent<EntityCollisionMessage>();
			return;
		}
		base.gameObject.transform.RemoveComponent<EntityCollisionMessage>();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0002A538 File Offset: 0x00028738
	public virtual void DebugServer(int rep, float time)
	{
		Vector3 vector = base.transform.position + Vector3.up * 1f;
		string text = "{0}: {1}\n{2}";
		Networkable net = this.net;
		this.DebugText(vector, string.Format(text, (net != null) ? net.ID.Value : 0UL, base.name, this.DebugText()), Color.white, time);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0002A5A3 File Offset: 0x000287A3
	public virtual string DebugText()
	{
		return "";
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0002A5AC File Offset: 0x000287AC
	public void OnDebugStart()
	{
		EntityDebug entityDebug = base.gameObject.GetComponent<EntityDebug>();
		if (entityDebug == null)
		{
			entityDebug = base.gameObject.AddComponent<EntityDebug>();
		}
		entityDebug.enabled = true;
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0002A5E1 File Offset: 0x000287E1
	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		if (base.isServer)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[] { time, color, pos, str });
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0002A61B File Offset: 0x0002881B
	public bool HasFlag(global::BaseEntity.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0002A628 File Offset: 0x00028828
	public bool HasAny(global::BaseEntity.Flags f)
	{
		return (this.flags & f) > (global::BaseEntity.Flags)0;
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0002A638 File Offset: 0x00028838
	public bool ParentHasFlag(global::BaseEntity.Flags f)
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return !(parentEntity == null) && parentEntity.HasFlag(f);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0002A660 File Offset: 0x00028860
	public void SetFlag(global::BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		global::BaseEntity.Flags flags = this.flags;
		if (b)
		{
			if (this.HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		else
		{
			if (!this.HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		this.OnFlagsChanged(flags, this.flags);
		if (networkupdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		else
		{
			base.InvalidateNetworkCache();
		}
		if (recursive && this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetFlag(f, b, true, true);
			}
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsOn()
	{
		return this.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0002A709 File Offset: 0x00028909
	public bool IsOpen()
	{
		return this.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsOnFire()
	{
		return this.HasFlag(global::BaseEntity.Flags.OnFire);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0002A712 File Offset: 0x00028912
	public bool IsLocked()
	{
		return this.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0002A71C File Offset: 0x0002891C
	public override bool IsDebugging()
	{
		return this.HasFlag(global::BaseEntity.Flags.Debugging);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0002A726 File Offset: 0x00028926
	public bool IsDisabled()
	{
		return this.HasFlag(global::BaseEntity.Flags.Disabled) || this.ParentHasFlag(global::BaseEntity.Flags.Disabled);
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0002A73C File Offset: 0x0002893C
	public bool IsBroken()
	{
		return this.HasFlag(global::BaseEntity.Flags.Broken);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0002A749 File Offset: 0x00028949
	public bool IsBusy()
	{
		return this.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0002A756 File Offset: 0x00028956
	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0002A76B File Offset: 0x0002896B
	public virtual void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (this.IsDebugging() && (old & global::BaseEntity.Flags.Debugging) != (next & global::BaseEntity.Flags.Debugging))
		{
			this.OnDebugStart();
		}
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0002A788 File Offset: 0x00028988
	protected void SendNetworkUpdate_Flags()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (!this.isSpawned)
		{
			return;
		}
		using (TimeWarning.New("SendNetworkUpdate_Flags", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Flags");
			List<Connection> subscribers = base.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityFlags);
				netWrite.EntityID(this.net.ID);
				netWrite.Int32((int)this.flags);
				SendInfo sendInfo = new SendInfo(subscribers);
				netWrite.Send(sendInfo);
			}
			base.gameObject.SendOnSendNetworkUpdate(this);
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0002A854 File Offset: 0x00028A54
	public bool IsOccupied(Socket_Base socket)
	{
		EntityLink entityLink = this.FindLink(socket);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0002A874 File Offset: 0x00028A74
	public bool IsOccupied(string socketName)
	{
		EntityLink entityLink = this.FindLink(socketName);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0002A894 File Offset: 0x00028A94
	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0002A8D8 File Offset: 0x00028AD8
	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0002A920 File Offset: 0x00028B20
	public EntityLink FindLink(string[] socketNames)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			for (int j = 0; j < socketNames.Length; j++)
			{
				if (entityLinks[i].socket.socketName == socketNames[j])
				{
					return entityLinks[i];
				}
			}
		}
		return null;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0002A978 File Offset: 0x00028B78
	public T FindLinkedEntity<T>() where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					return entityLink2.owner as T;
				}
			}
		}
		return default(T);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0002A9F4 File Offset: 0x00028BF4
	public void EntityLinkMessage<T>(Action<T> action) where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					action(entityLink2.owner as T);
				}
			}
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0002AA6C File Offset: 0x00028C6C
	public void EntityLinkBroadcast<T, S>(Action<T> action, Func<S, bool> canTraverseSocket) where T : global::BaseEntity where S : Socket_Base
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				if (entityLink.socket is S && canTraverseSocket(entityLink.socket as S))
				{
					for (int j = 0; j < entityLink.connections.Count; j++)
					{
						global::BaseEntity owner = entityLink.connections[j].owner;
						if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
						{
							owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
							global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
							if (owner is T)
							{
								action(owner as T);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0002AB98 File Offset: 0x00028D98
	public void EntityLinkBroadcast<T>(Action<T> action) where T : global::BaseEntity
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action(owner as T);
						}
					}
				}
			}
		}
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0002AC98 File Offset: 0x00028E98
	public void EntityLinkBroadcast()
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
					}
				}
			}
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0002AD5C File Offset: 0x00028F5C
	public bool ReceivedEntityLinkBroadcast()
	{
		return this.broadcastProtocol == global::BaseEntity.globalBroadcastProtocol;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0002AD6B File Offset: 0x00028F6B
	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return this.links;
		}
		if (!this.linkedToNeighbours && linkToNeighbours)
		{
			this.LinkToNeighbours();
		}
		return this.links;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0002AD94 File Offset: 0x00028F94
	private void LinkToEntity(global::BaseEntity other)
	{
		if (this == other)
		{
			return;
		}
		if (this.links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		using (TimeWarning.New("LinkToEntity", 0))
		{
			for (int i = 0; i < this.links.Count; i++)
			{
				EntityLink entityLink = this.links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink2 = other.links[j];
					if (entityLink.CanConnect(entityLink2))
					{
						if (!entityLink.Contains(entityLink2))
						{
							entityLink.Add(entityLink2);
						}
						if (!entityLink2.Contains(entityLink))
						{
							entityLink2.Add(entityLink);
						}
					}
				}
			}
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0002AE64 File Offset: 0x00029064
	private void LinkToNeighbours()
	{
		if (this.links.Count == 0)
		{
			return;
		}
		this.linkedToNeighbours = true;
		using (TimeWarning.New("LinkToNeighbours", 0))
		{
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			OBB obb = this.WorldSpaceBounds();
			global::Vis.Entities<global::BaseEntity>(obb.position, obb.extents.magnitude + 1f, list, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list.Count; i++)
			{
				global::BaseEntity baseEntity = list[i];
				if (baseEntity.isServer == base.isServer)
				{
					this.LinkToEntity(baseEntity);
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0002AF14 File Offset: 0x00029114
	private void InitEntityLinks()
	{
		using (TimeWarning.New("InitEntityLinks", 0))
		{
			if (base.isServer)
			{
				this.links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(this.prefabID));
			}
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0002AF70 File Offset: 0x00029170
	private void FreeEntityLinks()
	{
		using (TimeWarning.New("FreeEntityLinks", 0))
		{
			this.links.FreeLinks();
			this.linkedToNeighbours = false;
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0002AFB8 File Offset: 0x000291B8
	public void RefreshEntityLinks()
	{
		using (TimeWarning.New("RefreshEntityLinks", 0))
		{
			this.links.ClearLinks();
			this.LinkToNeighbours();
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0002B000 File Offset: 0x00029200
	[global::BaseEntity.RPC_Server]
	public void SV_RequestFile(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		FileStorage.Type type = (FileStorage.Type)msg.read.UInt8();
		string text = StringPool.Get(msg.read.UInt32());
		uint num2 = ((msg.read.Unread > 0) ? msg.read.UInt32() : 0U);
		bool flag = msg.read.Unread > 0 && msg.read.Bit();
		byte[] array = FileStorage.server.Get(num, type, this.net.ID, num2);
		if (array == null)
		{
			if (!flag)
			{
				return;
			}
			array = Array.Empty<byte>();
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			channel = 2,
			method = SendMethod.Reliable
		};
		this.ClientRPCEx<uint, uint, byte[], uint, byte>(sendInfo, null, text, num, (uint)array.Length, array, num2, (byte)type);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0002B0D0 File Offset: 0x000292D0
	public void SetParent(global::BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, 0U, worldPositionStays, sendImmediate);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0002B0DC File Offset: 0x000292DC
	public void SetParent(global::BaseEntity entity, string strBone, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, string.IsNullOrEmpty(strBone) ? 0U : StringPool.Get(strBone), worldPositionStays, sendImmediate);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0002B0FC File Offset: 0x000292FC
	public bool HasChild(global::BaseEntity c)
	{
		if (c == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = c.GetParentEntity();
		return parentEntity != null && this.HasChild(parentEntity);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0002B130 File Offset: 0x00029330
	public void SetParent(global::BaseEntity entity, uint boneID, bool worldPositionStays = false, bool sendImmediate = false)
	{
		if (entity != null)
		{
			if (entity == this)
			{
				Debug.LogError("Trying to parent to self " + this, base.gameObject);
				return;
			}
			if (this.HasChild(entity))
			{
				Debug.LogError("Trying to parent to child " + this, base.gameObject);
				return;
			}
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Hierarchy, 2, "SetParent {0} {1}", entity, boneID);
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			parentEntity.RemoveChild(this);
		}
		if (base.limitNetworking && parentEntity != null && parentEntity != entity)
		{
			global::BasePlayer basePlayer = parentEntity as global::BasePlayer;
			if (basePlayer.IsValid())
			{
				this.DestroyOnClient(basePlayer.net.connection);
			}
		}
		if (entity == null)
		{
			this.OnParentChanging(parentEntity, null);
			this.parentEntity.Set(null);
			base.transform.SetParent(null, worldPositionStays);
			this.parentBone = 0U;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
		else
		{
			Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
			Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
			Debug.Assert(entity.net.ID.IsValid, "Setting parent to entity that hasn't spawned yet! (id = 0)");
			entity.AddChild(this);
			this.OnParentChanging(parentEntity, entity);
			this.parentEntity.Set(entity);
			if (boneID != 0U && boneID != StringPool.closest)
			{
				base.transform.SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
			}
			else
			{
				base.transform.SetParent(entity.transform, worldPositionStays);
			}
			this.parentBone = boneID;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0002B2FC File Offset: 0x000294FC
	private void DestroyOnClient(Connection connection)
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.DestroyOnClient(connection);
			}
		}
		if (Network.Net.sv.IsConnected())
		{
			NetWrite netWrite = Network.Net.sv.StartWrite();
			netWrite.PacketID(Message.Type.EntityDestroy);
			netWrite.EntityID(this.net.ID);
			netWrite.UInt8(0);
			netWrite.Send(new SendInfo(connection));
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "EntityDestroy");
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0002B3A4 File Offset: 0x000295A4
	private void SendChildrenNetworkUpdate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0002B404 File Offset: 0x00029604
	private void SendChildrenNetworkUpdateImmediate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0002B464 File Offset: 0x00029664
	public virtual void SwitchParent(global::BaseEntity ent)
	{
		this.Log("SwitchParent Missed " + ent);
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0002B478 File Offset: 0x00029678
	public virtual void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (oldParent != null && oldParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity += oldParent.GetWorldVelocity();
			}
			if (newParent != null && newParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity -= newParent.GetWorldVelocity();
			}
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0002B4F0 File Offset: 0x000296F0
	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		return this.GetBuildingPrivilege(this.WorldSpaceBounds());
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0002B500 File Offset: 0x00029700
	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		global::BuildingBlock buildingBlock = null;
		BuildingPrivlidge buildingPrivlidge = null;
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(obb.position, 16f + obb.extents.magnitude, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock2 = list[i];
			if (buildingBlock2.isServer == base.isServer && buildingBlock2.IsOlderThan(buildingBlock) && obb.Distance(buildingBlock2.WorldSpaceBounds()) <= 16f)
			{
				BuildingManager.Building building = buildingBlock2.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (!(dominatingBuildingPrivilege == null))
					{
						buildingBlock = buildingBlock2;
						buildingPrivlidge = dominatingBuildingPrivilege;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return buildingPrivlidge;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0002B5B4 File Offset: 0x000297B4
	public void SV_RPCMessage(uint nameID, Message message)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		global::BasePlayer basePlayer = message.Player();
		if (!basePlayer.IsValid())
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: From invalid player " + basePlayer);
			}
			return;
		}
		if (basePlayer.isStalled)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: player is stalled " + basePlayer);
			}
			return;
		}
		if (this.OnRpcMessage(basePlayer, nameID, message))
		{
			return;
		}
		for (int i = 0; i < this.Components.Length; i++)
		{
			if (this.Components[i].OnRpcMessage(basePlayer, nameID, message))
			{
				return;
			}
		}
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0002B64C File Offset: 0x0002984C
	public void ClientRPCPlayer<T1, T2, T3, T4, T5>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0002B6A0 File Offset: 0x000298A0
	public void ClientRPCPlayer<T1, T2, T3, T4>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0002B6F4 File Offset: 0x000298F4
	public void ClientRPCPlayer<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0002B744 File Offset: 0x00029944
	public void ClientRPCPlayer<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0002B791 File Offset: 0x00029991
	public void ClientRPCPlayer<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0002B7D1 File Offset: 0x000299D1
	public void ClientRPCPlayer(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0002B810 File Offset: 0x00029A10
	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0002B868 File Offset: 0x00029A68
	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0002B8C0 File Offset: 0x00029AC0
	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0002B914 File Offset: 0x00029B14
	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0002B968 File Offset: 0x00029B68
	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0002B9B8 File Offset: 0x00029BB8
	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(this.net.group.subscribers), sourceConnection, funcName);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0002BA08 File Offset: 0x00029C08
	public void ClientRPCEx<T1, T2, T3, T4, T5>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(netWrite, arg1);
		this.ClientRPCWrite<T2>(netWrite, arg2);
		this.ClientRPCWrite<T3>(netWrite, arg3);
		this.ClientRPCWrite<T4>(netWrite, arg4);
		this.ClientRPCWrite<T5>(netWrite, arg5);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0002BA6C File Offset: 0x00029C6C
	public void ClientRPCEx<T1, T2, T3, T4>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(netWrite, arg1);
		this.ClientRPCWrite<T2>(netWrite, arg2);
		this.ClientRPCWrite<T3>(netWrite, arg3);
		this.ClientRPCWrite<T4>(netWrite, arg4);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0002BAC4 File Offset: 0x00029CC4
	public void ClientRPCEx<T1, T2, T3>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(netWrite, arg1);
		this.ClientRPCWrite<T2>(netWrite, arg2);
		this.ClientRPCWrite<T3>(netWrite, arg3);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0002BB14 File Offset: 0x00029D14
	public void ClientRPCEx<T1, T2>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(netWrite, arg1);
		this.ClientRPCWrite<T2>(netWrite, arg2);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0002BB5C File Offset: 0x00029D5C
	public void ClientRPCEx<T1>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(netWrite, arg1);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0002BB9C File Offset: 0x00029D9C
	public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite netWrite = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCSend(netWrite, sendInfo);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0002BBD0 File Offset: 0x00029DD0
	public void ClientRPCPlayerAndSpectators(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer basePlayer;
					if ((basePlayer = enumerator.Current as global::BasePlayer) != null)
					{
						this.ClientRPCPlayer(sourceConnection, basePlayer, funcName);
					}
				}
			}
		}
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0002BC7C File Offset: 0x00029E7C
	public void ClientRPCPlayerAndSpectators<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer basePlayer;
					if ((basePlayer = enumerator.Current as global::BasePlayer) != null)
					{
						this.ClientRPCPlayer<T1>(sourceConnection, basePlayer, funcName, arg1);
					}
				}
			}
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0002BD2C File Offset: 0x00029F2C
	public void ClientRPCPlayerAndSpectators<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2>(sourceConnection, player, funcName, arg1, arg2);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer basePlayer;
					if ((basePlayer = enumerator.Current as global::BasePlayer) != null)
					{
						this.ClientRPCPlayer<T1, T2>(sourceConnection, basePlayer, funcName, arg1, arg2);
					}
				}
			}
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0002BDD0 File Offset: 0x00029FD0
	public void ClientRPCPlayerAndSpectators<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, player, funcName, arg1, arg2, arg3);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer basePlayer;
					if ((basePlayer = enumerator.Current as global::BasePlayer) != null)
					{
						this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, basePlayer, funcName, arg1, arg2, arg3);
					}
				}
			}
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0002BE78 File Offset: 0x0002A078
	private NetWrite ClientRPCStart(Connection sourceConnection, string funcName)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.RPCMessage);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt32(StringPool.Get(funcName));
		netWrite.UInt64((sourceConnection == null) ? 0UL : sourceConnection.userid);
		return netWrite;
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x0002BEC7 File Offset: 0x0002A0C7
	private void ClientRPCWrite<T>(NetWrite write, T arg)
	{
		write.WriteObject(arg);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0002BED0 File Offset: 0x0002A0D0
	private void ClientRPCSend(NetWrite write, SendInfo sendInfo)
	{
		write.Send(sendInfo);
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060002EA RID: 746 RVA: 0x0002BEDC File Offset: 0x0002A0DC
	public float radiationLevel
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerRadiation triggerRadiation = this.triggers[i] as TriggerRadiation;
				if (!(triggerRadiation == null))
				{
					Vector3 vector = this.GetNetworkPosition();
					global::BaseEntity parentEntity = base.GetParentEntity();
					if (parentEntity != null)
					{
						vector = parentEntity.transform.TransformPoint(vector);
					}
					num = Mathf.Max(num, triggerRadiation.GetRadiation(vector, this.RadiationProtection()));
				}
			}
			return num;
		}
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float RadiationProtection()
	{
		return 0f;
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float RadiationExposureFraction()
	{
		return 1f;
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060002ED RID: 749 RVA: 0x0002BF6C File Offset: 0x0002A16C
	public float currentTemperature
	{
		get
		{
			float num = Climate.GetTemperature(base.transform.position);
			if (this.triggers == null)
			{
				return num;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerTemperature triggerTemperature = this.triggers[i] as TriggerTemperature;
				if (!(triggerTemperature == null))
				{
					num = triggerTemperature.WorkoutTemperature(this.GetNetworkPosition(), num);
				}
			}
			return num;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060002EE RID: 750 RVA: 0x0002BFD4 File Offset: 0x0002A1D4
	public float currentEnvironmentalWetness
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			Vector3 networkPosition = this.GetNetworkPosition();
			using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TriggerWetness triggerWetness;
					if ((triggerWetness = enumerator.Current as TriggerWetness) != null)
					{
						num += triggerWetness.WorkoutWetness(networkPosition);
					}
				}
			}
			return Mathf.Clamp01(num);
		}
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0002C054 File Offset: 0x0002A254
	public virtual void SetCreatorEntity(global::BaseEntity newCreatorEntity)
	{
		this.creatorEntity = newCreatorEntity;
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0002C05D File Offset: 0x0002A25D
	public virtual Vector3 GetLocalVelocityServer()
	{
		return Vector3.zero;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0002C064 File Offset: 0x0002A264
	public virtual Quaternion GetAngularVelocityServer()
	{
		return Quaternion.identity;
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0002C06B File Offset: 0x0002A26B
	public void EnableGlobalBroadcast(bool wants)
	{
		if (this.globalBroadcast == wants)
		{
			return;
		}
		this.globalBroadcast = wants;
		this.UpdateNetworkGroup();
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0002C084 File Offset: 0x0002A284
	public void EnableSaving(bool wants)
	{
		if (this.enableSaving == wants)
		{
			return;
		}
		this.enableSaving = wants;
		if (this.enableSaving)
		{
			if (!global::BaseEntity.saveList.Contains(this))
			{
				global::BaseEntity.saveList.Add(this);
				return;
			}
		}
		else
		{
			global::BaseEntity.saveList.Remove(this);
		}
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0002C0D0 File Offset: 0x0002A2D0
	public override void ServerInit()
	{
		this._spawnable = base.GetComponent<global::Spawnable>();
		base.ServerInit();
		if (this.enableSaving && !global::BaseEntity.saveList.Contains(this))
		{
			global::BaseEntity.saveList.Add(this);
		}
		if (this.flags != (global::BaseEntity.Flags)0)
		{
			this.OnFlagsChanged((global::BaseEntity.Flags)0, this.flags);
		}
		if (this.syncPosition && this.PositionTickRate >= 0f)
		{
			if (this.PositionTickFixedTime)
			{
				base.InvokeRepeatingFixedTime(new Action(this.NetworkPositionTick));
			}
			else
			{
				base.InvokeRandomized(new Action(this.NetworkPositionTick), this.PositionTickRate, this.PositionTickRate - this.PositionTickRate * 0.05f, this.PositionTickRate * 0.05f);
			}
		}
		global::BaseEntity.Query.Server.Add(this);
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060002F6 RID: 758 RVA: 0x0002C198 File Offset: 0x0002A398
	protected virtual float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060002F7 RID: 759 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool PositionTickFixedTime
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0002C1A0 File Offset: 0x0002A3A0
	protected void NetworkPositionTick()
	{
		if (!base.transform.hasChanged)
		{
			if (this.ticksSinceStopped >= 6)
			{
				return;
			}
			this.ticksSinceStopped++;
		}
		else
		{
			this.ticksSinceStopped = 0;
		}
		this.TransformChanged();
		base.transform.hasChanged = false;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0002C1F0 File Offset: 0x0002A3F0
	private void TransformChanged()
	{
		if (global::BaseEntity.Query.Server != null)
		{
			global::BaseEntity.Query.Server.Move(this);
		}
		if (this.net == null)
		{
			return;
		}
		base.InvalidateNetworkCache();
		if (!this.globalBroadcast && !ValidBounds.Test(base.transform.position))
		{
			this.OnInvalidPosition();
			return;
		}
		if (this.syncPosition)
		{
			if (!this.isCallingUpdateNetworkGroup)
			{
				base.Invoke(new Action(this.UpdateNetworkGroup), 5f);
				this.isCallingUpdateNetworkGroup = true;
			}
			base.SendNetworkUpdate_Position();
			this.OnPositionalNetworkUpdate();
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPositionalNetworkUpdate()
	{
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0002C27C File Offset: 0x0002A47C
	public void DoMovingWithoutARigidBodyCheck()
	{
		if (this.doneMovingWithoutARigidBodyCheck > 10)
		{
			return;
		}
		this.doneMovingWithoutARigidBodyCheck++;
		if (this.doneMovingWithoutARigidBodyCheck < 10)
		{
			return;
		}
		if (base.GetComponent<Collider>() == null)
		{
			return;
		}
		if (base.GetComponent<Rigidbody>() == null)
		{
			Debug.LogWarning("Entity moving without a rigid body! (" + base.gameObject + ")", this);
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0002C2E5 File Offset: 0x0002A4E5
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer)
		{
			base.gameObject.BroadcastOnParentSpawning();
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0002C300 File Offset: 0x0002A500
	public void OnParentSpawning()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
		{
			GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
		}
		global::BaseEntity baseEntity = ((base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null);
		this.Spawn();
		if (baseEntity != null)
		{
			this.SetParent(baseEntity, true, false);
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0002C3A0 File Offset: 0x0002A5A0
	public void SpawnAsMapEntity()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (((base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null) == null)
		{
			if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
			{
				GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
			}
			base.transform.parent = null;
			SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Server.EntityScene);
			base.gameObject.SetActive(true);
			this.Spawn();
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostMapEntitySpawn()
	{
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0002C44C File Offset: 0x0002A64C
	internal override void DoServerDestroy()
	{
		base.CancelInvoke(new Action(this.NetworkPositionTick));
		global::BaseEntity.saveList.Remove(this);
		this.RemoveFromTriggers();
		if (this.children != null)
		{
			global::BaseEntity[] array = this.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnParentRemoved();
			}
		}
		this.SetParent(null, true, false);
		global::BaseEntity.Query.Server.Remove(this, false);
		base.DoServerDestroy();
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00003384 File Offset: 0x00001584
	internal virtual void OnParentRemoved()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0002C4C4 File Offset: 0x0002A6C4
	public virtual void OnInvalidPosition()
	{
		Debug.Log(string.Concat(new object[]
		{
			"Invalid Position: ",
			this,
			" ",
			base.transform.position,
			" (destroying)"
		}));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0002C518 File Offset: 0x0002A718
	public BaseCorpse DropCorpse(string strCorpsePrefab)
	{
		Assert.IsTrue(base.isServer, "DropCorpse called on client!");
		if (!ConVar.Server.corpses)
		{
			return null;
		}
		if (string.IsNullOrEmpty(strCorpsePrefab))
		{
			return null;
		}
		BaseCorpse baseCorpse = GameManager.server.CreateEntity(strCorpsePrefab, default(Vector3), default(Quaternion), true) as BaseCorpse;
		if (baseCorpse == null)
		{
			Debug.LogWarning(string.Concat(new object[] { "Error creating corpse: ", base.gameObject, " - ", strCorpsePrefab }));
			return null;
		}
		baseCorpse.InitCorpse(this);
		return baseCorpse;
	}

	// Token: 0x06000304 RID: 772 RVA: 0x0002C5AC File Offset: 0x0002A7AC
	public override void UpdateNetworkGroup()
	{
		Assert.IsTrue(base.isServer, "UpdateNetworkGroup called on clientside entity!");
		this.isCallingUpdateNetworkGroup = false;
		if (this.net == null)
		{
			return;
		}
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateNetworkGroup", 0))
		{
			if (this.globalBroadcast)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.GlobalNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else if (this.ShouldInheritNetworkGroup() && this.parentEntity.IsSet())
			{
				global::BaseEntity parentEntity = base.GetParentEntity();
				if (!parentEntity.IsValid())
				{
					if (!Rust.Application.isLoadingSave)
					{
						Debug.LogWarning("UpdateNetworkGroup: Missing parent entity " + this.parentEntity.uid);
						base.Invoke(new Action(this.UpdateNetworkGroup), 2f);
						this.isCallingUpdateNetworkGroup = true;
					}
				}
				else if (parentEntity != null)
				{
					if (this.net.SwitchGroup(parentEntity.net.group))
					{
						base.SendNetworkGroupChange();
					}
				}
				else
				{
					Debug.LogWarning(base.gameObject + ": has parent id - but couldn't find parent! " + this.parentEntity);
				}
			}
			else if (base.limitNetworking)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.LimboNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else
			{
				base.UpdateNetworkGroup();
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0002C730 File Offset: 0x0002A930
	public virtual void Eat(BaseNpc baseNpc, float timeSpent)
	{
		baseNpc.AddCalories(100f);
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0002C740 File Offset: 0x0002A940
	public override bool ShouldNetworkTo(global::BasePlayer player)
	{
		if (player == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (base.limitNetworking)
		{
			if (parentEntity == null)
			{
				return false;
			}
			if (parentEntity != player)
			{
				return false;
			}
		}
		if (parentEntity != null)
		{
			return parentEntity.ShouldNetworkTo(player);
		}
		return base.ShouldNetworkTo(player);
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0002C795 File Offset: 0x0002A995
	public virtual void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = base.ShortPrefabName;
		info.attackerSteamID = 0UL;
		info.inflictorName = "";
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0002C7B6 File Offset: 0x0002A9B6
	public virtual void Push(Vector3 velocity)
	{
		this.SetVelocity(velocity);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0002C7C0 File Offset: 0x0002A9C0
	public virtual void ApplyInheritedVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = Vector3.Lerp(component.velocity, velocity, 10f * UnityEngine.Time.fixedDeltaTime);
			component.angularVelocity *= Mathf.Clamp01(1f - 10f * UnityEngine.Time.fixedDeltaTime);
			component.AddForce(-UnityEngine.Physics.gravity * Mathf.Clamp01(0.9f), ForceMode.Acceleration);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0002C840 File Offset: 0x0002AA40
	public virtual void SetVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = velocity;
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0002C864 File Offset: 0x0002AA64
	public virtual void SetAngularVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.angularVelocity = velocity;
		}
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0002C887 File Offset: 0x0002AA87
	public virtual Vector3 GetDropPosition()
	{
		return base.transform.position;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0002C894 File Offset: 0x0002AA94
	public virtual Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + Vector3.up;
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return true;
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000310 RID: 784 RVA: 0x0002C8A6 File Offset: 0x0002AAA6
	// (set) Token: 0x06000311 RID: 785 RVA: 0x0002C8B3 File Offset: 0x0002AAB3
	public virtual Vector3 ServerPosition
	{
		get
		{
			return base.transform.localPosition;
		}
		set
		{
			if (base.transform.localPosition == value)
			{
				return;
			}
			base.transform.localPosition = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000312 RID: 786 RVA: 0x0002C8E1 File Offset: 0x0002AAE1
	// (set) Token: 0x06000313 RID: 787 RVA: 0x0002C8EE File Offset: 0x0002AAEE
	public virtual Quaternion ServerRotation
	{
		get
		{
			return base.transform.localRotation;
		}
		set
		{
			if (base.transform.localRotation == value)
			{
				return;
			}
			base.transform.localRotation = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0002C91C File Offset: 0x0002AB1C
	public virtual string Admin_Who()
	{
		return string.Format("Owner ID: {0}", this.OwnerID);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0002C934 File Offset: 0x0002AB34
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void BroadcastSignalFromClient(global::BaseEntity.RPCMessage msg)
	{
		uint num = StringPool.Get("BroadcastSignalFromClient");
		if (num == 0U)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!player.rpcHistory.TryIncrement(num, (ulong)((long)ConVar.Server.maxpacketspersecond_rpc_signal)))
		{
			return;
		}
		global::BaseEntity.Signal signal = (global::BaseEntity.Signal)msg.read.Int32();
		string text = msg.read.String(256);
		this.SignalBroadcast(signal, text, msg.connection);
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0002C9A4 File Offset: 0x0002ABA4
	public void SignalBroadcast(global::BaseEntity.Signal signal, string arg, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int, string>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServerEx", (int)signal, arg);
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0002CA00 File Offset: 0x0002AC00
	public void SignalBroadcast(global::BaseEntity.Signal signal, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServer", (int)signal);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0002CA59 File Offset: 0x0002AC59
	protected virtual void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0002CA67 File Offset: 0x0002AC67
	protected virtual void OnSkinPreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && Skinnable.FindForEntity(name) != null)
		{
			Rust.Workshop.WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0002CA8E File Offset: 0x0002AC8E
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.OnSkinPreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0002CAA0 File Offset: 0x0002ACA0
	public bool HasAnySlot()
	{
		for (int i = 0; i < this.entitySlots.Length; i++)
		{
			if (this.entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0002CADC File Offset: 0x0002ACDC
	public global::BaseEntity GetSlot(global::BaseEntity.Slot slot)
	{
		return this.entitySlots[(int)slot].Get(base.isServer);
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0002CAF5 File Offset: 0x0002ACF5
	public string GetSlotAnchorName(global::BaseEntity.Slot slot)
	{
		return slot.ToString().ToLower();
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0002CB09 File Offset: 0x0002AD09
	public void SetSlot(global::BaseEntity.Slot slot, global::BaseEntity ent)
	{
		this.entitySlots[(int)slot].Set(ent);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0002CB24 File Offset: 0x0002AD24
	public EntityRef[] GetSlots()
	{
		return this.entitySlots;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0002CB2C File Offset: 0x0002AD2C
	public void SetSlots(EntityRef[] newSlots)
	{
		this.entitySlots = newSlots;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool HasSlot(global::BaseEntity.Slot slot)
	{
		return false;
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000322 RID: 802 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return global::BaseEntity.TraitFlag.None;
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0002CB35 File Offset: 0x0002AD35
	public bool HasTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) == f;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0002CB42 File Offset: 0x0002AD42
	public bool HasAnyTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) > global::BaseEntity.TraitFlag.None;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0002CB4F File Offset: 0x0002AD4F
	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			this.triggers = Facepunch.Pool.Get<List<TriggerBase>>();
		}
		this.triggers.Add(trigger);
		return true;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0002CB71 File Offset: 0x0002AD71
	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			return;
		}
		this.triggers.Remove(trigger);
		if (this.triggers.Count == 0)
		{
			Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0002CBA4 File Offset: 0x0002ADA4
	public void RemoveFromTriggers()
	{
		if (this.triggers == null)
		{
			return;
		}
		using (TimeWarning.New("RemoveFromTriggers", 0))
		{
			foreach (TriggerBase triggerBase in this.triggers.ToArray())
			{
				if (triggerBase)
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (this.triggers != null && this.triggers.Count == 0)
			{
				Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
			}
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0002CC30 File Offset: 0x0002AE30
	public T FindTrigger<T>() where T : TriggerBase
	{
		if (this.triggers == null)
		{
			return default(T);
		}
		foreach (TriggerBase triggerBase in this.triggers)
		{
			if (!(triggerBase as T == null))
			{
				return triggerBase as T;
			}
		}
		return default(T);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0002CCC0 File Offset: 0x0002AEC0
	public bool FindTrigger<T>(out T result) where T : TriggerBase
	{
		result = this.FindTrigger<T>();
		return result != null;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0002CCDF File Offset: 0x0002AEDF
	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			this.ForceUpdateTriggers(false, true, false);
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0002CCF4 File Offset: 0x0002AEF4
	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		List<TriggerBase> list = Facepunch.Pool.GetList<TriggerBase>();
		List<TriggerBase> list2 = Facepunch.Pool.GetList<TriggerBase>();
		if (this.triggers != null)
		{
			list.AddRange(this.triggers);
		}
		Collider componentInChildren = base.GetComponentInChildren<Collider>();
		if (componentInChildren is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = componentInChildren as CapsuleCollider;
			Vector3 vector = base.transform.position + new Vector3(0f, capsuleCollider.radius, 0f);
			Vector3 vector2 = base.transform.position + new Vector3(0f, capsuleCollider.height - capsuleCollider.radius, 0f);
			GamePhysics.OverlapCapsule<TriggerBase>(vector, vector2, capsuleCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is BoxCollider)
		{
			BoxCollider boxCollider = componentInChildren as BoxCollider;
			GamePhysics.OverlapOBB<TriggerBase>(new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, new Bounds(boxCollider.center, boxCollider.size)), list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is SphereCollider)
		{
			SphereCollider sphereCollider = componentInChildren as SphereCollider;
			GamePhysics.OverlapSphere<TriggerBase>(base.transform.TransformPoint(sphereCollider.center), sphereCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else
		{
			list2.AddRange(list);
		}
		if (exit)
		{
			foreach (TriggerBase triggerBase in list)
			{
				if (!list2.Contains(triggerBase))
				{
					triggerBase.OnTriggerExit(componentInChildren);
				}
			}
		}
		if (enter)
		{
			foreach (TriggerBase triggerBase2 in list2)
			{
				if (!list.Contains(triggerBase2))
				{
					triggerBase2.OnTriggerEnter(componentInChildren);
				}
			}
		}
		Facepunch.Pool.FreeList<TriggerBase>(ref list);
		Facepunch.Pool.FreeList<TriggerBase>(ref list2);
		if (invoke)
		{
			base.Invoke(new Action(this.ForceUpdateTriggersAction), UnityEngine.Time.time - UnityEngine.Time.fixedTime + UnityEngine.Time.fixedDeltaTime * 1.5f);
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0002CF10 File Offset: 0x0002B110
	public TriggerParent FindSuitableParent()
	{
		if (this.triggers == null)
		{
			return null;
		}
		using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TriggerParent triggerParent;
				if ((triggerParent = enumerator.Current as TriggerParent) != null && triggerParent.ShouldParent(this, true))
				{
					return triggerParent;
				}
			}
		}
		return null;
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600032D RID: 813 RVA: 0x0002CF80 File Offset: 0x0002B180
	// (set) Token: 0x0600032E RID: 814 RVA: 0x0002CF88 File Offset: 0x0002B188
	public float Weight { get; protected set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600032F RID: 815 RVA: 0x0002CF94 File Offset: 0x0002B194
	public EntityComponentBase[] Components
	{
		get
		{
			EntityComponentBase[] array;
			if ((array = this._components) == null)
			{
				array = (this._components = base.GetComponentsInChildren<EntityComponentBase>(true));
			}
			return array;
		}
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public virtual global::BasePlayer ToPlayer()
	{
		return null;
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x06000331 RID: 817 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsNpc
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0002CFBE File Offset: 0x0002B1BE
	public override void InitShared()
	{
		base.InitShared();
		this.InitEntityLinks();
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0002CFCC File Offset: 0x0002B1CC
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.FreeEntityLinks();
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0002CFDA File Offset: 0x0002B1DA
	public override void ResetState()
	{
		base.ResetState();
		this.parentBone = 0U;
		this.OwnerID = 0UL;
		this.flags = (global::BaseEntity.Flags)0;
		this.parentEntity = default(EntityRef);
		if (base.isServer)
		{
			this._spawnable = null;
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool InheritedVelocityDirection()
	{
		return true;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0002D014 File Offset: 0x0002B214
	public virtual Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		if (baseEntity.InheritedVelocityDirection())
		{
			return this.GetParentVelocity() * baseEntity.InheritedVelocityScale();
		}
		return Mathf.Max(Vector3.Dot(this.GetParentVelocity() * baseEntity.InheritedVelocityScale(), direction), 0f) * direction;
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0002D083 File Offset: 0x0002B283
	public virtual Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		return this.GetParentVelocity();
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0002D08C File Offset: 0x0002B28C
	public virtual Vector3 GetInheritedDropVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0002D0C0 File Offset: 0x0002B2C0
	public Vector3 GetParentVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition);
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002D120 File Offset: 0x0002B320
	public Vector3 GetWorldVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return this.GetLocalVelocity();
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition) + baseEntity.transform.TransformDirection(this.GetLocalVelocity());
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0002D196 File Offset: 0x0002B396
	public Vector3 GetLocalVelocity()
	{
		if (base.isServer)
		{
			return this.GetLocalVelocityServer();
		}
		return Vector3.zero;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0002D1AC File Offset: 0x0002B3AC
	public Quaternion GetAngularVelocity()
	{
		if (base.isServer)
		{
			return this.GetAngularVelocityServer();
		}
		return Quaternion.identity;
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0002D1C2 File Offset: 0x0002B3C2
	public virtual OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 PivotPoint()
	{
		return base.transform.position;
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0002D1F0 File Offset: 0x0002B3F0
	public Vector3 CenterPoint()
	{
		return this.WorldSpaceBounds().position;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0002D200 File Offset: 0x0002B400
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.WorldSpaceBounds().ClosestPoint(position);
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0002D21C File Offset: 0x0002B41C
	public virtual Vector3 TriggerPoint()
	{
		return this.CenterPoint();
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0002D224 File Offset: 0x0002B424
	public float Distance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).magnitude;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0002D248 File Offset: 0x0002B448
	public float SqrDistance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).sqrMagnitude;
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0002D26A File Offset: 0x0002B46A
	public float Distance(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0002D27D File Offset: 0x0002B47D
	public float SqrDistance(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0002D290 File Offset: 0x0002B490
	public float Distance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).Magnitude2D();
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0002D2A4 File Offset: 0x0002B4A4
	public float SqrDistance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).SqrMagnitude2D();
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0002D26A File Offset: 0x0002B46A
	public float Distance2D(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0002D27D File Offset: 0x0002B47D
	public float SqrDistance2D(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0002D2B8 File Offset: 0x0002B4B8
	public bool IsVisible(Ray ray, int layerMask, float maxDistance)
	{
		if (ray.origin.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction == Vector3.zero)
		{
			return false;
		}
		RaycastHit raycastHit;
		if (!this.WorldSpaceBounds().Trace(ray, out raycastHit, maxDistance))
		{
			return false;
		}
		RaycastHit raycastHit2;
		if (GamePhysics.Trace(ray, 0f, out raycastHit2, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null))
		{
			global::BaseEntity entity = raycastHit2.GetEntity();
			if (entity == this)
			{
				return true;
			}
			if (entity != null && base.GetParentEntity() && base.GetParentEntity().EqualNetID(entity) && raycastHit2.IsOnLayer(Rust.Layer.Vehicle_Detailed))
			{
				return true;
			}
			if (raycastHit2.distance <= raycastHit.distance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0002D378 File Offset: 0x0002B578
	public bool IsVisibleSpecificLayers(Vector3 position, Vector3 target, int layerMask, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = target - position;
		float magnitude = vector.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector2 = vector / magnitude;
		Vector3 vector3 = vector2 * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + vector3, vector2), layerMask, maxDistance);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0002D3D0 File Offset: 0x0002B5D0
	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = target - position;
		float magnitude = vector.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector2 = vector / magnitude;
		Vector3 vector3 = vector2 * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + vector3, vector2), 1218519041, maxDistance);
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0002D42C File Offset: 0x0002B62C
	public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = this.CenterPoint();
		if (this.IsVisible(position, vector, maxDistance))
		{
			return true;
		}
		Vector3 vector2 = this.ClosestPoint(position);
		return this.IsVisible(position, vector2, maxDistance);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0002D464 File Offset: 0x0002B664
	public bool IsVisibleAndCanSee(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = this.CenterPoint();
		if (this.IsVisible(position, vector, maxDistance) && this.IsVisible(vector, position, maxDistance))
		{
			return true;
		}
		Vector3 vector2 = this.ClosestPoint(position);
		return this.IsVisible(position, vector2, maxDistance) && this.IsVisible(vector2, position, maxDistance);
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0002D4B4 File Offset: 0x0002B6B4
	public bool IsOlderThan(global::BaseEntity other)
	{
		if (other == null)
		{
			return true;
		}
		Networkable net = this.net;
		ref NetworkableId ptr = ((net != null) ? net.ID : default(NetworkableId));
		Networkable net2 = other.net;
		NetworkableId networkableId = ((net2 != null) ? net2.ID : default(NetworkableId));
		return ptr.Value < networkableId.Value;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002D510 File Offset: 0x0002B710
	public virtual bool IsOutside()
	{
		OBB obb = this.WorldSpaceBounds();
		return this.IsOutside(obb.position);
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0002D530 File Offset: 0x0002B730
	public bool IsOutside(Vector3 position)
	{
		bool flag = true;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out raycastHit, 100f, 161546513))
		{
			global::BaseEntity baseEntity = raycastHit.collider.ToBaseEntity();
			if (baseEntity == null || !baseEntity.HasEntityInParents(this))
			{
				flag = false;
			}
		}
		return flag;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0002D590 File Offset: 0x0002B790
	public virtual float WaterFactor()
	{
		return WaterLevel.Factor(this.WorldSpaceBounds().ToBounds(), true, true, this);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0002D5B3 File Offset: 0x0002B7B3
	public virtual float AirFactor()
	{
		if (this.WaterFactor() <= 0.85f)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002D5D0 File Offset: 0x0002B7D0
	public bool WaterTestFromVolumes(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = this.triggers[i] as WaterVolume) != null && waterVolume.Test(pos, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002D630 File Offset: 0x0002B830
	public bool IsInWaterVolume(Vector3 pos)
	{
		if (this.triggers == null)
		{
			return false;
		}
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = this.triggers[i] as WaterVolume) != null && waterVolume.Test(pos, out waterInfo))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002D688 File Offset: 0x0002B888
	public bool WaterTestFromVolumes(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = this.triggers[i] as WaterVolume) != null && waterVolume.Test(bounds, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0002D6E8 File Offset: 0x0002B8E8
	public bool WaterTestFromVolumes(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = this.triggers[i] as WaterVolume) != null && waterVolume.Test(start, end, radius, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool BlocksWaterFor(global::BasePlayer player)
	{
		return false;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float Health()
	{
		return 0f;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float MaxHealth()
	{
		return 0f;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float MaxVelocity()
	{
		return 0f;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0002C198 File Offset: 0x0002A398
	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0002A500 File Offset: 0x00028700
	public virtual float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002D74A File Offset: 0x0002B94A
	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return this.impactEffect;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAttacked(HitInfo info)
	{
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public virtual global::Item GetItem()
	{
		return null;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002CFBB File Offset: 0x0002B1BB
	public virtual global::Item GetItem(ItemId itemId)
	{
		return null;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002D752 File Offset: 0x0002B952
	public virtual void GiveItem(global::Item item, global::BaseEntity.GiveItemReason reason = global::BaseEntity.GiveItemReason.Generic)
	{
		item.Remove(0f);
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanBeLooted(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x000037E7 File Offset: 0x000019E7
	public virtual global::BaseEntity GetEntity()
	{
		return this;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002D760 File Offset: 0x0002B960
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				string text = "{1}[{0}]";
				Networkable net = this.net;
				this._name = string.Format(text, (net != null) ? net.ID : default(NetworkableId), base.ShortPrefabName);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002D7C6 File Offset: 0x0002B9C6
	public virtual string Categorize()
	{
		return "entity";
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002D7D0 File Offset: 0x0002B9D0
	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color=#ffa>[",
				this.ToString(),
				"] ",
				str,
				"</color>"
			}), base.gameObject);
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"<color=#aff>[",
			this.ToString(),
			"] ",
			str,
			"</color>"
		}), base.gameObject);
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0002D85C File Offset: 0x0002BA5C
	public void SetModel(Model mdl)
	{
		if (this.model == mdl)
		{
			return;
		}
		this.model = mdl;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002D874 File Offset: 0x0002BA74
	public Model GetModel()
	{
		return this.model;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002D87C File Offset: 0x0002BA7C
	public virtual Transform[] GetBones()
	{
		if (this.model)
		{
			return this.model.GetBones();
		}
		return null;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0002D898 File Offset: 0x0002BA98
	public virtual Transform FindBone(string strName)
	{
		if (this.model)
		{
			return this.model.FindBone(strName);
		}
		return base.transform;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0002D8BA File Offset: 0x0002BABA
	public virtual uint FindBoneID(Transform boneTransform)
	{
		if (this.model)
		{
			return this.model.FindBoneID(boneTransform);
		}
		return StringPool.closest;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0002D8DB File Offset: 0x0002BADB
	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		if (this.model)
		{
			return this.model.FindClosestBone(worldPos);
		}
		return base.transform;
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600036F RID: 879 RVA: 0x0002D8FD File Offset: 0x0002BAFD
	// (set) Token: 0x06000370 RID: 880 RVA: 0x0002D905 File Offset: 0x0002BB05
	public ulong OwnerID { get; set; }

	// Token: 0x06000371 RID: 881 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002D910 File Offset: 0x0002BB10
	public virtual bool SupportsChildDeployables()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return parentEntity != null && parentEntity.ForceDeployableSetParent();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0002D938 File Offset: 0x0002BB38
	public virtual bool ForceDeployableSetParent()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return parentEntity != null && parentEntity.ForceDeployableSetParent();
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0002D960 File Offset: 0x0002BB60
	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		if (base.isClient)
		{
			return;
		}
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer)
			{
				baseEntity.OnEntityMessage(this, msg);
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06000376 RID: 886 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEntityMessage(global::BaseEntity from, string msg)
	{
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0002D9E4 File Offset: 0x0002BBE4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		info.msg.baseEntity = Facepunch.Pool.Get<ProtoBuf.BaseEntity>();
		if (info.forDisk)
		{
			if (this is global::BasePlayer)
			{
				if (baseEntity == null || baseEntity.enableSaving)
				{
					info.msg.baseEntity.pos = base.transform.localPosition;
					info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
				}
				else
				{
					info.msg.baseEntity.pos = base.transform.position;
					info.msg.baseEntity.rot = base.transform.rotation.eulerAngles;
				}
			}
			else
			{
				info.msg.baseEntity.pos = base.transform.localPosition;
				info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
			}
		}
		else
		{
			info.msg.baseEntity.pos = this.GetNetworkPosition();
			info.msg.baseEntity.rot = this.GetNetworkRotation().eulerAngles;
			info.msg.baseEntity.time = this.GetNetworkTime();
		}
		info.msg.baseEntity.flags = (int)this.flags;
		info.msg.baseEntity.skinid = this.skinID;
		if (info.forDisk && this is global::BasePlayer)
		{
			if (baseEntity != null && baseEntity.enableSaving)
			{
				info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
				info.msg.parent.uid = this.parentEntity.uid;
				info.msg.parent.bone = this.parentBone;
			}
		}
		else if (baseEntity != null)
		{
			info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
			info.msg.parent.uid = this.parentEntity.uid;
			info.msg.parent.bone = this.parentBone;
		}
		if (this.HasAnySlot())
		{
			info.msg.entitySlots = Facepunch.Pool.Get<EntitySlots>();
			info.msg.entitySlots.slotLock = this.entitySlots[0].uid;
			info.msg.entitySlots.slotFireMod = this.entitySlots[1].uid;
			info.msg.entitySlots.slotUpperModification = this.entitySlots[2].uid;
			info.msg.entitySlots.centerDecoration = this.entitySlots[5].uid;
			info.msg.entitySlots.lowerCenterDecoration = this.entitySlots[6].uid;
			info.msg.entitySlots.storageMonitor = this.entitySlots[7].uid;
		}
		if (info.forDisk && this._spawnable)
		{
			this._spawnable.Save(info);
		}
		if (this.OwnerID != 0UL && (info.forDisk || this.ShouldNetworkOwnerInfo()))
		{
			info.msg.ownerInfo = Facepunch.Pool.Get<OwnerInfo>();
			info.msg.ownerInfo.steamid = this.OwnerID;
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].SaveComponent(info);
				}
			}
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool ShouldNetworkOwnerInfo()
	{
		return false;
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0002DDAC File Offset: 0x0002BFAC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			ProtoBuf.BaseEntity baseEntity = info.msg.baseEntity;
			global::BaseEntity.Flags flags = this.flags;
			this.flags = (global::BaseEntity.Flags)baseEntity.flags;
			this.OnFlagsChanged(flags, this.flags);
			this.OnSkinChanged(this.skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (baseEntity.pos.IsNaNOrInfinity())
				{
					Debug.LogWarning(this.ToString() + " has broken position - " + baseEntity.pos);
					baseEntity.pos = Vector3.zero;
				}
				base.transform.localPosition = baseEntity.pos;
				base.transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			this.entitySlots[0].uid = info.msg.entitySlots.slotLock;
			this.entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			this.entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			this.entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			this.entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
			this.entitySlots[7].uid = info.msg.entitySlots.storageMonitor;
		}
		if (info.msg.parent != null)
		{
			if (base.isServer)
			{
				global::BaseEntity baseEntity2 = global::BaseNetworkable.serverEntities.Find(info.msg.parent.uid) as global::BaseEntity;
				this.SetParent(baseEntity2, info.msg.parent.bone, false, false);
			}
			this.parentEntity.uid = info.msg.parent.uid;
			this.parentBone = info.msg.parent.bone;
		}
		else
		{
			this.parentEntity.uid = default(NetworkableId);
			this.parentBone = 0U;
		}
		if (info.msg.ownerInfo != null)
		{
			this.OwnerID = info.msg.ownerInfo.steamid;
		}
		if (this._spawnable)
		{
			this._spawnable.Load(info);
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].LoadComponent(info);
				}
			}
		}
	}

	// Token: 0x0400024B RID: 587
	private static Queue<global::BaseEntity> globalBroadcastQueue = new Queue<global::BaseEntity>();

	// Token: 0x0400024C RID: 588
	private static uint globalBroadcastProtocol = 0U;

	// Token: 0x0400024D RID: 589
	private uint broadcastProtocol;

	// Token: 0x0400024E RID: 590
	private List<EntityLink> links = new List<EntityLink>();

	// Token: 0x0400024F RID: 591
	private bool linkedToNeighbours;

	// Token: 0x04000250 RID: 592
	[NonSerialized]
	public global::BaseEntity creatorEntity;

	// Token: 0x04000251 RID: 593
	private int ticksSinceStopped;

	// Token: 0x04000252 RID: 594
	private int doneMovingWithoutARigidBodyCheck = 1;

	// Token: 0x04000253 RID: 595
	private bool isCallingUpdateNetworkGroup;

	// Token: 0x04000254 RID: 596
	private EntityRef[] entitySlots = new EntityRef[8];

	// Token: 0x04000255 RID: 597
	protected List<TriggerBase> triggers;

	// Token: 0x04000256 RID: 598
	protected bool isVisible = true;

	// Token: 0x04000257 RID: 599
	protected bool isAnimatorVisible = true;

	// Token: 0x04000258 RID: 600
	protected bool isShadowVisible = true;

	// Token: 0x04000259 RID: 601
	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x0400025B RID: 603
	[Header("BaseEntity")]
	public Bounds bounds;

	// Token: 0x0400025C RID: 604
	public GameObjectRef impactEffect;

	// Token: 0x0400025D RID: 605
	public bool enableSaving = true;

	// Token: 0x0400025E RID: 606
	public bool syncPosition;

	// Token: 0x0400025F RID: 607
	public Model model;

	// Token: 0x04000260 RID: 608
	[global::InspectorFlags]
	public global::BaseEntity.Flags flags;

	// Token: 0x04000261 RID: 609
	[NonSerialized]
	public uint parentBone;

	// Token: 0x04000262 RID: 610
	[NonSerialized]
	public ulong skinID;

	// Token: 0x04000263 RID: 611
	private EntityComponentBase[] _components;

	// Token: 0x04000264 RID: 612
	[HideInInspector]
	public bool HasBrain;

	// Token: 0x04000265 RID: 613
	[NonSerialized]
	protected string _name;

	// Token: 0x04000267 RID: 615
	private global::Spawnable _spawnable;

	// Token: 0x04000268 RID: 616
	public static HashSet<global::BaseEntity> saveList = new HashSet<global::BaseEntity>();

	// Token: 0x02000B84 RID: 2948
	public class Menu : Attribute
	{
		// Token: 0x06004D32 RID: 19762 RVA: 0x000031B9 File Offset: 0x000013B9
		public Menu()
		{
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x001A0370 File Offset: 0x0019E570
		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			this.TitleToken = menuTitleToken;
			this.TitleEnglish = menuTitleEnglish;
		}

		// Token: 0x04003FB5 RID: 16309
		public string TitleToken;

		// Token: 0x04003FB6 RID: 16310
		public string TitleEnglish;

		// Token: 0x04003FB7 RID: 16311
		public string UseVariable;

		// Token: 0x04003FB8 RID: 16312
		public int Order;

		// Token: 0x04003FB9 RID: 16313
		public string ProxyFunction;

		// Token: 0x04003FBA RID: 16314
		public float Time;

		// Token: 0x04003FBB RID: 16315
		public string OnStart;

		// Token: 0x04003FBC RID: 16316
		public string OnProgress;

		// Token: 0x04003FBD RID: 16317
		public bool LongUseOnly;

		// Token: 0x04003FBE RID: 16318
		public bool PrioritizeIfNotWhitelisted;

		// Token: 0x04003FBF RID: 16319
		public bool PrioritizeIfUnlocked;

		// Token: 0x02000FC4 RID: 4036
		[Serializable]
		public struct Option
		{
			// Token: 0x04005139 RID: 20793
			public Translate.Phrase name;

			// Token: 0x0400513A RID: 20794
			public Translate.Phrase description;

			// Token: 0x0400513B RID: 20795
			public Sprite icon;

			// Token: 0x0400513C RID: 20796
			public int order;

			// Token: 0x0400513D RID: 20797
			public bool usableWhileWounded;
		}

		// Token: 0x02000FC5 RID: 4037
		public class Description : Attribute
		{
			// Token: 0x060055A8 RID: 21928 RVA: 0x001BAAEC File Offset: 0x001B8CEC
			public Description(string t, string e)
			{
				this.token = t;
				this.english = e;
			}

			// Token: 0x0400513E RID: 20798
			public string token;

			// Token: 0x0400513F RID: 20799
			public string english;
		}

		// Token: 0x02000FC6 RID: 4038
		public class Icon : Attribute
		{
			// Token: 0x060055A9 RID: 21929 RVA: 0x001BAB02 File Offset: 0x001B8D02
			public Icon(string i)
			{
				this.icon = i;
			}

			// Token: 0x04005140 RID: 20800
			public string icon;
		}

		// Token: 0x02000FC7 RID: 4039
		public class ShowIf : Attribute
		{
			// Token: 0x060055AA RID: 21930 RVA: 0x001BAB11 File Offset: 0x001B8D11
			public ShowIf(string testFunc)
			{
				this.functionName = testFunc;
			}

			// Token: 0x04005141 RID: 20801
			public string functionName;
		}

		// Token: 0x02000FC8 RID: 4040
		public class Priority : Attribute
		{
			// Token: 0x060055AB RID: 21931 RVA: 0x001BAB20 File Offset: 0x001B8D20
			public Priority(string priorityFunc)
			{
				this.functionName = priorityFunc;
			}

			// Token: 0x04005142 RID: 20802
			public string functionName;
		}

		// Token: 0x02000FC9 RID: 4041
		public class UsableWhileWounded : Attribute
		{
		}
	}

	// Token: 0x02000B85 RID: 2949
	[Serializable]
	public struct MovementModify
	{
		// Token: 0x04003FC0 RID: 16320
		public float drag;
	}

	// Token: 0x02000B86 RID: 2950
	[Flags]
	public enum Flags
	{
		// Token: 0x04003FC2 RID: 16322
		Placeholder = 1,
		// Token: 0x04003FC3 RID: 16323
		On = 2,
		// Token: 0x04003FC4 RID: 16324
		OnFire = 4,
		// Token: 0x04003FC5 RID: 16325
		Open = 8,
		// Token: 0x04003FC6 RID: 16326
		Locked = 16,
		// Token: 0x04003FC7 RID: 16327
		Debugging = 32,
		// Token: 0x04003FC8 RID: 16328
		Disabled = 64,
		// Token: 0x04003FC9 RID: 16329
		Reserved1 = 128,
		// Token: 0x04003FCA RID: 16330
		Reserved2 = 256,
		// Token: 0x04003FCB RID: 16331
		Reserved3 = 512,
		// Token: 0x04003FCC RID: 16332
		Reserved4 = 1024,
		// Token: 0x04003FCD RID: 16333
		Reserved5 = 2048,
		// Token: 0x04003FCE RID: 16334
		Broken = 4096,
		// Token: 0x04003FCF RID: 16335
		Busy = 8192,
		// Token: 0x04003FD0 RID: 16336
		Reserved6 = 16384,
		// Token: 0x04003FD1 RID: 16337
		Reserved7 = 32768,
		// Token: 0x04003FD2 RID: 16338
		Reserved8 = 65536,
		// Token: 0x04003FD3 RID: 16339
		Reserved9 = 131072,
		// Token: 0x04003FD4 RID: 16340
		Reserved10 = 262144,
		// Token: 0x04003FD5 RID: 16341
		Reserved11 = 524288,
		// Token: 0x04003FD6 RID: 16342
		InUse = 1048576
	}

	// Token: 0x02000B87 RID: 2951
	private readonly struct QueuedFileRequest : IEquatable<global::BaseEntity.QueuedFileRequest>
	{
		// Token: 0x06004D34 RID: 19764 RVA: 0x001A0386 File Offset: 0x0019E586
		public QueuedFileRequest(global::BaseEntity entity, FileStorage.Type type, uint part, uint crc, uint responseFunction, bool? respondIfNotFound)
		{
			this.Entity = entity;
			this.Type = type;
			this.Part = part;
			this.Crc = crc;
			this.ResponseFunction = responseFunction;
			this.RespondIfNotFound = respondIfNotFound;
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x001A03B8 File Offset: 0x0019E5B8
		public bool Equals(global::BaseEntity.QueuedFileRequest other)
		{
			if (object.Equals(this.Entity, other.Entity) && this.Type == other.Type && this.Part == other.Part && this.Crc == other.Crc && this.ResponseFunction == other.ResponseFunction)
			{
				bool? respondIfNotFound = this.RespondIfNotFound;
				bool? respondIfNotFound2 = other.RespondIfNotFound;
				return (respondIfNotFound.GetValueOrDefault() == respondIfNotFound2.GetValueOrDefault()) & (respondIfNotFound != null == (respondIfNotFound2 != null));
			}
			return false;
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x001A0444 File Offset: 0x0019E644
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.QueuedFileRequest)
			{
				global::BaseEntity.QueuedFileRequest queuedFileRequest = (global::BaseEntity.QueuedFileRequest)obj;
				return this.Equals(queuedFileRequest);
			}
			return false;
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x001A046C File Offset: 0x0019E66C
		public override int GetHashCode()
		{
			return (((((((((((this.Entity != null) ? this.Entity.GetHashCode() : 0) * 397) ^ (int)this.Type) * 397) ^ (int)this.Part) * 397) ^ (int)this.Crc) * 397) ^ (int)this.ResponseFunction) * 397) ^ this.RespondIfNotFound.GetHashCode();
		}

		// Token: 0x04003FD7 RID: 16343
		public readonly global::BaseEntity Entity;

		// Token: 0x04003FD8 RID: 16344
		public readonly FileStorage.Type Type;

		// Token: 0x04003FD9 RID: 16345
		public readonly uint Part;

		// Token: 0x04003FDA RID: 16346
		public readonly uint Crc;

		// Token: 0x04003FDB RID: 16347
		public readonly uint ResponseFunction;

		// Token: 0x04003FDC RID: 16348
		public readonly bool? RespondIfNotFound;
	}

	// Token: 0x02000B88 RID: 2952
	private readonly struct PendingFileRequest : IEquatable<global::BaseEntity.PendingFileRequest>
	{
		// Token: 0x06004D38 RID: 19768 RVA: 0x001A04E4 File Offset: 0x0019E6E4
		public PendingFileRequest(FileStorage.Type type, uint numId, uint crc, IServerFileReceiver receiver)
		{
			this.Type = type;
			this.NumId = numId;
			this.Crc = crc;
			this.Receiver = receiver;
			this.Time = UnityEngine.Time.realtimeSinceStartup;
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x001A050E File Offset: 0x0019E70E
		public bool Equals(global::BaseEntity.PendingFileRequest other)
		{
			return this.Type == other.Type && this.NumId == other.NumId && this.Crc == other.Crc && object.Equals(this.Receiver, other.Receiver);
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x001A0550 File Offset: 0x0019E750
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.PendingFileRequest)
			{
				global::BaseEntity.PendingFileRequest pendingFileRequest = (global::BaseEntity.PendingFileRequest)obj;
				return this.Equals(pendingFileRequest);
			}
			return false;
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x001A0577 File Offset: 0x0019E777
		public override int GetHashCode()
		{
			return (int)((((((this.Type * (FileStorage.Type)397) ^ (FileStorage.Type)this.NumId) * (FileStorage.Type)397) ^ (FileStorage.Type)this.Crc) * (FileStorage.Type)397) ^ (FileStorage.Type)((this.Receiver != null) ? this.Receiver.GetHashCode() : 0));
		}

		// Token: 0x04003FDD RID: 16349
		public readonly FileStorage.Type Type;

		// Token: 0x04003FDE RID: 16350
		public readonly uint NumId;

		// Token: 0x04003FDF RID: 16351
		public readonly uint Crc;

		// Token: 0x04003FE0 RID: 16352
		public readonly IServerFileReceiver Receiver;

		// Token: 0x04003FE1 RID: 16353
		public readonly float Time;
	}

	// Token: 0x02000B89 RID: 2953
	public static class Query
	{
		// Token: 0x04003FE2 RID: 16354
		public static global::BaseEntity.Query.EntityTree Server;

		// Token: 0x02000FCA RID: 4042
		public class EntityTree
		{
			// Token: 0x060055AD RID: 21933 RVA: 0x001BAB2F File Offset: 0x001B8D2F
			public EntityTree(float worldSize)
			{
				this.Grid = new Grid<global::BaseEntity>(32, worldSize);
				this.PlayerGrid = new Grid<global::BasePlayer>(32, worldSize);
				this.BrainGrid = new Grid<global::BaseEntity>(32, worldSize);
			}

			// Token: 0x060055AE RID: 21934 RVA: 0x001BAB64 File Offset: 0x001B8D64
			public void Add(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Add(ent, position.x, position.z);
			}

			// Token: 0x060055AF RID: 21935 RVA: 0x001BAB98 File Offset: 0x001B8D98
			public void AddPlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Add(player, position.x, position.z);
			}

			// Token: 0x060055B0 RID: 21936 RVA: 0x001BABCC File Offset: 0x001B8DCC
			public void AddBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Add(entity, position.x, position.z);
			}

			// Token: 0x060055B1 RID: 21937 RVA: 0x001BAC00 File Offset: 0x001B8E00
			public void Remove(global::BaseEntity ent, bool isPlayer = false)
			{
				this.Grid.Remove(ent);
				if (isPlayer)
				{
					global::BasePlayer basePlayer = ent as global::BasePlayer;
					if (basePlayer != null)
					{
						this.PlayerGrid.Remove(basePlayer);
					}
				}
			}

			// Token: 0x060055B2 RID: 21938 RVA: 0x001BAC3A File Offset: 0x001B8E3A
			public void RemovePlayer(global::BasePlayer player)
			{
				this.PlayerGrid.Remove(player);
			}

			// Token: 0x060055B3 RID: 21939 RVA: 0x001BAC49 File Offset: 0x001B8E49
			public void RemoveBrain(global::BaseEntity entity)
			{
				if (entity == null)
				{
					return;
				}
				this.BrainGrid.Remove(entity);
			}

			// Token: 0x060055B4 RID: 21940 RVA: 0x001BAC64 File Offset: 0x001B8E64
			public void Move(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Move(ent, position.x, position.z);
				global::BasePlayer basePlayer = ent as global::BasePlayer;
				if (basePlayer != null)
				{
					this.MovePlayer(basePlayer);
				}
				if (ent.HasBrain)
				{
					this.MoveBrain(ent);
				}
			}

			// Token: 0x060055B5 RID: 21941 RVA: 0x001BACBC File Offset: 0x001B8EBC
			public void MovePlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Move(player, position.x, position.z);
			}

			// Token: 0x060055B6 RID: 21942 RVA: 0x001BACF0 File Offset: 0x001B8EF0
			public void MoveBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Move(entity, position.x, position.z);
			}

			// Token: 0x060055B7 RID: 21943 RVA: 0x001BAD21 File Offset: 0x001B8F21
			public int GetInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.Grid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x060055B8 RID: 21944 RVA: 0x001BAD3E File Offset: 0x001B8F3E
			public int GetPlayersInSphere(Vector3 position, float distance, global::BasePlayer[] results, Func<global::BasePlayer, bool> filter = null)
			{
				return this.PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x060055B9 RID: 21945 RVA: 0x001BAD5B File Offset: 0x001B8F5B
			public int GetBrainsInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.BrainGrid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x04005143 RID: 20803
			private Grid<global::BaseEntity> Grid;

			// Token: 0x04005144 RID: 20804
			private Grid<global::BasePlayer> PlayerGrid;

			// Token: 0x04005145 RID: 20805
			private Grid<global::BaseEntity> BrainGrid;
		}
	}

	// Token: 0x02000B8A RID: 2954
	public class RPC_Shared : Attribute
	{
	}

	// Token: 0x02000B8B RID: 2955
	public struct RPCMessage
	{
		// Token: 0x04003FE3 RID: 16355
		public Connection connection;

		// Token: 0x04003FE4 RID: 16356
		public global::BasePlayer player;

		// Token: 0x04003FE5 RID: 16357
		public NetRead read;
	}

	// Token: 0x02000B8C RID: 2956
	public class RPC_Server : global::BaseEntity.RPC_Shared
	{
		// Token: 0x02000FCB RID: 4043
		public abstract class Conditional : Attribute
		{
			// Token: 0x060055BA RID: 21946 RVA: 0x0002CFBB File Offset: 0x0002B1BB
			public virtual string GetArgs()
			{
				return null;
			}
		}

		// Token: 0x02000FCC RID: 4044
		public class MaxDistance : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x060055BC RID: 21948 RVA: 0x001BAD78 File Offset: 0x001B8F78
			public MaxDistance(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x060055BD RID: 21949 RVA: 0x001BAD87 File Offset: 0x001B8F87
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x060055BE RID: 21950 RVA: 0x001BAD99 File Offset: 0x001B8F99
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && ent.Distance(player.eyes.position) <= maximumDistance;
			}

			// Token: 0x04005146 RID: 20806
			private float maximumDistance;
		}

		// Token: 0x02000FCD RID: 4045
		public class IsVisible : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x060055BF RID: 21951 RVA: 0x001BADC7 File Offset: 0x001B8FC7
			public IsVisible(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x060055C0 RID: 21952 RVA: 0x001BADD6 File Offset: 0x001B8FD6
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x060055C1 RID: 21953 RVA: 0x001BADE8 File Offset: 0x001B8FE8
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 2162688, null) && (ent.IsVisible(player.eyes.HeadRay(), 1218519041, maximumDistance) || ent.IsVisible(player.eyes.position, maximumDistance));
			}

			// Token: 0x04005147 RID: 20807
			private float maximumDistance;
		}

		// Token: 0x02000FCE RID: 4046
		public class FromOwner : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x060055C2 RID: 21954 RVA: 0x001BAE60 File Offset: 0x001B9060
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				return !(ent == null) && !(player == null) && ent.net != null && player.net != null && (ent.net.ID == player.net.ID || !(ent.parentEntity.uid != player.net.ID));
			}
		}

		// Token: 0x02000FCF RID: 4047
		public class IsActiveItem : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x060055C4 RID: 21956 RVA: 0x001BAEDC File Offset: 0x001B90DC
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				global::Item activeItem = player.GetActiveItem();
				return activeItem != null && !(activeItem.GetHeldEntity() != ent);
			}
		}

		// Token: 0x02000FD0 RID: 4048
		public class CallsPerSecond : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x060055C6 RID: 21958 RVA: 0x001BAF6A File Offset: 0x001B916A
			public CallsPerSecond(ulong limit)
			{
				this.callsPerSecond = limit;
			}

			// Token: 0x060055C7 RID: 21959 RVA: 0x001BAF79 File Offset: 0x001B9179
			public override string GetArgs()
			{
				return this.callsPerSecond.ToString();
			}

			// Token: 0x060055C8 RID: 21960 RVA: 0x001BAF86 File Offset: 0x001B9186
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, ulong callsPerSecond)
			{
				return !(ent == null) && !(player == null) && player.rpcHistory.TryIncrement(id, callsPerSecond);
			}

			// Token: 0x04005148 RID: 20808
			private ulong callsPerSecond;
		}
	}

	// Token: 0x02000B8D RID: 2957
	public enum Signal
	{
		// Token: 0x04003FE7 RID: 16359
		Attack,
		// Token: 0x04003FE8 RID: 16360
		Alt_Attack,
		// Token: 0x04003FE9 RID: 16361
		DryFire,
		// Token: 0x04003FEA RID: 16362
		Reload,
		// Token: 0x04003FEB RID: 16363
		Deploy,
		// Token: 0x04003FEC RID: 16364
		Flinch_Head,
		// Token: 0x04003FED RID: 16365
		Flinch_Chest,
		// Token: 0x04003FEE RID: 16366
		Flinch_Stomach,
		// Token: 0x04003FEF RID: 16367
		Flinch_RearHead,
		// Token: 0x04003FF0 RID: 16368
		Flinch_RearTorso,
		// Token: 0x04003FF1 RID: 16369
		Throw,
		// Token: 0x04003FF2 RID: 16370
		Relax,
		// Token: 0x04003FF3 RID: 16371
		Gesture,
		// Token: 0x04003FF4 RID: 16372
		PhysImpact,
		// Token: 0x04003FF5 RID: 16373
		Eat,
		// Token: 0x04003FF6 RID: 16374
		Startled,
		// Token: 0x04003FF7 RID: 16375
		Admire
	}

	// Token: 0x02000B8E RID: 2958
	public enum Slot
	{
		// Token: 0x04003FF9 RID: 16377
		Lock,
		// Token: 0x04003FFA RID: 16378
		FireMod,
		// Token: 0x04003FFB RID: 16379
		UpperModifier,
		// Token: 0x04003FFC RID: 16380
		MiddleModifier,
		// Token: 0x04003FFD RID: 16381
		LowerModifier,
		// Token: 0x04003FFE RID: 16382
		CenterDecoration,
		// Token: 0x04003FFF RID: 16383
		LowerCenterDecoration,
		// Token: 0x04004000 RID: 16384
		StorageMonitor,
		// Token: 0x04004001 RID: 16385
		Count
	}

	// Token: 0x02000B8F RID: 2959
	[Flags]
	public enum TraitFlag
	{
		// Token: 0x04004003 RID: 16387
		None = 0,
		// Token: 0x04004004 RID: 16388
		Alive = 1,
		// Token: 0x04004005 RID: 16389
		Animal = 2,
		// Token: 0x04004006 RID: 16390
		Human = 4,
		// Token: 0x04004007 RID: 16391
		Interesting = 8,
		// Token: 0x04004008 RID: 16392
		Food = 16,
		// Token: 0x04004009 RID: 16393
		Meat = 32,
		// Token: 0x0400400A RID: 16394
		Water = 32
	}

	// Token: 0x02000B90 RID: 2960
	public static class Util
	{
		// Token: 0x06004D3E RID: 19774 RVA: 0x001A05C0 File Offset: 0x0019E7C0
		public static global::BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
		{
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
				{
					if (x is global::BasePlayer)
					{
						global::BasePlayer basePlayer = x as global::BasePlayer;
						return string.IsNullOrEmpty(strFilter) || (strFilter == "!alive" && basePlayer.IsAlive()) || (strFilter == "!sleeping" && basePlayer.IsSleeping()) || strFilter[0] == '!' || basePlayer.displayName.Contains(strFilter, CompareOptions.IgnoreCase) || basePlayer.UserIDString.Contains(strFilter);
					}
					return !onlyPlayers && !string.IsNullOrEmpty(strFilter) && x.ShortPrefabName.Contains(strFilter);
				})
				select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x001A0620 File Offset: 0x0019E820
		public static global::BaseEntity[] FindTargetsOwnedBy(ulong ownedBy, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
				{
					global::BaseEntity baseEntity;
					if ((baseEntity = x as global::BaseEntity) != null)
					{
						if (baseEntity.OwnerID != ownedBy)
						{
							return false;
						}
						if (!hasFilter || baseEntity.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					return false;
				})
				select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x001A0694 File Offset: 0x0019E894
		public static global::BaseEntity[] FindTargetsAuthedTo(ulong authId, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
				{
					BuildingPrivlidge buildingPrivlidge;
					global::AutoTurret autoTurret;
					global::CodeLock codeLock;
					if ((buildingPrivlidge = x as BuildingPrivlidge) != null)
					{
						if (!buildingPrivlidge.IsAuthed(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					else if ((autoTurret = x as global::AutoTurret) != null)
					{
						if (!autoTurret.IsAuthed(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					else if ((codeLock = x as global::CodeLock) != null)
					{
						if (!codeLock.whitelistPlayers.Contains(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					return false;
				})
				select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x001A0708 File Offset: 0x0019E908
		public static T[] FindAll<T>() where T : global::BaseEntity
		{
			return global::BaseNetworkable.serverEntities.OfType<T>().ToArray<T>();
		}
	}

	// Token: 0x02000B91 RID: 2961
	public enum GiveItemReason
	{
		// Token: 0x0400400C RID: 16396
		Generic,
		// Token: 0x0400400D RID: 16397
		ResourceHarvested,
		// Token: 0x0400400E RID: 16398
		PickedUp,
		// Token: 0x0400400F RID: 16399
		Crafted
	}
}
