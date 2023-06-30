using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A2A RID: 2602
	public static class CoroutineEx
	{
		// Token: 0x06003DA8 RID: 15784 RVA: 0x00169590 File Offset: 0x00167790
		public static WaitForSeconds waitForSeconds(float seconds)
		{
			WaitForSeconds waitForSeconds;
			if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, out waitForSeconds))
			{
				waitForSeconds = new WaitForSeconds(seconds);
				CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSeconds);
			}
			return waitForSeconds;
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x001695C0 File Offset: 0x001677C0
		public static WaitForSecondsRealtimeEx waitForSecondsRealtime(float seconds)
		{
			WaitForSecondsRealtimeEx waitForSecondsRealtimeEx = Pool.Get<WaitForSecondsRealtimeEx>();
			waitForSecondsRealtimeEx.WaitTime = seconds;
			return waitForSecondsRealtimeEx;
		}

		// Token: 0x06003DAA RID: 15786 RVA: 0x001695CE File Offset: 0x001677CE
		public static IEnumerator Combine(params IEnumerator[] coroutines)
		{
			for (;;)
			{
				bool flag = true;
				foreach (IEnumerator enumerator in coroutines)
				{
					if (enumerator != null && enumerator.MoveNext())
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				yield return CoroutineEx.waitForEndOfFrame;
			}
			yield break;
			yield break;
		}

		// Token: 0x040037CC RID: 14284
		public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		// Token: 0x040037CD RID: 14285
		public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

		// Token: 0x040037CE RID: 14286
		private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();
	}
}
