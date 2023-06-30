using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
[CreateAssetMenu(fileName = "NewConversation", menuName = "Rust/ConversationData", order = 1)]
public class ConversationData : ScriptableObject
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001868 RID: 6248 RVA: 0x000B6A88 File Offset: 0x000B4C88
	public string providerName
	{
		get
		{
			return this.providerNameTranslated.translated;
		}
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x000B6A98 File Offset: 0x000B4C98
	public int GetSpeechNodeIndex(string speechShortName)
	{
		for (int i = 0; i < this.speeches.Length; i++)
		{
			if (this.speeches[i].shortname == speechShortName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x04001128 RID: 4392
	public string shortname;

	// Token: 0x04001129 RID: 4393
	public Translate.Phrase providerNameTranslated;

	// Token: 0x0400112A RID: 4394
	public ConversationData.SpeechNode[] speeches;

	// Token: 0x02000C45 RID: 3141
	[Serializable]
	public class ConversationCondition
	{
		// Token: 0x06004E6D RID: 20077 RVA: 0x001A2960 File Offset: 0x001A0B60
		public bool Passes(BasePlayer player, IConversationProvider provider)
		{
			bool flag = false;
			if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASSCRAP)
			{
				flag = (long)player.inventory.GetAmount(ItemManager.FindItemDefinition("scrap").itemid) >= (long)((ulong)this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASHEALTH)
			{
				flag = player.health >= this.conditionAmount;
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.PROVIDERBUSY)
			{
				flag = provider.ProviderBusy();
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONCOMPLETE)
			{
				flag = player.HasCompletedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONATTEMPTED)
			{
				flag = player.HasAttemptedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.CANACCEPT)
			{
				flag = player.CanAcceptMission(this.conditionAmount);
			}
			if (!this.inverse)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x04004331 RID: 17201
		public ConversationData.ConversationCondition.ConditionType conditionType;

		// Token: 0x04004332 RID: 17202
		public uint conditionAmount;

		// Token: 0x04004333 RID: 17203
		public bool inverse;

		// Token: 0x04004334 RID: 17204
		public string failedSpeechNode;

		// Token: 0x02000FDC RID: 4060
		public enum ConditionType
		{
			// Token: 0x04005174 RID: 20852
			NONE,
			// Token: 0x04005175 RID: 20853
			HASHEALTH,
			// Token: 0x04005176 RID: 20854
			HASSCRAP,
			// Token: 0x04005177 RID: 20855
			PROVIDERBUSY,
			// Token: 0x04005178 RID: 20856
			MISSIONCOMPLETE,
			// Token: 0x04005179 RID: 20857
			MISSIONATTEMPTED,
			// Token: 0x0400517A RID: 20858
			CANACCEPT
		}
	}

	// Token: 0x02000C46 RID: 3142
	[Serializable]
	public class ResponseNode
	{
		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06004E6F RID: 20079 RVA: 0x001A2A27 File Offset: 0x001A0C27
		public string responseText
		{
			get
			{
				return this.responseTextLocalized.translated;
			}
		}

		// Token: 0x06004E70 RID: 20080 RVA: 0x001A2A34 File Offset: 0x001A0C34
		public bool PassesConditions(BasePlayer player, IConversationProvider provider)
		{
			ConversationData.ConversationCondition[] array = this.conditions;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Passes(player, provider))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x001A2A68 File Offset: 0x001A0C68
		public string GetFailedSpeechNode(BasePlayer player, IConversationProvider provider)
		{
			foreach (ConversationData.ConversationCondition conversationCondition in this.conditions)
			{
				if (!conversationCondition.Passes(player, provider))
				{
					return conversationCondition.failedSpeechNode;
				}
			}
			return "";
		}

		// Token: 0x04004335 RID: 17205
		public Translate.Phrase responseTextLocalized;

		// Token: 0x04004336 RID: 17206
		public ConversationData.ConversationCondition[] conditions;

		// Token: 0x04004337 RID: 17207
		public string actionString;

		// Token: 0x04004338 RID: 17208
		public string resultingSpeechNode;
	}

	// Token: 0x02000C47 RID: 3143
	[Serializable]
	public class SpeechNode
	{
		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06004E73 RID: 20083 RVA: 0x001A2AA4 File Offset: 0x001A0CA4
		public string statement
		{
			get
			{
				return this.statementLocalized.translated;
			}
		}

		// Token: 0x04004339 RID: 17209
		public string shortname;

		// Token: 0x0400433A RID: 17210
		public Translate.Phrase statementLocalized;

		// Token: 0x0400433B RID: 17211
		public ConversationData.ResponseNode[] responses;

		// Token: 0x0400433C RID: 17212
		public Vector2 nodePosition;
	}
}
