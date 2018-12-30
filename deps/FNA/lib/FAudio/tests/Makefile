# targets:
#     crosstest: build Windows executable for verifying xaudio2 behavior on Windows
#     faudio_tests: build native executable for verifying FAudio behavior
#     test: build and execute tests against FAudio

WINEROOT = /usr/include/wine

CROSSCC = x86_64-w64-mingw32-gcc

LDFLAGS += -lFAudio

CFLAGS += -g -Wall

crosstest: faudio_tests.exe

faudio_tests.exe: xaudio2.c
	$(CROSSCC) $(CFLAGS) -Wall -I$(WINEROOT)/windows/ -o $@ $< -lole32

test: faudio_tests
	./faudio_tests

faudio_tests: xaudio2.c
	$(CC) $(CFLAGS) -I../src/ -o $@ $< $(LDFLAGS)
