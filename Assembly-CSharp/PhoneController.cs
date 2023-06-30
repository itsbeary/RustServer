using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class PhoneController : EntityComponent<global::BaseEntity>
{
	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x060020D9 RID: 8409 RVA: 0x000D8A4D File Offset: 0x000D6C4D
	// (set) Token: 0x060020DA RID: 8410 RVA: 0x000D8A55 File Offset: 0x000D6C55
	public global::Telephone.CallState serverState { get; set; }

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x060020DB RID: 8411 RVA: 0x000D8A60 File Offset: 0x000D6C60
	public uint AnsweringMessageId
	{
		get
		{
			global::Telephone telephone;
			if ((telephone = base.baseEntity as global::Telephone) == null)
			{
				return 0U;
			}
			return telephone.AnsweringMessageId;
		}
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000D8A84 File Offset: 0x000D6C84
	public void ServerInit()
	{
		if (this.PhoneNumber == 0 && !Rust.Application.isLoadingSave)
		{
			this.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
			if (this.AppendGridToName & !string.IsNullOrEmpty(this.PhoneName))
			{
				this.PhoneName = this.PhoneName + " " + PhoneController.PositionToGridCoord(base.transform.position);
			}
			TelephoneManager.RegisterTelephone(this, false);
		}
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000D8AEF File Offset: 0x000D6CEF
	public void PostServerLoad()
	{
		this.currentPlayer = null;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		TelephoneManager.RegisterTelephone(this, false);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000D8B12 File Offset: 0x000D6D12
	public void DoServerDestroy()
	{
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000D8B1A File Offset: 0x000D6D1A
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.ClearCurrentUser();
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000D8B22 File Offset: 0x000D6D22
	public void ClearCurrentUser()
	{
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
			this.currentPlayer = null;
		}
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000D8B58 File Offset: 0x000D6D58
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.currentPlayer == player)
		{
			return;
		}
		this.UpdateServerPlayer(player);
		if (this.serverState == global::Telephone.CallState.Dialing || this.serverState == global::Telephone.CallState.Ringing || this.serverState == global::Telephone.CallState.InProcess)
		{
			this.ServerHangUp(default(global::BaseEntity.RPCMessage));
		}
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000D8BAC File Offset: 0x000D6DAC
	private void UpdateServerPlayer(global::BasePlayer newPlayer)
	{
		if (this.currentPlayer == newPlayer)
		{
			return;
		}
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
		}
		this.currentPlayer = newPlayer;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, this.currentPlayer != null, false, true);
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(this);
		}
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000D8C24 File Offset: 0x000D6E24
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int num = msg.read.Int32();
		this.CallPhone(num);
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000D8C58 File Offset: 0x000D6E58
	public void CallPhone(int number)
	{
		if (number == this.PhoneNumber)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.CallSelf);
			return;
		}
		if (TelephoneManager.GetCurrentActiveCalls() + 1 > TelephoneManager.MaxConcurrentCalls)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.NetworkBusy);
			return;
		}
		PhoneController telephone = TelephoneManager.GetTelephone(number);
		if (!(telephone != null))
		{
			this.OnDialFailed(global::Telephone.DialFailReason.WrongNumber);
			return;
		}
		if (telephone.serverState == global::Telephone.CallState.Idle && telephone.CanReceiveCall())
		{
			this.SetPhoneState(global::Telephone.CallState.Dialing);
			this.lastDialedNumber = number;
			this.activeCallTo = telephone;
			this.activeCallTo.ReceiveCallFrom(this);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.Engaged);
		telephone.OnIncomingCallWhileBusy();
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000D8CE3 File Offset: 0x000D6EE3
	private bool CanReceiveCall()
	{
		return (!this.RequirePower || this.IsPowered()) && (!this.RequireParent || base.baseEntity.HasParent());
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000D8D10 File Offset: 0x000D6F10
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
		if (this.activeCallTo == null)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		this.UpdateServerPlayer(player);
		this.BeginCall();
		this.activeCallTo.BeginCall();
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000D8D71 File Offset: 0x000D6F71
	public void ReceiveCallFrom(PhoneController t)
	{
		this.activeCallTo = t;
		this.SetPhoneState(global::Telephone.CallState.Ringing);
		base.Invoke(new Action(this.TimeOutDialing), this.CallWaitingTime);
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000D8D99 File Offset: 0x000D6F99
	private void TimeOutDialing()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.ServerPlayAnsweringMessage(this);
		}
		this.SetPhoneState(global::Telephone.CallState.Idle);
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000D8DBC File Offset: 0x000D6FBC
	public void OnDialFailed(global::Telephone.DialFailReason reason)
	{
		this.SetPhoneState(global::Telephone.CallState.Idle);
		base.baseEntity.ClientRPC<int>(null, "ClientOnDialFailed", (int)reason);
		this.activeCallTo = null;
		if (base.IsInvoking(new Action(this.TimeOutCall)))
		{
			base.CancelInvoke(new Action(this.TimeOutCall));
		}
		if (base.IsInvoking(new Action(this.TriggerTimeOut)))
		{
			base.CancelInvoke(new Action(this.TriggerTimeOut));
		}
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000D8E5C File Offset: 0x000D705C
	public void ServerPlayAnsweringMessage(PhoneController fromPhone)
	{
		NetworkableId networkableId = default(NetworkableId);
		uint num = 0U;
		uint num2 = 0U;
		if (this.activeCallTo != null && this.activeCallTo.cachedCassette != null)
		{
			networkableId = this.activeCallTo.cachedCassette.net.ID;
			num = this.activeCallTo.cachedCassette.AudioId;
			if (num == 0U)
			{
				num2 = this.activeCallTo.cachedCassette.PreloadContent.GetSoundContentId(this.activeCallTo.cachedCassette.PreloadedAudio);
			}
		}
		if (networkableId.IsValid)
		{
			base.baseEntity.ClientRPC<NetworkableId, uint, uint, int, int>(null, "ClientPlayAnsweringMessage", networkableId, num, num2, fromPhone.HasVoicemailSlot() ? 1 : 0, this.activeCallTo.PhoneNumber);
			base.Invoke(new Action(this.TriggerTimeOut), this.activeCallTo.cachedCassette.MaxCassetteLength);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000D8F44 File Offset: 0x000D7144
	private void TriggerTimeOut()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000D8F50 File Offset: 0x000D7150
	public void SetPhoneStateWithPlayer(global::Telephone.CallState state)
	{
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		MobilePhone mobilePhone;
		if ((mobilePhone = base.baseEntity as MobilePhone) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000D8FB0 File Offset: 0x000D71B0
	private void SetPhoneState(global::Telephone.CallState state)
	{
		if (state == global::Telephone.CallState.Idle && this.currentPlayer == null)
		{
			base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		global::Telephone telephone;
		if ((telephone = base.baseEntity as global::Telephone) != null)
		{
			telephone.MarkDirtyForceUpdateOutputs();
		}
		MobilePhone mobilePhone;
		if ((mobilePhone = base.baseEntity as MobilePhone) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000D904C File Offset: 0x000D724C
	public void BeginCall()
	{
		if (this.IsMobile && this.activeCallTo != null && !this.activeCallTo.RequirePower)
		{
			this.currentPlayer != null;
		}
		this.SetPhoneStateWithPlayer(global::Telephone.CallState.InProcess);
		base.Invoke(new Action(this.TimeOutCall), (float)TelephoneManager.MaxCallLength);
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000D90A8 File Offset: 0x000D72A8
	public void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		this.ServerHangUp();
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000D90C4 File Offset: 0x000D72C4
	public void ServerHangUp()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
		this.SelfHangUp();
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x000D90E5 File Offset: 0x000D72E5
	private void SelfHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.SelfHangUp);
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000D90EE File Offset: 0x000D72EE
	private void RemoteHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.RemoteHangUp);
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x000D90F7 File Offset: 0x000D72F7
	private void TimeOutCall()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimeOutDuringCall);
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000D9100 File Offset: 0x000D7300
	public void OnReceivedVoiceFromUser(byte[] data)
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.OnReceivedDataFromConnectedPhone(data);
		}
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000D911C File Offset: 0x000D731C
	public void OnReceivedDataFromConnectedPhone(byte[] data)
	{
		base.baseEntity.ClientRPCEx<int, byte[]>(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 15f))
		{
			priority = Priority.Immediate
		}, null, "OnReceivedVoice", data.Length, data);
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000D9163 File Offset: 0x000D7363
	public void OnIncomingCallWhileBusy()
	{
		base.baseEntity.ClientRPC(null, "OnIncomingCallDuringCall");
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000D9176 File Offset: 0x000D7376
	public void DestroyShared()
	{
		if (this.isServer && this.serverState != global::Telephone.CallState.Idle && this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000D91A4 File Offset: 0x000D73A4
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		string text = msg.read.String(256);
		if (text.Length > 20)
		{
			text = text.Substring(0, 20);
		}
		this.PhoneName = text;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000D9200 File Offset: 0x000D7400
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int num = msg.read.Int32();
		using (PhoneDirectory phoneDirectory = Pool.Get<PhoneDirectory>())
		{
			TelephoneManager.GetPhoneDirectory(this.PhoneNumber, num, 12, phoneDirectory);
			base.baseEntity.ClientRPC<PhoneDirectory>(null, "ReceivePhoneDirectory", phoneDirectory);
		}
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000D9270 File Offset: 0x000D7470
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		if (this.savedNumbers == null)
		{
			this.savedNumbers = Pool.Get<PhoneDirectory>();
		}
		if (this.savedNumbers.entries == null)
		{
			this.savedNumbers.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		}
		int num = msg.read.Int32();
		string text = msg.read.String(256);
		if (!this.IsSavedContactValid(text, num))
		{
			return;
		}
		if (this.savedNumbers.entries.Count >= 10)
		{
			return;
		}
		PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
		directoryEntry.phoneName = text;
		directoryEntry.phoneNumber = num;
		directoryEntry.ShouldPool = false;
		this.savedNumbers.ShouldPool = false;
		this.savedNumbers.entries.Add(directoryEntry);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000D9344 File Offset: 0x000D7544
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		uint number = msg.read.UInt32();
		if (this.savedNumbers.entries.RemoveAll((PhoneDirectory.DirectoryEntry p) => (long)p.phoneNumber == (long)((ulong)number)) > 0)
		{
			base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000D93A7 File Offset: 0x000D75A7
	public string GetDirectoryName()
	{
		return this.PhoneName;
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000D93B0 File Offset: 0x000D75B0
	private static string PositionToGridCoord(Vector3 position)
	{
		Vector2 vector = new Vector2(TerrainMeta.NormalizeX(position.x), TerrainMeta.NormalizeZ(position.z));
		float num = TerrainMeta.Size.x / 1024f;
		int num2 = 7;
		Vector2 vector2 = vector * num * (float)num2;
		float num3 = Mathf.Floor(vector2.x) + 1f;
		float num4 = Mathf.Floor(num * (float)num2 - vector2.y);
		string text = string.Empty;
		float num5 = num3 / 26f;
		float num6 = num3 % 26f;
		if (num6 == 0f)
		{
			num6 = 26f;
		}
		if (num5 > 1f)
		{
			text += Convert.ToChar(64 + (int)num5).ToString();
		}
		text += Convert.ToChar(64 + (int)num6).ToString();
		return string.Format("{0}{1}", text, num4);
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x000D9498 File Offset: 0x000D7698
	public void WatchForDisconnects()
	{
		bool flag = false;
		if (this.currentPlayer != null)
		{
			if (this.currentPlayer.IsSleeping())
			{
				flag = true;
			}
			if (this.currentPlayer.IsDead())
			{
				flag = true;
			}
			if (Vector3.Distance(base.transform.position, this.currentPlayer.transform.position) > 5f)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			this.ServerHangUp();
			this.ClearCurrentUser();
		}
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000D950F File Offset: 0x000D770F
	public void OnParentChanged(global::BaseEntity newParent)
	{
		if (newParent != null && newParent is global::BasePlayer)
		{
			TelephoneManager.RegisterTelephone(this, true);
			return;
		}
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000D9536 File Offset: 0x000D7736
	private bool HasVoicemailSlot()
	{
		return this.MaxVoicemailSlots > 0;
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x000D9544 File Offset: 0x000D7744
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		PhoneController telephone = TelephoneManager.GetTelephone(msg.read.Int32());
		if (telephone == null)
		{
			return;
		}
		if (!global::Cassette.IsOggValid(array, telephone.cachedCassette))
		{
			return;
		}
		telephone.SaveVoicemail(array, msg.player.displayName);
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000D95B0 File Offset: 0x000D77B0
	public void SaveVoicemail(byte[] data, string playerName)
	{
		uint num = FileStorage.server.Store(data, FileStorage.Type.ogg, base.baseEntity.net.ID, 0U);
		if (this.savedVoicemail == null)
		{
			this.savedVoicemail = Pool.GetList<ProtoBuf.VoicemailEntry>();
		}
		ProtoBuf.VoicemailEntry voicemailEntry = Pool.Get<ProtoBuf.VoicemailEntry>();
		voicemailEntry.audioId = num;
		voicemailEntry.timestamp = DateTime.Now.ToBinary();
		voicemailEntry.userName = playerName;
		voicemailEntry.ShouldPool = false;
		this.savedVoicemail.Add(voicemailEntry);
		while (this.savedVoicemail.Count > this.MaxVoicemailSlots)
		{
			FileStorage.server.Remove(this.savedVoicemail[0].audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
			this.savedVoicemail.RemoveAt(0);
		}
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000D9681 File Offset: 0x000D7881
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 1, msg.read.UInt32());
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000D96A0 File Offset: 0x000D78A0
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 0, msg.read.UInt32());
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000D96C0 File Offset: 0x000D78C0
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		for (int i = 0; i < this.savedVoicemail.Count; i++)
		{
			if (this.savedVoicemail[i].audioId == num)
			{
				ProtoBuf.VoicemailEntry voicemailEntry = this.savedVoicemail[i];
				FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
				voicemailEntry.ShouldPool = true;
				Pool.Free<ProtoBuf.VoicemailEntry>(ref voicemailEntry);
				this.savedVoicemail.RemoveAt(i);
				base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
		}
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000D9758 File Offset: 0x000D7958
	public void DeleteAllVoicemail()
	{
		if (this.savedVoicemail == null)
		{
			return;
		}
		foreach (ProtoBuf.VoicemailEntry voicemailEntry in this.savedVoicemail)
		{
			voicemailEntry.ShouldPool = true;
			FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
		}
		Pool.FreeList<ProtoBuf.VoicemailEntry>(ref this.savedVoicemail);
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06002107 RID: 8455 RVA: 0x000D97E0 File Offset: 0x000D79E0
	public int MaxVoicemailSlots
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return 0;
			}
			return this.cachedCassette.MaximumVoicemailSlots;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06002108 RID: 8456 RVA: 0x000D97FD File Offset: 0x000D79FD
	// (set) Token: 0x06002109 RID: 8457 RVA: 0x000D982A File Offset: 0x000D7A2A
	public global::BasePlayer currentPlayer
	{
		get
		{
			if (this.currentPlayerRef.IsValid(this.isServer))
			{
				return this.currentPlayerRef.Get(this.isServer).ToPlayer();
			}
			return null;
		}
		set
		{
			this.currentPlayerRef.Set(value);
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x0600210A RID: 8458 RVA: 0x000D9838 File Offset: 0x000D7A38
	private bool isServer
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isServer;
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x0600210B RID: 8459 RVA: 0x000D9855 File Offset: 0x000D7A55
	// (set) Token: 0x0600210C RID: 8460 RVA: 0x000D985D File Offset: 0x000D7A5D
	public int lastDialedNumber { get; set; }

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x0600210D RID: 8461 RVA: 0x000D9866 File Offset: 0x000D7A66
	// (set) Token: 0x0600210E RID: 8462 RVA: 0x000D986E File Offset: 0x000D7A6E
	public PhoneDirectory savedNumbers { get; set; }

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x0600210F RID: 8463 RVA: 0x000D863A File Offset: 0x000D683A
	public global::BaseEntity ParentEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06002110 RID: 8464 RVA: 0x000D9878 File Offset: 0x000D7A78
	private global::Cassette cachedCassette
	{
		get
		{
			global::Telephone telephone;
			if (!(base.baseEntity != null) || (telephone = base.baseEntity as global::Telephone) == null)
			{
				return null;
			}
			return telephone.cachedCassette;
		}
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000D98AC File Offset: 0x000D7AAC
	private bool IsPowered()
	{
		global::IOEntity ioentity;
		return base.baseEntity != null && (ioentity = base.baseEntity as global::IOEntity) != null && ioentity.IsPowered();
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x000D98DE File Offset: 0x000D7ADE
	public bool IsSavedContactValid(string contactName, int contactNumber)
	{
		return contactName.Length > 0 && contactName.Length <= 20 && contactNumber >= 10000000 && contactNumber < 100000000;
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
	}

	// Token: 0x040019C6 RID: 6598
	private PhoneController activeCallTo;

	// Token: 0x040019C7 RID: 6599
	public int PhoneNumber;

	// Token: 0x040019C8 RID: 6600
	public string PhoneName;

	// Token: 0x040019C9 RID: 6601
	public bool CanModifyPhoneName = true;

	// Token: 0x040019CA RID: 6602
	public bool CanSaveNumbers = true;

	// Token: 0x040019CB RID: 6603
	public bool RequirePower = true;

	// Token: 0x040019CC RID: 6604
	public bool RequireParent;

	// Token: 0x040019CD RID: 6605
	public float CallWaitingTime = 12f;

	// Token: 0x040019CE RID: 6606
	public bool AppendGridToName;

	// Token: 0x040019CF RID: 6607
	public bool IsMobile;

	// Token: 0x040019D0 RID: 6608
	public bool CanSaveVoicemail;

	// Token: 0x040019D1 RID: 6609
	public GameObjectRef PhoneDialog;

	// Token: 0x040019D2 RID: 6610
	public VoiceProcessor VProcessor;

	// Token: 0x040019D3 RID: 6611
	public PreloadedCassetteContent PreloadedContent;

	// Token: 0x040019D4 RID: 6612
	public SoundDefinition DialToneSfx;

	// Token: 0x040019D5 RID: 6613
	public SoundDefinition RingingSfx;

	// Token: 0x040019D6 RID: 6614
	public SoundDefinition ErrorSfx;

	// Token: 0x040019D7 RID: 6615
	public SoundDefinition CallIncomingWhileBusySfx;

	// Token: 0x040019D8 RID: 6616
	public SoundDefinition PickupHandsetSfx;

	// Token: 0x040019D9 RID: 6617
	public SoundDefinition PutDownHandsetSfx;

	// Token: 0x040019DA RID: 6618
	public SoundDefinition FailedWrongNumber;

	// Token: 0x040019DB RID: 6619
	public SoundDefinition FailedNoAnswer;

	// Token: 0x040019DC RID: 6620
	public SoundDefinition FailedNetworkBusy;

	// Token: 0x040019DD RID: 6621
	public SoundDefinition FailedEngaged;

	// Token: 0x040019DE RID: 6622
	public SoundDefinition FailedRemoteHangUp;

	// Token: 0x040019DF RID: 6623
	public SoundDefinition FailedSelfHangUp;

	// Token: 0x040019E0 RID: 6624
	public Light RingingLight;

	// Token: 0x040019E1 RID: 6625
	public float RingingLightFrequency = 0.4f;

	// Token: 0x040019E2 RID: 6626
	public AudioSource answeringMachineSound;

	// Token: 0x040019E3 RID: 6627
	public EntityRef currentPlayerRef;

	// Token: 0x040019E6 RID: 6630
	public List<ProtoBuf.VoicemailEntry> savedVoicemail;
}
