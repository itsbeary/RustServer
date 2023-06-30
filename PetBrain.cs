using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class PetBrain : BaseAIBrain
{
	// Token: 0x06000F96 RID: 3990 RVA: 0x00082658 File Offset: 0x00080858
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PetBrain.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x00082698 File Offset: 0x00080898
	public override void AddStates()
	{
		base.AddStates();
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x000826A0 File Offset: 0x000808A0
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		PetBrain.Count++;
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x000826F2 File Offset: 0x000808F2
	public override void OnDestroy()
	{
		base.OnDestroy();
		PetBrain.Count--;
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x00082708 File Offset: 0x00080908
	public override void Think(float delta)
	{
		base.Think(delta);
		if (PetBrain.DrownInDeepWater)
		{
			BaseCombatEntity baseCombatEntity = this.GetBaseEntity() as BaseCombatEntity;
			if (baseCombatEntity != null && baseCombatEntity.WaterFactor() > 0.85f && !baseCombatEntity.IsDestroyed)
			{
				baseCombatEntity.Hurt(delta * (baseCombatEntity.MaxHealth() / PetBrain.DrownTimer), DamageType.Drowned, null, true);
			}
		}
		this.EvaluateLoadDefaultDesignTriggers();
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0008276C File Offset: 0x0008096C
	private bool EvaluateLoadDefaultDesignTriggers()
	{
		if (this.loadedDesignIndex == 0)
		{
			return true;
		}
		bool flag = false;
		if (PetBrain.IdleWhenOwnerOfflineOrDead)
		{
			flag = (PetBrain.IdleWhenOwnerOfflineOrDead && base.OwningPlayer == null) || base.OwningPlayer.IsSleeping() || base.OwningPlayer.IsDead();
		}
		if (PetBrain.IdleWhenOwnerMounted && !flag)
		{
			flag = base.OwningPlayer != null && base.OwningPlayer.isMounted;
		}
		if (base.OwningPlayer != null && Vector3.Distance(base.transform.position, base.OwningPlayer.transform.position) > PetBrain.ControlDistance)
		{
			flag = true;
		}
		if (flag)
		{
			base.LoadDefaultAIDesign();
			return true;
		}
		return false;
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x00082828 File Offset: 0x00080A28
	public override void OnAIDesignLoadedAtIndex(int index)
	{
		base.OnAIDesignLoadedAtIndex(index);
		BaseEntity baseEntity = this.GetBaseEntity();
		if (baseEntity != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(baseEntity.OwnerID);
			if (basePlayer != null)
			{
				basePlayer.SendClientPetStateIndex();
			}
			baseEntity.ClientRPC(null, "OnCommandGiven");
		}
	}

	// Token: 0x04000A33 RID: 2611
	[Header("Audio")]
	public SoundDefinition CommandGivenVocalSFX;

	// Token: 0x04000A34 RID: 2612
	[ServerVar]
	public static bool DrownInDeepWater = true;

	// Token: 0x04000A35 RID: 2613
	[ServerVar]
	public static bool IdleWhenOwnerOfflineOrDead = true;

	// Token: 0x04000A36 RID: 2614
	[ServerVar]
	public static bool IdleWhenOwnerMounted = true;

	// Token: 0x04000A37 RID: 2615
	[ServerVar]
	public static float DrownTimer = 15f;

	// Token: 0x04000A38 RID: 2616
	[ReplicatedVar]
	public static float ControlDistance = 100f;

	// Token: 0x04000A39 RID: 2617
	public static int Count;
}
