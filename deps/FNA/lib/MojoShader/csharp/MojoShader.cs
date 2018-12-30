#region License
/* MojoShader# - C# Wrapper for MojoShader
 *
 * Copyright (c) 2014-2017 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
#endregion

public static class MojoShader
{
	#region Native Library Name

	private const string nativeLibName = "mojoshader";

	#endregion

	#region UTF8 Marshaling

	private static byte[] UTF8_ToNative(string s)
	{
		if (s == null)
		{
			return null;
		}

		// Add a null terminator. That's kind of it... :/
		return System.Text.Encoding.UTF8.GetBytes(s + '\0');
	}

	private static unsafe string UTF8_ToManaged(IntPtr s)
	{
		if (s == IntPtr.Zero)
		{
			return null;
		}

		/* We get to do strlen ourselves! */
		byte* ptr = (byte*) s;
		while (*ptr != 0)
		{
			ptr++;
		}

		/* TODO: This #ifdef is only here because the equivalent
		 * .NET 2.0 constructor appears to be less efficient?
		 * Here's the pretty version, maybe steal this instead:
		 *
		string result = new string(
			(sbyte*) s, // Also, why sbyte???
			0,
			(int) (ptr - (byte*) s),
			System.Text.Encoding.UTF8
		);
		 * See the CoreCLR source for more info.
		 * -flibit
		 */
#if NETSTANDARD2_0
		/* Modern C# lets you just send the byte*, nice! */
		string result = System.Text.Encoding.UTF8.GetString(
			(byte*) s,
			(int) (ptr - (byte*) s)
		);
#else
		/* Old C# requires an extra memcpy, bleh! */
		int len = (int) (ptr - (byte*) s);
		char* chars = stackalloc char[len];
		int strLen = System.Text.Encoding.UTF8.GetChars((byte*) s, len, chars, len);
		string result = new string(chars, 0, strLen);
#endif
		return result;
	}

	#endregion

	#region Version Interface

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int MOJOSHADER_version();

	/* IntPtr refers to a statically allocated const char* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_changeset();

	#endregion

	#region Custom malloc/free Function Types

	/* data refers to a void* */
	public delegate IntPtr MOJOSHADER_malloc(int bytes, IntPtr data);

	/* ptr refers to a void, data to a void* */
	public delegate IntPtr MOJOSHADER_free(IntPtr ptr, IntPtr data);

	#endregion

	#region Parse Interface

	public enum MOJOSHADER_shaderType
	{
		MOJOSHADER_TYPE_UNKNOWN =	0,
		MOJOSHADER_TYPE_PIXEL =		(1 << 0),
		MOJOSHADER_TYPE_VERTEX =	(1 << 1),
		MOJOSHADER_TYPE_GEOMETRY =	(1 << 2),
		MOJOSHADER_TYPE_ANY =		-1 // 0xFFFFFFFF, ugh
	}

	public enum MOJOSHADER_attributeType
	{
		MOJOSHADER_ATTRIBUTE_UNKNOWN = -1,
		MOJOSHADER_ATTRIBUTE_BYTE,
		MOJOSHADER_ATTRIBUTE_UBYTE,
		MOJOSHADER_ATTRIBUTE_SHORT,
		MOJOSHADER_ATTRIBUTE_USHORT,
		MOJOSHADER_ATTRIBUTE_INT,
		MOJOSHADER_ATTRIBUTE_UINT,
		MOJOSHADER_ATTRIBUTE_FLOAT,
		MOJOSHADER_ATTRIBUTE_DOUBLE,
		MOJOSHADER_ATTRIBUTE_HALF_FLOAT
	}

	public enum MOJOSHADER_uniformType
	{
		MOJOSHADER_UNIFORM_UNKNOWN = -1,
		MOJOSHADER_UNIFORM_FLOAT,
		MOJOSHADER_UNIFORM_INT,
		MOJOSHADER_UNIFORM_BOOL
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_uniform
	{
		public MOJOSHADER_uniformType type;
		public int index;
		public int array_count;
		public int constant;
		public IntPtr name; // const char*
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct MOJOSHADER_constant
	{
		[FieldOffset(0)]
			public MOJOSHADER_uniformType type;
		[FieldOffset(4)]
			public int index;
		[FieldOffset(8)]
			public fixed float f[4];
		[FieldOffset(8)]
			public fixed int i[4];
		[FieldOffset(8)]
			public int b;
	}

	public enum MOJOSHADER_samplerType
	{
		MOJOSHADER_SAMPLER_UNKNOWN = -1,
		MOJOSHADER_SAMPLER_2D,
		MOJOSHADER_SAMPLER_CUBE,
		MOJOSHADER_SAMPLER_VOLUME
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_sampler
	{
		public MOJOSHADER_samplerType type;
		public int index;
		public IntPtr name; // const char*
		public int texbem;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_samplerMap
	{
		public int index;
		public MOJOSHADER_samplerType type;
	}

	public enum MOJOSHADER_usage
	{
		MOJOSHADER_USAGE_UNKNOWN = -1,
		MOJOSHADER_USAGE_POSITION,
		MOJOSHADER_USAGE_BLENDWEIGHT,
		MOJOSHADER_USAGE_BLENDINDICES,
		MOJOSHADER_USAGE_NORMAL,
		MOJOSHADER_USAGE_POINTSIZE,
		MOJOSHADER_USAGE_TEXCOORD,
		MOJOSHADER_USAGE_TANGENT,
		MOJOSHADER_USAGE_BINORMAL,
		MOJOSHADER_USAGE_TESSFACTOR,
		MOJOSHADER_USAGE_POSITIONT,
		MOJOSHADER_USAGE_COLOR,
		MOJOSHADER_USAGE_FOG,
		MOJOSHADER_USAGE_DEPTH,
		MOJOSHADER_USAGE_SAMPLE,
		MOJOSHADER_USAGE_TOTAL
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_attribute
	{
		public MOJOSHADER_usage usage;
		public int index;
		public IntPtr name; // const char*
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MOJOSHADER_swizzle
	{
		public MOJOSHADER_usage usage;
		public uint index;
		public fixed byte swizzles[4];
	}

	public enum MOJOSHADER_symbolRegisterSet
	{
		MOJOSHADER_SYMREGSET_BOOL = 0,
		MOJOSHADER_SYMREGSET_INT4,
		MOJOSHADER_SYMREGSET_FLOAT4,
		MOJOSHADER_SYMREGSET_SAMPLER,
		MOJOSHADER_SYMREGSET_TOTAL
	}

	public enum MOJOSHADER_symbolClass
	{
		MOJOSHADER_SYMCLASS_SCALAR = 0,
		MOJOSHADER_SYMCLASS_VECTOR,
		MOJOSHADER_SYMCLASS_MATRIX_ROWS,
		MOJOSHADER_SYMCLASS_MATRIX_COLUMNS,
		MOJOSHADER_SYMCLASS_OBJECT,
		MOJOSHADER_SYMCLASS_STRUCT,
		MOJOSHADER_SYMCLASS_TOTAL
	}

	public enum MOJOSHADER_symbolType
	{
		MOJOSHADER_SYMTYPE_VOID = 0,
		MOJOSHADER_SYMTYPE_BOOL,
		MOJOSHADER_SYMTYPE_INT,
		MOJOSHADER_SYMTYPE_FLOAT,
		MOJOSHADER_SYMTYPE_STRING,
		MOJOSHADER_SYMTYPE_TEXTURE,
		MOJOSHADER_SYMTYPE_TEXTURE1D,
		MOJOSHADER_SYMTYPE_TEXTURE2D,
		MOJOSHADER_SYMTYPE_TEXTURE3D,
		MOJOSHADER_SYMTYPE_TEXTURECUBE,
		MOJOSHADER_SYMTYPE_SAMPLER,
		MOJOSHADER_SYMTYPE_SAMPLER1D,
		MOJOSHADER_SYMTYPE_SAMPLER2D,
		MOJOSHADER_SYMTYPE_SAMPLER3D,
		MOJOSHADER_SYMTYPE_SAMPLERCUBE,
		MOJOSHADER_SYMTYPE_PIXELSHADER,
		MOJOSHADER_SYMTYPE_VERTEXSHADER,
		MOJOSHADER_SYMTYPE_PIXELFRAGMENT,
		MOJOSHADER_SYMTYPE_VERTEXFRAGMENT,
		MOJOSHADER_SYMTYPE_UNSUPPORTED,
		MOJOSHADER_SYMTYPE_TOTAL
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_symbolTypeInfo
	{
		public MOJOSHADER_symbolClass parameter_class;
		public MOJOSHADER_symbolType parameter_type;
		public uint rows;
		public uint columns;
		public uint elements;
		public uint member_count;
		public IntPtr members; // MOJOSHADER_symbolStructMember*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_symbolStructMember
	{
		public IntPtr name; //const char*
		public MOJOSHADER_symbolTypeInfo info;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_symbol
	{
		public IntPtr name; // const char*
		public MOJOSHADER_symbolRegisterSet register_set;
		public uint register_index;
		public uint register_count;
		public MOJOSHADER_symbolTypeInfo info;
	}

	public const int MOJOSHADER_POSITION_NONE =	-3;
	public const int MOJOSHADER_POSITION_BEFORE =	-2;
	public const int MOJOSHADER_POSITION_AFTER =	-1;
	
	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_error
	{
		public IntPtr error; // const char*
		public IntPtr filename; // const char*
		public int error_position;
	}

	public enum MOJOSHADER_preshaderOpcode
	{
		MOJOSHADER_PRESHADEROP_NOP,
		MOJOSHADER_PRESHADEROP_MOV,
		MOJOSHADER_PRESHADEROP_NEG,
		MOJOSHADER_PRESHADEROP_RCP,
		MOJOSHADER_PRESHADEROP_FRC,
		MOJOSHADER_PRESHADEROP_EXP,
		MOJOSHADER_PRESHADEROP_LOG,
		MOJOSHADER_PRESHADEROP_RSQ,
		MOJOSHADER_PRESHADEROP_SIN,
		MOJOSHADER_PRESHADEROP_COS,
		MOJOSHADER_PRESHADEROP_ASIN,
		MOJOSHADER_PRESHADEROP_ACOS,
		MOJOSHADER_PRESHADEROP_ATAN,
		MOJOSHADER_PRESHADEROP_MIN,
		MOJOSHADER_PRESHADEROP_MAX,
		MOJOSHADER_PRESHADEROP_LT,
		MOJOSHADER_PRESHADEROP_GE,
		MOJOSHADER_PRESHADEROP_ADD,
		MOJOSHADER_PRESHADEROP_MUL,
		MOJOSHADER_PRESHADEROP_ATAN2,
		MOJOSHADER_PRESHADEROP_DIV,
		MOJOSHADER_PRESHADEROP_CMP,
		MOJOSHADER_PRESHADEROP_MOVC,
		MOJOSHADER_PRESHADEROP_DOT,
		MOJOSHADER_PRESHADEROP_NOISE,
		MOJOSHADER_PRESHADEROP_SCALAR_OPS,
		MOJOSHADER_PRESHADEROP_MIN_SCALAR = MOJOSHADER_PRESHADEROP_SCALAR_OPS,
		MOJOSHADER_PRESHADEROP_MAX_SCALAR,
		MOJOSHADER_PRESHADEROP_LT_SCALAR,
		MOJOSHADER_PRESHADEROP_GE_SCALAR,
		MOJOSHADER_PRESHADEROP_ADD_SCALAR,
		MOJOSHADER_PRESHADEROP_MUL_SCALAR,
		MOJOSHADER_PRESHADEROP_ATAN2_SCALAR,
		MOJOSHADER_PRESHADEROP_DIV_SCALAR,
		MOJOSHADER_PRESHADEROP_DOT_SCALAR,
		MOJOSHADER_PRESHADEROP_NOISE_SCALAR
	}

	public enum MOJOSHADER_preshaderOperandType
	{
		MOJOSHADER_PRESHADEROPERAND_INPUT,
		MOJOSHADER_PRESHADEROPERAND_OUTPUT,
		MOJOSHADER_PRESHADEROPERAND_LITERAL,
		MOJOSHADER_PRESHADEROPERAND_TEMP
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_preshaderOperand
	{
		public MOJOSHADER_preshaderOperandType type;
		public uint index;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MOJOSHADER_preshaderInstruction
	{
		public MOJOSHADER_preshaderOpcode opcode;
		public uint element_count;
		public uint operand_count;
		// FIXME: public fixed MOJOSHADER_preshaderOperand operands[4];
		public MOJOSHADER_preshaderOperand operand1;
		public MOJOSHADER_preshaderOperand operand2;
		public MOJOSHADER_preshaderOperand operand3;
		public MOJOSHADER_preshaderOperand operand4;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_preshader
	{
		public uint literal_count;
		public IntPtr literals; // double*
		public uint temp_count;
		public uint symbol_count;
		public IntPtr symbols; // MOJOSHADER_symbol*
		public uint instruction_count;
		public IntPtr instructions; // MOJOSHADER_preshaderInstruction*
		public uint register_count;
		public IntPtr registers; // float*
		public IntPtr malloc; // MOJOSHADER_malloc
		public IntPtr free; // MOJOSHADER_free
		public IntPtr malloc_data; // void*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_parseData
	{
		public int error_count;
		public IntPtr errors; // MOJOSHADER_errors*
		public IntPtr profile; // const char*
		public IntPtr output; // const char*
		public int output_len;
		public int instruction_count;
		public MOJOSHADER_shaderType shader_type;
		public int major_ver;
		public int minor_ver;
		public IntPtr mainfn; // const char*
		public int uniform_count;
		public IntPtr uniforms; // MOJOSHADER_uniform*
		public int constant_count;
		public int sampler_count;
		public IntPtr samplers; // MOJOSHADER_sampler*
		public int attribute_count;
		public IntPtr attributes; // MOJOSHADER_attribute*
		public int output_count;
		public IntPtr outputs; // MOJOSHADER_attributes*
		public int swizzle_count;
		public IntPtr swizzles; // MOJOSHADER_swizzle*
		public int symbol_count;
		public IntPtr symbols; // MOJOSHADER_symbols*
		public IntPtr preshader; // MOJOSHADER_preshader*
		public IntPtr malloc; // MOJOSHADER_malloc
		public IntPtr free; // MOJOSHADER_free
		public IntPtr malloc_data; // void*
	}

	public const string MOJOSHADER_PROFILE_D3D =		"d3d";
	public const string MOJOSHADER_PROFILE_BYTECODE =	"bytecode";
	public const string MOJOSHADER_PROFILE_GLSL =		"glsl";
	public const string MOJOSHADER_PROFILE_GLSL120 =	"glsl120";
	public const string MOJOSHADER_PROFILE_GLSLES =		"glsles";
	public const string MOJOSHADER_PROFILE_ARB1 =		"arb1";
	public const string MOJOSHADER_PROFILE_NV2 =		"nv2";
	public const string MOJOSHADER_PROFILE_NV3 =		"nv3";
	public const string MOJOSHADER_PROFILE_NV4 =		"nv4";
	public const string MOJOSHADER_PROFILE_METAL =		"metal";

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int MOJOSHADER_maxShaderModel(
		byte[] profile
	);
	public static int MOJOSHADER_maxShaderModel(string profile)
	{
		return MOJOSHADER_maxShaderModel(UTF8_ToNative(profile));
	}

	/* IntPtr refers to a MOJOSHADER_parseData*, d to a void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr MOJOSHADER_parse(
		byte[] profile,
		byte[] mainfn,
		byte[] tokenbuf,
		uint bufsize,
		MOJOSHADER_swizzle[] swiz,
		uint swizcount,
		MOJOSHADER_samplerMap[] smap,
		uint smapcount,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr d
	);
	public static IntPtr MOJOSHADER_parse(
		string profile,
		string mainfn,
		byte[] tokenbuf,
		uint bufsize,
		MOJOSHADER_swizzle[] swiz,
		uint swizcount,
		MOJOSHADER_samplerMap[] smap,
		uint smapcount,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr d
	) {
		return MOJOSHADER_parse(
			UTF8_ToNative(profile),
			UTF8_ToNative(mainfn),
			tokenbuf,
			bufsize,
			swiz,
			swizcount,
			smap,
			smapcount,
			m,
			f,
			d
		);
	}

	/* data refers to a MOJOSHADER_parseData* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_freeParseData(IntPtr data);

	/* IntPtr refers to a MOJOSHADER_preshader*, d to a void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_parsePreshader(
		byte[] buf,
		uint len,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr d
	);

	/* preshader refers to a MOJOSHADER_preshader* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_freePreshader(IntPtr preshader);

	#endregion

	#region Effects Interface

	/* MOJOSHADER_effectState types... */

	public enum MOJOSHADER_renderStateType
	{
		MOJOSHADER_RS_ZENABLE,
		MOJOSHADER_RS_FILLMODE,
		MOJOSHADER_RS_SHADEMODE,
		MOJOSHADER_RS_ZWRITEENABLE,
		MOJOSHADER_RS_ALPHATESTENABLE,
		MOJOSHADER_RS_LASTPIXEL,
		MOJOSHADER_RS_SRCBLEND,
		MOJOSHADER_RS_DESTBLEND,
		MOJOSHADER_RS_CULLMODE,
		MOJOSHADER_RS_ZFUNC,
		MOJOSHADER_RS_ALPHAREF,
		MOJOSHADER_RS_ALPHAFUNC,
		MOJOSHADER_RS_DITHERENABLE,
		MOJOSHADER_RS_ALPHABLENDENABLE,
		MOJOSHADER_RS_FOGENABLE,
		MOJOSHADER_RS_SPECULARENABLE,
		MOJOSHADER_RS_FOGCOLOR,
		MOJOSHADER_RS_FOGTABLEMODE,
		MOJOSHADER_RS_FOGSTART,
		MOJOSHADER_RS_FOGEND,
		MOJOSHADER_RS_FOGDENSITY,
		MOJOSHADER_RS_RANGEFOGENABLE,
		MOJOSHADER_RS_STENCILENABLE,
		MOJOSHADER_RS_STENCILFAIL,
		MOJOSHADER_RS_STENCILZFAIL,
		MOJOSHADER_RS_STENCILPASS,
		MOJOSHADER_RS_STENCILFUNC,
		MOJOSHADER_RS_STENCILREF,
		MOJOSHADER_RS_STENCILMASK,
		MOJOSHADER_RS_STENCILWRITEMASK,
		MOJOSHADER_RS_TEXTUREFACTOR,
		MOJOSHADER_RS_WRAP0,
		MOJOSHADER_RS_WRAP1,
		MOJOSHADER_RS_WRAP2,
		MOJOSHADER_RS_WRAP3,
		MOJOSHADER_RS_WRAP4,
		MOJOSHADER_RS_WRAP5,
		MOJOSHADER_RS_WRAP6,
		MOJOSHADER_RS_WRAP7,
		MOJOSHADER_RS_WRAP8,
		MOJOSHADER_RS_WRAP9,
		MOJOSHADER_RS_WRAP10,
		MOJOSHADER_RS_WRAP11,
		MOJOSHADER_RS_WRAP12,
		MOJOSHADER_RS_WRAP13,
		MOJOSHADER_RS_WRAP14,
		MOJOSHADER_RS_WRAP15,
		MOJOSHADER_RS_CLIPPING,
		MOJOSHADER_RS_LIGHTING,
		MOJOSHADER_RS_AMBIENT,
		MOJOSHADER_RS_FOGVERTEXMODE,
		MOJOSHADER_RS_COLORVERTEX,
		MOJOSHADER_RS_LOCALVIEWER,
		MOJOSHADER_RS_NORMALIZENORMALS,
		MOJOSHADER_RS_DIFFUSEMATERIALSOURCE,
		MOJOSHADER_RS_SPECULARMATERIALSOURCE,
		MOJOSHADER_RS_AMBIENTMATERIALSOURCE,
		MOJOSHADER_RS_EMISSIVEMATERIALSOURCE,
		MOJOSHADER_RS_VERTEXBLEND,
		MOJOSHADER_RS_CLIPPLANEENABLE,
		MOJOSHADER_RS_POINTSIZE,
		MOJOSHADER_RS_POINTSIZE_MIN,
		MOJOSHADER_RS_POINTSPRITEENABLE,
		MOJOSHADER_RS_POINTSCALEENABLE,
		MOJOSHADER_RS_POINTSCALE_A,
		MOJOSHADER_RS_POINTSCALE_B,
		MOJOSHADER_RS_POINTSCALE_C,
		MOJOSHADER_RS_MULTISAMPLEANTIALIAS,
		MOJOSHADER_RS_MULTISAMPLEMASK,
		MOJOSHADER_RS_PATCHEDGESTYLE,
		MOJOSHADER_RS_DEBUGMONITORTOKEN,
		MOJOSHADER_RS_POINTSIZE_MAX,
		MOJOSHADER_RS_INDEXEDVERTEXBLENDENABLE,
		MOJOSHADER_RS_COLORWRITEENABLE,
		MOJOSHADER_RS_TWEENFACTOR,
		MOJOSHADER_RS_BLENDOP,
		MOJOSHADER_RS_POSITIONDEGREE,
		MOJOSHADER_RS_NORMALDEGREE,
		MOJOSHADER_RS_SCISSORTESTENABLE,
		MOJOSHADER_RS_SLOPESCALEDEPTHBIAS,
		MOJOSHADER_RS_ANTIALIASEDLINEENABLE,
		MOJOSHADER_RS_MINTESSELLATIONLEVEL,
		MOJOSHADER_RS_MAXTESSELLATIONLEVEL,
		MOJOSHADER_RS_ADAPTIVETESS_X,
		MOJOSHADER_RS_ADAPTIVETESS_Y,
		MOJOSHADER_RS_ADAPTIVETESS_Z,
		MOJOSHADER_RS_ADAPTIVETESS_W,
		MOJOSHADER_RS_ENABLEADAPTIVETESSELLATION,
		MOJOSHADER_RS_TWOSIDEDSTENCILMODE,
		MOJOSHADER_RS_CCW_STENCILFAIL,
		MOJOSHADER_RS_CCW_STENCILZFAIL,
		MOJOSHADER_RS_CCW_STENCILPASS,
		MOJOSHADER_RS_CCW_STENCILFUNC,
		MOJOSHADER_RS_COLORWRITEENABLE1,
		MOJOSHADER_RS_COLORWRITEENABLE2,
		MOJOSHADER_RS_COLORWRITEENABLE3,
		MOJOSHADER_RS_BLENDFACTOR,
		MOJOSHADER_RS_SRGBWRITEENABLE,
		MOJOSHADER_RS_DEPTHBIAS,
		MOJOSHADER_RS_SEPARATEALPHABLENDENABLE,
		MOJOSHADER_RS_SRCBLENDALPHA,
		MOJOSHADER_RS_DESTBLENDALPHA,
		MOJOSHADER_RS_BLENDOPALPHA,
		MOJOSHADER_RS_VERTEXSHADER = 146,
		MOJOSHADER_RS_PIXELSHADER = 147
	}

	public enum MOJOSHADER_zBufferType
	{
		MOJOSHADER_ZB_FALSE,
		MOJOSHADER_ZB_TRUE,
		MOJOSHADER_ZB_USEW
	}

	public enum MOJOSHADER_fillMode
	{
		MOJOSHADER_FILL_POINT		= 1,
		MOJOSHADER_FILL_WIREFRAME	= 2,
		MOJOSHADER_FILL_SOLID		= 3
	}

	public enum MOJOSHADER_shadeMode
	{
		MOJOSHADER_SHADE_FLAT		= 1,
		MOJOSHADER_SHADE_GOURAUD	= 2,
		MOJOSHADER_SHADE_PHONG		= 3,
	}

	public enum MOJOSHADER_blendMode
	{
		MOJOSHADER_BLEND_ZERO			= 1,
		MOJOSHADER_BLEND_ONE			= 2,
		MOJOSHADER_BLEND_SRCCOLOR		= 3,
		MOJOSHADER_BLEND_INVSRCCOLOR		= 4,
		MOJOSHADER_BLEND_SRCALPHA		= 5,
		MOJOSHADER_BLEND_INVSRCALPHA		= 6,
		MOJOSHADER_BLEND_DESTALPHA		= 7,
		MOJOSHADER_BLEND_INVDESTALPHA		= 8,
		MOJOSHADER_BLEND_DESTCOLOR		= 9,
		MOJOSHADER_BLEND_INVDESTCOLOR		= 10,
		MOJOSHADER_BLEND_SRCALPHASAT		= 11,
		MOJOSHADER_BLEND_BOTHSRCALPHA		= 12,
		MOJOSHADER_BLEND_BOTHINVSRCALPHA	= 13,
		MOJOSHADER_BLEND_BLENDFACTOR		= 14,
		MOJOSHADER_BLEND_INVBLENDFACTOR		= 15,
		MOJOSHADER_BLEND_SRCCOLOR2		= 16,
		MOJOSHADER_BLEND_INVSRCCOLOR2		= 17
	}

	public enum MOJOSHADER_cullMode
	{
		MOJOSHADER_CULL_NONE	= 1,
		MOJOSHADER_CULL_CW	= 2,
		MOJOSHADER_CULL_CCW	= 3
	}

	public enum MOJOSHADER_compareFunc
	{
		MOJOSHADER_CMP_NEVER		= 1,
		MOJOSHADER_CMP_LESS		= 2,
		MOJOSHADER_CMP_EQUAL		= 3,
		MOJOSHADER_CMP_LESSEQUAL	= 4,
		MOJOSHADER_CMP_GREATER		= 5,
		MOJOSHADER_CMP_NOTEQUAL		= 6,
		MOJOSHADER_CMP_GREATEREQUAL	= 7,
		MOJOSHADER_CMP_ALWAYS		= 8
	}

	public enum MOJOSHADER_fogMode
	{
		MOJOSHADER_FOG_NONE,
		MOJOSHADER_FOG_EXP,
		MOJOSHADER_FOG_EXP2,
		MOJOSHADER_FOG_LINEAR
	}

	public enum MOJOSHADER_stencilOp
	{
		MOJOSHADER_STENCILOP_KEEP	= 1,
		MOJOSHADER_STENCILOP_ZERO	= 2,
		MOJOSHADER_STENCILOP_REPLACE	= 3,
		MOJOSHADER_STENCILOP_INCRSAT	= 4,
		MOJOSHADER_STENCILOP_DECRSAT	= 5,
		MOJOSHADER_STENCILOP_INVERT	= 6,
		MOJOSHADER_STENCILOP_INCR	= 7,
		MOJOSHADER_STENCILOP_DECR	= 8
	}

	public enum MOJOSHADER_materialColorSource
	{
		MOJOSHADER_MCS_MATERIAL,
		MOJOSHADER_MCS_COLOR1,
		MOJOSHADER_MCS_COLOR2
	}

	public enum MOJOSHADER_vertexBlendFlags
	{
		MOJOSHADER_VBF_DISABLE	= 0,
		MOJOSHADER_VBF_1WEIGHTS	= 1,
		MOJOSHADER_VBF_2WEIGHTS	= 2,
		MOJOSHADER_VBF_3WEIGHTS	= 3,
		MOJOSHADER_VBF_TWEENING	= 255,
		MOJOSHADER_VBF_0WEIGHTS	= 256,
	}

	public enum MOJOSHADER_patchedEdgeStyle
	{
		MOJOSHADER_PATCHEDGE_DISCRETE,
		MOJOSHADER_PATCHEDGE_CONTINUOUS
	}

	public enum MOJOSHADER_debugMonitorTokens
	{
		MOJOSHADER_DMT_ENABLE,
		MOJOSHADER_DMT_DISABLE
	}

	public enum MOJOSHADER_blendOp
	{
		MOJOSHADER_BLENDOP_ADD		= 1,
		MOJOSHADER_BLENDOP_SUBTRACT	= 2,
		MOJOSHADER_BLENDOP_REVSUBTRACT	= 3,
		MOJOSHADER_BLENDOP_MIN		= 4,
		MOJOSHADER_BLENDOP_MAX		= 5
	}

	public enum MOJOSHADER_degreeType
	{
		MOJOSHADER_DEGREE_LINEAR	= 1,
		MOJOSHADER_DEGREE_QUADRATIC	= 2,
		MOJOSHADER_DEGREE_CUBIC		= 3,
		MOJOSHADER_DEGREE_QUINTIC	= 5
	}

	/* MOJOSHADER_effectSamplerState types... */

	public enum MOJOSHADER_samplerStateType
	{
		MOJOSHADER_SAMP_UNKNOWN0	= 0,
		MOJOSHADER_SAMP_UNKNOWN1	= 1,
		MOJOSHADER_SAMP_UNKNOWN2	= 2,
		MOJOSHADER_SAMP_UNKNOWN3	= 3,
		MOJOSHADER_SAMP_TEXTURE		= 4,
		MOJOSHADER_SAMP_ADDRESSU	= 5,
		MOJOSHADER_SAMP_ADDRESSV	= 6,
		MOJOSHADER_SAMP_ADDRESSW	= 7,
		MOJOSHADER_SAMP_BORDERCOLOR	= 8,
		MOJOSHADER_SAMP_MAGFILTER	= 9,
		MOJOSHADER_SAMP_MINFILTER	= 10,
		MOJOSHADER_SAMP_MIPFILTER	= 11,
		MOJOSHADER_SAMP_MIPMAPLODBIAS	= 12,
		MOJOSHADER_SAMP_MAXMIPLEVEL	= 13,
		MOJOSHADER_SAMP_MAXANISOTROPY	= 14,
		MOJOSHADER_SAMP_SRGBTEXTURE	= 15,
		MOJOSHADER_SAMP_ELEMENTINDEX	= 16,
		MOJOSHADER_SAMP_DMAPOFFSET	= 17
	}

	public enum MOJOSHADER_textureAddress
	{
		MOJOSHADER_TADDRESS_WRAP	= 1,
		MOJOSHADER_TADDRESS_MIRROR	= 2,
		MOJOSHADER_TADDRESS_CLAMP	= 3,
		MOJOSHADER_TADDRESS_BORDER	= 4,
		MOJOSHADER_TADDRESS_MIRRORONCE	= 5
	}

	public enum MOJOSHADER_textureFilterType
	{
		MOJOSHADER_TEXTUREFILTER_NONE,
		MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
		MOJOSHADER_TEXTUREFILTER_PYRAMIDALQUAD,
		MOJOSHADER_TEXTUREFILTER_GAUSSIANQUAD,
		MOJOSHADER_TEXTUREFILTER_CONVOLUTIONMONO
	}

	/* Effect value types... */

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectValue
	{
		public IntPtr name; // const char*
		public IntPtr semantic; // const char*
		public MOJOSHADER_symbolTypeInfo type;
		public uint value_count;
		public IntPtr values; // You know what, just look at the C header...
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectState
	{
		public MOJOSHADER_renderStateType type;
		public MOJOSHADER_effectValue value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectSamplerState
	{
		public MOJOSHADER_samplerStateType type;
		public MOJOSHADER_effectValue value;
	}

	/* typedef MOJOSHADER_effectValue MOJOSHADER_effectAnnotation; */
	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectAnnotation
	{
		public IntPtr name; // const char*
		public IntPtr semantic; // const char*
		public MOJOSHADER_symbolTypeInfo type;
		public uint value_count;
		public IntPtr values; // You know what, just look at the C header...
	}

	/* Effect interface structures... */

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectParam
	{
		public MOJOSHADER_effectValue value;
		public uint annotation_count;
		public IntPtr annotations; // MOJOSHADER_effectAnnotations*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectPass
	{
		public IntPtr name; // const char*
		public uint state_count;
		public IntPtr states; // MOJOSHADER_effectState*
		public uint annotation_count;
		public IntPtr annotations; // MOJOSHADER_effectAnnotations*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectTechnique
	{
		public IntPtr name; // const char*
		public uint pass_count;
		public IntPtr passes; // MOJOSHADER_effectPass*
		public uint annotation_count;
		public IntPtr annotations; // MOJOSHADER_effectAnnotations*
	}

	/* Effect "objects"... */

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectShader
	{
		public MOJOSHADER_symbolType type;
		public uint technique;
		public uint pass;
		public uint is_preshader;
		public uint preshader_param_count;
		public IntPtr preshader_params; // unsigned int*
		public uint param_count;
		public IntPtr parameters; // unsigned int*
		public uint sampler_count;
		public IntPtr samplers; // MOJOSHADER_samplerStateRegister*
		public IntPtr shader; // *shader/*preshader union
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectSamplerMap
	{
		public MOJOSHADER_symbolType type;
		public IntPtr name; // const char*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectString
	{
		public MOJOSHADER_symbolType type;
		public IntPtr stringvalue; // const char*
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectTexture
	{
		public MOJOSHADER_symbolType type;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct MOJOSHADER_effectObject
	{
		[FieldOffset(0)]
		public MOJOSHADER_symbolType type;
		[FieldOffset(0)]
		public MOJOSHADER_effectShader shader;
		[FieldOffset(0)]
		public MOJOSHADER_effectSamplerMap mapping;
		[FieldOffset(0)]
		public MOJOSHADER_effectString stringvalue;
		[FieldOffset(0)]
		public MOJOSHADER_effectTexture texture;
	}

	/* Effect state change types... */

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_samplerStateRegister
	{
		public IntPtr sampler_name; // const char*
		public uint sampler_register;
		public uint sampler_state_count;
		public IntPtr sampler_states; // const MOJOSHADER_effectSamplerState*
	}

	/* DO NOT USE STORE THIS STRUCT AS MANAGED MEMORY!
	 * Instead, call malloc(sizeof(MOJOSHADER_effectStateChanges))
	 * and send that to Begin().
	 */
	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effectStateChanges
	{
		public uint render_state_change_count;
		public IntPtr render_state_changes; // const MOJOSHADER_effectState*
		public uint sampler_state_change_count;
		public IntPtr sampler_state_changes; // const MOJOSHADER_samplerStateRegister*
		public uint vertex_sampler_state_change_count;
		public IntPtr vertex_sampler_state_changes; // const MOJOSHADER_samplerStateRegister*
	}

	/* Effect parsing interface... */

	[StructLayout(LayoutKind.Sequential)]
	public struct MOJOSHADER_effect
	{
		public int error_count;
		public IntPtr errors; // MOJOSHADER_error*
		public IntPtr profile; // const char*
		public int param_count;
		public IntPtr parameters; // MOJOSHADER_effectParam* params, lolC#
		public int technique_count;
		public IntPtr techniques; // MOJOSHADER_effectTechnique*
		public IntPtr current_technique; // const MOJOSHADER_effectTechnique*
		public int current_pass;
		public int object_count;
		public IntPtr objects; // MOJOSHADER_effectObject*
		public int restore_render_state;
		public IntPtr state_changes; // MOJOSHADER_effectStateChanges*
		public IntPtr m; // MOJOSHADER_malloc
		public IntPtr f; // MOJOSHADER_free
		public IntPtr malloc_data; // void*
	}

	/* IntPtr refers to a MOJOSHADER_effect*, d to a void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr MOJOSHADER_parseEffect(
		byte[] profile,
		byte[] buf,
		uint _len,
		MOJOSHADER_swizzle[] swiz,
		uint swizcount,
		MOJOSHADER_samplerMap[] smap,
		uint smapcount,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr d
	);
	public static IntPtr MOJOSHADER_parseEffect(
		string profile,
		byte[] buf,
		uint _len,
		MOJOSHADER_swizzle[] swiz,
		uint swizcount,
		MOJOSHADER_samplerMap[] smap,
		uint smapcount,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr d
	) {
		return MOJOSHADER_parseEffect(
			UTF8_ToNative(profile),
			buf,
			_len,
			swiz,
			swizcount,
			smap,
			smapcount,
			m,
			f,
			d
		);
	}

	/* effect refers to a MOJOSHADER_effect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_freeEffect(IntPtr effect);

	/* IntPtr/effect refer to a MOJOSHADER_effect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_cloneEffect(IntPtr effect);

	/* Effect parameter interface... */

	/* parameter refers to a MOJOSHADER_effectParam*, data to a void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_effectSetRawValueHandle(
		IntPtr parameter,
		IntPtr data,
		uint offset,
		uint len
	);

	/* effect refers to a MOJOSHADER_effect*, data to a void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern void MOJOSHADER_effectSetRawValueName(
		IntPtr effect,
		byte[] name,
		IntPtr data,
		uint offset,
		uint len
	);
	public static void MOJOSHADER_effectSetRawValueName(
		IntPtr effect,
		string name,
		IntPtr data,
		uint offset,
		uint len
	) {
		MOJOSHADER_effectSetRawValueName(
			effect,
			UTF8_ToNative(name),
			data,
			offset,
			len
		);
	}

	/* Effect technique interface... */

	/* IntPtr refers to a MOJOSHADER_effectTechnique*, effect to a MOJOSHADER_effect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_effectGetCurrentTechnique(
		IntPtr effect
	);

	/* effect refers to a MOJOSHADER_effect*, technique to a MOJOSHADER_effectTechnique* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_effectSetTechnique(
		IntPtr effect,
		IntPtr technique
	);

	/* IntPtr/technique refer to a MOJOSHADER_effectTechnique, effect to a MOJOSHADER_effect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_effectFindNextValidTechnique(
		IntPtr effect,
		IntPtr technique
	);

	/* OpenGL effect interface... */

	/* IntPtr refers to a MOJOSHADER_glEffect*, effect to a MOJOSHADER_effect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_glCompileEffect(IntPtr effect);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glDeleteEffect(IntPtr glEffect);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glEffectBegin(
		IntPtr glEffect,
		out uint numPasses,
		int saveShaderState,
		IntPtr stateChanges
	);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glEffectBeginPass(
		IntPtr glEffect,
		uint pass
	);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glEffectCommitChanges(
		IntPtr glEffect
	);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glEffectEndPass(IntPtr glEffect);

	/* glEffect refers to a MOJOSHADER_glEffect* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glEffectEnd(IntPtr glEffect);

	#endregion

	#region Preprocessor Interface

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region Assembler Interface

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region HLSL Support

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region Abtract Syntax Tree Interface

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region Intermediate Representation Interface

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region Compiler Interface

	// TODO: Needed for MojoShader#? -flibit

	#endregion

	#region OpenGL Interface

	public delegate IntPtr MOJOSHADER_glGetProcAddress(
		IntPtr fnname,
		IntPtr data
	);

	/* lookup_d refers to a void*.
	 * profs refers to a pre-allocated const char**.
	 * malloc_d to a void*.
	 */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int MOJOSHADER_glAvailableProfiles(
		MOJOSHADER_glGetProcAddress lookup,
		IntPtr lookup_d,
		IntPtr[] profs,
		int size,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr malloc_d
	);

	/* lookup_d refers to a void*, malloc_d to a void* */
	[DllImport(nativeLibName, EntryPoint = "MOJOSHADER_glBestProfile", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr INTERNAL_glBestProfile(
		MOJOSHADER_glGetProcAddress lookup,
		IntPtr lookup_d,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr malloc_d
	);
	public static string MOJOSHADER_glBestProfile(
		MOJOSHADER_glGetProcAddress lookup,
		IntPtr lookup_d,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr malloc_d
	) {
		return UTF8_ToManaged(
			INTERNAL_glBestProfile(
				lookup,
				lookup_d,
				m,
				f,
				malloc_d
			)
		);
	}

	/* IntPtr refers to a MOJOSHADER_glContext,
	 * lookup_d to a void*, malloc_d to a void*
	 */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr MOJOSHADER_glCreateContext(
		byte[] profile,
		MOJOSHADER_glGetProcAddress lookup,
		IntPtr lookup_d,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr malloc_d
	);
	public static IntPtr MOJOSHADER_glCreateContext(
		string profile,
		MOJOSHADER_glGetProcAddress lookup,
		IntPtr lookup_d,
		MOJOSHADER_malloc m,
		MOJOSHADER_free f,
		IntPtr malloc_d
	) {
		return MOJOSHADER_glCreateContext(
			UTF8_ToNative(profile),
			lookup,
			lookup_d,
			m,
			f,
			malloc_d
		);
	}

	/* ctx refers to a MOJOSHADER_glContext* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glMakeContextCurrent(IntPtr ctx);

	[DllImport(nativeLibName, EntryPoint = "MOJOSHADER_glGetError", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr INTERNAL_glGetError();
	public static string MOJOSHADER_glGetError()
	{
		return UTF8_ToManaged(INTERNAL_glGetError());
	}

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int MOJOSHADER_glMaxUniforms(
		MOJOSHADER_shaderType shader_type
	);

	/* IntPtr refers to a MOJOSHADER_glShader* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_glCompileShader(
		byte[] tokenbuf,
		uint bufsize,
		MOJOSHADER_swizzle[] swiz,
		uint swizcount,
		MOJOSHADER_samplerMap[] smap,
		uint smapcount
	);

	/* IntPtr refers to a const MOJOSHADER_parseData*
	 * shader refers to a MOJOSHADER_glShader*
	 */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_glGetShaderParseData(
		IntPtr shader
	);

	/* IntPtr refers to a MOJOSHADER_glProgram*
	 * vshader/pshader refer to a MOJOSHADER_glShader*
	 */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr MOJOSHADER_glLinkProgram(
		IntPtr vshader,
		IntPtr pshader
	);

	/* program refers to a MOJOSHADER_glProgram* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glBindProgram(IntPtr program);

	/* vshader/pshader refer to a MOJOSHADER_glShader* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glBindShaders(
		IntPtr vshader,
		IntPtr pshader
	);

	/* data refers to a const float* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetVertexShaderUniformF(
		uint idx,
		IntPtr data,
		uint vec4count
	);

	/* data refers to a float* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetVertexShaderUniformF(
		uint idx,
		IntPtr data,
		uint vec4count
	);

	/* data refers to a const int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetVertexShaderUniformI(
		uint idx,
		IntPtr data,
		uint ivec4count
	);

	/* data refers to an int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetVertexShaderUniformI(
		uint idx,
		IntPtr data,
		uint ivec4count
	);

	/* data refers to a const int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetVertexShaderUniformB(
		uint idx,
		IntPtr data,
		uint bcount
	);

	/* data refers to an int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetVertexShaderUniformB(
		uint idx,
		IntPtr data,
		uint bcount
	);

	/* data refers to a const float* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetPixelShaderUniformF(
		uint idx,
		IntPtr data,
		uint vec4count
	);

	/* data refers to a float* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetPixelShaderUniformF(
		uint idx,
		IntPtr data,
		uint vec4count
	);

	/* data refers to a const int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetPixelShaderUniformI(
		uint idx,
		IntPtr data,
		uint ivec4count
	);

	/* data refers to an int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetPixelShaderUniformI(
		uint idx,
		IntPtr data,
		uint ivec4count
	);

	/* data refers to a const int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetPixelShaderUniformB(
		uint idx,
		IntPtr data,
		uint bcount
	);

	/* data refers to an int* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glGetPixelShaderUniformB(
		uint idx,
		IntPtr data,
		uint bcount
	);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetLegacyBumpMapEnv(
		uint sampler,
		float mat00,
		float mat01,
		float mat10,
		float mat11,
		float lscale,
		float loffset
	);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern int MOJOSHADER_glGetVertexAttribLocation(
		MOJOSHADER_usage usage,
		int index
	);

	/* ptr refers to a const void* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetVertexAttribute(
		MOJOSHADER_usage usage,
		int index,
		uint size,
		MOJOSHADER_attributeType type,
		int normalized,
		uint stride,
		IntPtr ptr
	);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glSetVertexAttribDivisor(
		MOJOSHADER_usage usage,
		int index,
		uint divisor
	);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glProgramReady();

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glProgramViewportFlip(int flip);

	/* program refers to a MOJOSHADER_glProgram* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glDeleteProgram(IntPtr program);

	/* shader refers to a MOJOSHADER_glShader* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glDeleteShader(IntPtr shader);

	/* ctx refers to a MOJOSHADER_glContext* */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void MOJOSHADER_glDestroyContext(IntPtr ctx);

	#endregion
}
