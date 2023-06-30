using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Models;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200039C RID: 924
public class BoomBox : EntityComponent<global::BaseEntity>, INotifyLOD
{
	// Token: 0x060020A5 RID: 8357 RVA: 0x000D8290 File Offset: 0x000D6490
	[ServerVar]
	public static void ClearRadioByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			DeployableBoomBox deployableBoomBox;
			HeldBoomBox heldBoomBox;
			if ((deployableBoomBox = baseNetworkable as DeployableBoomBox) != null)
			{
				if (deployableBoomBox.ClearRadioByUserId(@uint))
				{
					num++;
				}
			}
			else if ((heldBoomBox = baseNetworkable as HeldBoomBox) != null && heldBoomBox.ClearRadioByUserId(@uint))
			{
				num++;
			}
		}
		arg.ReplyWith(string.Format("Stopped and cleared saved URL of {0} boom boxes", num));
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000D832C File Offset: 0x000D652C
	public static void LoadStations()
	{
		if (global::BoomBox.ValidStations != null)
		{
			return;
		}
		global::BoomBox.ValidStations = global::BoomBox.GetStationData() ?? new Dictionary<string, string>();
		global::BoomBox.ParseServerUrlList();
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000D8350 File Offset: 0x000D6550
	private static Dictionary<string, string> GetStationData()
	{
		Facepunch.Models.Manifest manifest = Facepunch.Application.Manifest;
		JObject jobject = ((manifest != null) ? manifest.Metadata : null);
		JArray jarray;
		if ((jarray = ((jobject != null) ? jobject["RadioStations"] : null) as JArray) != null && jarray.Count > 0)
		{
			string[] array = new string[2];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in jarray.Values<string>())
			{
				array = text.Split(new char[] { ',' });
				if (!dictionary.ContainsKey(array[0]))
				{
					dictionary.Add(array[0], array[1]);
				}
			}
			return dictionary;
		}
		return null;
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000D8404 File Offset: 0x000D6604
	private static bool IsStationValid(string url)
	{
		global::BoomBox.ParseServerUrlList();
		return (global::BoomBox.ValidStations != null && global::BoomBox.ValidStations.ContainsValue(url)) || (global::BoomBox.ServerValidStations != null && global::BoomBox.ServerValidStations.ContainsValue(url));
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000D8438 File Offset: 0x000D6638
	public static void ParseServerUrlList()
	{
		if (global::BoomBox.ServerValidStations == null)
		{
			global::BoomBox.ServerValidStations = new Dictionary<string, string>();
		}
		if (global::BoomBox.lastParsedServerList == global::BoomBox.ServerUrlList)
		{
			return;
		}
		global::BoomBox.ServerValidStations.Clear();
		if (!string.IsNullOrEmpty(global::BoomBox.ServerUrlList))
		{
			string[] array = global::BoomBox.ServerUrlList.Split(new char[] { ',' });
			if (array.Length % 2 != 0)
			{
				Debug.Log("Invalid number of stations in BoomBox.ServerUrlList, ensure you always have a name and a url");
				return;
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				if (global::BoomBox.ServerValidStations.ContainsKey(array[i]))
				{
					Debug.Log("Duplicate station name detected in BoomBox.ServerUrlList, all station names must be unique: " + array[i]);
				}
				else
				{
					global::BoomBox.ServerValidStations.Add(array[i], array[i + 1]);
				}
			}
		}
		global::BoomBox.lastParsedServerList = global::BoomBox.ServerUrlList;
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x060020AA RID: 8362 RVA: 0x000D84F3 File Offset: 0x000D66F3
	// (set) Token: 0x060020AB RID: 8363 RVA: 0x000D84FB File Offset: 0x000D66FB
	public string CurrentRadioIp { get; private set; } = "rustradio.facepunch.com";

	// Token: 0x060020AC RID: 8364 RVA: 0x000D8504 File Offset: 0x000D6704
	public void Server_UpdateRadioIP(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		if (global::BoomBox.IsStationValid(text))
		{
			if (msg.player != null)
			{
				ulong userID = msg.player.userID;
				this.AssignedRadioBy = userID;
			}
			this.CurrentRadioIp = text;
			base.baseEntity.ClientRPC<string>(null, "OnRadioIPChanged", this.CurrentRadioIp);
			if (this.IsOn())
			{
				this.ServerTogglePlay(false);
			}
		}
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000D8578 File Offset: 0x000D6778
	public void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (info.msg.boomBox == null)
		{
			info.msg.boomBox = Facepunch.Pool.Get<ProtoBuf.BoomBox>();
		}
		info.msg.boomBox.radioIp = this.CurrentRadioIp;
		info.msg.boomBox.assignedRadioBy = this.AssignedRadioBy;
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000D85CE File Offset: 0x000D67CE
	public bool ClearRadioByUserId(ulong id)
	{
		if (this.AssignedRadioBy == id)
		{
			this.CurrentRadioIp = string.Empty;
			this.AssignedRadioBy = 0UL;
			if (this.HasFlag(global::BaseEntity.Flags.On))
			{
				this.ServerTogglePlay(false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000D85FF File Offset: 0x000D67FF
	public void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.boomBox != null)
		{
			this.CurrentRadioIp = info.msg.boomBox.radioIp;
			this.AssignedRadioBy = info.msg.boomBox.assignedRadioBy;
		}
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x060020B0 RID: 8368 RVA: 0x000D863A File Offset: 0x000D683A
	public global::BaseEntity BaseEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000D8644 File Offset: 0x000D6844
	public void ServerTogglePlay(global::BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		bool flag = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(flag);
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000D8670 File Offset: 0x000D6870
	private void DeductCondition()
	{
		Action<float> hurtCallback = this.HurtCallback;
		if (hurtCallback == null)
		{
			return;
		}
		hurtCallback(this.ConditionLossRate * ConVar.Decay.scale);
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000D8690 File Offset: 0x000D6890
	public void ServerTogglePlay(bool play)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		this.SetFlag(global::BaseEntity.Flags.On, play);
		global::IOEntity ioentity;
		if ((ioentity = base.baseEntity as global::IOEntity) != null)
		{
			ioentity.SendChangedToRoot(true);
			ioentity.MarkDirtyForceUpdateOutputs();
		}
		if (play && !base.IsInvoking(new Action(this.DeductCondition)) && this.ConditionLossRate > 0f)
		{
			base.InvokeRepeating(new Action(this.DeductCondition), 1f, 1f);
			return;
		}
		if (base.IsInvoking(new Action(this.DeductCondition)))
		{
			base.CancelInvoke(new Action(this.DeductCondition));
		}
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000D8738 File Offset: 0x000D6938
	public void OnCassetteInserted(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, true);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000D878F File Offset: 0x000D698F
	public void OnCassetteRemoved(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, false);
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000D87C4 File Offset: 0x000D69C4
	private bool IsPowered()
	{
		return !(base.baseEntity == null) && (base.baseEntity.HasFlag(global::BaseEntity.Flags.Reserved8) || base.baseEntity is HeldBoomBox);
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000D87F8 File Offset: 0x000D69F8
	private bool IsOn()
	{
		return !(base.baseEntity == null) && base.baseEntity.IsOn();
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000D8815 File Offset: 0x000D6A15
	private bool HasFlag(global::BaseEntity.Flags f)
	{
		return !(base.baseEntity == null) && base.baseEntity.HasFlag(f);
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000D8833 File Offset: 0x000D6A33
	private void SetFlag(global::BaseEntity.Flags f, bool state)
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.SetFlag(f, state, false, true);
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x060020BA RID: 8378 RVA: 0x000D8852 File Offset: 0x000D6A52
	private bool isClient
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isClient;
		}
	}

	// Token: 0x0400198D RID: 6541
	public static Dictionary<string, string> ValidStations;

	// Token: 0x0400198E RID: 6542
	public static Dictionary<string, string> ServerValidStations;

	// Token: 0x0400198F RID: 6543
	[ReplicatedVar(Saved = true, Help = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc", ShowInAdminUI = true)]
	public static string ServerUrlList = string.Empty;

	// Token: 0x04001990 RID: 6544
	private static string lastParsedServerList;

	// Token: 0x04001991 RID: 6545
	public ShoutcastStreamer ShoutcastStreamer;

	// Token: 0x04001992 RID: 6546
	public GameObjectRef RadioIpDialog;

	// Token: 0x04001994 RID: 6548
	public ulong AssignedRadioBy;

	// Token: 0x04001995 RID: 6549
	public AudioSource SoundSource;

	// Token: 0x04001996 RID: 6550
	public float ConditionLossRate = 0.25f;

	// Token: 0x04001997 RID: 6551
	public ItemDefinition[] ValidCassettes;

	// Token: 0x04001998 RID: 6552
	public SoundDefinition PlaySfx;

	// Token: 0x04001999 RID: 6553
	public SoundDefinition StopSfx;

	// Token: 0x0400199A RID: 6554
	public const global::BaseEntity.Flags HasCassette = global::BaseEntity.Flags.Reserved1;

	// Token: 0x0400199B RID: 6555
	[ServerVar(Saved = true)]
	public static int BacktrackLength = 30;

	// Token: 0x0400199C RID: 6556
	public Action<float> HurtCallback;
}
