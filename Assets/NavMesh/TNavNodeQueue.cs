using System.Collections.Generic;

public class TNavNodeQueue
{
    List<TNavNode> nodes = new List<TNavNode>();

    public int Count
    {
        get
        {
            return this.nodes.Count;
        }
    }

    public void Clear()
    {
        this.nodes.Clear();
    }

    public TNavNode Peek()
    {
        return this.nodes[0];
    }

    public void Add(TNavNode node)
    {
        this.nodes.Add(node);

        this.bubbleUp(this.nodes.Count - 1);
    }

    public TNavNode Pop()
    {
        TNavNode node = this.nodes[0];

        int lastIndex = this.nodes.Count - 1;
        this.nodes[0] = this.nodes[lastIndex];
        this.nodes.RemoveAt(lastIndex);

        if(this.nodes.Count > 1)
            this.trickleDown(0);

        //foreach(var n in this.nodes)
        //{
        //    if(n.fScore < node.fScore)
        //    {
        //        int i = 0;
        //    }
        //}

        return node;
    }

    public void Modify(TNavNode node)
    {
        int index = this.nodes.IndexOf(node);
        if(index != -1)
        {
            this.bubbleUp(index);
        }
    }


    private void bubbleUp(int index)
    {
        while(index > 0)
        {
            TNavNode node = this.nodes[index];

            int parent = (index - 1) / 2;
            TNavNode parentNode = this.nodes[parent];

            if (node.fScore < parentNode.fScore)
            {
                this.nodes[index] = parentNode;
                this.nodes[parent] = node;

                index = parent;
            }
            else
                break;
        }
    }
    private void trickleDown(int index)
    {
        int childLeft = index * 2 + 1;

        while(childLeft < this.nodes.Count)
        {
            TNavNode node = this.nodes[index];

            int minNodeIndex = index;
            TNavNode minNode = node;

            TNavNode childLeftNode = this.nodes[childLeft];
            if (childLeftNode.fScore < minNode.fScore)
            {
                minNode = childLeftNode;
                minNodeIndex = childLeft;
            }

            int childRight = childLeft + 1;
            if (childRight < this.nodes.Count)
            {
                TNavNode childRightNode = this.nodes[childRight];
                if (childRightNode.fScore < minNode.fScore)
                {
                    minNode = childRightNode;
                    minNodeIndex = childRight;
                }
            }

            if (minNodeIndex != index)
            {
                this.nodes[minNodeIndex] = node;
                this.nodes[index] = minNode;

                index = minNodeIndex;
                childLeft = index * 2 + 1;
            }
            else
            {
                break;
            }
        }
    }
}
