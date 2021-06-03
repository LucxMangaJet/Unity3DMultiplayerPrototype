using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class UnderwaterProcess : ScriptableRendererFeature
{
    [SerializeField] Material material;

    private UnderwaterProcessPass pass;

    public override void Create()
    {
        pass = new UnderwaterProcessPass();
        pass.Material = material;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
