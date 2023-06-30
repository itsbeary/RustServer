using System;

// Token: 0x02000275 RID: 629
public class SocketMod : PrefabAttribute
{
	// Token: 0x06001CEE RID: 7406 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool DoCheck(Construction.Placement place)
	{
		return false;
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ModifyPlacement(Construction.Placement place)
	{
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x000C8618 File Offset: 0x000C6818
	protected override Type GetIndexedType()
	{
		return typeof(SocketMod);
	}

	// Token: 0x0400157B RID: 5499
	[NonSerialized]
	public Socket_Base baseSocket;

	// Token: 0x0400157C RID: 5500
	public Translate.Phrase FailedPhrase;
}
