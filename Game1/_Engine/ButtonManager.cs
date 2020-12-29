using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public static class ButtonManager
    {
        public static List<cButton> Buttons = new List<cButton>();


        public static void Tick()
        {
            foreach (cButton button in Buttons)
            {
                button.Tick();
            }
        }


        public static void paint()
        {
            foreach (cButton button in Buttons)
            {
                button.Paint();
            }
        }



    }
}
