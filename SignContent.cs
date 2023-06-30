using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x020003DE RID: 990
public class SignContent : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06002227 RID: 8743 RVA: 0x000DDC0A File Offset: 0x000DBE0A
	protected override uint CrcToLoad
	{
		get
		{
			return this.textureIDs[0];
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06002228 RID: 8744 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06002229 RID: 8745 RVA: 0x0000441C File Offset: 0x0000261C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x000DDC14 File Offset: 0x000DBE14
	public void CopyInfoFromSign(ISignage s, IUGCBrowserEntity b)
	{
		uint[] textureCRCs = s.GetTextureCRCs();
		this.textureIDs = new uint[textureCRCs.Length];
		textureCRCs.CopyTo(this.textureIDs, 0);
		this.editHistory.Clear();
		foreach (ulong num in b.EditingHistory)
		{
			this.editHistory.Add(num);
		}
		FileStorage.server.ReassignEntityId(s.NetworkID, this.net.ID);
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000DDCB4 File Offset: 0x000DBEB4
	public void CopyInfoToSign(ISignage s, IUGCBrowserEntity b)
	{
		FileStorage.server.ReassignEntityId(this.net.ID, s.NetworkID);
		s.SetTextureCRCs(this.textureIDs);
		b.EditingHistory.Clear();
		foreach (ulong num in this.editHistory)
		{
			b.EditingHistory.Add(num);
		}
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000DDD40 File Offset: 0x000DBF40
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.paintableSign == null)
		{
			info.msg.paintableSign = Pool.Get<PaintableSign>();
		}
		info.msg.paintableSign.crcs = Pool.GetList<uint>();
		foreach (uint num in this.textureIDs)
		{
			info.msg.paintableSign.crcs.Add(num);
		}
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000DDDB5 File Offset: 0x000DBFB5
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		FileStorage.server.RemoveAllByEntity(this.net.ID);
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000DDDD4 File Offset: 0x000DBFD4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintableSign != null)
		{
			this.textureIDs = new uint[info.msg.paintableSign.crcs.Count];
			for (int i = 0; i < info.msg.paintableSign.crcs.Count; i++)
			{
				this.textureIDs[i] = info.msg.paintableSign.crcs[i];
			}
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x0600222F RID: 8751 RVA: 0x000DDE53 File Offset: 0x000DC053
	public uint[] GetContentCRCs
	{
		get
		{
			return this.textureIDs;
		}
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x00003384 File Offset: 0x00001584
	public void ClearContent()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06002231 RID: 8753 RVA: 0x000DDE5B File Offset: 0x000DC05B
	public FileStorage.Type FileType
	{
		get
		{
			return this.StorageType;
		}
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06002232 RID: 8754 RVA: 0x000DDE63 File Offset: 0x000DC063
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06002233 RID: 8755 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x04001A6B RID: 6763
	private uint[] textureIDs = new uint[1];

	// Token: 0x04001A6C RID: 6764
	private List<ulong> editHistory = new List<ulong>();
}
