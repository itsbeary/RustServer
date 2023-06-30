using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F6 RID: 246
public class ZiplineLaunchPoint : global::BaseEntity
{
	// Token: 0x06001579 RID: 5497 RVA: 0x000A9634 File Offset: 0x000A7834
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ZiplineLaunchPoint.OnRpcMessage", 0))
		{
			if (rpc == 2256922575U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MountPlayer ");
				}
				using (TimeWarning.New("MountPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2256922575U, "MountPlayer", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2256922575U, "MountPlayer", this, player, 3f))
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
							this.MountPlayer(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in MountPlayer");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000A97F4 File Offset: 0x000A79F4
	public override void ResetState()
	{
		base.ResetState();
		this.ziplineTargets.Clear();
		this.linePoints = null;
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000A9810 File Offset: 0x000A7A10
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.FindZiplineTarget(ref this.ziplineTargets);
		this.CalculateZiplinePoints(this.ziplineTargets, ref this.linePoints);
		if (this.ziplineTargets.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		if (Vector3.Distance(this.linePoints[0], this.linePoints[this.linePoints.Count - 1]) > 100f && this.ArrivalPointRef != null && this.ArrivalPointRef.isValid)
		{
			global::ZiplineArrivalPoint ziplineArrivalPoint = base.gameManager.CreateEntity(this.ArrivalPointRef.resourcePath, this.linePoints[this.linePoints.Count - 1], default(Quaternion), true) as global::ZiplineArrivalPoint;
			ziplineArrivalPoint.SetPositions(this.linePoints);
			ziplineArrivalPoint.Spawn();
		}
		this.UpdateBuildingBlocks();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x000A98F8 File Offset: 0x000A7AF8
	private void FindZiplineTarget(ref List<Vector3> foundPositions)
	{
		foundPositions.Clear();
		Vector3 position = this.LineDeparturePoint.position;
		List<ZiplineTarget> list = Facepunch.Pool.GetList<ZiplineTarget>();
		GamePhysics.OverlapSphere<ZiplineTarget>(position + base.transform.forward * 200f, 200f, list, 1218511105, QueryTriggerInteraction.Ignore);
		float num = float.MaxValue;
		float num2 = 3f;
		foreach (ZiplineTarget ziplineTarget in list)
		{
			if (!ziplineTarget.IsChainPoint)
			{
				Vector3 position2 = ziplineTarget.transform.position;
				float num3 = Vector3.Dot((position2.WithY(position.y) - position).normalized, base.transform.forward);
				float num4 = Vector3.Distance(position, position2);
				if (num3 > 0.2f && ziplineTarget.IsValidPosition(position) && position.y + num2 > position2.y && num4 > 10f && num4 < num)
				{
					if (this.CheckLineOfSight(position, position2))
					{
						num = num4;
						ZiplineTarget ziplineTarget2 = ziplineTarget;
						foundPositions.Clear();
						foundPositions.Add(ziplineTarget2.transform.position);
					}
					else
					{
						foreach (ZiplineTarget ziplineTarget3 in list)
						{
							if (ziplineTarget3.IsChainPoint && ziplineTarget3.IsValidChainPoint(position, position2))
							{
								bool flag = this.CheckLineOfSight(position, ziplineTarget3.transform.position);
								bool flag2 = this.CheckLineOfSight(ziplineTarget3.transform.position, position2);
								if (flag && flag2)
								{
									num = num4;
									ZiplineTarget ziplineTarget2 = ziplineTarget;
									foundPositions.Clear();
									foundPositions.Add(ziplineTarget3.transform.position);
									foundPositions.Add(ziplineTarget2.transform.position);
								}
								else if (flag)
								{
									foreach (ZiplineTarget ziplineTarget4 in list)
									{
										if (!(ziplineTarget4 == ziplineTarget3) && ziplineTarget4.IsValidChainPoint(ziplineTarget3.Target.position, ziplineTarget.Target.position))
										{
											bool flag3 = this.CheckLineOfSight(ziplineTarget3.transform.position, ziplineTarget4.transform.position);
											bool flag4 = this.CheckLineOfSight(ziplineTarget4.transform.position, ziplineTarget.transform.position);
											if (flag3 && flag4)
											{
												num = num4;
												ZiplineTarget ziplineTarget2 = ziplineTarget;
												foundPositions.Clear();
												foundPositions.Add(ziplineTarget3.transform.position);
												foundPositions.Add(ziplineTarget4.transform.position);
												foundPositions.Add(ziplineTarget2.transform.position);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000A9C40 File Offset: 0x000A7E40
	private bool CheckLineOfSight(Vector3 from, Vector3 to)
	{
		Vector3 vector = this.CalculateLineMidPoint(from, to) - Vector3.up * 0.75f;
		return GamePhysics.LineOfSightRadius(from, to, 1218511105, 0.5f, 2f, null) && GamePhysics.LineOfSightRadius(from, vector, 1218511105, 0.5f, 2f, null) && GamePhysics.LineOfSightRadius(vector, to, 1218511105, 0.5f, 2f, null);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000A9CB8 File Offset: 0x000A7EB8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void MountPlayer(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (msg.player.Distance(this.LineDeparturePoint.position) > 3f)
		{
			return;
		}
		if (!this.IsPlayerFacingValidDirection(msg.player))
		{
			return;
		}
		if (this.ziplineTargets.Count == 0)
		{
			return;
		}
		Vector3 position = this.LineDeparturePoint.position;
		Quaternion quaternion = Quaternion.LookRotation((this.ziplineTargets[0].WithY(position.y) - position).normalized);
		Quaternion quaternion2 = Quaternion.LookRotation((position - msg.player.transform.position.WithY(position.y)).normalized);
		global::ZiplineMountable ziplineMountable = base.gameManager.CreateEntity(this.MountableRef.resourcePath, msg.player.transform.position + Vector3.up * 2.1f, quaternion2, true) as global::ZiplineMountable;
		if (ziplineMountable != null)
		{
			this.CalculateZiplinePoints(this.ziplineTargets, ref this.linePoints);
			ziplineMountable.SetDestination(this.linePoints, position, quaternion);
			ziplineMountable.Spawn();
			ziplineMountable.MountPlayer(msg.player);
			if (msg.player.GetMounted() != ziplineMountable)
			{
				ziplineMountable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			base.Invoke(new Action(this.ClearBusy), 2f);
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x00062BCC File Offset: 0x00060DCC
	private void ClearBusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x000A9E3C File Offset: 0x000A803C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.zipline == null)
		{
			info.msg.zipline = Facepunch.Pool.Get<Zipline>();
		}
		info.msg.zipline.destinationPoints = Facepunch.Pool.GetList<VectorData>();
		foreach (Vector3 vector in this.ziplineTargets)
		{
			info.msg.zipline.destinationPoints.Add(new VectorData(vector.x, vector.y, vector.z));
		}
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x000A9EF0 File Offset: 0x000A80F0
	[ServerVar(ServerAdmin = true)]
	public static void report(ConsoleSystem.Arg arg)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			ZiplineLaunchPoint ziplineLaunchPoint;
			if ((ziplineLaunchPoint = baseNetworkable as ZiplineLaunchPoint) != null)
			{
				float lineLength = ziplineLaunchPoint.GetLineLength();
				num2++;
				num += lineLength;
			}
			else if (baseNetworkable is global::ZiplineArrivalPoint)
			{
				num3++;
			}
		}
		arg.ReplyWith(string.Format("{0} ziplines, total distance: {1:F2}, avg length: {2:F2}, arrival points: {3}", new object[]
		{
			num2,
			num,
			num / (float)num2,
			num3
		}));
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x000A9FAC File Offset: 0x000A81AC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.zipline != null)
		{
			this.ziplineTargets.Clear();
			foreach (VectorData vectorData in info.msg.zipline.destinationPoints)
			{
				this.ziplineTargets.Add(vectorData);
			}
		}
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000AA034 File Offset: 0x000A8234
	private void CalculateZiplinePoints(List<Vector3> targets, ref List<Vector3> points)
	{
		if (points != null || targets.Count == 0)
		{
			return;
		}
		Vector3[] array = new Vector3[targets.Count + 1];
		array[0] = this.LineDeparturePoint.position;
		for (int i = 0; i < targets.Count; i++)
		{
			array[i + 1] = targets[i];
		}
		float[] array2 = new float[array.Length];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = this.LineSlackAmount;
		}
		points = Facepunch.Pool.GetList<Vector3>();
		Bezier.ApplyLineSlack(array, array2, ref points, 25);
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000AA0C0 File Offset: 0x000A82C0
	private Vector3 CalculateLineMidPoint(Vector3 start, Vector3 endPoint)
	{
		Vector3 vector = Vector3.Lerp(start, endPoint, 0.5f);
		vector.y -= this.LineSlackAmount;
		return vector;
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000AA0EC File Offset: 0x000A82EC
	private void UpdateBuildingBlocks()
	{
		BoxCollider[] array = this.BuildingBlocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		array = this.PointBuildingBlocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		SpawnableBoundsBlocker[] spawnableBoundsBlockers = this.SpawnableBoundsBlockers;
		for (int i = 0; i < spawnableBoundsBlockers.Length; i++)
		{
			spawnableBoundsBlockers[i].gameObject.SetActive(false);
		}
		int num = 0;
		if (this.ziplineTargets.Count > 0)
		{
			Vector3 vector = Vector3.zero;
			int num2 = 0;
			for (int j = 0; j < this.linePoints.Count; j++)
			{
				if (j != 0 && (!base.isClient || j != 1))
				{
					Vector3 vector2 = this.linePoints[j];
					Vector3 normalized = (vector2 - this.linePoints[j - 1].WithY(vector2.y)).normalized;
					if (vector != Vector3.zero && Vector3.Dot(normalized, vector) < 0.98f)
					{
						if (num < this.BuildingBlocks.Length)
						{
							this.<UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(this.BuildingBlocks[num], this.PointBuildingBlocks[num], this.SpawnableBoundsBlockers[num++], num2, j - 1);
						}
						num2 = j - 1;
					}
					vector = normalized;
				}
			}
			if (num < this.BuildingBlocks.Length)
			{
				this.<UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(this.BuildingBlocks[num], this.PointBuildingBlocks[num], this.SpawnableBoundsBlockers[num], num2, this.linePoints.Count - 1);
			}
		}
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x000AA285 File Offset: 0x000A8485
	private bool IsPlayerFacingValidDirection(global::BasePlayer ply)
	{
		return Vector3.Dot(ply.eyes.HeadForward(), base.transform.forward) > 0.2f;
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x000AA2AC File Offset: 0x000A84AC
	public float GetLineLength()
	{
		if (this.linePoints == null)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < this.linePoints.Count - 1; i++)
		{
			num += Vector3.Distance(this.linePoints[i], this.linePoints[i + 1]);
		}
		return num;
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x000AA328 File Offset: 0x000A8528
	[CompilerGenerated]
	private void <UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(BoxCollider longCollider, BoxCollider pointCollider, SpawnableBoundsBlocker spawnBlocker, int startIndex, int endIndex)
	{
		Vector3 vector = this.linePoints[startIndex];
		Vector3 vector2 = this.linePoints[endIndex];
		Vector3 vector3 = Vector3.zero;
		Quaternion quaternion = Quaternion.LookRotation((vector - vector2).normalized, Vector3.up);
		Vector3 vector4 = Vector3.Lerp(vector, vector2, 0.5f);
		longCollider.transform.position = vector4;
		longCollider.transform.rotation = quaternion;
		for (int i = startIndex; i < endIndex; i++)
		{
			Vector3 vector5 = longCollider.transform.InverseTransformPoint(this.linePoints[i]);
			if (vector5.y < vector3.y)
			{
				vector3 = vector5;
			}
		}
		float num = Mathf.Abs(vector3.y) + 2f;
		float num2 = Vector3.Distance(vector, vector2);
		Vector3 vector6 = (spawnBlocker.BoxCollider.size = new Vector3(0.5f, num, num2) + Vector3.one);
		longCollider.size = vector6;
		BoxCollider boxCollider = spawnBlocker.BoxCollider;
		vector6 = new Vector3(0f, -(num * 0.5f), 0f);
		boxCollider.center = vector6;
		longCollider.center = vector6;
		longCollider.gameObject.SetActive(true);
		pointCollider.transform.position = this.linePoints[endIndex];
		pointCollider.gameObject.SetActive(true);
		spawnBlocker.gameObject.SetActive(true);
		if (base.isServer)
		{
			spawnBlocker.ClearTrees();
		}
	}

	// Token: 0x04000D97 RID: 3479
	public Transform LineDeparturePoint;

	// Token: 0x04000D98 RID: 3480
	public LineRenderer ZiplineRenderer;

	// Token: 0x04000D99 RID: 3481
	public Collider MountCollider;

	// Token: 0x04000D9A RID: 3482
	public BoxCollider[] BuildingBlocks;

	// Token: 0x04000D9B RID: 3483
	public BoxCollider[] PointBuildingBlocks;

	// Token: 0x04000D9C RID: 3484
	public SpawnableBoundsBlocker[] SpawnableBoundsBlockers;

	// Token: 0x04000D9D RID: 3485
	public GameObjectRef MountableRef;

	// Token: 0x04000D9E RID: 3486
	public float LineSlackAmount = 2f;

	// Token: 0x04000D9F RID: 3487
	public bool RegenLine;

	// Token: 0x04000DA0 RID: 3488
	private List<Vector3> ziplineTargets = new List<Vector3>();

	// Token: 0x04000DA1 RID: 3489
	private List<Vector3> linePoints;

	// Token: 0x04000DA2 RID: 3490
	public GameObjectRef ArrivalPointRef;
}
