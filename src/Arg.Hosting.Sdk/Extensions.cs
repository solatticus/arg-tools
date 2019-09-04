using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Arg.Hosting.Sdk
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Returns the first index of a series of matched bytes or -1 if there is no matching occurance. 
        /// </summary>
        /// <param name="buffer">Containing buffer.</param>
        /// <param name="pattern">The byte[] pattern to search for.</param>
        /// <returns>The index to the start of the provided byte[] pattern or -1 if it doesn't exist.</returns>
        unsafe public static int IndexOfSequence(this byte[] buffer, ref byte[] pattern)
        {
            fixed (byte* p = buffer)
            {
                return new ReadOnlySpan<byte>(p, buffer.Length).IndexOf(pattern);
            }
        }
    }
}
