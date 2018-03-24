using System.Collections.Generic;

//先山寨一下
//todo  use min heap
public class PriorityQueue
{
    List<TNavNode> list = new List<TNavNode>();

    public int Count
    {
        get
        {
            return this.list.Count;
        }
    }

    public void Clear()
    {
        this.list.Clear();
    }

    public void Add(TNavNode node)
    {
        for (int i = 0; i < this.list.Count; i++)
        {
            if (node.fScore < this.list[i].fScore)
            {
                this.list.Insert(i, node);
                return;
            }

        }

        this.list.Add(node);
    }

    public TNavNode Pop()
    {
        if (this.list.Count > 0)
        {
            TNavNode node = this.list[0];
            this.list.RemoveAt(0);

            return node;
        }

        return null;
    }
}
