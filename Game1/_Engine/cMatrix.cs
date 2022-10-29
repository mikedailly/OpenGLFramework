using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class cMatrix
    {
        public const int _11 = 0;
        public const int _12 = 1;
        public const int _13 = 2;
        public const int _14 = 3;
        public const int _21 = 4;
        public const int _22 = 5;
        public const int _23 = 6;
        public const int _24 = 7;
        public const int _31 = 8;
        public const int _32 = 9;
        public const int _33 = 10;
        public const int _34 = 11;
        public const int _41 = 12;
        public const int _42 = 13;
        public const int _43 = 14;
        public const int _44 = 15;
        public const float DEG2RAD = (float)(Math.PI / 180.0);

        float[] m = new float[16];

        // ###################################################################################################################################################
        /// <summary>
        ///     Access array
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        // ###################################################################################################################################################
        public float this[int _index]
        {
            get { return m[_index]; }
            set { m[_index] = value; }
        }


        // ###################################################################################################################################################
        /// <summary>
        ///     Create a new matrix - unit matrix by default
        /// </summary>
        // ###################################################################################################################################################
        public cMatrix()
        {
            m[_11] = 1.0f;
            m[_22] = 1.0f;
            m[_33] = 1.0f;
            m[_44] = 1.0f;
        }



        // #############################################################################################
        /// <summary>
        ///     Matrix multiply
        /// </summary>
        ///
        /// <param name="_dest">Combined matrix destination</param>
        ///	<param name="_m1">matrix 1</param>
        ///	<param name="_m2">matrix 2</param>
        // #############################################################################################
        public static void Multiply(cMatrix _dest, cMatrix _m1, cMatrix _m2)
        {
            _dest[_11] = (_m1[_11] * _m2[_11]) + (_m1[_12] * _m2[_21]) + (_m1[_13] * _m2[_31]) + (_m1[_14] * _m2[_41]);
            _dest[_12] = (_m1[_11] * _m2[_12]) + (_m1[_12] * _m2[_22]) + (_m1[_13] * _m2[_32]) + (_m1[_14] * _m2[_42]);
            _dest[_13] = (_m1[_11] * _m2[_13]) + (_m1[_12] * _m2[_23]) + (_m1[_13] * _m2[_33]) + (_m1[_14] * _m2[_43]);
            _dest[_14] = (_m1[_11] * _m2[_14]) + (_m1[_12] * _m2[_24]) + (_m1[_13] * _m2[_34]) + (_m1[_14] * _m2[_44]);
            _dest[_21] = (_m1[_21] * _m2[_11]) + (_m1[_22] * _m2[_21]) + (_m1[_23] * _m2[_31]) + (_m1[_24] * _m2[_41]);
            _dest[_22] = (_m1[_21] * _m2[_12]) + (_m1[_22] * _m2[_22]) + (_m1[_23] * _m2[_32]) + (_m1[_24] * _m2[_42]);
            _dest[_23] = (_m1[_21] * _m2[_13]) + (_m1[_22] * _m2[_23]) + (_m1[_23] * _m2[_33]) + (_m1[_24] * _m2[_43]);
            _dest[_24] = (_m1[_21] * _m2[_14]) + (_m1[_22] * _m2[_24]) + (_m1[_23] * _m2[_34]) + (_m1[_24] * _m2[_44]);
            _dest[_31] = (_m1[_31] * _m2[_11]) + (_m1[_32] * _m2[_21]) + (_m1[_33] * _m2[_31]) + (_m1[_34] * _m2[_41]);
            _dest[_32] = (_m1[_31] * _m2[_12]) + (_m1[_32] * _m2[_22]) + (_m1[_33] * _m2[_32]) + (_m1[_34] * _m2[_42]);
            _dest[_33] = (_m1[_31] * _m2[_13]) + (_m1[_32] * _m2[_23]) + (_m1[_33] * _m2[_33]) + (_m1[_34] * _m2[_43]);
            _dest[_34] = (_m1[_31] * _m2[_14]) + (_m1[_32] * _m2[_24]) + (_m1[_33] * _m2[_34]) + (_m1[_34] * _m2[_44]);
            _dest[_41] = (_m1[_41] * _m2[_11]) + (_m1[_42] * _m2[_21]) + (_m1[_43] * _m2[_31]) + (_m1[_44] * _m2[_41]);
            _dest[_42] = (_m1[_41] * _m2[_12]) + (_m1[_42] * _m2[_22]) + (_m1[_43] * _m2[_32]) + (_m1[_44] * _m2[_42]);
            _dest[_43] = (_m1[_41] * _m2[_13]) + (_m1[_42] * _m2[_23]) + (_m1[_43] * _m2[_33]) + (_m1[_44] * _m2[_43]);
            _dest[_44] = (_m1[_41] * _m2[_14]) + (_m1[_42] * _m2[_24]) + (_m1[_43] * _m2[_34]) + (_m1[_44] * _m2[_44]);
        }

        // #############################################################################################
        /// <summary>
        ///     Matrix multiply
        /// </summary>
        ///	<param name="_m1">matrix 1</param>
        ///	<param name="_m2">matrix 2</param>
        /// <returns>A combined matrix</returns>
        // #############################################################################################
        public static cMatrix Multiply(cMatrix _m1, cMatrix _m2)
        {
            cMatrix dest = new cMatrix();
            Multiply(dest, _m1, _m2);
            return dest;
        }

        // ###################################################################################################################################################
        /// <summary>
        ///     Create a full fat matrix
        /// </summary>
        /// <param name="_rotx">X rotation (Pitch / Tilt) in degrees</param>
        /// <param name="_roty">Y rotation (Heading / Pan) in degrees</param>
        /// <param name="_rotz">Z rotation (Roll / Yaw) in degrees</param>
        /// <param name="_sx">X Scale</param>
        /// <param name="_sy">Y Scale</param>
        /// <param name="_sz">Z Scale</param>
        /// <param name="_tx">X Translate</param>
        /// <param name="_ty">Y Translate</param>
        /// <param name="_tz">Z Translate</param>
        // ###################################################################################################################################################
        public static cMatrix CreateMatrix(float _rotx, float _roty, float _rotz, float _sx, float _sy, float _sz, float _tx, float _ty, float _tz)
        {
            float pitch = -_rotx * DEG2RAD;
            float heading = -_roty * DEG2RAD;
            float roll = -_rotz * DEG2RAD;

            float cosh = (float)Math.Cos(heading);
            float cosr = (float)Math.Cos(roll);
            float cosp = (float)Math.Cos(pitch);

            float sinh = (float)Math.Sin(heading);
            float sinr = (float)Math.Sin(roll);
            float sinp = (float)Math.Sin(pitch);

            float sinrsinp = -sinr * -sinp;             // common elements
            float cosrsinp = cosr * -sinp;

            cMatrix mat = new cMatrix();

            mat[0] = ((cosr * cosh) + (sinrsinp * -sinh)) * _sx;
            mat[4] = (-sinr * cosp) * _sx;
            mat[8] = ((cosr * sinh) + (sinrsinp * cosh)) * _sx;
            mat[12] = _tx;

            mat[1] = ((sinr * cosh) + (cosrsinp * -sinh)) * _sy;
            mat[5] = (cosr * cosp) * _sy;
            mat[9] = ((sinr * sinh) + (cosrsinp * cosh)) * _sy;
            mat[13] = _ty;

            mat[2] = (cosp * -sinh) * _sz;
            mat[6] = sinp * _sz;
            mat[10] = (cosp * cosh) * _sz;
            mat[14] = _tz;

            mat[3] = mat[7] = mat[11] = 0.0f;
            mat[15] = 1.0f;

            return mat;
        }

        // #############################################################################################
        /// <summary>
        ///     Create a perspective matrix based on a Field Of View (FOV)
        /// </summary>
        /// <remarks>
        /// (1/tan(_fov*0.5))/aspect             0                 0                 0
        ///              0               1/tan(_fov*0.5)           0                 0
        ///              0                       0            far/(far-near)         1
        ///              0                       0        (-near*far)/(far-near)     0
        /// </remarks>
        /// <param name="_FOV">Field of view to use</param>
        /// <param name="_aspect">Screen aspect ratio</param>
        /// <param name="_zn">Near Z value</param>
        /// <param name="_zf">Far Z value</param>
        /// <returns>A projection matric</returns>
        // #############################################################################################
        public static cMatrix PerspectiveFOV(float _FOV, float _aspect, float _zn, float _zf)
        {
            cMatrix Mat = new cMatrix();
            if ((_FOV == 0.0f) || (_aspect == 0.0f) || (_zn == _zf)) return Mat;

            Mat[_22] = 1.0f / (float)Math.Tan(_FOV * 0.5f);
            Mat[_11] = Mat[_22] / _aspect;
            Mat[_34] = 1.0f;
            Mat[_33] = _zf / (_zf - _zn);
            Mat[_43] = -_zn * _zf / (_zf - _zn);
            return Mat;
        }


        // ###################################################################################################################################################
        /// <summary>
        ///     Transform verts
        /// </summary>
        /// <param name="_verts">Vertex array to transform with a matrix (x,y,z)</param>
        /// <returns>New transformed array</returns>
        // ###################################################################################################################################################
        public float[] Transform(float[] _verts)
        {
            float[] outv = new float[_verts.Length];

            int num_verts = _verts.Length / 3;
            for (int v = 0; v < num_verts; v++)
            {
                int index = v * 3;

                outv[index + 0] = (m[_11] * _verts[index]) + (m[_21] * _verts[index + 1]) + (m[_31] * _verts[index + 2]) + m[_41];
                outv[index + 1] = (m[_12] * _verts[index]) + (m[_22] * _verts[index + 1]) + (m[_32] * _verts[index + 2]) + m[_42];
                outv[index + 2] = (m[_13] * _verts[index]) + (m[_23] * _verts[index + 1]) + (m[_33] * _verts[index + 2]) + m[_43];
            }
            return outv;
        }
    }
}
