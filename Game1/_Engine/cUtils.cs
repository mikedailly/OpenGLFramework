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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    class cUtils
    {
        // #############################################################################################
        /// <summary>
        ///     Get OpenGL/ES error code
        /// </summary>
        /// <returns></returns>
        // #############################################################################################
        public static ErrorCode GetGLError()
        {
            return GL.GetError();
        }
    }
}
