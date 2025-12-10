
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Y2025.Day09;
[ProblemName("Movie Theater")]

record Point (long x, long y);
record Rectangle(long top, long left, long bottom, long right);

class Solution : Solver {

    public object PartOne(string input) {
        var points = ParseInput(input);
        return RectanglesOrderedByArea(points)
                .Select(p => p.Area)
                .Max();
    }

    public object PartTwo(string input) {
        var points = ParseInput(input);

        //TODO: comeback to optimize/understand better why we need two rectangles here
        var segments = Boundary(points).ToArray();
        var reactangles = RectanglesOrderedByArea(points);

        return reactangles
            .Where(r => segments.All(s => !AabbCollision(r.Rect, s)))
            .Select(r => r.Area)
            .Max();
    }

    IEnumerable<(long Area, Rectangle Rect)> RectanglesOrderedByArea(Point[] points) =>
        points.SelectMany(p1 => points,
            (p1, p2) => {
                var rect = RectangleFromPoints(p1, p2);
                return (Area: Area(rect), Rect: rect);
            })
          .OrderByDescending(x => x.Area);

    static long Area(Rectangle r) => (r.bottom - r.top + 1) * (r.right - r.left + 1);

    static Point[] ParseInput(string input) =>
        input.Split('\n')
            .Select(line => line.Split(','))
            .Select(parts => new Point(long.Parse(parts[0]), long.Parse(parts[1])))
        .ToArray();

    static Rectangle RectangleFromPoints(Point p1, Point p2) {
        var top = Math.Min(p1.x, p2.x);
        var bottom = Math.Max(p1.x, p2.x);
        var left = Math.Min(p1.y, p2.y);
        var right = Math.Max(p1.y, p2.y);
        return new Rectangle((long)top, (long)left, (long)bottom, (long)right);
    }

    static bool AabbCollision(Rectangle a, Rectangle b) {
        var aIsToTheLeft = a.right <= b.left;
        var aIsToTheRight = a.left >= b.right;
        var aIsAbove = a.bottom <= b.top;
        var aIsBelow = a.top >= b.bottom;
        return !(aIsToTheRight || aIsToTheLeft || aIsAbove || aIsBelow);
    }

    IEnumerable<Rectangle> Boundary(Point[] corners) =>
        corners
            .Prepend(corners[^1])
            .Zip(corners, (prev, curr) => RectangleFromPoints(prev, curr));

}
