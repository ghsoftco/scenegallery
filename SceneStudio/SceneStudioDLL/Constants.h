const float epsilonNormalDisplacement = 0.25f;

const float rotateDelta = Math::PIf / 4.0f;
const float scaleDelta = 0.04f;
const float rotateDeltaSmall = rotateDelta / 30.0f;
const float scaleDeltaSmall = scaleDelta / 4.0f;
const float wheelScale = 0.1f;

const bool runningLocalDirectoryTasks = false;
const bool SkipTextureLoading = false;

const UINT screenshotWidth = 400;
const UINT screenshotHeight = 400;
const String screenshotDirectory("../../Scenes/");

const bool DebuggingMode = false;
const bool DebuggingStatusText = false;
const UINT MaxUndoDepth = 128;