// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the SCENESTUDIODLL_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// SCENESTUDIODLL_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef SCENESTUDIODLL_EXPORTS
#define SCENESTUDIODLL_API __declspec(dllexport)
#else
#define SCENESTUDIODLL_API __declspec(dllimport)
#endif

SCENESTUDIODLL_API void* __stdcall SSInitD3D(HWND window);
SCENESTUDIODLL_API UINT32 __stdcall SSRenderFrame(void *context);
SCENESTUDIODLL_API UINT32 __stdcall SSMouseMove(void *context, UINT button, int x, int y, int xDelta, int yDelta);
SCENESTUDIODLL_API UINT32 __stdcall SSMouseDown(void *context, UINT button, int x, int y);
SCENESTUDIODLL_API UINT32 __stdcall SSMouseUp(void *context, UINT button);
SCENESTUDIODLL_API UINT32 __stdcall SSMouseWheel(void *context, int delta);
SCENESTUDIODLL_API UINT32 __stdcall SSKeyPress(void *context, int key, bool shift, bool ctrl);
SCENESTUDIODLL_API UINT32 __stdcall SSModelChosen(void *context, const char *model);

SCENESTUDIODLL_API UINT32 __stdcall SSProcessCommand(void *context, const char *string);

enum QueryType;
SCENESTUDIODLL_API const char* __stdcall SSQueryString(void *context, QueryType stringIndex);
SCENESTUDIODLL_API int __stdcall SSQueryInteger(void *context, QueryType integerIndex);
