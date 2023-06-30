using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x060021EF RID: 8687 RVA: 0x000DC93D File Offset: 0x000DAB3D
	public void Start()
	{
		this.Initialize();
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000DC945 File Offset: 0x000DAB45
	public void Initialize()
	{
		this.CacheResourceTypeItems();
		this.UpdateFraction();
		this.UpdateRemainingCategories();
		this.CountAllItems();
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000DC960 File Offset: 0x000DAB60
	private void CacheResourceTypeItems()
	{
		if (ResourceDispenser.cachedResourceItemTypes == null)
		{
			ResourceDispenser.cachedResourceItemTypes = new Dictionary<ResourceDispenser.GatherType, HashSet<int>>();
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(ItemManager.FindItemDefinition("wood").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Tree, hashSet);
			HashSet<int> hashSet2 = new HashSet<int>();
			hashSet2.Add(ItemManager.FindItemDefinition("stones").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("sulfur.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("metal.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("hq.metal.ore").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Ore, hashSet2);
		}
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x000DCA14 File Offset: 0x000DAC14
	public void DoGather(HitInfo info)
	{
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (!info.CanGather || info.DidGather)
		{
			return;
		}
		if (this.gatherType == ResourceDispenser.GatherType.UNSET)
		{
			Debug.LogWarning("Object :" + base.gameObject.name + ": has unset gathertype!");
			return;
		}
		BaseMelee baseMelee = ((info.Weapon == null) ? null : (info.Weapon as BaseMelee));
		float num;
		float num2;
		if (baseMelee != null)
		{
			ResourceDispenser.GatherPropertyEntry gatherInfoFromIndex = baseMelee.GetGatherInfoFromIndex(this.gatherType);
			num = gatherInfoFromIndex.gatherDamage * info.gatherScale;
			num2 = gatherInfoFromIndex.destroyFraction;
			if (num == 0f)
			{
				return;
			}
			baseMelee.SendPunch(new Vector3(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-0.25f, -0.5f), 0f) * -30f * (gatherInfoFromIndex.conditionLost / 6f), 0.05f);
			baseMelee.LoseCondition(gatherInfoFromIndex.conditionLost);
			if (!baseMelee.IsValid() || baseMelee.IsBroken())
			{
				return;
			}
			info.DidGather = true;
		}
		else
		{
			num = info.damageTypes.Total();
			num2 = 0.5f;
		}
		float num3 = this.fractionRemaining;
		this.GiveResources(info.InitiatorPlayer, num, num2, info.Weapon);
		this.UpdateFraction();
		float num4;
		if (this.fractionRemaining <= 0f)
		{
			num4 = base.baseEntity.MaxHealth();
			if (info.DidGather && num2 < this.maxDestroyFractionForFinishBonus)
			{
				this.AssignFinishBonus(info.InitiatorPlayer, 1f - num2, info.Weapon);
			}
		}
		else
		{
			num4 = (num3 - this.fractionRemaining) * base.baseEntity.MaxHealth();
		}
		HitInfo hitInfo = new HitInfo(info.Initiator, base.baseEntity, DamageType.Generic, num4, base.transform.position);
		hitInfo.gatherScale = 0f;
		hitInfo.PointStart = info.PointStart;
		hitInfo.PointEnd = info.PointEnd;
		hitInfo.WeaponPrefab = info.WeaponPrefab;
		hitInfo.Weapon = info.Weapon;
		base.baseEntity.OnAttacked(hitInfo);
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000DCC48 File Offset: 0x000DAE48
	public void AssignFinishBonus(BasePlayer player, float fraction, AttackEntity weapon)
	{
		base.SendMessage("FinishBonusAssigned", SendMessageOptions.DontRequireReceiver);
		if (fraction <= 0f)
		{
			return;
		}
		if (this.finishBonus != null)
		{
			foreach (ItemAmount itemAmount in this.finishBonus)
			{
				int num = Mathf.CeilToInt((float)((int)itemAmount.amount) * Mathf.Clamp01(fraction));
				int num2 = this.CalculateGatherBonus(player, itemAmount, (float)num);
				Item item = ItemManager.Create(itemAmount.itemDef, num + num2, 0UL);
				if (item != null)
				{
					Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, base.baseEntity, player, weapon);
					player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
			}
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000DCD14 File Offset: 0x000DAF14
	public void OnAttacked(HitInfo info)
	{
		this.DoGather(info);
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000DCD20 File Offset: 0x000DAF20
	private void GiveResources(BasePlayer entity, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (!entity.IsValid())
		{
			return;
		}
		if (gatherDamage <= 0f)
		{
			return;
		}
		ItemAmount itemAmount = null;
		int i = this.containedItems.Count;
		int num = UnityEngine.Random.Range(0, this.containedItems.Count);
		while (i > 0)
		{
			if (num >= this.containedItems.Count)
			{
				num = 0;
			}
			if (this.containedItems[num].amount > 0f)
			{
				itemAmount = this.containedItems[num];
				break;
			}
			num++;
			i--;
		}
		if (itemAmount == null)
		{
			return;
		}
		this.GiveResourceFromItem(entity, itemAmount, gatherDamage, destroyFraction, attackWeapon);
		this.UpdateVars();
		if (entity)
		{
			Debug.Assert(attackWeapon.GetItem() != null, "Attack Weapon " + attackWeapon + " has no Item");
			Debug.Assert(attackWeapon.ownerItemUID.IsValid, "Attack Weapon " + attackWeapon + " ownerItemUID is 0");
			Debug.Assert(attackWeapon.GetParentEntity() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is null");
			Debug.Assert(attackWeapon.GetParentEntity().IsValid(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			Debug.Assert(attackWeapon.GetParentEntity().ToPlayer() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is not a player");
			Debug.Assert(!attackWeapon.GetParentEntity().ToPlayer().IsDead(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			BasePlayer ownerPlayer = attackWeapon.GetOwnerPlayer();
			Debug.Assert(ownerPlayer != null, "Attack Weapon " + attackWeapon + " ownerPlayer is null");
			Debug.Assert(ownerPlayer == entity, "Attack Weapon " + attackWeapon + " ownerPlayer is not player");
			if (ownerPlayer != null)
			{
				Debug.Assert(ownerPlayer.inventory != null, "Attack Weapon " + attackWeapon + " ownerPlayer inventory is null");
				Debug.Assert(ownerPlayer.inventory.FindItemUID(attackWeapon.ownerItemUID) != null, "Attack Weapon " + attackWeapon + " FindItemUID is null");
			}
		}
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000DCF38 File Offset: 0x000DB138
	public void DestroyFraction(float fraction)
	{
		foreach (ItemAmount itemAmount in this.containedItems)
		{
			if (itemAmount.amount > 0f)
			{
				itemAmount.amount -= fraction / this.categoriesRemaining;
			}
		}
		this.UpdateVars();
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000DCFAC File Offset: 0x000DB1AC
	private void GiveResourceFromItem(BasePlayer entity, ItemAmount itemAmt, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (itemAmt.amount == 0f)
		{
			return;
		}
		float num = Mathf.Min(gatherDamage, base.baseEntity.Health()) / base.baseEntity.MaxHealth();
		float num2 = itemAmt.startAmount / this.startingItemCounts;
		float num3 = Mathf.Clamp(itemAmt.startAmount * num / num2, 0f, itemAmt.amount);
		num3 = Mathf.Round(num3);
		float num4 = num3 * destroyFraction * 2f;
		if (itemAmt.amount <= num3 + num4)
		{
			float num5 = (num3 + num4) / itemAmt.amount;
			num3 /= num5;
			num4 /= num5;
		}
		itemAmt.amount -= Mathf.Floor(num3);
		itemAmt.amount -= Mathf.Floor(num4);
		if (num3 < 1f)
		{
			num3 = ((UnityEngine.Random.Range(0f, 1f) <= num3) ? 1f : 0f);
			itemAmt.amount = 0f;
		}
		if (itemAmt.amount < 0f)
		{
			itemAmt.amount = 0f;
		}
		if (num3 >= 1f)
		{
			int num6 = this.CalculateGatherBonus(entity, itemAmt, num3);
			int num7 = Mathf.FloorToInt(num3) + num6;
			Item item = ItemManager.CreateByItemID(itemAmt.itemid, num7, 0UL);
			if (item == null)
			{
				return;
			}
			this.OverrideOwnership(item, attackWeapon);
			Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, base.baseEntity, entity, attackWeapon);
			entity.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
		}
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000DD11C File Offset: 0x000DB31C
	private int CalculateGatherBonus(BaseEntity entity, ItemAmount item, float amountToGive)
	{
		if (entity == null)
		{
			return 0;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		if (basePlayer == null)
		{
			return 0;
		}
		if (basePlayer.modifiers == null)
		{
			return 0;
		}
		amountToGive = (float)Mathf.FloorToInt(amountToGive);
		float num = 1f;
		ResourceDispenser.GatherType gatherType = this.gatherType;
		Modifier.ModifierType modifierType;
		if (gatherType != ResourceDispenser.GatherType.Tree)
		{
			if (gatherType != ResourceDispenser.GatherType.Ore)
			{
				return 0;
			}
			modifierType = Modifier.ModifierType.Ore_Yield;
		}
		else
		{
			modifierType = Modifier.ModifierType.Wood_Yield;
		}
		if (!this.IsProducedItemOfGatherType(item))
		{
			return 0;
		}
		num += basePlayer.modifiers.GetValue(modifierType, 0f);
		float num2 = basePlayer.modifiers.GetVariableValue(modifierType, 0f);
		float num3 = ((num > 1f) ? Mathf.Max(amountToGive * num - amountToGive, 0f) : 0f);
		num2 += num3;
		int num4 = 0;
		if (num2 >= 1f)
		{
			num4 = (int)num2;
			num2 -= (float)num4;
		}
		basePlayer.modifiers.SetVariableValue(modifierType, num2);
		return num4;
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000DD1FC File Offset: 0x000DB3FC
	private bool IsProducedItemOfGatherType(ItemAmount item)
	{
		if (this.gatherType == ResourceDispenser.GatherType.Tree)
		{
			return ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Tree].Contains(item.itemid);
		}
		return this.gatherType == ResourceDispenser.GatherType.Ore && ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Ore].Contains(item.itemid);
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		return false;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000DD249 File Offset: 0x000DB449
	private void UpdateVars()
	{
		this.UpdateFraction();
		this.UpdateRemainingCategories();
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000DD258 File Offset: 0x000DB458
	public void UpdateRemainingCategories()
	{
		int num = 0;
		using (List<ItemAmount>.Enumerator enumerator = this.containedItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.amount > 0f)
				{
					num++;
				}
			}
		}
		this.categoriesRemaining = (float)num;
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000DD2C0 File Offset: 0x000DB4C0
	public void CountAllItems()
	{
		this.startingItemCounts = this.containedItems.Sum((ItemAmount x) => x.startAmount);
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000DD2F4 File Offset: 0x000DB4F4
	private void UpdateFraction()
	{
		float num = this.containedItems.Sum((ItemAmount x) => x.startAmount);
		float num2 = this.containedItems.Sum((ItemAmount x) => x.amount);
		if (num == 0f)
		{
			this.fractionRemaining = 0f;
			return;
		}
		this.fractionRemaining = num2 / num;
	}

	// Token: 0x04001A50 RID: 6736
	public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;

	// Token: 0x04001A51 RID: 6737
	public List<ItemAmount> containedItems;

	// Token: 0x04001A52 RID: 6738
	public float maxDestroyFractionForFinishBonus = 0.2f;

	// Token: 0x04001A53 RID: 6739
	public List<ItemAmount> finishBonus;

	// Token: 0x04001A54 RID: 6740
	public float fractionRemaining = 1f;

	// Token: 0x04001A55 RID: 6741
	private float categoriesRemaining;

	// Token: 0x04001A56 RID: 6742
	private float startingItemCounts;

	// Token: 0x04001A57 RID: 6743
	private static Dictionary<ResourceDispenser.GatherType, HashSet<int>> cachedResourceItemTypes;

	// Token: 0x02000CDA RID: 3290
	public enum GatherType
	{
		// Token: 0x0400459C RID: 17820
		Tree,
		// Token: 0x0400459D RID: 17821
		Ore,
		// Token: 0x0400459E RID: 17822
		Flesh,
		// Token: 0x0400459F RID: 17823
		UNSET,
		// Token: 0x040045A0 RID: 17824
		LAST
	}

	// Token: 0x02000CDB RID: 3291
	[Serializable]
	public class GatherPropertyEntry
	{
		// Token: 0x040045A1 RID: 17825
		public float gatherDamage;

		// Token: 0x040045A2 RID: 17826
		public float destroyFraction;

		// Token: 0x040045A3 RID: 17827
		public float conditionLost;
	}

	// Token: 0x02000CDC RID: 3292
	[Serializable]
	public class GatherProperties
	{
		// Token: 0x06005005 RID: 20485 RVA: 0x001A7FB8 File Offset: 0x001A61B8
		public float GetProficiency()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				float num2 = fromIndex.gatherDamage * fromIndex.destroyFraction;
				if (num2 > 0f)
				{
					num += fromIndex.gatherDamage / num2;
				}
			}
			return num;
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x001A8004 File Offset: 0x001A6204
		public bool Any()
		{
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				if (fromIndex.gatherDamage > 0f || fromIndex.conditionLost > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x001A8042 File Offset: 0x001A6242
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
		{
			return this.GetFromIndex((ResourceDispenser.GatherType)index);
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x001A804B File Offset: 0x001A624B
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(ResourceDispenser.GatherType index)
		{
			switch (index)
			{
			case ResourceDispenser.GatherType.Tree:
				return this.Tree;
			case ResourceDispenser.GatherType.Ore:
				return this.Ore;
			case ResourceDispenser.GatherType.Flesh:
				return this.Flesh;
			default:
				return null;
			}
		}

		// Token: 0x040045A4 RID: 17828
		public ResourceDispenser.GatherPropertyEntry Tree;

		// Token: 0x040045A5 RID: 17829
		public ResourceDispenser.GatherPropertyEntry Ore;

		// Token: 0x040045A6 RID: 17830
		public ResourceDispenser.GatherPropertyEntry Flesh;
	}
}
