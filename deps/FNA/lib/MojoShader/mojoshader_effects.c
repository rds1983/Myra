/**
 * MojoShader; generate shader programs from bytecode of compiled
 *  Direct3D shaders.
 *
 * Please see the file LICENSE.txt in the source's root directory.
 *
 *  This file written by Ryan C. Gordon.
 */

#define __MOJOSHADER_INTERNAL__ 1
#include "mojoshader_internal.h"

#ifdef MOJOSHADER_EFFECT_SUPPORT

#include <math.h>

void MOJOSHADER_runPreshader(const MOJOSHADER_preshader *preshader,
                             float *outregs)
{
    const float *inregs = preshader->registers;

    // this is fairly straightforward, as there aren't any branching
    //  opcodes in the preshader instruction set (at the moment, at least).
    const int scalarstart = (int) MOJOSHADER_PRESHADEROP_SCALAR_OPS;

    double *temps = NULL;
    if (preshader->temp_count > 0)
    {
        temps = (double *) alloca(sizeof (double) * preshader->temp_count);
        memset(temps, '\0', sizeof (double) * preshader->temp_count);
    } // if

    double dst[4] = { 0, 0, 0, 0 };
    double src[3][4] = { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
    const double *src0 = &src[0][0];
    const double *src1 = &src[1][0];
    const double *src2 = &src[2][0];

    MOJOSHADER_preshaderInstruction *inst = preshader->instructions;
    int instit;

    for (instit = 0; instit < preshader->instruction_count; instit++, inst++)
    {
        const MOJOSHADER_preshaderOperand *operand = inst->operands;
        const int elems = inst->element_count;
        const int elemsbytes = sizeof (double) * elems;
        const int isscalarop = (inst->opcode >= scalarstart);

        assert(elems >= 0);
        assert(elems <= 4);

        // load up our operands...
        int opiter, elemiter;
        for (opiter = 0; opiter < inst->operand_count-1; opiter++, operand++)
        {
            const int isscalar = ((isscalarop) && (opiter == 0));
            const unsigned int index = operand->index;
            switch (operand->type)
            {
                case MOJOSHADER_PRESHADEROPERAND_LITERAL:
                {
                    if (!isscalar)
                    {
                        assert((index + elems) <= preshader->literal_count);
                        memcpy(&src[opiter][0], &preshader->literals[index], elemsbytes);
                    } // if
                    else
                    {
                        for (elemiter = 0; elemiter < elems; elemiter++)
                            src[opiter][elemiter] = preshader->literals[index];
                    } // else
                    break;
                } // case

                case MOJOSHADER_PRESHADEROPERAND_INPUT:
                    if (operand->array_register_count > 0)
                    {
                        int i;
                        const int *regsi = (const int *) inregs;
                        int arrIndex = regsi[((index >> 4) * 4) + ((index >> 2) & 3)];
                        for (i = 0; i < operand->array_register_count; i++)
                        {
                            arrIndex = regsi[operand->array_registers[i] + arrIndex];
                        }
                        src[opiter][0] = arrIndex;
                    } // if
                    else if (isscalar)
                        src[opiter][0] = inregs[index];
                    else
                    {
                        int cpy;
                        for (cpy = 0; cpy < elems; cpy++)
                            src[opiter][cpy] = inregs[index+cpy];
                    } // else
                    break;

                case MOJOSHADER_PRESHADEROPERAND_OUTPUT:
                    if (isscalar)
                        src[opiter][0] = outregs[index];
                    else
                    {
                        int cpy;
                        for (cpy = 0; cpy < elems; cpy++)
                            src[opiter][cpy] = outregs[index+cpy];
                    } // else
                    break;

                case MOJOSHADER_PRESHADEROPERAND_TEMP:
                    if (temps != NULL)
                    {
                        if (isscalar)
                            src[opiter][0] = temps[index];
                        else
                            memcpy(src[opiter], temps + index, elemsbytes);
                    } // if
                    break;

                default:
                    assert(0 && "unexpected preshader operand type.");
                    return;
            } // switch
        } // for

        // run the actual instruction, store result to dst.
        int i;
        switch (inst->opcode)
        {
            #define OPCODE_CASE(op, val) \
                case MOJOSHADER_PRESHADEROP_##op: \
                    for (i = 0; i < elems; i++) { dst[i] = val; } \
                    break;

            //OPCODE_CASE(NOP, 0.0)  // not a real instruction.
            OPCODE_CASE(MOV, src0[i])
            OPCODE_CASE(NEG, -src0[i])
            OPCODE_CASE(RCP, 1.0 / src0[i])
            OPCODE_CASE(FRC, src0[i] - floor(src0[i]))
            OPCODE_CASE(EXP, exp(src0[i]))
            OPCODE_CASE(LOG, log(src0[i]))
            OPCODE_CASE(RSQ, 1.0 / sqrt(src0[i]))
            OPCODE_CASE(SIN, sin(src0[i]))
            OPCODE_CASE(COS, cos(src0[i]))
            OPCODE_CASE(ASIN, asin(src0[i]))
            OPCODE_CASE(ACOS, acos(src0[i]))
            OPCODE_CASE(ATAN, atan(src0[i]))
            OPCODE_CASE(MIN, (src0[i] < src1[i]) ? src0[i] : src1[i])
            OPCODE_CASE(MAX, (src0[i] > src1[i]) ? src0[i] : src1[i])
            OPCODE_CASE(LT, (src0[i] < src1[i]) ? 1.0 : 0.0)
            OPCODE_CASE(GE, (src0[i] >= src1[i]) ? 1.0 : 0.0)
            OPCODE_CASE(ADD, src0[i] + src1[i])
            OPCODE_CASE(MUL,  src0[i] * src1[i])
            OPCODE_CASE(ATAN2, atan2(src0[i], src1[i]))
            OPCODE_CASE(DIV, src0[i] / src1[i])
            OPCODE_CASE(CMP, (src0[i] >= 0.0) ? src1[i] : src2[i])
            //OPCODE_CASE(NOISE, ???)  // !!! FIXME: don't know what this does
            //OPCODE_CASE(MOVC, ???)  // !!! FIXME: don't know what this does
            OPCODE_CASE(MIN_SCALAR, (src0[0] < src1[i]) ? src0[0] : src1[i])
            OPCODE_CASE(MAX_SCALAR, (src0[0] > src1[i]) ? src0[0] : src1[i])
            OPCODE_CASE(LT_SCALAR, (src0[0] < src1[i]) ? 1.0 : 0.0)
            OPCODE_CASE(GE_SCALAR, (src0[0] >= src1[i]) ? 1.0 : 0.0)
            OPCODE_CASE(ADD_SCALAR, src0[0] + src1[i])
            OPCODE_CASE(MUL_SCALAR, src0[0] * src1[i])
            OPCODE_CASE(ATAN2_SCALAR, atan2(src0[0], src1[i]))
            OPCODE_CASE(DIV_SCALAR, src0[0] / src1[i])
            //OPCODE_CASE(DOT_SCALAR)  // !!! FIXME: isn't this just a MUL?
            //OPCODE_CASE(NOISE_SCALAR, ???)  // !!! FIXME: ?
            #undef OPCODE_CASE

            case MOJOSHADER_PRESHADEROP_DOT:
            {
                double final = 0.0;
                for (i = 0; i < elems; i++)
                    final += src0[i] * src1[i];
                for (i = 0; i < elems; i++)
                    dst[i] = final;  // !!! FIXME: is this right?
                break;
            } // case

            default:
                assert(0 && "Unhandled preshader opcode!");
                break;
        } // switch

        // Figure out where dst wants to be stored.
        if (operand->type == MOJOSHADER_PRESHADEROPERAND_TEMP)
        {
            assert(preshader->temp_count >=
                    operand->index + (elemsbytes / sizeof (double)));
            memcpy(temps + operand->index, dst, elemsbytes);
        } // if
        else
        {
            assert(operand->type == MOJOSHADER_PRESHADEROPERAND_OUTPUT);
            for (i = 0; i < elems; i++)
                outregs[operand->index + i] = (float) dst[i];
        } // else
    } // for
} // MOJOSHADER_runPreshader

static MOJOSHADER_effect MOJOSHADER_out_of_mem_effect = {
    1, &MOJOSHADER_out_of_mem_error, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};

static uint32 readui32(const uint8 **_ptr, uint32 *_len)
{
    uint32 retval = 0;
    if (*_len < sizeof (retval))
        *_len = 0;
    else
    {
        const uint32 *ptr = (const uint32 *) *_ptr;
        retval = SWAP32(*ptr);
        *_ptr += sizeof (retval);
        *_len -= sizeof (retval);
    } // else
    return retval;
} // readui32

static char *readstring(const uint8 *base,
                        const uint32 offset,
                        MOJOSHADER_malloc m,
                        void *d)
{
    // !!! FIXME: sanity checks!
    // !!! FIXME: verify this doesn't go past EOF looking for a null.
    const char *str = ((const char *) base) + offset;
    const uint32 len = *((const uint32 *) str);
    char *strptr = NULL;
    if (len == 0) return NULL; /* No length? No string. */
    strptr = (char *) m(len, d);
    memcpy(strptr, str + 4, len);
    return strptr;
} // readstring

static int findparameter(const MOJOSHADER_effectParam *params,
                         const uint32 param_count,
                         const char *name)
{
    int i;
    for (i = 0; i < param_count; i++)
        if (strcmp(name, params[i].value.name) == 0)
            return i;
    assert(0 && "Parameter not found!");
}

static void readvalue(const uint8 *base,
                      const uint32 typeoffset,
                      const uint32 valoffset,
                      MOJOSHADER_effectValue *value,
                      MOJOSHADER_effectObject *objects,
                      MOJOSHADER_malloc m,
                      void *d)
{
    int i, j, k;
    const uint8 *typeptr = base + typeoffset;
    const uint8 *valptr = base + valoffset;
    unsigned int typelen = 9999999;  // !!! FIXME
    const uint32 type = readui32(&typeptr, &typelen);
    const uint32 valclass = readui32(&typeptr, &typelen);
    const uint32 name = readui32(&typeptr, &typelen);
    const uint32 semantic = readui32(&typeptr, &typelen);
    const uint32 numelements = readui32(&typeptr, &typelen);

    value->type.parameter_type = (MOJOSHADER_symbolType) type;
    value->type.parameter_class = (MOJOSHADER_symbolClass) valclass;
    value->name = readstring(base, name, m, d);
    value->semantic = readstring(base, semantic, m, d);
    value->type.elements = numelements;

    /* Class sanity check */
    assert(valclass >= MOJOSHADER_SYMCLASS_SCALAR && valclass <= MOJOSHADER_SYMCLASS_STRUCT);

    if (valclass == MOJOSHADER_SYMCLASS_SCALAR
     || valclass == MOJOSHADER_SYMCLASS_VECTOR
     || valclass == MOJOSHADER_SYMCLASS_MATRIX_ROWS
     || valclass == MOJOSHADER_SYMCLASS_MATRIX_COLUMNS)
    {
        /* These classes only ever contain scalar values */
        assert(type >= MOJOSHADER_SYMTYPE_BOOL && type <= MOJOSHADER_SYMTYPE_FLOAT);

        const uint32 columncount = readui32(&typeptr, &typelen);
        const uint32 rowcount = readui32(&typeptr, &typelen);

        value->type.columns = columncount;
        value->type.rows = rowcount;

        uint32 siz = 4 * rowcount;
        if (numelements > 0)
            siz *= numelements;
        value->value_count = siz;
        siz *= 4;
        value->values = m(siz, d);
        memset(value->values, '\0', siz);
        siz /= 16;
        for (i = 0; i < siz; i++)
            memcpy(value->valuesF + (i << 2), valptr + ((columncount << 2) * i), columncount << 2);
    } // if
    else if (valclass == MOJOSHADER_SYMCLASS_OBJECT)
    {
        /* This class contains either samplers or "objects" */
        assert(type >= MOJOSHADER_SYMTYPE_STRING && type <= MOJOSHADER_SYMTYPE_VERTEXSHADER);

        if (type == MOJOSHADER_SYMTYPE_SAMPLER
         || type == MOJOSHADER_SYMTYPE_SAMPLER1D
         || type == MOJOSHADER_SYMTYPE_SAMPLER2D
         || type == MOJOSHADER_SYMTYPE_SAMPLER3D
         || type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        {
            unsigned int vallen = 9999999; // !!! FIXME
            const uint32 numstates = readui32(&valptr, &vallen);

            value->value_count = numstates;

            const uint32 siz = sizeof(MOJOSHADER_effectSamplerState) * numstates;
            value->values = m(siz, d);
            memset(value->values, '\0', siz);

            for (i = 0; i < numstates; i++)
            {
                MOJOSHADER_effectSamplerState *state = &value->valuesSS[i];
                const uint32 stype = readui32(&valptr, &vallen) & ~0xA0;
                /*const uint32 FIXME =*/ readui32(&valptr, &vallen);
                const uint32 statetypeoffset = readui32(&valptr, &vallen);
                const uint32 statevaloffset = readui32(&valptr, &vallen);

                state->type = (MOJOSHADER_samplerStateType) stype;
                readvalue(base, statetypeoffset, statevaloffset,
                          &state->value, objects,
                          m, d);
                if (stype == MOJOSHADER_SAMP_TEXTURE)
                    objects[state->value.valuesI[0]].type = (MOJOSHADER_symbolType) type;
            } // for
        } // if
        else
        {
            uint32 numobjects = 1;
            if (numelements > 0)
                numobjects = numelements;

            value->value_count = numobjects;

            const uint32 siz = 4 * numobjects;
            value->values = m(siz, d);
            memcpy(value->values, valptr, siz);

            for (i = 0; i < value->value_count; i++)
                objects[value->valuesI[i]].type = (MOJOSHADER_symbolType) type;
        } // else
    } // else if
    else if (valclass == MOJOSHADER_SYMCLASS_STRUCT)
    {
        uint32 siz;

        value->type.member_count = readui32(&typeptr, &typelen);
        siz = value->type.member_count * sizeof (MOJOSHADER_symbolStructMember);
        value->type.members = (MOJOSHADER_symbolStructMember *) m(siz, d);

        uint32 structsize = 0;
        for (i = 0; i < value->type.member_count; i++)
        {
            MOJOSHADER_symbolStructMember *mem = &value->type.members[i];

            mem->info.parameter_type = (MOJOSHADER_symbolType) readui32(&typeptr, &typelen);
            mem->info.parameter_class = (MOJOSHADER_symbolClass) readui32(&typeptr, &typelen);

            const uint32 memname = readui32(&typeptr, &typelen);
            /*const uint32 memsemantic =*/ readui32(&typeptr, &typelen);
            mem->name = readstring(base, memname, m, d);

            mem->info.elements = readui32(&typeptr, &typelen);
            mem->info.columns = readui32(&typeptr, &typelen);
            mem->info.rows = readui32(&typeptr, &typelen);

            // !!! FIXME: Nested structs! -flibit
            assert(mem->info.parameter_class >= MOJOSHADER_SYMCLASS_SCALAR
                && mem->info.parameter_class <= MOJOSHADER_SYMCLASS_VECTOR);
            assert(mem->info.parameter_type >= MOJOSHADER_SYMTYPE_BOOL
                && mem->info.parameter_type <= MOJOSHADER_SYMTYPE_FLOAT);
            mem->info.member_count = 0;
            mem->info.members = NULL;

            uint32 memsize = 4 * mem->info.rows;
            if (mem->info.elements > 0)
                memsize *= mem->info.elements;
            structsize += memsize;
        } // for

        value->type.columns = structsize;
        value->type.rows = 1;
        value->value_count = structsize;
        if (numelements > 0)
            value->value_count *= numelements;

        siz = value->value_count * 4;
        value->values = m(siz, d);
        memset(value->values, '\0', siz);
        int dst_offset = 0, src_offset = 0;
        i = 0;
        do
        {
            for (j = 0; j < value->type.member_count; j++)
            {
                siz = value->type.members[j].info.rows * value->type.members[j].info.elements;
                for (k = 0; k < siz; k++)
                {
                    memcpy(value->valuesF + dst_offset,
                           typeptr + src_offset, /* Yes, typeptr. -flibit */
                           value->type.members[j].info.columns << 2);
                    dst_offset += 4;
                    src_offset += value->type.members[j].info.columns << 2;
                } // for
            }
        } while (++i < numelements);
    } // else if
} // readvalue

static void readannotations(const uint32 numannos,
                            const uint8 *base,
                            const uint8 **ptr,
                            uint32 *len,
                            MOJOSHADER_effectAnnotation **annotations,
                            MOJOSHADER_effectObject *objects,
                            MOJOSHADER_malloc m,
                            void *d)
{
    int i;
    if (numannos == 0) return;

    const uint32 siz = sizeof(MOJOSHADER_effectAnnotation) * numannos;
    *annotations = (MOJOSHADER_effectAnnotation *) m(siz, d);
    memset(*annotations, '\0', siz);

    for (i = 0; i < numannos; i++)
    {
        MOJOSHADER_effectAnnotation *anno = &(*annotations)[i];

        const uint32 typeoffset = readui32(ptr, len);
        const uint32 valoffset = readui32(ptr, len);

        readvalue(base, typeoffset, valoffset,
                  anno, objects,
                  m, d);
    } // for
} // readannotation

static void readparameters(const uint32 numparams,
                           const uint8 *base,
                           const uint8 **ptr,
                           uint32 *len,
                           MOJOSHADER_effectParam **params,
                           MOJOSHADER_effectObject *objects,
                           MOJOSHADER_malloc m,
                           void *d)
{
    int i;
    if (numparams == 0) return;

    uint32 siz = sizeof(MOJOSHADER_effectParam) * numparams;
    *params = (MOJOSHADER_effectParam *) m(siz, d);
    memset(*params, '\0', siz);

    for (i = 0; i < numparams; i++)
    {
        MOJOSHADER_effectParam *param = &(*params)[i];

        const uint32 typeoffset = readui32(ptr, len);
        const uint32 valoffset = readui32(ptr, len);
        /*const uint32 flags =*/ readui32(ptr, len);
        const uint32 numannos = readui32(ptr, len);

        param->annotation_count = numannos;
        readannotations(numannos, base, ptr, len,
                        &param->annotations, objects,
                        m, d);

        readvalue(base, typeoffset, valoffset,
                  &param->value, objects,
                  m, d);
    } // for
} // readparameters

static void readstates(const uint32 numstates,
                       const uint8 *base,
                       const uint8 **ptr,
                       uint32 *len,
                       MOJOSHADER_effectState **states,
                       MOJOSHADER_effectObject *objects,
                       MOJOSHADER_malloc m,
                       void *d)
{
    int i;
    if (numstates == 0) return;

    const uint32 siz = sizeof (MOJOSHADER_effectState) * numstates;
    *states = (MOJOSHADER_effectState *) m(siz, d);
    memset(*states, '\0', siz);

    for (i = 0; i < numstates; i++)
    {
        MOJOSHADER_effectState *state = &(*states)[i];

        const uint32 type = readui32(ptr, len);
        /*const uint32 FIXME =*/ readui32(ptr, len);
        const uint32 typeoffset = readui32(ptr, len);
        const uint32 valoffset = readui32(ptr, len);

        state->type = (MOJOSHADER_renderStateType) type;
        readvalue(base, typeoffset, valoffset,
                  &state->value, objects,
                  m, d);
    } // for
} // readstates

static void readpasses(const uint32 numpasses,
                       const uint8 *base,
                       const uint8 **ptr,
                       uint32 *len,
                       MOJOSHADER_effectPass **passes,
                       MOJOSHADER_effectObject *objects,
                       MOJOSHADER_malloc m,
                       void *d)
{
    int i;
    if (numpasses == 0) return;

    const uint32 siz = sizeof (MOJOSHADER_effectPass) * numpasses;
    *passes = (MOJOSHADER_effectPass *) m(siz, d);
    memset(*passes, '\0', siz);

    for (i = 0; i < numpasses; i++)
    {
        MOJOSHADER_effectPass *pass = &(*passes)[i];

        const uint32 passnameoffset = readui32(ptr, len);
        const uint32 numannos = readui32(ptr, len);
        const uint32 numstates = readui32(ptr, len);

        pass->name = readstring(base, passnameoffset, m, d);

        pass->annotation_count = numannos;
        readannotations(numannos, base, ptr, len,
                        &pass->annotations, objects,
                        m, d);

        pass->state_count = numstates;
        readstates(numstates, base, ptr, len,
                   &pass->states, objects,
                   m, d);
    } // for
} // readpasses

static void readtechniques(const uint32 numtechniques,
                           const uint8 *base,
                           const uint8 **ptr,
                           uint32 *len,
                           MOJOSHADER_effectTechnique **techniques,
                           MOJOSHADER_effectObject *objects,
                           MOJOSHADER_malloc m,
                           void *d)
{
    int i;
    if (numtechniques == 0) return;

    const uint32 siz = sizeof (MOJOSHADER_effectTechnique) * numtechniques;
    *techniques = (MOJOSHADER_effectTechnique *) m(siz, d);
    memset(*techniques, '\0', siz);

    for (i = 0; i < numtechniques; i++)
    {
        MOJOSHADER_effectTechnique *technique = &(*techniques)[i];

        const uint32 nameoffset = readui32(ptr, len);
        const uint32 numannos = readui32(ptr, len);
        const uint32 numpasses = readui32(ptr, len);

        technique->name = readstring(base, nameoffset, m, d);

        technique->annotation_count = numannos;
        readannotations(numannos, base, ptr, len,
                        &technique->annotations, objects,
                        m, d);

        technique->pass_count = numpasses;
        readpasses(numpasses, base, ptr, len,
                   &technique->passes, objects,
                   m, d);
    } // for
} // readtechniques

static void readsmallobjects(const uint32 numsmallobjects,
                             const uint8 **ptr,
                             uint32 *len,
                             MOJOSHADER_effect *effect,
                             const char *profile,
                             const MOJOSHADER_swizzle *swiz,
                             const unsigned int swizcount,
                             const MOJOSHADER_samplerMap *smap,
                             const unsigned int smapcount,
                             MOJOSHADER_malloc m,
                             MOJOSHADER_free f,
                             void *d)
{
    int i, j;
    if (numsmallobjects == 0) return;

    for (i = 1; i < numsmallobjects + 1; i++)
    {
        const uint32 index = readui32(ptr, len);
        const uint32 length = readui32(ptr, len);

        MOJOSHADER_effectObject *object = &effect->objects[index];
        if (object->type == MOJOSHADER_SYMTYPE_STRING)
        {
            if (length > 0)
            {
                char *str = (char *) m(length, d);
                memcpy(str, *ptr, length);
                object->string.string = str;
            } // if
        } // if
        else if (object->type == MOJOSHADER_SYMTYPE_TEXTURE
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE1D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE2D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE3D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURECUBE
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER1D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER2D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER3D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        {
            if (length > 0)
            {
                char *str = (char *) m(length, d);
                memcpy(str, *ptr, length);
                object->mapping.name = str;
            } // if
        } // else if
        else if (object->type == MOJOSHADER_SYMTYPE_PIXELSHADER
              || object->type == MOJOSHADER_SYMTYPE_VERTEXSHADER)
        {
            char mainfn[32];
            snprintf(mainfn, sizeof (mainfn), "ShaderFunction%u", (unsigned int) index);
            object->shader.technique = -1;
            object->shader.pass = -1;
            object->shader.shader = MOJOSHADER_parse(profile, mainfn, *ptr, length,
                                                     swiz, swizcount, smap, smapcount,
                                                     m, f, d);
            // !!! FIXME: check for errors.
            for (j = 0; j < object->shader.shader->symbol_count; j++)
                if (object->shader.shader->symbols[j].register_set == MOJOSHADER_SYMREGSET_SAMPLER)
                    object->shader.sampler_count++;
            object->shader.param_count = object->shader.shader->symbol_count;
            object->shader.params = (uint32 *) m(object->shader.param_count * sizeof (uint32), d);
            object->shader.samplers = (MOJOSHADER_samplerStateRegister *) m(object->shader.sampler_count * sizeof (MOJOSHADER_samplerStateRegister), d);
            uint32 curSampler = 0;
            for (j = 0; j < object->shader.shader->symbol_count; j++)
            {
                int par = findparameter(effect->params,
                                        effect->param_count,
                                        object->shader.shader->symbols[j].name);
                object->shader.params[j] = par;
                if (object->shader.shader->symbols[j].register_set == MOJOSHADER_SYMREGSET_SAMPLER)
                {
                    object->shader.samplers[curSampler].sampler_name = effect->params[par].value.name;
                    object->shader.samplers[curSampler].sampler_register = object->shader.shader->symbols[j].register_index;
                    object->shader.samplers[curSampler].sampler_state_count = effect->params[par].value.value_count;
                    object->shader.samplers[curSampler].sampler_states = effect->params[par].value.valuesSS;
                    curSampler++;
                } // if
            } // for
            if (object->shader.shader->preshader)
            {
                object->shader.preshader_param_count = object->shader.shader->preshader->symbol_count;
                object->shader.preshader_params = (uint32 *) m(object->shader.preshader_param_count * sizeof (uint32), d);
                for (j = 0; j < object->shader.shader->preshader->symbol_count; j++)
                {
                    object->shader.preshader_params[j] = findparameter(effect->params,
                                                                       effect->param_count,
                                                                       object->shader.shader->preshader->symbols[j].name);
                } // for
            } // if
        } // else if
        else
        {
            assert(0 && "Small object type unknown!");
        } // else

        /* Object block is always a multiple of four */
        const uint32 blocklen = (length + 3) - ((length - 1) % 4);
        *ptr += blocklen;
        *len -= blocklen;
    } // for
} // readstrings

static void readlargeobjects(const uint32 numlargeobjects,
                             const uint32 numsmallobjects,
                             const uint8 **ptr,
                             uint32 *len,
                             MOJOSHADER_effect *effect,
                             const char *profile,
                             const MOJOSHADER_swizzle *swiz,
                             const unsigned int swizcount,
                             const MOJOSHADER_samplerMap *smap,
                             const unsigned int smapcount,
                             MOJOSHADER_malloc m,
                             MOJOSHADER_free f,
                             void *d)
{
    int i, j;
    if (numlargeobjects == 0) return;

    int numobjects = numsmallobjects + numlargeobjects + 1;
    for (i = numsmallobjects + 1; i < numobjects; i++)
    {
        const uint32 technique = readui32(ptr, len);
        const uint32 index = readui32(ptr, len);
        /*const uint32 FIXME =*/ readui32(ptr, len);
        const uint32 state = readui32(ptr, len);
        const uint32 type = readui32(ptr, len);
        const uint32 length = readui32(ptr, len);

        uint32 objectIndex;
        if (technique == -1)
            objectIndex = effect->params[index].value.valuesSS[state].value.valuesI[0];
        else
            objectIndex = effect->techniques[technique].passes[index].states[state].value.valuesI[0];

        MOJOSHADER_effectObject *object = &effect->objects[objectIndex];
        if (object->type == MOJOSHADER_SYMTYPE_PIXELSHADER
         || object->type == MOJOSHADER_SYMTYPE_VERTEXSHADER)
        {
            object->shader.technique = technique;
            object->shader.pass = index;

            const char *emitter = profile;
            if (type == 2)
            {
                /* This is a standalone preshader!
                 * It exists solely for effect passes that do not use a single
                 * vertex/fragment shader.
                 */
                object->shader.is_preshader = 1;
                const uint32 start = *((uint32 *) *ptr) + 4;
                const char *array = readstring(*ptr, 0, m, d);
                object->shader.param_count = 1;
                object->shader.params = (uint32 *) m(sizeof (uint32), d);
                object->shader.params[0] = findparameter(effect->params,
                                                         effect->param_count,
                                                         array);
                f((void *) array, d);
                object->shader.preshader = MOJOSHADER_parsePreshader(*ptr + start, length,
                                                                     m, f, d);
                // !!! FIXME: check for errors.
                object->shader.preshader_param_count = object->shader.preshader->symbol_count;
                object->shader.preshader_params = (uint32 *) m(object->shader.preshader_param_count * sizeof (uint32), d);
                for (j = 0; j < object->shader.preshader->symbol_count; j++)
                {
                    object->shader.preshader_params[j] = findparameter(effect->params,
                                                                       effect->param_count,
                                                                       object->shader.preshader->symbols[j].name);
                } // for
            } // if
            else
            {
                char mainfn[32];
                snprintf(mainfn, sizeof (mainfn), "ShaderFunction%u", (unsigned int) objectIndex);
                object->shader.shader = MOJOSHADER_parse(emitter, mainfn, *ptr, length,
                                                         swiz, swizcount, smap, smapcount,
                                                         m, f, d);
                // !!! FIXME: check for errors.
                for (j = 0; j < object->shader.shader->symbol_count; j++)
                    if (object->shader.shader->symbols[j].register_set == MOJOSHADER_SYMREGSET_SAMPLER)
                        object->shader.sampler_count++;
                object->shader.param_count = object->shader.shader->symbol_count;
                object->shader.params = (uint32 *) m(object->shader.param_count * sizeof (uint32), d);
                object->shader.samplers = (MOJOSHADER_samplerStateRegister *) m(object->shader.sampler_count * sizeof (MOJOSHADER_samplerStateRegister), d);
                uint32 curSampler = 0;
                for (j = 0; j < object->shader.shader->symbol_count; j++)
                {
                    int par = findparameter(effect->params,
                                            effect->param_count,
                                            object->shader.shader->symbols[j].name);
                    object->shader.params[j] = par;
                    if (object->shader.shader->symbols[j].register_set == MOJOSHADER_SYMREGSET_SAMPLER)
                    {
                        object->shader.samplers[curSampler].sampler_name = effect->params[par].value.name;
                        object->shader.samplers[curSampler].sampler_register = object->shader.shader->symbols[j].register_index;
                        object->shader.samplers[curSampler].sampler_state_count = effect->params[par].value.value_count;
                        object->shader.samplers[curSampler].sampler_states = effect->params[par].value.valuesSS;
                        curSampler++;
                    } // if
                } // for
                if (object->shader.shader->preshader)
                {
                    object->shader.preshader_param_count = object->shader.shader->preshader->symbol_count;
                    object->shader.preshader_params = (uint32 *) m(object->shader.preshader_param_count * sizeof (uint32), d);
                    for (j = 0; j < object->shader.shader->preshader->symbol_count; j++)
                    {
                        object->shader.preshader_params[j] = findparameter(effect->params,
                                                                           effect->param_count,
                                                                           object->shader.shader->preshader->symbols[j].name);
                    } // for
                } // if
            }
        } // if
        else if (object->type == MOJOSHADER_SYMTYPE_TEXTURE
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE1D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE2D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURE3D
              || object->type == MOJOSHADER_SYMTYPE_TEXTURECUBE
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER1D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER2D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER3D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        {
            if (length > 0)
            {
                char *str = (char *) m(length, d);
                memcpy(str, *ptr, length);
                object->mapping.name = str;
            } // if
        } // else if
        else if (object->type != MOJOSHADER_SYMTYPE_VOID) // FIXME: Why? -flibit
        {
            assert(0 && "Large object type unknown!");
        } // else

        /* Object block is always a multiple of four */
        const uint32 blocklen = (length + 3) - ((length - 1) % 4);
        *ptr += blocklen;
        *len -= blocklen;
    } // for
} // readobjects

MOJOSHADER_effect *MOJOSHADER_parseEffect(const char *profile,
                                          const unsigned char *buf,
                                          const unsigned int _len,
                                          const MOJOSHADER_swizzle *swiz,
                                          const unsigned int swizcount,
                                          const MOJOSHADER_samplerMap *smap,
                                          const unsigned int smapcount,
                                          MOJOSHADER_malloc m,
                                          MOJOSHADER_free f,
                                          void *d)
{
    const uint8 *ptr = (const uint8 *) buf;
    uint32 len = (uint32) _len;

    /* Supply both m and f, or neither */
    if ( ((m == NULL) && (f != NULL)) || ((m != NULL) && (f == NULL)) )
        return &MOJOSHADER_out_of_mem_effect;

    /* Use default malloc/free if m/f were not passed */
    if (m == NULL) m = MOJOSHADER_internal_malloc;
    if (f == NULL) f = MOJOSHADER_internal_free;

    /* malloc base effect structure */
    MOJOSHADER_effect *retval = (MOJOSHADER_effect *) m(sizeof (MOJOSHADER_effect), d);
    if (retval == NULL)
        return &MOJOSHADER_out_of_mem_effect;
    memset(retval, '\0', sizeof (*retval));

    /* Store m/f/d in effect structure */
    retval->malloc = m;
    retval->free = f;
    retval->malloc_data = d;

    if (len < 8)
        goto parseEffect_unexpectedEOF;

    /* Read in header magic, seek to initial offset */
    const uint8 *base = NULL;
    uint32 header = readui32(&ptr, &len);
    if (header == 0xBCF00BCF)
    {
        /* The Effect compiler provided with XNA4 adds some extra mess at the
         * beginning of the file. It's useless though, so just skip it.
         * -flibit
         */
        const uint32 skip = readui32(&ptr, &len) - 8;
        ptr += skip;
        len += skip;
        header = readui32(&ptr, &len);
    } // if
    if (header != 0xFEFF0901)
        goto parseEffect_notAnEffectsFile;
    else
    {
        const uint32 offset = readui32(&ptr, &len);
        base = ptr;
        if (offset > len)
            goto parseEffect_unexpectedEOF;
        ptr += offset;
        len -= offset;
    } // else

    if (len < 16)
        goto parseEffect_unexpectedEOF;

    /* Parse structure counts */
    const uint32 numparams = readui32(&ptr, &len);
    const uint32 numtechniques = readui32(&ptr, &len);
    /*const uint32 FIXME =*/ readui32(&ptr, &len);
    const uint32 numobjects = readui32(&ptr, &len);

    /* Alloc structures now, so object types can be stored */
    retval->object_count = numobjects;
    const uint32 siz = sizeof (MOJOSHADER_effectObject) * numobjects;
    retval->objects = (MOJOSHADER_effectObject *) m(siz, d);
    if (retval->objects == NULL)
        goto parseEffect_outOfMemory;
    memset(retval->objects, '\0', siz);

    /* Parse effect parameters */
    retval->param_count = numparams;
    readparameters(numparams, base, &ptr, &len,
                   &retval->params, retval->objects,
                   m, d);

    /* Parse effect techniques */
    retval->technique_count = numtechniques;
    readtechniques(numtechniques, base, &ptr, &len,
                   &retval->techniques, retval->objects,
                   m, d);

    /* Initial effect technique/pass */
    retval->current_technique = &retval->techniques[0];
    retval->current_pass = -1;

    if (len < 8)
        goto parseEffect_unexpectedEOF;

    /* Parse object counts */
    const int numsmallobjects = readui32(&ptr, &len);
    const int numlargeobjects = readui32(&ptr, &len);

    /* Parse "small" object table */
    readsmallobjects(numsmallobjects, &ptr, &len,
                     retval,
                     profile, swiz, swizcount, smap, smapcount,
                     m, f, d);

    /* Parse "large" object table. */
    readlargeobjects(numlargeobjects, numsmallobjects, &ptr, &len,
                     retval,
                     profile, swiz, swizcount, smap, smapcount,
                     m, f, d);

    /* Store MojoShader profile in effect structure */
    retval->profile = (char *) m(strlen(profile) + 1, d);
    if (retval->profile == NULL)
        goto parseEffect_outOfMemory;
    strcpy((char *) retval->profile, profile);

    return retval;

// !!! FIXME: do something with this.
parseEffect_notAnEffectsFile:
parseEffect_unexpectedEOF:
parseEffect_outOfMemory:
    MOJOSHADER_freeEffect(retval);
    return &MOJOSHADER_out_of_mem_effect;
} // MOJOSHADER_parseEffect


void freetypeinfo(MOJOSHADER_symbolTypeInfo *typeinfo,
                  MOJOSHADER_free f, void *d)
{
    int i;
    for (i = 0; i < typeinfo->member_count; i++)
    {
        f((void *) typeinfo->members[i].name, d);
        freetypeinfo(&typeinfo->members[i].info, f, d);
    } // for
    f((void *) typeinfo->members, d);
} // freetypeinfo


void freevalue(MOJOSHADER_effectValue *value, MOJOSHADER_free f, void *d)
{
    int i;
    f((void *) value->name, d);
    f((void *) value->semantic, d);
    freetypeinfo(&value->type, f, d);
    if (value->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER
     || value->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER1D
     || value->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER2D
     || value->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER3D
     || value->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        for (i = 0; i < value->value_count; i++)
            freevalue(&value->valuesSS[i].value, f, d);
    f(value->values, d);
} // freevalue


void MOJOSHADER_freeEffect(const MOJOSHADER_effect *_effect)
{
    MOJOSHADER_effect *effect = (MOJOSHADER_effect *) _effect;
    if ((effect == NULL) || (effect == &MOJOSHADER_out_of_mem_effect))
        return;  // no-op.

    MOJOSHADER_free f = effect->free;
    void *d = effect->malloc_data;
    int i, j, k;

    /* Free errors */
    for (i = 0; i < effect->error_count; i++)
    {
        f((void *) effect->errors[i].error, d);
        f((void *) effect->errors[i].filename, d);
    } // for
    f((void *) effect->errors, d);

    /* Free profile string */
    f((void *) effect->profile, d);

    /* Free parameters, including annotations */
    for (i = 0; i < effect->param_count; i++)
    {
        MOJOSHADER_effectParam *param = &effect->params[i];
        freevalue(&param->value, f, d);
        for (j = 0; j < param->annotation_count; j++)
        {
            freevalue(&param->annotations[j], f, d);
        } // for
        f((void *) param->annotations, d);
    } // for
    f((void *) effect->params, d);

    /* Free techniques, including passes and all annotations */
    for (i = 0; i < effect->technique_count; i++)
    {
        MOJOSHADER_effectTechnique *technique = &effect->techniques[i];
        f((void *) technique->name, d);
        for (j = 0; j < technique->pass_count; j++)
        {
            MOJOSHADER_effectPass *pass = &technique->passes[j];
            f((void *) pass->name, d);
            for (k = 0; k < pass->state_count; k++)
            {
                freevalue(&pass->states[k].value, f, d);
            } // for
            f((void *) pass->states, d);
            for (k = 0; k < pass->annotation_count; k++)
            {
                freevalue(&pass->annotations[k], f, d);
            } // for
            f((void *) pass->annotations, d);
        } // for
        f((void *) technique->passes, d);
        for (j = 0; j < technique->annotation_count; j++)
        {
            freevalue(&technique->annotations[j], f, d);
        } // for
        f((void *) technique->annotations, d);
    } // for
    f((void *) effect->techniques, d);

    /* Free object table */
    for (i = 0; i < effect->object_count; i++)
    {
        MOJOSHADER_effectObject *object = &effect->objects[i];
        if (object->type == MOJOSHADER_SYMTYPE_PIXELSHADER
         || object->type == MOJOSHADER_SYMTYPE_VERTEXSHADER)
        {
            if (object->shader.is_preshader)
                MOJOSHADER_freePreshader(object->shader.preshader);
            else
                MOJOSHADER_freeParseData(object->shader.shader);
            f((void *) object->shader.params, d);
            f((void *) object->shader.samplers, d);
            f((void *) object->shader.preshader_params, d);
        } // if
        else if (object->type == MOJOSHADER_SYMTYPE_SAMPLER
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER1D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER2D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLER3D
              || object->type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
            f((void *) object->mapping.name, d);
        else if (object->type == MOJOSHADER_SYMTYPE_STRING)
            f((void *) object->string.string, d);
    } // for
    f((void *) effect->objects, d);

    /* Free base effect structure */
    f((void *) effect, d);
} // MOJOSHADER_freeEffect


// !!! FIXME: Out of memory check!
#define COPY_STRING(location) \
    if (src->location != NULL) \
    { \
        siz = strlen(src->location) + 1; \
        stringcopy = (char *) m(siz, d); \
        strcpy(stringcopy, src->location); \
        dst->location = stringcopy; \
    } // if


void copysymboltypeinfo(MOJOSHADER_symbolTypeInfo *dst,
                        MOJOSHADER_symbolTypeInfo *src,
                        MOJOSHADER_malloc m,
                        void *d)
{
    int i;
    uint32 siz = 0;
    char *stringcopy = NULL;
    memcpy(dst, src, sizeof (MOJOSHADER_symbolTypeInfo));
    if (dst->member_count > 0)
    {
        siz = dst->member_count * sizeof (MOJOSHADER_symbolStructMember);
        dst->members = (MOJOSHADER_symbolStructMember *) m(siz, d);
        for (i = 0; i < dst->member_count; i++)
        {
            COPY_STRING(members[i].name)
            copysymboltypeinfo(&dst->members[i].info, &src->members[i].info, m, d);
        } // for
    } // if
} // copysymboltypeinfo


void copyvalue(MOJOSHADER_effectValue *dst,
               MOJOSHADER_effectValue *src,
               MOJOSHADER_malloc m,
               void *d)
{
    int i;
    uint32 siz = 0;
    char *stringcopy = NULL;

    COPY_STRING(name)
    COPY_STRING(semantic)
    copysymboltypeinfo(&dst->type, &src->type, m, d);
    dst->value_count = src->value_count;

    if (dst->type.parameter_class == MOJOSHADER_SYMCLASS_SCALAR
     || dst->type.parameter_class == MOJOSHADER_SYMCLASS_VECTOR
     || dst->type.parameter_class == MOJOSHADER_SYMCLASS_MATRIX_ROWS
     || dst->type.parameter_class == MOJOSHADER_SYMCLASS_MATRIX_COLUMNS
     || dst->type.parameter_class == MOJOSHADER_SYMCLASS_STRUCT)
    {
        siz = dst->value_count * 4;
        dst->values = m(siz, d);
        // !!! FIXME: Out of memory check!
        memcpy(dst->values, src->values, siz);
    } // if
    else if (dst->type.parameter_class == MOJOSHADER_SYMCLASS_OBJECT)
    {
        if (dst->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER
         || dst->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER1D
         || dst->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER2D
         || dst->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLER3D
         || dst->type.parameter_type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        {
            siz = dst->value_count * sizeof (MOJOSHADER_effectSamplerState);
            dst->values = m(siz, d);
            // !!! FIXME: Out of memory check!
            memset(dst->values, '\0', siz);
            for (i = 0; i < dst->value_count; i++)
            {
                dst->valuesSS[i].type = src->valuesSS[i].type;
                copyvalue(&dst->valuesSS[i].value,
                          &src->valuesSS[i].value,
                          m, d);
            } // for
        } // if
        else
        {
            siz = dst->value_count * 4;
            dst->values = m(siz, d);
            // !!! FIXME: Out of memory check!
            memcpy(dst->values, src->values, siz);
        } // else
    } // else if

} // copyvalue


#undef COPY_STRING


void copysymbolinfo(MOJOSHADER_symbolTypeInfo *dst,
                    MOJOSHADER_symbolTypeInfo *src,
                    MOJOSHADER_malloc m,
                    void *d)
{
    int i;
    uint32 siz;
    char *stringcopy;

    dst->parameter_class = src->parameter_class;
    dst->parameter_type = src->parameter_type;
    dst->rows = src->rows;
    dst->columns = src->columns;
    dst->elements = src->elements;
    dst->member_count = src->member_count;

    if (dst->member_count > 0)
    {
        siz = sizeof (MOJOSHADER_symbolStructMember) * dst->member_count;
        dst->members = (MOJOSHADER_symbolStructMember *) m(siz, d);
        // !!! FIXME: Out of memory check!
        for (i = 0; i < dst->member_count; i++)
        {
            if (src->members[i].name != NULL)
            {
                siz = strlen(src->members[i].name) + 1;
                stringcopy = (char *) m(siz, d);
                strcpy(stringcopy, src->members[i].name);
                dst->members[i].name = stringcopy;
            } // if
            copysymbolinfo(&dst->members[i].info, &src->members[i].info, m, d);
        } // for
    } // if
} // copysymbolinfo


void copysymbol(MOJOSHADER_symbol *dst,
                MOJOSHADER_symbol *src,
                MOJOSHADER_malloc m,
                void *d)
{
    uint32 siz = strlen(src->name) + 1;
    char *stringcopy = (char *) m(siz, d);
    // !!! FIXME: Out of memory check!
    strcpy(stringcopy, src->name);
    dst->name = stringcopy;
    dst->register_set = src->register_set;
    dst->register_index = src->register_index;
    dst->register_count = src->register_count;
    copysymbolinfo(&dst->info, &src->info, m, d);
} // copysymbol


MOJOSHADER_preshader *copypreshader(const MOJOSHADER_preshader *src,
                                    MOJOSHADER_malloc m,
                                    void *d)
{
    int i, j;
    uint32 siz;
    MOJOSHADER_preshader *retval;

    retval = (MOJOSHADER_preshader *) m(sizeof (MOJOSHADER_preshader), d);
    // !!! FIXME: Out of memory check!
    memset(retval, '\0', sizeof (MOJOSHADER_preshader));

    siz = sizeof (double) * src->literal_count;
    retval->literal_count = src->literal_count;
    retval->literals = (double *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memcpy(retval->literals, src->literals, siz);

    retval->temp_count = src->temp_count;

    siz = sizeof (MOJOSHADER_symbol) * src->symbol_count;
    retval->symbol_count = src->symbol_count;
    retval->symbols = (MOJOSHADER_symbol *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->symbols, '\0', siz);

    for (i = 0; i < retval->symbol_count; i++)
        copysymbol(&retval->symbols[i], &src->symbols[i], m, d);

    siz = sizeof (MOJOSHADER_preshaderInstruction) * src->instruction_count;
    retval->instruction_count = src->instruction_count;
    retval->instructions = (MOJOSHADER_preshaderInstruction *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memcpy(retval->instructions, src->instructions, siz);
    for (i = 0; i < retval->instruction_count; i++)
        for (j = 0; j < retval->instructions[i].operand_count; j++)
        {
            siz = sizeof (unsigned int) * retval->instructions[i].operands[j].array_register_count;
            retval->instructions[i].operands[j].array_registers = (unsigned int *) m(siz, d);
            // !!! FIXME: Out of memory check!
            memcpy(retval->instructions[i].operands[j].array_registers,
                   src->instructions[i].operands[j].array_registers,
                   siz);
        } // for

    siz = sizeof (float) * 4 * src->register_count;
    retval->register_count = src->register_count;
    retval->registers = (float *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memcpy(retval->registers, src->registers, siz);

    return retval;
} // copypreshader


MOJOSHADER_parseData *copyparsedata(const MOJOSHADER_parseData *src,
                                    MOJOSHADER_malloc m,
                                    void *d)
{
    int i;
    uint32 siz;
    char *stringcopy;
    MOJOSHADER_parseData *retval;

    retval = (MOJOSHADER_parseData *) m(sizeof (MOJOSHADER_parseData), d);
    memset(retval, '\0', sizeof (MOJOSHADER_parseData));

    /* Copy malloc/free */
    retval->malloc = src->malloc;
    retval->free = src->free;
    retval->malloc_data = src->malloc_data;

    // !!! FIXME: Out of memory check!
    #define COPY_STRING(location) \
        siz = strlen(src->location) + 1; \
        stringcopy = (char *) m(siz, d); \
        strcpy(stringcopy, src->location); \
        retval->location = stringcopy; \

    /* Copy errors */
    siz = sizeof (MOJOSHADER_error) * src->error_count;
    retval->error_count = src->error_count;
    retval->errors = (MOJOSHADER_error *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->errors, '\0', siz);
    for (i = 0; i < retval->error_count; i++)
    {
        COPY_STRING(errors[i].error)
        COPY_STRING(errors[i].filename)
        retval->errors[i].error_position = src->errors[i].error_position;
    } // for

    /* Copy profile string constant */
    retval->profile = src->profile;

    /* Copy shader output */
    retval->output_len = src->output_len;
    stringcopy = (char *) m(src->output_len, d);
    memcpy(stringcopy, src->output, src->output_len);
    retval->output = stringcopy;

    /* Copy miscellaneous shader info */
    retval->instruction_count = src->instruction_count;
    retval->shader_type = src->shader_type;
    retval->major_ver = src->major_ver;
    retval->minor_ver = src->minor_ver;

    /* Copy uniforms */
    siz = sizeof (MOJOSHADER_uniform) * src->uniform_count;
    retval->uniform_count = src->uniform_count;
    retval->uniforms = (MOJOSHADER_uniform *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->uniforms, '\0', siz);
    for (i = 0; i < retval->uniform_count; i++)
    {
        retval->uniforms[i].type = src->uniforms[i].type;
        retval->uniforms[i].index = src->uniforms[i].index;
        retval->uniforms[i].array_count = src->uniforms[i].array_count;
        retval->uniforms[i].constant = src->uniforms[i].constant;
        COPY_STRING(uniforms[i].name)
    } // for

    /* Copy constants */
    siz = sizeof (MOJOSHADER_constant) * src->constant_count;
    retval->constant_count = src->constant_count;
    retval->constants = (MOJOSHADER_constant *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memcpy(retval->constants, src->constants, siz);

    /* Copy samplers */
    siz = sizeof (MOJOSHADER_sampler) * src->sampler_count;
    retval->sampler_count = src->sampler_count;
    retval->samplers = (MOJOSHADER_sampler *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->samplers, '\0', siz);
    for (i = 0; i < retval->sampler_count; i++)
    {
        retval->samplers[i].type = src->samplers[i].type;
        retval->samplers[i].index = src->samplers[i].index;
        COPY_STRING(samplers[i].name)
        retval->samplers[i].texbem = src->samplers[i].texbem;
    } // for

    /* Copy attributes */
    siz = sizeof (MOJOSHADER_attribute) * src->attribute_count;
    retval->attribute_count = src->attribute_count;
    retval->attributes = (MOJOSHADER_attribute *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->attributes, '\0', siz);
    for (i = 0; i < retval->attribute_count; i++)
    {
        retval->attributes[i].usage = src->attributes[i].usage;
        retval->attributes[i].index = src->attributes[i].index;
        COPY_STRING(attributes[i].name)
    } // for

    /* Copy outputs */
    siz = sizeof (MOJOSHADER_attribute) * src->output_count;
    retval->output_count = src->output_count;
    retval->outputs = (MOJOSHADER_attribute *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->outputs, '\0', siz);
    for (i = 0; i < retval->output_count; i++)
    {
        retval->outputs[i].usage = src->outputs[i].usage;
        retval->outputs[i].index = src->outputs[i].index;
        COPY_STRING(outputs[i].name)
    } // for

    #undef COPY_STRING

    /* Copy swizzles */
    siz = sizeof (MOJOSHADER_swizzle) * src->swizzle_count;
    retval->swizzle_count = src->swizzle_count;
    retval->swizzles = (MOJOSHADER_swizzle *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memcpy(retval->swizzles, src->swizzles, siz);

    /* Copy symbols */
    siz = sizeof (MOJOSHADER_symbol) * src->symbol_count;
    retval->symbol_count = src->symbol_count;
    retval->symbols = (MOJOSHADER_symbol *) m(siz, d);
    // !!! FIXME: Out of memory check!
    memset(retval->symbols, '\0', siz);
    for (i = 0; i < retval->symbol_count; i++)
        copysymbol(&retval->symbols[i], &src->symbols[i], m, d);

    /* Copy preshader */
    if (src->preshader != NULL)
        retval->preshader = copypreshader(src->preshader, m, d);

    return retval;
} // copyparsedata


MOJOSHADER_effect *MOJOSHADER_cloneEffect(const MOJOSHADER_effect *effect)
{
    int i, j, k;
    MOJOSHADER_effect *clone;
    MOJOSHADER_malloc m = effect->malloc;
    void *d = effect->malloc_data;
    uint32 siz = 0;
    char *stringcopy = NULL;
    uint32 curSampler;

    if ((effect == NULL) || (effect == &MOJOSHADER_out_of_mem_effect))
        return NULL;  // no-op.

    clone = (MOJOSHADER_effect *) m(sizeof (MOJOSHADER_effect), d);
    if (clone == NULL)
        return NULL; // Maybe out_of_mem_effect instead?
    memset(clone, '\0', sizeof (MOJOSHADER_effect));

    /* Copy malloc/free */
    clone->malloc = effect->malloc;
    clone->free = effect->free;
    clone->malloc_data = effect->malloc_data;

    #define COPY_STRING(location) \
        siz = strlen(effect->location) + 1; \
        stringcopy = (char *) m(siz, d); \
        if (stringcopy == NULL) \
            goto cloneEffect_outOfMemory; \
        strcpy(stringcopy, effect->location); \
        clone->location = stringcopy; \

    /* Copy errors */
    siz = sizeof (MOJOSHADER_error) * effect->error_count;
    clone->error_count = effect->error_count;
    clone->errors = (MOJOSHADER_error *) m(siz, d);
    if (clone->errors == NULL)
        goto cloneEffect_outOfMemory;
    memset(clone->errors, '\0', siz);
    for (i = 0; i < clone->error_count; i++)
    {
        COPY_STRING(errors[i].error)
        COPY_STRING(errors[i].filename)
        clone->errors[i].error_position = effect->errors[i].error_position;
    } // for

    /* Copy profile string */
    COPY_STRING(profile)

    /* Copy parameters */
    siz = sizeof (MOJOSHADER_effectParam) * effect->param_count;
    clone->param_count = effect->param_count;
    clone->params = (MOJOSHADER_effectParam *) m(siz, d);
    if (clone->params == NULL)
        goto cloneEffect_outOfMemory;
    memset(clone->params, '\0', siz);
    for (i = 0; i < clone->param_count; i++)
    {
        copyvalue(&clone->params[i].value, &effect->params[i].value, m, d);

        /* Copy parameter annotations */
        siz = sizeof (MOJOSHADER_effectAnnotation) * effect->params[i].annotation_count;
        clone->params[i].annotation_count = effect->params[i].annotation_count;
        clone->params[i].annotations = (MOJOSHADER_effectAnnotation *) m(siz, d);
        if (clone->params[i].annotations == NULL)
            goto cloneEffect_outOfMemory;
        memset(clone->params[i].annotations, '\0', siz);
        for (j = 0; j < clone->params[i].annotation_count; j++)
            copyvalue(&clone->params[i].annotations[j],
                      &effect->params[i].annotations[j],
                      m, d);
    } // for

    /* Copy techniques */
    siz = sizeof (MOJOSHADER_effectTechnique) * effect->technique_count;
    clone->technique_count = effect->technique_count;
    clone->techniques = (MOJOSHADER_effectTechnique *) m(siz, d);
    if (clone->techniques == NULL)
        goto cloneEffect_outOfMemory;
    memset(clone->techniques, '\0', siz);
    for (i = 0; i < clone->technique_count; i++)
    {
        COPY_STRING(techniques[i].name)

        /* Copy passes */
        siz = sizeof (MOJOSHADER_effectPass) * effect->techniques[i].pass_count;
        clone->techniques[i].pass_count = effect->techniques[i].pass_count;
        clone->techniques[i].passes = (MOJOSHADER_effectPass *) m(siz, d);
        if (clone->techniques[i].passes == NULL)
            goto cloneEffect_outOfMemory;
        memset(clone->techniques[i].passes, '\0', siz);
        for (j = 0; j < clone->techniques[i].pass_count; j++)
        {
            COPY_STRING(techniques[i].passes[j].name)

            /* Copy pass states */
            siz = sizeof (MOJOSHADER_effectState) * effect->techniques[i].passes[j].state_count;
            clone->techniques[i].passes[j].state_count = effect->techniques[i].passes[j].state_count;
            clone->techniques[i].passes[j].states = (MOJOSHADER_effectState *) m(siz, d);
            if (clone->techniques[i].passes[j].states == NULL)
                goto cloneEffect_outOfMemory;
            memset(clone->techniques[i].passes[j].states, '\0', siz);
            for (k = 0; k < clone->techniques[i].passes[j].state_count; k++)
            {
                clone->techniques[i].passes[j].states[k].type = effect->techniques[i].passes[j].states[k].type;
                copyvalue(&clone->techniques[i].passes[j].states[k].value,
                          &effect->techniques[i].passes[j].states[k].value,
                          m, d);
            } // for

            /* Copy pass annotations */
            siz = sizeof (MOJOSHADER_effectAnnotation) * effect->techniques[i].passes[j].annotation_count;
            clone->techniques[i].passes[j].annotation_count = effect->techniques[i].passes[j].annotation_count;
            clone->techniques[i].passes[j].annotations = (MOJOSHADER_effectAnnotation *) m(siz, d);
            if (clone->techniques[i].passes[j].annotations == NULL)
                goto cloneEffect_outOfMemory;
            memset(clone->techniques[i].passes[j].annotations, '\0', siz);
            for (k = 0; k < clone->techniques[i].passes[j].annotation_count; k++)
                copyvalue(&clone->techniques[i].passes[j].annotations[k],
                          &effect->techniques[i].passes[j].annotations[k],
                          m, d);
        } // for

        /* Copy technique annotations */
        siz = sizeof (MOJOSHADER_effectAnnotation) * effect->techniques[i].annotation_count;
        clone->techniques[i].annotation_count = effect->techniques[i].annotation_count;
        clone->techniques[i].annotations = (MOJOSHADER_effectAnnotation *) m(siz, d);
        if (clone->techniques[i].annotations == NULL)
            goto cloneEffect_outOfMemory;
        memset(clone->techniques[i].annotations, '\0', siz);
        for (j = 0; j < clone->techniques[i].annotation_count; j++)
            copyvalue(&clone->techniques[i].annotations[j],
                      &effect->techniques[i].annotations[j],
                      m, d);
    } // for

    /* Copy the current technique/pass */
    for (i = 0; i < effect->technique_count; i++)
        if (&effect->techniques[i] == effect->current_technique)
        {
            clone->current_technique = &clone->techniques[i];
            break;
        } // if
    assert(clone->current_technique != NULL);
    clone->current_pass = effect->current_pass;
    assert(clone->current_pass == -1);

    /* Copy object table */
    siz = sizeof (MOJOSHADER_effectObject) * effect->object_count;
    clone->object_count = effect->object_count;
    clone->objects = (MOJOSHADER_effectObject *) m(siz, d);
    if (clone->objects == NULL)
        goto cloneEffect_outOfMemory;
    memset(clone->objects, '\0', siz);
    for (i = 0; i < clone->object_count; i++)
    {
        clone->objects[i].type = effect->objects[i].type;
        if (clone->objects[i].type == MOJOSHADER_SYMTYPE_PIXELSHADER
         || clone->objects[i].type == MOJOSHADER_SYMTYPE_VERTEXSHADER)
        {
            clone->objects[i].shader.technique = effect->objects[i].shader.technique;
            clone->objects[i].shader.pass = effect->objects[i].shader.pass;
            clone->objects[i].shader.is_preshader = effect->objects[i].shader.is_preshader;
            siz = sizeof (uint32) * effect->objects[i].shader.preshader_param_count;
            clone->objects[i].shader.preshader_param_count = effect->objects[i].shader.preshader_param_count;
            clone->objects[i].shader.preshader_params = (uint32 *) m(siz, d);
            memcpy(clone->objects[i].shader.preshader_params,
                   effect->objects[i].shader.preshader_params,
                   siz);
            siz = sizeof (uint32) * effect->objects[i].shader.param_count;
            clone->objects[i].shader.param_count = effect->objects[i].shader.param_count;
            clone->objects[i].shader.params = (uint32 *) m(siz, d);
            memcpy(clone->objects[i].shader.params,
                   effect->objects[i].shader.params,
                   siz);

            if (clone->objects[i].shader.is_preshader)
            {
                clone->objects[i].shader.preshader = copypreshader(effect->objects[i].shader.preshader,
                                                                   m, d);
                continue;
            } // if

            clone->objects[i].shader.shader = copyparsedata(effect->objects[i].shader.shader,
                                                            m, d);

            siz = sizeof (MOJOSHADER_samplerStateRegister) * effect->objects[i].shader.sampler_count;
            clone->objects[i].shader.sampler_count = effect->objects[i].shader.sampler_count;
            clone->objects[i].shader.samplers = (MOJOSHADER_samplerStateRegister *) m(siz, d);
            if (clone->objects[i].shader.samplers == NULL)
                goto cloneEffect_outOfMemory;
            curSampler = 0;
            for (j = 0; j < clone->objects[i].shader.shader->symbol_count; j++)
                if (clone->objects[i].shader.shader->symbols[j].register_set == MOJOSHADER_SYMREGSET_SAMPLER)
                {
                    clone->objects[i].shader.samplers[curSampler].sampler_name = clone->params[clone->objects[i].shader.params[j]].value.name;
                    clone->objects[i].shader.samplers[curSampler].sampler_register = clone->objects[i].shader.shader->symbols[j].register_index;
                    clone->objects[i].shader.samplers[curSampler].sampler_state_count = clone->params[clone->objects[i].shader.params[j]].value.value_count;
                    clone->objects[i].shader.samplers[curSampler].sampler_states = clone->params[clone->objects[i].shader.params[j]].value.valuesSS;
                    curSampler++;
                } // if
        } // if
        else if (clone->objects[i].type == MOJOSHADER_SYMTYPE_SAMPLER
              || clone->objects[i].type == MOJOSHADER_SYMTYPE_SAMPLER1D
              || clone->objects[i].type == MOJOSHADER_SYMTYPE_SAMPLER2D
              || clone->objects[i].type == MOJOSHADER_SYMTYPE_SAMPLER3D
              || clone->objects[i].type == MOJOSHADER_SYMTYPE_SAMPLERCUBE)
        {
            COPY_STRING(objects[i].mapping.name)
        } // else if
        else if (clone->objects[i].type == MOJOSHADER_SYMTYPE_STRING)
        {
            COPY_STRING(objects[i].string.string)
        } // else if
    } // for

    #undef COPY_STRING

    return clone;

cloneEffect_outOfMemory:
    MOJOSHADER_freeEffect(clone);
    return NULL;
} // MOJOSHADER_cloneEffect


void MOJOSHADER_effectSetRawValueHandle(const MOJOSHADER_effectParam *parameter,
                                        const void *data,
                                        const unsigned int offset,
                                        const unsigned int len)
{
    // !!! FIXME: char* case is arbitary, for Win32 -flibit
    memcpy((char *) parameter->value.values + offset, data, len);
} // MOJOSHADER_effectSetRawValueHandle


void MOJOSHADER_effectSetRawValueName(const MOJOSHADER_effect *effect,
                                      const char *name,
                                      const void *data,
                                      const unsigned int offset,
                                      const unsigned int len)
{
    int i;
    for (i = 0; i < effect->param_count; i++)
    {
        if (strcmp(name, effect->params[i].value.name) == 0)
        {
            // !!! FIXME: char* case is arbitary, for Win32 -flibit
            memcpy((char *) effect->params[i].value.values + offset, data, len);
            return;
        } // if
    } // for
    assert(0 && "Effect parameter not found!");
} // MOJOSHADER_effectSetRawValueName


const MOJOSHADER_effectTechnique *MOJOSHADER_effectGetCurrentTechnique(const MOJOSHADER_effect *effect)
{
    return effect->current_technique;
} // MOJOSHADER_effectGetCurrentTechnique


void MOJOSHADER_effectSetTechnique(MOJOSHADER_effect *effect,
                                   const MOJOSHADER_effectTechnique *technique)
{
    int i;
    for (i = 0; i < effect->technique_count; i++)
    {
        if (technique == &effect->techniques[i])
        {
            effect->current_technique = technique;
            return;
        } // if
    } // for
    assert(0 && "Technique is not part of this effect!");
} // MOJOSHADER_effectSetTechnique


const MOJOSHADER_effectTechnique *MOJOSHADER_effectFindNextValidTechnique(const MOJOSHADER_effect *effect,
                                                                          const MOJOSHADER_effectTechnique *technique
)
{
    int i;
    if (technique == NULL)
        return &effect->techniques[0];
    for (i = 0; i < effect->technique_count; i++)
    {
        if (technique == &effect->techniques[i])
        {
            if (i == effect->technique_count - 1)
                return NULL; /* We were passed the last technique! */
            return &effect->techniques[i + 1];
        } // if
    } // for
    assert(0 && "Technique is not part of this effect!");
} // MOJOSHADER_effectFindNextValidTechnique

#endif // MOJOSHADER_EFFECT_SUPPORT

// end of mojoshader_effects.c ...

