using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B58 RID: 2904
	public class SimpleAIMemory
	{
		// Token: 0x06004642 RID: 17986 RVA: 0x0019959C File Offset: 0x0019779C
		public void SetKnown(BaseEntity ent, BaseEntity owner, AIBrainSenses brainSenses)
		{
			IAISenses iaisenses = owner as IAISenses;
			BasePlayer basePlayer = ent as BasePlayer;
			if (basePlayer != null && SimpleAIMemory.PlayerIgnoreList.Contains(basePlayer))
			{
				return;
			}
			bool flag = false;
			if (iaisenses != null && iaisenses.IsThreat(ent))
			{
				flag = true;
				if (brainSenses != null)
				{
					brainSenses.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
				}
			}
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					SimpleAIMemory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = ent.transform.position;
					seenInfo.Timestamp = Mathf.Max(UnityEngine.Time.realtimeSinceStartup, seenInfo.Timestamp);
					this.All[i] = seenInfo;
					return;
				}
			}
			if (basePlayer != null)
			{
				if (AI.ignoreplayers && !basePlayer.IsNpc)
				{
					return;
				}
				this.Players.Add(ent);
			}
			if (iaisenses != null)
			{
				if (iaisenses.IsTarget(ent))
				{
					this.Targets.Add(ent);
				}
				if (iaisenses.IsFriendly(ent))
				{
					this.Friendlies.Add(ent);
				}
				if (flag)
				{
					this.Threats.Add(ent);
				}
			}
			this.All.Add(new SimpleAIMemory.SeenInfo
			{
				Entity = ent,
				Position = ent.transform.position,
				Timestamp = UnityEngine.Time.realtimeSinceStartup
			});
		}

		// Token: 0x06004643 RID: 17987 RVA: 0x001996FE File Offset: 0x001978FE
		public void SetLOS(BaseEntity ent, bool flag)
		{
			if (ent == null)
			{
				return;
			}
			if (flag)
			{
				this.LOS.Add(ent);
				return;
			}
			this.LOS.Remove(ent);
		}

		// Token: 0x06004644 RID: 17988 RVA: 0x00199728 File Offset: 0x00197928
		public bool IsLOS(BaseEntity ent)
		{
			return this.LOS.Contains(ent);
		}

		// Token: 0x06004645 RID: 17989 RVA: 0x00199736 File Offset: 0x00197936
		public bool IsPlayerKnown(BasePlayer player)
		{
			return this.Players.Contains(player);
		}

		// Token: 0x06004646 RID: 17990 RVA: 0x00199744 File Offset: 0x00197944
		public void Forget(float secondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (UnityEngine.Time.realtimeSinceStartup - this.All[i].Timestamp >= secondsOld)
				{
					BaseEntity entity = this.All[i].Entity;
					if (entity != null)
					{
						if (entity is BasePlayer)
						{
							this.Players.Remove(entity);
						}
						this.Targets.Remove(entity);
						this.Threats.Remove(entity);
						this.Friendlies.Remove(entity);
						this.LOS.Remove(entity);
					}
					this.All.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x001997FB File Offset: 0x001979FB
		public static void AddIgnorePlayer(BasePlayer player)
		{
			if (SimpleAIMemory.PlayerIgnoreList.Contains(player))
			{
				return;
			}
			SimpleAIMemory.PlayerIgnoreList.Add(player);
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x00199817 File Offset: 0x00197A17
		public static void RemoveIgnorePlayer(BasePlayer player)
		{
			SimpleAIMemory.PlayerIgnoreList.Remove(player);
		}

		// Token: 0x06004649 RID: 17993 RVA: 0x00199825 File Offset: 0x00197A25
		public static void ClearIgnoredPlayers()
		{
			SimpleAIMemory.PlayerIgnoreList.Clear();
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x00199834 File Offset: 0x00197A34
		public static string GetIgnoredPlayers()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "Name", "Steam ID" });
			foreach (BasePlayer basePlayer in SimpleAIMemory.PlayerIgnoreList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.displayName,
					basePlayer.userID.ToString()
				});
			}
			return textTable.ToString();
		}

		// Token: 0x04003F2E RID: 16174
		public static HashSet<BasePlayer> PlayerIgnoreList = new HashSet<BasePlayer>();

		// Token: 0x04003F2F RID: 16175
		public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

		// Token: 0x04003F30 RID: 16176
		public List<BaseEntity> Players = new List<BaseEntity>();

		// Token: 0x04003F31 RID: 16177
		public HashSet<BaseEntity> LOS = new HashSet<BaseEntity>();

		// Token: 0x04003F32 RID: 16178
		public List<BaseEntity> Targets = new List<BaseEntity>();

		// Token: 0x04003F33 RID: 16179
		public List<BaseEntity> Threats = new List<BaseEntity>();

		// Token: 0x04003F34 RID: 16180
		public List<BaseEntity> Friendlies = new List<BaseEntity>();

		// Token: 0x02000FB7 RID: 4023
		public struct SeenInfo
		{
			// Token: 0x04005133 RID: 20787
			public BaseEntity Entity;

			// Token: 0x04005134 RID: 20788
			public Vector3 Position;

			// Token: 0x04005135 RID: 20789
			public float Timestamp;

			// Token: 0x04005136 RID: 20790
			public float Danger;
		}
	}
}
