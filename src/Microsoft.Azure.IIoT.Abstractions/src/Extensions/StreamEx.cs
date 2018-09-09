// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace System.IO {
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Stream extensions
    /// </summary>
    public static class StreamEx {

        /// <summary>
        /// Helper extension to convert an entire stream into a string...
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string ReadAsString(this Stream stream, Encoding encoder) {
            // Try to read as much as possible
            var buffer = stream.ReadAsBuffer();
            return encoder.GetString(buffer.Array, 0, buffer.Count);
        }

        /// <summary>
        /// Helper extension to convert an entire stream into a buffer...
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ArraySegment<byte> ReadAsBuffer(this Stream stream) {
            // Try to read as much as possible
            var body = new byte[1024];
            var offset = 0;
            try {
                while (true) {
                    var read = stream.Read(body, offset, body.Length - offset);
                    if (read == 0) {
                        break;
                    }

                    offset += read;
                    if (offset == body.Length) {
                        // Grow
                        var newbuf = new byte[body.Length * 2];
                        Buffer.BlockCopy(body, 0, newbuf, 0, body.Length);
                        body = newbuf;
                    }
                }
            }
            catch (IOException) { }
            return new ArraySegment<byte>(body, 0, offset);
        }

        /// <summary>
        /// Write remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static void Write(this Stream stream, byte[] buffer, int offset) =>
            stream.Write(buffer, offset, buffer.Length - offset);

        /// <summary>
        /// Write entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static void Write(this Stream stream, byte[] buffer) =>
            stream.Write(buffer, 0, buffer.Length);

        /// <summary>
        /// Write remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset) =>
            stream.WriteAsync(buffer, offset, buffer.Length - offset);

        /// <summary>
        /// Write entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Task WriteAsync(this Stream stream, byte[] buffer) =>
            stream.WriteAsync(buffer, 0, buffer.Length);

        /// <summary>
        /// Write remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset,
            CancellationToken ct) =>
            stream.WriteAsync(buffer, offset, buffer.Length - offset, ct);

        /// <summary>
        /// Write entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static Task WriteAsync(this Stream stream, byte[] buffer,
            CancellationToken ct) =>
            stream.WriteAsync(buffer, 0, buffer.Length, ct);

        /// <summary>
        /// Read remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int Read(this Stream stream, byte[] buffer, int offset) =>
            stream.Read(buffer, offset, buffer.Length - offset);

        /// <summary>
        /// Read entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static int Read(this Stream stream, byte[] buffer) =>
            stream.Read(buffer, 0, buffer.Length);

        /// <summary>
        /// Read remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset) =>
            stream.ReadAsync(buffer, offset, buffer.Length - offset);

        /// <summary>
        /// Read entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer) =>
            stream.ReadAsync(buffer, 0, buffer.Length);

        /// <summary>
        /// Read remaining buffer from offset
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset,
            CancellationToken ct) =>
            stream.ReadAsync(buffer, offset, buffer.Length - offset, ct);

        /// <summary>
        /// Read entire buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer,
            CancellationToken ct) =>
            stream.ReadAsync(buffer, 0, buffer.Length, ct);
    }
}
