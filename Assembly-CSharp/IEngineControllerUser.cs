using System;
using Rust;

// Token: 0x020004C2 RID: 1218
public interface IEngineControllerUser : IEntity
{
	// Token: 0x060027DA RID: 10202
	bool HasFlag(BaseEntity.Flags f);

	// Token: 0x060027DB RID: 10203
	bool IsDead();

	// Token: 0x060027DC RID: 10204
	void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true);

	// Token: 0x060027DD RID: 10205
	void Invoke(Action action, float time);

	// Token: 0x060027DE RID: 10206
	void CancelInvoke(Action action);

	// Token: 0x060027DF RID: 10207
	void OnEngineStartFailed();

	// Token: 0x060027E0 RID: 10208
	bool MeetsEngineRequirements();
}
