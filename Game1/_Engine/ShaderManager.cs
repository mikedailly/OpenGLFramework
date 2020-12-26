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
using Framework.Utils;
using System.IO;

namespace Framework
{
    //
    // Summary:
    //     An enumeration whose values specify various components of a vertex.
    //
    // Remarks:
    //     To be added.
    public enum eVertexAttribType
    {
        Position = 0,
        Normal = 1,
        Color = 2,
        TexCoord0 = 3,
        TexCoord1 = 4
    };

    class ShaderManager
    {
        public const GetProgramParameterName infolen = GetProgramParameterName.InfoLogLength;
        public const GetProgramParameterName LinkStat = GetProgramParameterName.LinkStatus;
        public const GetProgramParameterName ActiveAttributes = GetProgramParameterName.ActiveAttributes;
        public const GetProgramParameterName ActiveUniforms = GetProgramParameterName.ActiveUniforms;


        public static ErrorCode GetGLError()
        {
            return GL.GetError();
        }

        private static bool CompileShader(ShaderType type, string src, out int shader)
        {
            shader = GL.CreateShader(type);
            GL.ShaderSource(shader, src);
            GL.CompileShader(shader);

#if DEBUG
            int logLength = 0;
            GL.GetShader(shader, ShaderParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                Log.WriteLine("Shader compile log:\n{0}", GL.GetShaderInfoLog(shader));
            }
#endif

            int status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                GL.DeleteShader(shader);
                return false;
            }

            return true;
        }


        /// <summary>
        ///     Link the program
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        private static bool LinkProgram(int prog)
        {
            GL.LinkProgram(prog);

#if DEBUG
            int logLength = 0;

            GL.GetProgram(prog, infolen, out logLength);
            if (logLength > 0)
                Log.WriteLine("Program link log:\n{0}", GL.GetProgramInfoLog(prog));
#endif
            int status = 0;
            GL.GetProgram(prog, LinkStat, out status);
            return status != 0;
        }

        /// <summary>
        ///     Load a shader 
        /// </summary>
        /// <param name="_name">Shader name (no extension)</param>
        /// <returns>
        ///     cShader or null for error
        /// </returns>
        public static cShader LoadShader(string _name)
        {
            string basepath = AppDomain.CurrentDomain.BaseDirectory;
            string vshader_src = FileSystem.LoadTextFile(Path.Combine(basepath, _name + ".vsh"));
            string fshader_src = FileSystem.LoadTextFile(Path.Combine(basepath, _name + ".fsh"));

            return LoadShader(_name, vshader_src, fshader_src);
        }

        /// <summary>
        ///     Load a shader 
        /// </summary>
        /// <param name="_name">Shader name (no extension)</param>
        /// <returns>
        ///     cShader or null for error
        /// </returns>
        public static cShader LoadShader( string _shader, string _vsh, string _fsh )
        {
            int vertShader = -1, fragShader = -1;

            ErrorCode err = cUtils.GetGLError();

            // Create shader program.
            int program = GL.CreateProgram();
            err = cUtils.GetGLError();


            string basepath = AppDomain.CurrentDomain.BaseDirectory;

            // Create and compile vertex shader.
            string vshader_src = _vsh; // FileSystem.LoadTextFile( Path.Combine(basepath,_name + ".vsh"));
            if (!CompileShader(ShaderType.VertexShader, vshader_src, out vertShader))
            {

                string errstr = GL.GetShaderInfoLog(vertShader);
                Log.WriteLine(_shader+": VShader compilation error:\n" + errstr);

                GL.DeleteProgram(program);
                Log.WriteLine(_shader+": Failed to compile fragment shader");
                //return null;
            }

            // Create and compile fragment shader.
            string fshader_src = _fsh;   // FileSystem.LoadTextFile(Path.Combine(basepath,_name + ".fsh"));
            if (!CompileShader(ShaderType.FragmentShader, fshader_src, out fragShader))
            {
                string errstr = GL.GetShaderInfoLog(vertShader);
                Log.WriteLine(_shader+": FShader compilation error:\n" + errstr);

                GL.DeleteShader(vertShader);
                GL.DeleteProgram(program);
                Log.WriteLine(_shader+": Failed to compile fragment shader");
                return null;
            }


            // Attach vertex shader to program.
            GL.AttachShader(program, vertShader);

            // Attach fragment shader to program.
            GL.AttachShader(program, fragShader);

            // Bind attribute locations.
            // This needs to be done prior to linking.
            err = cUtils.GetGLError();
            GL.BindAttribLocation(program, (int)eVertexAttribType.Position, "in_position");
            GL.BindAttribLocation(program, (int)eVertexAttribType.Color, "in_colour");
            GL.BindAttribLocation(program, (int)eVertexAttribType.TexCoord0, "in_uv");
            //GL.BindAttribLocation(program, 0, "in_position");
            //GL.BindAttribLocation(program, 1, "in_colour");
            //GL.BindAttribLocation(program, 1, "in_uv");
            err = cUtils.GetGLError();



            // Link program.
            if (!LinkProgram(program))
            {
                Log.WriteLine("Failed to link program: {0:x}", program);

                if (vertShader != 0)
                    GL.DeleteShader(vertShader);

                if (fragShader != 0)
                    GL.DeleteShader(fragShader);

                if (program != 0)
                {
                    GL.DeleteProgram(program);
                    program = 0;
                }
                return null;
            }

            // Get number of attributes and uniforms
            int attrib_count = 0;
            GL.GetProgram(program, ActiveAttributes, out attrib_count);
            int uniform_count = 0;
            GL.GetProgram(program, ActiveUniforms, out uniform_count);

            // new shader object
            cShader shader = new cShader(program, attrib_count, uniform_count);
            shader.program = program;

            // First get all attributes
            int size;
            ActiveAttribType aty;
            for (int i = 0; i < attrib_count; i++)
            {
                string attname = GL.GetActiveAttrib(program, i, out size, out aty);
                shader.AddAttribute(attname, i, size, aty);
            }


            ActiveUniformType uty;
            for (int i = 0; i < uniform_count; i++)
            {
                string uniname = GL.GetActiveUniform(program, i, out size, out uty);
                shader.AddUniform(uniname, i, size, uty);
            }

            // Release vertex and fragment shaders.
            if (vertShader != 0)
            {
                GL.DetachShader(program, vertShader);
                GL.DeleteShader(vertShader);
            }

            if (fragShader != 0)
            {
                GL.DetachShader(program, fragShader);
                GL.DeleteShader(fragShader);
            }

            return shader;
        }
    }
}
