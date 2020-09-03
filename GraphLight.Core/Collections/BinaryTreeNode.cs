using System;

namespace GraphLight.Collections
{
    public class BinaryTreeNode<T> // структура для представления узлов дерева
    where T : IComparable<T>
    {
        public T key;
        public int height;
        public BinaryTreeNode<T> left;
        public BinaryTreeNode<T> right;

        public BinaryTreeNode(T k)
        {
            key = k;
            left = null;
            right = null;
            height = 1;
        }

        public override string ToString() =>
            left == null && right == null
                ? $"{key}"
                : $"[{key}:{left}-{right}]";
    };
}
