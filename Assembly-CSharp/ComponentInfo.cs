using System;

// Token: 0x020002AA RID: 682
public abstract class ComponentInfo<T> : ComponentInfo
{
	// Token: 0x06001D7B RID: 7547 RVA: 0x000CAE09 File Offset: 0x000C9009
	public void Initialize(T source)
	{
		this.component = source;
		this.Setup();
	}

	// Token: 0x04001646 RID: 5702
	public T component;
}
