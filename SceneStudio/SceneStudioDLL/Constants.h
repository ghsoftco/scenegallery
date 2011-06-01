const float epsilonNormalDisplacement = 0.25f;

const float rotateDelta = Math::PIf / 4.0f;
const float scaleDelta = 0.04f;
const float rotateDeltaSmall = rotateDelta / 30.0f;
const float scaleDeltaSmall = scaleDelta / 4.0f;
const float wheelScale = 0.1f;

const bool runningLocalDirectoryTasks = false;
const bool SkipTextureLoading = false;

const Vec2i screenshotDim(400, 400);
const Vec2i screenshotDimLarge(1000, 1000);
const String screenshotDirectory("../../Scenes/");
const String exemplarsDirectory("../../Scenes/exemplars/");
const String resultsDirectory("../../Scenes/results/");
const String noModelHash("*");

const bool DebuggingMode = false;
const bool DebuggingStatusText = false;
const UINT MaxUndoDepth = 128;

const UINT samplingSpheresToRender = 1000;
const float minZforSampledNormal = 0.95f;