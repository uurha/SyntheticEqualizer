namespace SubModules.Splines
{
    public class CubicPoly1D
    {
        private float _c0;
        private float _c1;
        private float _c2;
        private float _c3;

        /*
         * Compute coefficients for a cubic polynomial
         *   p(s) = c0 + c1*s + c2*s^2 + c3*s^3
         * such that
         *   p(0) = x0, p(1) = x1
         *  and
         *   p'(0) = t0, p'(1) = t1.
         */
        public void Init(float x0, float x1, float t0, float t1)
        {
            _c0 = x0;
            _c1 = t0;
            _c2 = -3 * x0 + 3 * x1 - 2 * t0 - t1;
            _c3 = 2 * x0 - 2 * x1 + t0 + t1;
        }

        public void InitCatmullRom(float x0, float x1, float x2, float x3, float tension)
        {
            Init(x1, x2, tension * (x2 - x0), tension * (x3 - x1));
        }

        public void InitNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2)
        {
            // compute tangents when parameterized in [t1,t2]
            var t1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
            var t2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

            // rescale tangents for parametrization in [0,1]
            t1 *= dt1;
            t2 *= dt1;
            Init(x1, x2, t1, t2);
        }

        public float Calc(float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;
            return _c0 + _c1 * t + _c2 * t2 + _c3 * t3;
        }
    }
}
