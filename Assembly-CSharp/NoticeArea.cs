using System;
using UnityEngine;

// Token: 0x02000845 RID: 2117
public class NoticeArea : SingletonComponent<NoticeArea>
{
	// Token: 0x06003605 RID: 13829 RVA: 0x0014784E File Offset: 0x00145A4E
	protected override void Awake()
	{
		base.Awake();
		this.notices = base.GetComponentsInChildren<IVitalNotice>(true);
	}

	// Token: 0x04002F96 RID: 12182
	public GameObjectRef itemPickupPrefab;

	// Token: 0x04002F97 RID: 12183
	public GameObjectRef itemPickupCondensedText;

	// Token: 0x04002F98 RID: 12184
	public GameObjectRef itemDroppedPrefab;

	// Token: 0x04002F99 RID: 12185
	public AnimationCurve pickupSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F9A RID: 12186
	public AnimationCurve pickupAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F9B RID: 12187
	public AnimationCurve reuseAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F9C RID: 12188
	public AnimationCurve reuseSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F9D RID: 12189
	private IVitalNotice[] notices;
}
