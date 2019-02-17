using UnityEngine;
using System;

public class PriorityQueue<E> where E : IComparable
{
    class TreeNode
    {
        public TreeNode left;
        public TreeNode right;
        public E item;
    }

    TreeNode m_root;

    public bool IsEmpty => m_root == null;

    public void Insert(E item)
    {
        Insert(item, ref m_root);
    }

    private void Insert(E item, ref TreeNode root)
    {
        if (root != null)
        {
            if (item.CompareTo(root.item) > 0)
            {
                Insert(item, ref root.right);
            }
            else
            {
                Insert(item, ref root.left);
            }
        }
        else
        {
            root = new TreeNode
            {
                item = item
            };
        }
    }

    public E Pull()
    {
        if (m_root == null)
            throw new Exception("Priotity Queue is Empty");

        return Pull(ref m_root);
    }

    private E Pull(ref TreeNode root)
    {
        if (root.left != null)
            return Pull(ref root.left);

        var i = root.item;
        root = root.right;
        return i;
    }
}
