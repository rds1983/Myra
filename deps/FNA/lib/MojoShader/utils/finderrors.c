/**
 * MojoShader; generate shader programs from bytecode of compiled
 *  Direct3D shaders.
 *
 * Please see the file LICENSE.txt in the source's root directory.
 *
 *  This file written by Ryan C. Gordon.
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>
#include <sys/types.h>
#include <errno.h>

#include "mojoshader.h"

#if FINDERRORS_COMPILE_SHADERS
#include "SDL.h"
static SDL_Window *sdlwindow = NULL;
static void *lookup(const char *fnname, void *unused)
{
    (void) unused;
    return SDL_GL_GetProcAddress(fnname);
} // lookup
#endif

#ifdef _MSC_VER
#define WIN32_LEAN_AND_MEAN 1
#include <windows.h>
#include <malloc.h>  // for alloca().
#define snprintf _snprintf
#else
#include <dirent.h>
#include <sys/stat.h>
#endif

#define report printf

static int isdir(const char *dname)
{
#ifdef _MSC_VER
    WIN32_FILE_ATTRIBUTE_DATA winstat;
    if (!GetFileAttributesExA(dname, GetFileExInfoStandard, &winstat))
        return 0;
    return winstat.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;
#else
    struct stat statbuf;
    if (stat(dname, &statbuf) == -1)
        return 0;
    return S_ISDIR(statbuf.st_mode);
#endif
}

static int do_dir(const char *dname, const char *profile);

static int do_file(const char *profile, const char *dname, const char *fn, int *total)
{
    int do_quit = 0;

    if ((strcmp(fn, ".") == 0) || (strcmp(fn, "..") == 0))
        return 1;  // skip these.

    #if FINDERRORS_COMPILE_SHADERS
    SDL_Event e;  // pump event queue to keep OS happy.
    while (SDL_PollEvent(&e))
    {
        if (e.type == SDL_QUIT)
            do_quit = 1;
    } // while
    SDL_GL_SwapWindow(sdlwindow);
    #endif

    if (do_quit)
    {
        report("FAIL: user requested quit!\n");
        return 0;
    } // if

    char *fname = (char *) alloca(strlen(fn) + strlen(dname) + 2);
    sprintf(fname, "%s/%s", dname, fn);

    if (isdir(fname))
    {
        *total += do_dir(fname, profile);
        return 1;
    } // if

    int assembly = 0;
    if (strstr(fn, ".bytecode") != NULL)
        assembly = 0;
    else if (strstr(fn, ".disasm") != NULL)
        assembly = 1;
    else
        return 1;

    (*total)++;

    FILE *io = fopen(fname, "rb");
    if (io == NULL)
    {
        report("FAIL: %s fopen() failed.\n", fname);
        return 1;
    } // if

    static unsigned char buf[1024 * 256];
    int rc = fread(buf, 1, sizeof (buf), io);
    fclose(io);
    if (rc == -1)
    {
        report("FAIL: %s %s\n", fname, strerror(errno));
        return 1;
    } // if

    if (assembly)
    {
        const MOJOSHADER_parseData *a;

        buf[rc] = '\0';  // make sure the source is null-terminated.
        a = MOJOSHADER_assemble(fname, (char *) buf, rc, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        if (a->error_count > 0)
        {
            report("FAIL: %s (line %d) %s\n",
                a->errors[0].filename ? a->errors[0].filename : "???",
                a->errors[0].error_position,
                a->errors[0].error);
            return 1;
        } // if

        else if (a->output_len > sizeof (buf))
        {
            report("FAIL: %s buffer overflow in finderrors.c\n", fname);
            return 1;
        } // if

        rc = a->output_len;
        memcpy(buf, a->output, rc);
        MOJOSHADER_freeParseData(a);
    } // if

    #if FINDERRORS_COMPILE_SHADERS
    MOJOSHADER_glShader *shader = MOJOSHADER_glCompileShader(buf, rc, NULL, 0, NULL, 0);
    if (shader == NULL)
        report("FAIL: %s %s\n", fname, MOJOSHADER_glGetError());
    else
    {
        const MOJOSHADER_parseData *pd = MOJOSHADER_glGetShaderParseData(shader);
        MOJOSHADER_glShader *v = (pd->shader_type == MOJOSHADER_TYPE_VERTEX) ? shader : NULL;
        MOJOSHADER_glShader *p = (pd->shader_type == MOJOSHADER_TYPE_PIXEL) ? shader : NULL;
        MOJOSHADER_glProgram *program = MOJOSHADER_glLinkProgram(v, p);
        if (program == NULL)
            report("FAIL: %s %s\n", fname, MOJOSHADER_glGetError());
        else
        {
            report("PASS: %s\n", fname);
            MOJOSHADER_glDeleteProgram(program);
        } // else
        MOJOSHADER_glDeleteShader(shader);
    }
    #else
    const MOJOSHADER_parseData *pd = MOJOSHADER_parse(profile, NULL, buf, rc, NULL, 0, NULL, 0, NULL, NULL, NULL);
    if (pd->error_count == 0)
        report("PASS: %s\n", fname);
	else
	{
		int i;
		for (i = 0; i < pd->error_count; i++)
		{
			report("FAIL: %s (position %d) %s\n", pd->errors[i].filename,
			       pd->errors[i].error_position, pd->errors[i].error);
		} // for
	} // else
    MOJOSHADER_freeParseData(pd);
    #endif

    return 1;
} // do_file


static int do_dir(const char *dname, const char *profile)
{
    int total = 0;

#ifdef _MSC_VER
	const size_t wildcardlen = strlen(dname) + 3;
	char *wildcard = (char *) alloca(wildcardlen);
	SDL_snprintf(wildcard, wildcardlen, "%s\\*", dname);

    WIN32_FIND_DATAA dent;
    HANDLE dirp = FindFirstFileA(wildcard, &dent);
    if (dirp != INVALID_HANDLE_VALUE)
    {
        do
        {
            if (!do_file(profile, dname, dent.cFileName, &total))
                break;
        } while (FindNextFileA(dirp, &dent) != 0);
        FindClose(dirp);
    } // if
#else
    struct dirent *dent = NULL;
    DIR *dirp = opendir(dname);
    if (dirp != NULL)
    {
        while ((dent = readdir(dirp)) != NULL)
        {
            if (!do_file(profile, dname, dent->d_name, &total))
                break;
        } // while
        closedir(dirp);
    } // if
#endif

    return total;
} // do_dir


int main(int argc, char **argv)
{
    //printf("MojoShader finderrors\n");
    //printf("Compiled against changeset %s\n", MOJOSHADER_CHANGESET);
    //printf("Linked against changeset %s\n", MOJOSHADER_changeset());
    //printf("\n");

    if (argc <= 2)
        printf("\n\nUSAGE: %s <profile> [dir1] ... [dirN]\n\n", argv[0]);
    else
    {
        int okay = 0;
        int total = 0;
        int i;
        const char *profile = argv[1];

        #if FINDERRORS_COMPILE_SHADERS
        MOJOSHADER_glContext *ctx = NULL;
        if (SDL_Init(SDL_INIT_VIDEO) == -1)
            fprintf(stderr, "SDL_Init() error: %s\n", SDL_GetError());
        else if (SDL_GL_LoadLibrary(NULL) == -1)
            fprintf(stderr, "SDL_GL_LoadLibrary() error: %s\n", SDL_GetError());
        else if ((sdlwindow = SDL_CreateWindow(argv[0], SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 640, 480, SDL_WINDOW_OPENGL)) == NULL)
            fprintf(stderr, "SDL_CreateWindow() error: %s\n", SDL_GetError());
        else if (SDL_GL_CreateContext(sdlwindow) == NULL)
            fprintf(stderr, "SDL_GL_CreateContext() error: %s\n", SDL_GetError());
        else if ((ctx = MOJOSHADER_glCreateContext(profile, lookup, 0, 0, 0, 0)) == NULL)
            fprintf(stderr, "MOJOSHADER_glCreateContext() fail: %s\n", MOJOSHADER_glGetError());
        else
        {
            printf("Best profile is '%s'\n", MOJOSHADER_glBestProfile(lookup, 0, NULL, NULL, NULL));
            MOJOSHADER_glMakeContextCurrent(ctx);
            okay = 1;
        }
        #else
        okay = 1;
        #endif

        if (okay)
        {
            for (i = 2; i < argc; i++)
                total += do_dir(argv[i], profile);
            printf("Saw %d files.\n", total);
        } // if

        #if FINDERRORS_COMPILE_SHADERS
        if (ctx)
            MOJOSHADER_glDestroyContext(ctx);
        SDL_Quit();
        #endif
    } // else

    return 0;
} // main

// end of finderrors.c ...

