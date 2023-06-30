using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x020000B2 RID: 178
public class PlayerModifiers : BaseModifiers<global::BasePlayer>
{
	// Token: 0x06001038 RID: 4152 RVA: 0x00087770 File Offset: 0x00085970
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerModifiers.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x000877B0 File Offset: 0x000859B0
	public override void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		base.ServerUpdate(ownerEntity);
		this.SendChangesToClient();
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000877C0 File Offset: 0x000859C0
	public ProtoBuf.PlayerModifiers Save()
	{
		ProtoBuf.PlayerModifiers playerModifiers = Pool.Get<ProtoBuf.PlayerModifiers>();
		playerModifiers.modifiers = Pool.GetList<ProtoBuf.Modifier>();
		foreach (global::Modifier modifier in this.All)
		{
			if (modifier != null)
			{
				playerModifiers.modifiers.Add(modifier.Save());
			}
		}
		return playerModifiers;
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x00087834 File Offset: 0x00085A34
	public void Load(ProtoBuf.PlayerModifiers m)
	{
		base.RemoveAll();
		if (m == null || m.modifiers == null)
		{
			return;
		}
		foreach (ProtoBuf.Modifier modifier in m.modifiers)
		{
			if (modifier != null)
			{
				global::Modifier modifier2 = new global::Modifier();
				modifier2.Init((global::Modifier.ModifierType)modifier.type, (global::Modifier.ModifierSource)modifier.source, modifier.value, modifier.duration, modifier.timeRemaing);
				base.Add(modifier2);
			}
		}
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x000878C8 File Offset: 0x00085AC8
	public void SendChangesToClient()
	{
		if (!this.dirty)
		{
			return;
		}
		base.SetDirty(false);
		using (ProtoBuf.PlayerModifiers playerModifiers = this.Save())
		{
			base.baseEntity.ClientRPCPlayer<ProtoBuf.PlayerModifiers>(null, base.baseEntity, "UpdateModifiers", playerModifiers);
		}
	}
}
