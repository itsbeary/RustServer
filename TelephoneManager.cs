using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003AA RID: 938
public static class TelephoneManager
{
	// Token: 0x0600211B RID: 8475 RVA: 0x000D9AA4 File Offset: 0x000D7CA4
	public static int GetUnusedTelephoneNumber()
	{
		int num = UnityEngine.Random.Range(10000000, 99990000);
		int num2 = 0;
		int num3 = 1000;
		while (TelephoneManager.allTelephones.ContainsKey(num) && num2 < num3)
		{
			num2++;
			num = UnityEngine.Random.Range(10000000, 99990000);
		}
		if (num2 == num3)
		{
			num = TelephoneManager.maxAssignedPhoneNumber + 1;
		}
		TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, num);
		return num;
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000D9B10 File Offset: 0x000D7D10
	public static void RegisterTelephone(PhoneController t, bool checkPhoneNumber = false)
	{
		if (checkPhoneNumber && TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && TelephoneManager.allTelephones[t.PhoneNumber] != t)
		{
			t.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
		}
		if (!TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && t.PhoneNumber != 0)
		{
			TelephoneManager.allTelephones.Add(t.PhoneNumber, t);
			TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, t.PhoneNumber);
		}
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000D9B95 File Offset: 0x000D7D95
	public static void DeregisterTelephone(PhoneController t)
	{
		if (TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber))
		{
			TelephoneManager.allTelephones.Remove(t.PhoneNumber);
		}
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000D9BBA File Offset: 0x000D7DBA
	public static PhoneController GetTelephone(int number)
	{
		if (TelephoneManager.allTelephones.ContainsKey(number))
		{
			return TelephoneManager.allTelephones[number];
		}
		return null;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000D9BD8 File Offset: 0x000D7DD8
	public static PhoneController GetRandomTelephone(int ignoreNumber)
	{
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.PhoneNumber != ignoreNumber)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000D9C40 File Offset: 0x000D7E40
	public static int GetCurrentActiveCalls()
	{
		int num = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.serverState != global::Telephone.CallState.Idle)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return 0;
		}
		return num / 2;
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000D9CA8 File Offset: 0x000D7EA8
	public static void GetPhoneDirectory(int ignoreNumber, int page, int perPage, PhoneDirectory directory)
	{
		directory.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		int num = page * perPage;
		int num2 = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Key != ignoreNumber && !string.IsNullOrEmpty(keyValuePair.Value.PhoneName))
			{
				num2++;
				if (num2 >= num)
				{
					PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
					directoryEntry.phoneName = keyValuePair.Value.GetDirectoryName();
					directoryEntry.phoneNumber = keyValuePair.Value.PhoneNumber;
					directory.entries.Add(directoryEntry);
					if (directory.entries.Count >= perPage)
					{
						directory.atEnd = false;
						return;
					}
				}
			}
		}
		directory.atEnd = true;
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000D9D84 File Offset: 0x000D7F84
	[ServerVar]
	public static void PrintAllPhones(ConsoleSystem.Arg arg)
	{
		TextTable textTable = new TextTable();
		textTable.AddColumns(new string[] { "Number", "Name", "Position" });
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			Vector3 position = keyValuePair.Value.transform.position;
			textTable.AddRow(new string[]
			{
				keyValuePair.Key.ToString(),
				keyValuePair.Value.GetDirectoryName(),
				string.Format("{0} {1} {2}", position.x, position.y, position.z)
			});
		}
		arg.ReplyWith(textTable.ToString());
	}

	// Token: 0x040019ED RID: 6637
	public const int MaxPhoneNumber = 99990000;

	// Token: 0x040019EE RID: 6638
	public const int MinPhoneNumber = 10000000;

	// Token: 0x040019EF RID: 6639
	[ServerVar]
	public static int MaxConcurrentCalls = 10;

	// Token: 0x040019F0 RID: 6640
	[ServerVar]
	public static int MaxCallLength = 120;

	// Token: 0x040019F1 RID: 6641
	private static Dictionary<int, PhoneController> allTelephones = new Dictionary<int, PhoneController>();

	// Token: 0x040019F2 RID: 6642
	private static int maxAssignedPhoneNumber = 99990000;
}
