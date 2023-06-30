using System;
using Facepunch;
using ProtoBuf;
using UnityEngine.Serialization;

// Token: 0x02000461 RID: 1121
public class ResourceEntity : global::BaseEntity
{
	// Token: 0x06002533 RID: 9523 RVA: 0x000EB4AF File Offset: 0x000E96AF
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		this.health = info.msg.resource.health;
	}

	// Token: 0x06002534 RID: 9524 RVA: 0x000EB4DC File Offset: 0x000E96DC
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] array = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(array);
		}
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000EB514 File Offset: 0x000E9714
	public override void ServerInit()
	{
		base.ServerInit();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		if (this.health == 0f)
		{
			this.health = this.startHealth;
		}
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000EB541 File Offset: 0x000E9741
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.resource = Pool.Get<BaseResource>();
			info.msg.resource.health = this.Health();
		}
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x000EB578 File Offset: 0x000E9778
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000EB580 File Offset: 0x000E9780
	public override float Health()
	{
		return this.health;
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnHealthChanged()
	{
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x000EB588 File Offset: 0x000E9788
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && !this.isKilled)
		{
			if (this.resourceDispenser != null)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				if (this.baseProtection)
				{
					this.baseProtection.Scale(info.damageTypes, 1f);
				}
				float num = info.damageTypes.Total();
				this.health -= num;
				if (this.health <= 0f)
				{
					this.OnKilled(info);
					return;
				}
				this.OnHealthChanged();
			}
		}
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x000EB622 File Offset: 0x000E9822
	public virtual void OnKilled(HitInfo info)
	{
		this.isKilled = true;
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x04001D6E RID: 7534
	[FormerlySerializedAs("health")]
	public float startHealth;

	// Token: 0x04001D6F RID: 7535
	[FormerlySerializedAs("protection")]
	public ProtectionProperties baseProtection;

	// Token: 0x04001D70 RID: 7536
	protected float health;

	// Token: 0x04001D71 RID: 7537
	internal ResourceDispenser resourceDispenser;

	// Token: 0x04001D72 RID: 7538
	[NonSerialized]
	protected bool isKilled;
}
