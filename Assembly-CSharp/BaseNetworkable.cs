using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020003BE RID: 958
public abstract class BaseNetworkable : BaseMonoBehaviour, IPrefabPostProcess, IEntity, NetworkHandler
{
	// Token: 0x0600216B RID: 8555 RVA: 0x000DAEE8 File Offset: 0x000D90E8
	public void BroadcastOnPostNetworkUpdate(global::BaseEntity entity)
	{
		foreach (Component component in this.postNetworkUpdateComponents)
		{
			IOnPostNetworkUpdate onPostNetworkUpdate = component as IOnPostNetworkUpdate;
			if (onPostNetworkUpdate != null)
			{
				onPostNetworkUpdate.OnPostNetworkUpdate(entity);
			}
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.BroadcastOnPostNetworkUpdate(entity);
		}
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000DAF84 File Offset: 0x000D9184
	public virtual void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (serverside)
		{
			return;
		}
		this.postNetworkUpdateComponents = base.GetComponentsInChildren<IOnPostNetworkUpdate>(true).Cast<Component>().ToList<Component>();
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x0600216D RID: 8557 RVA: 0x000DAFA2 File Offset: 0x000D91A2
	// (set) Token: 0x0600216E RID: 8558 RVA: 0x000DAFAA File Offset: 0x000D91AA
	public bool limitNetworking
	{
		get
		{
			return this._limitedNetworking;
		}
		set
		{
			if (value == this._limitedNetworking)
			{
				return;
			}
			this._limitedNetworking = value;
			if (this._limitedNetworking)
			{
				this.OnNetworkLimitStart();
			}
			else
			{
				this.OnNetworkLimitEnd();
			}
			this.UpdateNetworkGroup();
		}
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000DAFDC File Offset: 0x000D91DC
	private void OnNetworkLimitStart()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> list = this.GetSubscribers();
		if (list == null)
		{
			return;
		}
		list = list.ToList<Connection>();
		list.RemoveAll((Connection x) => this.ShouldNetworkTo(x.player as global::BasePlayer));
		this.OnNetworkSubscribersLeave(list);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitStart();
			}
		}
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000DB070 File Offset: 0x000D9270
	private void OnNetworkLimitEnd()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		this.OnNetworkSubscribersEnter(subscribers);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitEnd();
			}
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000DB0E8 File Offset: 0x000D92E8
	public global::BaseEntity GetParentEntity()
	{
		return this.parentEntity.Get(this.isServer);
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000DB0FB File Offset: 0x000D92FB
	public bool HasParent()
	{
		return this.parentEntity.IsValid(this.isServer);
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000DB10E File Offset: 0x000D930E
	public void AddChild(global::BaseEntity child)
	{
		if (this.children.Contains(child))
		{
			return;
		}
		this.children.Add(child);
		this.OnChildAdded(child);
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnChildAdded(global::BaseEntity child)
	{
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000DB132 File Offset: 0x000D9332
	public void RemoveChild(global::BaseEntity child)
	{
		this.children.Remove(child);
		this.OnChildRemoved(child);
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnChildRemoved(global::BaseEntity child)
	{
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06002177 RID: 8567 RVA: 0x000DB148 File Offset: 0x000D9348
	public GameManager gameManager
	{
		get
		{
			if (this.isServer)
			{
				return GameManager.server;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06002178 RID: 8568 RVA: 0x000DB162 File Offset: 0x000D9362
	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (this.isServer)
			{
				return PrefabAttribute.server;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06002179 RID: 8569 RVA: 0x000DB17C File Offset: 0x000D937C
	public static Group GlobalNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(0U);
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x0600217A RID: 8570 RVA: 0x000DB18E File Offset: 0x000D938E
	public static Group LimboNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(1U);
		}
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000CD2A7 File Offset: 0x000CB4A7
	public virtual float GetNetworkTime()
	{
		return UnityEngine.Time.time;
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000DB1A0 File Offset: 0x000D93A0
	public virtual void Spawn()
	{
		this.SpawnShared();
		if (this.net == null)
		{
			this.net = Network.Net.sv.CreateNetworkable();
		}
		this.creationFrame = UnityEngine.Time.frameCount;
		this.PreInitShared();
		this.InitShared();
		this.ServerInit();
		this.PostInitShared();
		this.UpdateNetworkGroup();
		this.isSpawned = true;
		this.SendNetworkUpdateImmediate(true);
		if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
		}
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000DB221 File Offset: 0x000D9421
	public bool IsFullySpawned()
	{
		return this.isSpawned;
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000DB229 File Offset: 0x000D9429
	public virtual void ServerInit()
	{
		global::BaseNetworkable.serverEntities.RegisterID(this);
		if (this.net != null)
		{
			this.net.handler = this;
		}
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000DB24A File Offset: 0x000D944A
	protected List<Connection> GetSubscribers()
	{
		if (this.net == null)
		{
			return null;
		}
		if (this.net.group == null)
		{
			return null;
		}
		return this.net.group.subscribers;
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x00003384 File Offset: 0x00001584
	public void KillMessage()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x00029C50 File Offset: 0x00027E50
	public virtual void AdminKill()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000DB275 File Offset: 0x000D9475
	public void Kill(global::BaseNetworkable.DestroyMode mode = global::BaseNetworkable.DestroyMode.None)
	{
		if (this.IsDestroyed)
		{
			Debug.LogWarning("Calling kill - but already IsDestroyed!? " + this);
			return;
		}
		base.gameObject.BroadcastOnParentDestroying();
		this.DoEntityDestroy();
		this.TerminateOnClient(mode);
		this.TerminateOnServer();
		this.EntityDestroy();
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000DB2B4 File Offset: 0x000D94B4
	private void TerminateOnClient(global::BaseNetworkable.DestroyMode mode)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Term {0}", mode);
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8((byte)mode);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000DB33B File Offset: 0x000D953B
	private void TerminateOnServer()
	{
		if (this.net == null)
		{
			return;
		}
		this.InvalidateNetworkCache();
		global::BaseNetworkable.serverEntities.UnregisterID(this);
		Network.Net.sv.DestroyNetworkable(ref this.net);
		base.StopAllCoroutines();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000DB379 File Offset: 0x000D9579
	internal virtual void DoServerDestroy()
	{
		this.isSpawned = false;
		Analytics.Azure.OnEntityDestroyed(this);
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000DB388 File Offset: 0x000D9588
	public virtual bool ShouldNetworkTo(global::BasePlayer player)
	{
		return this.net.group == null || player.net.subscriber.IsSubscribed(this.net.group);
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000DB3B4 File Offset: 0x000D95B4
	protected void SendNetworkGroupChange()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net.group == null)
		{
			Debug.LogWarning(this.ToString() + " changed its network group to null");
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.GroupChange);
		netWrite.EntityID(this.net.ID);
		netWrite.GroupID(this.net.group.ID);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000DB44C File Offset: 0x000D964C
	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		connection.validate.entityUpdates = connection.validate.entityUpdates + 1U;
		global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
		{
			forConnection = connection,
			forDisk = false
		};
		netWrite.PacketID(Message.Type.Entities);
		netWrite.UInt32(connection.validate.entityUpdates);
		this.ToStreamForNetwork(netWrite, saveInfo);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000DB4BC File Offset: 0x000D96BC
	public void SendNetworkUpdate(global::BasePlayer.NetworkQueue queue = global::BasePlayer.NetworkQueue.Update)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					global::BasePlayer basePlayer = subscribers[i].player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						basePlayer.QueueUpdate(queue, this);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000DB590 File Offset: 0x000D9790
	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdateImmediate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection connection = subscribers[i];
					global::BasePlayer basePlayer = connection.player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						this.SendAsSnapshot(connection, justCreated);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000DB66C File Offset: 0x000D986C
	protected void SendNetworkUpdate_Position()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate_Position", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Position");
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityPosition);
				netWrite.EntityID(this.net.ID);
				NetWrite netWrite2 = netWrite;
				Vector3 vector = this.GetNetworkPosition();
				netWrite2.Vector3(vector);
				NetWrite netWrite3 = netWrite;
				vector = this.GetNetworkRotation().eulerAngles;
				netWrite3.Vector3(vector);
				netWrite.Float(this.GetNetworkTime());
				NetworkableId uid = this.parentEntity.uid;
				if (uid.IsValid)
				{
					netWrite.EntityID(uid);
				}
				SendInfo sendInfo = new SendInfo(subscribers)
				{
					method = SendMethod.ReliableUnordered,
					priority = Priority.Immediate
				};
				netWrite.Send(sendInfo);
			}
		}
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000DB78C File Offset: 0x000D998C
	private void ToStream(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		using (saveInfo.msg = Facepunch.Pool.Get<ProtoBuf.Entity>())
		{
			this.Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				Debug.LogError(this + ": ToStream - no BaseEntity!?");
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				Debug.LogError(this + ": ToStream - no baseNetworkable!?");
			}
			saveInfo.msg.ToProto(stream);
			this.PostSave(saveInfo);
		}
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000DB81C File Offset: 0x000D9A1C
	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000DB824 File Offset: 0x000D9A24
	public void ToStreamForNetwork(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		if (!this.CanUseNetworkCache(saveInfo.forConnection))
		{
			this.ToStream(stream, saveInfo);
			return;
		}
		if (this._NetworkCache == null)
		{
			this._NetworkCache = ((global::BaseNetworkable.EntityMemoryStreamPool.Count > 0) ? (this._NetworkCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8));
			this.ToStream(this._NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)this._NetworkCache.Length;
		}
		this._NetworkCache.WriteTo(stream);
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000DB8B0 File Offset: 0x000D9AB0
	public void InvalidateNetworkCache()
	{
		using (TimeWarning.New("InvalidateNetworkCache", 0))
		{
			if (this._SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)this._SaveCache.Length;
				this._SaveCache.SetLength(0L);
				this._SaveCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._SaveCache);
				this._SaveCache = null;
			}
			if (this._NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)this._NetworkCache.Length;
				this._NetworkCache.SetLength(0L);
				this._NetworkCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._NetworkCache);
				this._NetworkCache = null;
			}
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000DB994 File Offset: 0x000D9B94
	public MemoryStream GetSaveCache()
	{
		if (this._SaveCache == null)
		{
			if (global::BaseNetworkable.EntityMemoryStreamPool.Count > 0)
			{
				this._SaveCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue();
			}
			else
			{
				this._SaveCache = new MemoryStream(8);
			}
			global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
			{
				forDisk = true
			};
			this.ToStream(this._SaveCache, saveInfo);
			ConVar.Server.savecachesize += (int)this._SaveCache.Length;
		}
		return this._SaveCache;
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000DBA10 File Offset: 0x000D9C10
	public virtual void UpdateNetworkGroup()
	{
		Assert.IsTrue(this.isServer, "UpdateNetworkGroup called on clientside entity!");
		if (this.net == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateGroups", 0))
		{
			if (this.net.UpdateGroups(base.transform.position))
			{
				this.SendNetworkGroupChange();
			}
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06002192 RID: 8594 RVA: 0x000DBA7C File Offset: 0x000D9C7C
	// (set) Token: 0x06002193 RID: 8595 RVA: 0x000DBA84 File Offset: 0x000D9C84
	public bool IsDestroyed { get; private set; }

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06002194 RID: 8596 RVA: 0x000DBA8D File Offset: 0x000D9C8D
	public string PrefabName
	{
		get
		{
			if (this._prefabName == null)
			{
				this._prefabName = StringPool.Get(this.prefabID);
			}
			return this._prefabName;
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06002195 RID: 8597 RVA: 0x000DBAAE File Offset: 0x000D9CAE
	public string ShortPrefabName
	{
		get
		{
			if (this._prefabNameWithoutExtension == null)
			{
				this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
			}
			return this._prefabNameWithoutExtension;
		}
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x0002C8A6 File Offset: 0x0002AAA6
	public virtual Vector3 GetNetworkPosition()
	{
		return base.transform.localPosition;
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x0002C8E1 File Offset: 0x0002AAE1
	public virtual Quaternion GetNetworkRotation()
	{
		return base.transform.localRotation;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000DBAD0 File Offset: 0x000D9CD0
	public string InvokeString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Facepunch.Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes(this, list);
		foreach (InvokeAction invokeAction in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(invokeAction.action.Method.Name);
		}
		Facepunch.Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000DBB64 File Offset: 0x000D9D64
	public global::BaseEntity LookupPrefab()
	{
		return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000DBB7C File Offset: 0x000D9D7C
	public bool EqualNetID(global::BaseNetworkable other)
	{
		return !other.IsRealNull() && other.net != null && this.net != null && other.net.ID == this.net.ID;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000DBBB3 File Offset: 0x000D9DB3
	public bool EqualNetID(NetworkableId otherID)
	{
		return this.net != null && otherID == this.net.ID;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000DBBD0 File Offset: 0x000D9DD0
	public virtual void ResetState()
	{
		if (this.children.Count > 0)
		{
			this.children.Clear();
		}
		ILootableEntity lootableEntity;
		if ((lootableEntity = this as ILootableEntity) != null)
		{
			lootableEntity.LastLootedBy = 0UL;
		}
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void InitShared()
	{
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PreInitShared()
	{
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostInitShared()
	{
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DestroyShared()
	{
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000DBC08 File Offset: 0x000D9E08
	public void OnNetworkGroupChange()
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ShouldInheritNetworkGroup())
				{
					baseEntity.net.SwitchGroup(this.net.group);
				}
				else if (this.isServer)
				{
					baseEntity.UpdateNetworkGroup();
				}
			}
		}
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000DBC8C File Offset: 0x000D9E8C
	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			global::BasePlayer basePlayer = connection.player as global::BasePlayer;
			if (!(basePlayer == null))
			{
				basePlayer.QueueUpdate(global::BasePlayer.NetworkQueue.Update, this as global::BaseEntity);
			}
		}
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000DBD00 File Offset: 0x000D9F00
	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "LeaveVisibility");
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8(0);
		netWrite.Send(new SendInfo(connections));
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000DBD5B File Offset: 0x000D9F5B
	private void EntityDestroy()
	{
		if (base.gameObject)
		{
			this.ResetState();
			this.gameManager.Retire(base.gameObject);
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000DBD84 File Offset: 0x000D9F84
	private void DoEntityDestroy()
	{
		if (this.IsDestroyed)
		{
			return;
		}
		this.IsDestroyed = true;
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.DestroyShared();
		if (this.isServer)
		{
			this.DoServerDestroy();
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Rust.Registry.Entity.Unregister(base.gameObject);
		}
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000DBDF0 File Offset: 0x000D9FF0
	private void SpawnShared()
	{
		this.IsDestroyed = false;
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Rust.Registry.Entity.Register(base.gameObject, this);
		}
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000DBE38 File Offset: 0x000DA038
	public virtual void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.prefabID == 0U)
		{
			Debug.LogError("PrefabID is 0! " + base.transform.GetRecursiveName(""), base.gameObject);
		}
		info.msg.baseNetworkable = Facepunch.Pool.Get<ProtoBuf.BaseNetworkable>();
		info.msg.baseNetworkable.uid = this.net.ID;
		info.msg.baseNetworkable.prefabID = this.prefabID;
		if (this.net.group != null)
		{
			info.msg.baseNetworkable.group = this.net.group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = this.creationFrame == UnityEngine.Time.frameCount;
		}
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostSave(global::BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000DBF00 File Offset: 0x000DA100
	public void InitLoad(NetworkableId entityID)
	{
		this.net = Network.Net.sv.CreateNetworkable(entityID);
		global::BaseNetworkable.serverEntities.RegisterID(this);
		this.PreServerLoad();
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PreServerLoad()
	{
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000DBF24 File Offset: 0x000DA124
	public virtual void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.baseNetworkable == null)
		{
			return;
		}
		ProtoBuf.BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
		if (this.prefabID != baseNetworkable.prefabID)
		{
			Debug.LogError(string.Concat(new object[] { "Prefab IDs don't match! ", this.prefabID, "/", baseNetworkable.prefabID, " -> ", base.gameObject }), base.gameObject);
		}
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000DBFAC File Offset: 0x000DA1AC
	public virtual void PostServerLoad()
	{
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x060021AF RID: 8623 RVA: 0x0000441C File Offset: 0x0000261C
	public bool isServer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x060021B0 RID: 8624 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool isClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000DBFC0 File Offset: 0x000DA1C0
	public T ToServer<T>() where T : global::BaseNetworkable
	{
		if (this.isServer)
		{
			return this as T;
		}
		return default(T);
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000DBFEC File Offset: 0x000DA1EC
	public static List<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		global::BaseNetworkable.connectionsInSphereList.Clear();
		float num = distance * distance;
		List<Connection> subscribers = global::BaseNetworkable.GlobalNetworkGroup.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					global::BaseNetworkable.connectionsInSphereList.Add(connection);
				}
			}
		}
		return global::BaseNetworkable.connectionsInSphereList;
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000DC068 File Offset: 0x000DA268
	public static void GetCloseConnections(Vector3 position, float distance, List<global::BasePlayer> players)
	{
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					players.Add(basePlayer);
				}
			}
		}
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000DC0F8 File Offset: 0x000DA2F8
	public static bool HasCloseConnections(Vector3 position, float distance)
	{
		if (Network.Net.sv == null)
		{
			return false;
		}
		if (Network.Net.sv.visibility == null)
		{
			return false;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04001A19 RID: 6681
	public List<Component> postNetworkUpdateComponents = new List<Component>();

	// Token: 0x04001A1A RID: 6682
	private bool _limitedNetworking;

	// Token: 0x04001A1B RID: 6683
	[NonSerialized]
	public EntityRef parentEntity;

	// Token: 0x04001A1C RID: 6684
	[NonSerialized]
	public readonly List<global::BaseEntity> children = new List<global::BaseEntity>();

	// Token: 0x04001A1D RID: 6685
	[NonSerialized]
	public bool canTriggerParent = true;

	// Token: 0x04001A1E RID: 6686
	private int creationFrame;

	// Token: 0x04001A1F RID: 6687
	protected bool isSpawned;

	// Token: 0x04001A20 RID: 6688
	private MemoryStream _NetworkCache;

	// Token: 0x04001A21 RID: 6689
	public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();

	// Token: 0x04001A22 RID: 6690
	private MemoryStream _SaveCache;

	// Token: 0x04001A23 RID: 6691
	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	// Token: 0x04001A24 RID: 6692
	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	// Token: 0x04001A25 RID: 6693
	[NonSerialized]
	public Networkable net;

	// Token: 0x04001A27 RID: 6695
	private string _prefabName;

	// Token: 0x04001A28 RID: 6696
	private string _prefabNameWithoutExtension;

	// Token: 0x04001A29 RID: 6697
	public static global::BaseNetworkable.EntityRealm serverEntities = new global::BaseNetworkable.EntityRealmServer();

	// Token: 0x04001A2A RID: 6698
	private const bool isServersideEntity = true;

	// Token: 0x04001A2B RID: 6699
	private static List<Connection> connectionsInSphereList = new List<Connection>();

	// Token: 0x02000CD2 RID: 3282
	public struct SaveInfo
	{
		// Token: 0x06004FF3 RID: 20467 RVA: 0x001A7D13 File Offset: 0x001A5F13
		internal bool SendingTo(Connection ownerConnection)
		{
			return ownerConnection != null && this.forConnection != null && this.forConnection == ownerConnection;
		}

		// Token: 0x04004580 RID: 17792
		public ProtoBuf.Entity msg;

		// Token: 0x04004581 RID: 17793
		public bool forDisk;

		// Token: 0x04004582 RID: 17794
		public Connection forConnection;
	}

	// Token: 0x02000CD3 RID: 3283
	public struct LoadInfo
	{
		// Token: 0x04004583 RID: 17795
		public ProtoBuf.Entity msg;

		// Token: 0x04004584 RID: 17796
		public bool fromDisk;
	}

	// Token: 0x02000CD4 RID: 3284
	public class EntityRealmServer : global::BaseNetworkable.EntityRealm
	{
		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06004FF4 RID: 20468 RVA: 0x001A7D2D File Offset: 0x001A5F2D
		protected override Manager visibilityManager
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return null;
				}
				return Network.Net.sv.visibility;
			}
		}
	}

	// Token: 0x02000CD5 RID: 3285
	public abstract class EntityRealm : IEnumerable<global::BaseNetworkable>, IEnumerable
	{
		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06004FF6 RID: 20470 RVA: 0x001A7D4A File Offset: 0x001A5F4A
		public int Count
		{
			get
			{
				return this.entityList.Count;
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06004FF7 RID: 20471
		protected abstract Manager visibilityManager { get; }

		// Token: 0x06004FF8 RID: 20472 RVA: 0x001A7D57 File Offset: 0x001A5F57
		public bool Contains(NetworkableId uid)
		{
			return this.entityList.Contains(uid);
		}

		// Token: 0x06004FF9 RID: 20473 RVA: 0x001A7D68 File Offset: 0x001A5F68
		public global::BaseNetworkable Find(NetworkableId uid)
		{
			global::BaseNetworkable baseNetworkable = null;
			if (!this.entityList.TryGetValue(uid, out baseNetworkable))
			{
				return null;
			}
			return baseNetworkable;
		}

		// Token: 0x06004FFA RID: 20474 RVA: 0x001A7D8C File Offset: 0x001A5F8C
		public void RegisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (this.entityList.Contains(ent.net.ID))
				{
					this.entityList[ent.net.ID] = ent;
					return;
				}
				this.entityList.Add(ent.net.ID, ent);
			}
		}

		// Token: 0x06004FFB RID: 20475 RVA: 0x001A7DE8 File Offset: 0x001A5FE8
		public void UnregisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				this.entityList.Remove(ent.net.ID);
			}
		}

		// Token: 0x06004FFC RID: 20476 RVA: 0x001A7E0C File Offset: 0x001A600C
		public Group FindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.Get(uid);
		}

		// Token: 0x06004FFD RID: 20477 RVA: 0x001A7E2C File Offset: 0x001A602C
		public Group TryFindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.TryGet(uid);
		}

		// Token: 0x06004FFE RID: 20478 RVA: 0x001A7E4C File Offset: 0x001A604C
		public void FindInGroup(uint uid, List<global::BaseNetworkable> list)
		{
			Group group = this.TryFindGroup(uid);
			if (group == null)
			{
				return;
			}
			int count = group.networkables.Values.Count;
			Networkable[] buffer = group.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				Networkable networkable = buffer[i];
				global::BaseNetworkable baseNetworkable = this.Find(networkable.ID);
				if (!(baseNetworkable == null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning("Group ID mismatch: " + baseNetworkable.ToString());
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		// Token: 0x06004FFF RID: 20479 RVA: 0x001A7EFC File Offset: 0x001A60FC
		public IEnumerator<global::BaseNetworkable> GetEnumerator()
		{
			return this.entityList.Values.GetEnumerator();
		}

		// Token: 0x06005000 RID: 20480 RVA: 0x001A7F13 File Offset: 0x001A6113
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x001A7F1B File Offset: 0x001A611B
		public void Clear()
		{
			this.entityList.Clear();
		}

		// Token: 0x04004585 RID: 17797
		private ListDictionary<NetworkableId, global::BaseNetworkable> entityList = new ListDictionary<NetworkableId, global::BaseNetworkable>();
	}

	// Token: 0x02000CD6 RID: 3286
	public enum DestroyMode : byte
	{
		// Token: 0x04004587 RID: 17799
		None,
		// Token: 0x04004588 RID: 17800
		Gib
	}
}
