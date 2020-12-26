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
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using OpenTK;
using System.IO;


namespace Framework
{
    public enum eMatrixType
    {
        Model = 0,
        View = 1,
        Projection = 2,
        ModelView = 3,
        ModelViewProjection = 4,
        MAX_TYPE = 5
    }

    public unsafe static class Render
    {
        /// <summary>Main window</summary>
        public static MainWindow Window;
        static DateTime LastTime = DateTime.Now;
        public static float[] SceneMatrix_floats = new float[ (int)eMatrixType.MAX_TYPE * 16 ];
        public static Matrix4[] SceneMatrix = new Matrix4[ (int)eMatrixType.MAX_TYPE ];

        /// <summary>Current Projection matrix</summary>
        public static Matrix4 ProjectionMatrix;
        /// <summary>Current View matrix</summary>
        public static Matrix4 ViewMatrix;
        /// <summary>Current Model matrix</summary>
        public static Matrix4 ModelMatrix;

        public static cShader CurrentShader;

        public static int MouseX;
        public static int MouseY;

        public static Stack<float[]> ClipRects;

        // #############################################################################################
        /// <summary>
        ///     Get client area width
        /// </summary>
        // #############################################################################################
        public static int Width
        {
            get
            {
                return Window.ClientSize.Width;
            }
        }

        // #############################################################################################
        /// <summary>
        ///     Get client area height
        /// </summary>
        // #############################################################################################
        public static int Height
        {
            get
            {
                return Window.ClientSize.Height;
            }
        }

        // #############################################################################################
        /// <summary>
        ///     Get/Set window visibility
        /// </summary>
        // #############################################################################################
        public static bool Visible {
            get
            {
                return Window.Visible;
            }
            set
            {
                Window.Visible = value;
            }
        }

        // #############################################################################################
        /// <summary>
        ///     Get/Set window visibility
        /// </summary>
        // #############################################################################################
        public static bool IsExiting
        {
            get
            {
                return Window.IsExiting;
            }            
        }


        // #############################################################################################
        /// <summary>
        ///     Process all window events
        /// </summary>
        // #############################################################################################
        public static void ProcessEvents()
        {
            Window.ProcessEvents();
        }


        // #############################################################################################
        /// <summary>
        ///     Save the window size/position
        /// </summary>
        // #############################################################################################
        public static void SaveWindowPos()
        {
            // first come out of full screen mode
            //if (Window.isFullScreen)
            //{
            //   Window.ToggleFullscreen(0);
            //}

            int[] buffer = new int[4];
            buffer[0] = Window.WinX;
            buffer[1] = Window.WinY;
            buffer[2] = Window.WinWidth;
            buffer[3] = Window.WinHeight;

            byte[] filebuff = new byte[4*4];
            System.Buffer.BlockCopy(buffer, 0, filebuff, 0, 16);

            try
            {
                File.WriteAllBytes("framework_win.dat", filebuff);
            }catch { }

        }

        // #############################################################################################
        /// <summary>
        ///     Load and set the window size and position
        /// </summary>
        // #############################################################################################
        public static void LoadWindowPos()
        {
            if (File.Exists("framework_win.dat"))
            {
                byte[] f = File.ReadAllBytes("framework_win.dat");

                int[] buffer = new int[4];
                System.Buffer.BlockCopy(f, 0, buffer, 0, 16);

                Window.WinX = buffer[0];
                Window.WinY = buffer[1];
                Window.WinWidth = buffer[2];
                Window.WinHeight = buffer[3];

                Window.X = Window.WinX;
                Window.Y = Window.WinY;
                Window.SetSize(Window.WinWidth, Window.WinHeight);
            }

        }


        // #############################################################################################
        /// <summary>
        ///     Create a new window
        /// </summary>
        /// <param name="_width">width of window</param>
        /// <param name="_height">height of window</param>
        // #############################################################################################
        public static void Init(int _width, int _height)
        {
            ClipRects = new Stack<float[]>();


            Window = new MainWindow();
            Window.Title = "Framework V0.1";
            Window.SetSize(_width, _height);
            Window.TargetUpdateFrequency = 1000;
            Window.VSync = VSyncMode.Off;

            SceneMatrix_floats = new float[(int)eMatrixType.MAX_TYPE * 16];
            SceneMatrix = new Matrix4[(int)eMatrixType.MAX_TYPE];

            for (int i = 0; i < (int)eMatrixType.MAX_TYPE; i++)
            {
                SceneMatrix[i] = Matrix4.Identity;
            }
            SetMatrix(eMatrixType.Model, SceneMatrix[(int)eMatrixType.Model]);
            SetMatrix(eMatrixType.View, SceneMatrix[(int)eMatrixType.View]);
            SetMatrix(eMatrixType.Projection, SceneMatrix[(int)eMatrixType.Projection]);


            GL.Enable(EnableCap.ScissorTest);
        }

        // #############################################################################################
        /// Function:<summary>
        ///				Waits until it's time to run a loop of the IDE
        ///			</summary>|
        // #############################################################################################  
        private static void WaitForTick(float _fps)
        {
            // Get frame delta, and check for time going "backwards" (does happen when time is synced to the internet)
            bool Ready = false;
            int mill = (int)(1000.0 / _fps);
            while(!Ready)
            {
                DateTime CurrentTime = DateTime.Now;

                // Time going backwards?
                if (LastTime.Ticks > CurrentTime.Ticks)
                {
                    LastTime = CurrentTime;
                    Ready = true;
                }

                TimeSpan DeltaTime = CurrentTime - LastTime;
                if (DeltaTime.TotalMilliseconds > mill)
                {
                    Ready = true;
                    LastTime = CurrentTime;
                }
                else if ((mill - DeltaTime.TotalMilliseconds) > 5)
                {
                    Thread.Sleep(5);
                }
            }
        }



        // #############################################################################################
        /// Function:<summary>
        ///             Clear the screen (colour and depth)
        ///          </summary>
        ///
        /// In:		 <param name="_col"></param>
        ///
        // #############################################################################################
        public static void Clear(uint _col)
        {
            float r, g, b, a;
            r = ((_col >> 16) & 0xff) / 255.0f;
            g = ((_col >> 8) & 0xff) / 255.0f;
            b = ((_col >> 0) & 0xff) / 255.0f;
            a = ((_col >> 24) & 0xff) / 255.0f;
            GL.ClearColor(r, g, b, a);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }


        // ***************************************************************************
        /// <summary>
        ///     Set matrix
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_m"></param>
        // ***************************************************************************
        public static void SetMatrix(eMatrixType _type, Matrix4 _m)
        {
            SceneMatrix[(int)_type] = _m;
            SetFloats(ref SceneMatrix[(int)_type], SceneMatrix_floats, (int)_type * 16);

            // Now work out new combined matrices
            Matrix4.Mult(ref SceneMatrix[(int)eMatrixType.Model], ref SceneMatrix[(int)eMatrixType.View], out SceneMatrix[(int)eMatrixType.ModelView]);                     // Model * View
            Matrix4.Mult(ref SceneMatrix[(int)eMatrixType.ModelView], ref SceneMatrix[(int)eMatrixType.Projection], out SceneMatrix[(int)eMatrixType.ModelViewProjection]); // ModelView * Projection

            // And store the new "floats" in the the combined array
            SetFloats(ref SceneMatrix[(int)eMatrixType.ModelView], SceneMatrix_floats, (int)eMatrixType.ModelView * 16);
            SetFloats(ref SceneMatrix[(int)eMatrixType.ModelViewProjection], SceneMatrix_floats, (int)eMatrixType.ModelViewProjection * 16);
        }


        public static void SetFloats(ref OpenTK.Matrix4 _mat, float[] _dest, int _index)
        {
            _dest[_index + 0] = _mat.M11;
            _dest[_index + 1] = _mat.M12;
            _dest[_index + 2] = _mat.M13;
            _dest[_index + 3] = _mat.M14;
            _dest[_index + 4] = _mat.M21;
            _dest[_index + 5] = _mat.M22;
            _dest[_index + 6] = _mat.M23;
            _dest[_index + 7] = _mat.M24;
            _dest[_index + 8] = _mat.M31;
            _dest[_index + 9] = _mat.M32;
            _dest[_index + 10] = _mat.M33;
            _dest[_index + 11] = _mat.M34;
            _dest[_index + 12] = _mat.M41;
            _dest[_index + 13] = _mat.M42;
            _dest[_index + 14] = _mat.M43;
            _dest[_index + 15] = _mat.M44;
        }

        public static Matrix4 CreateOrtho2(float _w, float _h, float _near, float _far)
        {
            Matrix4 m = new Matrix4();
            m.M11 = 2.0f / _w;
            m.M12 = 0.0f;
            m.M13 = 0.0f;
            m.M14 = -1.0f; ;

            m.M21 = 0.0f;
            m.M22 = 2.0f / _h;
            m.M23 = 0.0f;
            m.M24 = 1.0f;

            m.M31 = 0.0f;
            m.M32 = 0.0f;
            m.M33 = 1.0f / 10.0f; //1.0f / (_far-_near);
            m.M34 = -(1.0f / 10.0f);

            m.M41 = 0.0f;
            m.M42 = 0.0f;
            m.M43 = 0.0f; //_near / (_far - _near);
            m.M44 = 1.0f;
            return m;
        }

        // #############################################################################################
        /// <summary>
        ///     Begin scene
        /// </summary>
        /// <param name="_fps">Frame rate</param>
        // #############################################################################################
        public static void Begin(float _fps)
        {
            if (_fps > 0){
                WaitForTick(_fps);
            }

            float xscale = (Program.Screen_Width/(320.0f*3.0f));
            float yscale = (Program.Screen_Height / (256.0f * 3.0f));
            //global.MouseX = (int) ((Window.MouseX-Window.X-Program.Screen_Left )/ xscale);
            //global.MouseY = (int) ((Window.MouseY- Window.Y - Program.Screen_Top) / yscale);
            MouseX = (int)((Window.MouseX -  Program.Screen_Left) / xscale);
            MouseY = (int)((Window.MouseY - Program.Screen_Top) / yscale);

            GL.Viewport(Window.ClientRectangle.X, Window.ClientRectangle.Y, Window.ClientRectangle.Width, Window.ClientRectangle.Height);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Window.Width / (float)Window.Height, 1.0f, 64.0f);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();            
            GL.Ortho(0, Window.ClientRectangle.Width-1, Window.ClientRectangle.Height-1, 0.0, 0.0, 4.0);
          
            //GL.GetFloat(GetPName.ProjectionMatrix, out ProjectionMatrix);            
            //SetFloats(ref ProjectionMatrix, SceneMatrix_floats, (int)5 * 16);

            ViewMatrix = Matrix4.Identity;
            SetMatrix(eMatrixType.View, ViewMatrix);

            ModelMatrix = Matrix4.Identity;
            SetMatrix(eMatrixType.Model, ModelMatrix);

            ProjectionMatrix = CreateOrtho2(Window.ClientRectangle.Width - 1, -(Window.ClientRectangle.Height - 1), 0.0f, 10.0f);
            SetMatrix(eMatrixType.Projection, ProjectionMatrix);

            SetClipRect(0, 0, (int)Program.Screen_Width, (int)Program.Screen_Height);
            
            //Window.Run(_fps);
        }

        // #############################################################################################
        /// <summary>
        ///     End Scene
        /// </summary>
        // #############################################################################################
        public static void End()
        {
        }


        // #############################################################################################
        /// Function:<summary>
        ///             Flip "current" buffer
        ///          </summary>
        // #############################################################################################
        public static void Flip()
        {
            Window.SwapBuffers();
        }


        /// <summary>
        ///     Create a texture
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_data"></param>
        /// <returns></returns>
        public static cTexture CreateTexture(int _width, int _height, uint[] _data = null)
        {
            return cTexture.CreateTexture(_width, _height, _data);
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns>GL texture ID</returns>
        private int CreateRenderTarget()
        {
            // Each context will raise the ref count
            int Context = TextureManager.CurrentContext;
            IncrementImageRefCount(Context);

            // Create Color Texture            
            int colorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)m_minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)m_magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ImageRef.width, ImageRef.height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            // test for GL Error here (might be unsupported format)

            GL.BindTexture(TextureTarget.Texture2D, 0); // prevent feedback, reading and writing to the same image is a bad idea

            // Create Depth Renderbuffer
            var depthRenderBuffer = (uint)0;
            GL.Ext.GenRenderbuffers(1, out depthRenderBuffer);
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthRenderBuffer);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent32, ImageRef.width, ImageRef.height);
            m_depthRenderBuffer[TextureManager.CurrentContext] = depthRenderBuffer;

            // Create a FBO and attach the textures
            var surfaceID = (uint)0;
            GL.Ext.GenFramebuffers(1, out surfaceID);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, surfaceID);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, colorTexture, 0);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthRenderBuffer);
            m_surfaceID[TextureManager.CurrentContext] = surfaceID;

            return colorTexture;
        }
        */

        /// <summary>
        ///     Draw simple quad
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="_colour"></param>
        public static unsafe void DrawQuadSimple(float _x, float _y, float _w, float _h, UInt32 _colour = 0xffffffff)
        {
            GL.Begin(PrimitiveType.Quads);
            //_x = 0.0f;
            //_y = 0.0f;
            //_w = 1.0f;
            //_h = 1.0f;
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(_x, _y);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(_x + _w, _y);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(_x + _w, _y + _h);

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(_x, _y + _h);
            GL.End();
        }


        #region CLIP RECTS
        // #####################################################################################################################
        /// <summary>
        ///     Set current clip rect
        /// </summary>
        /// <param name="_x">left edge</param>
        /// <param name="_y">right edge</param>
        /// <param name="_w">width in pixels</param>
        /// <param name="_h">height in pixels</param>
        // #####################################################################################################################
        public static void SetClipRect(float _x, float _y, float _w, float _h)
        {
            GL.Scissor((int)_x, (int)_y, (int)_w, (int)_h);
        }

        // #####################################################################################################################
        /// <summary>
        ///     Push the current clip rect onto the stack, and set a new one
        /// </summary>
        /// <param name="_x">left edge</param>
        /// <param name="_y">right edge</param>
        /// <param name="_w">width in pixels</param>
        /// <param name="_h">height in pixels</param>
        // #####################################################################################################################
        public static void PushClipRect(float _x,float _y, float _w, float _h)
        {
            float[] arr = new float[4];
            GL.GetFloat(GetPName.ScissorBox, arr);
            ClipRects.Push(arr);

            SetClipRect(_x, _y, _w, _h);
        }

        // #####################################################################################################################
        /// <summary>
        ///     Pop a clip rect off the stack and set it.
        /// </summary>
        // #####################################################################################################################
        public static void PopClipRect()
        {
            if (ClipRects.Count > 0)
            {
                float[] rect = ClipRects.Pop();
                SetClipRect(rect[0], rect[1], rect[2], rect[3]);
            }
        }
        #endregion 

        /// <summary>
        ///     Draw text to the screen
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="text"></param>
        public static void DrawText(float _x, float _y, string text)
        {
            
        }

    }
}
