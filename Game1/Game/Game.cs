// **********************************************************************************************************************
// 
// Copyright (c)2020, Ogre Games Ltd. All Rights reserved.
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
using Framework.Utils;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class Game
    {
        public cFont Font;
        public bool Button_Pressed = false;

        // #############################################################################################
        /// <summary>
        ///     Initialise the editor
        /// </summary>
        // #############################################################################################
        public void init()
        {
            Log.WriteLine("Init...");

            Font = cFont.Create(@"ZXFont.png", 8, 8);
            Font.scale = 2.0f;

            cButton b1 = new cButton("Button", 1073, 312, 250, 64, Font, 0xff38D3FF);
            b1.onclick += DemoButtonClick; ;

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
        }

        // #############################################################################################
        /// <summary>
        ///     Simple demo button press
        /// </summary>
        /// <param name="_button">Button that clicked</param>
        // #############################################################################################
        private void DemoButtonClick(cButton _button)
        {
            if (Button_Pressed) Button_Pressed = false; else Button_Pressed = true;
        }

        // #############################################################################################
        /// <summary>
        ///     Shutdown the editor
        /// </summary>
        // #############################################################################################
        public void quit()
        {
        }

        // #############################################################################################
        /// <summary>
        ///     Process the editor
        /// </summary>
        // #############################################################################################
        public void process()
        {
            MainWindow window = Render.Window;
            ButtonManager.Tick();
        }


        // #############################################################################################
        /// <summary>
        ///     Render the editor
        /// </summary>
        /// <param name="_render">The renderer interface/class</param>
        // #############################################################################################
        public void render()
        {
            ButtonManager.paint();

            Font.Draw(100, 100, "HELLO WORLD", 0xffffffff, 2);
            Font.Draw(100, 150, "Hello World £123,456", 0xffffffff, 2);

            if(Button_Pressed) Font.Draw(10, 10, "Button Pressed", 0xffffffff, 2);
        }
    }
}
