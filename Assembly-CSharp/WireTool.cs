using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F2 RID: 242
public class WireTool : HeldEntity
{
	// Token: 0x06001534 RID: 5428 RVA: 0x000A6FC8 File Offset: 0x000A51C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WireTool.OnRpcMessage", 0))
		{
			if (rpc == 40328523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MakeConnection ");
				}
				using (TimeWarning.New("MakeConnection", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(40328523U, "MakeConnection", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(40328523U, "MakeConnection", this, player))
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
							this.MakeConnection(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in MakeConnection");
					}
				}
				return true;
			}
			if (rpc == 121409151U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestChangeColor ");
				}
				using (TimeWarning.New("RequestChangeColor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
					}
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
							this.RequestChangeColor(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RequestChangeColor");
					}
				}
				return true;
			}
			if (rpc == 2469840259U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestClear ");
				}
				using (TimeWarning.New("RequestClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestClear(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RequestClear");
					}
				}
				return true;
			}
			if (rpc == 2596458392U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetPlugged ");
				}
				using (TimeWarning.New("SetPlugged", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetPlugged(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in SetPlugged");
					}
				}
				return true;
			}
			if (rpc == 210386477U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryClear ");
				}
				using (TimeWarning.New("TryClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryClear(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in TryClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x06001535 RID: 5429 RVA: 0x000A76DC File Offset: 0x000A58DC
	public bool CanChangeColours
	{
		get
		{
			return this.wireType == IOEntity.IOType.Electric || this.wireType == IOEntity.IOType.Fluidic || this.wireType == IOEntity.IOType.Industrial;
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000A76FA File Offset: 0x000A58FA
	public void ClearPendingPlug()
	{
		this.pending.ent = null;
		this.pending.index = -1;
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x000A7714 File Offset: 0x000A5914
	public bool HasPendingPlug()
	{
		return this.pending.ent != null && this.pending.index != -1;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x000A773C File Offset: 0x000A593C
	public bool PendingPlugIsInput()
	{
		return this.pending.ent != null && this.pending.index != -1 && this.pending.input;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000A776C File Offset: 0x000A596C
	public bool PendingPlugIsType(IOEntity.IOType type)
	{
		return this.pending.ent != null && this.pending.index != -1 && ((this.pending.input && this.pending.ent.inputs[this.pending.index].type == type) || (!this.pending.input && this.pending.ent.outputs[this.pending.index].type == type));
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000A7802 File Offset: 0x000A5A02
	public bool PendingPlugIsOutput()
	{
		return this.pending.ent != null && this.pending.index != -1 && !this.pending.input;
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x000A7838 File Offset: 0x000A5A38
	public Vector3 PendingPlugWorldPos()
	{
		if (this.pending.ent == null || this.pending.index == -1)
		{
			return Vector3.zero;
		}
		if (this.pending.input)
		{
			return this.pending.ent.transform.TransformPoint(this.pending.ent.inputs[this.pending.index].handlePosition);
		}
		return this.pending.ent.transform.TransformPoint(this.pending.ent.outputs[this.pending.index].handlePosition);
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x000A78E8 File Offset: 0x000A5AE8
	public static bool CanPlayerUseWires(BasePlayer player)
	{
		if (!player.CanBuild())
		{
			return false;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(player.eyes.position, 0.1f, list, 536870912, QueryTriggerInteraction.Collide);
		bool flag = list.All((Collider collider) => collider.gameObject.CompareTag("IgnoreWireCheck"));
		Facepunch.Pool.FreeList<Collider>(ref list);
		return flag;
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x000A794D File Offset: 0x000A5B4D
	public static bool CanModifyEntity(BasePlayer player, IOEntity ent)
	{
		return player.CanBuild(ent.transform.position, ent.transform.rotation, ent.bounds) && ent.AllowWireConnections();
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x000A797B File Offset: 0x000A5B7B
	public bool PendingPlugRoot()
	{
		return this.pending.ent != null && this.pending.ent.IsRootEntity();
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000A79A4 File Offset: 0x000A5BA4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void TryClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		NetworkableId networkableId = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(networkableId);
		IOEntity ioentity = ((baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>());
		if (ioentity == null)
		{
			return;
		}
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		ioentity.ClearConnections();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000A7A10 File Offset: 0x000A5C10
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void MakeConnection(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = msg.read.Vector3();
			list.Add(vector);
		}
		NetworkableId networkableId = msg.read.EntityID();
		int num2 = msg.read.Int32();
		NetworkableId networkableId2 = msg.read.EntityID();
		int num3 = msg.read.Int32();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(networkableId);
		IOEntity ioentity = ((baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>());
		if (ioentity == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(networkableId2);
		IOEntity ioentity2 = ((baseNetworkable2 == null) ? null : baseNetworkable2.GetComponent<IOEntity>());
		if (ioentity2 == null)
		{
			return;
		}
		if (!this.ValidateLine(list, ioentity, ioentity2, player, num3))
		{
			return;
		}
		if (Vector3.Distance(baseNetworkable2.transform.position, baseNetworkable.transform.position) > WireTool.maxWireLength)
		{
			return;
		}
		if (num2 >= ioentity.inputs.Length)
		{
			return;
		}
		if (num3 >= ioentity2.outputs.Length)
		{
			return;
		}
		if (ioentity.inputs[num2].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity2.outputs[num3].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity.inputs[num2].rootConnectionsOnly && !ioentity2.IsRootEntity())
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity2))
		{
			return;
		}
		ioentity.inputs[num2].connectedTo.Set(ioentity2);
		ioentity.inputs[num2].connectedToSlot = num3;
		ioentity.inputs[num2].wireColour = wireColour;
		ioentity.inputs[num2].connectedTo.Init();
		ioentity2.outputs[num3].connectedTo.Set(ioentity);
		ioentity2.outputs[num3].connectedToSlot = num2;
		ioentity2.outputs[num3].linePoints = list.ToArray();
		ioentity2.outputs[num3].wireColour = wireColour;
		ioentity2.outputs[num3].connectedTo.Init();
		ioentity2.outputs[num3].worldSpaceLineEndRotation = ioentity.transform.TransformDirection(ioentity.inputs[num2].handleDirection);
		ioentity2.MarkDirtyForceUpdateOutputs();
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity2.SendChangedToRoot(true);
		ioentity2.RefreshIndustrialPreventBuilding();
		if (this.wireType == IOEntity.IOType.Industrial)
		{
			ioentity.NotifyIndustrialNetworkChanged();
			ioentity2.NotifyIndustrialNetworkChanged();
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	public void SetPlugged(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x000A7CE0 File Offset: 0x000A5EE0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		int num = msg.read.Int32();
		bool flag = msg.read.Bit();
		WireTool.AttemptClearSlot(BaseNetworkable.serverEntities.Find(networkableId), player, num, flag);
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000A7D34 File Offset: 0x000A5F34
	public static void AttemptClearSlot(BaseNetworkable clearEnt, BasePlayer ply, int clearIndex, bool isInput)
	{
		IOEntity ioentity = ((clearEnt == null) ? null : clearEnt.GetComponent<IOEntity>());
		if (ioentity == null)
		{
			return;
		}
		if (ply != null && !WireTool.CanModifyEntity(ply, ioentity))
		{
			return;
		}
		if (clearIndex >= (isInput ? ioentity.inputs.Length : ioentity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot ioslot = (isInput ? ioentity.inputs[clearIndex] : ioentity.outputs[clearIndex]);
		if (ioslot.connectedTo.Get(true) == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		IOEntity.IOSlot ioslot2 = (isInput ? ioentity2.outputs[ioslot.connectedToSlot] : ioentity2.inputs[ioslot.connectedToSlot]);
		if (isInput)
		{
			ioentity.UpdateFromInput(0, clearIndex);
		}
		else if (ioentity2)
		{
			ioentity2.UpdateFromInput(0, ioslot.connectedToSlot);
		}
		ioslot.Clear();
		ioslot2.Clear();
		ioentity.MarkDirtyForceUpdateOutputs();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.RefreshIndustrialPreventBuilding();
		if (ioentity2 != null)
		{
			ioentity2.RefreshIndustrialPreventBuilding();
		}
		if (isInput && ioentity2 != null)
		{
			ioentity2.SendChangedToRoot(true);
		}
		else if (!isInput)
		{
			foreach (IOEntity.IOSlot ioslot3 in ioentity.inputs)
			{
				if (ioslot3.mainPowerSlot && ioslot3.connectedTo.Get(true))
				{
					ioslot3.connectedTo.Get(true).SendChangedToRoot(true);
				}
			}
		}
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (ioentity != null && ioentity.ioType == IOEntity.IOType.Industrial)
		{
			ioentity.NotifyIndustrialNetworkChanged();
		}
		if (ioentity2 != null && ioentity2.ioType == IOEntity.IOType.Industrial)
		{
			ioentity2.NotifyIndustrialNetworkChanged();
		}
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000A7ED0 File Offset: 0x000A60D0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestChangeColor(BaseEntity.RPCMessage msg)
	{
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		int num = msg.read.Int32();
		bool flag = msg.read.Bit();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		IOEntity ioentity = BaseNetworkable.serverEntities.Find(networkableId) as IOEntity;
		if (ioentity == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot = (flag ? ioentity.inputs.ElementAtOrDefault(num) : ioentity.outputs.ElementAtOrDefault(num));
		if (ioslot == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		if (ioentity2 == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot2 = (flag ? ioentity2.outputs : ioentity2.inputs)[ioslot.connectedToSlot];
		ioslot.wireColour = wireColour;
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioslot2.wireColour = wireColour;
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000A7FB8 File Offset: 0x000A61B8
	private WireTool.WireColour IntToColour(int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		if (i >= 10)
		{
			i = 9;
		}
		WireTool.WireColour wireColour = (WireTool.WireColour)i;
		if (this.wireType == IOEntity.IOType.Fluidic && wireColour == WireTool.WireColour.Green)
		{
			wireColour = WireTool.WireColour.Default;
		}
		return wireColour;
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000A7FE8 File Offset: 0x000A61E8
	private bool ValidateLine(List<Vector3> lineList, IOEntity inputEntity, IOEntity outputEntity, BasePlayer byPlayer, int outputIndex)
	{
		if (lineList.Count < 2)
		{
			return false;
		}
		if (inputEntity == null || outputEntity == null)
		{
			return false;
		}
		Vector3 vector = lineList[0];
		float num = 0f;
		int count = lineList.Count;
		for (int i = 1; i < count; i++)
		{
			Vector3 vector2 = lineList[i];
			num += Vector3.Distance(vector, vector2);
			if (num > WireTool.maxWireLength)
			{
				return false;
			}
			vector = vector2;
		}
		Vector3 vector3 = lineList[count - 1];
		Bounds bounds = outputEntity.bounds;
		bounds.Expand(0.5f);
		if (!bounds.Contains(vector3))
		{
			return false;
		}
		Vector3 vector4 = outputEntity.transform.TransformPoint(lineList[0]);
		vector3 = inputEntity.transform.InverseTransformPoint(vector4);
		Bounds bounds2 = inputEntity.bounds;
		bounds2.Expand(0.5f);
		if (!bounds2.Contains(vector3))
		{
			return false;
		}
		if (byPlayer == null)
		{
			return false;
		}
		Vector3 vector5 = outputEntity.transform.TransformPoint(lineList[lineList.Count - 1]);
		return (byPlayer.Distance(vector5) <= 5f || byPlayer.Distance(vector4) <= 5f) && (outputIndex < 0 || outputIndex >= outputEntity.outputs.Length || outputEntity.outputs[outputIndex].type != IOEntity.IOType.Industrial || this.VerifyLineOfSight(lineList, outputEntity.transform.localToWorldMatrix));
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000A814C File Offset: 0x000A634C
	private bool VerifyLineOfSight(List<Vector3> positions, Matrix4x4 localToWorldSpace)
	{
		Vector3 vector = localToWorldSpace.MultiplyPoint3x4(positions[0]);
		for (int i = 1; i < positions.Count; i++)
		{
			Vector3 vector2 = localToWorldSpace.MultiplyPoint3x4(positions[i]);
			if (!this.VerifyLineOfSight(vector, vector2))
			{
				return false;
			}
			vector = vector2;
		}
		return true;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000A8198 File Offset: 0x000A6398
	private bool VerifyLineOfSight(Vector3 worldSpaceA, Vector3 worldSpaceB)
	{
		float num = Vector3.Distance(worldSpaceA, worldSpaceB);
		Vector3 normalized = (worldSpaceA - worldSpaceB).normalized;
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(worldSpaceB, normalized), 0.01f, list, num, 2162944, QueryTriggerInteraction.UseGlobal, null);
		bool flag = true;
		foreach (RaycastHit raycastHit in list)
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity != null && raycastHit.IsOnLayer(Rust.Layer.Deployed))
			{
				if (entity is VendingMachine)
				{
					flag = false;
					break;
				}
			}
			else if (!(entity != null) || !(entity is Door))
			{
				flag = false;
				break;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x04000D5F RID: 3423
	public Sprite InputSprite;

	// Token: 0x04000D60 RID: 3424
	public Sprite OutputSprite;

	// Token: 0x04000D61 RID: 3425
	public Sprite ClearSprite;

	// Token: 0x04000D62 RID: 3426
	public static float maxWireLength = 30f;

	// Token: 0x04000D63 RID: 3427
	private const int maxLineNodes = 16;

	// Token: 0x04000D64 RID: 3428
	public GameObjectRef plugEffect;

	// Token: 0x04000D65 RID: 3429
	public SoundDefinition clearStartSoundDef;

	// Token: 0x04000D66 RID: 3430
	public SoundDefinition clearSoundDef;

	// Token: 0x04000D67 RID: 3431
	public GameObjectRef ioLine;

	// Token: 0x04000D68 RID: 3432
	public IOEntity.IOType wireType;

	// Token: 0x04000D69 RID: 3433
	public float RadialMenuHoldTime = 0.25f;

	// Token: 0x04000D6A RID: 3434
	private const float IndustrialWallOffset = 0.03f;

	// Token: 0x04000D6B RID: 3435
	public static Translate.Phrase Default = new Translate.Phrase("wiretoolcolour.default", "Default");

	// Token: 0x04000D6C RID: 3436
	public static Translate.Phrase DefaultDesc = new Translate.Phrase("wiretoolcolour.default.desc", "Default connection color");

	// Token: 0x04000D6D RID: 3437
	public static Translate.Phrase Red = new Translate.Phrase("wiretoolcolour.red", "Red");

	// Token: 0x04000D6E RID: 3438
	public static Translate.Phrase RedDesc = new Translate.Phrase("wiretoolcolour.red.desc", "Red connection color");

	// Token: 0x04000D6F RID: 3439
	public static Translate.Phrase Green = new Translate.Phrase("wiretoolcolour.green", "Green");

	// Token: 0x04000D70 RID: 3440
	public static Translate.Phrase GreenDesc = new Translate.Phrase("wiretoolcolour.green.desc", "Green connection color");

	// Token: 0x04000D71 RID: 3441
	public static Translate.Phrase Blue = new Translate.Phrase("wiretoolcolour.blue", "Blue");

	// Token: 0x04000D72 RID: 3442
	public static Translate.Phrase BlueDesc = new Translate.Phrase("wiretoolcolour.blue.desc", "Blue connection color");

	// Token: 0x04000D73 RID: 3443
	public static Translate.Phrase Yellow = new Translate.Phrase("wiretoolcolour.yellow", "Yellow");

	// Token: 0x04000D74 RID: 3444
	public static Translate.Phrase YellowDesc = new Translate.Phrase("wiretoolcolour.yellow.desc", "Yellow connection color");

	// Token: 0x04000D75 RID: 3445
	public static Translate.Phrase LightBlue = new Translate.Phrase("wiretoolcolour.light_blue", "Light Blue");

	// Token: 0x04000D76 RID: 3446
	public static Translate.Phrase LightBlueDesc = new Translate.Phrase("wiretoolcolour.light_blue.desc", "Light Blue connection color");

	// Token: 0x04000D77 RID: 3447
	public static Translate.Phrase Orange = new Translate.Phrase("wiretoolcolour.orange", "Orange");

	// Token: 0x04000D78 RID: 3448
	public static Translate.Phrase OrangeDesc = new Translate.Phrase("wiretoolcolour.orange.desc", "Orange connection color");

	// Token: 0x04000D79 RID: 3449
	public static Translate.Phrase Purple = new Translate.Phrase("wiretoolcolour.purple", "Purple");

	// Token: 0x04000D7A RID: 3450
	public static Translate.Phrase PurpleDesc = new Translate.Phrase("wiretoolcolour.purple.desc", "Purple connection color");

	// Token: 0x04000D7B RID: 3451
	public static Translate.Phrase White = new Translate.Phrase("wiretoolcolour.white", "White");

	// Token: 0x04000D7C RID: 3452
	public static Translate.Phrase WhiteDesc = new Translate.Phrase("wiretoolcolour.white.desc", "White connection color");

	// Token: 0x04000D7D RID: 3453
	public static Translate.Phrase Pink = new Translate.Phrase("wiretoolcolour.pink", "Pink");

	// Token: 0x04000D7E RID: 3454
	public static Translate.Phrase PinkDesc = new Translate.Phrase("wiretoolcolour.pink.desc", "Pink connection color");

	// Token: 0x04000D7F RID: 3455
	public WireTool.PendingPlug_t pending;

	// Token: 0x04000D80 RID: 3456
	private const float IndustrialThickness = 0.01f;

	// Token: 0x02000C2C RID: 3116
	public enum WireColour
	{
		// Token: 0x040042BE RID: 17086
		Default,
		// Token: 0x040042BF RID: 17087
		Red,
		// Token: 0x040042C0 RID: 17088
		Green,
		// Token: 0x040042C1 RID: 17089
		Blue,
		// Token: 0x040042C2 RID: 17090
		Yellow,
		// Token: 0x040042C3 RID: 17091
		Pink,
		// Token: 0x040042C4 RID: 17092
		Purple,
		// Token: 0x040042C5 RID: 17093
		Orange,
		// Token: 0x040042C6 RID: 17094
		White,
		// Token: 0x040042C7 RID: 17095
		LightBlue,
		// Token: 0x040042C8 RID: 17096
		Count
	}

	// Token: 0x02000C2D RID: 3117
	public struct PendingPlug_t
	{
		// Token: 0x040042C9 RID: 17097
		public IOEntity ent;

		// Token: 0x040042CA RID: 17098
		public bool input;

		// Token: 0x040042CB RID: 17099
		public int index;

		// Token: 0x040042CC RID: 17100
		public GameObject tempLine;
	}
}
