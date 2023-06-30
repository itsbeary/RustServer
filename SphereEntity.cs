using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000428 RID: 1064
public class SphereEntity : global::BaseEntity
{
	// Token: 0x06002421 RID: 9249 RVA: 0x000E6BC9 File Offset: 0x000E4DC9
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.currentRadius;
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000E6BF8 File Offset: 0x000E4DF8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			if (info.msg.sphereEntity != null)
			{
				this.currentRadius = (this.lerpRadius = info.msg.sphereEntity.radius);
			}
			this.UpdateScale();
		}
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000E6C46 File Offset: 0x000E4E46
	public void LerpRadiusTo(float radius, float speed)
	{
		this.lerpRadius = radius;
		this.lerpSpeed = speed;
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000E6C56 File Offset: 0x000E4E56
	protected void UpdateScale()
	{
		base.transform.localScale = new Vector3(this.currentRadius, this.currentRadius, this.currentRadius);
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000E6C7C File Offset: 0x000E4E7C
	protected void Update()
	{
		if (this.currentRadius == this.lerpRadius)
		{
			return;
		}
		if (base.isServer)
		{
			this.currentRadius = Mathf.MoveTowards(this.currentRadius, this.lerpRadius, Time.deltaTime * this.lerpSpeed);
			this.UpdateScale();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x04001C26 RID: 7206
	public float currentRadius = 1f;

	// Token: 0x04001C27 RID: 7207
	public float lerpRadius = 1f;

	// Token: 0x04001C28 RID: 7208
	public float lerpSpeed = 1f;
}
