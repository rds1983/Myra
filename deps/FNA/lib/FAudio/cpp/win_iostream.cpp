#include <windows.h> /* Barf */
#include <FAudio.h>

/* These wrapper calls are just SDL_RWops for Windows.
 * XACT specifically asks for FILE_FLAG_NO_BUFFERING,
 * so that's slightly less work for us!
 */

size_t FAUDIOCALL wrap_io_read(
	void *data,
	void *dst,
	size_t size,
	size_t count
) {
	DWORD byte_read;
	if (!ReadFile((HANDLE) data, dst, size * count, &byte_read, NULL))
	{
		return 0;
	}
	return byte_read;
}

int64_t FAUDIOCALL wrap_io_seek(void *data, int64_t offset, int whence)
{
	DWORD windowswhence;
	LARGE_INTEGER windowsoffset;
	HANDLE io = (HANDLE) data;

	switch (whence)
	{
	case FAUDIO_SEEK_SET:
		windowswhence = FILE_BEGIN;
		break;
	case FAUDIO_SEEK_CUR:
		windowswhence = FILE_CURRENT;
		break;
	case FAUDIO_SEEK_END:
		windowswhence = FILE_END;
		break;
	}

	windowsoffset.QuadPart = offset;
	if (!SetFilePointerEx(io, windowsoffset, &windowsoffset, windowswhence))
	{
		return -1;
	}
	return windowsoffset.QuadPart;
}

int FAUDIOCALL wrap_io_close(void *data)
{
	CloseHandle((HANDLE) data);
	return 0;
}
