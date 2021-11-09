using System;
using System.Linq;
using Extensions;
using UnityEngine;

namespace SubModules.Splines
{
    public enum SplineType
    {
        Centripetal,
        Chordal,
        Catmullrom
    }

    [Serializable]
    public class SplineCurve : MonoBehaviour
    {
        [SerializeField] private Vector3[] controlPoints;
        [SerializeField] private bool isClosed;
        [SerializeField] private SplineType type;
        [SerializeField] private float tensionValue;
        [Min(2)] [SerializeField] private int resolution = 2;

        private float[] _cacheArcLengths;
        private CurvePoint[] _cachedPoints = new CurvePoint[0];
        protected bool _needsUpdate;
        private bool _dirty = true;

        public Vector3[] GetControlPoints()
        {
            return controlPoints;
        }

        public void SetDirty()
        {
            _dirty = true;
        }

        public void SetControlPoints(Vector3[] points)
        {
            controlPoints = points;
            SetDirty();
        }

        public CurvePoint GetPointAt(float u)
        {
            var t = GetUtoTMapping(u);
            return GetPoint(t, false);
        }

        public CurvePoint[] GetPoints(bool isWorld)
        {
            var points = _cachedPoints;
            if (_dirty) CachePoints();

            if (isWorld)
            {
                points = _cachedPoints.Select(x => new CurvePoint()
                                                   {
                                                       normal = x.normal,
                                                       position = x.position.TransformPoint(this),
                                                       tangent = x.tangent
                                                   }).ToArray();
            }
            return points;
        }

        private void CachePoints()
        {
            _dirty = false;
            _cachedPoints = new CurvePoint[resolution + 1];
            for (var d = 0; d <= resolution; d++) _cachedPoints[d] = GetPoint((float)d / resolution, false);
        }

        public CurvePoint[] GetPoints(float from, float to)
        {
            var curvePoints = new CurvePoint[resolution + 1];
            var delta = (to - from);

            for (var d = 0; d <= resolution; d++)
            {
                var t = (float)d / resolution * delta;
                curvePoints[d] = GetPoint(t, false);
            }
            return curvePoints;
        }

        public CurvePoint[] GetSpacedPoints()
        {
            var spacedPoints = new CurvePoint[resolution + 1];
            for (var d = 0; d <= resolution; d++) spacedPoints[d] = GetPointAt((float)d / resolution);
            return spacedPoints;
        }

        /// <summary>
        /// Get total curve arc length
        /// </summary>
        /// <param name="divisions"></param>
        /// <returns></returns>
        public float GetLength(int divisions = 200)
        {
            var lengths = GetLengths(divisions);
            return lengths[lengths.Length - 1];
        }

        /// <summary>
        /// Get list of cumulative segment lengths
        /// </summary>
        /// <param name="divisions"></param>
        /// <returns></returns>
        public float[] GetLengths(int divisions = 200)
        {
            if (_cacheArcLengths != null &&
                _cacheArcLengths.Length == divisions + 1 &&
                !_needsUpdate)
                return _cacheArcLengths;
            _needsUpdate = false;
            var cache = new float[divisions + 1];
            var last = GetVectorPoint(0);
            float sum = 0;
            cache[0] = 0;

            for (var p = 1; p <= divisions; p++)
            {
                var current = GetVectorPoint((float)p / divisions);
                sum += Vector3.Distance(current, last);
                cache[p] = (sum);
                last = current;
            }
            _cacheArcLengths = cache;
            return cache; // { sums: cache, sum: sum }; Sum is in the last element.
        }

        public void UpdateArcLengths()
        {
            _needsUpdate = true;
            GetLengths();
        }

        // Given u ( 0 .. 1 ), get a t to find p. This gives you points which are equidistant

        public float GetUtoTMapping(float u, float distance = 0)
        {
            var arcLengths = GetLengths();
            int i;
            var il = arcLengths.Length;
            float targetArcLength; // The targeted u distance value to get

            if (distance != 0)
                targetArcLength = distance;
            else
                targetArcLength = u * arcLengths[il - 1];

            // binary search for the index with largest value smaller than target u distance
            var low = 0;
            var high = il - 1;

            while (low <= high)
            {
                i = Mathf.FloorToInt((low + (high - low)) / 2f);
                var comparison = arcLengths[i] - targetArcLength;

                if (comparison < 0)
                {
                    low = i + 1;
                }
                else if (comparison > 0)
                {
                    high = i - 1;
                }
                else
                {
                    high = i;
                    break;
                }
            }
            i = high;
            if (Math.Abs(arcLengths[i] - targetArcLength) < Mathf.Epsilon) return i / (float)(il - 1);

            // we could get finer grain at lengths, or use simple interpolation between two points
            var lengthBefore = arcLengths[i];
            var lengthAfter = arcLengths[i + 1];
            var segmentLength = lengthAfter - lengthBefore;

            // determine where we are between the 'before' and 'after' points
            var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

            // add that fractional amount to t
            var t = (i + segmentFraction) / (il - 1);
            return t;
        }

        public Vector3 GetNormal(float t)
        {
            var tangent = GetTangent(t);
            var normal = Vector3.Cross(tangent, Vector3.right).normalized;
            return normal;
        }

        public Vector3 GetTangent(float t)
        {
            const float delta = 0.0001f;
            var t1 = t - delta;
            var t2 = t + delta;

            // Capping in case of danger
            if (t1 < 0) t1 = 0;
            if (t2 > 1) t2 = 1;
            var pt1 = GetVectorPoint(t1);
            var pt2 = GetVectorPoint(t2);
            var tangent = (pt2 - pt1).normalized;
            return tangent;
        }

        public Vector3 GetTangentAt(float u)
        {
            var t = GetUtoTMapping(u);
            return GetTangent(t);
        }

        public CurvePoint GetPoint(float t, bool isWorld)
        {
            var localPoint = GetVectorPoint(t);

            return new CurvePoint()
                   {
                       position = isWorld ? localPoint.TransformPoint(this) : localPoint,
                       tangent = GetTangent(t),
                       normal = GetNormal(t)
                   };
        }

        public Vector3 GetVectorPoint(float t)
        {
            var l = controlPoints.Length;
            var p = (l - (isClosed ? 0 : 1)) * t;
            var intPoint = Mathf.FloorToInt(p);
            var weight = p - intPoint;

            if (isClosed)
            {
                intPoint += intPoint > 0 ? 0 : (Mathf.FloorToInt(Mathf.Abs(intPoint) / (float)l) + 1) * l;
            }
            else if (weight == 0 &&
                     intPoint == l - 1)
            {
                intPoint = l - 2;
                weight = 1;
            }

            var p0 = isClosed || intPoint > 0
                         ? controlPoints[(intPoint - 1) % l].TransformPoint(this)
                         : (controlPoints[0].TransformPoint(this) - controlPoints[1].TransformPoint(this)) * 2 +
                           controlPoints[0].TransformPoint(this);
            var p1 = controlPoints[intPoint % l].TransformPoint(this);
            var p2 = controlPoints[(intPoint + 1) % l].TransformPoint(this);

            var p3 = isClosed || intPoint + 2 < l
                         ? controlPoints[(intPoint + 2) % l].TransformPoint(this)
                         : controlPoints[l - 1].TransformPoint(this) - controlPoints[l - 2].TransformPoint(this) +
                           controlPoints[l - 1].TransformPoint(this);
            var px = new CubicPoly1D();
            var py = new CubicPoly1D();
            var pz = new CubicPoly1D();

            switch (type)
            {
                case SplineType.Centripetal:
                case SplineType.Chordal:
                {
                    // init Centripetal / Chordal Catmull-Rom
                    var pow = type == SplineType.Chordal ? 0.5f : 0.25f;
                    var dt0 = Mathf.Pow((p0 - p1).sqrMagnitude, pow);
                    var dt1 = Mathf.Pow((p1 - p2).sqrMagnitude, pow);
                    var dt2 = Mathf.Pow((p2 - p3).sqrMagnitude, pow);

                    // safety check for repeated points
                    const float delta = 0.0001f;
                    if (dt1 < delta) dt1 = 1.0f;
                    if (dt0 < delta) dt0 = dt1;
                    if (dt2 < delta) dt2 = dt1;
                    px.InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2);
                    py.InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2);
                    pz.InitNonuniformCatmullRom(p0.z, p1.z, p2.z, p3.z, dt0, dt1, dt2);
                    break;
                }
                case SplineType.Catmullrom:
                    px.InitCatmullRom(p0.x, p1.x, p2.x, p3.x, tensionValue);
                    py.InitCatmullRom(p0.y, p1.y, p2.y, p3.y, tensionValue);
                    pz.InitCatmullRom(p0.z, p1.z, p2.z, p3.z, tensionValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var position = new Vector3(px.Calc(weight), py.Calc(weight), pz.Calc(weight));
            return transform.InverseTransformPoint(position);
        }
    }
}
