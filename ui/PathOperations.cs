using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ui
{
    public static class PathOperations
    {
        /// Indicates whether the given coordinate can be used as the next path element
        public static bool CoordIsAvailable(CellCoord candidate, Func<CellCoord, char?> grid, CellCoord last, string keyString)
        {
            string pathString = candidate.Equals(last) ? 
                keyString : // if same cell then will only have 1 elem path
                grid(last)! + keyString; // otherwise path begins with last coordinate
            return TraceLeg(last, candidate)
                .Take(pathString.Length)
                .Select((coord, index) =>
                {
                    var ch = grid(coord);
                    return ch == null || ch == pathString[index];
                })
                .All(x => x);
        }

        /// Excel-like column names.  E.g. A,B..Z,AA,AB..AZ,BA..ZZ,AAA,AAB
        public static string ColName(int position)
        {
            // 0 before A, but show Z instead of A0
            if (position == 0) return string.Empty;
            var mod = position % 26;
            var digit = (char) (mod + '@');
            var nextMagnitude = position / 26;
            if (mod == 0)
            {
                digit = 'Z';
                nextMagnitude--;
            }

            return ColName(nextMagnitude) + digit;
        }

        /// Enumerate all the possible subsequent cells in a path that has passed through 'previous' and 'current'.
        public static IEnumerable<CellCoord> Project(CellCoord previous, CellCoord current, int colCount, int rowCount)
        {
            throw new NotImplementedException("No need yet... this is an optimisation");
        }

        /// Trace the full path defined by the given coords
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<CellCoord> Trace(IEnumerable<CellCoord> coords)
        {
            return coords.Aggregate(Enumerable.Empty<CellCoord>(), (soFar, next) =>
            {
                CellCoord? last = soFar.LastOrDefault();
                var isSingleCellLeg = last == null || Equals(last, next);
                if (isSingleCellLeg)
                    return soFar.Append(next);
                return Enumerable.Concat(soFar, TraceLeg(last!, next).Skip(1));
            });
        }
        
       /// Return the series of coords plotting a deterministic path 'direct' between the two given coords
        public static IEnumerable<CellCoord> TraceLeg(CellCoord coordFrom, CellCoord coordTo)
        {
            if (coordFrom.Equals(coordTo))
                return new[] {coordTo};

            var ixRow = StepCloser(coordFrom.RowIndex, coordTo.RowIndex);
            var ixCol = StepCloser(coordFrom.ColIndex, coordTo.ColIndex);
            var altFrom = CellCoord.Create(ixCol, ixRow);
            var tail = TraceLeg(altFrom, coordTo);
            return tail.Prepend(coordFrom);
        }

        private static int StepCloser(int from, int to)
        {
            if (from == to) return from;
            if (from > to) return from - 1;
            return from + 1;
        }
    }
}