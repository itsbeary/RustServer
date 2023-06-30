using System;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FC RID: 2556
	public abstract class BaseEntityHandler<T> : BaseHandler<T> where T : class
	{
		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06003D08 RID: 15624 RVA: 0x001660C9 File Offset: 0x001642C9
		// (set) Token: 0x06003D09 RID: 15625 RVA: 0x001660D1 File Offset: 0x001642D1
		private protected AppIOEntity Entity { protected get; private set; }

		// Token: 0x06003D0A RID: 15626 RVA: 0x001660DA File Offset: 0x001642DA
		public override void EnterPool()
		{
			base.EnterPool();
			this.Entity = null;
		}

		// Token: 0x06003D0B RID: 15627 RVA: 0x001660EC File Offset: 0x001642EC
		public override ValidationResult Validate()
		{
			ValidationResult validationResult = base.Validate();
			if (validationResult != ValidationResult.Success)
			{
				return validationResult;
			}
			AppIOEntity appIOEntity = BaseNetworkable.serverEntities.Find(base.Request.entityId) as AppIOEntity;
			if (appIOEntity == null)
			{
				return ValidationResult.NotFound;
			}
			BuildingPrivlidge buildingPrivilege = appIOEntity.GetBuildingPrivilege();
			if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(base.UserId))
			{
				return ValidationResult.NotFound;
			}
			this.Entity = appIOEntity;
			base.Client.Subscribe(new EntityTarget(base.Request.entityId));
			return ValidationResult.Success;
		}
	}
}
