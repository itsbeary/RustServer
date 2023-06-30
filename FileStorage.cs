using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using UnityEngine.Assertions;

// Token: 0x02000750 RID: 1872
public class FileStorage : IDisposable
{
	// Token: 0x06003443 RID: 13379 RVA: 0x00142DDC File Offset: 0x00140FDC
	protected FileStorage(string name, bool server)
	{
		if (server)
		{
			string text = Server.rootFolder + "/" + name + ".db";
			this.db = new Database();
			this.db.Open(text, true);
			if (!this.db.TableExists("data"))
			{
				this.db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )");
				this.db.Execute("CREATE INDEX IF NOT EXISTS entindex ON data ( entid )");
			}
		}
	}

	// Token: 0x06003444 RID: 13380 RVA: 0x00142E70 File Offset: 0x00141070
	~FileStorage()
	{
		this.Dispose();
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x00142E9C File Offset: 0x0014109C
	public void Dispose()
	{
		if (this.db != null)
		{
			this.db.Close();
			this.db = null;
		}
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x00142EB8 File Offset: 0x001410B8
	private uint GetCRC(byte[] data, FileStorage.Type type)
	{
		uint crc32Result;
		using (TimeWarning.New("FileStorage.GetCRC", 0))
		{
			this.crc.Reset();
			this.crc.SlurpBlock(data, 0, data.Length);
			this.crc.UpdateCRC((byte)type);
			crc32Result = (uint)this.crc.Crc32Result;
		}
		return crc32Result;
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x00142F24 File Offset: 0x00141124
	public uint Store(byte[] data, FileStorage.Type type, NetworkableId entityID, uint numID = 0U)
	{
		uint num2;
		using (TimeWarning.New("FileStorage.Store", 0))
		{
			uint num = this.GetCRC(data, type);
			if (this.db != null)
			{
				this.db.Execute<int, byte[], long, int, int>("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", (int)num, data, (long)entityID.Value, (int)type, (int)numID);
			}
			this._cache.Remove(num);
			this._cache.Add(num, new FileStorage.CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			num2 = num;
		}
		return num2;
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x00142FBC File Offset: 0x001411BC
	public byte[] Get(uint crc, FileStorage.Type type, NetworkableId entityID, uint numID = 0U)
	{
		byte[] array;
		using (TimeWarning.New("FileStorage.Get", 0))
		{
			FileStorage.CacheData cacheData;
			if (this._cache.TryGetValue(crc, out cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				array = cacheData.data;
			}
			else if (this.db == null)
			{
				array = null;
			}
			else
			{
				byte[] array2 = this.db.QueryBlob<int, int, long, int>("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ? LIMIT 1", (int)crc, (int)type, (long)entityID.Value, (int)numID);
				if (array2 == null)
				{
					array = null;
				}
				else
				{
					this._cache.Remove(crc);
					this._cache.Add(crc, new FileStorage.CacheData
					{
						data = array2,
						entityID = entityID,
						numID = 0U
					});
					array = array2;
				}
			}
		}
		return array;
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x00143080 File Offset: 0x00141280
	public void Remove(uint crc, FileStorage.Type type, NetworkableId entityID)
	{
		using (TimeWarning.New("FileStorage.Remove", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, long>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", (int)crc, (int)type, (long)entityID.Value);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x001430E4 File Offset: 0x001412E4
	public void RemoveExact(uint crc, FileStorage.Type type, NetworkableId entityID, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveExact", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, long, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ?", (int)crc, (int)type, (long)entityID.Value, (int)numid);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x00143148 File Offset: 0x00141348
	public void RemoveEntityNum(NetworkableId entityid, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveEntityNum", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long, int>("DELETE FROM data WHERE entid = ? AND part = ?", (long)entityid.Value, (int)numid);
			}
			IEnumerable<KeyValuePair<uint, FileStorage.CacheData>> cache = this._cache;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> <>9__0;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (KeyValuePair<uint, FileStorage.CacheData> x) => x.Value.entityID == entityid && x.Value.numID == numid);
			}
			foreach (uint num in (from x in cache.Where(func)
				select x.Key).ToArray<uint>())
			{
				this._cache.Remove(num);
			}
		}
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x00143230 File Offset: 0x00141430
	internal void RemoveAllByEntity(NetworkableId entityid)
	{
		using (TimeWarning.New("FileStorage.RemoveAllByEntity", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long>("DELETE FROM data WHERE entid = ?", (long)entityid.Value);
			}
		}
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x00143284 File Offset: 0x00141484
	public void ReassignEntityId(NetworkableId oldId, NetworkableId newId)
	{
		using (TimeWarning.New("FileStorage.ReassignEntityId", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long, long>("UPDATE data SET entid = ? WHERE entid = ?", (long)newId.Value, (long)oldId.Value);
			}
		}
	}

	// Token: 0x04002AB3 RID: 10931
	private Database db;

	// Token: 0x04002AB4 RID: 10932
	private CRC32 crc = new CRC32();

	// Token: 0x04002AB5 RID: 10933
	private MruDictionary<uint, FileStorage.CacheData> _cache = new MruDictionary<uint, FileStorage.CacheData>(1000, null);

	// Token: 0x04002AB6 RID: 10934
	public static FileStorage server = new FileStorage("sv.files." + 239, true);

	// Token: 0x02000E69 RID: 3689
	private class CacheData
	{
		// Token: 0x04004BC5 RID: 19397
		public byte[] data;

		// Token: 0x04004BC6 RID: 19398
		public NetworkableId entityID;

		// Token: 0x04004BC7 RID: 19399
		public uint numID;
	}

	// Token: 0x02000E6A RID: 3690
	public enum Type
	{
		// Token: 0x04004BC9 RID: 19401
		png,
		// Token: 0x04004BCA RID: 19402
		jpg,
		// Token: 0x04004BCB RID: 19403
		ogg
	}
}
