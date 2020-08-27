// TODO: Uncomment this line to generate unit tests (see output window)
// #define GENERATE_TEST

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphLight.Algorithm
{
    public class SimplexSolver
    {
        #region ����

        public readonly List<SimplexConstraint> Constraints = new List<SimplexConstraint>();
        public readonly List<int> Basis = new List<int>();

        /// <summary>
        /// ������� ������������� �����������
        /// </summary>
        private double[,] _constraintMatrix;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        private double[] _constraintValues;

        /// <summary>
        /// ���������� �����������
        /// </summary>
        private int _constraintCount;

        /// <summary>
        /// ���������� ����������, ������� ������������
        /// </summary>
        private int _variableCount;

        #endregion

        /// <summary>
        /// ��������� �����������
        /// </summary>
        /// <param name="operation">���� �� ������ �����������</param>
        /// <param name="b">��������� ����</param>
        /// <param name="a">������������ ��� �������� ����������</param>
        public void AddConstraint(SimplexOperation operation, double b, params double[] a)
        {
            Constraints.Add(new SimplexConstraint(operation, b, a));
            _constraintCount++;
        }

        /// <summary>
        /// ������ �� ������������ ������
        /// </summary>
        /// <param name="simplexTask">��� ������ - ������� ��� �������� ������� �������.</param>
        /// <param name="target">������������ ������� �������.</param>
        /// <returns>����������� ������� ������� �������.</returns>
        public double Solve(SimplexTask simplexTask, params double[] target)
        {
            dumpConstraints(target);

            // ����������� ����������� �����������
            foreach (var constraint in Constraints)
                constraint.SetSize(target.Length);

            makeCanonical(ref target);
            var basisTarget = createBasis(target.Length);
            _variableCount = basisTarget.Length;

            _constraintMatrix = new double[_constraintCount, _variableCount];
            _constraintValues = new double[_constraintCount];
            for (var i = 0; i < _constraintCount; i++)
            {
                var c = Constraints[i];
                _constraintValues[i] = c.B;
                for (var j = 0; j < c.A.Count; j++)
                    _constraintMatrix[i, j] = c.A[j];
            }

            // ��������� ������ ���� - ������� �����
            if (solve(SimplexTask.Minimize, basisTarget) != 0)
                throw new Exception("����� �� ������.");

            var result = solve(simplexTask, target);

            dumpSolutionCheck(result);
            return result;
        }

        /// <summary>
        /// ������� ���. ������� ������������ ����� � ������� �������� �����.
        /// </summary>
        /// <param name="varCount">���������� ����������, ������ � ������������.</param>
        private double[] createBasis(int varCount)
        {
            // ������� ������������ ����� � ������� �������
            var basisTarget = new double[varCount + _constraintCount];
            for (var i = 0; i < _constraintCount; i++)
            {
                var tmpBasis = new double[_constraintCount];
                tmpBasis[i] = 1;
                Constraints[i].A.AddRange(tmpBasis);
                Basis.Add(varCount + i);
                basisTarget[varCount + i] = 1;
            }
            return basisTarget;
        }

        /// <summary>
        /// �������� ����������� � ������������� ����.
        /// ��������� ������������ ����������.
        /// </summary>
        /// <param name="target">������������ ������� �������</param>
        private void makeCanonical(ref double[] target)
        {
            var cnt = 0;
            for (var i = 0; i < _constraintCount; i++)
            {
                var sign = Constraints[i].Sign;
                if (sign == SimplexOperation.Equal)
                    continue;
                for (var j = 0; j < _constraintCount; j++)
                    if (i != j)
                        Constraints[j].A.Add(0);
                    else
                        Constraints[j].A.Add(sign == SimplexOperation.Greater ? -1 : 1);
                cnt++;
            }

            // ������ ������������ ������� �������
            Array.Resize(ref target, target.Length + cnt);
        }

        /// <summary>
        /// ������ ����������� �������� ����������
        /// </summary>
        public double[] Solution
        {
            get
            {
                var varCount = _variableCount - _constraintCount;
                var solution = new double[varCount];
                for (var i = 0; i < _constraintCount; i++)
                    solution[Basis[i]] = _constraintValues[i];
                return solution;
            }
        }

        /// <summary>
        /// ������ ������ ����������� ��� ������������
        /// </summary>
        /// <param name="simplexTask"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private double solve(SimplexTask simplexTask, double[] target)
        {
            while (true)
            {
                var c_ = getRelatives(target);
                if (c_.All(simplexTask.CheckForSolution))
                    break;

                // ����� �� �������� ����������, ������� ����� ������ � �����
                var include = getVariablesToInclude(simplexTask, c_);

                // ����� �������� ����������, ������� ����� ������� �� ������
                var exclude = getVariablesToExclude(include);

                gaussStep(include, exclude);

                // ������ �����
                Basis[exclude] = include;
            }

            // ��������� �������� ������� �������
            var sum = Basis.Select((t, i) => target[t] * _constraintValues[i]).Sum();
            return sum;
        }

        /// <summary>
        /// ��������� ������ ������������� ������.
        /// </summary>
        /// <returns>������ ������������� ������</returns>
        private double[] getRelatives(double[] target)
        {
            var cb = new double[Basis.Count];
            for (var i = 0; i < Basis.Count; i++)
                cb[i] = target[Basis[i]];
            var rel = new double[target.Length];
            for (var i = 0; i < target.Length; i++)
            {
                var t = target[i];
                var j = 0;
                for (var row = 0; row < _constraintCount; row++)
                    t -= cb[j++] * _constraintMatrix[row, i];
                rel[i] = t;
            }
            return rel;
        }

        /// <summary>
        /// 5. ������� ����������, ������� ����� ������� �� ������
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        private int getVariablesToExclude(int include)
        {
            var b_ = new double[_constraintCount];
            for (var i = 0; i < _constraintCount; i++)
            {
                if (_constraintMatrix[i, include] <= 0)
                    b_[i] = double.MaxValue;
                else
                    b_[i] = _constraintValues[i] / _constraintMatrix[i, include];
            }

            var min = double.MaxValue;
            var mins = new List<int>();
            for (var i = 0; i < _constraintCount; i++)
            {
                if (min > b_[i])
                {
                    min = b_[i];
                    mins.Clear();
                    mins.Add(i);
                }
                else if (min == b_[i])
                    mins.Add(i);
            }
            return mins[0];
        }

        /// <summary>
        /// 6. ������ ������� ������� � 1, � ��������� �������� ������� � 0
        /// </summary>
        /// <param name="include">����� �� �������� ����������, ������� ����� ������ � �����</param>
        /// <param name="exclude">������ (� ������� Basis) ������ �������� ����������, ������� ����� ������� �� ������</param>
        private void gaussStep(int include, int exclude)
        {
            var lead = _constraintMatrix[exclude, include];

            // ������ ������� ������� � 1
            if (lead != 1)
                mulRow(exclude, 1 / lead);

            // ������ ��������� �������� ������� � 0
            for (var i = 0; i < _constraintCount; i++)
                if (i != exclude && _constraintMatrix[i, include] != 0)
                    addRow(i, exclude, -_constraintMatrix[i, include]);
        }

        private void addRow(int row, int rowToAdd, double k)
        {
            for (var col = 0; col < _variableCount; col++)
                _constraintMatrix[row, col] += _constraintMatrix[rowToAdd, col] * k;
            _constraintValues[row] += _constraintValues[rowToAdd] * k;
        }

        public void mulRow(int row, double k)
        {
            for (var col = 0; col < _variableCount; col++)
                _constraintMatrix[row, col] *= k;
            _constraintValues[row] *= k;
        }

        /// <summary>
        /// ���������, ���������� �� �������. ���� � �������� �������
        /// ����������� ��� �� ������ �������� ������ 0, �� ������� ���.
        /// </summary>
        /// <param name="j">����� �������, ������� �������������� ������ � �����.</param>
        /// <returns></returns>
        private bool hasSolution(int j)
        {
            for (var i = 0; i < _constraintCount; i++)
                if (_constraintMatrix[i, j] > 0)
                    return true;
            return false;
            //return Constraints.Any(t => t.A[j] > 0);
        }

        /// <summary>
        /// ����� �� �������� ����������, ������� ����� ������ � �����
        /// </summary>
        /// <param name="simplexTask">��� ������ (min/max)</param>
        /// <param name="arr">������ ������������� ������</param>
        /// <returns></returns>
        private int getVariablesToInclude(SimplexTask simplexTask, double[] arr)
        {
            var include = simplexTask.InitialValue;
            var includes = new List<int>();
            var sl = Basis.ToArray();
            Array.Sort(sl);
            for (var i = 0; i < arr.Length; i++)
            {
                // �������� ������� ����������.
                if (Array.BinarySearch(sl, i) > 0)
                    continue;
                if (simplexTask.CheckForInclude(include, arr[i]))
                {
                    include = arr[i];
                    includes.Clear();
                    includes.Add(i);
                }
                else if (include == arr[i])
                    includes.Add(i);
            }

            return includes.First(hasSolution);
        }

        #region Unit test generator

        [Conditional("GENERATE_TEST")]
        private void dumpConstraints(IEnumerable<double> target)
        {
            Debug.WriteLine("[TestMethod] public void Test1() {");
            Debug.WriteLine("// Constraints");
            Debug.WriteLine("var solver = new SimplexSolver();");
            foreach (var cons in Constraints)
            {
                var a = string.Join(", ", cons.A);
                Debug.WriteLine("solver.AddConstraint(SimplexOperation.{0}, {1}, {2});", cons.Sign, cons.B, a);
            }

            Debug.WriteLine("");
            Debug.WriteLine("// Calculate solution");
            var t = string.Join(", ", target);
            Debug.WriteLine("var target = new double[] {{{0}}};", new[] { t });
            Debug.WriteLine("var result = solver.Solve(SimplexTask.Minimize, target);");
        }

        [Conditional("GENERATE_TEST")]
        private void dumpSolutionCheck(double result)
        {
            Debug.WriteLine("");
            Debug.WriteLine("// Result verification");
            Debug.WriteLine("Assert.AreEqual({0}, result, 0.000001);", result);
            var s = string.Join(", ", Solution);
            Debug.WriteLine("var expected = new[] {{ {0} }};", new[] { s });
            Debug.WriteLine("Assert.AreEqual(expected.Length, solver.Solution.Length, \"Collections must of equal size.\");");
            Debug.WriteLine("for (var i = 0; i < expected.Length; i++)");
            Debug.WriteLine("    Assert.AreEqual(expected[i], solver.Solution[i], 0.000001, \"Differet items at index \" + i);");
            Debug.WriteLine("}");
        }

        #endregion
    }
}


