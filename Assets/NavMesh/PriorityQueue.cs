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
        this.list.Add(node);
    }

    public TNavNode Pop()
    {
        int cout = this.list.Count;

        int index = 0;
        TNavNode node = this.list[0];
        for(int i = 1; i < cout; i++)
        {
            TNavNode cur = this.list[i];
            if (cur.fScore < node.fScore)
            {
                node = cur;
                index = i;
            }
        }

        this.list[index] = this.list[cout - 1];
        this.list.RemoveAt(cout-1);

        return node;
    }
}
