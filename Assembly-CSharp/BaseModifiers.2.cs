using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000435 RID: 1077
public abstract class BaseModifiers<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x0600246B RID: 9323 RVA: 0x000E843D File Offset: 0x000E663D
	public int ActiveModifierCoount
	{
		get
		{
			return this.All.Count;
		}
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x000E844C File Offset: 0x000E664C
	public void Add(List<ModifierDefintion> modDefs)
	{
		foreach (ModifierDefintion modifierDefintion in modDefs)
		{
			this.Add(modifierDefintion);
		}
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000E849C File Offset: 0x000E669C
	protected void Add(ModifierDefintion def)
	{
		Modifier modifier = new Modifier();
		modifier.Init(def.type, def.source, def.value, def.duration, def.duration);
		this.Add(modifier);
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000E84DC File Offset: 0x000E66DC
	protected void Add(Modifier modifier)
	{
		if (!this.CanAdd(modifier))
		{
			return;
		}
		int maxModifiersForSourceType = this.GetMaxModifiersForSourceType(modifier.Source);
		if (this.GetTypeSourceCount(modifier.Type, modifier.Source) >= maxModifiersForSourceType)
		{
			Modifier shortestLifeModifier = this.GetShortestLifeModifier(modifier.Type, modifier.Source);
			if (shortestLifeModifier == null)
			{
				return;
			}
			this.Remove(shortestLifeModifier);
		}
		this.All.Add(modifier);
		if (!this.totalValues.ContainsKey(modifier.Type))
		{
			this.totalValues.Add(modifier.Type, modifier.Value);
		}
		else
		{
			Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
			Modifier.ModifierType type = modifier.Type;
			dictionary[type] += modifier.Value;
		}
		this.SetDirty(true);
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000E8595 File Offset: 0x000E6795
	private bool CanAdd(Modifier modifier)
	{
		return !this.All.Contains(modifier);
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000E85A8 File Offset: 0x000E67A8
	private int GetMaxModifiersForSourceType(Modifier.ModifierSource source)
	{
		if (source == Modifier.ModifierSource.Tea)
		{
			return 1;
		}
		return int.MaxValue;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000E85B4 File Offset: 0x000E67B4
	private int GetTypeSourceCount(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		int num = 0;
		foreach (Modifier modifier in this.All)
		{
			if (modifier.Type == type && modifier.Source == source)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000E861C File Offset: 0x000E681C
	private Modifier GetShortestLifeModifier(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		Modifier modifier = null;
		foreach (Modifier modifier2 in this.All)
		{
			if (modifier2.Type == type && modifier2.Source == source)
			{
				if (modifier == null)
				{
					modifier = modifier2;
				}
				else if (modifier2.TimeRemaining < modifier.TimeRemaining)
				{
					modifier = modifier2;
				}
			}
		}
		return modifier;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000E8694 File Offset: 0x000E6894
	private void Remove(Modifier modifier)
	{
		if (!this.All.Contains(modifier))
		{
			return;
		}
		this.All.Remove(modifier);
		Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
		Modifier.ModifierType type = modifier.Type;
		dictionary[type] -= modifier.Value;
		this.SetDirty(true);
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000E86E7 File Offset: 0x000E68E7
	public void RemoveAll()
	{
		this.All.Clear();
		this.totalValues.Clear();
		this.SetDirty(true);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000E8708 File Offset: 0x000E6908
	public float GetValue(Modifier.ModifierType type, float defaultValue = 0f)
	{
		float num;
		if (this.totalValues.TryGetValue(type, out num))
		{
			return num;
		}
		return defaultValue;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000E8728 File Offset: 0x000E6928
	public float GetVariableValue(Modifier.ModifierType type, float defaultValue)
	{
		float num;
		if (this.modifierVariables.TryGetValue(type, out num))
		{
			return num;
		}
		return defaultValue;
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000E8748 File Offset: 0x000E6948
	public void SetVariableValue(Modifier.ModifierType type, float value)
	{
		float num;
		if (this.modifierVariables.TryGetValue(type, out num))
		{
			this.modifierVariables[type] = value;
			return;
		}
		this.modifierVariables.Add(type, value);
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000E8780 File Offset: 0x000E6980
	public void RemoveVariable(Modifier.ModifierType type)
	{
		this.modifierVariables.Remove(type);
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000E878F File Offset: 0x000E698F
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000E87A5 File Offset: 0x000E69A5
	protected void SetDirty(bool flag)
	{
		this.dirty = flag;
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000E87AE File Offset: 0x000E69AE
	public virtual void ServerInit(T owner)
	{
		this.owner = owner;
		this.ResetTicking();
		this.RemoveAll();
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000E87C3 File Offset: 0x000E69C3
	public void ResetTicking()
	{
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000E87DC File Offset: 0x000E69DC
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastTickTime;
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick += num;
		if (this.timeSinceLastTick <= ConVar.Server.modifierTickRate)
		{
			return;
		}
		if (this.owner != null && !this.owner.IsDead())
		{
			this.TickModifiers(ownerEntity, this.timeSinceLastTick);
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000E885C File Offset: 0x000E6A5C
	protected virtual void TickModifiers(BaseCombatEntity ownerEntity, float delta)
	{
		for (int i = this.All.Count - 1; i >= 0; i--)
		{
			Modifier modifier = this.All[i];
			modifier.Tick(ownerEntity, delta);
			if (modifier.Expired)
			{
				this.Remove(modifier);
			}
		}
	}

	// Token: 0x04001C58 RID: 7256
	public List<Modifier> All = new List<Modifier>();

	// Token: 0x04001C59 RID: 7257
	protected Dictionary<Modifier.ModifierType, float> totalValues = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001C5A RID: 7258
	protected Dictionary<Modifier.ModifierType, float> modifierVariables = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001C5B RID: 7259
	protected T owner;

	// Token: 0x04001C5C RID: 7260
	protected bool dirty = true;

	// Token: 0x04001C5D RID: 7261
	protected float timeSinceLastTick;

	// Token: 0x04001C5E RID: 7262
	protected float lastTickTime;
}
