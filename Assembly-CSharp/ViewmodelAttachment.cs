using System;

// Token: 0x0200097F RID: 2431
public class ViewmodelAttachment : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged, IViewModelUpdated
{
	// Token: 0x04003479 RID: 13433
	public GameObjectRef modelObject;

	// Token: 0x0400347A RID: 13434
	public string targetBone;

	// Token: 0x0400347B RID: 13435
	public bool hideViewModelIronSights;
}
