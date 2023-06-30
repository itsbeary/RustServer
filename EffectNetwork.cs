using System;
using Network;
using Network.Visibility;
using UnityEngine;

// Token: 0x0200033E RID: 830
public static class EffectNetwork
{
	// Token: 0x06001F4E RID: 8014 RVA: 0x000D44EC File Offset: 0x000D26EC
	public static void Send(Effect effect)
	{
		if (Net.sv == null)
		{
			return;
		}
		if (!Net.sv.IsConnected())
		{
			return;
		}
		using (TimeWarning.New("EffectNetwork.Send", 0))
		{
			if (!string.IsNullOrEmpty(effect.pooledString))
			{
				effect.pooledstringid = StringPool.Get(effect.pooledString);
			}
			if (effect.pooledstringid == 0U)
			{
				Debug.Log("String ID is 0 - unknown effect " + effect.pooledString);
			}
			else if (effect.broadcast)
			{
				NetWrite netWrite = Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.Effect);
				effect.WriteToStream(netWrite);
				netWrite.Send(new SendInfo(BaseNetworkable.GlobalNetworkGroup.subscribers));
			}
			else
			{
				Group group;
				if (effect.entity.IsValid)
				{
					BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(effect.entity) as BaseEntity;
					if (!baseEntity.IsValid())
					{
						return;
					}
					group = baseEntity.net.group;
				}
				else
				{
					group = Net.sv.visibility.GetGroup(effect.worldPos);
				}
				if (group != null)
				{
					NetWrite netWrite2 = Net.sv.StartWrite();
					netWrite2.PacketID(Message.Type.Effect);
					effect.WriteToStream(netWrite2);
					netWrite2.Send(new SendInfo(group.subscribers));
				}
			}
		}
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000D4644 File Offset: 0x000D2844
	public static void Send(Effect effect, Connection target)
	{
		effect.pooledstringid = StringPool.Get(effect.pooledString);
		if (effect.pooledstringid == 0U)
		{
			Debug.LogWarning("EffectNetwork.Send - unpooled effect name: " + effect.pooledString);
			return;
		}
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Effect);
		effect.WriteToStream(netWrite);
		netWrite.Send(new SendInfo(target));
	}
}
