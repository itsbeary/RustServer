using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B09 RID: 2825
	public class EventRecord : Pool.IPooled
	{
		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x060044DC RID: 17628 RVA: 0x001932FD File Offset: 0x001914FD
		// (set) Token: 0x060044DD RID: 17629 RVA: 0x00193305 File Offset: 0x00191505
		public string EventType { get; private set; }

		// Token: 0x060044DF RID: 17631 RVA: 0x00193321 File Offset: 0x00191521
		public void EnterPool()
		{
			this.Timestamp = default(DateTime);
			this.EventType = null;
			this.IsServer = false;
			this.Data.Clear();
		}

		// Token: 0x060044E0 RID: 17632 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x060044E1 RID: 17633 RVA: 0x00193348 File Offset: 0x00191548
		public static EventRecord New(string type, bool isServer = true)
		{
			EventRecord eventRecord = Pool.Get<EventRecord>();
			eventRecord.EventType = type;
			eventRecord.AddField("type", type);
			eventRecord.AddField("guid", Guid.NewGuid());
			eventRecord.IsServer = isServer;
			if (isServer)
			{
				eventRecord.AddField("wipe_id", SaveRestore.WipeId);
			}
			eventRecord.Timestamp = DateTime.UtcNow;
			return eventRecord;
		}

		// Token: 0x060044E2 RID: 17634 RVA: 0x001933A8 File Offset: 0x001915A8
		public EventRecord AddObject(string key, object data)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = JsonConvert.SerializeObject(data),
				IsObject = true
			});
			return this;
		}

		// Token: 0x060044E3 RID: 17635 RVA: 0x001933DF File Offset: 0x001915DF
		public EventRecord SetTimestamp(DateTime timestamp)
		{
			this.Timestamp = timestamp;
			return this;
		}

		// Token: 0x060044E4 RID: 17636 RVA: 0x001933EC File Offset: 0x001915EC
		public EventRecord AddField(string key, bool value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = (value ? "true" : "false")
			});
			return this;
		}

		// Token: 0x060044E5 RID: 17637 RVA: 0x00193424 File Offset: 0x00191624
		public EventRecord AddField(string key, string value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = value
			});
			return this;
		}

		// Token: 0x060044E6 RID: 17638 RVA: 0x00193450 File Offset: 0x00191650
		public EventRecord AddField(string key, int value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)value)
			});
			return this;
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x00193480 File Offset: 0x00191680
		public EventRecord AddField(string key, uint value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)((ulong)value))
			});
			return this;
		}

		// Token: 0x060044E8 RID: 17640 RVA: 0x001934B0 File Offset: 0x001916B0
		public EventRecord AddField(string key, ulong value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)value)
			});
			return this;
		}

		// Token: 0x060044E9 RID: 17641 RVA: 0x001934E0 File Offset: 0x001916E0
		public EventRecord AddField(string key, long value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?(value)
			});
			return this;
		}

		// Token: 0x060044EA RID: 17642 RVA: 0x00193510 File Offset: 0x00191710
		public EventRecord AddField(string key, float value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?((double)value)
			});
			return this;
		}

		// Token: 0x060044EB RID: 17643 RVA: 0x00193540 File Offset: 0x00191740
		public EventRecord AddField(string key, double value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?(value)
			});
			return this;
		}

		// Token: 0x060044EC RID: 17644 RVA: 0x00193570 File Offset: 0x00191770
		public EventRecord AddField(string key, TimeSpan value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?(value.TotalSeconds)
			});
			return this;
		}

		// Token: 0x060044ED RID: 17645 RVA: 0x001935A8 File Offset: 0x001917A8
		public EventRecord AddField(string key, Guid value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Guid = new Guid?(value)
			});
			return this;
		}

		// Token: 0x060044EE RID: 17646 RVA: 0x001935D8 File Offset: 0x001917D8
		public EventRecord AddField(string key, Vector3 value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Vector = new Vector3?(value)
			});
			return this;
		}

		// Token: 0x060044EF RID: 17647 RVA: 0x00193608 File Offset: 0x00191808
		public EventRecord AddField(string key, BaseEntity entity)
		{
			if (((entity != null) ? entity.net : null) == null)
			{
				return this;
			}
			BasePlayer basePlayer;
			EventRecordField eventRecordField;
			if ((basePlayer = entity as BasePlayer) != null && !basePlayer.IsNpc && !basePlayer.IsBot)
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(basePlayer.userID);
				List<EventRecordField> data = this.Data;
				eventRecordField = new EventRecordField(key, "_userid")
				{
					String = userWipeId
				};
				data.Add(eventRecordField);
				if (basePlayer.isMounted)
				{
					this.AddField(key + "_mounted", basePlayer.GetMounted());
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					List<EventRecordField> data2 = this.Data;
					eventRecordField = new EventRecordField(key, "_admin")
					{
						String = "true"
					};
					data2.Add(eventRecordField);
				}
			}
			BaseProjectile baseProjectile;
			if ((baseProjectile = entity as BaseProjectile) != null)
			{
				Item item = baseProjectile.GetItem();
				if (item != null)
				{
					ItemContainer contents = item.contents;
					int? num;
					if (contents == null)
					{
						num = null;
					}
					else
					{
						List<Item> itemList = contents.itemList;
						num = ((itemList != null) ? new int?(itemList.Count) : null);
					}
					if ((num ?? 0) > 0)
					{
						List<string> list = Pool.GetList<string>();
						foreach (Item item2 in item.contents.itemList)
						{
							list.Add(item2.info.shortname);
						}
						this.AddObject(key + "_inventory", list);
						Pool.FreeList<string>(ref list);
					}
				}
			}
			BuildingBlock buildingBlock;
			if ((buildingBlock = entity as BuildingBlock) != null)
			{
				List<EventRecordField> data3 = this.Data;
				eventRecordField = new EventRecordField(key, "_grade")
				{
					Number = new long?((long)buildingBlock.grade)
				};
				data3.Add(eventRecordField);
			}
			List<EventRecordField> data4 = this.Data;
			eventRecordField = new EventRecordField(key, "_prefab")
			{
				String = entity.ShortPrefabName
			};
			data4.Add(eventRecordField);
			List<EventRecordField> data5 = this.Data;
			eventRecordField = new EventRecordField(key, "_pos")
			{
				Vector = new Vector3?(entity.transform.position)
			};
			data5.Add(eventRecordField);
			List<EventRecordField> data6 = this.Data;
			eventRecordField = new EventRecordField(key, "_rot")
			{
				Vector = new Vector3?(entity.transform.rotation.eulerAngles)
			};
			data6.Add(eventRecordField);
			List<EventRecordField> data7 = this.Data;
			eventRecordField = new EventRecordField(key, "_id")
			{
				Number = new long?((long)entity.net.ID.Value)
			};
			data7.Add(eventRecordField);
			return this;
		}

		// Token: 0x060044F0 RID: 17648 RVA: 0x001938BC File Offset: 0x00191ABC
		public EventRecord AddField(string key, Item item)
		{
			List<EventRecordField> data = this.Data;
			EventRecordField eventRecordField = new EventRecordField(key, "_name")
			{
				String = item.info.shortname
			};
			data.Add(eventRecordField);
			List<EventRecordField> data2 = this.Data;
			eventRecordField = new EventRecordField(key, "_amount")
			{
				Number = new long?((long)item.amount)
			};
			data2.Add(eventRecordField);
			List<EventRecordField> data3 = this.Data;
			eventRecordField = new EventRecordField(key, "_skin")
			{
				Number = new long?((long)item.skin)
			};
			data3.Add(eventRecordField);
			List<EventRecordField> data4 = this.Data;
			eventRecordField = new EventRecordField(key, "_condition")
			{
				Float = new double?((double)item.conditionNormalized)
			};
			data4.Add(eventRecordField);
			return this;
		}

		// Token: 0x060044F1 RID: 17649 RVA: 0x00193978 File Offset: 0x00191B78
		public void Submit()
		{
			if (this.IsServer)
			{
				if (Analytics.StatsBlacklist != null && Analytics.StatsBlacklist.Contains(this.EventType))
				{
					EventRecord eventRecord = this;
					Pool.Free<EventRecord>(ref eventRecord);
					return;
				}
				Analytics.AzureWebInterface.server.EnqueueEvent(this);
			}
		}

		// Token: 0x04003D4B RID: 15691
		public DateTime Timestamp;

		// Token: 0x04003D4C RID: 15692
		[NonSerialized]
		public bool IsServer;

		// Token: 0x04003D4E RID: 15694
		public List<EventRecordField> Data = new List<EventRecordField>();
	}
}
