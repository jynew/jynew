using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace BeautifyEffect {
				public class BeautifySAdvancedOptionsInfo {

								public bool pendingChanges;
								public ShaderAdvancedOption[] options;

								public void ReadOptions () {
												pendingChanges = false;
												// Populate known options
												options = new ShaderAdvancedOption[] {
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_ORTHO", name = "Orthographic Mode", description = "Enables support for orthographic camera projection."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_ENABLE_CORE_EFFECTS",
																				name = "Use Core Filters",
																				description = "Enables sharpen, brightness, contrast and vibrance effects."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_ENABLE_DITHER",
																				name = "Use Dithering",
																				description = "Disabling dithering can improve performance on old mobile devices."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_DITHER_FINAL",
																				name = "Dither at the end",
																				description = "Applies dithering at the end of the stack (recommended). Disable to apply dithering at the start."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_EYE_ADAPTATION_DYNAMIC_RANGE",
																				name = "Use Dynamic Range Eye Adaptation",
																				description = "Disable to use legacy eye adaptation method (less accurate, provided for compatibility and always used in Best Performance mode)."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_SUN_FLARES_OCCLUSION_DEPTH",
																				name = "Use Sun Depth-Based Occlusion",
																				description = "Uses camera depth buffer to compute Sun occlusion instead of relying on raycasting."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_ACES_FITTED",
																				name = "Use Alternate ACES Tonemapping Operator",
																				description = "Uses an alternate algorithm that poduces less saturation on bright colors. ACES is only available in best quality mode."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_OUTLINE_SOBEL",
																				name = "Use Sobel Outline method",
																				description = "Uses a color-based edge detection algorithm instead of a depth based method. Useful for 2D projects."
																},
																new ShaderAdvancedOption {
																				id = "BEAUTIFY_USE_PROCEDURAL_SEPIA",
																				name = "Use Procedural Sepia",
																				description = "Uses a formula based instead of Look-Up texture to produce Sepia effect."
																},
                new ShaderAdvancedOption {
                    id = "BEAUTIFY_BETTER_FASTER_LUT",
                                                                                name = "Better Fast LUT",
                                                                                description = "Improves LUT quality in Best Performance Mode."
                                                                }



												};


												Shader shader = Shader.Find ("Beautify/Beautify");
												if (shader != null) {
																string path = AssetDatabase.GetAssetPath (shader);
																string file = Path.GetDirectoryName (path) + "/BeautifyAdvancedParams.cginc";
																string[] lines = File.ReadAllLines (file, Encoding.UTF8);
																for (int k = 0; k < lines.Length; k++) {
																				for (int o = 0; o < options.Length; o++) {
																								if (lines [k].Contains (options [o].id)) {
																												options [o].enabled = !lines [k].StartsWith ("//");
																								}
																				}
																}
												}
								}


								public bool GetAdvancedOptionState (string optionId) {
												if (options == null)
																return false;
												for (int k = 0; k < options.Length; k++) {
																if (options [k].id.Equals (optionId)) {
																				return options [k].enabled;
																}
												}
												return false;
								}

								public void UpdateAdvancedOptionsFile () {
												// Reloads the file and updates it accordingly
												Shader shader = Shader.Find ("Beautify/Beautify");
												if (shader != null) {
																string path = AssetDatabase.GetAssetPath (shader);
																string file = Path.GetDirectoryName (path) + "/BeautifyAdvancedParams.cginc";
																string[] lines = File.ReadAllLines (file, Encoding.UTF8);
																for (int k = 0; k < lines.Length; k++) {
																				for (int o = 0; o < options.Length; o++) {
																								if (lines [k].Contains (options [o].id)) {
																												if (options [o].enabled) {
																																lines [k] = "#define " + options [o].id;
																												} else {
																																lines [k] = "//#define " + options [o].id;
																												}
																								}
																				}
																}
																File.WriteAllLines (file, lines, Encoding.UTF8);
																pendingChanges = false;
																AssetDatabase.Refresh ();
												}
								}

		
				}

				public struct ShaderAdvancedOption {
								public string id;
								public string name;
								public string description;
								public bool enabled;
				}
	
	
}