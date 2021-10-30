Asset Creator - Vladyslav Horobets (Hovl).
All that is in the folder "AAA Projectiles" can be used in commerce, even demo scene files.
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

2)Light.
2.1)You can disable light in the main effect component (delete light and disable light in PS). 
    Light strongly loads the game if you don't use light probes or something else.

3)Scripts
3.1) Use "ParticleCollisionInstance" script with particle system. It works if you turn on "Send Collision Messages " in the particle system (Collision tab).

4)Quality
4.1) For better sparks quality enable "Anisotropic textures: Forced On" in quality settings.

Contact me if you have any problems or questions.
My email: gorobecn2@gmail.com