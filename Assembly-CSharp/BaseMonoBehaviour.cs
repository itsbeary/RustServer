using System;
using ConVar;
using UnityEngine;

// Token: 0x020008F1 RID: 2289
public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	// Token: 0x060037A9 RID: 14249 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsDebugging()
	{
		return false;
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x0014CFCB File Offset: 0x0014B1CB
	public virtual string GetLogColor()
	{
		return "yellow";
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x0014CFD4 File Offset: 0x0014B1D4
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x060037AC RID: 14252 RVA: 0x0014D044 File Offset: 0x0014B244
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1, arg2);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0014D0B8 File Offset: 0x0014B2B8
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			str,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x02000EC0 RID: 3776
	public enum LogEntryType
	{
		// Token: 0x04004D43 RID: 19779
		General,
		// Token: 0x04004D44 RID: 19780
		Network,
		// Token: 0x04004D45 RID: 19781
		Hierarchy,
		// Token: 0x04004D46 RID: 19782
		Serialization
	}
}
