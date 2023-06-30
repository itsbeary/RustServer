using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class RFManager
{
	// Token: 0x060028AE RID: 10414 RVA: 0x000FB5BC File Offset: 0x000F97BC
	public static int ClampFrequency(int freq)
	{
		return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x000FB5D0 File Offset: 0x000F97D0
	public static List<IRFObject> GetListenList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._listeners.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._listeners.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000FB60C File Offset: 0x000F980C
	public static List<IRFObject> GetBroadcasterList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._broadcasters.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._broadcasters.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000FB648 File Offset: 0x000F9848
	public static void AddListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			Debug.Log("adding same listener twice");
			return;
		}
		listenList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000FB688 File Offset: 0x000F9888
	public static void RemoveListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			listenList.Remove(obj);
		}
		obj.RFSignalUpdate(false);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000FB6BC File Offset: 0x000F98BC
	public static void AddBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			return;
		}
		broadcasterList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000FB6F0 File Offset: 0x000F98F0
	public static void RemoveBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			broadcasterList.Remove(obj);
		}
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x000FB723 File Offset: 0x000F9923
	public static bool IsReserved(int frequency)
	{
		return frequency >= RFManager.reserveRangeMin && frequency <= RFManager.reserveRangeMax;
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000FB738 File Offset: 0x000F9938
	public static void ReserveErrorPrint(BasePlayer player)
	{
		player.ChatMessage(RFManager.reserveString);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000FB745 File Offset: 0x000F9945
	public static void ChangeFrequency(int oldFrequency, int newFrequency, IRFObject obj, bool isListener, bool isOn = true)
	{
		newFrequency = RFManager.ClampFrequency(newFrequency);
		if (isListener)
		{
			RFManager.RemoveListener(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddListener(newFrequency, obj);
				return;
			}
		}
		else
		{
			RFManager.RemoveBroadcaster(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddBroadcaster(newFrequency, obj);
			}
		}
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000FB778 File Offset: 0x000F9978
	public static void MarkFrequencyDirty(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		bool flag = broadcasterList.Count > 0;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = listenList.Count - 1; i >= 0; i--)
		{
			IRFObject irfobject = listenList[i];
			if (!irfobject.IsValidEntityReference<IRFObject>())
			{
				flag2 = true;
			}
			else
			{
				if (flag)
				{
					flag = false;
					foreach (IRFObject irfobject2 in broadcasterList)
					{
						if (!irfobject2.IsValidEntityReference<IRFObject>())
						{
							flag3 = true;
						}
						else if (Vector3.Distance(irfobject2.GetPosition(), irfobject.GetPosition()) <= irfobject2.GetMaxRange())
						{
							flag = true;
							break;
						}
					}
				}
				irfobject.RFSignalUpdate(flag);
			}
		}
		if (flag2)
		{
			Debug.LogWarning("Found null entries in the RF listener list for frequency " + frequency + "... cleaning up.");
			for (int j = listenList.Count - 1; j >= 0; j--)
			{
				if (listenList[j] == null)
				{
					listenList.RemoveAt(j);
				}
			}
		}
		if (flag3)
		{
			Debug.LogWarning("Found null entries in the RF broadcaster list for frequency " + frequency + "... cleaning up.");
			for (int k = broadcasterList.Count - 1; k >= 0; k--)
			{
				if (broadcasterList[k] == null)
				{
					broadcasterList.RemoveAt(k);
				}
			}
		}
	}

	// Token: 0x040020FD RID: 8445
	public static Dictionary<int, List<IRFObject>> _listeners = new Dictionary<int, List<IRFObject>>();

	// Token: 0x040020FE RID: 8446
	public static Dictionary<int, List<IRFObject>> _broadcasters = new Dictionary<int, List<IRFObject>>();

	// Token: 0x040020FF RID: 8447
	public static int minFreq = 1;

	// Token: 0x04002100 RID: 8448
	public static int maxFreq = 9999;

	// Token: 0x04002101 RID: 8449
	private static int reserveRangeMin = 4760;

	// Token: 0x04002102 RID: 8450
	private static int reserveRangeMax = 4790;

	// Token: 0x04002103 RID: 8451
	public static string reserveString = string.Concat(new object[]
	{
		"Channels ",
		RFManager.reserveRangeMin,
		" to ",
		RFManager.reserveRangeMax,
		" are restricted."
	});
}
