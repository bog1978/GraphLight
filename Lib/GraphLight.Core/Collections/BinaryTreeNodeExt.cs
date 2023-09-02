using System;

namespace GraphLight.Collections
{
    public static class BinaryTreeNodeExt
    {
        #region Другое

        public static BinaryTreeNode<T> Insert<T>(this BinaryTreeNode<T> p, T k) // вставка ключа k в дерево с корнем p
        where T : IComparable<T>
        {
            if (p == null)
                return new BinaryTreeNode<T>(k);

            if (k.CompareTo(p.key) < 0)
                p.left = p.left.Insert(k);
            else
                p.right = p.right.Insert(k);

            return p.Balance();
        }

        public static BinaryTreeNode<T> Remove<T>(this BinaryTreeNode<T> p, T k) // удаление ключа k из дерева p
            where T : IComparable<T>
        {
            if (p == null)
                return null;

            var cmp = k.CompareTo(p.key);
            if (cmp < 0)
                p.left = p.left.Remove(k);
            else if (cmp > 0)
                p.right = p.right.Remove(k);
            else
            {
                var q = p.left;
                var r = p.right;
                if (r == null) return q;
                var min = r.FindMin();
                min.right = r.RemoveMin();
                min.left = q;
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
                    if (p.right.BFactor() < 0)
                        p.right = p.right.RotateRight();
                    return p.RotateLeft();
                case -2:
                    if (p.left.BFactor() > 0)
                        p.left = p.left.RotateLeft();
                    return p.RotateRight();
                default:
                    return p; // балансировка не нужна
            }
        }

        private static int BFactor<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
            => p.right.Height() - p.left.Height();

        private static BinaryTreeNode<T> FindMin<T>(this BinaryTreeNode<T> p) // поиск узла с минимальным ключом в дереве p 
            where T : IComparable<T>
        {
            return p.left != null ? p.left.FindMin() : p;
        }

        private static void FixHeight<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
        {
            var hl = p.left.Height();
            var hr = p.right.Height();
            p.height = (hl > hr ? hl : hr) + 1;
        }

        private static int Height<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
            => p?.height ?? 0;

        private static BinaryTreeNode<T> RemoveMin<T>(this BinaryTreeNode<T> p) // удаление узла с минимальным ключом из дерева p
            where T : IComparable<T>
        {
            if (p.left == null)
                return p.right;
            p.left = p.left.RemoveMin();
            return p.Balance();
        }

        private static BinaryTreeNode<T> RotateLeft<T>(this BinaryTreeNode<T> q) // левый поворот вокруг q
            where T : IComparable<T>
        {
            var p = q.right;
            q.right = p.left;
            p.left = q;
            q.FixHeight();
            p.FixHeight();
            return p;
        }

        private static BinaryTreeNode<T> RotateRight<T>(this BinaryTreeNode<T> p) // правый поворот вокруг p
            where T : IComparable<T>
        {
            var q = p.left;
            p.left = q.right;
            q.right = p;
            p.FixHeight();
            q.FixHeight();
            return q;
        }

        public static bool HasChild<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T> =>
            p?.left != null || p?.right != null;

        public static int With<T>(this BinaryTreeNode<T> p)
            where T : IComparable<T>
        {
            if (!p.HasChild())
                return 1;

            var wl = p.left?.With() ?? 1;
            var wr = p.right?.With() ?? 1;
            return 1 + wl + wr;
        }

        public static void Dump<T>(this BinaryTreeNode<T> node, string indent)
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

            Console.WriteLine($"{indent}{node.key}:{node.With()}");

            if (!node.HasChild())
                return;

            Dump(node.left, indent + "├──");
            Dump(node.right, indent + "└──");
        }

        #endregion
    }
}