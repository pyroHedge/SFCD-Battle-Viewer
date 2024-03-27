using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroPatchViewer
{
    /// <summary>
    /// General holder for byte data.
    /// </summary>
    public class ByteChunk
    {
        public byte[] Bytes { get; set; }
        public int Start { get; set; }
        public int Length { get { return Bytes.Length; } }
        public int End { get { return Start + Length - 1; } }
        
        public ByteChunk(IEnumerable<byte> bytes, int start = 0)
        {
            Bytes = bytes.ToArray();
            Start = start;
        }

        public ByteChunk(IEnumerable<byte> data, int start, int length)
        {
            Bytes = data.Take(new Range(start, start + length)).ToArray(); //this won't error and will cease grabbing elements if the length of data is exceeded
            Start = start;
        }

        /// <summary>
        /// Get bytes based upon the location as defined by the byte chunk.
        /// </summary>
        /// <param name="location">The first byte to get.</param>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>Array of bytes starting at location.</returns>
        public byte[] GetDataByLocation(int location, int length)
        {
            length = Math.Max(0, Math.Min(length - 1, End - location)); //error handling
            return Bytes.Take(new Range(location - Start, location - Start + length)).ToArray();
        }

        /// <summary>
        /// Get how many bytes offset from zero is the Start location of the ByteChunk.
        /// </summary>
        /// <param name="alignBytes">Bytes per "row" of display data, or grouping of bytes in a file.</param>
        /// <returns>Number of bytes between zero and Start mod alignSize.</returns>
        public int GetOffset(int alignSize = 16)
        {
            return Start % alignSize;
        }

        /// <summary>
        /// Get bytes from the ByteChunk, returned as an array.
        /// </summary>
        /// <param name="start">The first byte to get.</param>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>Array of bytes.</returns>
        public byte[] GetBytes(int start, int length)
        {
            return Bytes.Take(new Range(start, start + length - 1)).ToArray();
        }
    }
}
