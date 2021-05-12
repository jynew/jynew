// Advanced customization parameters
// To prevent overusing multi_compile keywords, these options are exposed here as they're one-time toggles (meaning you won't be toggling them at runtime)
// Note that changes to this file will be lost when you update Beautify! Remember to revert this file to your preferences after updating.

// Uncomment this one to support Orthographic Camera
//#define BEAUTIFY_ORTHO

// Comment out this one to disable sharpen, brightness, contrast and vibrance effects
#define BEAUTIFY_ENABLE_CORE_EFFECTS

// Comment out this one to disable dithering. Can improve performance on old mobile devices.
#define BEAUTIFY_ENABLE_DITHER

// Dither is applied at the end of stack. Comment out this line to apply dither at the start.
#define BEAUTIFY_DITHER_FINAL

// Comment out this line to simplify eye adaptation to brightness increase/reduction (old method)
#define BEAUTIFY_EYE_ADAPTATION_DYNAMIC_RANGE

// Uncomment this to enable occlusion on objects that do not have colliders
//#define BEAUTIFY_SUN_FLARES_OCCLUSION_DEPTH

// Uncomment this line to use an alternate ACES tonemap operator
//#define BEAUTIFY_ACES_FITTED

// Uncomment this line to use a color-based edge detection algorithm instead of a depth based method. Useful for 2D projects.
//#define BEAUTIFY_OUTLINE_SOBEL

// Uncomment this line to use a formula instead of Look-Up texture to produce Sepia effect
//#define BEAUTIFY_USE_PROCEDURAL_SEPIA

// Uncomment this line to improve LUT transform in Best Performance mode
//#define BEAUTIFY_BETTER_FASTER_LUT
