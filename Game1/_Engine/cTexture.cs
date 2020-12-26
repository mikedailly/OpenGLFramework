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
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class cTexture
    {
        public static List<cTexture> m_Textures = new List<cTexture>();

        /// <summary>openGL texture ID</summary>
        public uint id;
        /// <summary>width of texture</summary>
        public int width;
        /// <summary>height of texture</summary>
        public int height;

        /// <summary>If loaded from a file, holds the image</summary>
        public cImage image;

        // #############################################################################################
        /// <summary>
        ///     Update the texture
        /// </summary>
        /// <param name="_data">the ARGB texture data</param>
        // #############################################################################################
        public unsafe void Update(uint[] _data)
        {
            ErrorCode err;

            // Set the texture
            GL.BindTexture(TextureTarget.Texture2D, id);

            // upload the data
            if (_data != null)
            {
                fixed (UInt32* PixelData = &_data[0])
                {
                    IntPtr intPtr = new IntPtr(PixelData);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, intPtr);

                    //IntPtr pPixel = new IntPtr(PixelData);
                    //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pPixel);

                    err = GL.GetError();
                }
            }
        }


        // #############################################################################################
        /// <summary>
        ///     Set the texture
        /// </summary>
        /// <param name="_unit">[Option]the unit to set the texture to</param>
        // #############################################################################################
        public void Set(int _unit = 0)
        {
            GL.Enable(EnableCap.Texture2D);
            //GL.ActiveTexture(TextureUnit.Texture0 + _unit);
            GL.BindTexture(TextureTarget.Texture2D, id);

            if (Render.CurrentShader != null)
            {
                int mTextureUniformHandle = Render.CurrentShader.KnownUniforms[(int)eUniform.BaseTexture];
                GL.Uniform1(mTextureUniformHandle, 0);
            }

        }


        // #############################################################################################
        /// <summary>
        ///     Create a new texture
        /// </summary>
        /// <param name="_width">width of texture</param>
        /// <param name="_height">height of texture</param>
        /// <param name="_data">data to fill texture with - or null for empty</param>
        /// <returns></returns>
        // #############################################################################################
        public static unsafe cTexture CreateTexture(int _width, int _height, uint[] _data = null)
        {
            uint tex;
            GL.GenTextures(1, out tex);
            ErrorCode err = GL.GetError();


            GL.BindTexture(TextureTarget.Texture2D, tex);
            err = GL.GetError();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
            err = GL.GetError();



            // do we have data to fill in?
            if (_data != null)
            {
                fixed (uint* PixelData = &_data[0])
                {
                    IntPtr pPixel = new IntPtr(PixelData);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pPixel);
                    err = GL.GetError();
                }
            }

            // unbind
            GL.BindTexture(TextureTarget.Texture2D, 0);
            err = GL.GetError();


            // create a new texture and return it....
            cTexture texture = new cTexture();
            texture.id = tex;
            texture.width = _width;
            texture.height = _height;

            // add to the context list
            m_Textures.Add(texture);

            return texture;
        }

        // #############################################################################################
        /// <summary>
        ///     Load in a texture
        /// </summary>
        /// <param name="_filename"></param>
        /// <returns>
        ///     a new cTexture
        /// </returns>
        // #############################################################################################
        public static cTexture Load(string _filename)
        {
            cImage image = new cImage(_filename);
            cTexture texture = CreateTexture(image.Width, image.Height, image.Raw);
            texture.image = image;
            return texture;
        }
    }
}
