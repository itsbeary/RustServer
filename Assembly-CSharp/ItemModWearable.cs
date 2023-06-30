using System;
using Rust;
using UnityEngine;

// Token: 0x0200060B RID: 1547
public class ItemModWearable : ItemMod
{
	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06002DE8 RID: 11752 RVA: 0x00114707 File Offset: 0x00112907
	public Wearable targetWearable
	{
		get
		{
			if (this.entityPrefab.isValid)
			{
				return this.entityPrefab.Get().GetComponent<Wearable>();
			}
			return null;
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x00114728 File Offset: 0x00112928
	private void DoPrepare()
	{
		if (!this.entityPrefab.isValid)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab is null! " + base.gameObject, base.gameObject);
		}
		if (this.entityPrefab.isValid && this.targetWearable == null)
		{
			Debug.LogWarning("ItemModWearable: entityPrefab doesn't have a Wearable component! " + base.gameObject, this.entityPrefab.Get());
		}
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x00114798 File Offset: 0x00112998
	public override void ModInit()
	{
		if (string.IsNullOrEmpty(this.entityPrefab.resourcePath))
		{
			Debug.LogWarning(this + " - entityPrefab is null or something.. - " + this.entityPrefab.guid);
		}
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x001147C7 File Offset: 0x001129C7
	public bool ProtectsArea(HitArea area)
	{
		return !(this.armorProperties == null) && this.armorProperties.Contains(area);
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x001147E5 File Offset: 0x001129E5
	public bool HasProtections()
	{
		return this.protectionProperties != null;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x001147F3 File Offset: 0x001129F3
	internal float GetProtection(Item item, DamageType damageType)
	{
		if (this.protectionProperties == null)
		{
			return 0f;
		}
		return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x0011481D File Offset: 0x00112A1D
	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x00114832 File Offset: 0x00112A32
	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (this.protectionProperties == null)
		{
			return;
		}
		protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x00114858 File Offset: 0x00112A58
	private bool IsHeadgear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x0011488C File Offset: 0x00112A8C
	public bool IsFootwear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		return component != null && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != (Wearable.OccupationSlots)0;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x001148C4 File Offset: 0x00112AC4
	public override void OnAttacked(Item item, HitInfo info)
	{
		if (!item.hasCondition)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < 25; i++)
		{
			DamageType damageType = (DamageType)i;
			if (info.damageTypes.Has(damageType))
			{
				num += Mathf.Clamp(info.damageTypes.types[i] * this.GetProtection(item, damageType), 0f, item.condition);
				if (num >= item.condition)
				{
					break;
				}
			}
		}
		item.LoseCondition(num);
		if (item != null && item.isBroken && item.GetOwnerPlayer() && this.IsHeadgear() && info.damageTypes.Total() >= item.GetOwnerPlayer().health)
		{
			item.Drop(item.GetOwnerPlayer().transform.position + new Vector3(0f, 1.8f, 0f), item.GetOwnerPlayer().GetInheritedDropVelocity() + Vector3.up * 3f, default(Quaternion)).SetAngularVelocity(UnityEngine.Random.rotation.eulerAngles * 5f);
		}
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x001149F0 File Offset: 0x00112BF0
	public bool CanExistWith(ItemModWearable wearable)
	{
		if (wearable == null)
		{
			return true;
		}
		Wearable targetWearable = this.targetWearable;
		Wearable targetWearable2 = wearable.targetWearable;
		return (targetWearable.occupationOver & targetWearable2.occupationOver) == (Wearable.OccupationSlots)0 && (targetWearable.occupationUnder & targetWearable2.occupationUnder) == (Wearable.OccupationSlots)0;
	}

	// Token: 0x040025AB RID: 9643
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x040025AC RID: 9644
	public GameObjectRef entityPrefabFemale = new GameObjectRef();

	// Token: 0x040025AD RID: 9645
	public ProtectionProperties protectionProperties;

	// Token: 0x040025AE RID: 9646
	public ArmorProperties armorProperties;

	// Token: 0x040025AF RID: 9647
	public ClothingMovementProperties movementProperties;

	// Token: 0x040025B0 RID: 9648
	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x040025B1 RID: 9649
	public bool blocksAiming;

	// Token: 0x040025B2 RID: 9650
	public bool emissive;

	// Token: 0x040025B3 RID: 9651
	public float accuracyBonus;

	// Token: 0x040025B4 RID: 9652
	public bool blocksEquipping;

	// Token: 0x040025B5 RID: 9653
	public float eggVision;

	// Token: 0x040025B6 RID: 9654
	public float weight;

	// Token: 0x040025B7 RID: 9655
	public bool equipOnRightClick = true;

	// Token: 0x040025B8 RID: 9656
	public bool npcOnly;

	// Token: 0x040025B9 RID: 9657
	public GameObjectRef breakEffect = new GameObjectRef();

	// Token: 0x040025BA RID: 9658
	public GameObjectRef viewmodelAddition;
}
