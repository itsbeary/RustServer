using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D7 RID: 215
public class SprayCanSpray_Freehand : SprayCanSpray
{
	// Token: 0x0600130F RID: 4879 RVA: 0x000994B4 File Offset: 0x000976B4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray_Freehand.OnRpcMessage", 0))
		{
			if (rpc == 2020094435U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddPointMidSpray ");
				}
				using (TimeWarning.New("Server_AddPointMidSpray", 0))
				{
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
							this.Server_AddPointMidSpray(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_AddPointMidSpray");
					}
				}
				return true;
			}
			if (rpc == 117883393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_FinishEditing ");
				}
				using (TimeWarning.New("Server_FinishEditing", 0))
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
							this.Server_FinishEditing(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_FinishEditing");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06001310 RID: 4880 RVA: 0x00099714 File Offset: 0x00097914
	private bool AcceptingChanges
	{
		get
		{
			return this.editingPlayer.IsValid(true);
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00099722 File Offset: 0x00097922
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.LinePoints == null || this.LinePoints.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00099748 File Offset: 0x00097948
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.sprayLine == null)
		{
			info.msg.sprayLine = Facepunch.Pool.Get<SprayLine>();
		}
		if (info.msg.sprayLine.linePoints == null)
		{
			info.msg.sprayLine.linePoints = Facepunch.Pool.GetList<LinePoint>();
		}
		bool flag = this.AcceptingChanges && info.forDisk;
		if (this.LinePoints != null && !flag)
		{
			this.CopyPoints(this.LinePoints, info.msg.sprayLine.linePoints);
		}
		info.msg.sprayLine.width = this.width;
		info.msg.sprayLine.colour = new Vector3(this.colour.r, this.colour.g, this.colour.b);
		if (!info.forDisk)
		{
			info.msg.sprayLine.editingPlayer = this.editingPlayer.uid;
		}
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00099848 File Offset: 0x00097A48
	public void SetColour(Color newColour)
	{
		this.colour = newColour;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00099851 File Offset: 0x00097A51
	public void SetWidth(float lineWidth)
	{
		this.width = lineWidth;
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0009985C File Offset: 0x00097A5C
	[global::BaseEntity.RPC_Server]
	private void Server_AddPointMidSpray(global::BaseEntity.RPCMessage msg)
	{
		if (!this.AcceptingChanges || this.editingPlayer.Get(true) != msg.player)
		{
			return;
		}
		if (this.LinePoints.Count + 1 > 60)
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		if (Vector3.Distance(vector, this.LinePoints[0].LocalPosition) >= 10f)
		{
			return;
		}
		this.LinePoints.Add(new AlignedLineDrawer.LinePoint
		{
			LocalPosition = vector,
			WorldNormal = vector2
		});
		this.UpdateGroundWatch();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00099905 File Offset: 0x00097B05
	public void EnableChanges(global::BasePlayer byPlayer)
	{
		base.OwnerID = byPlayer.userID;
		this.editingPlayer.Set(byPlayer);
		base.Invoke(new Action(this.TimeoutEditing), 30f);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00099936 File Offset: 0x00097B36
	private void TimeoutEditing()
	{
		if (this.editingPlayer.IsSet)
		{
			this.editingPlayer.Set(null);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00099960 File Offset: 0x00097B60
	[global::BaseEntity.RPC_Server]
	private void Server_FinishEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer basePlayer = this.editingPlayer.Get(true);
		if (msg.player != basePlayer)
		{
			return;
		}
		bool flag = msg.read.Int32() == 1;
		SprayCan sprayCan;
		if (basePlayer != null && basePlayer.GetHeldEntity() != null && (sprayCan = basePlayer.GetHeldEntity() as SprayCan) != null)
		{
			sprayCan.ClearPaintingLine(flag);
		}
		this.editingPlayer.Set(null);
		SprayList sprayList = SprayList.Deserialize(msg.read);
		int count = sprayList.linePoints.Count;
		if (count > 70)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		if (this.LinePoints.Count <= 1)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		base.CancelInvoke(new Action(this.TimeoutEditing));
		this.LinePoints.Clear();
		for (int i = 0; i < count; i++)
		{
			if (sprayList.linePoints[i].localPosition.sqrMagnitude < 100f)
			{
				this.LinePoints.Add(new AlignedLineDrawer.LinePoint
				{
					LocalPosition = sprayList.linePoints[i].localPosition,
					WorldNormal = sprayList.linePoints[i].worldNormal
				});
			}
		}
		this.OnDeployed(null, basePlayer, null);
		this.UpdateGroundWatch();
		Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
		Facepunch.Pool.Free<SprayList>(ref sprayList);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x00099AF0 File Offset: 0x00097CF0
	public void AddInitialPoint(Vector3 atNormal)
	{
		this.LinePoints = new List<AlignedLineDrawer.LinePoint>
		{
			new AlignedLineDrawer.LinePoint
			{
				LocalPosition = Vector3.zero,
				WorldNormal = atNormal
			}
		};
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x00099B2C File Offset: 0x00097D2C
	private void UpdateGroundWatch()
	{
		if (base.isServer && this.LinePoints.Count > 1)
		{
			Vector3 vector = Vector3.Lerp(this.LinePoints[0].LocalPosition, this.LinePoints[this.LinePoints.Count - 1].LocalPosition, 0.5f);
			if (this.groundWatch != null)
			{
				this.groundWatch.groundPosition = vector;
			}
		}
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00099BA4 File Offset: 0x00097DA4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sprayLine != null)
		{
			if (info.msg.sprayLine.linePoints != null)
			{
				this.LinePoints.Clear();
				this.CopyPoints(info.msg.sprayLine.linePoints, this.LinePoints);
			}
			this.colour = new Color(info.msg.sprayLine.colour.x, info.msg.sprayLine.colour.y, info.msg.sprayLine.colour.z);
			this.width = info.msg.sprayLine.width;
			this.editingPlayer.uid = info.msg.sprayLine.editingPlayer;
			this.UpdateGroundWatch();
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00099C84 File Offset: 0x00097E84
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			LinePoint linePoint2 = Facepunch.Pool.Get<LinePoint>();
			linePoint2.localPosition = linePoint.LocalPosition;
			linePoint2.worldNormal = linePoint.WorldNormal;
			to.Add(linePoint2);
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00099CF8 File Offset: 0x00097EF8
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<Vector3> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			to.Add(linePoint.LocalPosition);
			to.Add(linePoint.WorldNormal);
		}
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00099D60 File Offset: 0x00097F60
	private void CopyPoints(List<LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (LinePoint linePoint in from)
		{
			to.Add(new AlignedLineDrawer.LinePoint
			{
				LocalPosition = linePoint.localPosition,
				WorldNormal = linePoint.worldNormal
			});
		}
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00099DD8 File Offset: 0x00097FD8
	public static void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			to.Add(linePoint);
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x00099E2C File Offset: 0x0009802C
	public override void ResetState()
	{
		base.ResetState();
		this.editingPlayer.Set(null);
	}

	// Token: 0x04000BF3 RID: 3059
	public AlignedLineDrawer LineDrawer;

	// Token: 0x04000BF4 RID: 3060
	public List<AlignedLineDrawer.LinePoint> LinePoints = new List<AlignedLineDrawer.LinePoint>();

	// Token: 0x04000BF5 RID: 3061
	private Color colour = Color.white;

	// Token: 0x04000BF6 RID: 3062
	private float width;

	// Token: 0x04000BF7 RID: 3063
	private EntityRef<global::BasePlayer> editingPlayer;

	// Token: 0x04000BF8 RID: 3064
	public GroundWatch groundWatch;

	// Token: 0x04000BF9 RID: 3065
	public MeshCollider meshCollider;

	// Token: 0x04000BFA RID: 3066
	public const int MaxLinePointLength = 60;

	// Token: 0x04000BFB RID: 3067
	public const float SimplifyTolerance = 0.008f;
}
