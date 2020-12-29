using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class cButton
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void ButtonEventHandler(cButton _button);

        public string text;
        public UInt32 colour;
        public UInt32 roll_over_colour;
        public UInt32 text_colour;
        public UInt32 outline_text_colour;
        public int x;
        public int y;
        public int width;
        public int height;
        public cFont font;

        float scale = 1.0f;
        bool button_locked = false;
        bool roll_over = false;

        /// <summary>Empty space for user data</summary>
        public object UserData;

        public event ButtonEventHandler onclick;

        // ############################################################################################
        /// <summary>
        ///     Create a new button
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="_font"></param>
        /// <param name="_colour"></param>
        // ############################################################################################
        public cButton(string _text, int _x, int _y, int _w, int _h, cFont _font, UInt32 _colour)
        {
            x = _x;
            y = _y;
            width = _w >> 1;
            height = _h >> 1;
            colour = _colour;
            font = _font;

            text = _text;
            text_colour = 0xffffffff;
            outline_text_colour = 0xff000000;
            roll_over_colour = 0xff00ff00;

            button_locked = false;

            ButtonManager.Buttons.Add(this);
        }

        // ############################################################################################
        /// <summary>
        ///     Tick the button
        /// </summary>
        // ############################################################################################
        public void Tick()
        {
            MainWindow window = Render.Window;

            int mouse_x = window.MouseX;
            int mouse_y = window.MouseY;
            int buttons = window.MouseButtons;

            float dx = Math.Abs(mouse_x - x);
            float dy = Math.Abs(mouse_y - y);
            if (dx < (width >> 1) && dy < (height >> 1))
            {
                roll_over = true;
                if (buttons == 1)
                {
                    if (!button_locked)
                    {
                        button_locked = true;
                        scale = 0.75f;
                    }
                    else
                    {
                        scale = 0.75f;
                    }
                }
                else
                {
                    if (button_locked)
                    {
                        onclick(this);
                    }
                    button_locked = false;
                    scale = 1.0f;
                }
            }
            else
            {
                roll_over = false;
                scale = 1.0f;
                if ((buttons != 1) || (!button_locked))
                {
                    button_locked = false;
                }
            }
        }


        // ############################################################################################
        /// <summary>
        ///     Paint the button
        /// </summary>
        // ############################################################################################
        public void Paint()
        {
            int xx = x - ((int)(width * scale) >> 1);
            int yy = y - ((int)(height * scale) >> 1);
            UInt32 col = colour;
            if (roll_over) col = roll_over_colour;
            Render.FillRect(xx, yy, xx + (width * scale), yy + (height * scale), col);

            int w = font.StringWidth(text) >> 1;
            int h = font.StringHeight(text) >> 1;
            w = (int)(w * scale);
            h = (int)(h * scale);
            font.DrawOutline(x - w, y - h, text, text_colour, outline_text_colour, scale);

        }

    }

}
