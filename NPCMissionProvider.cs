using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class NPCMissionProvider : NPCTalking, IMissionProvider
{
	// Token: 0x06001872 RID: 6258 RVA: 0x00051298 File Offset: 0x0004F498
	public NetworkableId ProviderID()
	{
		return this.net.ID;
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 ProviderPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity Entity()
	{
		return this;
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000B6B88 File Offset: 0x000B4D88
	public override void OnConversationEnded(BasePlayer player)
	{
		player.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().Value.ToString(), 0f);
		base.OnConversationEnded(player);
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x000B6BBC File Offset: 0x000B4DBC
	public override void OnConversationStarted(BasePlayer speakingTo)
	{
		speakingTo.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().Value.ToString(), 1f);
		base.OnConversationStarted(speakingTo);
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x000B6BF0 File Offset: 0x000B4DF0
	public bool ContainsSpeech(string speech)
	{
		ConversationData[] conversations = this.conversations;
		for (int i = 0; i < conversations.Length; i++)
		{
			ConversationData.SpeechNode[] speeches = conversations[i].speeches;
			for (int j = 0; j < speeches.Length; j++)
			{
				if (speeches[j].shortname == speech)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x000B6C3C File Offset: 0x000B4E3C
	public string IntroOverride(string overrideSpeech)
	{
		if (!this.ContainsSpeech(overrideSpeech))
		{
			return "intro";
		}
		return overrideSpeech;
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x000B6C50 File Offset: 0x000B4E50
	public override string GetConversationStartSpeech(BasePlayer player)
	{
		string text = "";
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Active)
			{
				text = this.IntroOverride("missionactive");
			}
			if (missionInstance.status == BaseMission.MissionStatus.Completed && missionInstance.providerID == this.ProviderID() && Time.time - missionInstance.endTime < 5f)
			{
				text = this.IntroOverride("missionreturn");
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = base.GetConversationStartSpeech(player);
		}
		return text;
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x000B6D04 File Offset: 0x000B4F04
	public override void OnConversationAction(BasePlayer player, string action)
	{
		if (action.Contains("assignmission"))
		{
			int num = action.IndexOf(" ");
			BaseMission fromShortName = MissionManifest.GetFromShortName(action.Substring(num + 1));
			if (fromShortName)
			{
				BaseMission.AssignMission(player, this, fromShortName);
			}
		}
		base.OnConversationAction(player, action);
	}

	// Token: 0x0400112E RID: 4398
	public MissionManifest manifest;
}
