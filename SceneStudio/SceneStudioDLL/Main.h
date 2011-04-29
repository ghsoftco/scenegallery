/*
Main.h
Written by Matthew Fisher

Main.h is included by all source files and includes every header file in the correct order.
*/
#pragma once

//
// Config.h includes a series of #defines used to control compiling options
//
#include "Config.h"

//
// Engine.h includes everything that rarely changes between applications, such as vector/Matrix4 libraries,
// OpenGL/DirectX graphics devices, software rasterizers, etc.
//
#include "Engine.h"

#include "Constants.h"

#include "SceneStudioDLL.h"

#include "UIEvent.h"
#include "UIEventLogger.h"
#include "ModelDatabase.h"
#include "Model.h"
#include "ModelInstance.h"
#include "AssetManager.h"
#include "Scene.h"
#include "SceneCamera.h"
#include "Picker.h"
#include "UndoStack.h"

#include "AppParameters.h"
#include "App.h"
