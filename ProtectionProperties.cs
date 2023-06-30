using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200075D RID: 1885
[CreateAssetMenu(menuName = "Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
	// Token: 0x0600347E RID: 13438 RVA: 0x00144614 File Offset: 0x00142814
	public void OnValidate()
	{
		if (this.amounts.Length < 25)
		{
			float[] array = new float[25];
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= this.amounts.Length)
				{
					if (i == 21)
					{
						array[i] = this.amounts[9];
					}
				}
				else
				{
					array[i] = this.amounts[i];
				}
			}
			this.amounts = array;
		}
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x00144674 File Offset: 0x00142874
	public void Clear()
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] = 0f;
		}
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x001446A4 File Offset: 0x001428A4
	public void Add(float amount)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] += amount;
		}
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x001446D5 File Offset: 0x001428D5
	public void Add(DamageType index, float amount)
	{
		this.amounts[(int)index] += amount;
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x001446E8 File Offset: 0x001428E8
	public void Add(ProtectionProperties other, float scale)
	{
		for (int i = 0; i < Mathf.Min(other.amounts.Length, this.amounts.Length); i++)
		{
			this.amounts[i] += other.amounts[i] * scale;
		}
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x00144730 File Offset: 0x00142930
	public void Add(List<Item> items, HitArea area = (HitArea)(-1))
	{
		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area))
			{
				component.CollectProtection(item, this);
			}
		}
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x0014477C File Offset: 0x0014297C
	public void Multiply(float multiplier)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] *= multiplier;
		}
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x001447AD File Offset: 0x001429AD
	public void Multiply(DamageType index, float multiplier)
	{
		this.amounts[(int)index] *= multiplier;
	}

	// Token: 0x06003486 RID: 13446 RVA: 0x001447C0 File Offset: 0x001429C0
	public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			if (this.amounts[i] != 0f)
			{
				damageList.Scale((DamageType)i, 1f - Mathf.Clamp(this.amounts[i] * ProtectionAmount, -1f, 1f));
			}
		}
	}

	// Token: 0x06003487 RID: 13447 RVA: 0x00144815 File Offset: 0x00142A15
	public float Get(DamageType damageType)
	{
		return this.amounts[(int)damageType];
	}

	// Token: 0x04002AF8 RID: 11000
	[TextArea]
	public string comments;

	// Token: 0x04002AF9 RID: 11001
	[Range(0f, 100f)]
	public float density = 1f;

	// Token: 0x04002AFA RID: 11002
	[ArrayIndexIsEnumRanged(enumType = typeof(DamageType), min = -4f, max = 3f)]
	public float[] amounts = new float[25];
}
