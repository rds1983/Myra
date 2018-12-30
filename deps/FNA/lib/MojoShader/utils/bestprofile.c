/**
 * MojoShader; generate shader programs from bytecode of compiled
 *  Direct3D shaders.
 *
 * Please see the file LICENSE.txt in the source's root directory.
 *
 *  This file written by Ryan C. Gordon.
 */

#include <stdio.h>
#include "mojoshader.h"
#include "SDL.h"

static void *lookup(const char *fnname, void *unused)
{
    (void) unused;
    return SDL_GL_GetProcAddress(fnname);
} // lookup

int main(int argc, char **argv)
{
    int retval = 1;

    #if 0
    printf("MojoShader bestprofile\n");
    printf("Compiled against changeset %s\n", MOJOSHADER_CHANGESET);
    printf("Linked against changeset %s\n", MOJOSHADER_changeset());
    printf("\n");
    #endif

    SDL_Window *sdlwindow = NULL;
    if (SDL_Init(SDL_INIT_VIDEO) == -1)
        fprintf(stderr, "SDL_Init() error: %s\n", SDL_GetError());
    else if (SDL_GL_LoadLibrary(NULL) == -1)
        fprintf(stderr, "SDL_GL_LoadLibrary() error: %s\n", SDL_GetError());
    else if ((sdlwindow = SDL_CreateWindow(argv[0], SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 640, 480, SDL_WINDOW_OPENGL)) == NULL)
        fprintf(stderr, "SDL_CreateWindow() error: %s\n", SDL_GetError());
    else if (SDL_GL_CreateContext(sdlwindow) == NULL)
        fprintf(stderr, "SDL_GL_CreateContext() error: %s\n", SDL_GetError());
    else
    {
        const char *best = MOJOSHADER_glBestProfile(lookup, NULL, NULL, NULL, NULL);
        MOJOSHADER_glContext *ctx = MOJOSHADER_glCreateContext(best, lookup, 0, 0, 0, 0);
        if (ctx == NULL)
            printf("MOJOSHADER_glCreateContext() fail: %s\n", MOJOSHADER_glGetError());
        else
        {
            printf("%s\n", best);
            retval = 0;  // success.
            MOJOSHADER_glDestroyContext(ctx);
        } // else
    } // else

    SDL_Quit();
    return retval;
} // main

// end of bestprofile.c ...

