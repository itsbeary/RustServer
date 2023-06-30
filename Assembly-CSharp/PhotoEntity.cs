using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;

// Token: 0x0200017D RID: 381
public class PhotoEntity : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060017BF RID: 6079 RVA: 0x000B3B6D File Offset: 0x000B1D6D
	// (set) Token: 0x060017C0 RID: 6080 RVA: 0x000B3B75 File Offset: 0x000B1D75
	public ulong PhotographerSteamId { get; private set; }

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060017C1 RID: 6081 RVA: 0x000B3B7E File Offset: 0x000B1D7E
	// (set) Token: 0x060017C2 RID: 6082 RVA: 0x000B3B86 File Offset: 0x000B1D86
	public uint ImageCrc { get; private set; }

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060017C3 RID: 6083 RVA: 0x000B3B8F File Offset: 0x000B1D8F
	protected override uint CrcToLoad
	{
		get
		{
			return this.ImageCrc;
		}
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x000B3B98 File Offset: 0x000B1D98
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.photo != null)
		{
			this.PhotographerSteamId = info.msg.photo.photographerSteamId;
			this.ImageCrc = info.msg.photo.imageCrc;
		}
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x000B3BE8 File Offset: 0x000B1DE8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.photo = Pool.Get<Photo>();
		info.msg.photo.photographerSteamId = this.PhotographerSteamId;
		info.msg.photo.imageCrc = this.ImageCrc;
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x000B3C38 File Offset: 0x000B1E38
	public void SetImageData(ulong steamId, byte[] data)
	{
		this.ImageCrc = FileStorage.server.Store(data, FileStorage.Type.jpg, this.net.ID, 0U);
		this.PhotographerSteamId = steamId;
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x000825D0 File Offset: 0x000807D0
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060017C8 RID: 6088 RVA: 0x000B3C5F File Offset: 0x000B1E5F
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.ImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[] { this.ImageCrc };
		}
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x000B3C7F File Offset: 0x000B1E7F
	public void ClearContent()
	{
		this.ImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060017CA RID: 6090 RVA: 0x00007A44 File Offset: 0x00005C44
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060017CB RID: 6091 RVA: 0x000B3C8F File Offset: 0x000B1E8F
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.PhotographerSteamId <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong> { this.PhotographerSteamId };
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060017CC RID: 6092 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}
}
