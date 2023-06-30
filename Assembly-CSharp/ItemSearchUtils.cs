using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

// Token: 0x02000865 RID: 2149
public static class ItemSearchUtils
{
	// Token: 0x06003638 RID: 13880 RVA: 0x00148468 File Offset: 0x00146668
	public static IOrderedEnumerable<ItemDefinition> SearchForItems(string searchString, Func<ItemDefinition, bool> validFilter = null)
	{
		if (searchString == "")
		{
			searchString = "BALLS BALLS BALLS";
		}
		return from y in ItemManager.itemList.Where((ItemDefinition x) => ItemSearchUtils.IsValidSearchResult(searchString, x) && (validFilter == null || validFilter(x))).Take(60)
			orderby ItemSearchUtils.ScoreSearchResult(searchString, y)
			select y;
	}

	// Token: 0x06003639 RID: 13881 RVA: 0x001484D4 File Offset: 0x001466D4
	private static bool IsValidSearchResult(string search, ItemDefinition target)
	{
		return ((target.isRedirectOf != null && target.redirectVendingBehaviour == ItemDefinition.RedirectVendingBehaviour.ListAsUniqueItem) || !target.hidden) && (target.shortname.Contains(search, CompareOptions.IgnoreCase) || target.displayName.translated.Contains(search, CompareOptions.IgnoreCase) || target.displayDescription.translated.Contains(search, CompareOptions.IgnoreCase));
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x0014853C File Offset: 0x0014673C
	private static float ScoreSearchResult(string search, ItemDefinition target)
	{
		float num = 0f;
		if (target.shortname.Equals(search, StringComparison.CurrentCultureIgnoreCase) || target.displayName.translated.Equals(search, StringComparison.CurrentCultureIgnoreCase))
		{
			num -= (float)(500 - search.Length);
		}
		float num2 = (target.shortname.Contains(search, CompareOptions.IgnoreCase) ? ((float)search.Length / (float)target.shortname.Length) : 0f);
		float num3 = (target.displayName.translated.Contains(search, CompareOptions.IgnoreCase) ? ((float)search.Length / (float)target.displayName.translated.Length) : 0f);
		float num4 = Mathf.Max(num2, num3);
		num -= 50f * num4;
		if (target.displayDescription.translated.Contains(search, CompareOptions.IgnoreCase))
		{
			num -= (float)search.Length;
		}
		return num;
	}
}
