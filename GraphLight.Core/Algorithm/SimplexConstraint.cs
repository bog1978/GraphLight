using System;
using System.Collections.Generic;

namespace GraphLight.Algorithm
{
    public class SimplexConstraint
    {
        public List<double> A = new List<double>();
        public double B;
        public SimplexOperation Sign;

        public SimplexConstraint(SimplexOperation sign, double b, params double[] a)
        {
            A.AddRange(a);
            B = b;
            Sign = sign;
        }

        public void SetSize(int size)
        {
            if (A.Count > size)
                throw new Exception("Can't trim constraint size");
            if (A.Count < size)
                A.AddRange(new double[size - A.Count]);
        }
    }
}