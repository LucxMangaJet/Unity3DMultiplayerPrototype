using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterProcessPass : ScriptableRenderPass
{
    public Material Material;

    private static readonly int tempRTPropertyId = Shader.PropertyToID("_TempRT");
    // Name of the grab texture used in the shaders.
    private static readonly int grabTexturePropertyId = Shader.PropertyToID("_CameraColorTexture");

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!Material)
            return;

        Camera camera = renderingData.cameraData.camera;

        CommandBuffer cmd = CommandBufferPool.Get();

        cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);

        cmd.GetTemporaryRT(tempRTPropertyId, renderingData.cameraData.cameraTargetDescriptor);
        cmd.Blit(BuiltinRenderTextureType.RenderTexture, tempRTPropertyId);
        cmd.SetGlobalTexture(grabTexturePropertyId, tempRTPropertyId);

        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, Material);
        cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);

        //this should be before skybox... how?
    }
}
