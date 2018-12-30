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

#include <SDL.h>
#include <SDL_main.h>
#include <SDL_opengl.h>
#include "theorafile.h"

/* GL Function typedefs */
#define GL_PROC(ret, func, parms) \
	typedef ret (GLAPIENTRY *glfntype_##func) parms;
#include "glfuncs.h"
#undef GL_PROC
/* GL Function declarations */
#define GL_PROC(ret, func, parms) \
	glfntype_##func INTERNAL_##func;
#include "glfuncs.h"
#undef GL_PROC

static const GLchar *GLVert =
	"#version 110\n"
	"attribute vec2 pos;\n"
	"attribute vec2 tex;\n"
	"void main() {\n"
		"gl_Position = vec4(pos.xy, 0.0, 1.0);\n"
		"gl_TexCoord[0].xy = tex;\n"
	"}\n";

/* This shader was originally from SDL 1.3 */
static const GLchar *GLFrag =
	"#version 110\n"
	"uniform sampler2D samp0;\n"
	"uniform sampler2D samp1;\n"
	"uniform sampler2D samp2;\n"
	"const vec3 offset = vec3(-0.0625, -0.5, -0.5);\n"
	"const vec3 Rcoeff = vec3(1.164,  0.000,  1.596);\n"
	"const vec3 Gcoeff = vec3(1.164, -0.391, -0.813);\n"
	"const vec3 Bcoeff = vec3(1.164,  2.018,  0.000);\n"
	"void main() {\n"
	"	vec2 tcoord;\n"
	"	vec3 yuv, rgb;\n"
	"	tcoord = gl_TexCoord[0].xy;\n"
	"	yuv.x = texture2D(samp0, tcoord).r;\n"
	"	yuv.y = texture2D(samp1, tcoord).r;\n"
	"	yuv.z = texture2D(samp2, tcoord).r;\n"
	"	yuv += offset;\n"
	"	rgb.r = dot(yuv, Rcoeff);\n"
	"	rgb.g = dot(yuv, Gcoeff);\n"
	"	rgb.b = dot(yuv, Bcoeff);\n"
	"	gl_FragColor = vec4(rgb, 1.0);\n"
	"}\n";

void AudioCallback(void *userdata, Uint8* stream, int len)
{
	const int samples = len / 4;
	int read = tf_readaudio((OggTheora_File*) userdata, (float*) stream, samples);
	if (read < samples)
	{
		SDL_memset(stream + read * 4, '\0', (samples - read) * 4);
	}
}

int main(int argc, char **argv)
{
	/* SDL variables */
	SDL_Window *window;
	SDL_GLContext context;
	SDL_AudioDeviceID audio;
	SDL_AudioSpec spec;
	SDL_Event evt;
	Uint8 run = 1;

	/* Theorafile variables */
	OggTheora_File fileIn;
	int width, height, channels, samplerate;
	double fps;
	int curframe = 0, thisframe, newframe;
	char *frame = NULL;

	/* OpenGL variables */
	GLuint yuvTextures[3];
	GLuint vertex = 0;
	GLuint fragment = 0;
	GLuint program = 0;
	GLint shaderlen = 0;

	/* Vertex client arrays */
	static struct { float pos[2]; float tex[2]; } verts[4] = {
		{ { -1.0f,  1.0f }, { 0.0f, 0.0f } },
		{ {  1.0f,  1.0f }, { 1.0f, 0.0f } },
		{ { -1.0f, -1.0f }, { 0.0f, 1.0f } },
		{ {  1.0f, -1.0f }, { 1.0f, 1.0f } }
	};

	/* We need a file name! */
	if (argc < 2 || argc > 2)
	{
		SDL_Log("Need a file name!\n");
		return 1;
	}

	/* Open the Theora file */
	if (tf_fopen(argv[1], &fileIn) < 0)
	{
		SDL_Log("Failed to open file.\n");
		return 1;
	}

	/* This is a video test, people! */
	if (!tf_hasvideo(&fileIn))
	{
		SDL_Log("No video!\n");
		return 1;
	}

	/* Get the video metadata, allocate first frame */
	tf_videoinfo(&fileIn, &width, &height, &fps);
	frame = (char*) SDL_malloc(width * height * 2);
	while (!tf_readvideo(&fileIn, frame, 1));

	/* Create window (and audio device, if applicable) */
	SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO);
	window = SDL_CreateWindow(
		"Theorafile Test",
		SDL_WINDOWPOS_CENTERED,
		SDL_WINDOWPOS_CENTERED,
		width,
		height,
		SDL_WINDOW_OPENGL
	);
	context = SDL_GL_CreateContext(window);
	SDL_GL_SetSwapInterval(1);

	/* GL function loading */
	#define GL_PROC(ret, func, parms) \
		INTERNAL_##func = (glfntype_##func) SDL_GL_GetProcAddress(#func);
	#include "glfuncs.h"
	#undef GL_PROC

	/* Remap GL function names to internal entry points */
	#include "glmacros.h"

	if (tf_hasaudio(&fileIn))
	{
		/* Get the audio metadata, allocate queue */
		tf_audioinfo(&fileIn, &channels, &samplerate);
		SDL_zero(spec);
		spec.freq = samplerate;
		spec.format = AUDIO_F32;
		spec.channels = channels;
		spec.samples = 4096;
		spec.callback = AudioCallback;
		spec.userdata = &fileIn;
		audio = SDL_OpenAudioDevice(
			NULL,
			0,
			&spec,
			NULL,
			0
		);
		SDL_PauseAudioDevice(audio, 0);
	}

	/* Initial GL state */
	glDepthMask(GL_FALSE);
	glDisable(GL_DEPTH_TEST);
	glDisable(GL_ALPHA_TEST);
	glDisable(GL_BLEND);
	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

	/* YUV buffers */
	glGenTextures(3, yuvTextures);
	#define GEN_TEXTURE(index, w, h, ptr) \
		glActiveTexture(GL_TEXTURE0 + index); \
		glBindTexture(GL_TEXTURE_2D, yuvTextures[index]); \
		glTexImage2D( \
			GL_TEXTURE_2D, \
			0, \
			GL_LUMINANCE8, \
			w, \
			h, \
			0, \
			GL_LUMINANCE, \
			GL_UNSIGNED_BYTE, \
			ptr \
		); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_BASE_LEVEL, 0); \
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, 0);
	GEN_TEXTURE(
		0,
		width,
		height,
		frame
	)
	GEN_TEXTURE(
		1,
		width / 2,
		height / 2,
		frame + (width * height)
	)
	GEN_TEXTURE(
		2,
		width / 2,
		height / 2,
		frame + (width * height) + (width / 2 * height / 2)
	)
	#undef GEN_TEXTURE

	/* Vertex shader... */
	shaderlen = (GLint) SDL_strlen(GLVert);
	vertex = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vertex, 1, &GLVert, &shaderlen);
	glCompileShader(vertex);

	/* Fragment shader... */
	shaderlen = (GLint) SDL_strlen(GLFrag);
	fragment = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fragment, 1, &GLFrag, &shaderlen);
	glCompileShader(fragment);

	/* Program object... */
	program = glCreateProgram();
	glAttachShader(program, vertex);
	glAttachShader(program, fragment);
	glBindAttribLocation(program, 0, "pos");
	glBindAttribLocation(program, 1, "tex");
	glLinkProgram(program);
	glDeleteShader(vertex);
	glDeleteShader(fragment);

	/* ... Finally. */
	glUseProgram(program);
	glUniform1i(glGetUniformLocation(program, "samp0"), 0);
	glUniform1i(glGetUniformLocation(program, "samp1"), 1);
	glUniform1i(glGetUniformLocation(program, "samp2"), 2);

	/* Vertex buffers */
	glVertexAttribPointer(0, 2, GL_FLOAT, 0, sizeof (verts[0]), &verts[0].pos[0]);
	glVertexAttribPointer(1, 2, GL_FLOAT, 0, sizeof (verts[0]), &verts[0].tex[0]);
	glEnableVertexAttribArray(0);
	glEnableVertexAttribArray(1);

	while (run)
	{
		while (SDL_PollEvent(&evt))
		{
			if (evt.type == SDL_QUIT)
			{
				run = 0;
			}
			else if (evt.type == SDL_KEYDOWN)
			{
				/* Slowdown simulator */
				SDL_Delay(1000);
			}
		}

		/* Loop this video! */
		SDL_LockAudioDevice(audio);
		if (tf_eos(&fileIn))
		{
			tf_reset(&fileIn);
		}
		SDL_UnlockAudioDevice(audio);

		/* Based on when we started, what frame should we be on? */
		thisframe = (int) (SDL_GetTicks() / (1000.0 / fps));
		if (thisframe > curframe)
		{
			/* Keep reading frames until we're caught up */
			SDL_LockAudioDevice(audio);
			newframe = tf_readvideo(&fileIn, frame, thisframe - curframe);
			SDL_UnlockAudioDevice(audio);
			curframe = thisframe;

			/* Only update the textures if we need to! */
			if (newframe)
			{
				glActiveTexture(GL_TEXTURE0);
				glTexSubImage2D(
					GL_TEXTURE_2D,
					0,
					0,
					0,
					width,
					height,
					GL_LUMINANCE,
					GL_UNSIGNED_BYTE,
					frame
				);
				glActiveTexture(GL_TEXTURE1);
				glTexSubImage2D(
					GL_TEXTURE_2D,
					0,
					0,
					0,
					width / 2,
					height / 2,
					GL_LUMINANCE,
					GL_UNSIGNED_BYTE,
					frame + (width * height)
				);
				glActiveTexture(GL_TEXTURE2);
				glTexSubImage2D(
					GL_TEXTURE_2D,
					0,
					0,
					0,
					width / 2,
					height / 2,
					GL_LUMINANCE,
					GL_UNSIGNED_BYTE,
					frame + (width * height) + (width / 2 * height / 2)
				);
			}
		}

		/* Draw! */
		glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);
		SDL_GL_SwapWindow(window);
	}

	/* Clean up. We out. */
	glDeleteProgram(program);
	glDeleteTextures(3, yuvTextures);
	SDL_free(frame);
	if (tf_hasaudio(&fileIn))
	{
		SDL_CloseAudioDevice(audio);
	}
	tf_close(&fileIn);
	SDL_GL_DeleteContext(context);
	SDL_DestroyWindow(window);
	SDL_Quit();
	SDL_Log("Test complete.\n");
	return 0;
}
