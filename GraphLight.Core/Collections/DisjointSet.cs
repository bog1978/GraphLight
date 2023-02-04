using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Collections
{
    internal class DisjointSet<TItem>
    {
        private readonly IDictionary<TItem, int> _index;
        private readonly int[] _parents;
        private readonly int[] _ranks;

        public DisjointSet(IReadOnlyCollection<TItem> items)
        {
            var i = 0;
            _index = items.ToDictionary(x => x, x => i++);
            _parents = Enumerable.Range(0, items.Count).ToArray();
            _ranks = new int[items.Count];
        }

        public int Find(TItem item)
        {
            return FindSetInternal(_index[item]);
        }

        public bool Unite(TItem x, TItem y)
        {
            var xSet = Find(x);
            var ySet = Find(y);
            if (xSet == ySet)
                return false;
            if (_ranks[xSet] < _ranks[ySet])
                (xSet, ySet) = (ySet, xSet);
            _parents[ySet] = xSet;
            if (_ranks[xSet] == _ranks[ySet])
                ++_ranks[xSet];
            return true;
        }

        private int FindSetInternal(int v)
        {
            // TODO: Передалать на цикл.
            if (v == _parents[v])
                return v;
            return _parents[v] = FindSetInternal(_parents[v]);
        }
    }
}
