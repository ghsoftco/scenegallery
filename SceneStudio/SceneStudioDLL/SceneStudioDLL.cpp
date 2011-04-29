#include "Main.h"

SCENESTUDIODLL_API void* __stdcall SSInitD3D(HWND window)
{
    App *app = new App;
    app->InitD3D(window);
    return app;
}

SCENESTUDIODLL_API UINT32 __stdcall SSRenderFrame(void *context)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    if(app.mutex().TryAcquire())
    {
        app.RenderFrame();
        app.mutex().Release();
    }
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSMouseMove(void *context, UINT button, int x, int y, int xDelta, int yDelta)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    if(app.mutex().TryAcquire())
    {
        app.MouseMove(button, x, y, xDelta, yDelta);
        app.mutex().Release();
    }
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSMouseDown(void *context, UINT button, int x, int y)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    if(app.mutex().TryAcquire())
    {
        app.MouseDown(button, x, y);
        app.mutex().Release();
    }
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSMouseUp(void *context, UINT button)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    if(app.mutex().TryAcquire())
    {
        app.MouseUp(button);
        app.mutex().Release();
    }
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSMouseWheel(void *context, int delta)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    if(app.mutex().TryAcquire())
    {
        app.MouseWheel(delta);
        app.mutex().Release();
    }
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSKeyPress(void *context, int key, bool shift, bool ctrl)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    app.mutex().Acquire();
    app.KeyPress(key, shift, ctrl);
    app.mutex().Release();
    return 0;
}

SCENESTUDIODLL_API const char* __stdcall SSQueryString(void *context, QueryType q)
{
    if(context == NULL) return 0;
    App &app = *(App*)context;
    app.mutex().Acquire();
    const char *result = app.QueryString(q);
    app.mutex().Release();
    return result;
}

SCENESTUDIODLL_API int __stdcall SSQueryInteger(void *context, QueryType q)
{
    if(context == NULL) return 0;
    App &app = *(App*)context;
    app.mutex().Acquire();
    int result = app.QueryInteger(q);
    app.mutex().Release();
    return result;
}

SCENESTUDIODLL_API UINT32 __stdcall SSModelChosen(void *context, const char *model)
{
    if(context == NULL) return 0;
    App &app = *(App*)context;
    app.mutex().Acquire();
    app.ModelChosen(model);
    app.mutex().Release();
    return 0;
}

SCENESTUDIODLL_API UINT32 __stdcall SSProcessCommand(void *context, const char *string)
{
    if(context == NULL) return 1;
    App &app = *(App*)context;
    app.mutex().Acquire();
    UINT32 result = app.ProcessCommand(string);
    app.mutex().Release();
    return result;
}
