using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ui
{
    public static class PathOperations
    {
        private const int AsciiAt = '@';

        /// Excel-like column names.  E.g. A,B..Z,AA,AB..AZ,BA..ZZ,AAA,AAB
        public static string ColName(int position) {
            // 0 before A, but show Z instead of A0
            if (position == 0) return string.Empty;
            var mod = position % 26;
            var digit = (char) (mod + AsciiAt);
            var nextMagnitude = position / 26;
            if (mod == 0) {
                digit = 'Z';
                nextMagnitude--;
            }
            return ColName(nextMagnitude) + digit;
        }

        /// Return the series of coords plotting a deterministic path 'direct' between the two given coords
        public static IEnumerable<CellCoord> Trace(CellCoord coordFrom, CellCoord coordTo)
        {
            if (coordFrom.Equals(coordTo))
                return new[] {coordTo};
            
            var ixRow = StepCloser(coordFrom.RowIndex, coordTo.RowIndex);
            var ixCol = StepCloser(coordFrom.ColIndex, coordTo.ColIndex);
            var altFrom = CellCoord.Create(ixCol, ixRow);
            var tail = Trace(altFrom, coordTo);
            return tail.Prepend(coordFrom);
        }

        private static int StepCloser(int from, int to) {
            if (from == to) return from;
            if (from > to) return from - 1;
            return from + 1;
        }        
    }
}