using System;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class RealmedCollider : BasePrefab
{
	// Token: 0x06002A99 RID: 10905 RVA: 0x001039BC File Offset: 0x00101BBC
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		if (this.ServerCollider != this.ClientCollider)
		{
			if (clientside)
			{
				if (this.ServerCollider)
				{
					process.RemoveComponent(this.ServerCollider);
					this.ServerCollider = null;
				}
			}
			else if (this.ClientCollider)
			{
				process.RemoveComponent(this.ClientCollider);
				this.ClientCollider = null;
			}
		}
		process.RemoveComponent(this);
	}

	// Token: 0x040022D5 RID: 8917
	public Collider ServerCollider;

	// Token: 0x040022D6 RID: 8918
	public Collider ClientCollider;
}
