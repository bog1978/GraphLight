namespace GraphLight.Algorithm
{
    public abstract class SimplexTask
    {
        public static readonly SimplexTask Maximize = new TaskMax();
        public static readonly SimplexTask Minimize = new TaskMin();

        public abstract double InitialValue { get; }
        public abstract bool CheckForInclude(double current, double testing);
        public abstract bool CheckForSolution(double x);

        #region Nested type: TaskMax

        private sealed class TaskMax : SimplexTask
        {
            public override double InitialValue => double.MinValue;

            public override bool CheckForInclude(double current, double testing) => current < testing;

            public override bool CheckForSolution(double x) => x <= 0;
        }

        #endregion

        #region Nested type: TaskMin

        private sealed class TaskMin : SimplexTask
        {
            public override double InitialValue => double.MaxValue;

            public override bool CheckForInclude(double current, double testing) => current > testing;

            public override bool CheckForSolution(double x) => x >= 0;
        }

        #endregion
    }
}