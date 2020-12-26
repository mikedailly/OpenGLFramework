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
using System.Threading;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System.IO;
using OpenTK.Input;
using Framework.Utils;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace Framework
{

    // ****************************************************************************
    /// <summary>
    ///     Main CSpect loop
    /// </summary>
    // ****************************************************************************
    static public class Program
    {
        public const int SCREEN_WIDTH = 1600;
        public const int SCREEN_HEIGHT = 1024;

        public static string command_line;

        public static string Exe_Path = ".";

        public static UInt32[] Screen;
        public static bool DoQuit;
        public static bool FullScreen;
        public static bool bFocus;

        public static int MouseLButton;
        public static int MouseRButton;
        public static int MouseMButton;
        public static bool bMouseClicked;
        public static int[] KeyFlags = new int[256];

        public static float Screen_Left = 0;
        public static float Screen_Top = 0;
        public static float Screen_Width = SCREEN_WIDTH;
        public static float Screen_Height = SCREEN_HEIGHT;


        //***************************************************************
        /// <summary>
        ///     Test a substring
        /// </summary>
        /// <param name="_s"></param>
        /// <param name="_start"></param>
        /// <param name="_len"></param>
        /// <returns></returns>
        //***************************************************************
        static string SubString(string _s,int _start, int _len)
        {
            if ((_start + _len) > _s.Length)
            {
                return _s.Substring(_start);
            }else
            {
                return _s.Substring(_start, _len);
            }
        }

        //***************************************************************
        /// <summary>
        ///     Parse the command line     
        /// </summary>
        /// <param name="_args">Command line "string" (not array)</param>
        //***************************************************************
        public static void ParseCommandLine(string _args)
        {
            int len = _args.Length;
            int i = 0;

            while (i < len)
            {
                // option?
                if (_args[i] == '-')
                {
                    i++;
                    if (SubString(_args, i, 8) == "example1")
                    {
                        i += 7;
                    }
                    else if (SubString(_args, i, 4) == "test")
                    {
                        i += 3;
                    }
                }

                i++;
            }

        }


        // ************************************************************************
        /// <summary>
        ///     Render the display with Aspect Ratio compensation
        /// </summary>
        /// <param name="_texture">Texture to draw aspect correct</param>
        // ************************************************************************
        public static void DrawDisplay(cTexture _texture, float _xscale, float _yscale, UInt32 _colour)
        {
            bool _maintain_aspect = true;
            float w = _texture.width * _xscale;
            float h = _texture.height * _yscale;

            float _dw = Render.Width;
            float _dh = Render.Height;

            // If we want to maintain the aspect ratio, then use the height and "fit" the width.
            float aspect, hh, ww, x1, x2, y1, y2;
            if (_maintain_aspect)
            {
                aspect = (float)w / (float)h;
                hh = ((float)_dw / aspect);
                if (hh < _dh)
                {
                    aspect = (float)h / (float)w;
                    hh = ((float)_dw * aspect);
                    y1 = ((float)_dh - hh) / 2.0f;
                    y2 = y1 + hh;
                    x1 = 0;
                    x2 = (float)_dw;
                }
                else if (hh > _dh)
                {
                    aspect = (float)w / (float)h;
                    ww = ((float)_dh * aspect);
                    x1 = ((float)_dw - ww) / 2.0f;
                    x2 = x1 + ww;
                    y1 = 0;
                    y2 = (float)_dh;
                }
                else
                {
                    aspect = 1;
                    x1 = 0;
                    x2 = (float)_dw;
                    y1 = 0;
                    y2 = (float)_dh;
                }
            }
            else
            {
                aspect = 1;
                x1 = 0;
                x2 = (float)_dw;
                y1 = 0;
                y2 = (float)_dh;
            }




            _texture.Set();
            Render.DrawQuadSimple(x1, y1, (x2 - x1) - 1, (y2 - y1) - 1, _colour);
    }

        public static void ShowCursor(bool _on_off)
        {
            MainWindow win = Render.Window;
            win.CursorVisible = _on_off;
        }

        public static void ProcessMouse()
        {
            MainWindow win = Render.Window;
            bFocus = win.Focused;

            if (!bMouseClicked) return;

            int l = win.WinX;
            int w = win.WinWidth;
            int t = win.WinY;
            int h = win.WinHeight;

            int mx = win.MouseX;
            int my = win.MouseY;


            MouseState s = OpenTK.Input.Mouse.GetCursorState();
            mx = s.X;
            my = s.Y;
        }




        // ************************************************************************
        /// <summary>
        ///     Main loop
        /// </summary>
        /// <param name="args">Program arguments</param>
        // ************************************************************************
        [STAThread]
        static void Main(string[] args)
       {
            Exe_Path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            
            string _args = "";
            bool first=true;
            foreach(string s in args)
            {
                if (!first) _args += " ";
                _args += s;
                first = false;
            }
            command_line = _args;
            ParseCommandLine(_args);

            Render.Init(SCREEN_WIDTH, SCREEN_HEIGHT);            
            Render.Visible = true;

            Render.Window.VSync = VSyncMode.On;
            float fps_target = 60.0f;
            if(FullScreen)
            {
                Render.Window.ToggleFullscreen(1);
            }

            Game game = new Game();
            game.init();

            int back_colour = 0;
            while (true)
            {
                Render.ProcessEvents();
                if (Render.IsExiting) break;
                if (DoQuit) break;
                    
                Render.Begin(fps_target);
                game.process();

                back_colour = (back_colour + 1) & 0xff;
                Render.Clear(0x404040);
                game.render();


                Render.End();
                Render.Flip();
            }

            // If we're in fullscreen mode, then come out of it before exiting - and possibly saving the window location
            if (Render.Window.isFullScreen)
            {
                Render.ProcessEvents();
                Render.Window.ToggleFullscreen(0);
                Render.ProcessEvents();
                Render.Begin(fps_target);
                Render.End();
                Render.Flip();
            }


            game.quit();
            Log.Flush();
            // Kill audio thread

            //

        }


    }
}
