using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class Spawnable : MonoBehaviour, IServerComponent
{
	// Token: 0x06002AD3 RID: 10963 RVA: 0x0010548E File Offset: 0x0010368E
	protected void OnEnable()
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Add();
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x0010549E File Offset: 0x0010369E
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x001054B8 File Offset: 0x001036B8
	private void Add()
	{
		this.SpawnPosition = base.transform.position;
		this.SpawnRotation = base.transform.rotation;
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (this.Population != null)
			{
				SingletonComponent<SpawnHandler>.Instance.AddInstance(this);
				return;
			}
			if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
			{
				global::BaseEntity component = base.GetComponent<global::BaseEntity>();
				if (component != null && component.enableSaving && !component.syncPosition)
				{
					SingletonComponent<SpawnHandler>.Instance.AddRespawn(new SpawnIndividual(component.prefabID, this.SpawnPosition, this.SpawnRotation));
				}
			}
		}
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x0010555C File Offset: 0x0010375C
	private void Remove()
	{
		if (SingletonComponent<SpawnHandler>.Instance && this.Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.RemoveInstance(this);
		}
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x00105583 File Offset: 0x00103783
	internal void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.Population == null)
		{
			return;
		}
		info.msg.spawnable = Pool.Get<ProtoBuf.Spawnable>();
		info.msg.spawnable.population = this.Population.FilenameStringId;
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x001055BF File Offset: 0x001037BF
	internal void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.spawnable != null)
		{
			this.Population = FileSystem.Load<SpawnPopulation>(StringPool.Get(info.msg.spawnable.population), true);
		}
		this.Add();
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x001055F5 File Offset: 0x001037F5
	protected void OnValidate()
	{
		this.Population = null;
	}

	// Token: 0x04002308 RID: 8968
	[ReadOnly]
	public SpawnPopulation Population;

	// Token: 0x04002309 RID: 8969
	[SerializeField]
	private bool ForceSpawnOnly;

	// Token: 0x0400230A RID: 8970
	[SerializeField]
	private string ForceSpawnInfoMessage = string.Empty;

	// Token: 0x0400230B RID: 8971
	internal uint PrefabID;

	// Token: 0x0400230C RID: 8972
	internal bool SpawnIndividual;

	// Token: 0x0400230D RID: 8973
	internal Vector3 SpawnPosition;

	// Token: 0x0400230E RID: 8974
	internal Quaternion SpawnRotation;
}
