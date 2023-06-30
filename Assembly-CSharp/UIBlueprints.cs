using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081E RID: 2078
public class UIBlueprints : ListComponent<UIBlueprints>
{
	// Token: 0x04002EDD RID: 11997
	public GameObjectRef buttonPrefab;

	// Token: 0x04002EDE RID: 11998
	public ScrollRect scrollRect;

	// Token: 0x04002EDF RID: 11999
	public CanvasGroup ScrollRectCanvasGroup;

	// Token: 0x04002EE0 RID: 12000
	public InputField searchField;

	// Token: 0x04002EE1 RID: 12001
	public GameObject searchFieldPlaceholder;

	// Token: 0x04002EE2 RID: 12002
	public GameObject listAvailable;

	// Token: 0x04002EE3 RID: 12003
	public GameObject listLocked;

	// Token: 0x04002EE4 RID: 12004
	public GameObject Categories;

	// Token: 0x04002EE5 RID: 12005
	public VerticalLayoutGroup CategoryVerticalLayoutGroup;

	// Token: 0x04002EE6 RID: 12006
	public BlueprintCategoryButton FavouriteCategoryButton;
}
