using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class BasePet : NPCPlayer, IThinker
{
	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06001A13 RID: 6675 RVA: 0x000BD895 File Offset: 0x000BBA95
	// (set) Token: 0x06001A14 RID: 6676 RVA: 0x000BD89D File Offset: 0x000BBA9D
	public PetBrain Brain { get; protected set; }

	// Token: 0x06001A15 RID: 6677 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x0002A08D File Offset: 0x0002828D
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x0002A085 File Offset: 0x00028285
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x000BD8A8 File Offset: 0x000BBAA8
	public static void ProcessMovementQueue()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = BasePet.movementupdatebudgetms / 1000f;
		while (BasePet._movementProcessQueue.Count > 0 && Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BasePet basePet = BasePet._movementProcessQueue.Dequeue();
			if (basePet != null)
			{
				basePet.DoBudgetedMoveUpdate();
				basePet.inQueue = false;
			}
		}
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x000BD902 File Offset: 0x000BBB02
	public void DoBudgetedMoveUpdate()
	{
		if (this.Brain != null)
		{
			this.Brain.DoMovementTick();
		}
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x000BD91D File Offset: 0x000BBB1D
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<PetBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddPet(this);
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000BD940 File Offset: 0x000BBB40
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x000BD9B1 File Offset: 0x000BBBB1
	internal override void DoServerDestroy()
	{
		if (this.Brain.OwningPlayer != null)
		{
			this.Brain.OwningPlayer.ClearClientPetLink();
		}
		AIThinkManager.RemovePet(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000BD9E2 File Offset: 0x000BBBE2
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x000BD9EA File Offset: 0x000BBBEA
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x000BDA0C File Offset: 0x000BBC0C
	public void ApplyPetStatModifiers()
	{
		if (this.inventory == null)
		{
			return;
		}
		for (int i = 0; i < this.inventory.containerWear.capacity; i++)
		{
			Item slot = this.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				ItemModPetStats component = slot.info.GetComponent<ItemModPetStats>();
				if (component != null)
				{
					component.Apply(this);
				}
			}
		}
		this.Heal(this.MaxHealth());
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x000BDA80 File Offset: 0x000BBC80
	private void OnPhysicsNeighbourChanged()
	{
		if (this.Brain != null && this.Brain.Navigator != null)
		{
			this.Brain.Navigator.ForceToGround();
		}
	}

	// Token: 0x040012A5 RID: 4773
	public static Dictionary<ulong, BasePet> ActivePetByOwnerID = new Dictionary<ulong, BasePet>();

	// Token: 0x040012A6 RID: 4774
	[ServerVar]
	public static bool queuedMovementsAllowed = true;

	// Token: 0x040012A7 RID: 4775
	[ServerVar]
	public static bool onlyQueueBaseNavMovements = true;

	// Token: 0x040012A8 RID: 4776
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet movements per frame")]
	public static float movementupdatebudgetms = 1f;

	// Token: 0x040012A9 RID: 4777
	public float BaseAttackRate = 2f;

	// Token: 0x040012AA RID: 4778
	public float BaseAttackDamge = 20f;

	// Token: 0x040012AB RID: 4779
	public DamageType AttackDamageType = DamageType.Slash;

	// Token: 0x040012AD RID: 4781
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x040012AE RID: 4782
	private BaseEntity _mapMarkerInstance;

	// Token: 0x040012AF RID: 4783
	[HideInInspector]
	public bool inQueue;

	// Token: 0x040012B0 RID: 4784
	public static Queue<BasePet> _movementProcessQueue = new Queue<BasePet>();
}
