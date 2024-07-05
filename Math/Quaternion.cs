using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX;


namespace ArcticFoxEngine {

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
	/// <summary>
	/// Represents a four dimensional mathematical quaternion.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Quaternion : IEquatable<Quaternion>, IFormattable
	{
		/// <summary>
		/// The size of the <see cref="Quaternion"/> type, in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Utilities.SizeOf<Quaternion>();

		/// <summary>
		/// A <see cref="Quaternion"/> with all of its components set to zero.
		/// </summary>
		public static readonly Quaternion Zero = new Quaternion();

		/// <summary>
		/// A <see cref="Quaternion"/> with all of its components set to one.
		/// </summary>
		public static readonly Quaternion One = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

		/// <summary>
		/// The identity <see cref="Quaternion"/> (0, 0, 0, 1).
		/// </summary>
		public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

		/// <summary>
		/// The X component of the quaternion.
		/// </summary>
		public float x;

		/// <summary>
		/// The Y component of the quaternion.
		/// </summary>
		public float y;

		/// <summary>
		/// The Z component of the quaternion.
		/// </summary>
		public float z;

		/// <summary>
		/// The W component of the quaternion.
		/// </summary>
		public float w;

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="value">The value that will be assigned to all components.</param>
		public Quaternion(float value)
		{
			x = value;
			y = value;
			z = value;
			w = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="value">A vector containing the values with which to initialize the components.</param>
		public Quaternion(SharpDX.Vector4 value)
		{
			x = value.X;
			y = value.Y;
			z = value.Z;
			w = value.W;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="value">A vector containing the values with which to initialize the X, Y, and Z components.</param>
		/// <param name="w">Initial value for the W component of the quaternion.</param>
		public Quaternion(SharpDX.Vector3 value, float w)
		{
			x = value.X;
			y = value.Y;
			z = value.Z;
			this.w = w;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
		/// <param name="z">Initial value for the Z component of the quaternion.</param>
		/// <param name="w">Initial value for the W component of the quaternion.</param>
		public Quaternion(SharpDX.Vector2 value, float z, float w)
		{
			x = value.X;
			y = value.Y;
			this.z = z;
			this.w = w;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="x">Initial value for the X component of the quaternion.</param>
		/// <param name="y">Initial value for the Y component of the quaternion.</param>
		/// <param name="z">Initial value for the Z component of the quaternion.</param>
		/// <param name="w">Initial value for the W component of the quaternion.</param>
		public Quaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quaternion"/> struct.
		/// </summary>
		/// <param name="values">The values to assign to the X, Y, Z, and W components of the quaternion. This must be an array with four elements.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
		public Quaternion(float[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (values.Length != 4)
				throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for Quaternion.");

			x = values[0];
			y = values[1];
			z = values[2];
			w = values[3];
		}

		/// <summary>
		/// Gets a value indicating whether this instance is equivalent to the identity quaternion.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is an identity quaternion; otherwise, <c>false</c>.
		/// </value>
		public bool IsIdentity
		{
			get { return Equals(Identity); }
		}

		/// <summary>
		/// Gets a value indicting whether this instance is normalized.
		/// </summary>
		public bool IsNormalized
		{
			get { return MathUtil.IsOne(x * x + y * y + z * z + w * w); }
		}

		/// <summary>
		/// Gets the angle of the quaternion.
		/// </summary>
		/// <value>The quaternion's angle.</value>
		public float Angle
		{
			get
			{
				float length = x * x + y * y + z * z;
				if (MathUtil.IsZero(length))
					return 0.0f;

				return (float)(2.0 * Math.Acos(MathUtil.Clamp(w, -1f, 1f)));
			}
		}

		/// <summary>
		/// Gets the axis components of the quaternion.
		/// </summary>
		/// <value>The axis components of the quaternion.</value>
		public SharpDX.Vector3 Axis
		{
			get
			{
				float length = x * x + y * y + z * z;
				if (MathUtil.IsZero(length))
					return SharpDX.Vector3.UnitX;

				float inv = 1.0f / (float)Math.Sqrt(length);
				return new SharpDX.Vector3(x * inv, y * inv, z * inv);
			}
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return x;
					case 1: return y;
					case 2: return z;
					case 3: return w;
				}

				throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
			}

			set
			{
				switch (index)
				{
					case 0: x = value; break;
					case 1: y = value; break;
					case 2: z = value; break;
					case 3: w = value; break;
					default: throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
				}
			}
		}

		/// <summary>
		/// Conjugates the quaternion.
		/// </summary>
		public void Conjugate()
		{
			x = -x;
			y = -y;
			z = -z;
		}

		/// <summary>
		/// Conjugates and renormalizes the quaternion.
		/// </summary>
		public void Invert()
		{
			float lengthSq = LengthSquared();
			if (!MathUtil.IsZero(lengthSq))
			{
				lengthSq = 1.0f / lengthSq;

				x = -x * lengthSq;
				y = -y * lengthSq;
				z = -z * lengthSq;
				w = w * lengthSq;
			}
		}

		/// <summary>
		/// Calculates the length of the quaternion.
		/// </summary>
		/// <returns>The length of the quaternion.</returns>
		/// <remarks>
		/// <see cref="LengthSquared"/> may be preferred when only the relative length is needed
		/// and speed is of the essence.
		/// </remarks>
		public float Length()
		{
			return (float)Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		/// <summary>
		/// Calculates the squared length of the quaternion.
		/// </summary>
		/// <returns>The squared length of the quaternion.</returns>
		/// <remarks>
		/// This method may be preferred to <see cref="Length"/> when only a relative length is needed
		/// and speed is of the essence.
		/// </remarks>
		public float LengthSquared()
		{
			return x * x + y * y + z * z + w * w;
		}

		/// <summary>
		/// Converts the quaternion into a unit quaternion.
		/// </summary>
		public void Normalize()
		{
			float length = Length();
			if (!MathUtil.IsZero(length))
			{
				float inverse = 1.0f / length;
				x *= inverse;
				y *= inverse;
				z *= inverse;
				w *= inverse;
			}
		}

		/// <summary>
		/// Creates an array containing the elements of the quaternion.
		/// </summary>
		/// <returns>A four-element array containing the components of the quaternion.</returns>
		public float[] ToArray()
		{
			return new float[] { x, y, z, w };
		}

		/// <summary>
		/// Adds two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to add.</param>
		/// <param name="right">The second quaternion to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two quaternions.</param>
		public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result.x = left.x + right.x;
			result.y = left.y + right.y;
			result.z = left.z + right.z;
			result.w = left.w + right.w;
		}

		/// <summary>
		/// Adds two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to add.</param>
		/// <param name="right">The second quaternion to add.</param>
		/// <returns>The sum of the two quaternions.</returns>
		public static Quaternion Add(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Add(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Subtracts two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to subtract.</param>
		/// <param name="right">The second quaternion to subtract.</param>
		/// <param name="result">When the method completes, contains the difference of the two quaternions.</param>
		public static void Subtract(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result.x = left.x - right.x;
			result.y = left.y - right.y;
			result.z = left.z - right.z;
			result.w = left.w - right.w;
		}

		/// <summary>
		/// Subtracts two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to subtract.</param>
		/// <param name="right">The second quaternion to subtract.</param>
		/// <returns>The difference of the two quaternions.</returns>
		public static Quaternion Subtract(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Scales a quaternion by the given value.
		/// </summary>
		/// <param name="value">The quaternion to scale.</param>
		/// <param name="scale">The amount by which to scale the quaternion.</param>
		/// <param name="result">When the method completes, contains the scaled quaternion.</param>
		public static void Multiply(ref Quaternion value, float scale, out Quaternion result)
		{
			result.x = value.x * scale;
			result.y = value.y * scale;
			result.z = value.z * scale;
			result.w = value.w * scale;
		}

		/// <summary>
		/// Scales a quaternion by the given value.
		/// </summary>
		/// <param name="value">The quaternion to scale.</param>
		/// <param name="scale">The amount by which to scale the quaternion.</param>
		/// <returns>The scaled quaternion.</returns>
		public static Quaternion Multiply(Quaternion value, float scale)
		{
			Quaternion result;
			Multiply(ref value, scale, out result);
			return result;
		}

		/// <summary>
		/// Multiplies a quaternion by another.
		/// </summary>
		/// <param name="left">The first quaternion to multiply.</param>
		/// <param name="right">The second quaternion to multiply.</param>
		/// <param name="result">When the method completes, contains the multiplied quaternion.</param>
		public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			float lx = left.x;
			float ly = left.y;
			float lz = left.z;
			float lw = left.w;
			float rx = right.x;
			float ry = right.y;
			float rz = right.z;
			float rw = right.w;
			float a = ly * rz - lz * ry;
			float b = lz * rx - lx * rz;
			float c = lx * ry - ly * rx;
			float d = lx * rx + ly * ry + lz * rz;
			result.x = lx * rw + rx * lw + a;
			result.y = ly * rw + ry * lw + b;
			result.z = lz * rw + rz * lw + c;
			result.w = lw * rw - d;
		}

		/// <summary>
		/// Multiplies a quaternion by another.
		/// </summary>
		/// <param name="left">The first quaternion to multiply.</param>
		/// <param name="right">The second quaternion to multiply.</param>
		/// <returns>The multiplied quaternion.</returns>
		public static Quaternion Multiply(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given quaternion.
		/// </summary>
		/// <param name="value">The quaternion to negate.</param>
		/// <param name="result">When the method completes, contains a quaternion facing in the opposite direction.</param>
		public static void Negate(ref Quaternion value, out Quaternion result)
		{
			result.x = -value.x;
			result.y = -value.y;
			result.z = -value.z;
			result.w = -value.w;
		}

		/// <summary>
		/// Reverses the direction of a given quaternion.
		/// </summary>
		/// <param name="value">The quaternion to negate.</param>
		/// <returns>A quaternion facing in the opposite direction.</returns>
		public static Quaternion Negate(Quaternion value)
		{
			Quaternion result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
		/// </summary>
		/// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
		/// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
		/// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
		/// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
		/// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
		/// <param name="result">When the method completes, contains a new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
		public static void Barycentric(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, float amount1, float amount2, out Quaternion result)
		{
			Quaternion start, end;
			Slerp(ref value1, ref value2, amount1 + amount2, out start);
			Slerp(ref value1, ref value3, amount1 + amount2, out end);
			Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
		}

		/// <summary>
		/// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
		/// </summary>
		/// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
		/// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
		/// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
		/// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
		/// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
		/// <returns>A new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</returns>
		public static Quaternion Barycentric(Quaternion value1, Quaternion value2, Quaternion value3, float amount1, float amount2)
		{
			Quaternion result;
			Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out result);
			return result;
		}

		/// <summary>
		/// Conjugates a quaternion.
		/// </summary>
		/// <param name="value">The quaternion to conjugate.</param>
		/// <param name="result">When the method completes, contains the conjugated quaternion.</param>
		public static void Conjugate(ref Quaternion value, out Quaternion result)
		{
			result.x = -value.x;
			result.y = -value.y;
			result.z = -value.z;
			result.w = value.w;
		}

		/// <summary>
		/// Conjugates a quaternion.
		/// </summary>
		/// <param name="value">The quaternion to conjugate.</param>
		/// <returns>The conjugated quaternion.</returns>
		public static Quaternion Conjugate(Quaternion value)
		{
			Quaternion result;
			Conjugate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Calculates the dot product of two quaternions.
		/// </summary>
		/// <param name="left">First source quaternion.</param>
		/// <param name="right">Second source quaternion.</param>
		/// <param name="result">When the method completes, contains the dot product of the two quaternions.</param>
		public static void Dot(ref Quaternion left, ref Quaternion right, out float result)
		{
			result = left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
		}

		/// <summary>
		/// Calculates the dot product of two quaternions.
		/// </summary>
		/// <param name="left">First source quaternion.</param>
		/// <param name="right">Second source quaternion.</param>
		/// <returns>The dot product of the two quaternions.</returns>
		public static float Dot(Quaternion left, Quaternion right)
		{
			return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
		}

		/// <summary>
		/// Exponentiates a quaternion.
		/// </summary>
		/// <param name="value">The quaternion to exponentiate.</param>
		/// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
		public static void Exponential(ref Quaternion value, out Quaternion result)
		{
			float angle = (float)Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
			float sin = (float)Math.Sin(angle);

			if (!MathUtil.IsZero(sin))
			{
				float coeff = sin / angle;
				result.x = coeff * value.x;
				result.y = coeff * value.y;
				result.z = coeff * value.z;
			}
			else
			{
				result = value;
			}

			result.w = (float)Math.Cos(angle);
		}

		/// <summary>
		/// Exponentiates a quaternion.
		/// </summary>
		/// <param name="value">The quaternion to exponentiate.</param>
		/// <returns>The exponentiated quaternion.</returns>
		public static Quaternion Exponential(Quaternion value)
		{
			Quaternion result;
			Exponential(ref value, out result);
			return result;
		}

		/// <summary>
		/// Conjugates and renormalizes the quaternion.
		/// </summary>
		/// <param name="value">The quaternion to conjugate and renormalize.</param>
		/// <param name="result">When the method completes, contains the conjugated and renormalized quaternion.</param>
		public static void Invert(ref Quaternion value, out Quaternion result)
		{
			result = value;
			result.Invert();
		}

		/// <summary>
		/// Conjugates and renormalizes the quaternion.
		/// </summary>
		/// <param name="value">The quaternion to conjugate and renormalize.</param>
		/// <returns>The conjugated and renormalized quaternion.</returns>
		public static Quaternion Invert(Quaternion value)
		{
			Quaternion result;
			Invert(ref value, out result);
			return result;
		}

		/// <summary>
		/// Performs a linear interpolation between two quaternions.
		/// </summary>
		/// <param name="start">Start quaternion.</param>
		/// <param name="end">End quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <param name="result">When the method completes, contains the linear interpolation of the two quaternions.</param>
		/// <remarks>
		/// This method performs the linear interpolation based on the following formula.
		/// <code>start + (end - start) * amount</code>
		/// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
		/// </remarks>
		public static void Lerp(ref Quaternion start, ref Quaternion end, float amount, out Quaternion result)
		{
			float inverse = 1.0f - amount;

			if (Dot(start, end) >= 0.0f)
			{
				result.x = inverse * start.x + amount * end.x;
				result.y = inverse * start.y + amount * end.y;
				result.z = inverse * start.z + amount * end.z;
				result.w = inverse * start.w + amount * end.w;
			}
			else
			{
				result.x = inverse * start.x - amount * end.x;
				result.y = inverse * start.y - amount * end.y;
				result.z = inverse * start.z - amount * end.z;
				result.w = inverse * start.w - amount * end.w;
			}

			result.Normalize();
		}

		/// <summary>
		/// Performs a linear interpolation between two quaternion.
		/// </summary>
		/// <param name="start">Start quaternion.</param>
		/// <param name="end">End quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <returns>The linear interpolation of the two quaternions.</returns>
		/// <remarks>
		/// This method performs the linear interpolation based on the following formula.
		/// <code>start + (end - start) * amount</code>
		/// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
		/// </remarks>
		public static Quaternion Lerp(Quaternion start, Quaternion end, float amount)
		{
			Quaternion result;
			Lerp(ref start, ref end, amount, out result);
			return result;
		}

		/// <summary>
		/// Calculates the natural logarithm of the specified quaternion.
		/// </summary>
		/// <param name="value">The quaternion whose logarithm will be calculated.</param>
		/// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
		public static void Logarithm(ref Quaternion value, out Quaternion result)
		{
			if (Math.Abs(value.w) < 1.0)
			{
				float angle = (float)Math.Acos(value.w);
				float sin = (float)Math.Sin(angle);

				if (!MathUtil.IsZero(sin))
				{
					float coeff = angle / sin;
					result.x = value.x * coeff;
					result.y = value.y * coeff;
					result.z = value.z * coeff;
				}
				else
				{
					result = value;
				}
			}
			else
			{
				result = value;
			}

			result.w = 0.0f;
		}

		/// <summary>
		/// Calculates the natural logarithm of the specified quaternion.
		/// </summary>
		/// <param name="value">The quaternion whose logarithm will be calculated.</param>
		/// <returns>The natural logarithm of the quaternion.</returns>
		public static Quaternion Logarithm(Quaternion value)
		{
			Quaternion result;
			Logarithm(ref value, out result);
			return result;
		}

		/// <summary>
		/// Converts the quaternion into a unit quaternion.
		/// </summary>
		/// <param name="value">The quaternion to normalize.</param>
		/// <param name="result">When the method completes, contains the normalized quaternion.</param>
		public static void Normalize(ref Quaternion value, out Quaternion result)
		{
			Quaternion temp = value;
			result = temp;
			result.Normalize();
		}

		/// <summary>
		/// Converts the quaternion into a unit quaternion.
		/// </summary>
		/// <param name="value">The quaternion to normalize.</param>
		/// <returns>The normalized quaternion.</returns>
		public static Quaternion Normalize(Quaternion value)
		{
			value.Normalize();
			return value;
		}

		/// <summary>
		/// Creates a quaternion given a rotation and an axis.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <param name="result">When the method completes, contains the newly created quaternion.</param>
		public static void RotationAxis(ref SharpDX.Vector3 axis, float angle, out Quaternion result)
		{
			SharpDX.Vector3 normalized;
			SharpDX.Vector3.Normalize(ref axis, out normalized);

			float half = angle * 0.5f;
			float sin = (float)Math.Sin(half);
			float cos = (float)Math.Cos(half);

			result.x = normalized.X * sin;
			result.y = normalized.Y * sin;
			result.z = normalized.Z * sin;
			result.w = cos;
		}

		/// <summary>
		/// Creates a quaternion given a rotation and an axis.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle of rotation.</param>
		/// <returns>The newly created quaternion.</returns>
		public static Quaternion RotationAxis(SharpDX.Vector3 axis, float angle)
		{
			Quaternion result;
			RotationAxis(ref axis, angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a quaternion given a rotation matrix.
		/// </summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <param name="result">When the method completes, contains the newly created quaternion.</param>
		public static void RotationMatrix(ref Matrix matrix, out Quaternion result)
		{
			float sqrt;
			float half;
			float scale = matrix.M11 + matrix.M22 + matrix.M33;

			if (scale > 0.0f)
			{
				sqrt = (float)Math.Sqrt(scale + 1.0f);
				result.w = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				result.x = (matrix.M23 - matrix.M32) * sqrt;
				result.y = (matrix.M31 - matrix.M13) * sqrt;
				result.z = (matrix.M12 - matrix.M21) * sqrt;
			}
			else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
				half = 0.5f / sqrt;

				result.x = 0.5f * sqrt;
				result.y = (matrix.M12 + matrix.M21) * half;
				result.z = (matrix.M13 + matrix.M31) * half;
				result.w = (matrix.M23 - matrix.M32) * half;
			}
			else if (matrix.M22 > matrix.M33)
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
				half = 0.5f / sqrt;

				result.x = (matrix.M21 + matrix.M12) * half;
				result.y = 0.5f * sqrt;
				result.z = (matrix.M32 + matrix.M23) * half;
				result.w = (matrix.M31 - matrix.M13) * half;
			}
			else
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
				half = 0.5f / sqrt;

				result.x = (matrix.M31 + matrix.M13) * half;
				result.y = (matrix.M32 + matrix.M23) * half;
				result.z = 0.5f * sqrt;
				result.w = (matrix.M12 - matrix.M21) * half;
			}
		}

		/// <summary>
		/// Creates a quaternion given a rotation matrix.
		/// </summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <param name="result">When the method completes, contains the newly created quaternion.</param>
		public static void RotationMatrix(ref Matrix3x3 matrix, out Quaternion result)
		{
			float sqrt;
			float half;
			float scale = matrix.M11 + matrix.M22 + matrix.M33;

			if (scale > 0.0f)
			{
				sqrt = (float)Math.Sqrt(scale + 1.0f);
				result.w = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				result.x = (matrix.M23 - matrix.M32) * sqrt;
				result.y = (matrix.M31 - matrix.M13) * sqrt;
				result.z = (matrix.M12 - matrix.M21) * sqrt;
			}
			else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
				half = 0.5f / sqrt;

				result.x = 0.5f * sqrt;
				result.y = (matrix.M12 + matrix.M21) * half;
				result.z = (matrix.M13 + matrix.M31) * half;
				result.w = (matrix.M23 - matrix.M32) * half;
			}
			else if (matrix.M22 > matrix.M33)
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
				half = 0.5f / sqrt;

				result.x = (matrix.M21 + matrix.M12) * half;
				result.y = 0.5f * sqrt;
				result.z = (matrix.M32 + matrix.M23) * half;
				result.w = (matrix.M31 - matrix.M13) * half;
			}
			else
			{
				sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
				half = 0.5f / sqrt;

				result.x = (matrix.M31 + matrix.M13) * half;
				result.y = (matrix.M32 + matrix.M23) * half;
				result.z = 0.5f * sqrt;
				result.w = (matrix.M12 - matrix.M21) * half;
			}
		}

		/// <summary>
		/// Creates a left-handed, look-at quaternion.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at quaternion.</param>
		public static void LookAtLH(ref SharpDX.Vector3 eye, ref SharpDX.Vector3 target, ref SharpDX.Vector3 up, out Quaternion result)
		{
			Matrix3x3 matrix;
			Matrix3x3.LookAtLH(ref eye, ref target, ref up, out matrix);
			RotationMatrix(ref matrix, out result);
		}

		/// <summary>
		/// Creates a left-handed, look-at quaternion.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at quaternion.</returns>
		public static Quaternion LookAtLH(SharpDX.Vector3 eye, SharpDX.Vector3 target, SharpDX.Vector3 up)
		{
			Quaternion result;
			LookAtLH(ref eye, ref target, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed, look-at quaternion.
		/// </summary>
		/// <param name="forward">The camera's forward direction.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at quaternion.</param>
		public static void RotationLookAtLH(ref SharpDX.Vector3 forward, ref SharpDX.Vector3 up, out Quaternion result)
		{
			SharpDX.Vector3 eye = SharpDX.Vector3.Zero;
			LookAtLH(ref eye, ref forward, ref up, out result);
		}

		/// <summary>
		/// Creates a left-handed, look-at quaternion.
		/// </summary>
		/// <param name="forward">The camera's forward direction.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at quaternion.</returns>
		public static Quaternion RotationLookAtLH(SharpDX.Vector3 forward, SharpDX.Vector3 up)
		{
			Quaternion result;
			RotationLookAtLH(ref forward, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, look-at quaternion.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at quaternion.</param>
		public static void LookAtRH(ref SharpDX.Vector3 eye, ref SharpDX.Vector3 target, ref SharpDX.Vector3 up, out Quaternion result)
		{
			Matrix3x3 matrix;
			Matrix3x3.LookAtRH(ref eye, ref target, ref up, out matrix);
			RotationMatrix(ref matrix, out result);
		}

		/// <summary>
		/// Creates a right-handed, look-at quaternion.
		/// </summary>
		/// <param name="eye">The position of the viewer's eye.</param>
		/// <param name="target">The camera look-at target.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at quaternion.</returns>
		public static Quaternion LookAtRH(SharpDX.Vector3 eye, SharpDX.Vector3 target, SharpDX.Vector3 up)
		{
			Quaternion result;
			LookAtRH(ref eye, ref target, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed, look-at quaternion.
		/// </summary>
		/// <param name="forward">The camera's forward direction.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <param name="result">When the method completes, contains the created look-at quaternion.</param>
		public static void RotationLookAtRH(ref SharpDX.Vector3 forward, ref SharpDX.Vector3 up, out Quaternion result)
		{
			SharpDX.Vector3 eye = SharpDX.Vector3.Zero;
			LookAtRH(ref eye, ref forward, ref up, out result);
		}

		/// <summary>
		/// Creates a right-handed, look-at quaternion.
		/// </summary>
		/// <param name="forward">The camera's forward direction.</param>
		/// <param name="up">The camera's up vector.</param>
		/// <returns>The created look-at quaternion.</returns>
		public static Quaternion RotationLookAtRH(SharpDX.Vector3 forward, SharpDX.Vector3 up)
		{
			Quaternion result;
			RotationLookAtRH(ref forward, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a left-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUpSharpDX.Vector">The up vector of the camera.</param>
		/// <param name="cameraForwardSharpDX.Vector">The forward vector of the camera.</param>
		/// <param name="result">When the method completes, contains the created billboard quaternion.</param>
		public static void BillboardLH(ref SharpDX.Vector3 objectPosition, ref SharpDX.Vector3 cameraPosition, ref SharpDX.Vector3 cameraUpVector, ref SharpDX.Vector3 cameraForwardVector, out Quaternion result)
		{
			Matrix3x3 matrix;
			Matrix3x3.BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out matrix);
			RotationMatrix(ref matrix, out result);
		}

		/// <summary>
		/// Creates a left-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUp">The up vector of the camera.</param>
		/// <param name="cameraForward">The forward vector of the camera.</param>
		/// <returns>The created billboard quaternion.</returns>
		public static Quaternion BillboardLH(SharpDX.Vector3 objectPosition, SharpDX.Vector3 cameraPosition, SharpDX.Vector3 cameraUp, SharpDX.Vector3 cameraForward)
		{
			Quaternion result;
			BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUp, ref cameraForward, out result);
			return result;
		}

		/// <summary>
		/// Creates a right-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUp">The up vector of the camera.</param>
		/// <param name="cameraForward">The forward vector of the camera.</param>
		/// <param name="result">When the method completes, contains the created billboard quaternion.</param>
		public static void BillboardRH(ref SharpDX.Vector3 objectPosition, ref SharpDX.Vector3 cameraPosition, ref SharpDX.Vector3 cameraUp, ref SharpDX.Vector3 cameraForward, out Quaternion result)
		{
			Matrix3x3 matrix;
			Matrix3x3.BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUp, ref cameraForward, out matrix);
			RotationMatrix(ref matrix, out result);
		}

		/// <summary>
		/// Creates a right-handed spherical billboard that rotates around a specified object position.
		/// </summary>
		/// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraUp">The up vector of the camera.</param>
		/// <param name="cameraForward">The forward vector of the camera.</param>
		/// <returns>The created billboard quaternion.</returns>
		public static Quaternion BillboardRH(SharpDX.Vector3 objectPosition, SharpDX.Vector3 cameraPosition, SharpDX.Vector3 cameraUp, SharpDX.Vector3 cameraForward)
		{
			Quaternion result;
			BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUp, ref cameraForward, out result);
			return result;
		}

		/// <summary>
		/// Creates a quaternion given a rotation matrix.
		/// </summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <returns>The newly created quaternion.</returns>
		public static Quaternion RotationMatrix(Matrix matrix)
		{
			Quaternion result;
			RotationMatrix(ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Creates a quaternion given a yaw, pitch, and roll value.
		/// </summary>
		/// <param name="yaw">The yaw of rotation.</param>
		/// <param name="pitch">The pitch of rotation.</param>
		/// <param name="roll">The roll of rotation.</param>
		/// <param name="result">When the method completes, contains the newly created quaternion.</param>
		public static void RotationYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
		{
			float halfRoll = roll * 0.5f;
			float halfPitch = pitch * 0.5f;
			float halfYaw = yaw * 0.5f;

			float sinRoll = (float)Math.Sin(halfRoll);
			float cosRoll = (float)Math.Cos(halfRoll);
			float sinPitch = (float)Math.Sin(halfPitch);
			float cosPitch = (float)Math.Cos(halfPitch);
			float sinYaw = (float)Math.Sin(halfYaw);
			float cosYaw = (float)Math.Cos(halfYaw);

			result.x = cosYaw * sinPitch * cosRoll + sinYaw * cosPitch * sinRoll;
			result.y = sinYaw * cosPitch * cosRoll - cosYaw * sinPitch * sinRoll;
			result.z = cosYaw * cosPitch * sinRoll - sinYaw * sinPitch * cosRoll;
			result.w = cosYaw * cosPitch * cosRoll + sinYaw * sinPitch * sinRoll;
		}

		/// <summary>
		/// Creates a quaternion given a yaw, pitch, and roll value.
		/// </summary>
		/// <param name="yaw">The yaw of rotation.</param>
		/// <param name="pitch">The pitch of rotation.</param>
		/// <param name="roll">The roll of rotation.</param>
		/// <returns>The newly created quaternion.</returns>
		public static Quaternion RotationYawPitchRoll(float yaw, float pitch, float roll)
		{
			Quaternion result;
			RotationYawPitchRoll(yaw, pitch, roll, out result);
			return result;
		}

		/// <summary>
		/// Interpolates between two quaternions, using spherical linear interpolation.
		/// </summary>
		/// <param name="start">Start quaternion.</param>
		/// <param name="end">End quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <param name="result">When the method completes, contains the spherical linear interpolation of the two quaternions.</param>
		public static void Slerp(ref Quaternion start, ref Quaternion end, float amount, out Quaternion result)
		{
			float opposite;
			float inverse;
			float dot = Dot(start, end);

			if (Math.Abs(dot) > 1.0f - MathUtil.ZeroTolerance)
			{
				inverse = 1.0f - amount;
				opposite = amount * Math.Sign(dot);
			}
			else
			{
				float acos = (float)Math.Acos(Math.Abs(dot));
				float invSin = (float)(1.0 / Math.Sin(acos));

				inverse = (float)Math.Sin((1.0f - amount) * acos) * invSin;
				opposite = (float)Math.Sin(amount * acos) * invSin * Math.Sign(dot);
			}

			result.x = inverse * start.x + opposite * end.x;
			result.y = inverse * start.y + opposite * end.y;
			result.z = inverse * start.z + opposite * end.z;
			result.w = inverse * start.w + opposite * end.w;
		}

		/// <summary>
		/// Interpolates between two quaternions, using spherical linear interpolation.
		/// </summary>
		/// <param name="start">Start quaternion.</param>
		/// <param name="end">End quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
		/// <returns>The spherical linear interpolation of the two quaternions.</returns>
		public static Quaternion Slerp(Quaternion start, Quaternion end, float amount)
		{
			Quaternion result;
			Slerp(ref start, ref end, amount, out result);
			return result;
		}

		/// <summary>
		/// Interpolates between quaternions, using spherical quadrangle interpolation.
		/// </summary>
		/// <param name="value1">First source quaternion.</param>
		/// <param name="value2">Second source quaternion.</param>
		/// <param name="value3">Third source quaternion.</param>
		/// <param name="value4">Fourth source quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
		/// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
		public static void Squad(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, ref Quaternion value4, float amount, out Quaternion result)
		{
			Quaternion start, end;
			Slerp(ref value1, ref value4, amount, out start);
			Slerp(ref value2, ref value3, amount, out end);
			Slerp(ref start, ref end, 2.0f * amount * (1.0f - amount), out result);
		}

		/// <summary>
		/// Interpolates between quaternions, using spherical quadrangle interpolation.
		/// </summary>
		/// <param name="value1">First source quaternion.</param>
		/// <param name="value2">Second source quaternion.</param>
		/// <param name="value3">Third source quaternion.</param>
		/// <param name="value4">Fourth source quaternion.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
		/// <returns>The spherical quadrangle interpolation of the quaternions.</returns>
		public static Quaternion Squad(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4, float amount)
		{
			Quaternion result;
			Squad(ref value1, ref value2, ref value3, ref value4, amount, out result);
			return result;
		}

		/// <summary>
		/// Sets up control points for spherical quadrangle interpolation.
		/// </summary>
		/// <param name="value1">First source quaternion.</param>
		/// <param name="value2">Second source quaternion.</param>
		/// <param name="value3">Third source quaternion.</param>
		/// <param name="value4">Fourth source quaternion.</param>
		/// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
		public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
		{
			Quaternion q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
			Quaternion q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
			Quaternion q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
			Quaternion q1 = value2;

			Quaternion q1Exp, q2Exp;
			Exponential(ref q1, out q1Exp);
			Exponential(ref q2, out q2Exp);

			Quaternion[] results = new Quaternion[3];
			results[0] = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
			results[1] = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
			results[2] = q2;

			return results;
		}

		/// <summary>
		/// Adds two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to add.</param>
		/// <param name="right">The second quaternion to add.</param>
		/// <returns>The sum of the two quaternions.</returns>
		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Add(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Subtracts two quaternions.
		/// </summary>
		/// <param name="left">The first quaternion to subtract.</param>
		/// <param name="right">The second quaternion to subtract.</param>
		/// <returns>The difference of the two quaternions.</returns>
		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given quaternion.
		/// </summary>
		/// <param name="value">The quaternion to negate.</param>
		/// <returns>A quaternion facing in the opposite direction.</returns>
		public static Quaternion operator -(Quaternion value)
		{
			Quaternion result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Scales a quaternion by the given value.
		/// </summary>
		/// <param name="value">The quaternion to scale.</param>
		/// <param name="scale">The amount by which to scale the quaternion.</param>
		/// <returns>The scaled quaternion.</returns>
		public static Quaternion operator *(float scale, Quaternion value)
		{
			Quaternion result;
			Multiply(ref value, scale, out result);
			return result;
		}

		/// <summary>
		/// Scales a quaternion by the given value.
		/// </summary>
		/// <param name="value">The quaternion to scale.</param>
		/// <param name="scale">The amount by which to scale the quaternion.</param>
		/// <returns>The scaled quaternion.</returns>
		public static Quaternion operator *(Quaternion value, float scale)
		{
			Quaternion result;
			Multiply(ref value, scale, out result);
			return result;
		}

		/// <summary>
		/// Multiplies a quaternion by another.
		/// </summary>
		/// <param name="left">The first quaternion to multiply.</param>
		/// <param name="right">The second quaternion to multiply.</param>
		/// <returns>The multiplied quaternion.</returns>
		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Tests for equality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator ==(Quaternion left, Quaternion right)
		{
			return left.Equals(ref right);
		}

		/// <summary>
		/// Tests for inequality between two objects.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public static bool operator !=(Quaternion left, Quaternion right)
		{
			return !left.Equals(ref right);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", x, y, z, w);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public string ToString(string format)
		{
			if (format == null)
				return ToString();

			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", x.ToString(format, CultureInfo.CurrentCulture),
				y.ToString(format, CultureInfo.CurrentCulture), z.ToString(format, CultureInfo.CurrentCulture), w.ToString(format, CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", x, y, z, w);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
				return ToString(formatProvider);

			return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", x.ToString(format, formatProvider),
				y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x.GetHashCode();
				hashCode = hashCode * 397 ^ y.GetHashCode();
				hashCode = hashCode * 397 ^ z.GetHashCode();
				hashCode = hashCode * 397 ^ w.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="Quaternion"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Quaternion"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Quaternion"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public bool Equals(ref Quaternion other)
		{
			return MathUtil.NearEqual(other.x, x) && MathUtil.NearEqual(other.y, y) && MathUtil.NearEqual(other.z, z) && MathUtil.NearEqual(other.w, w);
		}

		/// <summary>
		/// Determines whether the specified <see cref="Quaternion"/> is equal to this instance.
		/// </summary>
		/// <param name="other">The <see cref="Quaternion"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Quaternion"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		[MethodImpl((MethodImplOptions)0x100)] // MethodImplOptions.AggressiveInlining
		public bool Equals(Quaternion other)
		{
			return Equals(ref other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to this instance.
		/// </summary>
		/// <param name="value">The <see cref="object"/> to compare with this instance.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object value)
		{
			if (!(value is Quaternion))
				return false;

			var strongValue = (Quaternion)value;
			return Equals(ref strongValue);
		}

		public static implicit operator SharpDX.Quaternion(Quaternion q)
		{
			return new SharpDX.Quaternion(q.x, q.y, q.z, q.w);
		}

	}
}