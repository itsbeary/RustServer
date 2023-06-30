using System;
using Rust;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class LargeShredder : BaseEntity
{
	// Token: 0x060024A6 RID: 9382 RVA: 0x000E8C80 File Offset: 0x000E6E80
	public virtual void OnEntityEnteredTrigger(BaseEntity ent)
	{
		if (ent.IsDestroyed)
		{
			return;
		}
		Rigidbody component = ent.GetComponent<Rigidbody>();
		if (this.isShredding || this.currentlyShredding != null)
		{
			component.velocity = -component.velocity * 3f;
			return;
		}
		this.shredRail.transform.position = this.shredRailStartPos.position;
		this.shredRail.transform.rotation = Quaternion.LookRotation(this.shredRailStartRotation);
		this.entryRotation = ent.transform.rotation;
		Quaternion rotation = ent.transform.rotation;
		component.isKinematic = true;
		this.currentlyShredding = ent;
		ent.transform.rotation = rotation;
		this.isShredding = true;
		this.SetShredding(true);
		this.shredStartTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000E8D58 File Offset: 0x000E6F58
	public void CreateShredResources()
	{
		if (this.currentlyShredding == null)
		{
			return;
		}
		MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
		if (component == null)
		{
			return;
		}
		if (component.associatedPlayer != null && GameInfo.HasAchievements)
		{
			component.associatedPlayer.stats.Add("cars_shredded", 1, Stats.Steam);
			component.associatedPlayer.stats.Save(true);
		}
		foreach (ItemAmount itemAmount in component.shredResources)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			float num = 0.5f;
			if (item.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num, num), 1f, UnityEngine.Random.Range(-num, num)), default(Quaternion), null, 0U) == null)
			{
				item.Remove(0f);
			}
		}
		BaseModularVehicle component2 = this.currentlyShredding.GetComponent<BaseModularVehicle>();
		if (component2)
		{
			foreach (BaseVehicleModule baseVehicleModule in component2.AttachedModuleEntities)
			{
				if (baseVehicleModule.AssociatedItemDef && baseVehicleModule.AssociatedItemDef.Blueprint)
				{
					foreach (ItemAmount itemAmount2 in baseVehicleModule.AssociatedItemDef.Blueprint.ingredients)
					{
						int num2 = Mathf.FloorToInt(itemAmount2.amount * 0.5f);
						if (num2 != 0)
						{
							Item item2 = ItemManager.Create(itemAmount2.itemDef, num2, 0UL);
							float num3 = 0.5f;
							if (item2.CreateWorldObject(this.resourceSpawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-num3, num3), 1f, UnityEngine.Random.Range(-num3, num3)), default(Quaternion), null, 0U) == null)
							{
								item2.Remove(0f);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x000E8FD8 File Offset: 0x000E71D8
	public void UpdateBonePosition(float delta)
	{
		float num = delta / this.shredDurationPosition;
		float num2 = delta / this.shredDurationRotation;
		this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, num);
		this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, num2));
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000E904A File Offset: 0x000E724A
	public void SetShredding(bool isShredding)
	{
		if (isShredding)
		{
			base.InvokeRandomized(new Action(this.FireShredEffect), 0.25f, 0.75f, 0.25f);
			return;
		}
		base.CancelInvoke(new Action(this.FireShredEffect));
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000E9083 File Offset: 0x000E7283
	public void FireShredEffect()
	{
		Effect.server.Run(this.shredSoundEffect.resourcePath, base.transform.position + Vector3.up * 3f, Vector3.up, null, false);
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000E90BC File Offset: 0x000E72BC
	public void ServerUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved10, this.isShredding, false, true);
		if (this.isShredding)
		{
			float num = Time.realtimeSinceStartup - this.shredStartTime;
			float num2 = num / this.shredDurationPosition;
			float num3 = num / this.shredDurationRotation;
			this.shredRail.transform.localPosition = Vector3.Lerp(this.shredRailStartPos.localPosition, this.shredRailEndPos.localPosition, num2);
			this.shredRail.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(this.shredRailStartRotation, this.shredRailEndRotation, num3));
			MagnetLiftable component = this.currentlyShredding.GetComponent<MagnetLiftable>();
			this.currentlyShredding.transform.position = this.shredRail.transform.position;
			Vector3 vector = base.transform.TransformDirection(component.shredDirection);
			if (Vector3.Dot(-vector, this.currentlyShredding.transform.forward) > Vector3.Dot(vector, this.currentlyShredding.transform.forward))
			{
				vector = base.transform.TransformDirection(-component.shredDirection);
			}
			bool flag = Vector3.Dot(component.transform.up, Vector3.up) >= -0.95f;
			Quaternion quaternion = QuaternionEx.LookRotationForcedUp(vector, flag ? (-base.transform.right) : base.transform.right);
			float num4 = Time.time * this.shredSwaySpeed;
			float num5 = Mathf.PerlinNoise(num4, 0f);
			float num6 = Mathf.PerlinNoise(0f, num4 + 150f);
			quaternion *= Quaternion.Euler(num5 * this.shredSwayAmount, 0f, num6 * this.shredSwayAmount);
			this.currentlyShredding.transform.rotation = Quaternion.Lerp(this.entryRotation, quaternion, num3);
			if (num > 5f)
			{
				this.CreateShredResources();
				this.currentlyShredding.Kill(BaseNetworkable.DestroyMode.None);
				this.currentlyShredding = null;
				this.isShredding = false;
				this.SetShredding(false);
			}
		}
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000E92D4 File Offset: 0x000E74D4
	private void Update()
	{
		this.ServerUpdate();
	}

	// Token: 0x04001C88 RID: 7304
	public Transform shredRail;

	// Token: 0x04001C89 RID: 7305
	public Transform shredRailStartPos;

	// Token: 0x04001C8A RID: 7306
	public Transform shredRailEndPos;

	// Token: 0x04001C8B RID: 7307
	public Vector3 shredRailStartRotation;

	// Token: 0x04001C8C RID: 7308
	public Vector3 shredRailEndRotation;

	// Token: 0x04001C8D RID: 7309
	public LargeShredderTrigger trigger;

	// Token: 0x04001C8E RID: 7310
	public float shredDurationRotation = 2f;

	// Token: 0x04001C8F RID: 7311
	public float shredDurationPosition = 5f;

	// Token: 0x04001C90 RID: 7312
	public float shredSwayAmount = 1f;

	// Token: 0x04001C91 RID: 7313
	public float shredSwaySpeed = 3f;

	// Token: 0x04001C92 RID: 7314
	public BaseEntity currentlyShredding;

	// Token: 0x04001C93 RID: 7315
	public GameObject[] shreddingWheels;

	// Token: 0x04001C94 RID: 7316
	public float shredRotorSpeed = 1f;

	// Token: 0x04001C95 RID: 7317
	public GameObjectRef shredSoundEffect;

	// Token: 0x04001C96 RID: 7318
	public Transform resourceSpawnPoint;

	// Token: 0x04001C97 RID: 7319
	private Quaternion entryRotation;

	// Token: 0x04001C98 RID: 7320
	public const string SHRED_STAT = "cars_shredded";

	// Token: 0x04001C99 RID: 7321
	private bool isShredding;

	// Token: 0x04001C9A RID: 7322
	private float shredStartTime;
}
