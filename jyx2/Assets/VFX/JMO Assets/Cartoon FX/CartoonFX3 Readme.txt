Cartoon FX Pack 3, version 1.0
2013/08/26
© 2013, Jean Moreno
==============================


PREFABS
-------
Particle Systems prefabs are located in "CFX3 Prefabs" folder.
Particle Systems optimized for Mobile are located in "CFX3 Prefabs (Mobile)" folder.
They should work out of the box for most needs. If you need/don't need looping effect, (un)check the according "Looping" checkbox for each Particle System (you can use the Cartoon FX Easy Editor for that too).
All Assets have a CFX3_ (Desktop) or CFXM3_ (Mobile) prefix so that you don't mix them with your own Assets.


MOBILE OPTIMIZED PREFABS
------------------------
Mobile prefabs feature the following optimizations:
- Added a particle additive shader that uses only the alpha channel to save up on texture memory usage
- Textures' formats have been set to Compressed
- Textures have all been resized to small resolution sizes through Unity; you can however scale them up if you need better quality

It is recommended to use CFX Spawn System for object spawning on mobile (the system also works on any other GameObject, not just effects!), see below.


CARTOON FX EASY EDITOR
----------------------
You can find the "Cartoon FX Easy Editor" in the menu:
Window -> CartoonFX Easy Editor
It allows you to easily change one or several Particle Systems properties:
"Scale Size" to change the size of your Particle Systems (changing speed, velocity, gravity, etc. values to get an accurate scaled up version of the system; also, if the ParticleSystem uses a Mesh as Shape, it will automatically create a new scaled Mesh).
It will also scale lights' intensity accordingly if any are found.
Tip: If you don't want to scale a particular module, disable it before scaling the system and re-enable it afterwards!
"Set Speed" to change the playback speed of your Particle Systems in percentage according to the base effect speed. 100% = normal speed.
"Tint Colors" to change the hue only of the colors of your Particle Systems (including gradients).

The "Copy Modules" section allows you to copy all values/curves/gradients/etc. from one or several Shuriken modules to one or several other Particle Systems.
Just select which modules you want to copy, choose the source Particle System to copy values from, select the GameObjects you want to change, and click on "Copy properties to selected GameObject(s)".

"Include Children" works for both Properties and Copy Modules sections!


CARTOON FX SPAWN SYSTEM
-----------------------
CFX_SpawnSystem allows you to easily preload your effects at the beginning of a Scene and get them later, avoiding the need to call Instantiate. It is highly recommended for mobile platforms!
Create an empty GameObject and drag the script on it. You can then add GameObjects to it with its custom interface.
To get an object in your code, use CFX_SpawnSystem.GetNextObject(object), where 'object' is the original reference to the GameObject (same as if you used Instantiate).
Use the CFX_SpawnSystem.AllObjectsLoaded boolean value to check when objects have finished loading.


TROUBLESHOOTING
---------------
* Almost all prefabs have auto-destruction scripts for the Demo scene; remove them if you do not want your particle system to destroy itself upon completion.
* If you have problems with z-sorting (transparent objects appearing in front of other when their position is actually behind), try changing the values in the Particle System -> Renderer -> Sorting Fudge; as long as the relative order is respected between the different particle systems of a same prefab, it should work ok.
* Some prefabs work with the Collision module: they are set to World Collision by default but it requires CPU. Disable it or change to "Planes" collision mode to save ressources.
* Sometimes when instantiating a Particle System, it would start to emit before being translated, thus creating particles in between its original and desired positions. Drag and drop the CFX_ShurikenThreadFix script on the parent object to fix this problem.


PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL! THANKS!
Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:

jean.moreno.public+unity@gmail.com

I'd be happy to see any effects used in your project, so feel free to drop me a line about that! :)




RELEASE NOTES
-------------

v1.0
- Initial release
