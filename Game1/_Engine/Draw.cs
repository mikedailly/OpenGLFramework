// **********************************************************************************************************************
// 
// Copyright (c)2020, Ogre Games Ltd. All Rights reserved.
// 
// Date				Version		Comment
// ----------------------------------------------------------------------------------------------------------------------
// 26/10/2022		V1.0        1st version
// 
// **********************************************************************************************************************using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    // #############################################################################################
    /// <summary>
    ///     This class is for when you have a "software" screen.
    /// </summary>
    // #############################################################################################
    public static class Draw
    {
        public enum eLineDir
        {
            Vertical,
            Horizontal
        }

        public static UInt32[] Screen;
        public static int Width;
        public static int Height;

        // #############################################################################################
        /// <summary>
        ///     Init the software screen location and size
        /// </summary>
        /// <param name="_screen">The screen array</param>
        /// <param name="_width">Width in pixels</param>
        /// <param name="_height">Height in pixels</param>
        // #############################################################################################
        public static void init(UInt32[] _screen, int _width, int _height)
        {
            Screen = _screen;
            Width = _width;
            Height = _height;
        }


        // #############################################################################################
        /// <summary>
        ///     Plot a single pixel
        /// </summary>
        /// <param name="_x">X Coordinate</param>
        /// <param name="_y">Y Coordinate</param>
        /// <param name="_col">Colour to plot</param>
        // #############################################################################################
        public static void Plot(int _x, int _y, UInt32 _col)
        {
            // Clip
            if (_x < 0 || _x >= Width) return;
            if (_y < 0 || _y >= Height) return;

            Screen[_x + (_y * Width)] = _col;
        }


        // #############################################################################################
        /// <summary>
        ///     Draw outline box
        /// </summary>
        /// <param name="_x1">Top Left X</param>
        /// <param name="_y1">Top Left Y</param>
        /// <param name="_x2">Bottom Right X</param>
        /// <param name="_y2">Bottom Right Y</param>
        /// <param name="_col">Colour to draw with</param>
        // #############################################################################################
        public static void Rect(int _x1, int _y1, int _x2, int _y2, UInt32 _col)
        {
            // Top=Bottom
            for (int x = _x1; x <= _x2; x++)
            {
                Plot(x, _y1, _col);
                Plot(x, _y2, _col);
            }

            // Top=Bottom
            for (int y = _y1; y < _y2; y++)
            {
                Plot(_x1, y, _col);
                Plot(_x2, y, _col);
            }
        }

        // #############################################################################################
        /// <summary>
        ///     Draw outline box
        /// </summary>
        /// <param name="_x1">Top Left X</param>
        /// <param name="_y1">Top Left Y</param>
        /// <param name="_x2">Bottom Right X</param>
        /// <param name="_y2">Bottom Right Y</param>
        /// <param name="_col">Colour to draw with</param>
        // #############################################################################################
        public static void FillRect(int _x1, int _y1, int _x2, int _y2, UInt32 _col)
        {
            // Cull Fully
            if (_x2 < 0 || _y2 < 0) return;
            if (_x1 >= Width || _y1 >= Height) return;

            // clip
            if (_x1 < 0) _x1 = 0;
            if (_y1 < 0) _y1 = 0;
            // clip
            if (_x2 >= Width) _x2 = Width - 1;
            if (_y2 >= Height) _y2 = Height - 1;

            // Top=Bottom
            for (int y = _y1; y < _y2; y++)
            {
                for (int x = _x1; x < _x2; x++)
                {
                    Plot(x, y, _col);
                }
            }
        }


        // #############################################################################################
        /// <summary>
        ///     Draw a clipped line.
        /// </summary>
        /// <remarks>
        ///     We keep a float path to try and maintain some "fractional" movement in the lines,
        ///     which will help keep them smooth where possibly - especially for 3D vectors
        /// </remarks>
        /// <param name="_x1">Top Left X</param>
        /// <param name="_y1">Top Left Y</param>
        /// <param name="_x2">Bottom Right X</param>
        /// <param name="_y2">Bottom Right Y</param>
        /// <param name="_col">Colour to draw with</param>
        // #############################################################################################
        public static void Line(float _x1, float _y1, float _x2, float _y2, UInt32 _col)
        {
            _x1 += 0.5f;
            _y1 += 0.5f;
            _x2 += 0.5f;
            _y2 += 0.5f;

            // Can't be bothered with fixed point
            float dx = (float)_x2 - _x1;
            float dy = (float)_y2 - _y1;

            // Work out major axis and delta for minor one.
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                dy = dy / Math.Abs(dx);
                int pixels = (int)Math.Abs(_x2 - _x1);

                int d = 1;
                if (_x2 < _x1) d = -1;
                for (int xx = 0; xx <= pixels; xx++)
                {
                    Plot((int)_x1, (int)Math.Floor(_y1), _col);
                    _x1 += d;
                    _y1 += dy;
                }
            }
            else
            {
                dx = dx / Math.Abs(dy);
                int pixels = (int)Math.Abs(_y2 - _y1);

                int d = 1;
                if (_y2 < _y1) d = -1;
                for (int yy = 0; yy <= pixels; yy++)
                {
                    Plot((int)Math.Floor(_x1), (int)_y1, _col);
                    _y1 += d;
                    _x1 += dx;
                }
            }

            // Plot last pixel...
            Plot((int)_x2, (int)_y2, _col);
        }

        // #############################################################################################
        /// <summary>
        ///     Draw a clipped line
        /// </summary>
        /// <param name="_x1">Top Left X</param>
        /// <param name="_y1">Top Left Y</param>
        /// <param name="_x2">Bottom Right X</param>
        /// <param name="_y2">Bottom Right Y</param>
        /// <param name="_col">Colour to draw with</param>
        // #############################################################################################
        public static void Line(int _x1, int _y1, int _x2, int _y2, UInt32 _col)
        {
            Line((float)_x1, (float)_y1, (float)_x2, (float)_y2, _col);
        }


        // #############################################################################################
        /// <summary>
        ///         Draw a simple circle - or elpise really.
        /// </summary>
        /// <param name="_x">Center X</param>
        /// <param name="_y">Center Y</param>
        /// <param name="_radx">X radius</param>
        /// <param name="_rady">Y radius</param>
        /// <param name="_col">Circle Colour</param>
        // #############################################################################################
        public static void Circle(int _x, int _y, int _radx, int _rady, UInt32 _col)
        {
            double i, angle, x1, y1;

            for (i = 0; i < 360; i += 0.1f)
            {
                angle = i;
                x1 = (float)_radx * Math.Cos(angle * Math.PI / 180.0f);
                y1 = (float)_rady * Math.Sin(angle * Math.PI / 180.0f);
                Plot((int)(_x + x1), (int)(_y + y1), _col);
            }
        }
        // #############################################################################################
        /// <summary>
        ///         Draw a simple circle - or elpise really.
        /// </summary>
        /// <param name="_x">Center X</param>
        /// <param name="_y">Center Y</param>
        /// <param name="_radx">X radius</param>
        /// <param name="_rady">Y radius</param>
        /// <param name="_col">Circle Colour</param>
        // #############################################################################################
        public static void Circle(float _x, float _y, float _radx, float _rady, UInt32 _col)
        {
            Circle((int)_x, (int)_y, (int)_radx, (int)_rady, _col);
        }

        // #############################################################################################
        /// <summary>
        ///     Draw a cliped sprite
        /// </summary>
        /// <param name="_x">X coordinate</param>
        /// <param name="_y">Y coordinate</param>
        /// <param name="_sprite">Sprite to draw</param>
        /// <param name="_transparent">The chroma-key colour</param>
        // #############################################################################################
        public static void Sprite(int _x, int _y, cImage _sprite, UInt32 _transparent)
        {

            int w = _sprite.Width;
            int h = _sprite.Height;
            UInt32[] pixels = _sprite.Raw;

            // Full Cull
            if (_x > Width) return;
            if (_y > Height) return;
            if ((_x + w) < 0) return;
            if ((_y + h) < 0) return;

            int offset = 0;
            int modulo = 0;

            // Clip X
            if (_x < 0)
            {
                offset = -_x;
                modulo = offset;
                w -= offset;
                _x = 0;

            }
            if ((_x + w) > Width)
            {
                int d = (_x + w) - Width;
                modulo += d;
                w -= d;
            }
            // clip Y
            if (_y < 0)
            {
                offset += _sprite.Width * -_y;
                h -= -+_y;
                _y = 0;
            }
            if ((_y + h) > Height)
            {
                int d = (_y + h) - Height;
                h -= d;
            }

            if (w <= 0 || h <= 0) return;


            int scr_off_base = _x + (_y * Width);
            for (int yy = 0; yy < h; yy++)
            {
                int scr_off = scr_off_base;
                for (int xx = 0; xx < w; xx++)
                {
                    UInt32 c = pixels[offset++];
                    if (c != _transparent) Screen[scr_off] = c;
                    scr_off++;
                }
                offset += modulo;
                scr_off_base += Width;
            }
        }
        // #############################################################################################
        /// <summary>
        ///     Draw a cliped sprite
        /// </summary>
        /// <param name="_x">X coordinate</param>
        /// <param name="_y">Y coordinate</param>
        /// <param name="_sprite">Sprite to draw</param>
        /// <param name="_transparent">The chroma-key colour</param>
        // #############################################################################################
        public static void Sprite(float _x, float _y, cImage _sprite, UInt32 _transparent)
        {
            Sprite((int)_x, (int)_y, _sprite, _transparent);
        }
    }
}
