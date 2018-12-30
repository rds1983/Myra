/* Theorafile - Ogg Theora Video Decoder Library
 *
 * Copyright (c) 2017 Ethan Lee.
 * Based on TheoraPlay, Copyright (c) 2011-2016 Ryan C. Gordon.
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

#ifndef THEORAFILE_H
#define THEORAFILE_H

#include <theora/theoradec.h>
#include <vorbis/codec.h>

#ifdef __cplusplus
extern "C"
{
#endif /* __cplusplus */

#ifndef DECLSPEC
#if defined(_WIN32)
#define DECLSPEC __declspec(dllexport)
#else
#define DECLSPEC
#endif
#endif

/* I/O Handle + Callbacks */
typedef struct tf_callbacks
{
	size_t (*read_func) (void *ptr, size_t size, size_t nmemb, void *datasource);
	int (*seek_func) (void *datasource, ogg_int64_t offset, int origin);
	int (*close_func) (void *datasource);
} tf_callbacks;

/* File Handle */
typedef struct OggTheora_File
{
	/* Current State */
	ogg_sync_state sync;
	ogg_page page;
	int eos;

	/* Stream Data */
	int tpackets;
	int vpackets;
	ogg_stream_state tstream;
	ogg_stream_state vstream;

	/* Metadata */
	th_info tinfo;
	vorbis_info vinfo;
	th_comment tcomment;
	vorbis_comment vcomment;

	/* Theora Data */
	th_dec_ctx *tdec;

	/* Vorbis Data */
	int vdsp_init;
	vorbis_dsp_state vdsp;
	int vblock_init;
	vorbis_block vblock;

	/* I/O Data */
	tf_callbacks io;
	void *datasource;
} OggTheora_File;

/* Open/Close */
#define TF_EUNKNOWN		-1
#define TF_EUNSUPPORTED		-2
#define TF_ENODATASOURCE	-3
DECLSPEC int tf_open_callbacks(
	void *datasource,
	OggTheora_File *file,
	tf_callbacks io
);
DECLSPEC int tf_fopen(
	const char *fname,
	OggTheora_File *file
);
DECLSPEC void tf_close(OggTheora_File *file);

/* File Info */
DECLSPEC int tf_hasvideo(OggTheora_File *file);
DECLSPEC int tf_hasaudio(OggTheora_File *file);
DECLSPEC void tf_videoinfo(
	OggTheora_File *file,
	int *width,
	int *height,
	double *fps
);
DECLSPEC void tf_audioinfo(
	OggTheora_File *file,
	int *channels,
	int *samplerate
);

/* Stream State */
DECLSPEC int tf_eos(OggTheora_File *file);
DECLSPEC void tf_reset(OggTheora_File *file);

/* Data Reading */
DECLSPEC int tf_readvideo(OggTheora_File *file, char *buffer, int numframes);
DECLSPEC int tf_readaudio(OggTheora_File *file, float *buffer, int length);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* THEORAFILE_H */
