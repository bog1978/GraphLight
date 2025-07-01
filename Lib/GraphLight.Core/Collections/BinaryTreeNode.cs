using System;

namespace GraphLight.Collections
{
    public class BinaryTreeNode<T>(T k) // структура для представления узлов дерева
        where T : IComparable<T>
    {
        public T Key => k;
        public int Height { get; set; } = 1;
        public BinaryTreeNode<T>? Left { get; set; }
        public BinaryTreeNode<T>? Right { get; set; }

        public override string ToString() =>
            Left == null && Right == null
                ? $"{Key}"
                : $"[{Key}:{Left}-{Right}]";
    };
}