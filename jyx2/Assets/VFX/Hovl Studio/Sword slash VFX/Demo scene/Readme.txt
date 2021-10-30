Asset Creator - Vladislav Horobets (Hovl).
-----------------------------------------------------

Using:

If you want to use post-effects like in the demo video:
https://youtu.be/hZSZ2Q8MF3k

1) Shaders
1.1)The "Use depth" on the material from the custom shaders is the Soft Particle Factor.
1.2)Use "Center glow"[MaterialToggle] only with particle system. This option is used to darken the main texture with a white texture (white is visible, black is invisible).
    If you turn on this feature, you need to use "Custom vertex stream" (Uv0.Custom.xy) in tab "Render". And don't forget to use "Custom data" parameters in your PS.
1.3)The distortion shader only works with standard rendering. Delete (if exist) distortion particles from effects if you use LWRP or HDRP!
1.4)You can change the cutoff in all shaders (except Add_CenterGlow and Blend_CenterGlow ) using (Uv0.Custom.xy) in particle system.
1.5)SwordSlash shader use Custom data in particle system.
    For this you need to anable "Custom vertex stream" in Render tab and add "Custom 1.xy, UV2, Custom 2.xy".
    Then enable Custom data tab and use fist 2 components in both custom tabs.
    Custom1 x: slash dissolve power
    Custom1 y: Slash lenght
    Custom2 x: HUE color
    Custom2 y: Must set to random from 0 to 1 (for random texture position)

2)Quality
2.1) For better sparks quality enable "Anisotropic textures: Forced On" in quality settings.

  SUPPORT ASSET FOR URP or HDRP here --> https://assetstore.unity.com/packages/slug/157764
  SUPPORT ASSET FOR URP or HDRP here --> https://assetstore.unity.com/packages/slug/157764
  SUPPORT ASSET FOR URP or HDRP here --> https://assetstore.unity.com/packages/slug/157764

Contact me if you have any questions.
My email: gorobecn2@gmail.com


Thank you for reading, I really appreciate it.
Please rate this asset in the Asset Store ^^