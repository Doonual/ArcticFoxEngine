using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX;
using Newtonsoft.Json.Linq;

namespace ArcticFoxEngine {
	/// <summary>
	/// Represents a 4x4 mathematical matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Matrix : IEquatable<Matrix>, IFormattable {
		/// <summary>
		/// The size of the <see cref="Matrix"/> type, in bytes.
		/// </summary>
		public static readonly int SizeInBytes = 4 * 4 * sizeof(float);

		/// <summary>
		/// A <see cref="Matrix"/> with all of its components set to zero.
		/// </summary>
		public static readonly Matrix Zero = new Matrix();

		/// <summary>
		/// The identity <see cref="Matrix"/>.
		/// </summary>
		public static readonly Matrix Identity = new Matrix() { M00 = 1.0f, M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };

		/// <summary>
		/// Value at row 1 column 1 of the matrix.
		/// </summary>
		public float M00;

		/// <summary>
		/// Value at row 1 column 2 of the matrix.
		/// </summary>
		public float M01;

		/// <summary>
		/// Value at row 1 column 3 of the matrix.
		/// </summary>
		public float M02;

		/// <summary>
		/// Value at row 1 column 4 of the matrix.
		/// </summary>
		public float M03;

		/// <summary>
		/// Value at row 2 column 1 of the matrix.
		/// </summary>
		public float M10;

		/// <summary>
		/// Value at row 2 column 2 of the matrix.
		/// </summary>
		public float M11;

		/// <summary>
		/// Value at row 2 column 3 of the matrix.
		/// </summary>
		public float M12;

		/// <summary>
		/// Value at row 2 column 4 of the matrix.
		/// </summary>
		public float M13;

		/// <summary>
		/// Value at row 3 column 1 of the matrix.
		/// </summary>
		public float M20;

		/// <summary>
		/// Value at row 3 column 2 of the matrix.
		/// </summary>
		public float M21;

		/// <summary>
		/// Value at row 3 column 3 of the matrix.
		/// </summary>
		public float M22;

		/// <summary>
		/// Value at row 3 column 4 of the matrix.
		/// </summary>
		public float M23;

		/// <summary>
		/// Value at row 4 column 1 of the matrix.
		/// </summary>
		public float M30;

		/// <summary>
		/// Value at row 4 column 2 of the matrix.
		/// </summary>
		public float M31;

		/// <summary>
		/// Value at row 4 column 3 of the matrix.
		/// </summary>
		public float M32;

		/// <summary>
		/// Value at row 4 column 4 of the matrix.
		/// </summary>
		public float M33;

		/// <summary>
		/// Gets or sets the up <see cref="Vector3"/> of the matrix; that is M21, M22, and M23.
		/// </summary>
		public Vector3 Up {
			get {
				Vector3 vector3;
				vector3.x = this.M10;
				vector3.y = this.M11;
				vector3.z = this.M12;
				return vector3;
			}
			set {
				this.M10 = value.x;
				this.M11 = value.y;
				this.M12 = value.z;
			}
		}

		/// <summary>
		/// Gets or sets the down <see cref="Vector3"/> of the matrix; that is -M21, -M22, and -M23.
		/// </summary>
		public Vector3 Down {
			get {
				Vector3 vector3;
				vector3.x = -this.M10;
				vector3.y = -this.M11;
				vector3.z = -this.M12;
				return vector3;
			}
			set {
				this.M10 = -value.x;
				this.M11 = -value.y;
				this.M12 = -value.z;
			}
		}

		/// <summary>
		/// Gets or sets the right <see cref="Vector3"/> of the matrix; that is M11, M12, and M13.
		/// </summary>
		public Vector3 Right {
			get {
				Vector3 vector3;
				vector3.x = this.M00;
				vector3.y = this.M01;
				vector3.z = this.M02;
				return vector3;
			}
			set {
				this.M00 = value.x;
				this.M01 = value.y;
				this.M02 = value.z;
			}
		}

		/// <summary>
		/// Gets or sets the left <see cref="Vector3"/> of the matrix; that is -M11, -M12, and -M13.
		/// </summary>
		public Vector3 Left {
			get {
				Vector3 vector3;
				vector3.x = -this.M00;
				vector3.y = -this.M01;
				vector3.z = -this.M02;
				return vector3;
			}
			set {
				this.M00 = -value.x;
				this.M01 = -value.y;
				this.M02 = -value.z;
			}
		}

		/// <summary>
		/// Gets or sets the forward <see cref="Vector3"/> of the matrix; that is -M31, -M32, and -M33.
		/// </summary>
		public Vector3 Forward {
			get {
				Vector3 vector3;
				vector3.x = -this.M20;
				vector3.y = -this.M21;
				vector3.z = -this.M22;
				return vector3;
			}
			set {
				this.M20 = -value.x;
				this.M21 = -value.y;
				this.M22 = -value.z;
			}
		}

		/// <summary>
		/// Gets or sets the backward <see cref="Vector3"/> of the matrix; that is M31, M32, and M33.
		/// </summary>
		public Vector3 Backward {
			get {
				Vector3 vector3;
				vector3.x = this.M20;
				vector3.y = this.M21;
				vector3.z = this.M22;
				return vector3;
			}
			set {
				this.M20 = value.x;
				this.M21 = value.y;
				this.M22 = value.z;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix"/> struct.
		/// </summary>
		/// <param name="value">The value that will be assigned to all components.</param>
		public Matrix(float value) {
			M00 = M01 = M02 = M03 =
			M10 = M11 = M12 = M13 =
			M20 = M21 = M22 = M23 =
			M30 = M31 = M32 = M33 = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix"/> struct.
		/// </summary>
		/// <param name="M00">The value to assign at row 1 column 1 of the matrix.</param>
		/// <param name="M01">The value to assign at row 1 column 2 of the matrix.</param>
		/// <param name="M02">The value to assign at row 1 column 3 of the matrix.</param>
		/// <param name="M03">The value to assign at row 1 column 4 of the matrix.</param>
		/// <param name="M10">The value to assign at row 2 column 1 of the matrix.</param>
		/// <param name="M11">The value to assign at row 2 column 2 of the matrix.</param>
		/// <param name="M12">The value to assign at row 2 column 3 of the matrix.</param>
		/// <param name="M13">The value to assign at row 2 column 4 of the matrix.</param>
		/// <param name="M20">The value to assign at row 3 column 1 of the matrix.</param>
		/// <param name="M21">The value to assign at row 3 column 2 of the matrix.</param>
		/// <param name="M22">The value to assign at row 3 column 3 of the matrix.</param>
		/// <param name="M23">The value to assign at row 3 column 4 of the matrix.</param>
		/// <param name="M30">The value to assign at row 4 column 1 of the matrix.</param>
		/// <param name="M31">The value to assign at row 4 column 2 of the matrix.</param>
		/// <param name="M32">The value to assign at row 4 column 3 of the matrix.</param>
		/// <param name="M33">The value to assign at row 4 column 4 of the matrix.</param>
		public Matrix(float M00, float M01, float M02, float M03,
			float M10, float M11, float M12, float M13,
			float M20, float M21, float M22, float M23,
			float M30, float M31, float M32, float M33) {
			this.M00 = M00; this.M01 = M01; this.M02 = M02; this.M03 = M03;
			this.M10 = M10; this.M11 = M11; this.M12 = M12; this.M13 = M13;
			this.M20 = M20; this.M21 = M21; this.M22 = M22; this.M23 = M23;
			this.M30 = M30; this.M31 = M31; this.M32 = M32; this.M33 = M33;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix"/> struct.
		/// </summary>
		/// <param name="values">The values to assign to the components of the matrix. This must be an array with sixteen elements.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than sixteen elements.</exception>
		public Matrix(float[] values) {
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length != 16)
				throw new ArgumentOutOfRangeException("values", "There must be sixteen and only sixteen input values for Matrix.");

			M00 = values[0];
			M01 = values[1];
			M02 = values[2];
			M03 = values[3];

			M10 = values[4];
			M11 = values[5];
			M12 = values[6];
			M13 = values[7];

			M20 = values[8];
			M21 = values[9];
			M22 = values[10];
			M23 = values[11];

			M30 = values[12];
			M31 = values[13];
			M32 = values[14];
			M33 = values[15];
		}

		/// <summary>
		/// Gets or sets the first row in the matrix; that is M11, M12, M13, and M14.
		/// </summary>
		public Vector4 Row0 {
			get { return new Vector4(M00, M01, M02, M03); }
			set { M00 = value.x; M01 = value.y; M02 = value.z; M03 = value.w; }
		}

		/// <summary>
		/// Gets or sets the second row in the matrix; that is M21, M22, M23, and M24.
		/// </summary>
		public Vector4 Row1 {
			get { return new Vector4(M10, M11, M12, M13); }
			set { M10 = value.x; M11 = value.y; M12 = value.z; M13 = value.w; }
		}

		/// <summary>
		/// Gets or sets the third row in the matrix; that is M31, M32, M33, and M34.
		/// </summary>
		public Vector4 Row2 {
			get { return new Vector4(M20, M21, M22, M23); }
			set { M20 = value.x; M21 = value.y; M22 = value.z; M23 = value.w; }
		}

		/// <summary>
		/// Gets or sets the fourth row in the matrix; that is M41, M42, M43, and M44.
		/// </summary>
		public Vector4 Row3 {
			get { return new Vector4(M30, M31, M32, M33); }
			set { M30 = value.x; M31 = value.y; M32 = value.z; M33 = value.w; }
		}

		/// <summary>
		/// Gets or sets the first column in the matrix; that is M11, M21, M31, and M41.
		/// </summary>
		public Vector4 Column0 {
			get { return new Vector4(M00, M10, M20, M30); }
			set { M00 = value.x; M10 = value.y; M20 = value.z; M30 = value.w; }
		}

		/// <summary>
		/// Gets or sets the second column in the matrix; that is M12, M22, M32, and M42.
		/// </summary>
		public Vector4 Column1 {
			get { return new Vector4(M01, M11, M21, M31); }
			set { M01 = value.x; M11 = value.y; M21 = value.z; M31 = value.w; }
		}

		/// <summary>
		/// Gets or sets the third column in the matrix; that is M13, M23, M33, and M43.
		/// </summary>
		public Vector4 Column2 {
			get { return new Vector4(M02, M12, M22, M32); }
			set { M02 = value.x; M12 = value.y; M22 = value.z; M32 = value.w; }
		}

		/// <summary>
		/// Gets or sets the fourth column in the matrix; that is M14, M24, M34, and M44.
		/// </summary>
		public Vector4 Column3 {
			get { return new Vector4(M03, M13, M23, M33); }
			set { M03 = value.x; M13 = value.y; M23 = value.z; M33 = value.w; }
		}

		/// <summary>
		/// Gets or sets the translation of the matrix; that is M41, M42, and M43.
		/// </summary>
		public Vector3 TranslationVector {
			get { return new Vector3(M30, M31, M32); }
			set { M30 = value.x; M31 = value.y; M32 = value.z; }
		}

		/// <summary>
		/// Gets or sets the scale of the matrix; that is M11, M22, and M33.
		/// </summary>
		public Vector3 ScaleVector {
			get { return new Vector3(M00, M11, M22); }
			set { M00 = value.x; M11 = value.y; M22 = value.z; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is an identity matrix.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is an identity matrix; otherwise, <c>false</c>.
		/// </value>
		public bool IsIdentity {
			get { return this.Equals(Identity); }
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="index">The zero-based index of the component to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
		public float this[int index] {
			get {
				switch (index) {
					case 0: return M00;
					case 1: return M01;
					case 2: return M02;
					case 3: return M03;
					case 4: return M10;
					case 5: return M11;
					case 6: return M12;
					case 7: return M13;
					case 8: return M20;
					case 9: return M21;
					case 10: return M22;
					case 11: return M23;
					case 12: return M30;
					case 13: return M31;
					case 14: return M32;
					case 15: return M33;
				}

				throw new ArgumentOutOfRangeException("index", "Indices for Matrix run from 0 to 15, inclusive.");
			}

			set {
				switch (index) {
					case 0: M00 = value; break;
					case 1: M01 = value; break;
					case 2: M02 = value; break;
					case 3: M03 = value; break;
					case 4: M10 = value; break;
					case 5: M11 = value; break;
					case 6: M12 = value; break;
					case 7: M13 = value; break;
					case 8: M20 = value; break;
					case 9: M21 = value; break;
					case 10: M22 = value; break;
					case 11: M23 = value; break;
					case 12: M30 = value; break;
					case 13: M31 = value; break;
					case 14: M32 = value; break;
					case 15: M33 = value; break;
					default: throw new ArgumentOutOfRangeException("index", "Indices for Matrix run from 0 to 15, inclusive.");
				}
			}
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="row">The row of the matrix to access.</param>
		/// <param name="column">The column of the matrix to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
		public float this[int row, int column] {
			get {
				if (row < 0 || row > 3)
					throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
				if (column < 0 || column > 3)
					throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

				return this[(row * 4) + column];
			}

			set {
				if (row < 0 || row > 3)
					throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
				if (column < 0 || column > 3)
					throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

				this[(row * 4) + column] = value;
			}
		}

		/// <summary>
		/// Calculates the determinant of the matrix.
		/// </summary>
		/// <returns>The determinant of the matrix.</returns>
		public float Determinant() {
			float temp1 = (M22 * M33) - (M23 * M32);
			float temp2 = (M21 * M33) - (M23 * M31);
			float temp3 = (M21 * M32) - (M22 * M31);
			float temp4 = (M20 * M33) - (M23 * M30);
			float temp5 = (M20 * M32) - (M22 * M30);
			float temp6 = (M20 * M31) - (M21 * M30);

			return ((((M00 * (((M11 * temp1) - (M12 * temp2)) + (M13 * temp3))) - (M01 * (((M10 * temp1) -
				(M12 * temp4)) + (M13 * temp5)))) + (M02 * (((M10 * temp2) - (M11 * temp4)) + (M13 * temp6)))) -
				(M03 * (((M10 * temp3) - (M11 * temp5)) + (M12 * temp6))));
		}


		/// <summary>
		/// Transposes the matrix.
		/// </summary>
		public Matrix Transpose() {
			
			Matrix temp = new Matrix();
			temp.M00 = M00;
			temp.M01 = M10;
			temp.M02 = M20;
			temp.M03 = M30;
			temp.M10 = M01;
			temp.M11 = M11;
			temp.M12 = M21;
			temp.M13 = M31;
			temp.M20 = M02;
			temp.M21 = M12;
			temp.M22 = M22;
			temp.M23 = M32;
			temp.M30 = M03;
			temp.M31 = M13;
			temp.M32 = M23;
			temp.M33 = M33;

			return temp;

		}

		/// <summary>
		/// Orthogonalizes the specified matrix.
		/// </summary>
		/// <remarks>
		/// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
		/// means that any given row in the matrix will be orthogonal to any other given row in the
		/// matrix.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public void Orthogonalize() {
			Orthogonalize(ref this, out this);
		}

		/// <summary>
		/// Orthonormalizes the specified matrix.
		/// </summary>
		/// <remarks>
		/// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
		/// other and making all rows and columns of unit length. This means that any given row will
		/// be orthogonal to any other given row and any given column will be orthogonal to any other
		/// given column. Any given row will not be orthogonal to any given column. Every row and every
		/// column will be of unit length.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public void Orthonormalize() {
			Orthonormalize(ref this, out this);
		}

		/// <summary>
		/// Decomposes a matrix into an orthonormalized matrix Q and a right triangular matrix R.
		/// </summary>
		/// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
		/// <param name="R">When the method completes, contains the right triangular matrix of the decomposition.</param>
		public void DecomposeQR(out Matrix Q, out Matrix R) {
			Matrix temp = this;
			temp.Transpose();
			Orthonormalize(ref temp, out Q);
			Q.Transpose();

			R = new Matrix();
			R.M00 = Vector4.Dot(Q.Column0, Column0);
			R.M01 = Vector4.Dot(Q.Column0, Column1);
			R.M02 = Vector4.Dot(Q.Column0, Column2);
			R.M03 = Vector4.Dot(Q.Column0, Column3);

			R.M11 = Vector4.Dot(Q.Column1, Column1);
			R.M12 = Vector4.Dot(Q.Column1, Column2);
			R.M13 = Vector4.Dot(Q.Column1, Column3);

			R.M22 = Vector4.Dot(Q.Column2, Column2);
			R.M23 = Vector4.Dot(Q.Column2, Column3);

			R.M33 = Vector4.Dot(Q.Column3, Column3);
		}

		/// <summary>
		/// Decomposes a matrix into a lower triangular matrix L and an orthonormalized matrix Q.
		/// </summary>
		/// <param name="L">When the method completes, contains the lower triangular matrix of the decomposition.</param>
		/// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
		public void DecomposeLQ(out Matrix L, out Matrix Q) {
			Orthonormalize(ref this, out Q);

			L = new Matrix();
			L.M00 = Vector4.Dot(Q.Row0, Row0);

			L.M10 = Vector4.Dot(Q.Row0, Row1);
			L.M11 = Vector4.Dot(Q.Row1, Row1);

			L.M20 = Vector4.Dot(Q.Row0, Row2);
			L.M21 = Vector4.Dot(Q.Row1, Row2);
			L.M22 = Vector4.Dot(Q.Row2, Row2);

			L.M30 = Vector4.Dot(Q.Row0, Row3);
			L.M31 = Vector4.Dot(Q.Row1, Row3);
			L.M32 = Vector4.Dot(Q.Row2, Row3);
			L.M33 = Vector4.Dot(Q.Row3, Row3);
		}

		/// <summary>
		/// Decomposes a matrix into a scale, rotation, and translation.
		/// </summary>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
		/// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose an SRT transformation matrix only.
		/// </remarks>
		public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation) {
			//Source: Unknown
			//References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

			//Get the translation.
			translation.x = this.M30;
			translation.y = this.M31;
			translation.z = this.M32;

			//Scaling is the length of the rows.
			scale.x = (float)Math.Sqrt((M00 * M00) + (M01 * M01) + (M02 * M02));
			scale.y = (float)Math.Sqrt((M10 * M10) + (M11 * M11) + (M12 * M12));
			scale.z = (float)Math.Sqrt((M20 * M20) + (M21 * M21) + (M22 * M22));

			//If any of the scaling factors are zero, than the rotation matrix can not exist.
			if (MathUtil.IsZero(scale.x) ||
				MathUtil.IsZero(scale.y) ||
				MathUtil.IsZero(scale.z)) {
				rotation = Quaternion.Identity;
				return false;
			}

			//The rotation is the left over matrix after dividing out the scaling.
			Matrix rotationmatrix = new Matrix();
			rotationmatrix.M00 = M00 / scale.x;
			rotationmatrix.M01 = M01 / scale.x;
			rotationmatrix.M02 = M02 / scale.x;

			rotationmatrix.M10 = M10 / scale.y;
			rotationmatrix.M11 = M11 / scale.y;
			rotationmatrix.M12 = M12 / scale.y;

			rotationmatrix.M20 = M20 / scale.z;
			rotationmatrix.M21 = M21 / scale.z;
			rotationmatrix.M22 = M22 / scale.z;

			rotationmatrix.M33 = 1f;

			Quaternion.RotationMatrix(ref rotationmatrix, out rotation);
			return true;
		}

		/// <summary>
		/// Decomposes a uniform scale matrix into a scale, rotation, and translation.
		/// A uniform scale matrix has the same scale in every axis.
		/// </summary>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
		/// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose only an SRT transformation matrix that has the same scale in every axis.
		/// </remarks>
		public bool DecomposeUniformScale(out float scale, out Quaternion rotation, out Vector3 translation) {
			//Get the translation.
			translation.x = this.M30;
			translation.y = this.M31;
			translation.z = this.M32;

			//Scaling is the length of the rows. ( just take one row since this is a uniform matrix)
			scale = (float)Math.Sqrt((M00 * M00) + (M01 * M01) + (M02 * M02));
			var inv_scale = 1f / scale;

			//If any of the scaling factors are zero, then the rotation matrix can not exist.
			if (Math.Abs(scale) < MathUtil.ZeroTolerance) {
				rotation = Quaternion.Identity;
				return false;
			}

			//The rotation is the left over matrix after dividing out the scaling.
			Matrix rotationmatrix = new Matrix();
			rotationmatrix.M00 = M00 * inv_scale;
			rotationmatrix.M01 = M01 * inv_scale;
			rotationmatrix.M02 = M02 * inv_scale;

			rotationmatrix.M10 = M10 * inv_scale;
			rotationmatrix.M11 = M11 * inv_scale;
			rotationmatrix.M12 = M12 * inv_scale;

			rotationmatrix.M20 = M20 * inv_scale;
			rotationmatrix.M21 = M21 * inv_scale;
			rotationmatrix.M22 = M22 * inv_scale;

			rotationmatrix.M33 = 1f;

			Quaternion.RotationMatrix(ref rotationmatrix, out rotation);
			return true;
		}

		/// <summary>
		/// Exchanges two rows in the matrix.
		/// </summary>
		/// <param name="firstRow">The first row to exchange. This is an index of the row starting at zero.</param>
		/// <param name="secondRow">The second row to exchange. This is an index of the row starting at zero.</param>
		public void ExchangeRows(int firstRow, int secondRow) {
			if (firstRow < 0)
				throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be greater than or equal to zero.");
			if (firstRow > 3)
				throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be less than or equal to three.");
			if (secondRow < 0)
				throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be greater than or equal to zero.");
			if (secondRow > 3)
				throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be less than or equal to three.");

			if (firstRow == secondRow)
				return;

			float temp0 = this[secondRow, 0];
			float temp1 = this[secondRow, 1];
			float temp2 = this[secondRow, 2];
			float temp3 = this[secondRow, 3];

			this[secondRow, 0] = this[firstRow, 0];
			this[secondRow, 1] = this[firstRow, 1];
			this[secondRow, 2] = this[firstRow, 2];
			this[secondRow, 3] = this[firstRow, 3];

			this[firstRow, 0] = temp0;
			this[firstRow, 1] = temp1;
			this[firstRow, 2] = temp2;
			this[firstRow, 3] = temp3;
		}

		/// <summary>
		/// Exchanges two columns in the matrix.
		/// </summary>
		/// <param name="firstColumn">The first column to exchange. This is an index of the column starting at zero.</param>
		/// <param name="secondColumn">The second column to exchange. This is an index of the column starting at zero.</param>
		public void ExchangeColumns(int firstColumn, int secondColumn) {
			if (firstColumn < 0)
				throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be greater than or equal to zero.");
			if (firstColumn > 3)
				throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be less than or equal to three.");
			if (secondColumn < 0)
				throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be greater than or equal to zero.");
			if (secondColumn > 3)
				throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be less than or equal to three.");

			if (firstColumn == secondColumn)
				return;

			float temp0 = this[0, secondColumn];
			float temp1 = this[1, secondColumn];
			float temp2 = this[2, secondColumn];
			float temp3 = this[3, secondColumn];

			this[0, secondColumn] = this[0, firstColumn];
			this[1, secondColumn] = this[1, firstColumn];
			this[2, secondColumn] = this[2, firstColumn];
			this[3, secondColumn] = this[3, firstColumn];

			this[0, firstColumn] = temp0;
			this[1, firstColumn] = temp1;
			this[2, firstColumn] = temp2;
			this[3, firstColumn] = temp3;
		}

		/// <summary>
		/// Creates an array containing the elements of the matrix.
		/// </summary>
		/// <returns>A sixteen-element array containing the components of the matrix.</returns>
		public float[] ToArray() {
			return new[] { M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23, M30, M31, M32, M33 };
		}

		/// <summary>
		/// Determines the sum of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two matrices.</param>
		public static void Add(ref Matrix left, ref Matrix right, out Matrix result) {
			result.M00 = left.M00 + right.M00;
			result.M01 = left.M01 + right.M01;
			result.M02 = left.M02 + right.M02;
			result.M03 = left.M03 + right.M03;
			result.M10 = left.M10 + right.M10;
			result.M11 = left.M11 + right.M11;
			result.M12 = left.M12 + right.M12;
			result.M13 = left.M13 + right.M13;
			result.M20 = left.M20 + right.M20;
			result.M21 = left.M21 + right.M21;
			result.M22 = left.M22 + right.M22;
			result.M23 = left.M23 + right.M23;
			result.M30 = left.M30 + right.M30;
			result.M31 = left.M31 + right.M31;
			result.M32 = left.M32 + right.M32;
			result.M33 = left.M33 + right.M33;
		}

		/// <summary>
		/// Determines the sum of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		/// <returns>The sum of the two matrices.</returns>
		public static Matrix Add(Matrix left, Matrix right) {
			Matrix result;
			Add(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Determines the difference between two matrices.
		/// </summary>
		/// <param name="left">The first matrix to subtract.</param>
		/// <param name="right">The second matrix to subtract.</param>
		/// <param name="result">When the method completes, contains the difference between the two matrices.</param>
		public static void Subtract(ref Matrix left, ref Matrix right, out Matrix result) {
			result.M00 = left.M00 - right.M00;
			result.M01 = left.M01 - right.M01;
			result.M02 = left.M02 - right.M02;
			result.M03 = left.M03 - right.M03;
			result.M10 = left.M10 - right.M10;
			result.M11 = left.M11 - right.M11;
			result.M12 = left.M12 - right.M12;
			result.M13 = left.M13 - right.M13;
			result.M20 = left.M20 - right.M20;
			result.M21 = left.M21 - right.M21;
			result.M22 = left.M22 - right.M22;
			result.M23 = left.M23 - right.M23;
			result.M30 = left.M30 - right.M30;
			result.M31 = left.M31 - right.M31;
			result.M32 = left.M32 - right.M32;
			result.M33 = left.M33 - right.M33;
		}

		/// <summary>
		/// Determines the difference between two matrices.
		/// </summary>
		/// <param name="left">The first matrix to subtract.</param>
		/// <param name="right">The second matrix to subtract.</param>
		/// <returns>The difference between the two matrices.</returns>
		public static Matrix Subtract(Matrix left, Matrix right) {
			Matrix result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Multiply(ref Matrix left, float right, out Matrix result) {
			result.M00 = left.M00 * right;
			result.M01 = left.M01 * right;
			result.M02 = left.M02 * right;
			result.M03 = left.M03 * right;
			result.M10 = left.M10 * right;
			result.M11 = left.M11 * right;
			result.M12 = left.M12 * right;
			result.M13 = left.M13 * right;
			result.M20 = left.M20 * right;
			result.M21 = left.M21 * right;
			result.M22 = left.M22 * right;
			result.M23 = left.M23 * right;
			result.M30 = left.M30 * right;
			result.M31 = left.M31 * right;
			result.M32 = left.M32 * right;
			result.M33 = left.M33 * right;
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix Multiply(Matrix left, float right) {
			Matrix result;
			Multiply(ref left, right, out result);
			return result;
		}

		/// <summary>
		/// Determines the product of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to multiply.</param>
		/// <param name="right">The second matrix to multiply.</param>
		/// <param name="result">The product of the two matrices.</param>
		public static void Multiply(ref Matrix left, ref Matrix right, out Matrix result) {
			Matrix temp = new Matrix();
			temp.M00 = (left.M00 * right.M00) + (left.M01 * right.M10) + (left.M02 * right.M20) + (left.M03 * right.M30);
			temp.M01 = (left.M00 * right.M01) + (left.M01 * right.M11) + (left.M02 * right.M21) + (left.M03 * right.M31);
			temp.M02 = (left.M00 * right.M02) + (left.M01 * right.M12) + (left.M02 * right.M22) + (left.M03 * right.M32);
			temp.M03 = (left.M00 * right.M03) + (left.M01 * right.M13) + (left.M02 * right.M23) + (left.M03 * right.M33);
			temp.M10 = (left.M10 * right.M00) + (left.M11 * right.M10) + (left.M12 * right.M20) + (left.M13 * right.M30);
			temp.M11 = (left.M10 * right.M01) + (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31);
			temp.M12 = (left.M10 * right.M02) + (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32);
			temp.M13 = (left.M10 * right.M03) + (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33);
			temp.M20 = (left.M20 * right.M00) + (left.M21 * right.M10) + (left.M22 * right.M20) + (left.M23 * right.M30);
			temp.M21 = (left.M20 * right.M01) + (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31);
			temp.M22 = (left.M20 * right.M02) + (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32);
			temp.M23 = (left.M20 * right.M03) + (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33);
			temp.M30 = (left.M30 * right.M00) + (left.M31 * right.M10) + (left.M32 * right.M20) + (left.M33 * right.M30);
			temp.M31 = (left.M30 * right.M01) + (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31);
			temp.M32 = (left.M30 * right.M02) + (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32);
			temp.M33 = (left.M30 * right.M03) + (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33);
			result = temp;
		}

		/// <summary>
		/// Determines the product of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to multiply.</param>
		/// <param name="right">The second matrix to multiply.</param>
		/// <returns>The product of the two matrices.</returns>
		public static Matrix Multiply(Matrix left, Matrix right) {
			Matrix result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Divide(ref Matrix left, float right, out Matrix result) {
			float inv = 1.0f / right;

			result.M00 = left.M00 * inv;
			result.M01 = left.M01 * inv;
			result.M02 = left.M02 * inv;
			result.M03 = left.M03 * inv;
			result.M10 = left.M10 * inv;
			result.M11 = left.M11 * inv;
			result.M12 = left.M12 * inv;
			result.M13 = left.M13 * inv;
			result.M20 = left.M20 * inv;
			result.M21 = left.M21 * inv;
			result.M22 = left.M22 * inv;
			result.M23 = left.M23 * inv;
			result.M30 = left.M30 * inv;
			result.M31 = left.M31 * inv;
			result.M32 = left.M32 * inv;
			result.M33 = left.M33 * inv;
		}

		/// <summary>
		/// Scales a matrix by the given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix Divide(Matrix left, float right) {
			Matrix result;
			Divide(ref left, right, out result);
			return result;
		}

		/// <summary>
		/// Determines the quotient of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to divide.</param>
		/// <param name="right">The second matrix to divide.</param>
		/// <param name="result">When the method completes, contains the quotient of the two matrices.</param>
		public static void Divide(ref Matrix left, ref Matrix right, out Matrix result) {
			result.M00 = left.M00 / right.M00;
			result.M01 = left.M01 / right.M01;
			result.M02 = left.M02 / right.M02;
			result.M03 = left.M03 / right.M03;
			result.M10 = left.M10 / right.M10;
			result.M11 = left.M11 / right.M11;
			result.M12 = left.M12 / right.M12;
			result.M13 = left.M13 / right.M13;
			result.M20 = left.M20 / right.M20;
			result.M21 = left.M21 / right.M21;
			result.M22 = left.M22 / right.M22;
			result.M23 = left.M23 / right.M23;
			result.M30 = left.M30 / right.M30;
			result.M31 = left.M31 / right.M31;
			result.M32 = left.M32 / right.M32;
			result.M33 = left.M33 / right.M33;
		}

		/// <summary>
		/// Determines the quotient of two matrices.
		/// </summary>
		/// <param name="left">The first matrix to divide.</param>
		/// <param name="right">The second matrix to divide.</param>
		/// <returns>The quotient of the two matrices.</returns>
		public static Matrix Divide(Matrix left, Matrix right) {
			Matrix result;
			Divide(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Performs the exponential operation on a matrix.
		/// </summary>
		/// <param name="value">The matrix to perform the operation on.</param>
		/// <param name="exponent">The exponent to raise the matrix to.</param>
		/// <param name="result">When the method completes, contains the exponential matrix.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
		public static void Exponent(ref Matrix value, int exponent, out Matrix result) {
			//Source: http://rosettacode.org
			//Reference: http://rosettacode.org/wiki/Matrix-exponentiation_operator

			if (exponent < 0)
				throw new ArgumentOutOfRangeException("exponent", "The exponent can not be negative.");

			if (exponent == 0) {
				result = Matrix.Identity;
				return;
			}

			if (exponent == 1) {
				result = value;
				return;
			}

			Matrix identity = Matrix.Identity;
			Matrix temp = value;

			while (true) {
				if ((exponent & 1) != 0)
					identity = identity * temp;

				exponent /= 2;

				if (exponent > 0)
					temp *= temp;
				else
					break;
			}

			result = identity;
		}

		/// <summary>
		/// Performs the exponential operation on a matrix.
		/// </summary>
		/// <param name="value">The matrix to perform the operation on.</param>
		/// <param name="exponent">The exponent to raise the matrix to.</param>
		/// <returns>The exponential matrix.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
		public static Matrix Exponent(Matrix value, int exponent) {
			Matrix result;
			Exponent(ref value, exponent, out result);
			return result;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="value">The matrix to be negated.</param>
		/// <param name="result">When the method completes, contains the negated matrix.</param>
		public static void Negate(ref Matrix value, out Matrix result) {
			result.M00 = -value.M00;
			result.M01 = -value.M01;
			result.M02 = -value.M02;
			result.M03 = -value.M03;
			result.M10 = -value.M10;
			result.M11 = -value.M11;
			result.M12 = -value.M12;
			result.M13 = -value.M13;
			result.M20 = -value.M20;
			result.M21 = -value.M21;
			result.M22 = -value.M22;
			result.M23 = -value.M23;
			result.M30 = -value.M30;
			result.M31 = -value.M31;
			result.M32 = -value.M32;
			result.M33 = -value.M33;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="value">The matrix to be negated.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix Negate(Matrix value) {
			Matrix result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Performs a linear interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <param name="result">When the method completes, contains the linear interpolation of the two matrices.</param>
		/// <remarks>
		/// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
		/// </remarks>
		public static void Lerp(ref Matrix start, ref Matrix end, float amount, out Matrix result) {
			result.M00 = MathUtil.Lerp(start.M00, end.M00, amount);
			result.M01 = MathUtil.Lerp(start.M01, end.M01, amount);
			result.M02 = MathUtil.Lerp(start.M02, end.M02, amount);
			result.M03 = MathUtil.Lerp(start.M03, end.M03, amount);
			result.M10 = MathUtil.Lerp(start.M10, end.M10, amount);
			result.M11 = MathUtil.Lerp(start.M11, end.M11, amount);
			result.M12 = MathUtil.Lerp(start.M12, end.M12, amount);
			result.M13 = MathUtil.Lerp(start.M13, end.M13, amount);
			result.M20 = MathUtil.Lerp(start.M20, end.M20, amount);
			result.M21 = MathUtil.Lerp(start.M21, end.M21, amount);
			result.M22 = MathUtil.Lerp(start.M22, end.M22, amount);
			result.M23 = MathUtil.Lerp(start.M23, end.M23, amount);
			result.M30 = MathUtil.Lerp(start.M30, end.M30, amount);
			result.M31 = MathUtil.Lerp(start.M31, end.M31, amount);
			result.M32 = MathUtil.Lerp(start.M32, end.M32, amount);
			result.M33 = MathUtil.Lerp(start.M33, end.M33, amount);
		}

		/// <summary>
		/// Performs a linear interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <returns>The linear interpolation of the two matrices.</returns>
		/// <remarks>
		/// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
		/// </remarks>
		public static Matrix Lerp(Matrix start, Matrix end, float amount) {
			Matrix result;
			Lerp(ref start, ref end, amount, out result);
			return result;
		}

		/// <summary>
		/// Performs a cubic interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <param name="result">When the method completes, contains the cubic interpolation of the two matrices.</param>
		public static void SmoothStep(ref Matrix start, ref Matrix end, float amount, out Matrix result) {
			amount = MathUtil.SmoothStep(amount);
			Lerp(ref start, ref end, amount, out result);
		}

		/// <summary>
		/// Performs a cubic interpolation between two matrices.
		/// </summary>
		/// <param name="start">Start matrix.</param>
		/// <param name="end">End matrix.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <returns>The cubic interpolation of the two matrices.</returns>
		public static Matrix SmoothStep(Matrix start, Matrix end, float amount) {
			Matrix result;
			SmoothStep(ref start, ref end, amount, out result);
			return result;
		}

		/// <summary>
		/// Calculates the transpose of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose transpose is to be calculated.</param>
		public static Matrix Transpose(Matrix value) {
			Matrix temp = new Matrix();
			temp.M00 = value.M00;
			temp.M01 = value.M10;
			temp.M02 = value.M20;
			temp.M03 = value.M30;
			temp.M10 = value.M01;
			temp.M11 = value.M11;
			temp.M12 = value.M21;
			temp.M13 = value.M31;
			temp.M20 = value.M02;
			temp.M21 = value.M12;
			temp.M22 = value.M22;
			temp.M23 = value.M32;
			temp.M30 = value.M03;
			temp.M31 = value.M13;
			temp.M32 = value.M23;
			temp.M33 = value.M33;

			return temp;
		}

		/// <summary>
		/// Calculates the transpose of the specified matrix.
		/// </summary>
		/// <param name="value">The matrix whose transpose is to be calculated.</param>
		/// <param name="result">When the method completes, contains the transpose of the specified matrix.</param>
		public static void TransposeByRef(ref Matrix value, ref Matrix result) {
			result.M00 = value.M00;
			result.M01 = value.M10;
			result.M02 = value.M20;
			result.M03 = value.M30;
			result.M10 = value.M01;
			result.M11 = value.M11;
			result.M12 = value.M21;
			result.M13 = value.M31;
			result.M20 = value.M02;
			result.M21 = value.M12;
			result.M22 = value.M22;
			result.M23 = value.M32;
			result.M30 = value.M03;
			result.M31 = value.M13;
			result.M32 = value.M23;
			result.M33 = value.M33;
		}


		/// <summary>
		/// Calculates the inverse of the specified matrix.
		/// </summary>
		public Matrix Invert() {
			float b0 = (M20 * M31) - (M21 * M30);
			float b1 = (M20 * M32) - (M22 * M30);
			float b2 = (M23 * M30) - (M20 * M33);
			float b3 = (M21 * M32) - (M22 * M31);
			float b4 = (M23 * M31) - (M21 * M33);
			float b5 = (M22 * M33) - (M23 * M32);

			float d11 = M11 * b5 + M12 * b4 + M13 * b3;
			float d12 = M10 * b5 + M12 * b2 + M13 * b1;
			float d13 = M10 * -b4 + M11 * b2 + M13 * b0;
			float d14 = M10 * b3 + M11 * -b1 + M12 * b0;

			float det = M00 * d11 - M01 * d12 + M02 * d13 - M03 * d14;
			if (Math.Abs(det) == 0.0f) {
				return Matrix.Zero;
			}

			det = 1f / det;

			float a0 = (M00 * M11) - (M01 * M10);
			float a1 = (M00 * M12) - (M02 * M10);
			float a2 = (M03 * M10) - (M00 * M13);
			float a3 = (M01 * M12) - (M02 * M11);
			float a4 = (M03 * M11) - (M01 * M13);
			float a5 = (M02 * M13) - (M03 * M12);

			float d21 = M01 * b5 + M02 * b4 + M03 * b3;
			float d22 = M00 * b5 + M02 * b2 + M03 * b1;
			float d23 = M00 * -b4 + M01 * b2 + M03 * b0;
			float d24 = M00 * b3 + M01 * -b1 + M02 * b0;

			float d31 = M31 * a5 + M32 * a4 + M33 * a3;
			float d32 = M30 * a5 + M32 * a2 + M33 * a1;
			float d33 = M30 * -a4 + M31 * a2 + M33 * a0;
			float d34 = M30 * a3 + M31 * -a1 + M32 * a0;

			float d41 = M21 * a5 + M22 * a4 + M23 * a3;
			float d42 = M20 * a5 + M22 * a2 + M23 * a1;
			float d43 = M20 * -a4 + M21 * a2 + M23 * a0;
			float d44 = M20 * a3 + M21 * -a1 + M22 * a0;

			Matrix result = new Matrix();

			result.M00 = +d11 * det; result.M01 = -d21 * det; result.M02 = +d31 * det; result.M03 = -d41 * det;
			result.M10 = -d12 * det; result.M11 = +d22 * det; result.M12 = -d32 * det; result.M13 = +d42 * det;
			result.M20 = +d13 * det; result.M21 = -d23 * det; result.M22 = +d33 * det; result.M23 = -d43 * det;
			result.M30 = -d14 * det; result.M31 = +d24 * det; result.M32 = -d34 * det; result.M33 = +d44 * det;

			return result;
		}

		/// <summary>
		/// Orthogonalizes the specified matrix.
		/// </summary>
		/// <param name="value">The matrix to orthogonalize.</param>
		/// <param name="result">When the method completes, contains the orthogonalized matrix.</param>
		/// <remarks>
		/// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
		/// means that any given row in the matrix will be orthogonal to any other given row in the
		/// matrix.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public static void Orthogonalize(ref Matrix value, out Matrix result) {
			//Uses the modified Gram-Schmidt process.
			//q1 = m1
			//q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
			//q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
			//q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3

			//By separating the above algorithm into multiple lines, we actually increase accuracy.
			result = value;

			result.Row1 = result.Row1 - (Vector4.Dot(result.Row0, result.Row1) / Vector4.Dot(result.Row0, result.Row0)) * result.Row0;

			result.Row2 = result.Row2 - (Vector4.Dot(result.Row0, result.Row2) / Vector4.Dot(result.Row0, result.Row0)) * result.Row0;
			result.Row2 = result.Row2 - (Vector4.Dot(result.Row1, result.Row2) / Vector4.Dot(result.Row1, result.Row1)) * result.Row1;

			result.Row3 = result.Row3 - (Vector4.Dot(result.Row0, result.Row3) / Vector4.Dot(result.Row0, result.Row0)) * result.Row0;
			result.Row3 = result.Row3 - (Vector4.Dot(result.Row1, result.Row3) / Vector4.Dot(result.Row1, result.Row1)) * result.Row1;
			result.Row3 = result.Row3 - (Vector4.Dot(result.Row2, result.Row3) / Vector4.Dot(result.Row2, result.Row2)) * result.Row2;
		}

		/// <summary>
		/// Orthogonalizes the specified matrix.
		/// </summary>
		/// <param name="value">The matrix to orthogonalize.</param>
		/// <returns>The orthogonalized matrix.</returns>
		/// <remarks>
		/// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
		/// means that any given row in the matrix will be orthogonal to any other given row in the
		/// matrix.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public static Matrix Orthogonalize(Matrix value) {
			Matrix result;
			Orthogonalize(ref value, out result);
			return result;
		}

		/// <summary>
		/// Orthonormalizes the specified matrix.
		/// </summary>
		/// <param name="value">The matrix to orthonormalize.</param>
		/// <param name="result">When the method completes, contains the orthonormalized matrix.</param>
		/// <remarks>
		/// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
		/// other and making all rows and columns of unit length. This means that any given row will
		/// be orthogonal to any other given row and any given column will be orthogonal to any other
		/// given column. Any given row will not be orthogonal to any given column. Every row and every
		/// column will be of unit length.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public static void Orthonormalize(ref Matrix value, out Matrix result) {
			//Uses the modified Gram-Schmidt process.
			//Because we are making unit vectors, we can optimize the math for orthonormalization
			//and simplify the projection operation to remove the division.
			//q1 = m1 / |m1|
			//q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
			//q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
			//q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|

			//By separating the above algorithm into multiple lines, we actually increase accuracy.
			result = value;

			result.Row0 = result.Row0.Normalize();

			result.Row1 = result.Row1 - Vector4.Dot(result.Row0, result.Row1) * result.Row0;
			result.Row1 = result.Row1.Normalize();

			result.Row2 = result.Row2 - Vector4.Dot(result.Row0, result.Row2) * result.Row0;
			result.Row2 = result.Row2 - Vector4.Dot(result.Row1, result.Row2) * result.Row1;
			result.Row2 = result.Row2.Normalize();

			result.Row3 = result.Row3 - Vector4.Dot(result.Row0, result.Row3) * result.Row0;
			result.Row3 = result.Row3 - Vector4.Dot(result.Row1, result.Row3) * result.Row1;
			result.Row3 = result.Row3 - Vector4.Dot(result.Row2, result.Row3) * result.Row2;
			result.Row3 = result.Row3.Normalize();
		}

		/// <summary>
		/// Orthonormalizes the specified matrix.
		/// </summary>
		/// <param name="value">The matrix to orthonormalize.</param>
		/// <returns>The orthonormalized matrix.</returns>
		/// <remarks>
		/// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
		/// other and making all rows and columns of unit length. This means that any given row will
		/// be orthogonal to any other given row and any given column will be orthogonal to any other
		/// given column. Any given row will not be orthogonal to any given column. Every row and every
		/// column will be of unit length.</para>
		/// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
		/// tends to be numerically unstable. The numeric stability decreases according to the rows
		/// so that the first row is the most stable and the last row is the least stable.</para>
		/// <para>This operation is performed on the rows of the matrix rather than the columns.
		/// If you wish for this operation to be performed on the columns, first transpose the
		/// input and than transpose the output.</para>
		/// </remarks>
		public static Matrix Orthonormalize(Matrix value) {
			Matrix result;
			Orthonormalize(ref value, out result);
			return result;
		}

		/// <summary>
		/// Brings the matrix into upper triangular form using elementary row operations.
		/// </summary>
		/// <param name="value">The matrix to put into upper triangular form.</param>
		/// <param name="result">When the method completes, contains the upper triangular matrix.</param>
		/// <remarks>
		/// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
		/// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
		/// of linear equations, than this often means that either no solution exists or an infinite
		/// number of solutions exist.
		/// </remarks>
		public static void UpperTriangularForm(ref Matrix value, out Matrix result) {
			//Adapted from the row echelon code.
			result = value;
			int lead = 0;
			int rowcount = 4;
			int columncount = 4;

			for (int r = 0; r < rowcount; ++r) {
				if (columncount <= lead)
					return;

				int i = r;

				while (MathUtil.IsZero(result[i, lead])) {
					i++;

					if (i == rowcount) {
						i = r;
						lead++;

						if (lead == columncount)
							return;
					}
				}

				if (i != r) {
					result.ExchangeRows(i, r);
				}

				float multiplier = 1f / result[r, lead];

				for (; i < rowcount; ++i) {
					if (i != r) {
						result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
						result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
						result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
						result[i, 3] -= result[r, 3] * multiplier * result[i, lead];
					}
				}

				lead++;
			}
		}

		/// <summary>
		/// Brings the matrix into upper triangular form using elementary row operations.
		/// </summary>
		/// <param name="value">The matrix to put into upper triangular form.</param>
		/// <returns>The upper triangular matrix.</returns>
		/// <remarks>
		/// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
		/// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
		/// of linear equations, than this often means that either no solution exists or an infinite
		/// number of solutions exist.
		/// </remarks>
		public static Matrix UpperTriangularForm(Matrix value) {
			Matrix result;
			UpperTriangularForm(ref value, out result);
			return result;
		}

		/// <summary>
		/// Brings the matrix into lower triangular form using elementary row operations.
		/// </summary>
		/// <param name="value">The matrix to put into lower triangular form.</param>
		/// <param name="result">When the method completes, contains the lower triangular matrix.</param>
		/// <remarks>
		/// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
		/// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
		/// of linear equations, than this often means that either no solution exists or an infinite
		/// number of solutions exist.
		/// </remarks>
		public static void LowerTriangularForm(ref Matrix value, out Matrix result) {
			//Adapted from the row echelon code.
			Matrix temp = value;
			result = temp.Transpose();


			int lead = 0;
			int rowcount = 4;
			int columncount = 4;

			for (int r = 0; r < rowcount; ++r) {
				if (columncount <= lead)
					return;

				int i = r;

				while (MathUtil.IsZero(result[i, lead])) {
					i++;

					if (i == rowcount) {
						i = r;
						lead++;

						if (lead == columncount)
							return;
					}
				}

				if (i != r) {
					result.ExchangeRows(i, r);
				}

				float multiplier = 1f / result[r, lead];

				for (; i < rowcount; ++i) {
					if (i != r) {
						result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
						result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
						result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
						result[i, 3] -= result[r, 3] * multiplier * result[i, lead];
					}
				}

				lead++;
			}

			result = result.Transpose();
		}

		/// <summary>
		/// Brings the matrix into lower triangular form using elementary row operations.
		/// </summary>
		/// <param name="value">The matrix to put into lower triangular form.</param>
		/// <returns>The lower triangular matrix.</returns>
		/// <remarks>
		/// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
		/// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
		/// of linear equations, than this often means that either no solution exists or an infinite
		/// number of solutions exist.
		/// </remarks>
		public static Matrix LowerTriangularForm(Matrix value) {
			Matrix result;
			LowerTriangularForm(ref value, out result);
			return result;
		}

		/// <summary>
		/// Brings the matrix into row echelon form using elementary row operations;
		/// </summary>
		/// <param name="value">The matrix to put into row echelon form.</param>
		/// <param name="result">When the method completes, contains the row echelon form of the matrix.</param>
		public static void RowEchelonForm(ref Matrix value, out Matrix result) {
			//Source: Wikipedia pseudo code
			//Reference: http://en.wikipedia.org/wiki/Row_echelon_form#Pseudocode

			result = value;
			int lead = 0;
			int rowcount = 4;
			int columncount = 4;

			for (int r = 0; r < rowcount; ++r) {
				if (columncount <= lead)
					return;

				int i = r;

				while (MathUtil.IsZero(result[i, lead])) {
					i++;

					if (i == rowcount) {
						i = r;
						lead++;

						if (lead == columncount)
							return;
					}
				}

				if (i != r) {
					result.ExchangeRows(i, r);
				}

				float multiplier = 1f / result[r, lead];
				result[r, 0] *= multiplier;
				result[r, 1] *= multiplier;
				result[r, 2] *= multiplier;
				result[r, 3] *= multiplier;

				for (; i < rowcount; ++i) {
					if (i != r) {
						result[i, 0] -= result[r, 0] * result[i, lead];
						result[i, 1] -= result[r, 1] * result[i, lead];
						result[i, 2] -= result[r, 2] * result[i, lead];
						result[i, 3] -= result[r, 3] * result[i, lead];
					}
				}

				lead++;
			}
		}

		/// <summary>
		/// Brings the matrix into row echelon form using elementary row operations;
		/// </summary>
		/// <param name="value">The matrix to put into row echelon form.</param>
		/// <returns>When the method completes, contains the row echelon form of the matrix.</returns>
		public static Matrix RowEchelonForm(Matrix value) {
			Matrix result;
			RowEchelonForm(ref value, out result);
			return result;
		}

		/// <summary>
		/// Brings the matrix into reduced row echelon form using elementary row operations.
		/// </summary>
		/// <param name="value">The matrix to put into reduced row echelon form.</param>
		/// <param name="augment">The fifth column of the matrix.</param>
		/// <param name="result">When the method completes, contains the resultant matrix after the operation.</param>
		/// <param name="augmentResult">When the method completes, contains the resultant fifth column of the matrix.</param>
		/// <remarks>
		/// <para>The fifth column is often called the augmented part of the matrix. This is because the fifth
		/// column is really just an extension of the matrix so that there is a place to put all of the
		/// non-zero components after the operation is complete.</para>
		/// <para>Often times the resultant matrix will the identity matrix or a matrix similar to the identity
		/// matrix. Sometimes, however, that is not possible and numbers other than zero and one may appear.</para>
		/// <para>This method can be used to solve systems of linear equations. Upon completion of this method,
		/// the <paramref name="augmentResult"/> will contain the solution for the system. It is up to the user
		/// to analyze both the input and the result to determine if a solution really exists.</para>
		/// </remarks>
		public static void ReducedRowEchelonForm(ref Matrix value, ref Vector4 augment, out Matrix result, out Vector4 augmentResult) {
			//Source: http://rosettacode.org
			//Reference: http://rosettacode.org/wiki/Reduced_row_echelon_form

			float[,] matrix = new float[4, 5];

			matrix[0, 0] = value[0, 0];
			matrix[0, 1] = value[0, 1];
			matrix[0, 2] = value[0, 2];
			matrix[0, 3] = value[0, 3];
			matrix[0, 4] = augment[0];

			matrix[1, 0] = value[1, 0];
			matrix[1, 1] = value[1, 1];
			matrix[1, 2] = value[1, 2];
			matrix[1, 3] = value[1, 3];
			matrix[1, 4] = augment[1];

			matrix[2, 0] = value[2, 0];
			matrix[2, 1] = value[2, 1];
			matrix[2, 2] = value[2, 2];
			matrix[2, 3] = value[2, 3];
			matrix[2, 4] = augment[2];

			matrix[3, 0] = value[3, 0];
			matrix[3, 1] = value[3, 1];
			matrix[3, 2] = value[3, 2];
			matrix[3, 3] = value[3, 3];
			matrix[3, 4] = augment[3];

			int lead = 0;
			int rowcount = 4;
			int columncount = 5;

			for (int r = 0; r < rowcount; r++) {
				if (columncount <= lead)
					break;

				int i = r;

				while (matrix[i, lead] == 0) {
					i++;

					if (i == rowcount) {
						i = r;
						lead++;

						if (columncount == lead)
							break;
					}
				}

				for (int j = 0; j < columncount; j++) {
					float temp = matrix[r, j];
					matrix[r, j] = matrix[i, j];
					matrix[i, j] = temp;
				}

				float div = matrix[r, lead];

				for (int j = 0; j < columncount; j++) {
					matrix[r, j] /= div;
				}

				for (int j = 0; j < rowcount; j++) {
					if (j != r) {
						float sub = matrix[j, lead];
						for (int k = 0; k < columncount; k++) matrix[j, k] -= (sub * matrix[r, k]);
					}
				}

				lead++;
			}

			result.M00 = matrix[0, 0];
			result.M01 = matrix[0, 1];
			result.M02 = matrix[0, 2];
			result.M03 = matrix[0, 3];

			result.M10 = matrix[1, 0];
			result.M11 = matrix[1, 1];
			result.M12 = matrix[1, 2];
			result.M13 = matrix[1, 3];

			result.M20 = matrix[2, 0];
			result.M21 = matrix[2, 1];
			result.M22 = matrix[2, 2];
			result.M23 = matrix[2, 3];

			result.M30 = matrix[3, 0];
			result.M31 = matrix[3, 1];
			result.M32 = matrix[3, 2];
			result.M33 = matrix[3, 3];

			augmentResult.x = matrix[0, 4];
			augmentResult.y = matrix[1, 4];
			augmentResult.z = matrix[2, 4];
			augmentResult.w = matrix[3, 4];
		}

		/// <summary>
		/// Creates a left-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUpVector">The up vector of the camera.</param>
		/// <param name="cameraForwardVector">The forward vector of the camera.</param>
		/// <param name="result">When the method completes, contains the created billboard matrix.</param>
		public static void BillboardLH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Matrix result) {
			Vector3 crossed;
			Vector3 final;
			Vector3 difference = cameraPosition - objectPosition;

			float lengthSq = difference.SqrLength();
			if (MathUtil.IsZero(lengthSq))
				difference = -cameraForwardVector;
			else
				difference *= (float)(1.0 / Math.Sqrt(lengthSq));

			crossed = Vector3.Cross(cameraUpVector, difference);
			crossed.Normalize();
			final = Vector3.Cross(difference, crossed);

			result.M00 = crossed.x;
			result.M01 = crossed.y;
			result.M02 = crossed.z;
			result.M03 = 0.0f;
			result.M10 = final.x;
			result.M11 = final.y;
			result.M12 = final.z;
			result.M13 = 0.0f;
			result.M20 = difference.x;
			result.M21 = difference.y;
			result.M22 = difference.z;
			result.M23 = 0.0f;
			result.M30 = objectPosition.x;
			result.M31 = objectPosition.y;
			result.M32 = objectPosition.z;
			result.M33 = 1.0f;
		}

		/// <summary>
		/// Creates a left-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUpVector">The up vector of the camera.</param>
		/// <param name="cameraForwardVector">The forward vector of the camera.</param>
		/// <returns>The created billboard matrix.</returns>
		public static Matrix BillboardLH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector) {
			Matrix result;
			BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUpVector">The up vector of the camera.</param>
		/// <param name="cameraForwardVector">The forward vector of the camera.</param>
		/// <param name="result">When the method completes, contains the created billboard matrix.</param>
		public static void BillboardRH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Matrix result) {
			Vector3 crossed;
			Vector3 final;
			Vector3 difference = objectPosition - cameraPosition;

			float lengthSq = difference.SqrLength();
			if (MathUtil.IsZero(lengthSq))
				difference = -cameraForwardVector;
			else
				difference *= (float)(1.0 / Math.Sqrt(lengthSq));

			crossed = Vector3.Cross(cameraUpVector, difference);
			crossed.Normalize();
			final = Vector3.Cross(difference, crossed);

			result.M00 = crossed.x;
			result.M01 = crossed.y;
			result.M02 = crossed.z;
			result.M03 = 0.0f;
			result.M10 = final.x;
			result.M11 = final.y;
			result.M12 = final.z;
			result.M13 = 0.0f;
			result.M20 = difference.x;
			result.M21 = difference.y;
			result.M22 = difference.z;
			result.M23 = 0.0f;
			result.M30 = objectPosition.x;
			result.M31 = objectPosition.y;
			result.M32 = objectPosition.z;
			result.M33 = 1.0f;
		}

		/// <summary>
		/// Creates a right-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUpVector">The up vector of the camera.</param>
		/// <param name="cameraForwardVector">The forward vector of the camera.</param>
		/// <returns>The created billboard matrix.</returns>
		public static Matrix BillboardRH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector) {
			Matrix result;
			BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, look-at matrix.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at matrix.</param>
		public static void LookAtLH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Matrix result) {
			Vector3 xaxis, yaxis, zaxis;
			zaxis = (target - eye).Normalize();
			xaxis = Vector3.Cross(up, zaxis).Normalize();
			yaxis = Vector3.Cross(zaxis, xaxis);

			result = Matrix.Identity;
			result.M00 = xaxis.x; result.M10 = xaxis.y; result.M20 = xaxis.z;
			result.M01 = yaxis.x; result.M11 = yaxis.y; result.M21 = yaxis.z;
			result.M02 = zaxis.x; result.M12 = zaxis.y; result.M22 = zaxis.z;

			result.M30 = Vector3.Dot(xaxis, eye);
			result.M31 = Vector3.Dot(yaxis, eye);
			result.M32 = Vector3.Dot(zaxis, eye);

			result.M30 = -result.M30;
			result.M31 = -result.M31;
			result.M32 = -result.M32;
		}

		/// <summary>
		/// Creates a left-handed, look-at matrix.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at matrix.</returns>
		public static Matrix LookAtLH(Vector3 eye, Vector3 target, Vector3 up) {
			Matrix result;
			LookAtLH(ref eye, ref target, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, look-at matrix.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at matrix.</param>
		public static void LookAtRH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Matrix result) {
			Vector3 xaxis, yaxis, zaxis;
			zaxis = (eye - target).Normalize();
			xaxis = Vector3.Cross(up, zaxis).Normalize();
			yaxis = Vector3.Cross(zaxis, xaxis);

			result = Matrix.Identity;
			result.M00 = xaxis.x; result.M10 = xaxis.y; result.M20 = xaxis.z;
			result.M01 = yaxis.x; result.M11 = yaxis.y; result.M21 = yaxis.z;
			result.M02 = zaxis.x; result.M12 = zaxis.y; result.M22 = zaxis.z;

			result.M30 = Vector3.Dot(xaxis, eye);
			result.M31 = Vector3.Dot(yaxis, eye);
			result.M32 = Vector3.Dot(zaxis, eye);

			result.M30 = -result.M30;
			result.M31 = -result.M31;
			result.M32 = -result.M32;
		}

		/// <summary>
		/// Creates a right-handed, look-at matrix.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at matrix.</returns>
		public static Matrix LookAtRH(Vector3 eye, Vector3 target, Vector3 up) {
			Matrix result;
			LookAtRH(ref eye, ref target, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, orthographic projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void OrthoLH(float width, float height, float znear, float zfar, out Matrix result) {
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;

			OrthoOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
		}

		/// <summary>
		/// Creates a left-handed, orthographic projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix OrthoLH(float width, float height, float znear, float zfar) {
			Matrix result;
			OrthoLH(width, height, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, orthographic projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void OrthoRH(float width, float height, float znear, float zfar, out Matrix result) {
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;

			OrthoOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
		}

		/// <summary>
		/// Creates a right-handed, orthographic projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix OrthoRH(float width, float height, float znear, float zfar) {
			Matrix result;
			OrthoRH(width, height, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, customized orthographic projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void OrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result) {
			float zRange = 1.0f / (zfar - znear);

			result = Matrix.Identity;
			result.M00 = 2.0f / (right - left);
			result.M11 = 2.0f / (top - bottom);
			result.M22 = zRange;
			result.M30 = (left + right) / (left - right);
			result.M31 = (top + bottom) / (bottom - top);
			result.M32 = -znear * zRange;
		}

		/// <summary>
		/// Creates a left-handed, customized orthographic projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix OrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar) {
			Matrix result;
			OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, customized orthographic projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void OrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result) {
			OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
			result.M22 *= -1.0f;
		}

		/// <summary>
		/// Creates a right-handed, customized orthographic projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix OrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar) {
			Matrix result;
			OrthoOffCenterRH(left, right, bottom, top, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, perspective projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveLH(float width, float height, float znear, float zfar, out Matrix result) {
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;

			PerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
		}

		/// <summary>
		/// Creates a left-handed, perspective projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveLH(float width, float height, float znear, float zfar) {
			Matrix result;
			PerspectiveLH(width, height, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, perspective projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveRH(float width, float height, float znear, float zfar, out Matrix result) {
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;

			PerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
		}

		/// <summary>
		/// Creates a right-handed, perspective projection matrix.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveRH(float width, float height, float znear, float zfar) {
			Matrix result;
			PerspectiveRH(width, height, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, perspective projection matrix based on a field of view.
		/// </summary>
		/// <param name="fov">Field of view in the y direction, in radians.</param>
		/// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveFovLH(float fov, float aspect, float znear, float zfar, out Matrix result) {
			float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
			float q = zfar / (zfar - znear);

			result = new Matrix();
			result.M00 = yScale / aspect;
			result.M11 = yScale;
			result.M22 = q;
			result.M23 = 1.0f;
			result.M32 = -q * znear;
		}

		/// <summary>
		/// Creates a left-handed, perspective projection matrix based on a field of view.
		/// </summary>
		/// <param name="fov">Field of view in the y direction, in radians.</param>
		/// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveFovLH(float fov, float aspect, float znear, float zfar) {
			Matrix result;
			PerspectiveFovLH(fov, aspect, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, perspective projection matrix based on a field of view.
		/// </summary>
		/// <param name="fov">Field of view in the y direction, in radians.</param>
		/// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveFovRH(float fov, float aspect, float znear, float zfar, out Matrix result) {
			float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
			float q = zfar / (znear - zfar);

			result = new Matrix();
			result.M00 = yScale / aspect;
			result.M11 = yScale;
			result.M22 = q;
			result.M23 = -1.0f;
			result.M32 = q * znear;
		}

		/// <summary>
		/// Creates a right-handed, perspective projection matrix based on a field of view.
		/// </summary>
		/// <param name="fov">Field of view in the y direction, in radians.</param>
		/// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveFovRH(float fov, float aspect, float znear, float zfar) {
			Matrix result;
			PerspectiveFovRH(fov, aspect, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, customized perspective projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result) {
			float zRange = zfar / (zfar - znear);

			result = new Matrix();
			result.M00 = 2.0f * znear / (right - left);
			result.M11 = 2.0f * znear / (top - bottom);
			result.M20 = (left + right) / (left - right);
			result.M21 = (top + bottom) / (bottom - top);
			result.M22 = zRange;
			result.M23 = 1.0f;
			result.M32 = -znear * zRange;
		}

		/// <summary>
		/// Creates a left-handed, customized perspective projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar) {
			Matrix result;
			PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, customized perspective projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <param name="result">When the method completes, contains the created projection matrix.</param>
		public static void PerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result) {
			PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
			result.M20 *= -1.0f;
			result.M21 *= -1.0f;
			result.M22 *= -1.0f;
			result.M23 *= -1.0f;
		}

		/// <summary>
		/// Creates a right-handed, customized perspective projection matrix.
		/// </summary>
		/// <param name="left">Minimum x-value of the viewing volume.</param>
		/// <param name="right">Maximum x-value of the viewing volume.</param>
		/// <param name="bottom">Minimum y-value of the viewing volume.</param>
		/// <param name="top">Maximum y-value of the viewing volume.</param>
		/// <param name="znear">Minimum z-value of the viewing volume.</param>
		/// <param name="zfar">Maximum z-value of the viewing volume.</param>
		/// <returns>The created projection matrix.</returns>
		public static Matrix PerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar) {
			Matrix result;
			PerspectiveOffCenterRH(left, right, bottom, top, znear, zfar, out result);
			return result;
		}


		/// <summary>
		/// Creates a matrix that scales along the x-axis, y-axis, and y-axis.
		/// </summary>
		/// <param name="scale">Scaling factor for all three axes.</param>
		/// <param name="result">When the method completes, contains the created scaling matrix.</param>
		public static void Scaling(ref Vector3 scale, out Matrix result) {
			Scaling(scale.x, scale.y, scale.z, out result);
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis, y-axis, and y-axis.
		/// </summary>
		/// <param name="scale">Scaling factor for all three axes.</param>
		/// <returns>The created scaling matrix.</returns>
		public static Matrix Scaling(Vector3 scale) {
			Matrix result;
			Scaling(ref scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis, y-axis, and y-axis.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		/// <param name="z">Scaling factor that is applied along the z-axis.</param>
		/// <param name="result">When the method completes, contains the created scaling matrix.</param>
		public static void Scaling(float x, float y, float z, out Matrix result) {
			result = Matrix.Identity;
			result.M00 = x;
			result.M11 = y;
			result.M22 = z;
		}

		/// <summary>
		/// Creates a matrix that scales along the x-axis, y-axis, and y-axis.
		/// </summary>
		/// <param name="x">Scaling factor that is applied along the x-axis.</param>
		/// <param name="y">Scaling factor that is applied along the y-axis.</param>
		/// <param name="z">Scaling factor that is applied along the z-axis.</param>
		/// <returns>The created scaling matrix.</returns>
		public static Matrix Scaling(float x, float y, float z) {
			Matrix result;
			Scaling(x, y, z, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that uniformly scales along all three axis.
		/// </summary>
		/// <param name="scale">The uniform scale that is applied along all axis.</param>
		/// <param name="result">When the method completes, contains the created scaling matrix.</param>
		public static void Scaling(float scale, out Matrix result) {
			result = Matrix.Identity;
			result.M00 = result.M11 = result.M22 = scale;
		}

		/// <summary>
		/// Creates a matrix that uniformly scales along all three axis.
		/// </summary>
		/// <param name="scale">The uniform scale that is applied along all axis.</param>
		/// <returns>The created scaling matrix.</returns>
		public static Matrix Scaling(float scale) {
			Matrix result;
			Scaling(scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <param name="result">When the method completes, contains the created rotation matrix.</param>
		public static void RotationX(float angle, out Matrix result) {
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);

			result = Matrix.Identity;
			result.M11 = cos;
			result.M12 = sin;
			result.M21 = -sin;
			result.M22 = cos;
		}

		/// <summary>
		/// Creates a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <returns>The created rotation matrix.</returns>
		public static Matrix RotationX(float angle) {
			Matrix result;
			RotationX(angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <param name="result">When the method completes, contains the created rotation matrix.</param>
		public static void RotationY(float angle, out Matrix result) {
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);

			result = Matrix.Identity;
			result.M00 = cos;
			result.M02 = -sin;
			result.M20 = sin;
			result.M22 = cos;
		}

		/// <summary>
		/// Creates a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <returns>The created rotation matrix.</returns>
		public static Matrix RotationY(float angle) {
			Matrix result;
			RotationY(angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <param name="result">When the method completes, contains the created rotation matrix.</param>
		public static void RotationZ(float angle, out Matrix result) {
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);

			result = Matrix.Identity;
			result.M00 = cos;
			result.M01 = sin;
			result.M10 = -sin;
			result.M11 = cos;
		}

		/// <summary>
		/// Creates a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <returns>The created rotation matrix.</returns>
		public static Matrix RotationZ(float angle) {
			Matrix result;
			RotationZ(angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around an arbitrary axis.
		/// </summary>
		/// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <param name="result">When the method completes, contains the created rotation matrix.</param>
		public static void RotationAxis(ref Vector3 axis, float angle, out Matrix result) {
			float x = axis.x;
			float y = axis.y;
			float z = axis.z;
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);
			float xx = x * x;
			float yy = y * y;
			float zz = z * z;
			float xy = x * y;
			float xz = x * z;
			float yz = y * z;

			result = Matrix.Identity;
			result.M00 = xx + (cos * (1.0f - xx));
			result.M01 = (xy - (cos * xy)) + (sin * z);
			result.M02 = (xz - (cos * xz)) - (sin * y);
			result.M10 = (xy - (cos * xy)) - (sin * z);
			result.M11 = yy + (cos * (1.0f - yy));
			result.M12 = (yz - (cos * yz)) + (sin * x);
			result.M20 = (xz - (cos * xz)) + (sin * y);
			result.M21 = (yz - (cos * yz)) - (sin * x);
			result.M22 = zz + (cos * (1.0f - zz));
		}

		/// <summary>
		/// Creates a matrix that rotates around an arbitrary axis.
		/// </summary>
		/// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
		/// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
		/// <returns>The created rotation matrix.</returns>
		public static Matrix RotationAxis(Vector3 axis, float angle) {
			Matrix result;
			RotationAxis(ref axis, angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="rotation">The quaternion to use to build the matrix.</param>
		/// <param name="result">The created rotation matrix.</param>
		public static Matrix RotationQuaternion(Quaternion rotation) {
			
			float xx = rotation.x * rotation.x;
			float yy = rotation.y * rotation.y;
			float zz = rotation.z * rotation.z;
			float xy = rotation.x * rotation.y;
			float zw = rotation.z * rotation.w;
			float zx = rotation.z * rotation.x;
			float yw = rotation.y * rotation.w;
			float yz = rotation.y * rotation.z;
			float xw = rotation.x * rotation.w;

			Matrix result = Matrix.Identity;
			result.M00 = 1.0f - (2.0f * (yy + zz));
			result.M01 = 2.0f * (xy + zw);
			result.M02 = 2.0f * (zx - yw);
			result.M10 = 2.0f * (xy - zw);
			result.M11 = 1.0f - (2.0f * (zz + xx));
			result.M12 = 2.0f * (yz + xw);
			result.M20 = 2.0f * (zx + yw);
			result.M21 = 2.0f * (yz - xw);
			result.M22 = 1.0f - (2.0f * (yy + xx));
			return result;

		}

		/// <summary>
		/// Creates a rotation matrix with a specified yaw, pitch, and roll.
		/// </summary>
		/// <param name="yaw">Yaw around the y-axis, in radians.</param>
		/// <param name="pitch">Pitch around the x-axis, in radians.</param>
		/// <param name="roll">Roll around the z-axis, in radians.</param>
		/// <param name="result">When the method completes, contains the created rotation matrix.</param>
		public static Matrix RotationYawPitchRoll(float yaw, float pitch, float roll) {
			Quaternion quaternion = new Quaternion();
			Quaternion.RotationYawPitchRoll(yaw, pitch, roll, out quaternion);
			return RotationQuaternion(quaternion);
		}

		/// <summary>
		/// Creates a translation matrix using the specified offsets.
		/// </summary>
		/// <param name="x">X-coordinate offset.</param>
		/// <param name="y">Y-coordinate offset.</param>
		/// <param name="z">Z-coordinate offset.</param>
		public static Matrix Translation(float x, float y, float z) {
			Matrix result = Matrix.Identity;
			result.M30 = x;
			result.M31 = y;
			result.M32 = z;
			return result;
		}

		/// <summary>
		/// Creates a translation matrix using the specified offsets.
		/// </summary>
		/// <param name="translation">Offset.</param>
		public static Matrix Translation(Vector3 offset) {
			return Translation(offset.x, offset.y, offset.z);
		}

		/// <summary>
		/// Creates a skew/shear matrix by means of a translation vector, a rotation vector, and a rotation angle.
		/// shearing is performed in the direction of translation vector, where translation vector and rotation vector define the shearing plane.
		/// The effect is such that the skewed rotation vector has the specified angle with rotation itself.
		/// </summary>
		/// <param name="angle">The rotation angle.</param>
		/// <param name="rotationVec">The rotation vector</param>
		/// <param name="transVec">The translation vector</param>
		/// <param name="matrix">Contains the created skew/shear matrix. </param>
		public static void Skew(float angle, ref Vector3 rotationVec, ref Vector3 transVec, out Matrix matrix) {
			//http://elckerlyc.ewi.utwente.nl/browser/Elckerlyc/Hmi/HmiMath/src/hmi/math/Mat3f.java
			float MINIMAL_SKEW_ANGLE = 0.000001f;

			Vector3 e0 = rotationVec;
			Vector3 e1 = transVec.Normalize();

			float rv1;
			rv1 = Vector3.Dot(rotationVec, e1);
			e0 += rv1 * e1;
			float rv0;
			rv0 = Vector3.Dot(rotationVec, e0);
			float cosa = (float)Math.Cos(angle);
			float sina = (float)Math.Sin(angle);
			float rr0 = rv0 * cosa - rv1 * sina;
			float rr1 = rv0 * sina + rv1 * cosa;

			if (rr0 < MINIMAL_SKEW_ANGLE)
				throw new ArgumentException("illegal skew angle");

			float d = (rr1 / rr0) - (rv1 / rv0);

			matrix = Matrix.Identity;
			matrix.M00 = d * e1[0] * e0[0] + 1.0f;
			matrix.M01 = d * e1[0] * e0[1];
			matrix.M02 = d * e1[0] * e0[2];
			matrix.M10 = d * e1[1] * e0[0];
			matrix.M11 = d * e1[1] * e0[1] + 1.0f;
			matrix.M12 = d * e1[1] * e0[2];
			matrix.M20 = d * e1[2] * e0[0];
			matrix.M21 = d * e1[2] * e0[1];
			matrix.M22 = d * e1[2] * e0[2] + 1.0f;
		}

		/// <summary>
		/// Creates a 3D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
		public static void AffineTransformation(float scaling, ref Quaternion rotation, ref Vector3 translation, out Matrix result) {
			result = Scaling(scaling) * RotationQuaternion(rotation) * Translation(translation);
		}

		/// <summary>
		/// Creates a 3D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created affine transformation matrix.</returns>
		public static Matrix AffineTransformation(float scaling, Quaternion rotation, Vector3 translation) {
			Matrix result;
			AffineTransformation(scaling, ref rotation, ref translation, out result);
			return result;
		}

		/// <summary>
		/// Creates a 3D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
		public static void AffineTransformation(float scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix result) {
			result = Scaling(scaling) * Translation(-rotationCenter) * RotationQuaternion(rotation) *
				Translation(rotationCenter) * Translation(translation);
		}

		/// <summary>
		/// Creates a 3D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created affine transformation matrix.</returns>
		public static Matrix AffineTransformation(float scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation) {
			Matrix result;
			AffineTransformation(scaling, ref rotationCenter, ref rotation, ref translation, out result);
			return result;
		}

		/// <summary>
		/// Creates a 2D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
		public static void AffineTransformation2D(float scaling, float rotation, ref Vector2 translation, out Matrix result) {
			result = Scaling(scaling, scaling, 1.0f) * RotationZ(rotation) * Translation((Vector3)translation);
		}

		/// <summary>
		/// Creates a 2D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created affine transformation matrix.</returns>
		public static Matrix AffineTransformation2D(float scaling, float rotation, Vector2 translation) {
			Matrix result;
			AffineTransformation2D(scaling, rotation, ref translation, out result);
			return result;
		}

		/// <summary>
		/// Creates a 2D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
		public static void AffineTransformation2D(float scaling, ref Vector2 rotationCenter, float rotation, ref Vector2 translation, out Matrix result) {
			result = Scaling(scaling, scaling, 1.0f) * Translation((Vector3)(-rotationCenter)) * RotationZ(rotation) *
				Translation((Vector3)rotationCenter) * Translation((Vector3)translation);
		}

		/// <summary>
		/// Creates a 2D affine transformation matrix.
		/// </summary>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created affine transformation matrix.</returns>
		public static Matrix AffineTransformation2D(float scaling, Vector2 rotationCenter, float rotation, Vector2 translation) {
			Matrix result;
			AffineTransformation2D(scaling, ref rotationCenter, rotation, ref translation, out result);
			return result;
		}

		/// <summary>
		/// Creates a transformation matrix.
		/// </summary>
		/// <param name="scalingCenter">Center point of the scaling operation.</param>
		/// <param name="scalingRotation">Scaling rotation amount.</param>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created transformation matrix.</param>
		public static void Transformation(ref Vector3 scalingCenter, ref Quaternion scalingRotation, ref Vector3 scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix result) {
			Matrix sr = RotationQuaternion(scalingRotation);

			result = Translation(-scalingCenter) * Transpose(sr) * Scaling(scaling) * sr * Translation(scalingCenter) * Translation(-rotationCenter) *
				RotationQuaternion(rotation) * Translation(rotationCenter) * Translation(translation);
		}

		/// <summary>
		/// Creates a transformation matrix.
		/// </summary>
		/// <param name="scalingCenter">Center point of the scaling operation.</param>
		/// <param name="scalingRotation">Scaling rotation amount.</param>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created transformation matrix.</returns>
		public static Matrix Transformation(Vector3 translation, Quaternion rotation, Vector3 scale) {

			Matrix scaleMatrix = Scaling(scale);
			Matrix rotationMatrix = RotationQuaternion(rotation);
			Matrix translationMatrix = Translation(translation);

			return scaleMatrix * rotationMatrix * translationMatrix;

		}

		/// <summary>
		/// Creates a 2D transformation matrix.
		/// </summary>
		/// <param name="scalingCenter">Center point of the scaling operation.</param>
		/// <param name="scalingRotation">Scaling rotation amount.</param>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <param name="result">When the method completes, contains the created transformation matrix.</param>
		public static void Transformation2D(ref Vector2 scalingCenter, float scalingRotation, ref Vector2 scaling, ref Vector2 rotationCenter, float rotation, ref Vector2 translation, out Matrix result) {
			result = Translation((Vector3)(-scalingCenter)) * RotationZ(-scalingRotation) * Scaling((Vector3)scaling) * RotationZ(scalingRotation) * Translation((Vector3)scalingCenter) *
				Translation((Vector3)(-rotationCenter)) * RotationZ(rotation) * Translation((Vector3)rotationCenter) * Translation((Vector3)translation);

			result.M22 = 1f;
			result.M33 = 1f;
		}

		/// <summary>
		/// Creates a 2D transformation matrix.
		/// </summary>
		/// <param name="scalingCenter">Center point of the scaling operation.</param>
		/// <param name="scalingRotation">Scaling rotation amount.</param>
		/// <param name="scaling">Scaling factor.</param>
		/// <param name="rotationCenter">The center of the rotation.</param>
		/// <param name="rotation">The rotation of the transformation.</param>
		/// <param name="translation">The translation factor of the transformation.</param>
		/// <returns>The created transformation matrix.</returns>
		public static Matrix Transformation2D(Vector2 scalingCenter, float scalingRotation, Vector2 scaling, Vector2 rotationCenter, float rotation, Vector2 translation) {
			Matrix result;
			Transformation2D(ref scalingCenter, scalingRotation, ref scaling, ref rotationCenter, rotation, ref translation, out result);
			return result;
		}

		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="left">The first matrix to add.</param>
		/// <param name="right">The second matrix to add.</param>
		/// <returns>The sum of the two matrices.</returns>
		public static Matrix operator +(Matrix left, Matrix right) {
			Matrix result;
			Add(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Assert a matrix (return it unchanged).
		/// </summary>
		/// <param name="value">The matrix to assert (unchanged).</param>
		/// <returns>The asserted (unchanged) matrix.</returns>
		public static Matrix operator +(Matrix value) {
			return value;
		}

		/// <summary>
		/// Subtracts two matrices.
		/// </summary>
		/// <param name="left">The first matrix to subtract.</param>
		/// <param name="right">The second matrix to subtract.</param>
		/// <returns>The difference between the two matrices.</returns>
		public static Matrix operator -(Matrix left, Matrix right) {
			Matrix result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="value">The matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix operator -(Matrix value) {
			Matrix result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="right">The matrix to scale.</param>
		/// <param name="left">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix operator *(float left, Matrix right) {
			Matrix result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Vector4 operator *(Matrix left, Vector4 right) {
			Vector4 outVec;

			Matrix leftT = left.Transpose();

			outVec.x = leftT.M00 * right.x + leftT.M01 * right.y + leftT.M02 * right.z + leftT.M03 * right.w;
			outVec.y = leftT.M10 * right.x + leftT.M11 * right.y + leftT.M12 * right.z + leftT.M13 * right.w;
			outVec.z = leftT.M20 * right.x + leftT.M21 * right.y + leftT.M22 * right.z + leftT.M23 * right.w;
			outVec.w = leftT.M30 * right.x + leftT.M31 * right.y + leftT.M32 * right.z + leftT.M33 * right.w;
			return outVec;
		}
		public static Vector3 operator *(Matrix left, Vector3 right) {
			return (Vector3)(left * (Vector4)right);
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix operator *(Matrix left, float right) {
			Matrix result;
			Multiply(ref left, right, out result);
			return result;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="left">The first matrix to multiply.</param>
		/// <param name="right">The second matrix to multiply.</param>
		/// <returns>The product of the two matrices.</returns>
		public static Matrix operator *(Matrix left, Matrix right) {
			Matrix result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Scales a matrix by a given value.
		/// </summary>
		/// <param name="left">The matrix to scale.</param>
		/// <param name="right">The amount by which to scale.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix operator /(Matrix left, float right) {
			Matrix result;
			Divide(ref left, right, out result);
			return result;
		}

		/// <summary>
		/// Divides two matrices.
		/// </summary>
		/// <param name="left">The first matrix to divide.</param>
		/// <param name="right">The second matrix to divide.</param>
		/// <returns>The quotient of the two matrices.</returns>
		public static Matrix operator /(Matrix left, Matrix right) {
			Matrix result;
			Divide(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Tests for equality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator ==(Matrix left, Matrix right) {
			return left.Equals(ref right);
		}

		/// <summary>
		/// Tests for inequality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator !=(Matrix left, Matrix right) {
			return !left.Equals(ref right);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() {
			return string.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
				M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23, M30, M31, M32, M33);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(string format) {
			if (format == null)
				return ToString();

			return string.Format(format, CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
				M00.ToString(format, CultureInfo.CurrentCulture), M01.ToString(format, CultureInfo.CurrentCulture), M02.ToString(format, CultureInfo.CurrentCulture), M03.ToString(format, CultureInfo.CurrentCulture),
				M10.ToString(format, CultureInfo.CurrentCulture), M11.ToString(format, CultureInfo.CurrentCulture), M12.ToString(format, CultureInfo.CurrentCulture), M13.ToString(format, CultureInfo.CurrentCulture),
				M20.ToString(format, CultureInfo.CurrentCulture), M21.ToString(format, CultureInfo.CurrentCulture), M22.ToString(format, CultureInfo.CurrentCulture), M23.ToString(format, CultureInfo.CurrentCulture),
				M30.ToString(format, CultureInfo.CurrentCulture), M31.ToString(format, CultureInfo.CurrentCulture), M32.ToString(format, CultureInfo.CurrentCulture), M33.ToString(format, CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(IFormatProvider formatProvider) {
			return string.Format(formatProvider, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
				M00.ToString(formatProvider), M01.ToString(formatProvider), M02.ToString(formatProvider), M03.ToString(formatProvider),
				M10.ToString(formatProvider), M11.ToString(formatProvider), M12.ToString(formatProvider), M13.ToString(formatProvider),
				M20.ToString(formatProvider), M21.ToString(formatProvider), M22.ToString(formatProvider), M23.ToString(formatProvider),
				M30.ToString(formatProvider), M31.ToString(formatProvider), M32.ToString(formatProvider), M33.ToString(formatProvider));
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(string format, IFormatProvider formatProvider) {
			if (format == null)
				return ToString(formatProvider);

			return string.Format(formatProvider, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
				M00.ToString(format, formatProvider), M01.ToString(format, formatProvider), M02.ToString(format, formatProvider), M03.ToString(format, formatProvider),
				M10.ToString(format, formatProvider), M11.ToString(format, formatProvider), M12.ToString(format, formatProvider), M13.ToString(format, formatProvider),
				M20.ToString(format, formatProvider), M21.ToString(format, formatProvider), M22.ToString(format, formatProvider), M23.ToString(format, formatProvider),
				M30.ToString(format, formatProvider), M31.ToString(format, formatProvider), M32.ToString(format, formatProvider), M33.ToString(format, formatProvider));
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			unchecked {
				var hashCode = M00.GetHashCode();
				hashCode = (hashCode * 397) ^ M01.GetHashCode();
				hashCode = (hashCode * 397) ^ M02.GetHashCode();
				hashCode = (hashCode * 397) ^ M03.GetHashCode();
				hashCode = (hashCode * 397) ^ M10.GetHashCode();
				hashCode = (hashCode * 397) ^ M11.GetHashCode();
				hashCode = (hashCode * 397) ^ M12.GetHashCode();
				hashCode = (hashCode * 397) ^ M13.GetHashCode();
				hashCode = (hashCode * 397) ^ M20.GetHashCode();
				hashCode = (hashCode * 397) ^ M21.GetHashCode();
				hashCode = (hashCode * 397) ^ M22.GetHashCode();
				hashCode = (hashCode * 397) ^ M23.GetHashCode();
				hashCode = (hashCode * 397) ^ M30.GetHashCode();
				hashCode = (hashCode * 397) ^ M31.GetHashCode();
				hashCode = (hashCode * 397) ^ M32.GetHashCode();
				hashCode = (hashCode * 397) ^ M33.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="Matrix"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Matrix"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Matrix"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(ref Matrix other) {
			return (MathUtil.NearEqual(other.M00, M00) &&
				MathUtil.NearEqual(other.M01, M01) &&
				MathUtil.NearEqual(other.M02, M02) &&
				MathUtil.NearEqual(other.M03, M03) &&
				MathUtil.NearEqual(other.M10, M10) &&
				MathUtil.NearEqual(other.M11, M11) &&
				MathUtil.NearEqual(other.M12, M12) &&
				MathUtil.NearEqual(other.M13, M13) &&
				MathUtil.NearEqual(other.M20, M20) &&
				MathUtil.NearEqual(other.M21, M21) &&
				MathUtil.NearEqual(other.M22, M22) &&
				MathUtil.NearEqual(other.M23, M23) &&
				MathUtil.NearEqual(other.M30, M30) &&
				MathUtil.NearEqual(other.M31, M31) &&
				MathUtil.NearEqual(other.M32, M32) &&
				MathUtil.NearEqual(other.M33, M33));
		}

		/// <summary>
		/// Determines whether the specified <see cref="Matrix"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Matrix"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Matrix"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public bool Equals(Matrix other) {
			return Equals(ref other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object value) {
			if (!(value is Matrix))
				return false;

			var strongValue = (Matrix)value;
			return Equals(ref strongValue);
		}

	}
}
