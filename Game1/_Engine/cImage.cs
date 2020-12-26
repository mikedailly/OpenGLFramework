// **********************************************************************************************************************
// 
// Copyright (c)2020, Ogre Games Ltd. All Rights reserved.
// 
//  "Install-Package OpenTK -Version 3.0.1"
// 
// Date				Version		Comment
// ----------------------------------------------------------------------------------------------------------------------
// 26/12/2020		V1.0        1st version
// 
// **********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public unsafe class cImage
    {
        public string FullFilename;
        public string Filename;
        public int TileIndex;
        public UInt32[] Raw;
        public int Width;
        public int Height;


        // #############################################################################################
        /// <summary>
        ///     Get/Set array access into the image
        /// </summary>
        /// <param name="_x">x coordinate</param>
        /// <param name="_y">y coordinate</param>
        /// <returns>pixel</returns>
        // #############################################################################################
        public uint this[int _x, int _y]
        {
            get
            {
                return Raw[_x + (_y * Width)];

            }
            set
            {
                Raw[_x + (_y * Width)] = value;
            }
            // get and set accessors  
        }

        // #############################################################################################
        /// Constructor: <summary>
        ///              	Create a new tile holder
        ///              </summary>
        ///
        /// In:		<param name="_filename">full path+filename to tile.png</param>
        ///
        // #############################################################################################
        public unsafe cImage(int _w, int _h)
        {
            Raw = new uint[_w * _h];
            Width = _w;
            Height = _h;
        }
        // #############################################################################################
        /// Constructor: <summary>
        ///              	Create a new tile holder
        ///              </summary>
        ///
        /// In:		<param name="_filename">full path+filename to tile.png</param>
        ///
            // #############################################################################################
        public unsafe cImage(string _filename)
        {
            FullFilename = _filename;

            if (!File.Exists(_filename)) return;

            // Now load the PNG and get the pixels from it....
            Bitmap pngImage = new Bitmap(FullFilename);
            Raw = new UInt32[pngImage.Width * pngImage.Height];
            Width = pngImage.Width;
            Height = pngImage.Height;

            var data = pngImage.LockBits(
                        new Rectangle(0, 0, pngImage.Width, pngImage.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        pngImage.PixelFormat);
            //System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte* pData = (byte*)data.Scan0;
            int index = 0;
            for (int y = 0; y < pngImage.Height; y++)
            {
                for (int x = 0; x < pngImage.Width; x++)
                {
                    UInt32 col;
                    int ind = (y * data.Stride);
                    if (pngImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    {
                        ind += x * 3;
                        col = 0xff000000 | ((UInt32)pData[ind] + (UInt32)(pData[ind + 1] << 8) + (UInt32)(pData[ind + 2] << 16));
                    }
                    else
                    {
                        ind += x * 4;
                        col = (UInt32)pData[ind] + (UInt32)(pData[ind + 1] << 8) + (UInt32)(pData[ind + 2] << 16) + (UInt32)(pData[ind + 3] << 24);
                    }
                    Raw[index++] = col;
                }
            }
            pngImage.UnlockBits(data);
        }


        // #############################################################################################
        /// Constructor: <summary>
        ///              	Save image to PNG file
        ///              </summary>
        ///
        /// In:		<param name="_filename">full path+filename to save as</param>
        ///
        // #############################################################################################
        public unsafe void Save(string _filename)
        {
            FullFilename = _filename;
            Bitmap pngImage = null;
            // Now load the PNG and get the pixels from it....
            fixed (uint* pData = &Raw[0])
            {
                IntPtr data = (IntPtr)pData;

                pngImage = new Bitmap(Width, Height, Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, data);
            }
            pngImage.Save(_filename);
        }

        // #############################################################################################
        /// Function:<summary>
        ///          	Show nicer debug info
        ///          </summary>
        // #############################################################################################
        public override string ToString()
        {
            return "i=" + TileIndex.ToString() + ",  w=" + Width.ToString() + ", h=" + Height.ToString();
        }


        // #############################################################################################
        /// <summary>
        ///     Save a raw array as a PNG
        /// </summary>
        /// <param name="_filename">filename to save as</param>
        /// <param name="_w">width of image</param>
        /// <param name="_h">height of image</param>
        /// <param name="_buffer">buffer to save - must be w*h</param>
        // #############################################################################################
        public static void SavePNG(string _filename, int _w, int _h, UInt32[] _buffer)
        {
            cImage img = new cImage(_w, _h);
            for (int i = 0; i < _buffer.Length; i++)
            {
                img.Raw[i] = _buffer[i];
            }
            img.Save(_filename);
        }
    }
}
