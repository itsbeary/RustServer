using System;
using Rust;

// Token: 0x0200044B RID: 1099
public class PlayerInput : EntityComponent<BasePlayer>
{
	// Token: 0x060024EB RID: 9451 RVA: 0x000EA240 File Offset: 0x000E8440
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.state.Clear();
	}

	// Token: 0x04001CD8 RID: 7384
	public InputState state = new InputState();

	// Token: 0x04001CD9 RID: 7385
	[NonSerialized]
	public bool hadInputBuffer = true;
}
