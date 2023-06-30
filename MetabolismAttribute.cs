using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
[Serializable]
public class MetabolismAttribute
{
	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x0600232D RID: 9005 RVA: 0x000E1399 File Offset: 0x000DF599
	public float greatFraction
	{
		get
		{
			return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
		}
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000E13B2 File Offset: 0x000DF5B2
	public void Reset()
	{
		this.value = Mathf.Clamp(UnityEngine.Random.Range(this.startMin, this.startMax), this.min, this.max);
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000E13DC File Offset: 0x000DF5DC
	public float Fraction()
	{
		return Mathf.InverseLerp(this.min, this.max, this.value);
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000E13F5 File Offset: 0x000DF5F5
	public float InverseFraction()
	{
		return 1f - this.Fraction();
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000E1403 File Offset: 0x000DF603
	public void Add(float val)
	{
		this.value = Mathf.Clamp(this.value + val, this.min, this.max);
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000E1424 File Offset: 0x000DF624
	public void Subtract(float val)
	{
		this.value = Mathf.Clamp(this.value - val, this.min, this.max);
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000E1445 File Offset: 0x000DF645
	public void Increase(float fTarget)
	{
		fTarget = Mathf.Clamp(fTarget, this.min, this.max);
		if (fTarget <= this.value)
		{
			return;
		}
		this.value = fTarget;
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000E146C File Offset: 0x000DF66C
	public void MoveTowards(float fTarget, float fRate)
	{
		if (fRate == 0f)
		{
			return;
		}
		this.value = Mathf.Clamp(Mathf.MoveTowards(this.value, fTarget, fRate), this.min, this.max);
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000E149B File Offset: 0x000DF69B
	public bool HasChanged()
	{
		bool flag = this.lastValue != this.value;
		this.lastValue = this.value;
		return flag;
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000E14BC File Offset: 0x000DF6BC
	public bool HasGreatlyChanged()
	{
		float greatFraction = this.greatFraction;
		bool flag = this.lastGreatFraction != greatFraction;
		this.lastGreatFraction = greatFraction;
		return flag;
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000E14E3 File Offset: 0x000DF6E3
	public void SetValue(float newValue)
	{
		this.value = newValue;
	}

	// Token: 0x04001B11 RID: 6929
	public float startMin;

	// Token: 0x04001B12 RID: 6930
	public float startMax;

	// Token: 0x04001B13 RID: 6931
	public float min;

	// Token: 0x04001B14 RID: 6932
	public float max;

	// Token: 0x04001B15 RID: 6933
	public float value;

	// Token: 0x04001B16 RID: 6934
	internal float lastValue;

	// Token: 0x04001B17 RID: 6935
	internal float lastGreatFraction;

	// Token: 0x04001B18 RID: 6936
	private const float greatInterval = 0.1f;

	// Token: 0x02000CED RID: 3309
	public enum Type
	{
		// Token: 0x040045E1 RID: 17889
		Calories,
		// Token: 0x040045E2 RID: 17890
		Hydration,
		// Token: 0x040045E3 RID: 17891
		Heartrate,
		// Token: 0x040045E4 RID: 17892
		Poison,
		// Token: 0x040045E5 RID: 17893
		Radiation,
		// Token: 0x040045E6 RID: 17894
		Bleeding,
		// Token: 0x040045E7 RID: 17895
		Health,
		// Token: 0x040045E8 RID: 17896
		HealthOverTime
	}
}
