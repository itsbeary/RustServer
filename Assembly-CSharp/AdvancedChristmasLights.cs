using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000032 RID: 50
public class AdvancedChristmasLights : global::IOEntity
{
	// Token: 0x0600013D RID: 317 RVA: 0x000217C4 File Offset: 0x0001F9C4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AdvancedChristmasLights.OnRpcMessage", 0))
		{
			if (rpc == 1435781224U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationStyle ");
				}
				using (TimeWarning.New("SetAnimationStyle", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1435781224U, "SetAnimationStyle", this, player, 3f))
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
							this.SetAnimationStyle(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetAnimationStyle");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0002192C File Offset: 0x0001FB2C
	public void ClearPoints()
	{
		this.points.Clear();
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00021939 File Offset: 0x0001FB39
	public void FinishEditing()
	{
		this.finalized = true;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00021942 File Offset: 0x0001FB42
	public bool IsFinalized()
	{
		return this.finalized;
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0002194C File Offset: 0x0001FB4C
	public void AddPoint(Vector3 newPoint, Vector3 newNormal)
	{
		if (base.isServer && this.points.Count == 0)
		{
			newPoint = this.wireEmission.position;
		}
		AdvancedChristmasLights.pointEntry pointEntry = default(AdvancedChristmasLights.pointEntry);
		pointEntry.point = newPoint;
		pointEntry.normal = newNormal;
		this.points.Add(pointEntry);
		if (base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000142 RID: 322 RVA: 0x000219AE File Offset: 0x0001FBAE
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000219B1 File Offset: 0x0001FBB1
	protected override int GetPickupCount()
	{
		return Mathf.Max(this.lengthUsed, 1);
	}

	// Token: 0x06000144 RID: 324 RVA: 0x000219BF File Offset: 0x0001FBBF
	public void AddLengthUsed(int addLength)
	{
		this.lengthUsed += addLength;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x000219CF File Offset: 0x0001FBCF
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000146 RID: 326 RVA: 0x000219D8 File Offset: 0x0001FBD8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lightString = Facepunch.Pool.Get<LightString>();
		info.msg.lightString.points = Facepunch.Pool.GetList<LightString.StringPoint>();
		info.msg.lightString.lengthUsed = this.lengthUsed;
		info.msg.lightString.animationStyle = (int)this.animationStyle;
		foreach (AdvancedChristmasLights.pointEntry pointEntry in this.points)
		{
			LightString.StringPoint stringPoint = Facepunch.Pool.Get<LightString.StringPoint>();
			stringPoint.point = pointEntry.point;
			stringPoint.normal = pointEntry.normal;
			info.msg.lightString.points.Add(stringPoint);
		}
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00021AB0 File Offset: 0x0001FCB0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lightString != null)
		{
			this.ClearPoints();
			foreach (LightString.StringPoint stringPoint in info.msg.lightString.points)
			{
				this.AddPoint(stringPoint.point, stringPoint.normal);
			}
			this.lengthUsed = info.msg.lightString.lengthUsed;
			this.animationStyle = (AdvancedChristmasLights.AnimationType)info.msg.lightString.animationStyle;
			if (info.fromDisk)
			{
				this.FinishEditing();
			}
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00021B70 File Offset: 0x0001FD70
	public bool IsStyle(AdvancedChristmasLights.AnimationType testType)
	{
		return testType == this.animationStyle;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanPlayerManipulate(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00021B7C File Offset: 0x0001FD7C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetAnimationStyle(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		num = Mathf.Clamp(num, 1, 7);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Set animation style to :",
				num,
				" old was : ",
				(int)this.animationStyle
			}));
		}
		AdvancedChristmasLights.AnimationType animationType = (AdvancedChristmasLights.AnimationType)num;
		if (animationType == this.animationStyle)
		{
			return;
		}
		this.animationStyle = animationType;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04000188 RID: 392
	public GameObjectRef bulbPrefab;

	// Token: 0x04000189 RID: 393
	public LineRenderer lineRenderer;

	// Token: 0x0400018A RID: 394
	public List<AdvancedChristmasLights.pointEntry> points = new List<AdvancedChristmasLights.pointEntry>();

	// Token: 0x0400018B RID: 395
	public List<BaseBulb> bulbs = new List<BaseBulb>();

	// Token: 0x0400018C RID: 396
	public float bulbSpacing = 0.25f;

	// Token: 0x0400018D RID: 397
	public float wireThickness = 0.02f;

	// Token: 0x0400018E RID: 398
	public Transform wireEmission;

	// Token: 0x0400018F RID: 399
	public AdvancedChristmasLights.AnimationType animationStyle = AdvancedChristmasLights.AnimationType.ON;

	// Token: 0x04000190 RID: 400
	public RendererLOD _lod;

	// Token: 0x04000191 RID: 401
	[Tooltip("This many units used will result in +1 power usage")]
	public float lengthToPowerRatio = 5f;

	// Token: 0x04000192 RID: 402
	private bool finalized;

	// Token: 0x04000193 RID: 403
	private int lengthUsed;

	// Token: 0x02000B5E RID: 2910
	public struct pointEntry
	{
		// Token: 0x04003F57 RID: 16215
		public Vector3 point;

		// Token: 0x04003F58 RID: 16216
		public Vector3 normal;
	}

	// Token: 0x02000B5F RID: 2911
	public enum AnimationType
	{
		// Token: 0x04003F5A RID: 16218
		ON = 1,
		// Token: 0x04003F5B RID: 16219
		FLASHING,
		// Token: 0x04003F5C RID: 16220
		CHASING,
		// Token: 0x04003F5D RID: 16221
		FADE,
		// Token: 0x04003F5E RID: 16222
		SLOWGLOW = 6
	}
}
