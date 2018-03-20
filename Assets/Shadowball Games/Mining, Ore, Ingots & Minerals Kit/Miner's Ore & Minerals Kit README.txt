Thank you for purchasing the Mining, Ore, & Minerals Kit!  I very much hope this asset pack proves to be versatile for many your mining and smelting needs.

Please take a moment to review this document, as it covers the intended method with which you should modify the materials in this package, as well as some other noteworthy topics.


//  Showcase Renders  //
////////////////////////

The screen shots you saw on the Asset Store are taken straight from the Cameras located in the Showcase scenes.  To view the content at its selected angle, simply toggle the visibility of one of the root parent objects and switch to Game view.  The specific lighting conditions generated for the screen shots may not look so good when viewed from another angle without tweaking the lighting.


//  Source Files  //
////////////////////

This asset pack comes complete with all the high-resolution models, low-resolution models, and ray casting cages used for baking all the various texture maps (cavity, ambient occlusion, normal maps, etc.).  Please note that the low-resolution models should be used for baking maps and not the ones already loaded in the game engine.

Due to the size of the files, they are not included with this asset pack.  You can download them from Google Drive through the link below.

https://drive.google.com/drive/folders/0B5MdTe6u1qY1S1o4akkyUnE2M3M?usp=sharing

xNormal was used for baking texture maps.  Get it for free here:  http://www.xnormal.net/1.aspx
Quixel NDO was used for supplemental detailing on normal maps.  Get it here:  http://quixel.se/dev/ndo

Albedo maps (flat-lit color maps) for the stone formations have been saved as layers within each of the PSD files.  These, along with the other maps you would bake from the source models, are meant for PBR/IBL shader systems (such as what can be achieved with the Marmoset Skyshop, Lux, Jove, Materializer, Shader Forge, and other custom shader systems).


//  Rock Texture PSD Files  //
//////////////////////////////

Each PSD file contains a group of Adjustment Layers that have been pre-set to reflect a specific material type.  The default material is Copper.  If you wish to switch to a different material, simply activate the layer with the preset you want, and hide the others.  You can at any time save the file as a new diffuse texture in case you are using multiple materials in your project.  Be sure to save as a 32 bit texture in order for the alpha channel (gloss) to save with it.

Because the default materials only use a greyscale gloss map, you will want to tweak the Specular Color channel to match the material preset you have chosen.  For example, if you are using the Copper material preset in Photoshop, adjust the Specular Color channel of the Material in Unity to something around an orange to help sell the type of metal it's supposed to be.


//  Metallic Materials  //
//////////////////////////

The metallic materials used on the Ores, Ingots, and Coins all have their own unique base color, specular color, and shininess values which are used on top of a greyscale texture.  This allows for customized tweaking of the existing materials or creation of your own metals/stones that utilize the same base models and textures.  You should refer to the Specular Color of these Materials when adjusting the Specular Color of the materials associated with the stone formations so the metal of the ore and the deposits appear generally the same.


//  Other Stuff  //
///////////////////

In addition to there being flaky, veiny, and chunky deposit types, you can also combine the flaky or veiny material with a chunky configuration to achieve an even denser mineral deposit.  You can also use this method to include multiple deposit types on the same rock formation: for instance, copper veins with iron chunks.




Finally, please rate, and if you have any comments or critiques, leave a user review so we can continue to improve our content.

Thank you!
