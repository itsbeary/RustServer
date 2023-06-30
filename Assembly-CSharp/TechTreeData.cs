using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000583 RID: 1411
[CreateAssetMenu(fileName = "NewTechTree", menuName = "Rust/Tech Tree", order = 2)]
public class TechTreeData : ScriptableObject
{
	// Token: 0x06002B5C RID: 11100 RVA: 0x00107914 File Offset: 0x00105B14
	public TechTreeData.NodeInstance GetByID(int id)
	{
		if (UnityEngine.Application.isPlaying)
		{
			if (this._idToNode == null)
			{
				this._idToNode = this.nodes.ToDictionary((TechTreeData.NodeInstance n) => n.id, (TechTreeData.NodeInstance n) => n);
			}
			TechTreeData.NodeInstance nodeInstance;
			this._idToNode.TryGetValue(id, out nodeInstance);
			return nodeInstance;
		}
		this._idToNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance2 in this.nodes)
		{
			if (nodeInstance2.id == id)
			{
				return nodeInstance2;
			}
		}
		return null;
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x001079E8 File Offset: 0x00105BE8
	public TechTreeData.NodeInstance GetEntryNode()
	{
		if (UnityEngine.Application.isPlaying && this._entryNode != null && this._entryNode.groupName == "Entry")
		{
			return this._entryNode;
		}
		this._entryNode = null;
		foreach (TechTreeData.NodeInstance nodeInstance in this.nodes)
		{
			if (nodeInstance.groupName == "Entry")
			{
				this._entryNode = nodeInstance;
				return nodeInstance;
			}
		}
		Debug.LogError("NO ENTRY NODE FOR TECH TREE, This will Fail hard");
		return null;
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x00107A94 File Offset: 0x00105C94
	public void ClearInputs(TechTreeData.NodeInstance node)
	{
		foreach (int num in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(num);
			byID.inputs.Clear();
			this.ClearInputs(byID);
		}
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x00107AFC File Offset: 0x00105CFC
	public void SetupInputs(TechTreeData.NodeInstance node)
	{
		foreach (int num in node.outputs)
		{
			TechTreeData.NodeInstance byID = this.GetByID(num);
			if (!byID.inputs.Contains(node.id))
			{
				byID.inputs.Add(node.id);
			}
			this.SetupInputs(byID);
		}
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x00107B7C File Offset: 0x00105D7C
	public bool PlayerHasPathForUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		TechTreeData.NodeInstance entryNode = this.GetEntryNode();
		return entryNode != null && this.CheckChainRecursive(player, entryNode, node);
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x00107BA0 File Offset: 0x00105DA0
	public bool CheckChainRecursive(BasePlayer player, TechTreeData.NodeInstance start, TechTreeData.NodeInstance target)
	{
		if (start.groupName != "Entry")
		{
			if (start.IsGroup())
			{
				using (List<int>.Enumerator enumerator = start.inputs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						if (!this.PlayerHasPathForUnlock(player, this.GetByID(num)))
						{
							return false;
						}
					}
					goto IL_69;
				}
			}
			if (!this.HasPlayerUnlocked(player, start))
			{
				return false;
			}
		}
		IL_69:
		bool flag = false;
		foreach (int num2 in start.outputs)
		{
			if (num2 == target.id)
			{
				return true;
			}
			if (this.CheckChainRecursive(player, this.GetByID(num2), target))
			{
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x00107C8C File Offset: 0x00105E8C
	public bool PlayerCanUnlock(BasePlayer player, TechTreeData.NodeInstance node)
	{
		return this.PlayerHasPathForUnlock(player, node) && !this.HasPlayerUnlocked(player, node);
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x00107CA8 File Offset: 0x00105EA8
	public bool HasPlayerUnlocked(BasePlayer player, TechTreeData.NodeInstance node)
	{
		if (node.IsGroup())
		{
			bool flag = true;
			foreach (int num in node.outputs)
			{
				TechTreeData.NodeInstance byID = this.GetByID(num);
				if (!this.HasPlayerUnlocked(player, byID))
				{
					flag = false;
				}
			}
			return flag;
		}
		return player.blueprints.HasUnlocked(node.itemDef);
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x00107D28 File Offset: 0x00105F28
	public void GetNodesRequiredToUnlock(BasePlayer player, TechTreeData.NodeInstance node, List<TechTreeData.NodeInstance> foundNodes)
	{
		foundNodes.Add(node);
		if (node == this.GetEntryNode())
		{
			return;
		}
		if (node.inputs.Count == 1)
		{
			this.GetNodesRequiredToUnlock(player, this.GetByID(node.inputs[0]), foundNodes);
			return;
		}
		List<TechTreeData.NodeInstance> list = Pool.GetList<TechTreeData.NodeInstance>();
		int num = int.MaxValue;
		foreach (int num2 in node.inputs)
		{
			List<TechTreeData.NodeInstance> list2 = Pool.GetList<TechTreeData.NodeInstance>();
			this.GetNodesRequiredToUnlock(player, this.GetByID(num2), list2);
			int num3 = 0;
			foreach (TechTreeData.NodeInstance nodeInstance in list2)
			{
				if (!(nodeInstance.itemDef == null) && !this.HasPlayerUnlocked(player, nodeInstance))
				{
					num3 += ResearchTable.ScrapForResearch(nodeInstance.itemDef, ResearchTable.ResearchType.TechTree);
				}
			}
			if (num3 < num)
			{
				list.Clear();
				list.AddRange(list2);
				num = num3;
			}
			Pool.FreeList<TechTreeData.NodeInstance>(ref list2);
		}
		foundNodes.AddRange(list);
		Pool.FreeList<TechTreeData.NodeInstance>(ref list);
	}

	// Token: 0x0400235F RID: 9055
	public string shortname;

	// Token: 0x04002360 RID: 9056
	public int nextID;

	// Token: 0x04002361 RID: 9057
	private Dictionary<int, TechTreeData.NodeInstance> _idToNode;

	// Token: 0x04002362 RID: 9058
	private TechTreeData.NodeInstance _entryNode;

	// Token: 0x04002363 RID: 9059
	public List<TechTreeData.NodeInstance> nodes = new List<TechTreeData.NodeInstance>();

	// Token: 0x02000D74 RID: 3444
	[Serializable]
	public class NodeInstance
	{
		// Token: 0x06005120 RID: 20768 RVA: 0x001AB716 File Offset: 0x001A9916
		public bool IsGroup()
		{
			return this.itemDef == null && this.groupName != "Entry" && !string.IsNullOrEmpty(this.groupName);
		}

		// Token: 0x040047FC RID: 18428
		public int id;

		// Token: 0x040047FD RID: 18429
		public ItemDefinition itemDef;

		// Token: 0x040047FE RID: 18430
		public Vector2 graphPosition;

		// Token: 0x040047FF RID: 18431
		public List<int> outputs = new List<int>();

		// Token: 0x04004800 RID: 18432
		public List<int> inputs = new List<int>();

		// Token: 0x04004801 RID: 18433
		public string groupName;

		// Token: 0x04004802 RID: 18434
		public int costOverride = -1;
	}
}
