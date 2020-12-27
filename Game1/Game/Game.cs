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

        // #############################################################################################
        /// <summary>
        ///     Initialise the editor
        /// </summary>
        // #############################################################################################
        public void init()
        {
            Log.WriteLine("Init...");

            Font = cFont.Create(@"ZXFont.png", 8, 8);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
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
        }


        // #############################################################################################
        /// <summary>
        ///     Render the editor
        /// </summary>
        /// <param name="_render">The renderer interface/class</param>
        // #############################################################################################
        public void render()
        {
            Font.Draw(100, 100, "HELLO WORLD", 0xffffffff, 3);
            Font.Draw(100, 150, "Hello World £123,456", 0xffffffff, 3);
        }
    }
}
