using System.Collections.Generic;
using GraphLight.Algorithm;

namespace WpfSimplexSolver
{
    public class OptimizationTask
    {
        public string Name { get; private set; }
        public SimplexTask Task { get; private set; }
        public static readonly List<OptimizationTask> Values = new List<OptimizationTask>
            {
                new OptimizationTask {Name = "MIN", Task = SimplexTask.Minimize},
                new OptimizationTask {Name = "MAX", Task = SimplexTask.Maximize},
            };
    }
}