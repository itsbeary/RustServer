using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AAF RID: 2735
	[ConsoleSystem.Factory("audio")]
	public class Audio : ConsoleSystem
	{
		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06004179 RID: 16761 RVA: 0x00183C56 File Offset: 0x00181E56
		// (set) Token: 0x0600417A RID: 16762 RVA: 0x00183C60 File Offset: 0x00181E60
		[ClientVar(Help = "Volume", Saved = true)]
		public static int speakers
		{
			get
			{
				return (int)UnityEngine.AudioSettings.speakerMode;
			}
			set
			{
				value = Mathf.Clamp(value, 2, 7);
				if (!Application.isEditor)
				{
					AudioConfiguration configuration = UnityEngine.AudioSettings.GetConfiguration();
					configuration.speakerMode = (AudioSpeakerMode)value;
					using (TimeWarning.New("Audio Settings Reset", 250))
					{
						UnityEngine.AudioSettings.Reset(configuration);
					}
				}
			}
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		public static void printSounds(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x0600417C RID: 16764 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar(ClientAdmin = true, Help = "print active engine sound info")]
		public static void printEngineSounds(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x04003B5E RID: 15198
		[ClientVar(Help = "Volume", Saved = true)]
		public static float master = 1f;

		// Token: 0x04003B5F RID: 15199
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolume = 1f;

		// Token: 0x04003B60 RID: 15200
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolumemenu = 1f;

		// Token: 0x04003B61 RID: 15201
		[ClientVar(Help = "Volume", Saved = true)]
		public static float game = 1f;

		// Token: 0x04003B62 RID: 15202
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voices = 1f;

		// Token: 0x04003B63 RID: 15203
		[ClientVar(Help = "Volume", Saved = true)]
		public static float instruments = 1f;

		// Token: 0x04003B64 RID: 15204
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voiceProps = 1f;

		// Token: 0x04003B65 RID: 15205
		[ClientVar(Help = "Volume", Saved = true)]
		public static float eventAudio = 1f;

		// Token: 0x04003B66 RID: 15206
		[ClientVar(Help = "Ambience System")]
		public static bool ambience = true;

		// Token: 0x04003B67 RID: 15207
		[ClientVar(Help = "Max ms per frame to spend updating sounds")]
		public static float framebudget = 0.3f;

		// Token: 0x04003B68 RID: 15208
		[ClientVar]
		public static float minupdatefraction = 0.1f;

		// Token: 0x04003B69 RID: 15209
		[ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
		public static bool advancedocclusion = false;

		// Token: 0x04003B6A RID: 15210
		[ClientVar(Help = "Use higher quality sound fades on some sounds")]
		public static bool hqsoundfade = false;

		// Token: 0x04003B6B RID: 15211
		[ClientVar(Saved = false)]
		public static bool debugVoiceLimiting = false;
	}
}
