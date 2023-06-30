using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Facepunch
{
	// Token: 0x02000AF7 RID: 2807
	public class VirtualScroll : MonoBehaviour
	{
		// Token: 0x060043A5 RID: 17317 RVA: 0x0018E9DD File Offset: 0x0018CBDD
		public void Awake()
		{
			this.ScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScrollChanged));
			if (this.DataSourceObject != null)
			{
				this.SetDataSource(this.DataSourceObject.GetComponent<VirtualScroll.IDataSource>(), false);
			}
		}

		// Token: 0x060043A6 RID: 17318 RVA: 0x0018EA1B File Offset: 0x0018CC1B
		public void OnDestroy()
		{
			this.ScrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnScrollChanged));
		}

		// Token: 0x060043A7 RID: 17319 RVA: 0x0018EA39 File Offset: 0x0018CC39
		private void OnScrollChanged(Vector2 pos)
		{
			this.Rebuild();
		}

		// Token: 0x060043A8 RID: 17320 RVA: 0x0018EA41 File Offset: 0x0018CC41
		public void SetDataSource(VirtualScroll.IDataSource source, bool forceRebuild = false)
		{
			if (this.dataSource == source && !forceRebuild)
			{
				return;
			}
			this.dataSource = source;
			this.FullRebuild();
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x060043A9 RID: 17321 RVA: 0x0018EA5D File Offset: 0x0018CC5D
		private int BlockHeight
		{
			get
			{
				return this.ItemHeight + this.ItemSpacing;
			}
		}

		// Token: 0x060043AA RID: 17322 RVA: 0x0018EA6C File Offset: 0x0018CC6C
		public void FullRebuild()
		{
			foreach (int num in this.ActivePool.Keys.ToArray<int>())
			{
				this.Recycle(num);
			}
			this.Rebuild();
		}

		// Token: 0x060043AB RID: 17323 RVA: 0x0018EAAC File Offset: 0x0018CCAC
		public void DataChanged()
		{
			foreach (KeyValuePair<int, GameObject> keyValuePair in this.ActivePool)
			{
				this.dataSource.SetItemData(keyValuePair.Key, keyValuePair.Value);
			}
			this.Rebuild();
		}

		// Token: 0x060043AC RID: 17324 RVA: 0x0018EB18 File Offset: 0x0018CD18
		public void Rebuild()
		{
			if (this.dataSource == null)
			{
				return;
			}
			int itemCount = this.dataSource.GetItemCount();
			RectTransform rectTransform = ((this.OverrideContentRoot != null) ? this.OverrideContentRoot : (this.ScrollRect.viewport.GetChild(0) as RectTransform));
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)(this.BlockHeight * itemCount - this.ItemSpacing + this.Padding.top + this.Padding.bottom));
			int num = Mathf.Max(2, Mathf.CeilToInt(this.ScrollRect.viewport.rect.height / (float)this.BlockHeight));
			int num2 = Mathf.FloorToInt((rectTransform.anchoredPosition.y - (float)this.Padding.top) / (float)this.BlockHeight);
			int num3 = num2 + num;
			this.RecycleOutOfRange(num2, (float)num3);
			for (int i = num2; i <= num3; i++)
			{
				if (i >= 0 && i < itemCount)
				{
					this.BuildItem(i);
				}
			}
		}

		// Token: 0x060043AD RID: 17325 RVA: 0x0018EC18 File Offset: 0x0018CE18
		private void RecycleOutOfRange(int startVisible, float endVisible)
		{
			foreach (int num in (from x in this.ActivePool.Keys
				where x < startVisible || (float)x > endVisible
				select (x)).ToArray<int>())
			{
				this.Recycle(num);
			}
		}

		// Token: 0x060043AE RID: 17326 RVA: 0x0018EC98 File Offset: 0x0018CE98
		private void Recycle(int key)
		{
			GameObject gameObject = this.ActivePool[key];
			gameObject.SetActive(false);
			this.ActivePool.Remove(key);
			this.InactivePool.Push(gameObject);
		}

		// Token: 0x060043AF RID: 17327 RVA: 0x0018ECD4 File Offset: 0x0018CED4
		private void BuildItem(int i)
		{
			if (i < 0)
			{
				return;
			}
			if (this.ActivePool.ContainsKey(i))
			{
				return;
			}
			GameObject item = this.GetItem();
			item.SetActive(true);
			this.dataSource.SetItemData(i, item);
			RectTransform rectTransform = item.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.pivot = new Vector2(0.5f, 1f);
			rectTransform.offsetMin = new Vector2(0f, 0f);
			rectTransform.offsetMax = new Vector2(0f, (float)this.ItemHeight);
			rectTransform.sizeDelta = new Vector2((float)((this.Padding.left + this.Padding.right) * -1), (float)this.ItemHeight);
			rectTransform.anchoredPosition = new Vector2((float)(this.Padding.left - this.Padding.right) * 0.5f, (float)(-1 * (i * this.BlockHeight + this.Padding.top)));
			this.ActivePool[i] = item;
		}

		// Token: 0x060043B0 RID: 17328 RVA: 0x0018EE00 File Offset: 0x0018D000
		private GameObject GetItem()
		{
			if (this.InactivePool.Count == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.SourceObject);
				gameObject.transform.SetParent((this.OverrideContentRoot != null) ? this.OverrideContentRoot : this.ScrollRect.viewport.GetChild(0), false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(false);
				this.InactivePool.Push(gameObject);
			}
			return this.InactivePool.Pop();
		}

		// Token: 0x04003CD6 RID: 15574
		public int ItemHeight = 40;

		// Token: 0x04003CD7 RID: 15575
		public int ItemSpacing = 10;

		// Token: 0x04003CD8 RID: 15576
		public RectOffset Padding;

		// Token: 0x04003CD9 RID: 15577
		[Tooltip("Optional, we'll try to GetComponent IDataSource from this object on awake")]
		public GameObject DataSourceObject;

		// Token: 0x04003CDA RID: 15578
		public GameObject SourceObject;

		// Token: 0x04003CDB RID: 15579
		public ScrollRect ScrollRect;

		// Token: 0x04003CDC RID: 15580
		public RectTransform OverrideContentRoot;

		// Token: 0x04003CDD RID: 15581
		private VirtualScroll.IDataSource dataSource;

		// Token: 0x04003CDE RID: 15582
		private Dictionary<int, GameObject> ActivePool = new Dictionary<int, GameObject>();

		// Token: 0x04003CDF RID: 15583
		private Stack<GameObject> InactivePool = new Stack<GameObject>();

		// Token: 0x02000F85 RID: 3973
		public interface IDataSource
		{
			// Token: 0x06005504 RID: 21764
			int GetItemCount();

			// Token: 0x06005505 RID: 21765
			void SetItemData(int i, GameObject obj);
		}
	}
}
