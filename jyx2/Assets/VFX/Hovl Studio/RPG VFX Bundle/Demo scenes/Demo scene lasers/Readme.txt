Asset Creator - Vladyslav Horobets (Hovl).
-----------------------------------------------------

If you want to use post-effects like in the demo video:
https://youtu.be/hZSZ2Q8MF3k

Using:

1) Shaders
1.1)The "Use depth" on the material from the custom shaders is the Soft Particle Factor.
1.2)Use "Center glow"[MaterialToggle] only with particle system. This option is used to darken the main texture with a white texture (white is visible, black is invisible).
    If you turn on this feature, you need to use "Custom vertex stream" (Uv0.Custom.xy) in tab "Render". And don't forget to use "Custom data" parameters in your PS.
1.3)The distortion shader only works with standard rendering. Delete (if exist) distortion particles from effects if you use LWRP or HDRP!
1.4)You can change the cutoff in all shaders (except Add_CenterGlow and Blend_CenterGlow ) using (Uv0.Custom.xy) in particle system.

2)Scripts
2.1)"Hovl_DemoLasers" script is needed to control the rotation of the laser and to change the prefabs.
2.2)"Hovl_Laser" can work independently without the help of additional scripts, but the DisablePrepare() function is called using "Hovl_DemoLasers" script.
    The MaxLength property is responsible for the length of the laser. Beyond this length, the laser doesn't work.
    The script won't work if you don't select "Hit Effect". "Hit Effect" is a special effect at the end of the laser.
    "Hit Offset" is the shift of the position of the "Hit Effect" over the position of the collision site.
    "MainTextureLength" and "NoiseTextureLength" are responsible for the tiling of the texture.

3)Quality
3.1) For better sparks quality enable "Anisotropic textures: Forced On" in quality settings.

Contact me if you have any problems or questions.
My email: gorobecn2@gmail.com