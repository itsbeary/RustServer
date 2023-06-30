using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x06002323 RID: 8995 RVA: 0x000E109C File Offset: 0x000DF29C
	public virtual void Reset()
	{
		this.calories.Reset();
		this.hydration.Reset();
		this.heartrate.Reset();
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000E10BF File Offset: 0x000DF2BF
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000E10D5 File Offset: 0x000DF2D5
	public virtual void ServerInit(T owner)
	{
		this.Reset();
		this.owner = owner;
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000E10E4 File Offset: 0x000DF2E4
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		this.timeSinceLastMetabolism += delta;
		if (this.timeSinceLastMetabolism <= ConVar.Server.metabolismtick)
		{
			return;
		}
		if (this.owner && !this.owner.IsDead())
		{
			this.RunMetabolism(ownerEntity, this.timeSinceLastMetabolism);
			this.DoMetabolismDamage(ownerEntity, this.timeSinceLastMetabolism);
		}
		this.timeSinceLastMetabolism = 0f;
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000E1158 File Offset: 0x000DF358
	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value <= 20f)
		{
			using (TimeWarning.New("Calories Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.calories.value) * delta * 0.083333336f, DamageType.Hunger, null, true);
			}
		}
		if (this.hydration.value <= 20f)
		{
			using (TimeWarning.New("Hyration Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.hydration.value) * delta * 0.13333334f, DamageType.Thirst, null, true);
			}
		}
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000E122C File Offset: 0x000DF42C
	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.calories.value) * delta * 0.016666668f);
		}
		if (this.hydration.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.hydration.value) * delta * 0.016666668f);
		}
		this.hydration.MoveTowards(0f, delta * 0.008333334f);
		this.calories.MoveTowards(0f, delta * 0.016666668f);
		this.heartrate.MoveTowards(0.05f, delta * 0.016666668f);
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000E12F4 File Offset: 0x000DF4F4
	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		MetabolismAttribute metabolismAttribute = this.FindAttribute(type);
		if (metabolismAttribute == null)
		{
			return;
		}
		metabolismAttribute.Add(amount);
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000E1314 File Offset: 0x000DF514
	public bool ShouldDie()
	{
		return this.owner && this.owner.Health() <= 0f;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000E1344 File Offset: 0x000DF544
	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Calories:
			return this.calories;
		case MetabolismAttribute.Type.Hydration:
			return this.hydration;
		case MetabolismAttribute.Type.Heartrate:
			return this.heartrate;
		default:
			return null;
		}
	}

	// Token: 0x04001B0C RID: 6924
	protected T owner;

	// Token: 0x04001B0D RID: 6925
	public MetabolismAttribute calories = new MetabolismAttribute();

	// Token: 0x04001B0E RID: 6926
	public MetabolismAttribute hydration = new MetabolismAttribute();

	// Token: 0x04001B0F RID: 6927
	public MetabolismAttribute heartrate = new MetabolismAttribute();

	// Token: 0x04001B10 RID: 6928
	protected float timeSinceLastMetabolism;
}
