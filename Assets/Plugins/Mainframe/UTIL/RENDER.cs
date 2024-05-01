//  ___  ___  _  _  ___   ___  ___                                                                    
// | _ \| __|| \| ||   \ | __|| _ \                                                                   
// |   /| _| | .` || |) || _| |   /                                                                   
// |_|_\|___||_|\_||___/ |___||_|_\                                                                   
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using UnityEngine;
using UnityEngine.Rendering;
#endregion

namespace Mainframe
{
public enum RenderPipelineType { Default = 0, Custom = 1, Universal = 2, HighDefinition = 3, }

public static class RENDER
{
  public static int VSyncCount => QualitySettings.vSyncCount;
  public static float VSyncDeltaTime => 1f / QualitySettings.vSyncCount;

  // Render Pipeline
  //----------------------------------------------------------------------------------------------------
  public static RenderPipelineType RenderPipeline()
  {
    RenderPipelineAsset current = GraphicsSettings.currentRenderPipeline;
    if(current == null) { return RenderPipelineType.Default; }

    string srpName = current.GetType().ToString();

    if(srpName.Contains("Universal")) { return RenderPipelineType.Universal; }
    else if(srpName.Contains("HDRenderPipeline")) { return RenderPipelineType.HighDefinition; }
    else return RenderPipelineType.Custom;
  }

  public static string GetStandardShaderName()
  {
    string name = RenderPipeline() switch
                  {
                    RenderPipelineType.Default        => "Standard",
                    RenderPipelineType.Universal      => "Universal Render Pipeline/Lit",
                    RenderPipelineType.HighDefinition => "HDRP/Lit",
                    RenderPipelineType.Custom         => "Standard",
                    _                                 => "Standard"
                  };
    return name;
  }
}
}
