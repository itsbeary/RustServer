using System;

// Token: 0x0200020A RID: 522
public class NPCPlayerNavigatorTester : BaseMonoBehaviour
{
	// Token: 0x06001B78 RID: 7032 RVA: 0x000C276C File Offset: 0x000C096C
	private void Update()
	{
		if (this.TargetNode != this.currentNode)
		{
			base.GetComponent<BaseNavigator>().SetDestination(this.TargetNode.Path, this.TargetNode, 0.5f);
			this.currentNode = this.TargetNode;
		}
	}

	// Token: 0x04001346 RID: 4934
	public BasePathNode TargetNode;

	// Token: 0x04001347 RID: 4935
	private BasePathNode currentNode;
}
