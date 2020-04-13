using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Sae.Infra.Data.Extensions
{
    public static class PolylineExtension
    {
        /// <summary>
        /// Adds an arc (fillet), if able, at each polyline vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="radius">The arc radius.</param>
        public static void FilletAll(this Polyline pline, double radius)
        {
            var n = pline.Closed ? 0 : 1;
            for (var i = n; i < pline.NumberOfVertices - n; i += 1 + pline.FilletAt(i, radius))
            { }
        }

        /// <summary>
        /// Adds an arc (fillet) at the specified vertex.
        /// </summary>
        /// <param name="pline">The instance to which the method applies.</param>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="radius">The arc radius.</param>
        /// <returns>1 if the operation succeeded, 0 if it failed.</returns>
        public static int FilletAt(this Polyline pline, int index, double radius)
        {
            int prev = index == 0 && pline.Closed ? pline.NumberOfVertices - 1 : index - 1;
            if (pline.GetSegmentType(prev) != SegmentType.Line ||
                pline.GetSegmentType(index) != SegmentType.Line)
            {
                return 0;
            }
            var seg1 = pline.GetLineSegment2dAt(prev);
            var seg2 = pline.GetLineSegment2dAt(index);
            var vec1 = seg1.StartPoint - seg1.EndPoint;
            var vec2 = seg2.EndPoint - seg2.StartPoint;
            var angle = (Math.PI - vec1.GetAngleTo(vec2)) / 2.0;
            var dist = radius * Math.Tan(angle);
            if (dist == 0.0 || dist > seg1.Length || dist > seg2.Length)
            {
                return 0;
            }
            var pt1 = seg1.EndPoint + vec1.GetNormal() * dist;
            var pt2 = seg2.StartPoint + vec2.GetNormal() * dist;
            var bulge = Math.Tan(angle / 2.0);
            if (Clockwise(seg1.StartPoint, seg1.EndPoint, seg2.EndPoint))
            {
                bulge = -bulge;
            }
            pline.AddVertexAt(index, pt1, bulge, 0.0, 0.0);
            pline.SetPointAt(index + 1, pt2);
            return 1;
        }

        /// <summary>
        /// Evaluates if the points are clockwise.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <returns>true if points are clockwise, false otherwise.</returns>
        private static bool Clockwise(Point2d p1, Point2d p2, Point2d p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)) < 1e-8;
        }
    }
}