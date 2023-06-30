using System;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class OreResourceEntity : StagedResourceEntity
{
	// Token: 0x06000F66 RID: 3942 RVA: 0x000817DC File Offset: 0x0007F9DC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("OreResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0008181C File Offset: 0x0007FA1C
	protected override void UpdateNetworkStage()
	{
		int stage = this.stage;
		base.UpdateNetworkStage();
		if (this.stage != stage && this._hotSpot)
		{
			this.DelayedBonusSpawn();
		}
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x00081852 File Offset: 0x0007FA52
	public void CleanupBonus()
	{
		if (this._hotSpot)
		{
			this._hotSpot.Kill(BaseNetworkable.DestroyMode.None);
		}
		this._hotSpot = null;
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x00081874 File Offset: 0x0007FA74
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.CleanupBonus();
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x00081882 File Offset: 0x0007FA82
	public override void OnKilled(HitInfo info)
	{
		this.CleanupBonus();
		Analytics.Server.OreKilled(this, info);
		base.OnKilled(info);
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x00081898 File Offset: 0x0007FA98
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitialSpawnBonusSpot), 0f);
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x000818B7 File Offset: 0x0007FAB7
	private void InitialSpawnBonusSpot()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this._hotSpot = this.SpawnBonusSpot(Vector3.zero);
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x000818D3 File Offset: 0x0007FAD3
	public void FinishBonusAssigned()
	{
		Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.up, null, false);
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x00081900 File Offset: 0x0007FB00
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnAttacked(info);
			return;
		}
		if (!info.DidGather && info.gatherScale > 0f)
		{
			Jackhammer jackhammer = info.Weapon as Jackhammer;
			if (this._hotSpot || jackhammer)
			{
				if (this._hotSpot == null)
				{
					this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
				}
				if (Vector3.Distance(info.HitPositionWorld, this._hotSpot.transform.position) <= this._hotSpot.GetComponent<SphereCollider>().radius * 1.5f || jackhammer != null)
				{
					float num = ((jackhammer == null) ? 1f : jackhammer.HotspotBonusScale);
					this.bonusesKilled++;
					info.gatherScale = 1f + Mathf.Clamp((float)this.bonusesKilled * 0.5f, 0f, 2f * num);
					this._hotSpot.FireFinishEffect();
					base.ClientRPC<int, Vector3>(null, "PlayBonusLevelSound", this.bonusesKilled, this._hotSpot.transform.position);
				}
				else if (this.bonusesKilled > 0)
				{
					this.bonusesKilled = 0;
					Effect.server.Run(this.bonusFailEffect.resourcePath, base.transform.position, base.transform.up, null, false);
				}
				if (this.bonusesKilled > 0)
				{
					this.CleanupBonus();
				}
			}
		}
		if (this._hotSpot == null)
		{
			this.DelayedBonusSpawn();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x00081A98 File Offset: 0x0007FC98
	public void DelayedBonusSpawn()
	{
		base.CancelInvoke(new Action(this.RespawnBonus));
		base.Invoke(new Action(this.RespawnBonus), 0.25f);
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x00081AC3 File Offset: 0x0007FCC3
	public void RespawnBonus()
	{
		this.CleanupBonus();
		this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x00081AE0 File Offset: 0x0007FCE0
	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		if (base.isClient)
		{
			return null;
		}
		if (!this.bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 vector = Vector3.zero;
		MeshCollider stageComponent = base.GetStageComponent<MeshCollider>();
		Vector3 vector2 = base.transform.InverseTransformPoint(stageComponent.bounds.center);
		if (lastDirection == Vector3.zero)
		{
			Vector3 vector3 = this.RandomCircle(1f, false);
			this.lastNodeDir = vector3.normalized;
			Vector3 vector4 = base.transform.TransformDirection(vector3.normalized);
			vector3 = base.transform.position + base.transform.up * (vector2.y + 0.5f) + vector4.normalized * 2.5f;
			vector = vector3;
		}
		else
		{
			Vector3 vector5 = Vector3.Cross(this.lastNodeDir, Vector3.up);
			float num = UnityEngine.Random.Range(0.25f, 0.5f);
			float num2 = ((UnityEngine.Random.Range(0, 2) == 0) ? (-1f) : 1f);
			Vector3 normalized2 = (this.lastNodeDir + vector5 * num * num2).normalized;
			this.lastNodeDir = normalized2;
			vector = base.transform.position + base.transform.TransformDirection(normalized2) * 2f;
			float num3 = UnityEngine.Random.Range(1f, 1.5f);
			vector += base.transform.up * (vector2.y + num3);
		}
		this.bonusesSpawned++;
		Vector3 normalized3 = (stageComponent.bounds.center - vector).normalized;
		RaycastHit raycastHit;
		if (stageComponent.Raycast(new Ray(vector, normalized3), out raycastHit, 10f))
		{
			OreHotSpot oreHotSpot = GameManager.server.CreateEntity(this.bonusPrefab.resourcePath, raycastHit.point - normalized3 * 0.025f, Quaternion.LookRotation(raycastHit.normal, Vector3.up), true) as OreHotSpot;
			oreHotSpot.Spawn();
			oreHotSpot.SendMessage("OreOwner", this);
			return oreHotSpot;
		}
		return null;
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x00081D20 File Offset: 0x0007FF20
	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		Vector2 vector = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		return new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x00081D5C File Offset: 0x0007FF5C
	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		Vector3 vector2 = new Vector3(vector.x * degreesOffset, changeHeight ? (UnityEngine.Random.Range(-1f, 1f) * degreesOffset) : 0f, vector.y * degreesOffset);
		return (input + vector2).normalized;
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x00081DDC File Offset: 0x0007FFDC
	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 normalized = (hemiInput + Vector3.one * degreesOffset).normalized;
		Vector3 normalized2 = (hemiInput + Vector3.one * -degreesOffset).normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], normalized2[i], normalized[i]);
		}
		return inputVec;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x00081E68 File Offset: 0x00080068
	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		Vector2 vector = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.y).normalized * distance;
		vector2.y = UnityEngine.Random.Range(minHeight, maxHeight);
		return vector2;
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0002C05D File Offset: 0x0002A25D
	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		return Vector3.zero;
	}

	// Token: 0x04000A23 RID: 2595
	public GameObjectRef bonusPrefab;

	// Token: 0x04000A24 RID: 2596
	public GameObjectRef finishEffect;

	// Token: 0x04000A25 RID: 2597
	public GameObjectRef bonusFailEffect;

	// Token: 0x04000A26 RID: 2598
	public OreHotSpot _hotSpot;

	// Token: 0x04000A27 RID: 2599
	public SoundPlayer bonusSound;

	// Token: 0x04000A28 RID: 2600
	private int bonusesKilled;

	// Token: 0x04000A29 RID: 2601
	private int bonusesSpawned;

	// Token: 0x04000A2A RID: 2602
	private Vector3 lastNodeDir = Vector3.zero;
}
