using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class cMatrixStack
    {
        Stack<cMatrix> stack;


        // #############################################################################################################################
        /// <summary>
        ///     Create a new matrix stack
        /// </summary>
        // #############################################################################################################################
        public cMatrixStack()
        {
            stack = new Stack<cMatrix>(32);
        }

        // #############################################################################################################################
        /// <summary>
        ///     Clear the stack - usually at the start of each frame
        /// </summary>
        // #############################################################################################################################
        public void Reset()
        {
            stack.Clear();
            stack.Push(new cMatrix());          // push a unit matrix
        }


        // #############################################################################################################################
        /// <summary>
        ///     Push a matrix onto the stack
        /// </summary>
        /// <param name="_matrix">Matrix to push</param>
        // #############################################################################################################################
        public void Push(cMatrix _matrix)
        {
            if (stack.Count == 0)
            {
                stack.Push(_matrix);
            }
            else
            {
                cMatrix m = stack.Peek();
                cMatrix d = cMatrix.Multiply(_matrix, m);
                stack.Push(d);
            }
        }

        // #############################################################################################################################
        /// <summary>
        ///     Pop the top of the stack off
        /// </summary>
        // #############################################################################################################################
        public cMatrix Pop()
        {
            return stack.Pop();
        }


        // #############################################################################################################################
        /// <summary>
        ///     Get the top of the stack
        /// </summary>
        /// <returns>The current top of the stack</returns>
        // #############################################################################################################################
        public cMatrix Peek()
        {
            return stack.Peek();
        }
    }


}
