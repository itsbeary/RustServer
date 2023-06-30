using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public class ModularCarCodeLock
{
	// Token: 0x17000333 RID: 819
	// (get) Token: 0x060026EE RID: 9966 RVA: 0x000F427F File Offset: 0x000F247F
	public bool HasALock
	{
		get
		{
			return this.isServer && !string.IsNullOrEmpty(this.Code);
		}
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x060026EF RID: 9967 RVA: 0x000F4299 File Offset: 0x000F2499
	public bool CentralLockingIsOn
	{
		get
		{
			return this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000335 RID: 821
	// (get) Token: 0x060026F0 RID: 9968 RVA: 0x000F42BB File Offset: 0x000F24BB
	// (set) Token: 0x060026F1 RID: 9969 RVA: 0x000F42C3 File Offset: 0x000F24C3
	public List<ulong> WhitelistPlayers { get; private set; } = new List<ulong>();

	// Token: 0x060026F2 RID: 9970 RVA: 0x000F42CC File Offset: 0x000F24CC
	public ModularCarCodeLock(ModularCar owner, bool isServer)
	{
		this.owner = owner;
		this.isServer = isServer;
		if (isServer)
		{
			this.CheckEnableCentralLocking();
		}
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000F430C File Offset: 0x000F250C
	public bool PlayerCanDestroyLock(BaseVehicleModule viaModule)
	{
		return this.HasALock && viaModule.healthFraction <= 0.2f;
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000F4328 File Offset: 0x000F2528
	public bool CodeEntryBlocked(BasePlayer player)
	{
		return !this.HasLockPermission(player) && this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved10);
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000F4358 File Offset: 0x000F2558
	public void Load(BaseNetworkable.LoadInfo info)
	{
		this.Code = info.msg.modularCar.lockCode;
		if (this.Code == null)
		{
			this.Code = "";
		}
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.AddRange(info.msg.modularCar.whitelistUsers);
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000F43B4 File Offset: 0x000F25B4
	public bool HasLockPermission(BasePlayer player)
	{
		return !this.HasALock || (player.IsValid() && !player.IsDead() && this.WhitelistPlayers.Contains(player.userID));
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000F43E3 File Offset: 0x000F25E3
	public bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return (lockType == ModularCarCodeLock.LockType.Door && !this.CentralLockingIsOn) || this.HasLockPermission(player);
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x060026F8 RID: 9976 RVA: 0x000F43F9 File Offset: 0x000F25F9
	// (set) Token: 0x060026F9 RID: 9977 RVA: 0x000F4401 File Offset: 0x000F2601
	public string Code { get; private set; } = "";

	// Token: 0x060026FA RID: 9978 RVA: 0x000F440A File Offset: 0x000F260A
	public void PostServerLoad()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.CheckEnableCentralLocking();
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000F4425 File Offset: 0x000F2625
	public bool CanHaveALock()
	{
		return !this.owner.IsDead() && this.owner.HasDriverMountPoints();
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000F4441 File Offset: 0x000F2641
	public bool TryAddALock(string code, ulong userID)
	{
		if (!this.isServer)
		{
			return false;
		}
		if (this.owner.IsDead())
		{
			return false;
		}
		this.TrySetNewCode(code, userID);
		return this.HasALock;
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000F446B File Offset: 0x000F266B
	public bool IsValidLockCode(string code)
	{
		return code != null && code.Length == 4 && code.IsNumeric();
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000F4481 File Offset: 0x000F2681
	public bool TrySetNewCode(string newCode, ulong userID)
	{
		if (!this.IsValidLockCode(newCode))
		{
			return false;
		}
		this.Code = newCode;
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.Add(userID);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000F44B9 File Offset: 0x000F26B9
	public void RemoveLock()
	{
		if (!this.isServer)
		{
			return;
		}
		if (!this.HasALock)
		{
			return;
		}
		this.Code = "";
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000F44E4 File Offset: 0x000F26E4
	public bool TryOpenWithCode(BasePlayer player, string codeEntered)
	{
		if (this.CodeEntryBlocked(player))
		{
			return false;
		}
		if (!(codeEntered == this.Code))
		{
			if (Time.realtimeSinceStartup > this.lastWrongTime + 60f)
			{
				this.wrongCodes = 0;
			}
			player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this.owner, false);
			this.wrongCodes++;
			if (this.wrongCodes > 5)
			{
				player.ShowToast(GameTip.Styles.Red_Normal, CodeLock.blockwarning, Array.Empty<string>());
			}
			if ((float)this.wrongCodes >= CodeLock.maxFailedAttempts)
			{
				this.owner.SetFlag(BaseEntity.Flags.Reserved10, true, false, true);
				this.owner.Invoke(new Action(this.ClearCodeEntryBlocked), CodeLock.lockoutCooldown);
			}
			this.lastWrongTime = Time.realtimeSinceStartup;
			return false;
		}
		if (!this.WhitelistPlayers.Contains(player.userID))
		{
			this.WhitelistPlayers.Add(player.userID);
			this.wrongCodes = 0;
		}
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000F45EF File Offset: 0x000F27EF
	private void ClearCodeEntryBlocked()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.wrongCodes = 0;
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000F460C File Offset: 0x000F280C
	public void CheckEnableCentralLocking()
	{
		if (this.CentralLockingIsOn)
		{
			return;
		}
		bool flag = false;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.owner.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleSeating vehicleModuleSeating;
				if ((vehicleModuleSeating = enumerator.Current as VehicleModuleSeating) != null && vehicleModuleSeating.HasADriverSeat() && vehicleModuleSeating.AnyMounted())
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			this.owner.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000F469C File Offset: 0x000F289C
	public void ToggleCentralLocking()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved2, !this.CentralLockingIsOn, false, true);
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000F46BC File Offset: 0x000F28BC
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.modularCar.hasLock = this.HasALock;
		if (info.forDisk)
		{
			info.msg.modularCar.lockCode = this.Code;
		}
		info.msg.modularCar.whitelistUsers = Pool.Get<List<ulong>>();
		info.msg.modularCar.whitelistUsers.AddRange(this.WhitelistPlayers);
	}

	// Token: 0x04001F53 RID: 8019
	private readonly bool isServer;

	// Token: 0x04001F54 RID: 8020
	private readonly ModularCar owner;

	// Token: 0x04001F55 RID: 8021
	public const BaseEntity.Flags FLAG_CENTRAL_LOCKING = BaseEntity.Flags.Reserved2;

	// Token: 0x04001F56 RID: 8022
	public const BaseEntity.Flags FLAG_CODE_ENTRY_BLOCKED = BaseEntity.Flags.Reserved10;

	// Token: 0x04001F57 RID: 8023
	public const float LOCK_DESTROY_HEALTH = 0.2f;

	// Token: 0x04001F5A RID: 8026
	private int wrongCodes;

	// Token: 0x04001F5B RID: 8027
	private float lastWrongTime = float.NegativeInfinity;

	// Token: 0x02000D20 RID: 3360
	public enum LockType
	{
		// Token: 0x040046D2 RID: 18130
		Door,
		// Token: 0x040046D3 RID: 18131
		General
	}
}
