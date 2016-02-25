using System.Collections.Generic;
using GraphLight.Algorithm;

namespace WpfSimplexSolver
{
    public class RestrictionSign
    {
        public static readonly List<RestrictionSign> Values = new List<RestrictionSign>
            {
                new RestrictionSign("=", SimplexOperation.Equal),
                new RestrictionSign("<=", SimplexOperation.Less),
                new RestrictionSign(">=", SimplexOperation.Greater),
            };

        private RestrictionSign(string name, SimplexOperation operation)
        {
            Name = name;
            Operation = operation;
        }

        public string Name { get; private set; }
        public SimplexOperation Operation { get; private set; }
    }
}