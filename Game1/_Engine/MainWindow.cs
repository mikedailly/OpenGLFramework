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
using System.Text;
using Framework.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Framework
{
    public class MainWindow : GameWindow
    {
        public bool isFullScreen;
        public int WinX;
        public int WinY;
        public int WinWidth;
        public int WinHeight;

        public int MouseX;
        public int MouseY;
        public int MouseButtons;


        public MainWindow()
        {
            isFullScreen = false;

            MouseButtons = 0;
            MouseX = 0;
            MouseY = 0;


            KeyDown += window_KeyDown;
            KeyUp += window_KeyUp;
            KeyPress += window_KeyPress;
            MouseDown += window_MouseDown;
            MouseUp += MainWindow_MouseUp;
            MouseMove += MainWindow_MouseMove;
            FocusedChanged += MainWindow_FocusedChanged;
        }

        private void MainWindow_FocusedChanged(object sender, EventArgs e)
        {
            Program.bMouseClicked = false;
        }

        private void MainWindow_MouseMove(object sender, MouseMoveEventArgs e)
        {
            MouseX = e.X; //+WinX;
            MouseY = e.Y;// + WinY;
            //Log.WriteLine("X2: " + MouseX.ToString() + " Y2: " + MouseY.ToString());
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int button = 0;
            if (e.Button == MouseButton.Left)
            {
                Program.MouseLButton = 0;
                button = 1;
            }
            if (e.Button == MouseButton.Right)
            {
                Program.MouseRButton = 0;
                button |= 2;
            }
            if (e.Button == MouseButton.Middle)
            {
                Program.MouseMButton = 0;
                button |= 4;
            }
            int clear = (int) (button ^ 0xffffffff);
            MouseButtons &= clear;
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int button = 0;
            if(e.Button == MouseButton.Left)
            {
                Program.MouseLButton = 1;
                button = 1;
            }
            if(e.Button == MouseButton.Right)
            {
                Program.MouseRButton = 1;
                button |= 2;
            }
            if (e.Button == MouseButton.Middle)
            {
                Program.MouseMButton = 1;
                button |= 4;
            }
            MouseButtons |= button;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            WinX = base.X;
            WinY = base.Y;
            WinWidth = ClientRectangle.Width;
            WinHeight = ClientRectangle.Height;
        }


        protected override void OnMove(EventArgs e)
        {
            WinX = base.X;
            WinY = base.Y;
        }


        public void SetSize(int _w, int _h)
        {
            WinX = base.X;
            WinY = base.Y;
            WinWidth = _w;
            WinHeight = _h;
            ClientRectangle = new Rectangle(0, 0, _w, _h);
        }

        protected override void OnResize(EventArgs e)
        {

            base.OnResize(e);
            
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);

            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadMatrix(ref projection);


            WinX = base.X;
            WinY = base.Y;
            WinWidth = ClientRectangle.Width;
            WinHeight = ClientRectangle.Height;
        }


        public static char GetAscii(Key _key, bool _shift, bool _control, bool _alt)
        {
            char c = ' ';
            switch(_key)
            {
                case Key.Number0: if (!_shift) c = '0'; else c = ')';  break;
                case Key.Number1: if (!_shift) c = '1';  else c = '!'; break;
                case Key.Number2: if (!_shift) c = '2';  else c = '"'; break;
                case Key.Number3: if (!_shift) c = '3';  else c = '£'; break;
                case Key.Number4: if (!_shift) c = '4';  else c = '$'; break;
                case Key.Number5: if (!_shift) c = '5';  else c = '%'; break;
                case Key.Number6: if (!_shift) c = '6';  else c = '^'; break;
                case Key.Number7: if (!_shift) c = '7';  else c = '&'; break;
                case Key.Number8: if (!_shift) c = '8';  else c = '*'; break;
                case Key.Number9: if (!_shift) c = '9'; else c = '('; break;
                case Key.A: if(!_shift) c = 'a'; else c = 'A'; break;
                case Key.B: if(!_shift) c = 'b'; else c = 'B'; break;
                case Key.C: if(!_shift) c = 'c'; else c = 'C'; break;
                case Key.D: if(!_shift) c = 'd'; else c = 'D'; break;
                case Key.E: if(!_shift) c = 'e'; else c = 'E'; break;
                case Key.F: if(!_shift) c = 'f'; else c = 'F'; break;
                case Key.G: if(!_shift) c = 'g'; else c = 'G'; break;
                case Key.H: if(!_shift) c = 'h'; else c = 'H'; break;
                case Key.I: if(!_shift) c = 'i'; else c = 'I'; break;
                case Key.J: if(!_shift) c = 'j'; else c = 'J'; break;
                case Key.K: if(!_shift) c = 'k'; else c = 'K'; break;
                case Key.L: if(!_shift) c = 'l'; else c = 'L'; break;
                case Key.M: if(!_shift) c = 'm'; else c = 'M'; break;
                case Key.N: if(!_shift) c = 'n'; else c = 'N'; break;
                case Key.O: if(!_shift) c = 'o'; else c = 'O'; break;
                case Key.P: if(!_shift) c = 'p'; else c = 'P'; break;
                case Key.Q: if(!_shift) c = 'q'; else c = 'Q'; break;
                case Key.R: if(!_shift) c = 'r'; else c = 'R'; break;
                case Key.S: if(!_shift) c = 's'; else c = 'S'; break;
                case Key.T: if(!_shift) c = 't'; else c = 'T'; break;
                case Key.U: if(!_shift) c = 'u'; else c = 'U'; break;
                case Key.V: if(!_shift) c = 'v'; else c = 'V'; break;
                case Key.W: if(!_shift) c = 'w'; else c = 'W'; break;
                case Key.X: if(!_shift) c = 'x'; else c = 'X'; break;
                case Key.Y: if(!_shift) c = 'y'; else c = 'Y'; break;
                case Key.Z: if (!_shift) c = 'z'; else c = 'Z'; break;
                case Key.Comma: if (!_shift) c = ','; else c = '<';  break;
                case Key.BracketRight: if (!_shift) c = ']'; else c = '}'; break;
                case Key.BracketLeft: if (!_shift) c = '['; else c = '{'; break;
                case Key.NonUSBackSlash: if (!_shift) c = '\\'; else c = '|';  break;
                case Key.Space: c = ' '; break;
                case Key.Slash: if (!_shift) c = '/'; else c = '?';  break;
                case Key.Tab: c = '\t'; break;
                case Key.BackSlash: if (!_shift) c = '#'; else c = '~'; break;
                case Key.Tilde: if (!_shift) c = '`'; else c = '¬'; break;
                case Key.Semicolon: if (!_shift) c = ';'; else c = ':';  break;
                case Key.Quote: if (!_shift) c = '\''; else c = '@'; break;
                case Key.Period: if (!_shift) c = '.'; else c = '>'; break;
                case Key.Minus: if (!_shift) c = '-'; else c = '_'; break;
                case Key.KeypadSubtract: c = '-'; break;
                case Key.KeypadPlus: c = '+'; break;
                case Key.KeypadPeriod: c = '.'; break;
                case Key.KeypadMultiply: c = '*'; break;
                case Key.KeypadDivide: c = '/'; break;
                case Key.Keypad9: c = '9'; break;
                case Key.Keypad8: c = '8'; break;
                case Key.Keypad7: c = '7'; break;
                case Key.Keypad6: c = '6'; break;
                case Key.Keypad5: c = '5'; break;
                case Key.Keypad4: c = '4'; break;
                case Key.Keypad3: c = '3'; break;
                case Key.Keypad2: c = '2'; break;
                case Key.Keypad1: c = '1'; break;
                case Key.Keypad0: c = '0'; break;
                case Key.Delete: c = (char)0x2e; break;         // delete key (beside insert/home/etc)
                case Key.Back: c = (char)0x08; break;           // backspace key (above Enter)
                case Key.Enter: c = (char)0x0d; break;
                case Key.KeypadEnter: c = (char)0x0d; break;
                default:
                    c = (char)0xff;
                    break;
            }
            return c;

        }

        public void window_KeyDown(object sender,KeyboardKeyEventArgs e)
        {
            Program.KeyFlags[(int)e.Key] = 1;
         


            // Hard code fullscreen enter/exit  (windows)
            if( e.Key == Key.Enter && e.Alt)
            {
                ToggleFullscreen();
            }
        }

        public void ToggleFullscreen(int _force=0)
        {
            // toggle mode?
            if (_force == 0)
            {
                if (!isFullScreen)
                {
                    isFullScreen = true;
                    WindowState = WindowState.Fullscreen;
                }
                else
                {
                    isFullScreen = false;
                    WindowState = WindowState.Normal;
                }
            } else if (_force == 1)
            {
                // force fullscreen
                isFullScreen = true;
                WindowState = WindowState.Fullscreen;
            }
            else if (_force == 2)
            {
                // force windowed
                isFullScreen = false;
                WindowState = WindowState.Normal;
            }
        }

        public void window_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            Program.KeyFlags[(int)e.Key] = 0;            
        }
        public void window_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

    }
}
