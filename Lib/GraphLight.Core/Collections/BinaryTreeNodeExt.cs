using System;

namespace GraphLight.Collections
{
    public static class BinaryTreeNodeExt
    {
        #region Другое

        public static BinaryTreeNode<T> Insert<T>(this BinaryTreeNode<T>? p, T k) // вставка ключа k в дерево с корнем p
            where T : IComparable<T>
        {
            if (p == null)
                return new BinaryTreeNode<T>(k);

            if (k.CompareTo(p.Key) < 0)
                p.Left = p.Left.Insert(k);
            else
                p.Right = p.Right.Insert(k);

            return p.Balance();
        }

        public static BinaryTreeNode<T>? Remove<T>(this BinaryTreeNode<T>? p, T k) // удаление ключа k из дерева p
            where T : IComparable<T>
        {
            if (p == null)
                return null;

            var cmp = k.CompareTo(p.Key);
            if (cmp < 0)
                p.Left = p.Left.Remove(k);
            else if (cmp > 0)
                p.Right = p.Right.Remove(k);
            else
            {
                var q = p.Left;
                var r = p.Right;
                if (r == null) return q;
                var min = r.FindMin();
                min.Right = r.RemoveMin();
                min.Left = q;
                return min.Balance();
            }

            return p.Balance();
        }

        private static BinaryTreeNode<T> Balance<T>(this BinaryTreeNode<T> p) // балансировка узла p
            where T : IComparable<T>
        {
            p.FixHeight();
            var bFactor = p.BFactor();

            switch (bFactor)
            {
                case 2:
                    if (p.Right?.BFactor() < 0)
                        p.Right = p.Right.RotateRight();
                    return p.RotateLeft();
                case -2:
                    if (p.Left?.BFactor() > 0)
                        p.Left = p.Left.RotateLeft();
                    return p.RotateRight();
                default:
                    return p; // балансировка не нужна
            }
        }

        private static int BFactor<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
            => p.Right.Height() - p.Left.Height();

        private static BinaryTreeNode<T> FindMin<T>(this BinaryTreeNode<T> p) // поиск узла с минимальным ключом в дереве p 
            where T : IComparable<T>
        {
            return p.Left != null ? p.Left.FindMin() : p;
        }

        private static void FixHeight<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
        {
            var hl = p.Left.Height();
            var hr = p.Right.Height();
            p.Height = (hl > hr ? hl : hr) + 1;
        }

        private static int Height<T>(this BinaryTreeNode<T>? p)
            where T : IComparable<T>
            => p?.Height ?? 0;

        private static BinaryTreeNode<T>? RemoveMin<T>(this BinaryTreeNode<T> p) // удаление узла с минимальным ключом из дерева p
            where T : IComparable<T>
        {
            if (p.Left == null)
                return p.Right;
            p.Left = p.Left.RemoveMin();
            return p.Balance();
        }

        private static BinaryTreeNode<T> RotateLeft<T>(this BinaryTreeNode<T> q) // левый поворот вокруг q
            where T : IComparable<T>
        {
            var p = q.Right ?? throw new GraphException("Ошибка в алгоритме.");
            q.Right = p.Left;
            p.Left = q;
            q.FixHeight();
            p.FixHeight();
            return p;
        }

        private static BinaryTreeNode<T> RotateRight<T>(this BinaryTreeNode<T> p) // правый поворот вокруг p
            where T : IComparable<T>
        {
            var q = p.Left ?? throw new GraphException("Ошибка в алгоритме.");
            p.Left = q.Right;
            q.Right = p;
            p.FixHeight();
            q.FixHeight();
            return q;
        }

        private static bool HasChild<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T> =>
            p.Left != null || p.Right != null;

        private static int With<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
        {
            if (!p.HasChild())
                return 1;

            var wl = p.Left?.With() ?? 1;
            var wr = p.Right?.With() ?? 1;
            return 1 + wl + wr;
        }

        public static void Dump<T>(this BinaryTreeNode<T>? node, string indent)
            where T : IComparable<T>
        {
            indent = indent.Replace("└──└", "   └");
            indent = indent.Replace("└──├", "   ├");
            indent = indent.Replace("├──├", "│  ├");
            indent = indent.Replace("├──└", "│  └");

            if (node == null)
            {
                Console.WriteLine($"{indent}?");
                return;
            }

            Console.WriteLine($"{indent}{node.Key}:{node.With()}");

            if (!node.HasChild())
                return;

            Dump(node.Left, indent + "├──");
            Dump(node.Right, indent + "└──");
        }

        #endregion
    }
}