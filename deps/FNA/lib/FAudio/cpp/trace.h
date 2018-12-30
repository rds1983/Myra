#ifndef FACT_CPP_TRACE_H
#define FACT_CPP_TRACE_H

#ifdef TRACING_ENABLE
#include <stdio.h>
#include <stdarg.h>

#if defined _WIN32 || defined __CYGWIN__
#define TRACE_FILE	"c:\\temp\\faudio_cpp.log"
#else
#define TRACE_FILE	"/tmp/faudio_cpp.log"
#endif

#define TRACE_FUNC()	do { trace_msg(__FUNCTION__); } while (0)
#define TRACE_MSG(f,...) do {trace_msg("%s: " f, __FUNCTION__, __VA_ARGS__);} while (0)


static void trace_msg(const char *msg, ...) {
	va_list args;
	va_start(args, msg);

#ifdef _MSC_VER
	FILE *fp = NULL;
	if (fopen_s(&fp, TRACE_FILE, "a") != 0) {
		fp = NULL;
	}
#else
	FILE *fp = fopen(TRACE_FILE, "a");
#endif
	if (fp) {
		vfprintf(fp, msg, args);
		fputc('\n', fp);
		fclose(fp);
	}
	va_end(args);
}

#else
#define TRACE_FUNC()
#define TRACE_MSG(f, ...)
#endif // TRACING_ENABLE

#endif // FACT_CPP_TRACE_H