#!/bin/bash

export PROTONFOLDER=~/.steam/steam/compatibilitytools.d/flibitProton/dist

set -x
set -e

cp build_wine64/libFAudio.so $PROTONFOLDER/lib64/
cp build_wine64/*.dll.so $PROTONFOLDER/lib64/wine/
cp build_wine64/xaudio2_0.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_0.dll
cp build_wine64/xaudio2_1.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_1.dll
cp build_wine64/xaudio2_2.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_2.dll
cp build_wine64/xaudio2_3.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_3.dll
cp build_wine64/xaudio2_4.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_4.dll
cp build_wine64/xaudio2_5.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_5.dll
cp build_wine64/xaudio2_6.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_6.dll
cp build_wine64/xaudio2_7.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_7.dll
cp build_wine64/xaudio2_8.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_8.dll
cp build_wine64/xaudio2_9.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xaudio2_9.dll
cp build_wine64/x3daudio1_3.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/x3daudio1_3.dll
cp build_wine64/x3daudio1_4.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/x3daudio1_4.dll
cp build_wine64/x3daudio1_5.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/x3daudio1_5.dll
cp build_wine64/x3daudio1_6.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/x3daudio1_6.dll
cp build_wine64/x3daudio1_7.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/x3daudio1_7.dll
cp build_wine64/xactengine3_0.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_0.dll
cp build_wine64/xactengine3_1.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_1.dll
cp build_wine64/xactengine3_2.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_2.dll
cp build_wine64/xactengine3_3.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_3.dll
cp build_wine64/xactengine3_4.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_4.dll
cp build_wine64/xactengine3_5.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_5.dll
cp build_wine64/xactengine3_6.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_6.dll
cp build_wine64/xactengine3_7.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xactengine3_7.dll
cp build_wine64/xapofx1_1.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xapofx1_1.dll
cp build_wine64/xapofx1_2.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xapofx1_2.dll
cp build_wine64/xapofx1_3.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xapofx1_3.dll
cp build_wine64/xapofx1_4.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xapofx1_4.dll
cp build_wine64/xapofx1_5.dll.so.fake $PROTONFOLDER/lib64/wine/fakedlls/xapofx1_5.dll

cp build_wine32/libFAudio.so $PROTONFOLDER/lib/
cp build_wine32/*.dll.so $PROTONFOLDER/lib/wine/
cp build_wine32/xaudio2_0.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_0.dll
cp build_wine32/xaudio2_1.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_1.dll
cp build_wine32/xaudio2_2.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_2.dll
cp build_wine32/xaudio2_3.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_3.dll
cp build_wine32/xaudio2_4.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_4.dll
cp build_wine32/xaudio2_5.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_5.dll
cp build_wine32/xaudio2_6.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_6.dll
cp build_wine32/xaudio2_7.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_7.dll
cp build_wine32/xaudio2_8.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_8.dll
cp build_wine32/xaudio2_9.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xaudio2_9.dll
cp build_wine32/x3daudio1_3.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/x3daudio1_3.dll
cp build_wine32/x3daudio1_4.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/x3daudio1_4.dll
cp build_wine32/x3daudio1_5.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/x3daudio1_5.dll
cp build_wine32/x3daudio1_6.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/x3daudio1_6.dll
cp build_wine32/x3daudio1_7.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/x3daudio1_7.dll
cp build_wine32/xactengine3_0.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_0.dll
cp build_wine32/xactengine3_1.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_1.dll
cp build_wine32/xactengine3_2.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_2.dll
cp build_wine32/xactengine3_3.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_3.dll
cp build_wine32/xactengine3_4.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_4.dll
cp build_wine32/xactengine3_5.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_5.dll
cp build_wine32/xactengine3_6.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_6.dll
cp build_wine32/xactengine3_7.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xactengine3_7.dll
cp build_wine32/xapofx1_1.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xapofx1_1.dll
cp build_wine32/xapofx1_2.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xapofx1_2.dll
cp build_wine32/xapofx1_3.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xapofx1_3.dll
cp build_wine32/xapofx1_4.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xapofx1_4.dll
cp build_wine32/xapofx1_5.dll.so.fake $PROTONFOLDER/lib/wine/fakedlls/xapofx1_5.dll
