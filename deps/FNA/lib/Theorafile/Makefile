# Makefile for Theorafile
# Written by Ethan "flibitijibibo" Lee

# System information
UNAME = $(shell uname)
ARCH = $(shell uname -m)

# Detect Windows target
WINDOWS_TARGET=0
ifeq ($(OS), Windows_NT) # cygwin/msys2
	WINDOWS_TARGET=1
endif
ifneq (,$(findstring w64-mingw32,$(CC))) # mingw-w64 on Linux
	WINDOWS_TARGET=1
endif

# Compiler
ifeq ($(WINDOWS_TARGET),1)
	TARGET = dll
	LDFLAGS += -static-libgcc
else ifeq ($(UNAME), Darwin)
	CC += -mmacosx-version-min=10.6
	TARGET = dylib
else
	TARGET = so
endif

# CPU Arch Flags
ifeq ($(ARCH), x86_64)
	DEFINES += -DOC_X86_ASM -DOC_X86_64_ASM
else # Assuming x86...
	DEFINES += -DOC_X86_ASM
endif

# Includes
INCLUDES = -I. -Ilib -Ilib/ogg -Ilib/vorbis -Ilib/theora

# Source
TFSRC = \
	theorafile.c \
	lib/ogg/bitwise.c \
	lib/ogg/framing.c \
	lib/vorbis/analysis.c \
	lib/vorbis/bitrate.c \
	lib/vorbis/block.c \
	lib/vorbis/codebook.c \
	lib/vorbis/envelope.c \
	lib/vorbis/floor0.c \
	lib/vorbis/floor1.c \
	lib/vorbis/vinfo.c \
	lib/vorbis/lookup.c \
	lib/vorbis/lpc.c \
	lib/vorbis/lsp.c \
	lib/vorbis/mapping0.c \
	lib/vorbis/mdct.c \
	lib/vorbis/psy.c \
	lib/vorbis/registry.c \
	lib/vorbis/res0.c \
	lib/vorbis/sharedbook.c \
	lib/vorbis/smallft.c \
	lib/vorbis/synthesis.c \
	lib/vorbis/window.c \
	lib/theora/apiwrapper.c \
	lib/theora/bitpack.c \
	lib/theora/decapiwrapper.c \
	lib/theora/decinfo.c \
	lib/theora/decode.c \
	lib/theora/dequant.c \
	lib/theora/fragment.c \
	lib/theora/huffdec.c \
	lib/theora/idct.c \
	lib/theora/tinfo.c \
	lib/theora/internal.c \
	lib/theora/quant.c \
	lib/theora/state.c \
	lib/theora/x86/mmxfrag.c \
	lib/theora/x86/mmxidct.c \
	lib/theora/x86/mmxstate.c \
	lib/theora/x86/x86state.c

# Targets
all: clean
	$(CC) -O3 -fpic -fPIC -shared -o libtheorafile.$(TARGET) $(TFSRC) $(INCLUDES) $(DEFINES) -lm $(LDFLAGS)

clean:
	rm -f libtheorafile.$(TARGET)

test:
	$(CC) -g -o theorafile-test sdl2test/sdl2test.c $(TFSRC) $(INCLUDES) $(DEFINES) `sdl2-config --cflags --libs` -lm
