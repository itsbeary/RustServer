using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B4B RID: 2891
	public class Memory
	{
		// Token: 0x06004605 RID: 17925 RVA: 0x00197F38 File Offset: 0x00196138
		public Memory.SeenInfo Update(BaseEntity entity, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			return this.Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
		}

		// Token: 0x06004606 RID: 17926 RVA: 0x00197F60 File Offset: 0x00196160
		public Memory.SeenInfo Update(BaseEntity entity, Vector3 position, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			extendedInfo = default(Memory.ExtendedInfo);
			bool flag = false;
			for (int i = 0; i < this.AllExtended.Count; i++)
			{
				if (this.AllExtended[i].Entity == entity)
				{
					Memory.ExtendedInfo extendedInfo2 = this.AllExtended[i];
					extendedInfo2.Direction = direction;
					extendedInfo2.Dot = dot;
					extendedInfo2.DistanceSqr = distanceSqr;
					extendedInfo2.LineOfSight = lineOfSight;
					if (updateLastHurtUsTime)
					{
						extendedInfo2.LastHurtUsTime = lastHurtUsTime;
					}
					this.AllExtended[i] = extendedInfo2;
					extendedInfo = extendedInfo2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (updateLastHurtUsTime)
				{
					Memory.ExtendedInfo extendedInfo3 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight,
						LastHurtUsTime = lastHurtUsTime
					};
					this.AllExtended.Add(extendedInfo3);
					extendedInfo = extendedInfo3;
				}
				else
				{
					Memory.ExtendedInfo extendedInfo4 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight
					};
					this.AllExtended.Add(extendedInfo4);
					extendedInfo = extendedInfo4;
				}
			}
			return this.Update(entity, position, score);
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x001980B7 File Offset: 0x001962B7
		public Memory.SeenInfo Update(BaseEntity ent, float danger = 0f)
		{
			return this.Update(ent, ent.ServerPosition, danger);
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x001980C8 File Offset: 0x001962C8
		public Memory.SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0f)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					Memory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = position;
					seenInfo.Timestamp = Time.realtimeSinceStartup;
					seenInfo.Danger += danger;
					this.All[i] = seenInfo;
					return seenInfo;
				}
			}
			Memory.SeenInfo seenInfo2 = new Memory.SeenInfo
			{
				Entity = ent,
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = danger
			};
			this.All.Add(seenInfo2);
			this.Visible.Add(ent);
			return seenInfo2;
		}

		// Token: 0x06004609 RID: 17929 RVA: 0x00198188 File Offset: 0x00196388
		public void AddDanger(Vector3 position, float amount)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Mathf.Approximately(this.All[i].Position.x, position.x) && Mathf.Approximately(this.All[i].Position.y, position.y) && Mathf.Approximately(this.All[i].Position.z, position.z))
				{
					Memory.SeenInfo seenInfo = this.All[i];
					seenInfo.Danger = amount;
					this.All[i] = seenInfo;
					return;
				}
			}
			this.All.Add(new Memory.SeenInfo
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			});
		}

		// Token: 0x0600460A RID: 17930 RVA: 0x00198270 File Offset: 0x00196470
		public Memory.SeenInfo GetInfo(BaseEntity entity)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if (seenInfo.Entity == entity)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x0600460B RID: 17931 RVA: 0x001982DC File Offset: 0x001964DC
		public Memory.SeenInfo GetInfo(Vector3 position)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if ((seenInfo.Position - position).sqrMagnitude < 1f)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x00198354 File Offset: 0x00196554
		public Memory.ExtendedInfo GetExtendedInfo(BaseEntity entity)
		{
			foreach (Memory.ExtendedInfo extendedInfo in this.AllExtended)
			{
				if (extendedInfo.Entity == entity)
				{
					return extendedInfo;
				}
			}
			return default(Memory.ExtendedInfo);
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x001983C0 File Offset: 0x001965C0
		internal void Forget(float maxSecondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				float num = Time.realtimeSinceStartup - this.All[i].Timestamp;
				if (num > maxSecondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
						for (int j = 0; j < this.AllExtended.Count; j++)
						{
							if (this.AllExtended[j].Entity == this.All[i].Entity)
							{
								this.AllExtended.RemoveAt(j);
								break;
							}
						}
					}
					this.All.RemoveAt(i);
					i--;
				}
				else if (num > 0f)
				{
					float num2 = num / maxSecondsOld;
					if (this.All[i].Danger > 0f)
					{
						Memory.SeenInfo seenInfo = this.All[i];
						seenInfo.Danger -= num2;
						this.All[i] = seenInfo;
					}
					if (num >= 1f)
					{
						for (int k = 0; k < this.AllExtended.Count; k++)
						{
							if (this.AllExtended[k].Entity == this.All[i].Entity)
							{
								Memory.ExtendedInfo extendedInfo = this.AllExtended[k];
								extendedInfo.LineOfSight = 0;
								this.AllExtended[k] = extendedInfo;
								break;
							}
						}
					}
				}
			}
			for (int l = 0; l < this.Visible.Count; l++)
			{
				if (this.Visible[l] == null)
				{
					this.Visible.RemoveAt(l);
					l--;
				}
			}
			for (int m = 0; m < this.AllExtended.Count; m++)
			{
				if (this.AllExtended[m].Entity == null)
				{
					this.AllExtended.RemoveAt(m);
					m--;
				}
			}
		}

		// Token: 0x04003EEB RID: 16107
		public List<BaseEntity> Visible = new List<BaseEntity>();

		// Token: 0x04003EEC RID: 16108
		public List<Memory.SeenInfo> All = new List<Memory.SeenInfo>();

		// Token: 0x04003EED RID: 16109
		public List<Memory.ExtendedInfo> AllExtended = new List<Memory.ExtendedInfo>();

		// Token: 0x02000FB0 RID: 4016
		public struct SeenInfo
		{
			// Token: 0x04005111 RID: 20753
			public BaseEntity Entity;

			// Token: 0x04005112 RID: 20754
			public Vector3 Position;

			// Token: 0x04005113 RID: 20755
			public float Timestamp;

			// Token: 0x04005114 RID: 20756
			public float Danger;
		}

		// Token: 0x02000FB1 RID: 4017
		public struct ExtendedInfo
		{
			// Token: 0x04005115 RID: 20757
			public BaseEntity Entity;

			// Token: 0x04005116 RID: 20758
			public Vector3 Direction;

			// Token: 0x04005117 RID: 20759
			public float Dot;

			// Token: 0x04005118 RID: 20760
			public float DistanceSqr;

			// Token: 0x04005119 RID: 20761
			public byte LineOfSight;

			// Token: 0x0400511A RID: 20762
			public float LastHurtUsTime;
		}
	}
}
