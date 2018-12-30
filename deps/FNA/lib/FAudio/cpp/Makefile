CXXFLAGS += -I../src -DFAUDIOCPP_EXPORTS -D_WIN32_WINNT=0x0600
LDFLAGS += -Wl,--enable-stdcall-fixup -static-libgcc -static-libstdc++ -L.. -lFAudio -lole32

DLLTOOL ?= dlltool

TARGET ?= $(shell $(CC) -v 2>&1 | grep -i Target:)

ifneq (,$(findstring x86_64,$(TARGET)))
	BUILD_DIR = build_win64
else
	BUILD_DIR = build_win32
endif

# Mingw dir
MINGW_DIR = $(shell sdl2-config --prefix)

# Source lists
XAPO_SRC = XAPOBase.cpp

XAUDIO2_SRC_x20 = \
	com_utils.cpp \
	xaudio2.cpp \
	XAudio2fx.cpp \
	win_registry.cpp

XAUDIO2_SRC_x28 = \
	com_utils.cpp \
	xaudio2.cpp \
	XAudio2fx.cpp \
	win_device.cpp \
	X3DAudio.cpp \
	XAPOFX.cpp

X3DAUDIO_SRC = 	X3DAudio.cpp

XAPOFX_SRC = \
	com_utils_xapofx.cpp \
	XAPOFX.cpp

XACT3_SRC = \
	com_utils_xact.cpp \
	xact3.cpp \
	win_registry.cpp \
	win_iostream.cpp

# Object code lists
XAPO_TARGET = $(BUILD_DIR)/XAPOBase.lib
XAPO_OBJ = $(XAPO_SRC:%.cpp=$(BUILD_DIR)/%.o)

XAUDIO20_TARGET = $(BUILD_DIR)/xaudio2_0.dll
XAUDIO20_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x20.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO21_TARGET = $(BUILD_DIR)/xaudio2_1.dll
XAUDIO21_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x21.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO22_TARGET = $(BUILD_DIR)/xaudio2_2.dll
XAUDIO22_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x22.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO23_TARGET = $(BUILD_DIR)/xaudio2_3.dll
XAUDIO23_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x23.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO24_TARGET = $(BUILD_DIR)/xaudio2_4.dll
XAUDIO24_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x24.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO25_TARGET = $(BUILD_DIR)/xaudio2_5.dll
XAUDIO25_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x25.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO26_TARGET = $(BUILD_DIR)/xaudio2_6.dll
XAUDIO26_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x26.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO27_TARGET = $(BUILD_DIR)/xaudio2_7.dll
XAUDIO27_OBJ = $(XAUDIO2_SRC_x20:%.cpp=$(BUILD_DIR)/%_x27.o) $(BUILD_DIR)/xaudiodll_x20.o

XAUDIO28_TARGET = $(BUILD_DIR)/xaudio2_8.dll
XAUDIO28_OBJ = $(XAUDIO2_SRC_x28:%.cpp=$(BUILD_DIR)/%_x28.o) $(BUILD_DIR)/xaudiodll_x28.o

XAUDIO29_TARGET = $(BUILD_DIR)/xaudio2_9.dll
XAUDIO29_OBJ = $(XAUDIO2_SRC_x28:%.cpp=$(BUILD_DIR)/%_x29.o) $(BUILD_DIR)/xaudiodll_x28.o

X3DAUDIO_TARGET = $(BUILD_DIR)/x3daudio1_7.dll
X3DAUDIO_OBJ = $(X3DAUDIO_SRC:%.cpp=$(BUILD_DIR)/%.o)

XAPOFX11_TARGET = $(BUILD_DIR)/xapofx1_1.dll
XAPOFX11_OBJ = $(XAPOFX_SRC:%.cpp=$(BUILD_DIR)/%_fx11.o)

XAPOFX12_TARGET = $(BUILD_DIR)/xapofx1_2.dll
XAPOFX12_OBJ = $(XAPOFX_SRC:%.cpp=$(BUILD_DIR)/%_fx12.o)

XAPOFX13_TARGET = $(BUILD_DIR)/xapofx1_3.dll
XAPOFX13_OBJ = $(XAPOFX_SRC:%.cpp=$(BUILD_DIR)/%_fx13.o)

XAPOFX14_TARGET = $(BUILD_DIR)/xapofx1_4.dll
XAPOFX14_OBJ = $(XAPOFX_SRC:%.cpp=$(BUILD_DIR)/%_fx14.o)

XAPOFX15_TARGET = $(BUILD_DIR)/xapofx1_5.dll
XAPOFX15_OBJ = $(XAPOFX_SRC:%.cpp=$(BUILD_DIR)/%_fx15.o)

XACT30_TARGET = $(BUILD_DIR)/xactengine3_0.dll
XACT30_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x30.o) $(BUILD_DIR)/xactdll_x30.o

XACT31_TARGET = $(BUILD_DIR)/xactengine3_1.dll
XACT31_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x31.o) $(BUILD_DIR)/xactdll_x30.o

XACT32_TARGET = $(BUILD_DIR)/xactengine3_2.dll
XACT32_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x32.o) $(BUILD_DIR)/xactdll_x30.o

XACT33_TARGET = $(BUILD_DIR)/xactengine3_3.dll
XACT33_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x33.o) $(BUILD_DIR)/xactdll_x30.o

XACT34_TARGET = $(BUILD_DIR)/xactengine3_4.dll
XACT34_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x34.o) $(BUILD_DIR)/xactdll_x30.o

XACT35_TARGET = $(BUILD_DIR)/xactengine3_5.dll
XACT35_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x35.o) $(BUILD_DIR)/xactdll_x30.o

XACT36_TARGET = $(BUILD_DIR)/xactengine3_6.dll
XACT36_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x36.o) $(BUILD_DIR)/xactdll_x30.o

XACT37_TARGET = $(BUILD_DIR)/xactengine3_7.dll
XACT37_OBJ = $(XACT3_SRC:%.cpp=$(BUILD_DIR)/%_x37.o) $(BUILD_DIR)/xactdll_x30.o


# Targets
$(BUILD_DIR)/%.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_x20.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=0 -c -o $@ $<

$(BUILD_DIR)/%_x21.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=1 -c -o $@ $<

$(BUILD_DIR)/%_x22.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=2 -c -o $@ $<

$(BUILD_DIR)/%_x23.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=3 -c -o $@ $<

$(BUILD_DIR)/%_x24.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=4 -c -o $@ $<

$(BUILD_DIR)/%_x25.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=5 -c -o $@ $<

$(BUILD_DIR)/%_x26.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=6 -c -o $@ $<

$(BUILD_DIR)/%_x27.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=7 -c -o $@ $<

$(BUILD_DIR)/%_x28.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=8 -DUNICODE -c -o $@ $<

$(BUILD_DIR)/%_x29.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXAUDIO2_VERSION=9 -DUNICODE -c -o $@ $<

$(BUILD_DIR)/%_fx11.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_fx12.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_fx13.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_fx14.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_fx15.o:%.cpp
	$(CXX) $(CXXFLAGS) -c -o $@ $<

$(BUILD_DIR)/%_x30.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=0 -c -o $@ $<

$(BUILD_DIR)/%_x31.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=1 -c -o $@ $<

$(BUILD_DIR)/%_x32.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=2 -c -o $@ $<

$(BUILD_DIR)/%_x33.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=3 -c -o $@ $<

$(BUILD_DIR)/%_x34.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=4 -c -o $@ $<

$(BUILD_DIR)/%_x35.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=5 -c -o $@ $<

$(BUILD_DIR)/%_x36.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=6 -c -o $@ $<

$(BUILD_DIR)/%_x37.o:%.cpp
	$(CXX) $(CXXFLAGS) -DXACT3_VERSION=7 -c -o $@ $<

all: $(XAUDIO20_TARGET) $(XAUDIO21_TARGET) $(XAUDIO22_TARGET) $(XAUDIO23_TARGET) \
     $(XAUDIO24_TARGET) $(XAUDIO25_TARGET) $(XAUDIO26_TARGET) $(XAUDIO27_TARGET) \
     $(XAUDIO28_TARGET) $(XAUDIO29_TARGET) $(X3DAUDIO_TARGET) $(XAPO_TARGET) \
     $(XAPOFX11_TARGET) $(XAPOFX12_TARGET) $(XAPOFX13_TARGET) \
     $(XAPOFX14_TARGET) $(XAPOFX15_TARGET) \
     $(XACT30_TARGET) $(XACT31_TARGET) $(XACT32_TARGET) $(XACT33_TARGET) \
     $(XACT34_TARGET) $(XACT35_TARGET) $(XACT36_TARGET) $(XACT37_TARGET)

$(XAPO_TARGET): directories $(XAPO_OBJ)
	$(AR) rcs $@ $(XAPO_OBJ)

$(XAUDIO20_TARGET) : directories $(XAUDIO20_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO20_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO21_TARGET) : directories $(XAUDIO21_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO21_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO22_TARGET) : directories $(XAUDIO22_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO22_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO23_TARGET) : directories $(XAUDIO23_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO23_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO24_TARGET) : directories $(XAUDIO24_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO24_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO25_TARGET) : directories $(XAUDIO25_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO25_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO26_TARGET) : directories $(XAUDIO26_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO26_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO27_TARGET) : directories $(XAUDIO27_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO27_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAUDIO28_TARGET) : directories $(XAUDIO28_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO28_OBJ) $(XAPO_TARGET) $(LDFLAGS) -lsetupapi

$(XAUDIO29_TARGET) : directories $(XAUDIO29_OBJ) $(XAPO_TARGET)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAUDIO29_OBJ) $(XAPO_TARGET) $(LDFLAGS) -lsetupapi

$(X3DAUDIO_TARGET) : directories $(X3DAUDIO_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(X3DAUDIO_OBJ) $(LDFLAGS)
	cp $(X3DAUDIO_TARGET) $(BUILD_DIR)/x3daudio1_3.dll
	cp $(X3DAUDIO_TARGET) $(BUILD_DIR)/x3daudio1_4.dll
	cp $(X3DAUDIO_TARGET) $(BUILD_DIR)/x3daudio1_5.dll
	cp $(X3DAUDIO_TARGET) $(BUILD_DIR)/x3daudio1_6.dll

$(XAPOFX11_TARGET) : directories $(XAPOFX11_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAPOFX11_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAPOFX12_TARGET) : directories $(XAPOFX12_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAPOFX12_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAPOFX13_TARGET) : directories $(XAPOFX13_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAPOFX13_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAPOFX14_TARGET) : directories $(XAPOFX14_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAPOFX14_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XAPOFX15_TARGET) : directories $(XAPOFX15_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XAPOFX15_OBJ) $(XAPO_TARGET) $(LDFLAGS)

$(XACT30_TARGET) : directories $(XACT30_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT30_OBJ) $(LDFLAGS)

$(XACT31_TARGET) : directories $(XACT31_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT31_OBJ) $(LDFLAGS)

$(XACT32_TARGET) : directories $(XACT32_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT32_OBJ) $(LDFLAGS)

$(XACT33_TARGET) : directories $(XACT33_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT33_OBJ) $(LDFLAGS)

$(XACT34_TARGET) : directories $(XACT34_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT34_OBJ) $(LDFLAGS)

$(XACT35_TARGET) : directories $(XACT35_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT35_OBJ) $(LDFLAGS)

$(XACT36_TARGET) : directories $(XACT36_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT36_OBJ) $(LDFLAGS)

$(XACT37_TARGET) : directories $(XACT37_OBJ)
	$(CXX) $(CXXFLAGS) -shared -o $@ $(XACT37_OBJ) $(LDFLAGS)

$(BUILD_DIR)/xaudiodll_x20.o: xaudio2_7.def
	$(DLLTOOL) -d $< -e $@

$(BUILD_DIR)/xaudiodll_x28.o: xaudio2_9.def
	$(DLLTOOL) -d $< -e $@

$(BUILD_DIR)/xactdll_x30.o: xact3_7.def
	$(DLLTOOL) -d $< -e $@

.PHONY: directories clean

directories:
	mkdir -p $(BUILD_DIR)

	cp ../FAudio.dll $(BUILD_DIR)
	cp $(MINGW_DIR)/bin/SDL2.dll $(BUILD_DIR)
	cp $(MINGW_DIR)/bin/libwinpthread*.dll $(BUILD_DIR)

	if [ -f $(MINGW_DIR)/bin/libiconv*.dll ]; then \
		cp $(MINGW_DIR)/bin/libiconv*.dll $(BUILD_DIR); \
	fi

	if [ -n $(FAUDIO_FFMPEG) ] && [ -n $(FAUDIO_FFMPEG_PREFIX) ]; then	\
		cp $(FAUDIO_FFMPEG_PREFIX)/bin/avcodec*.dll $(BUILD_DIR);	\
		cp $(FAUDIO_FFMPEG_PREFIX)/bin/avutil*.dll $(BUILD_DIR);	\
		cp $(FAUDIO_FFMPEG_PREFIX)/bin/swresample*.dll $(BUILD_DIR);	\
	fi

clean:
	rm -f $(XAUDIO20_TARGET) $(XAUDIO21_TARGET) $(XAUDIO22_TARGET) $(XAUDIO23_TARGET) \
	      $(XAUDIO24_TARGET) $(XAUDIO25_TARGET) $(XAUDIO26_TARGET) $(XAUDIO27_TARGET) \
	      $(XAUDIO28_TARGET) $(XAUDIO29_TARGET) $(X3DAUDIO_TARGET) $(XAPO_TARGET) \
              $(XAPOFX11_TARGET) $(XAPOFX12_TARGET) $(XAPOFX13_TARGET) \
              $(XAPOFX14_TARGET) $(XAPOFX15_TARGET) \
	      $(XACT30_TARGET) $(XACT31_TARGET) $(XACT32_TARGET) $(XACT33_TARGET) \
	      $(XACT34_TARGET) $(XACT35_TARGET) $(XACT36_TARGET) $(XACT37_TARGET) \
	      $(BUILD_DIR)/*.o $(BUILD_DIR)/*.dll
	rmdir $(BUILD_DIR)
