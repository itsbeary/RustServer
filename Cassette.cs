using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000057 RID: 87
public class Cassette : global::BaseEntity, IUGCBrowserEntity, IServerFileReceiver
{
	// Token: 0x06000964 RID: 2404 RVA: 0x000591EC File Offset: 0x000573EC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Cassette.OnRpcMessage", 0))
		{
			if (rpc == 4031457637U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_MakeNewFile ");
				}
				using (TimeWarning.New("Server_MakeNewFile", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4031457637U, "Server_MakeNewFile", this, player, 1UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_MakeNewFile(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_MakeNewFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00059354 File Offset: 0x00057554
	[ServerVar]
	public static void ClearCassettes(ConsoleSystem.Arg arg)
	{
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = enumerator.Current as global::Cassette) != null && cassette.ClearSavedAudio())
				{
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted the contents of {0} cassettes", num));
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000593C8 File Offset: 0x000575C8
	[ServerVar]
	public static void ClearCassettesByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = enumerator.Current as global::Cassette) != null && cassette.CreatorSteamId == @uint)
				{
					cassette.ClearSavedAudio();
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted {0} cassettes recorded by {1}", num, @uint));
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000967 RID: 2407 RVA: 0x00059454 File Offset: 0x00057654
	// (set) Token: 0x06000968 RID: 2408 RVA: 0x0005945C File Offset: 0x0005765C
	public uint AudioId { get; private set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000969 RID: 2409 RVA: 0x00059465 File Offset: 0x00057665
	public SoundDefinition PreloadedAudio
	{
		get
		{
			return this.PreloadContent.GetSoundContent(this.preloadedAudioId, this.PreloadType);
		}
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00059480 File Offset: 0x00057680
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.cassette != null)
		{
			uint audioId = this.AudioId;
			this.AudioId = info.msg.cassette.audioId;
			this.CreatorSteamId = info.msg.cassette.creatorSteamId;
			this.preloadedAudioId = info.msg.cassette.preloadAudioId;
			if (base.isServer && info.msg.cassette.holder.IsValid)
			{
				global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(info.msg.cassette.holder);
				ICassettePlayer cassettePlayer;
				if (baseNetworkable != null && (cassettePlayer = baseNetworkable as ICassettePlayer) != null)
				{
					this.currentCassettePlayer = cassettePlayer;
				}
			}
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00059544 File Offset: 0x00057744
	public void AssignPreloadContent()
	{
		switch (this.PreloadType)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.ShortTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Medium:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.MediumTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Long:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.LongTapeContent.Length);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x000595C0 File Offset: 0x000577C0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cassette = Facepunch.Pool.Get<ProtoBuf.Cassette>();
		info.msg.cassette.audioId = this.AudioId;
		info.msg.cassette.creatorSteamId = this.CreatorSteamId;
		info.msg.cassette.preloadAudioId = this.preloadedAudioId;
		if (!this.currentCassettePlayer.IsUnityNull<ICassettePlayer>() && this.currentCassettePlayer.ToBaseEntity.IsValid())
		{
			info.msg.cassette.holder = this.currentCassettePlayer.ToBaseEntity.net.ID;
		}
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0005966C File Offset: 0x0005786C
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		ICassettePlayer cassettePlayer = this.currentCassettePlayer;
		if (cassettePlayer != null)
		{
			cassettePlayer.OnCassetteRemoved(this);
		}
		this.currentCassettePlayer = null;
		ICassettePlayer cassettePlayer2;
		if (newParent != null && (cassettePlayer2 = newParent as ICassettePlayer) != null)
		{
			base.Invoke(new Action(this.DelayedCassetteInserted), 0.1f);
			this.currentCassettePlayer = cassettePlayer2;
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000596CB File Offset: 0x000578CB
	private void DelayedCassetteInserted()
	{
		if (this.currentCassettePlayer != null)
		{
			this.currentCassettePlayer.OnCassetteInserted(this);
		}
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x000596E1 File Offset: 0x000578E1
	public void SetAudioId(uint id, ulong userId)
	{
		this.AudioId = id;
		this.CreatorSteamId = userId;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x000596F8 File Offset: 0x000578F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_MakeNewFile(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		global::HeldEntity heldEntity;
		if (base.GetParentEntity() != null && (heldEntity = base.GetParentEntity() as global::HeldEntity) != null && heldEntity.GetOwnerPlayer() != msg.player)
		{
			Debug.Log("Player mismatch!");
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		ulong num = msg.read.UInt64();
		if (!global::Cassette.IsOggValid(array, this))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		uint num2 = FileStorage.server.Store(array, FileStorage.Type.ogg, this.net.ID, 0U);
		this.SetAudioId(num2, num);
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x000597AC File Offset: 0x000579AC
	private bool ClearSavedAudio()
	{
		if (this.AudioId == 0U)
		{
			return false;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this.AudioId = 0U;
		this.CreatorSteamId = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x000597E4 File Offset: 0x000579E4
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.ClearSavedAudio();
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000973 RID: 2419 RVA: 0x000597F3 File Offset: 0x000579F3
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.AudioId <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[] { this.AudioId };
		}
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00059813 File Offset: 0x00057A13
	public void ClearContent()
	{
		this.AudioId = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000975 RID: 2421 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
	public UGCType ContentType
	{
		get
		{
			return UGCType.AudioOgg;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000976 RID: 2422 RVA: 0x00059823 File Offset: 0x00057A23
	public List<ulong> EditingHistory
	{
		get
		{
			return new List<ulong> { this.CreatorSteamId };
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000977 RID: 2423 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00059836 File Offset: 0x00057A36
	public static bool IsOggValid(byte[] data, global::Cassette c)
	{
		return global::Cassette.IsOggValid(data, c.MaxCassetteLength);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00059844 File Offset: 0x00057A44
	private static bool IsOggValid(byte[] data, float maxLength)
	{
		if (data == null)
		{
			return false;
		}
		if (global::Cassette.ByteToMegabyte(data.Length) >= global::Cassette.MaxCassetteFileSizeMB)
		{
			Debug.Log("Audio file is too large! Aborting");
			return false;
		}
		double oggLength = global::Cassette.GetOggLength(data);
		if (oggLength > (double)(maxLength * 1.2f))
		{
			Debug.Log(string.Format("Audio duration is longer than cassette limit! {0} > {1}", oggLength, maxLength * 1.2f));
			return false;
		}
		return true;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000598A7 File Offset: 0x00057AA7
	private static float ByteToMegabyte(int byteSize)
	{
		return (float)byteSize / 1024f / 1024f;
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x000598B8 File Offset: 0x00057AB8
	private static double GetOggLength(byte[] t)
	{
		int num = t.Length;
		long num2 = -1L;
		int num3 = -1;
		for (int i = num - 1 - 8 - 2 - 4; i >= 0; i--)
		{
			if (t[i] == 79 && t[i + 1] == 103 && t[i + 2] == 103 && t[i + 3] == 83)
			{
				num2 = BitConverter.ToInt64(new byte[]
				{
					t[i + 6],
					t[i + 7],
					t[i + 8],
					t[i + 9],
					t[i + 10],
					t[i + 11],
					t[i + 12],
					t[i + 13]
				}, 0);
				break;
			}
		}
		for (int j = 0; j < num - 8 - 2 - 4; j++)
		{
			if (t[j] == 118 && t[j + 1] == 111 && t[j + 2] == 114 && t[j + 3] == 98 && t[j + 4] == 105 && t[j + 5] == 115)
			{
				num3 = BitConverter.ToInt32(new byte[]
				{
					t[j + 11],
					t[j + 12],
					t[j + 13],
					t[j + 14]
				}, 0);
				break;
			}
		}
		if (RecorderTool.debugRecording)
		{
			Debug.Log(string.Format("{0} / {1}", num2, num3));
		}
		return (double)num2 / (double)num3;
	}

	// Token: 0x0400062C RID: 1580
	public float MaxCassetteLength = 15f;

	// Token: 0x0400062D RID: 1581
	[ReplicatedVar]
	public static float MaxCassetteFileSizeMB = 5f;

	// Token: 0x0400062F RID: 1583
	public ulong CreatorSteamId;

	// Token: 0x04000630 RID: 1584
	public PreloadedCassetteContent.PreloadType PreloadType;

	// Token: 0x04000631 RID: 1585
	public PreloadedCassetteContent PreloadContent;

	// Token: 0x04000632 RID: 1586
	public SoundDefinition InsertCassetteSfx;

	// Token: 0x04000633 RID: 1587
	public int ViewmodelIndex;

	// Token: 0x04000634 RID: 1588
	public Sprite HudSprite;

	// Token: 0x04000635 RID: 1589
	public int MaximumVoicemailSlots = 1;

	// Token: 0x04000636 RID: 1590
	private int preloadedAudioId;

	// Token: 0x04000637 RID: 1591
	private ICassettePlayer currentCassettePlayer;
}
