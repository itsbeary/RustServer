using System;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class ConversationManager : MonoBehaviour
{
	// Token: 0x02000C48 RID: 3144
	public class Conversation : MonoBehaviour
	{
		// Token: 0x06004E75 RID: 20085 RVA: 0x001A2AB1 File Offset: 0x001A0CB1
		public int GetSpeechNodeIndex(string name)
		{
			if (this.data == null)
			{
				return -1;
			}
			return this.data.GetSpeechNodeIndex(name);
		}

		// Token: 0x0400433D RID: 17213
		public ConversationData data;

		// Token: 0x0400433E RID: 17214
		public int currentSpeechNodeIndex;

		// Token: 0x0400433F RID: 17215
		public IConversationProvider provider;
	}
}
