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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;


namespace Framework
{
    public class cFont
    {
        public float scale;
        public int width;
        public int height;
        public int width_delta;
        public int height_delta;
        public cTexture Texture;

        float PixelU = 0.0f;
        float PixelV = 0.0f;

        int chars_per_row = 0;

        string characters = "";
        Dictionary<char, int> Lookup;


        // #########################################################################################################
        /// <summary>
        ///     Get/Set the charactcer string
        /// </summary>
        // #########################################################################################################
        string Characters
        {
            get { return characters; }
            set
            {
                characters = value;
                Lookup = new Dictionary<char, int>();
                for (int i = 0; i < value.Length; i++)
                {
                    Lookup.Add(value[i], i);
                }
            }
        }


        // #########################################################################################################
        /// <summary>
        ///     Create a new font
        /// </summary>
        /// <param name="_w">Width of each chacter</param>
        /// <param name="_h">Height of each character</param>
        // #########################################################################################################
        public cFont(int _w, int _h)
        {
            width_delta = width = _w;
            height_delta = height = _h;
            scale = 1.0f;
        }


        // #########################################################################################################
        /// <summary>
        ///     Draw text on screen
        /// </summary>
        /// <param name="_x">X coordinate</param>
        /// <param name="_y">Y coordinate</param>
        /// <param name="_text">Text to draw</param>
        /// <param name="_colour">colour+alpha to use</param>
        /// <param name="_ocolour">outline colour+alpha to use</param>
        /// <param name="_scale">scale to draw at</param>
        // #########################################################################################################
        public void DrawOutline(float _x, float _y, string _text, UInt32 _colour = 0xffffffff, UInt32 _ocolour = 0xffffffff, float _scale = 1.0f)
        {
            float off = scale * _scale;
            Draw(_x + off, _y, _text, _ocolour, _scale);
            Draw(_x - off, _y, _text, _ocolour, _scale);
            Draw(_x, _y + off, _text, _ocolour, _scale);
            Draw(_x, _y - off, _text, _ocolour, _scale);
            Draw(_x, _y, _text, _colour, _scale);
        }

        // #########################################################################################################
        /// <summary>
        ///     Draw text on screen
        /// </summary>
        /// <param name="_x">X coordinate</param>
        /// <param name="_y">Y coordinate</param>
        /// <param name="_text">Text to draw</param>
        /// <param name="_colour">colour+alpha to use</param>
        /// <param name="_scale">scale to draw at</param>
        // #########################################################################################################
        public void Draw(float _x, float _y, string _text, UInt32 _colour = 0xffffffff, float _scale = 1.0f)
        {
            float r = ((_colour >> 16) & 0xff) / 255.0f;
            float g = ((_colour >> 8) & 0xff) / 255.0f;
            float b = ((_colour >> 0) & 0xff) / 255.0f;
            float a = ((_colour >> 24) & 0xff) / 255.0f;

            _scale *= scale;

            Texture.Set();
            GL.Begin(PrimitiveType.Quads);

            float xpos = _x;
            float ypos = _y;
            float dx = width * _scale;
            float dy = height * _scale;
            float delta_dx = width_delta * _scale;
            float delta_dy = height_delta * _scale;

            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];
                int index = -1;
                if (Lookup.TryGetValue(c, out index))
                {
                    int column = index % chars_per_row;
                    int row = index / chars_per_row;

                    float u1 = column * width * PixelU;
                    float v1 = row * height * PixelV;

                    float u2 = u1 + (PixelU * width);
                    float v2 = v1 + (PixelV * height);


                    GL.TexCoord2(u1, v1);
                    GL.Color4(r, g, b, a);
                    GL.Vertex2(xpos, ypos);

                    GL.TexCoord2(u2, v1);
                    GL.Color4(r, g, b, a);
                    GL.Vertex2(xpos + dx, ypos);

                    GL.TexCoord2(u2, v2);
                    GL.Color4(r, g, b, a);
                    GL.Vertex2(xpos + dx, ypos + dy);

                    GL.TexCoord2(u1, v2);
                    GL.Color4(r, g, b, a);
                    GL.Vertex2(xpos, ypos + dy);

                    xpos += delta_dx;
                }
            }

            GL.End();
        }


        // #########################################################################################################
        /// <summary>
        ///     Load and create a new font
        /// </summary>
        /// <param name="_filename">Name of font texture</param>
        /// <param name="_character_width"></param>
        /// <param name="_character_height"></param>
        // #########################################################################################################
        public static cFont Create(string _filename, int _character_width, int _character_height)
        {
            cFont font = new cFont(_character_width, _character_height);
            font.Texture = cTexture.Load(_filename);

            font.PixelU = 1.0f / font.Texture.width;
            font.PixelV = 1.0f / font.Texture.height;

            font.Characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_£abcdefghijklmnopqrstuvwxyz{|}~©";

            font.chars_per_row = font.Texture.width / _character_width;

            return font;
        }

    }
}
