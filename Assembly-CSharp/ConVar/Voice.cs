using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AEF RID: 2799
	[ConsoleSystem.Factory("voice")]
	public class Voice : ConsoleSystem
	{
		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06004349 RID: 17225 RVA: 0x0018D310 File Offset: 0x0018B510
		// (set) Token: 0x0600434A RID: 17226 RVA: 0x0018D317 File Offset: 0x0018B517
		[ReplicatedVar]
		public static float voiceRangeBoostAmount
		{
			get
			{
				return Voice._voiceRangeBoostAmount;
			}
			set
			{
				Voice._voiceRangeBoostAmount = Mathf.Clamp(value, 0f, 200f);
			}
		}

		// Token: 0x0600434B RID: 17227 RVA: 0x0018D330 File Offset: 0x0018B530
		[ServerVar(Help = "Enabled/disables voice range boost for a player eg. ToggleVoiceRangeBoost sam 1")]
		public static void ToggleVoiceRangeBoost(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (player == null)
			{
				arg.ReplyWith("Invalid player: " + arg.GetString(0, ""));
				return;
			}
			bool @bool = arg.GetBool(1, false);
			player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceRangeBoost, @bool);
			arg.ReplyWith(string.Format("Set {0} volume boost to {1}", player.displayName, @bool));
		}

		// Token: 0x04003CAD RID: 15533
		[ClientVar(Saved = true)]
		public static bool loopback = false;

		// Token: 0x04003CAE RID: 15534
		private static float _voiceRangeBoostAmount = 50f;
	}
}
