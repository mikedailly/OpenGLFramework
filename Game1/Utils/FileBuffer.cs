using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Utils;

namespace FrameWork.Utils
{
    // ########################################################################################################
    /// <summary>
    ///     Simple file buffer access
    /// </summary>
    // ########################################################################################################
    public class FileBuffer
    {
        /// <summary>Default buffer size</summary>
        public const int DEFAULT_SIZE = 1024 * 1024;

        /// <summary>The actual buffer</summary>
        public byte[] buffer = null;

        /// <summary>The file name</summary>
        string file = "";

        /// <summary>The current file index for streaming data from</summary>
        public int CurrentIndex;

        /// <summary>Allow buffer to grow on write?</summary>
        public bool resize;

        #region Create class
        // ########################################################################################################
        /// <summary>
        ///     Create a new empty buffer
        /// </summary>
        // ########################################################################################################
        public FileBuffer()
        {
            buffer = new byte[DEFAULT_SIZE];
            CurrentIndex = 0;
            resize = true;
        }

        // ########################################################################################################
        /// <summary>
        ///     Create a new Filebuffer of a certain size
        /// </summary>
        /// <param name="_buffer">size of buffer to create</param>
        // ########################################################################################################
        public FileBuffer(int _size)
        {
            buffer = new byte[_size];
            resize = true;
            CurrentIndex = 0;
        }

        // ########################################################################################################
        /// <summary>
        ///     Create a new Filebuffer - from a byte array
        /// </summary>
        /// <param name="_buffer">Buffer to use as file</param>
        // ########################################################################################################
        public FileBuffer(byte[] _buffer)
        {
            buffer = _buffer;
            CurrentIndex = 0;
            resize = false;
        }
        #endregion

        #region utils
        // ########################################################################################################
        /// <summary>
        ///     show buffer as string (debug)
        /// </summary>
        // ########################################################################################################
        public string TextFile
        {
            get
            {
                if (buffer == null) return "";
                if (file == "")
                {
                    file = BufferToString(buffer);
                }
                return file;
            }
        }
        // ########################################################################################################
        /// <summary>
        ///     Return a byte from the file/buffer - no bounds check/resize
        /// </summary>
        // ########################################################################################################
        byte this[int _index]
        {
            get { return buffer[_index]; }
            set { buffer[_index] = value; }
        }

        // ########################################################################################################
        /// <summary>
        ///     Length of file/buffer
        /// </summary>
        // ########################################################################################################
        public int Length
        {
            get { return buffer.Length; }
        }

        // ########################################################################################################
        /// <summary>
        ///     End of file/buffer?
        /// </summary>
        // ########################################################################################################
        public bool EOF
        {
            get
            {
                if (CurrentIndex >= buffer.Length) return true;
                return false;
            }
        }

        // ########################################################################################################
        /// <summary>
        ///     Convert a byte buffer to a string
        /// </summary>
        /// <param name="_buffer">buffer to convert</param>
        /// <returns>
        ///     string
        /// </returns>
        // ########################################################################################################
        public static string BufferToString(byte[] _buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _buffer.Length; i++)
            {
                sb.Append((char)_buffer[i]);
            }
            return sb.ToString();
        }
        #endregion

        #region resize
        // ########################################################################################################
        /// <summary>
        ///     Check to see if we're allowed to resize, and if we need to
        /// </summary>
        /// <param name="_space_needed">amount of space needed</param>
        /// <returns>
        ///     true for space, false for no space
        /// </returns>
        // ########################################################################################################
        public bool CheckReseize(int _space_needed = 1)
        {
            if (!resize)
            {
                if ((CurrentIndex + _space_needed) >= buffer.Length) return false;
                return true;
            }

            if ((CurrentIndex + _space_needed) >= buffer.Length)
            {
                byte[] buff = new byte[buffer.Length + (buffer.Length >> 2)];           // add 1/4 on...
                Array.Copy(buffer, buff, buffer.Length);
                buffer = buff;
                return true;
            }
            return true;
        }
        #endregion

        #region Read
        // ########################################################################################################
        /// <summary>
        ///     Get the next byte from the buffer, and advance
        /// </summary>
        /// <returns>
        ///     Next byte (or 0 if past the end)
        /// </returns>
        // ########################################################################################################
        public byte NextByte()
        {
            if (CurrentIndex >= buffer.Length) return 0;       // out of bounds
            byte b = buffer[CurrentIndex++];
            return b;
        }


        // ########################################################################################################
        /// <summary>
        ///     PEEK the next byte from the buffer - do not advance
        /// </summary>
        /// <returns>
        ///     Next byte (or 0x00 if past the end)
        /// </returns>
        // ########################################################################################################
        public byte PeekByte()
        {
            if (CurrentIndex >= buffer.Length) return 0;       // out of bounds
            byte b = buffer[CurrentIndex];
            return b;
        }

        // ########################################################################################################
        /// <summary>
        ///     Read a byte from the stream
        /// </summary>
        /// <return>Next byte from buffer</return>
        // ########################################################################################################
        public int Read8()
        {
            if (CurrentIndex >= buffer.Length) return 0;       // out of bounds
            return buffer[CurrentIndex++];
        }

        // ########################################################################################################
        /// <summary>
        ///     Read a 16bit int from the stream
        /// </summary>
        /// <param name="_add">address/index into file</param>
        /// <param name="_w">word to write</param>
        // ########################################################################################################
        public int Read16()
        {
            if ((CurrentIndex + 2) >= buffer.Length) return 0;       // out of bounds

            int a = buffer[CurrentIndex++];
            a |= (buffer[CurrentIndex++] << 8);

            return a;
        }


        // ########################################################################################################
        /// <summary>
        ///     Read a 24 bit int from the stream
        /// </summary>
        /// <param name="_add">address/index into file</param>
        /// <param name="_w">word to write</param>
        // ########################################################################################################
        public int Read24()
        {
            if ((CurrentIndex + 3) >= buffer.Length) return 0;       // out of bounds

            int a = buffer[CurrentIndex++];
            a |= (buffer[CurrentIndex++] << 8);
            a |= (buffer[CurrentIndex++] << 16);

            return a;
        }


        // ########################################################################################################
        /// <summary>
        ///     Read a 32 bit int from the stream
        /// </summary>
        /// <param name="_add">address/index into file</param>
        /// <param name="_w">word to write</param>
        // ########################################################################################################
        public int Read32()
        {
            if ((CurrentIndex + 4) >= buffer.Length) return 0;       // out of bounds

            int a = buffer[CurrentIndex++];
            a |= (buffer[CurrentIndex++] << 8);
            a |= (buffer[CurrentIndex++] << 16);
            a |= (buffer[CurrentIndex++] << 24);

            return a;
        }
        #endregion

        #region Write
        // ########################################################################################################
        /// <summary>
        ///     Poke a byte into the file space
        /// </summary>
        /// <param name="_add">address/index into file</param>
        /// <param name="_b">byte to write</param>
        // ########################################################################################################
        public void Poke8(int _add, int _b)
        {
            if (!CheckReseize(1)) return;
            buffer[_add] = (byte)(_b & 0xff);
        }

        // ########################################################################################################
        /// <summary>
        ///     Poke a word into the file space
        /// </summary>
        /// <param name="_add">address/index into file</param>
        /// <param name="_w">word to write</param>
        // ########################################################################################################
        public void Poke16(int _add, int _w)
        {
            if (!CheckReseize(2)) return;

            buffer[_add] = (byte)(_w & 0xff);
            buffer[_add + 1] = (byte)((_w >> 8) & 0xff);
        }


        // ########################################################################################################
        /// <summary>
        ///     Write a byte out to the file
        /// </summary>
        /// <param name="_b">byte to write</param>
        // ########################################################################################################
        public void Write8(int _b)
        {
            if (!CheckReseize(1)) return;
            buffer[CurrentIndex++] = (byte)(_b & 0xff);
        }

        // ########################################################################################################
        /// <summary>
        ///     Write a 16bit value out to the file
        /// </summary>
        /// <param name="_w">word to write</param>
        // ########################################################################################################
        public void Write16(int _w)
        {
            if (!CheckReseize(2)) return;
            buffer[CurrentIndex++] = (byte)(_w & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 8) & 0xff);
        }

        // ########################################################################################################
        /// <summary>
        ///     Write a 24bit value out to the file
        /// </summary>
        /// <param name="_w">24bit int to write</param>
        // ########################################################################################################
        public void Write24(int _w)
        {
            if (!CheckReseize(3)) return;
            buffer[CurrentIndex++] = (byte)(_w & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 8) & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 16) & 0xff);
        }


        // ########################################################################################################
        /// <summary>
        ///     Write a 32bit value out to the file
        /// </summary>
        /// <param name="_w">32bit int to write</param>
        // ########################################################################################################
        public void Write32(int _w)
        {
            if (!CheckReseize(4)) return;
            buffer[CurrentIndex++] = (byte)(_w & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 8) & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 16) & 0xff);
            buffer[CurrentIndex++] = (byte)((_w >> 24) & 0xff);
        }
        #endregion        

        #region Load / Save
        // ########################################################################################################
        /// <summary>
        ///     Load a file and return a buffer
        /// </summary>
        /// <param name="_name">name of file to load</param>
        /// <returns>
        ///     A filebuffer or null;
        /// </returns>
        // ########################################################################################################
        public static FileBuffer Load(string _name)
        {
            if (System.IO.File.Exists(_name))
            {
                byte[] b = System.IO.File.ReadAllBytes(_name);
                return new FileBuffer(b);
            }
            return null;
        }


        // ########################################################################################################
        /// <summary>
        ///     Save a buffer (part or whole) to a file
        /// </summary>
        /// <param name="_name">name of file</param>
        /// <param name="_data">buffer to save</param>
        /// <param name="_offset">offset into buffer</param>
        /// <param name="_size">number of bytes to write</param>
        // ########################################################################################################
        public void Save(string _name, int _offset, int _size = -1)
        {
            if (_size == -1) _size = CurrentIndex;
            byte[] buff = new byte[_size];
            Buffer.BlockCopy(buffer, _offset, buff, 0, _size);

            try
            {
                System.IO.File.WriteAllBytes(_name, buff);
            }
            catch (Exception ex)
            {
                Log.WriteLine("Error saving file - '" + _name + "',  " + ex.Message);
                return;
            }
        }
        #endregion

    }
}
