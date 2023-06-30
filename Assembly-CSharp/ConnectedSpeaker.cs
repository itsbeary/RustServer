using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class ConnectedSpeaker : global::IOEntity
{
	// Token: 0x060009FF RID: 2559 RVA: 0x0005DA44 File Offset: 0x0005BC44
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ConnectedSpeaker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0005DA84 File Offset: 0x0005BC84
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved8) != next.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			if (next.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(this, global::BoomBox.BacktrackLength);
				if (connectionSource != null)
				{
					base.ClientRPC<NetworkableId>(null, "Client_PlayAudioFrom", connectionSource.ToEntity().net.ID);
					this.connectedTo.Set(connectionSource.ToEntity());
					return;
				}
			}
			else if (this.connectedTo.IsSet)
			{
				base.ClientRPC<NetworkableId>(null, "Client_StopPlayingAudio", this.connectedTo.uid);
				this.connectedTo.Set(null);
			}
		}
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0005DB58 File Offset: 0x0005BD58
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.connectedSpeaker != null)
		{
			this.connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0005DB8C File Offset: 0x0005BD8C
	private IAudioConnectionSource GetConnectionSource(global::IOEntity entity, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		global::IOEntity.IOSlot[] inputs = entity.inputs;
		for (int i = 0; i < inputs.Length; i++)
		{
			global::IOEntity ioentity = inputs[i].connectedTo.Get(base.isServer);
			if (ioentity == this)
			{
				return null;
			}
			IAudioConnectionSource audioConnectionSource;
			if (ioentity != null && (audioConnectionSource = ioentity as IAudioConnectionSource) != null)
			{
				return audioConnectionSource;
			}
			if (ioentity != null)
			{
				IAudioConnectionSource connectionSource = this.GetConnectionSource(ioentity, depth - 1);
				if (connectionSource != null)
				{
					return connectionSource;
				}
			}
		}
		return null;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0005DC08 File Offset: 0x0005BE08
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Pool.Get<ProtoBuf.ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = this.connectedTo.uid;
	}

	// Token: 0x040006A8 RID: 1704
	public AudioSource SoundSource;

	// Token: 0x040006A9 RID: 1705
	private EntityRef<global::IOEntity> connectedTo;

	// Token: 0x040006AA RID: 1706
	public VoiceProcessor VoiceProcessor;
}
