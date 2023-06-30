using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class StagedResourceEntity : ResourceEntity
{
	// Token: 0x06001323 RID: 4899 RVA: 0x00099E60 File Offset: 0x00098060
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x00099EA0 File Offset: 0x000980A0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		int num = info.msg.resource.stage;
		if (info.fromDisk && base.isServer)
		{
			this.health = this.startHealth;
			num = 0;
		}
		if (num != this.stage)
		{
			this.stage = num;
			this.UpdateStage();
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x00099F0C File Offset: 0x0009810C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.resource == null)
		{
			info.msg.resource = Pool.Get<BaseResource>();
		}
		info.msg.resource.health = this.Health();
		info.msg.resource.stage = this.stage;
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x00099F69 File Offset: 0x00098169
	protected override void OnHealthChanged()
	{
		base.Invoke(new Action(this.UpdateNetworkStage), 0.1f);
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x00099F83 File Offset: 0x00098183
	protected virtual void UpdateNetworkStage()
	{
		if (this.FindBestStage() != this.stage)
		{
			this.stage = this.FindBestStage();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.UpdateStage();
		}
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00099FAC File Offset: 0x000981AC
	private int FindBestStage()
	{
		float num = Mathf.InverseLerp(0f, this.MaxHealth(), this.Health());
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (num >= this.stages[i].health)
			{
				return i;
			}
		}
		return this.stages.Count - 1;
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x0009A009 File Offset: 0x00098209
	public T GetStageComponent<T>() where T : Component
	{
		return this.stages[this.stage].instance.GetComponentInChildren<T>();
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0009A028 File Offset: 0x00098228
	private void UpdateStage()
	{
		if (this.stages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			this.stages[i].instance.SetActive(i == this.stage);
		}
		GroundWatch.PhysicsChanged(base.gameObject);
	}

	// Token: 0x04000BFC RID: 3068
	public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();

	// Token: 0x04000BFD RID: 3069
	public int stage;

	// Token: 0x04000BFE RID: 3070
	public GameObjectRef changeStageEffect;

	// Token: 0x04000BFF RID: 3071
	public GameObject gibSourceTest;

	// Token: 0x02000C16 RID: 3094
	[Serializable]
	public class ResourceStage
	{
		// Token: 0x04004265 RID: 16997
		public float health;

		// Token: 0x04004266 RID: 16998
		public GameObject instance;
	}
}
