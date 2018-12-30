/**
 * MojoShader; generate shader programs from bytecode of compiled
 *  Direct3D shaders.
 *
 * Please see the file LICENSE.txt in the source's root directory.
 *
 *  This file written by Ryan C. Gordon.
 */

#ifndef MOJOSHADER_EFFECTS_H
#define MOJOSHADER_EFFECTS_H

#ifdef MOJOSHADER_EFFECT_SUPPORT

/* MOJOSHADER_effectState types... */

typedef enum MOJOSHADER_renderStateType
{
    /* Note that we are NOT using the actual RS values from D3D here.
     * For some reason, in the binary data, it's 0-based.
     * Even worse, it doesn't even seem to be in order.
     * Here is the list of changes compared to the real D3DRS enum:
     * - All of the RS_WRAP values are in a row, not separate!
     *
     * -flibit
     */
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

    /* These aren't really "states", but these numbers are
     * referred to by MOJOSHADER_effectStateType as such.
     */
    MOJOSHADER_RS_VERTEXSHADER = 146,
    MOJOSHADER_RS_PIXELSHADER = 147
} MOJOSHADER_renderStateType;

typedef enum MOJOSHADER_zBufferType
{
    MOJOSHADER_ZB_FALSE,
    MOJOSHADER_ZB_TRUE,
    MOJOSHADER_ZB_USEW
} MOJOSHADER_zBufferType;

typedef enum MOJOSHADER_fillMode
{
    MOJOSHADER_FILL_POINT     = 1,
    MOJOSHADER_FILL_WIREFRAME = 2,
    MOJOSHADER_FILL_SOLID     = 3
} MOJOSHADER_fillMode;

typedef enum MOJOSHADER_shadeMode
{
    MOJOSHADER_SHADE_FLAT    = 1,
    MOJOSHADER_SHADE_GOURAUD = 2,
    MOJOSHADER_SHADE_PHONG   = 3,
} MOJOSHADER_shadeMode;

typedef enum MOJOSHADER_blendMode
{
    MOJOSHADER_BLEND_ZERO            = 1,
    MOJOSHADER_BLEND_ONE             = 2,
    MOJOSHADER_BLEND_SRCCOLOR        = 3,
    MOJOSHADER_BLEND_INVSRCCOLOR     = 4,
    MOJOSHADER_BLEND_SRCALPHA        = 5,
    MOJOSHADER_BLEND_INVSRCALPHA     = 6,
    MOJOSHADER_BLEND_DESTALPHA       = 7,
    MOJOSHADER_BLEND_INVDESTALPHA    = 8,
    MOJOSHADER_BLEND_DESTCOLOR       = 9,
    MOJOSHADER_BLEND_INVDESTCOLOR    = 10,
    MOJOSHADER_BLEND_SRCALPHASAT     = 11,
    MOJOSHADER_BLEND_BOTHSRCALPHA    = 12,
    MOJOSHADER_BLEND_BOTHINVSRCALPHA = 13,
    MOJOSHADER_BLEND_BLENDFACTOR     = 14,
    MOJOSHADER_BLEND_INVBLENDFACTOR  = 15,
    MOJOSHADER_BLEND_SRCCOLOR2       = 16,
    MOJOSHADER_BLEND_INVSRCCOLOR2    = 17
} MOJOSHADER_blendMode;

typedef enum MOJOSHADER_cullMode
{
    MOJOSHADER_CULL_NONE = 1,
    MOJOSHADER_CULL_CW   = 2,
    MOJOSHADER_CULL_CCW  = 3
} MOJOSHADER_cullMode;

typedef enum MOJOSHADER_compareFunc
{
    MOJOSHADER_CMP_NEVER        = 1,
    MOJOSHADER_CMP_LESS         = 2,
    MOJOSHADER_CMP_EQUAL        = 3,
    MOJOSHADER_CMP_LESSEQUAL    = 4,
    MOJOSHADER_CMP_GREATER      = 5,
    MOJOSHADER_CMP_NOTEQUAL     = 6,
    MOJOSHADER_CMP_GREATEREQUAL = 7,
    MOJOSHADER_CMP_ALWAYS       = 8
} MOJOSHADER_compareFunc;

typedef enum MOJOSHADER_fogMode
{
    MOJOSHADER_FOG_NONE,
    MOJOSHADER_FOG_EXP,
    MOJOSHADER_FOG_EXP2,
    MOJOSHADER_FOG_LINEAR
} MOJOSHADER_fogMode;

typedef enum MOJOSHADER_stencilOp
{
    MOJOSHADER_STENCILOP_KEEP    = 1,
    MOJOSHADER_STENCILOP_ZERO    = 2,
    MOJOSHADER_STENCILOP_REPLACE = 3,
    MOJOSHADER_STENCILOP_INCRSAT = 4,
    MOJOSHADER_STENCILOP_DECRSAT = 5,
    MOJOSHADER_STENCILOP_INVERT  = 6,
    MOJOSHADER_STENCILOP_INCR    = 7,
    MOJOSHADER_STENCILOP_DECR    = 8
} MOJOSHADER_stencilOp;

typedef enum MOJOSHADER_materialColorSource
{
    MOJOSHADER_MCS_MATERIAL,
    MOJOSHADER_MCS_COLOR1,
    MOJOSHADER_MCS_COLOR2
} MOJOSHADER_materialColorSource;

typedef enum MOJOSHADER_vertexBlendFlags
{
    MOJOSHADER_VBF_DISABLE  = 0,
    MOJOSHADER_VBF_1WEIGHTS = 1,
    MOJOSHADER_VBF_2WEIGHTS = 2,
    MOJOSHADER_VBF_3WEIGHTS = 3,
    MOJOSHADER_VBF_TWEENING = 255,
    MOJOSHADER_VBF_0WEIGHTS = 256,
} MOJOSHADER_vertexBlendFlags;

typedef enum MOJOSHADER_patchedEdgeStyle
{
    MOJOSHADER_PATCHEDGE_DISCRETE,
    MOJOSHADER_PATCHEDGE_CONTINUOUS
} MOJOSHADER_patchedEdgeStyle;

typedef enum MOJOSHADER_debugMonitorTokens
{
    MOJOSHADER_DMT_ENABLE,
    MOJOSHADER_DMT_DISABLE
} MOJOSHADER_debugMonitorTokens;

typedef enum MOJOSHADER_blendOp
{
    MOJOSHADER_BLENDOP_ADD         = 1,
    MOJOSHADER_BLENDOP_SUBTRACT    = 2,
    MOJOSHADER_BLENDOP_REVSUBTRACT = 3,
    MOJOSHADER_BLENDOP_MIN         = 4,
    MOJOSHADER_BLENDOP_MAX         = 5
} MOJOSHADER_blendOp;

typedef enum MOJOSHADER_degreeType
{
    MOJOSHADER_DEGREE_LINEAR    = 1,
    MOJOSHADER_DEGREE_QUADRATIC = 2,
    MOJOSHADER_DEGREE_CUBIC     = 3,
    MOJOSHADER_DEGREE_QUINTIC   = 5
} MOJOSHADER_degreeType;


/* MOJOSHADER_effectSamplerState types... */

typedef enum MOJOSHADER_samplerStateType
{
    MOJOSHADER_SAMP_UNKNOWN0      = 0,
    MOJOSHADER_SAMP_UNKNOWN1      = 1,
    MOJOSHADER_SAMP_UNKNOWN2      = 2,
    MOJOSHADER_SAMP_UNKNOWN3      = 3,
    MOJOSHADER_SAMP_TEXTURE       = 4,
    MOJOSHADER_SAMP_ADDRESSU      = 5,
    MOJOSHADER_SAMP_ADDRESSV      = 6,
    MOJOSHADER_SAMP_ADDRESSW      = 7,
    MOJOSHADER_SAMP_BORDERCOLOR   = 8,
    MOJOSHADER_SAMP_MAGFILTER     = 9,
    MOJOSHADER_SAMP_MINFILTER     = 10,
    MOJOSHADER_SAMP_MIPFILTER     = 11,
    MOJOSHADER_SAMP_MIPMAPLODBIAS = 12,
    MOJOSHADER_SAMP_MAXMIPLEVEL   = 13,
    MOJOSHADER_SAMP_MAXANISOTROPY = 14,
    MOJOSHADER_SAMP_SRGBTEXTURE   = 15,
    MOJOSHADER_SAMP_ELEMENTINDEX  = 16,
    MOJOSHADER_SAMP_DMAPOFFSET    = 17
} MOJOSHADER_samplerStateType;

typedef enum MOJOSHADER_textureAddress
{
    MOJOSHADER_TADDRESS_WRAP       = 1,
    MOJOSHADER_TADDRESS_MIRROR     = 2,
    MOJOSHADER_TADDRESS_CLAMP      = 3,
    MOJOSHADER_TADDRESS_BORDER     = 4,
    MOJOSHADER_TADDRESS_MIRRORONCE = 5
} MOJOSHADER_textureAddress;

typedef enum MOJOSHADER_textureFilterType
{
    MOJOSHADER_TEXTUREFILTER_NONE,
    MOJOSHADER_TEXTUREFILTER_POINT,
    MOJOSHADER_TEXTUREFILTER_LINEAR,
    MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
    MOJOSHADER_TEXTUREFILTER_PYRAMIDALQUAD,
    MOJOSHADER_TEXTUREFILTER_GAUSSIANQUAD,
    MOJOSHADER_TEXTUREFILTER_CONVOLUTIONMONO
} MOJOSHADER_textureFilterType;


/* Effect value types... */

typedef struct MOJOSHADER_effectSamplerState MOJOSHADER_effectSamplerState;

typedef struct MOJOSHADER_effectValue
{
    const char *name;
    const char *semantic;
    MOJOSHADER_symbolTypeInfo type;
    unsigned int value_count;
    union
    {
         /* Raw value types */
        void                           *values;
        int                            *valuesI;
        float                          *valuesF;
        /* As used by MOJOSHADER_effectState */
        MOJOSHADER_zBufferType         *valuesZBT;
        MOJOSHADER_fillMode            *valuesFiM;
        MOJOSHADER_shadeMode           *valuesSM;
        MOJOSHADER_blendMode           *valuesBM;
        MOJOSHADER_cullMode            *valuesCM;
        MOJOSHADER_compareFunc         *valuesCF;
        MOJOSHADER_fogMode             *valuesFoM;
        MOJOSHADER_stencilOp           *valuesSO;
        MOJOSHADER_materialColorSource *valuesMCS;
        MOJOSHADER_vertexBlendFlags    *valuesVBF;
        MOJOSHADER_patchedEdgeStyle    *valuesPES;
        MOJOSHADER_debugMonitorTokens  *valuesDMT;
        MOJOSHADER_blendOp             *valuesBO;
        MOJOSHADER_degreeType          *valuesDT;
        /* As used by MOJOSHADER_effectSamplerState */
        MOJOSHADER_textureAddress      *valuesTA;
        MOJOSHADER_textureFilterType   *valuesTFT;
        /* As used by MOJOSHADER_effectParameter */
        MOJOSHADER_effectSamplerState  *valuesSS;
    };
} MOJOSHADER_effectValue;

typedef struct MOJOSHADER_effectState
{
    MOJOSHADER_renderStateType type;
    MOJOSHADER_effectValue value;
} MOJOSHADER_effectState;

struct MOJOSHADER_effectSamplerState
{
    MOJOSHADER_samplerStateType type;
    MOJOSHADER_effectValue value;
};

typedef MOJOSHADER_effectValue MOJOSHADER_effectAnnotation;


/* Effect interface structures... */

typedef struct MOJOSHADER_effectParam
{
    MOJOSHADER_effectValue value;
    unsigned int annotation_count;
    MOJOSHADER_effectAnnotation *annotations;
} MOJOSHADER_effectParam;

typedef struct MOJOSHADER_effectPass
{
    const char *name;
    unsigned int state_count;
    MOJOSHADER_effectState *states;
    unsigned int annotation_count;
    MOJOSHADER_effectAnnotation* annotations;
} MOJOSHADER_effectPass;

typedef struct MOJOSHADER_effectTechnique
{
    const char *name;
    unsigned int pass_count;
    MOJOSHADER_effectPass *passes;
    unsigned int annotation_count;
    MOJOSHADER_effectAnnotation* annotations;
} MOJOSHADER_effectTechnique;


/* Effect "objects"... */

/* Defined later in the state change types... */
typedef struct MOJOSHADER_samplerStateRegister MOJOSHADER_samplerStateRegister;

typedef struct MOJOSHADER_effectShader
{
    MOJOSHADER_symbolType type;
    unsigned int technique;
    unsigned int pass;
    unsigned int is_preshader;
    unsigned int preshader_param_count;
    unsigned int *preshader_params;
    unsigned int param_count;
    unsigned int *params;
    unsigned int sampler_count;
    MOJOSHADER_samplerStateRegister *samplers;
    union
    {
        const MOJOSHADER_parseData *shader;
        const MOJOSHADER_preshader *preshader;
    };
} MOJOSHADER_effectShader;

typedef struct MOJOSHADER_effectSamplerMap
{
    MOJOSHADER_symbolType type;
    const char *name;
} MOJOSHADER_effectSamplerMap;

typedef struct MOJOSHADER_effectString
{
    MOJOSHADER_symbolType type;
    const char *string;
} MOJOSHADER_effectString;

typedef struct MOJOSHADER_effectTexture
{
    MOJOSHADER_symbolType type;
    /* FIXME: Does this even do anything? */
} MOJOSHADER_effectTexture;

typedef union MOJOSHADER_effectObject
{
    MOJOSHADER_symbolType type;
    union
    {
        MOJOSHADER_effectShader shader;
        MOJOSHADER_effectSamplerMap mapping;
        MOJOSHADER_effectString string;
        MOJOSHADER_effectTexture texture;
    };
} MOJOSHADER_effectObject;


/* Effect state change types... */

/* Used to store sampler states with accompanying sampler registers */
struct MOJOSHADER_samplerStateRegister
{
    const char *sampler_name;
    unsigned int sampler_register;
    unsigned int sampler_state_count;
    const MOJOSHADER_effectSamplerState *sampler_states;
};

/*
 * Used to acquire the desired render state by the effect pass.
 */
typedef struct MOJOSHADER_effectStateChanges
{
    /* Render state changes caused by effect technique */
    unsigned int render_state_change_count;
    const MOJOSHADER_effectState *render_state_changes;

    /* Sampler state changes caused by effect technique */
    unsigned int sampler_state_change_count;
    const MOJOSHADER_samplerStateRegister *sampler_state_changes;

    /* Vertex sampler state changes caused by effect technique */
    unsigned int vertex_sampler_state_change_count;
    const MOJOSHADER_samplerStateRegister *vertex_sampler_state_changes;
} MOJOSHADER_effectStateChanges;


/*
 * Structure used to return data from parsing of an effect file...
 */
/* !!! FIXME: most of these ints should be unsigned. */
typedef struct MOJOSHADER_effect
{
    /*
     * The number of elements pointed to by (errors).
     */
    int error_count;

    /*
     * (error_count) elements of data that specify errors that were generated
     *  by parsing this shader.
     * This can be NULL if there were no errors or if (error_count) is zero.
     */
    MOJOSHADER_error *errors;

    /*
     * The name of the profile used to parse the shader. Will be NULL on error.
     */
    const char *profile;

    /*
     * The number of params pointed to by (params).
     */
    int param_count;

    /*
     * (param_count) elements of data that specify parameter bind points for
     *  this effect.
     * This can be NULL on error or if (param_count) is zero.
     */
    MOJOSHADER_effectParam *params;

    /*
     * The number of elements pointed to by (techniques).
     */
    int technique_count;

    /*
     * (technique_count) elements of data that specify techniques used in
     *  this effect. Each technique contains a series of passes, and each pass
     *  specifies state and shaders that affect rendering.
     * This can be NULL on error or if (technique_count) is zero.
     */
    MOJOSHADER_effectTechnique *techniques;

    /*
     * The technique currently being rendered by this effect.
     */
    const MOJOSHADER_effectTechnique *current_technique;

    /*
     * The index of the current pass being rendered by this effect.
     */
    int current_pass;

    /*
     * The number of elements pointed to by (objects).
     */
    int object_count;

    /*
     * (object_count) elements of data that specify objects used in
     *  this effect.
     * This can be NULL on error or if (object_count) is zero.
     */
    MOJOSHADER_effectObject *objects;

    /*
     * Value used to determine whether or not to restore the previous shader
     * state after rendering an effect, as requested by application.
     */
    int restore_shader_state;

    /*
     * The structure provided by the appliation to store the state changes.
     */
    MOJOSHADER_effectStateChanges *state_changes;

    /*
     * This is the malloc implementation you passed to MOJOSHADER_parseEffect().
     */
    MOJOSHADER_malloc malloc;

    /*
     * This is the free implementation you passed to MOJOSHADER_parseEffect().
     */
    MOJOSHADER_free free;

    /*
     * This is the pointer you passed as opaque data for your allocator.
     */
    void *malloc_data;
} MOJOSHADER_effect;


/* Effect parsing interface... */

/* !!! FIXME: document me. */
DECLSPEC MOJOSHADER_effect *MOJOSHADER_parseEffect(const char *profile,
                                                   const unsigned char *buf,
                                                   const unsigned int _len,
                                                   const MOJOSHADER_swizzle *swiz,
                                                   const unsigned int swizcount,
                                                   const MOJOSHADER_samplerMap *smap,
                                                   const unsigned int smapcount,
                                                   MOJOSHADER_malloc m,
                                                   MOJOSHADER_free f,
                                                   void *d);


/* !!! FIXME: document me. */
DECLSPEC void MOJOSHADER_freeEffect(const MOJOSHADER_effect *effect);


/* !!! FIXME: document me. */
DECLSPEC MOJOSHADER_effect *MOJOSHADER_cloneEffect(const MOJOSHADER_effect *effect);


/* Effect parameter interface... */

/* Set the constant value for the specified effect parameter.
 *
 * This function maps to ID3DXEffect::SetRawValue.
 *
 * (parameter) is a parameter obtained from a MOJOSHADER_effect*.
 * (data) is the constant values to be applied to the parameter.
 * (offset) is the offset, in bytes, of the parameter data being modified.
 * (len) is the size, in bytes, of the data buffer being applied.
 *
 * This function is thread safe.
 */
DECLSPEC void MOJOSHADER_effectSetRawValueHandle(const MOJOSHADER_effectParam *parameter,
                                                 const void *data,
                                                 const unsigned int offset,
                                                 const unsigned int len);

/* Set the constant value for the effect parameter, specified by name.
 *  Note: this function is slower than MOJOSHADER_effectSetRawValueHandle(),
 *  but we still provide it to fully map to ID3DXEffect.
 *
 * This function maps to ID3DXEffect::SetRawValue.
 *
 * (effect) is a MOJOSHADER_effect* obtained from MOJOSHADER_parseEffect().
 * (name) is the human-readable name of the parameter being modified.
 * (data) is the constant values to be applied to the parameter.
 * (offset) is the offset, in bytes, of the parameter data being modified.
 * (len) is the size, in bytes, of the data buffer being applied.
 *
 * This function is thread safe.
 */
DECLSPEC void MOJOSHADER_effectSetRawValueName(const MOJOSHADER_effect *effect,
                                               const char *name,
                                               const void *data,
                                               const unsigned int offset,
                                               const unsigned int len);


/* Effect technique interface... */

/* Get the current technique in use by an effect.
 *
 * This function maps to ID3DXEffect::GetCurrentTechnique.
 *
 * (effect) is a MOJOSHADER_effect* obtained from MOJOSHADER_parseEffect().
 *
 * This function returns the technique currently used by the given effect.
 *
 * This function is thread safe.
 */
DECLSPEC const MOJOSHADER_effectTechnique *MOJOSHADER_effectGetCurrentTechnique(const MOJOSHADER_effect *effect);

/* Set the current technique to be used an effect.
 *
 * This function maps to ID3DXEffect::SetTechnique.
 *
 * (effect) is a MOJOSHADER_effect* obtained from MOJOSHADER_parseEffect().
 * (technique) is the technique to be used by the effect when rendered.
 *
 * This function is thread safe.
 */
DECLSPEC void MOJOSHADER_effectSetTechnique(MOJOSHADER_effect *effect,
                                            const MOJOSHADER_effectTechnique *technique);

/* Get the next technique in an effect's list.
 *
 * This function maps to ID3DXEffect::FindNextValidTechnique.
 *
 * (effect) is a MOJOSHADER_effect* obtained from MOJOSHADER_parseEffect().
 * (technique) can either be a technique found in the given effect, or NULL to
 *  find the first technique in the given effect.
 *
 * This function returns either the next technique after the passed technique,
 *  or the first technique if the passed technique is NULL.
 *
 * This function is thread safe.
 */
DECLSPEC const MOJOSHADER_effectTechnique *MOJOSHADER_effectFindNextValidTechnique(const MOJOSHADER_effect *effect,
                                                                                   const MOJOSHADER_effectTechnique *technique);


/* OpenGL effect interface... */

typedef struct MOJOSHADER_glEffect MOJOSHADER_glEffect;

/* Fully compile/link the shaders found within the effect.
 *
 * The MOJOSHADER_glEffect* is solely for use within the OpenGL-specific calls.
 *  In all other cases you will be using the MOJOSHADER_effect* instead.
 *
 * In a typical use case, you will be calling this immediately after obtaining
 *  the MOJOSHADER_effect*.
 *
 * (effect) is a MOJOSHADER_effect* obtained from MOJOSHADER_parseEffect().
 *
 * This function returns a MOJOSHADER_glEffect*, containing OpenGL-specific
 *  data for an accompanying MOJOSHADER_effect*.
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC MOJOSHADER_glEffect *MOJOSHADER_glCompileEffect(MOJOSHADER_effect *effect);

/* Delete the shaders that were allocated for an effect.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glDeleteEffect(MOJOSHADER_glEffect *glEffect);

/* Prepare the effect for rendering with the currently applied technique.
 *
 * This function maps to ID3DXEffect::Begin.
 *
 * In addition to the expected Begin parameters, we also include a parameter
 *  to pass in a MOJOSHADER_effectRenderState. Rather than change the render
 *  state within MojoShader itself we will simply provide what the effect wants
 *  and allow you to use this information with your own renderer.
 *  MOJOSHADER_glEffectBeginPass will update with the render state desired by
 *  the current effect pass.
 *
 * Note that we only provide the ability to preserve the shader state, but NOT
 * the ability to preserve the render/sampler states. You are expected to
 * track your own GL state and restore these states as needed for your
 * application.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 * (numPasses) will be filled with the number of passes that this technique
 *  will need to fully render.
 * (saveShaderState) is a boolean value informing the effect whether or not to
 *  restore the shader bindings after calling MOJOSHADER_glEffectEnd.
 * (renderState) will be filled by the effect to inform you of the render state
 *  changes introduced by the technique and its passes.
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glEffectBegin(MOJOSHADER_glEffect *glEffect,
                                       unsigned int *numPasses,
                                       int saveShaderState,
                                       MOJOSHADER_effectStateChanges *stateChanges);

/* Begin an effect pass from the currently applied technique.
 *
 * This function maps to ID3DXEffect::BeginPass.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 * (pass) is the index of the effect pass as found in the current technique.
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glEffectBeginPass(MOJOSHADER_glEffect *glEffect,
                                           unsigned int pass);

/* Push render state changes that occurred within an actively rendering pass.
 *
 * This function maps to ID3DXEffect::CommitChanges.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glEffectCommitChanges(MOJOSHADER_glEffect *glEffect);

/* End an effect pass from the currently applied technique.
 *
 * This function maps to ID3DXEffect::EndPass.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glEffectEndPass(MOJOSHADER_glEffect *glEffect);

/* Complete rendering the effect technique, and restore the render state.
 *
 * This function maps to ID3DXEffect::End.
 *
 * (glEffect) is a MOJOSHADER_glEffect* obtained from
 *  MOJOSHADER_glCompileEffect().
 *
 * This call is NOT thread safe! As most OpenGL implementations are not thread
 * safe, you should probably only call this from the same thread that created
 * the GL context.
 */
DECLSPEC void MOJOSHADER_glEffectEnd(MOJOSHADER_glEffect *glEffect);

#endif /* MOJOSHADER_EFFECT_SUPPORT */

#endif /* MOJOSHADER_EFFECTS_H */
