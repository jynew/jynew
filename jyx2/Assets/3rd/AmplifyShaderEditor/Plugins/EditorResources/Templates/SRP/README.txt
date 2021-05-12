Both High Definition and Lightweight rendering pipelines are in development and in a preview state, so Amplify Shader Editor may have some nodes that are not fully updated for them. 
At the moment there are, for each SRP, two separate packages inside Amplify Shader Editor. They contemplate two major SRP versions that exist at the moment.
For Unity 2018.2 and bellow, users can only install v3.x.x for both HD and LW rendering pipelines and as such they will need to unpack the unity packages containing the (Legacy) tag.
For Unity 2018.3 and above, users can only install v4.x.x for both HD and LW rendering pipelines and as such they will need to unpack the unity packages NOT containing the (Legacy) tag.

Unity 2018.2.x, HD and LW v3.x.x:
* HDSRPTemplates (Legacy).unitypackage
    * HD PBR
    * HD unlit

* LWSRPTemplates (Legacy).unitypackage
    * Lightweight PBR
    * Lightweight Unlit

Unity 2018.3.x, HD and LW v4.x.x:
* HDSRPTemplates.unitypackage
    * HD PBR
    * HD unlit

* LWSRPTemplates.unitypackage
    * Lightweight PBR
    * Lightweight Unlit

Upon unpacking, the templates they may not be instantly available at the ( Create > Amplify Shader > ... ) menu over you project view, but a user can create p.e. a new Amplify Surface Shader, go to its Shader Type menu over the left Node Properties window and select its newly installed template.