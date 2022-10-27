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
        public const int GAME_WIDTH = (160 * 8);
        public const int GAME_HEIGHT = (120 * 8);

        /// <summary>Our "software" screen</summary>
        public static UInt32[] Screen;
        /// <summary>Texture that holds our software screen</summary>
        public cTexture ScreenTexture;

        public cFont Font;
        public bool Button_Pressed = false;

        public cImage BallSprite;
        // #############################################################################################
        /// <summary>
        ///     Initialise the "game"
        /// </summary>
        // #############################################################################################
        public void init()
        {
            Log.WriteLine("Init...");

            Screen = new uint[GAME_WIDTH * GAME_HEIGHT];
            ScreenTexture = Render.CreateTexture(GAME_WIDTH, GAME_HEIGHT);
            Draw.init(Screen, GAME_WIDTH, GAME_HEIGHT);
            
            Font = cFont.Create(@"ZXFont.png", 8, 8);
            Font.scale = 2.0f;

            BallSprite = new cImage("graphics\\BallSprite.png");

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
        ///     Shutdown the "game"
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
        ///     Render the "game"
        /// </summary>
        /// <param name="_render">The renderer interface/class</param>
        // #############################################################################################
        public void render()
        {
            // clear software screen with "black" and no alpha
            for (int i = 0; i < GAME_HEIGHT * GAME_WIDTH; i++)
            {
                Screen[i] = 0xff000000;
            }
            Draw.Line(10, 10, GAME_WIDTH - 10, GAME_HEIGHT - 10,0xffff0000);
            Draw.FillRect(100, 100, 300, 200, 0xff00ff00);
            Draw.Rect(90, 90, 310, 210,0xff00ff00);
            Draw.Circle(400, 400, 30, 40, 0xff0000ff);
            Draw.Sprite(300, 10, BallSprite, 0xffff00ff);


            ScreenTexture.Update(Screen);
            Program.DrawDisplay(ScreenTexture, 1, 1, 0xffffffff);


            ButtonManager.paint();

            Font.Draw(100, 100, "HELLO WORLD", 0xffffffff, 2);
            Font.Draw(100, 150, "Hello World £123,456", 0xffffffff, 2);

            if(Button_Pressed) Font.Draw(10, 10, "Button Pressed", 0xffffffff, 2);
        }
    }
}
