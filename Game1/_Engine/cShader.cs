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
    public enum eUniform
    {
        ModelViewProjectionMatrix,
        ViewMatrix,
        ModelMatrix,
        BaseTexture,
        g_Matrix,
        Paramaters,
        Count
    }

    public class ShaderAttrib
    {
        public string Name;
        public int Index;
        public int Size;
        public ActiveAttribType Type;
    }

    public class ShaderUniform
    {
        public string Name;
        public int Index;
        public int Size;
        public ActiveUniformType Type;
    }

    public class cShader
    {
        /// <summary>known shader uniforms</summary>
        public int[] KnownUniforms;

        /// <summary>Actual shader program.</summary>
        public int program;


        /// <summary>List of attributes and types</summary>
        public List<ShaderAttrib> Attributes;

        /// <summary>List of uniforms and types</summary>
        public List<ShaderUniform> Uniforms;


        // ********************************************************************************
        /// <summary>
        ///     Create a new shader program
        /// </summary>
        /// <param name="_program">The shader program</param>
        /// <param name="_attributes">Number of attributes</param>
        /// <param name="_uniforms">Number of _uniforms</param>
        // ********************************************************************************
        public cShader(int _program, int _attributes, int _uniforms)
        {
            program = _program;
            KnownUniforms = new int[(int)eUniform.Count];
            for (int i = 0; i < (int)eUniform.Count; i++)
            {
                KnownUniforms[i] = -1;
            }

            Attributes = new List<ShaderAttrib>(_attributes);
            Uniforms = new List<ShaderUniform>(_uniforms);

        }

        // ********************************************************************************
        /// <summary>
        ///     Add an Attribute
        /// </summary>
        /// <param name="_name">Attribute name</param>
        /// <param name="_index">attribute index</param>
        /// <param name="_size">element size</param>
        /// <param name="_type">TYPE of element (float, vec2, vec4 etc)</param>
        // ********************************************************************************
        public void AddAttribute(string _name, int _index, int _size, ActiveAttribType _type)
        {
            ShaderAttrib att = new ShaderAttrib();
            att.Name = _name;
            att.Index = _index;
            att.Size = _size;
            att.Type = _type;
            Attributes.Add(att);
            ErrorCode err = cUtils.GetGLError();

            /*
            //
            switch (_name)
            {
                case "in_position":
                    GL.BindAttribLocation(program, (int)eVertexAttribType.Position, _name);
                    break;
                case "in_colour":
                    GL.BindAttribLocation(program, (int)eVertexAttribType.Color, _name);
                    break;
                case "in_normal":
                    GL.BindAttribLocation(program, (int)eVertexAttribType.Normal, _name);
                    break;
                case "in_text0":
                case "in_tex0":
                case "in_uv":
                case "in_uv0":
                    GL.BindAttribLocation(program, (int)eVertexAttribType.TexCoord0, _name);
                    break;
                case "in_text1":
                case "in_tex1":
                case "in_uv1":
                    7GL.BindAttribLocation(program, (int)eVertexAttribType.TexCoord1, _name);
                    break;
            }
            err = GL.GetErrorCode();*/
        }



        // ********************************************************************************
        /// <summary>
        ///     Add a uniform
        /// </summary>
        /// <param name="_name">Uniform name</param>
        /// <param name="_index">uniform index</param>
        /// <param name="_size">element size</param>
        /// <param name="_type">TYPE of element (float, vec2, vec4 etc)</param>
        // ********************************************************************************
        public void AddUniform(string _name, int _index, int _size, ActiveUniformType _type)
        {
            ShaderUniform uni = new ShaderUniform();
            uni.Name = _name;
            uni.Index = _index;
            uni.Size = _size;
            uni.Type = _type;
            Uniforms.Add(uni);

            // Get known uniform locations.
            switch (_name)
            {
                case "g_Matrix[0]":
                    KnownUniforms[(int)eUniform.g_Matrix] = GL.GetUniformLocation(program, "g_Matrix[0]");
                    break;
                case "ModelViewProjectionMatrix":
                    KnownUniforms[(int)eUniform.ModelViewProjectionMatrix] = GL.GetUniformLocation(program, "ModelViewProjectionMatrix");
                    break;
                case "ViewMatrix":
                    KnownUniforms[(int)eUniform.ViewMatrix] = GL.GetUniformLocation(program, "ViewMatrix");
                    break;
                case "ModelMatrix":
                    KnownUniforms[(int)eUniform.ModelMatrix] = GL.GetUniformLocation(program, "ModelMatrix");
                    break;
                case "BaseTexture":
                    KnownUniforms[(int)eUniform.BaseTexture] = GL.GetUniformLocation(program, "BaseTexture");
                    break;
                case "Paramaters":
                    KnownUniforms[(int)eUniform.Paramaters] = GL.GetUniformLocation(program, "Paramaters");
                    break;
            }
        }

        // ********************************************************************************
        /// <summary>
        ///     Update the shader matrices
        /// </summary>
        // ********************************************************************************
        internal void UpdateMatrices()
        {
            
            if (KnownUniforms[(int)eUniform.g_Matrix] >= 0)
            {
                GL.UniformMatrix4(KnownUniforms[(int)eUniform.g_Matrix], 5, false, Render.SceneMatrix_floats);
            }
            if (KnownUniforms[(int)eUniform.ModelViewProjectionMatrix] >= 0)
            {
                GL.UniformMatrix4(KnownUniforms[(int)eUniform.ModelViewProjectionMatrix], false, ref Render.ProjectionMatrix);
            }
            /*if (KnownUniforms[(int)eUniform.ViewMatrix] >= 0)
            {
                GL.UniformMatrix4(KnownUniforms[(int)eUniform.ViewMatrix], false, ref Scene.ViewMatrix);
            }
            if (KnownUniforms[(int)eUniform.ModelMatrix] >= 0)
            {
                GL.UniformMatrix4(KnownUniforms[(int)eUniform.ModelMatrix], false, ref Scene.ModelMatrix);
            }*/
            
        }

        // ********************************************************************************
        /// <summary>
        ///     Set the current shader, and fill in the current projections etc
        /// </summary>
        // ********************************************************************************
        public void Begin()
        {
            // clear error code
            ErrorCode err = cUtils.GetGLError();


            GL.UseProgram(program);
            err = cUtils.GetGLError();
            if (err != ErrorCode.NoError) return;

            UpdateMatrices();

            Render.CurrentShader = this;
        }
        // ********************************************************************************
        /// <summary>
        ///     Set a Vec4 uniform
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <param name="_c"></param>
        /// <param name="_d"></param>
        // ********************************************************************************
        internal void Uniform4f(float _a, float _b, float _c, float _d)
        {
            ErrorCode err = cUtils.GetGLError();
            if (KnownUniforms[(int)eUniform.Paramaters] == -1) return;
            GL.Uniform4(KnownUniforms[(int)eUniform.Paramaters], _a, _b, _c, _d);
            err = cUtils.GetGLError();
        }

        // ********************************************************************************
        /// <summary>
        ///     Clear the current shader
        /// </summary>
            // ********************************************************************************
        public void End()
        {
            GL.UseProgram(0);
        }

    }
}
