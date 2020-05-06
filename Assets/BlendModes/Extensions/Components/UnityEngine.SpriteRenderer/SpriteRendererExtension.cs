using UnityEngine;

namespace BlendModes
{
    [ExtendedComponent(typeof(SpriteRenderer))]
   public class SpriteRendererExtension : RendererExtension<SpriteRenderer>
   {
        private static ShaderProperty[] cachedDefaultProperties;

       public override string[] GetSupportedShaderFamilies ()
        {
           return new[] {
                "SpritesDefault",
                "SpritesHsbc",
               "SpritesVectorGradient"
            };
        }

       public override ShaderProperty[] GetDefaultShaderProperties ()
       {
            return cachedDefaultProperties ?? (cachedDefaultProperties = new[] {
                    new ShaderProperty("_Hue", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Saturation", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Brightness", ShaderPropertyType.Float, 0),
                    new ShaderProperty("_Contrast", ShaderPropertyType.Float, 0)
                });
        }

        protected override string GetDefaultShaderName ()
        {
            return "Sprites/Default";
       }
    }
}
