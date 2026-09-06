using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class GenerateDepthTexture2 : MonoBehaviour {
	public LayerMask casterLayer;
	public Shader shader;
	public float Near;
	public float Far;
	public float Size;
	public int TextureSize = 512;
	[Range(0,0.1f)]
	public float Bias;
	[Range(0,1)]
	public float Strength;
	private Matrix4x4 biasMatrix;
	private Camera depthCamera;
	[ShowInInspector] private RenderTexture depthTexture;
	private bool isActive = true;
	private bool _isActive = true;

    //---------Uniform Val
    readonly int _depthVPBias = Shader.PropertyToID("_depthVPBias");
    readonly int _depthV = Shader.PropertyToID("_depthV");
    readonly int _kkShadowMap = Shader.PropertyToID("_kkShadowMap");
    readonly int _bias = Shader.PropertyToID("_bias");
    readonly int _strength = Shader.PropertyToID("_strength");
    readonly int _texmapScale = Shader.PropertyToID("_texmapScale");
    readonly int _farplaneScale = Shader.PropertyToID("_farplaneScale");
    //-----------

    void Awake () {
		GameObject go = new GameObject("depthCamera");
		depthCamera = go.AddComponent<Camera>();
        depthCamera.transform.parent = transform;
        depthCamera.transform.localPosition = Vector3.zero;
		depthCamera.transform.rotation = transform.rotation;
		depthCamera.transform.localPosition += transform.forward * Near;
		depthCamera.orthographic = true;
		
		depthCamera.clearFlags = CameraClearFlags.SolidColor;
		depthCamera.backgroundColor = Color.white;
		depthTexture = new RenderTexture(TextureSize,TextureSize, 16, RenderTextureFormat.ARGB32);
		depthTexture.filterMode = FilterMode.Point;
		depthCamera.targetTexture = depthTexture;
		depthCamera.SetReplacementShader(shader, null);
		depthCamera.enabled = false;
		biasMatrix = Matrix4x4.identity;
		biasMatrix[ 0, 0 ] = 0.5f;
		biasMatrix[ 1, 1 ] = 0.5f;
		biasMatrix[ 2, 2 ] = 0.5f;
		biasMatrix[ 0, 3 ] = 0.5f;
		biasMatrix[ 1, 3 ] = 0.5f;
		biasMatrix[ 2, 3 ] = 0.5f;

        //Shader.DisableKeyword("HARD_SHADOW");
        //Shader.EnableKeyword("SOFT_SHADOW_2x2");
        //Shader.DisableKeyword("SOFT_SHADOW_4Samples");
        //Shader.DisableKeyword("SOFT_SHADOW_4x4");
    }

	void OnDestroy()
	{
		RenderTexture.DestroyImmediate(depthTexture);
	}

    bool bInited = false;
    int i = 0;
    void LateUpdate()
    {
        i++;
        if ((i %= 100) == 0)
            return;

        if (_isActive != isActive)
        {
            _isActive = isActive;
            if (isActive)
            {
                depthTexture = new RenderTexture(TextureSize, TextureSize, 16, RenderTextureFormat.ARGB32);
                depthTexture.filterMode = FilterMode.Point;
            }
            else
            {
                RenderTexture.DestroyImmediate(depthTexture);
            }
        }
        if (isActive)
        {
            depthCamera.cullingMask = casterLayer;
            depthCamera.orthographicSize = Size;
            depthCamera.farClipPlane = Far;
            depthCamera.Render();
            Matrix4x4 depthProjectionMatrix = depthCamera.projectionMatrix;
            Matrix4x4 depthViewMatrix = depthCamera.worldToCameraMatrix;
            Matrix4x4 depthVP = depthProjectionMatrix * depthViewMatrix;
            Matrix4x4 depthVPBias = biasMatrix * depthVP;
            Shader.SetGlobalMatrix(_depthVPBias, depthVPBias);
            Shader.SetGlobalMatrix(_depthV, depthViewMatrix);
            Shader.SetGlobalTexture(_kkShadowMap, depthCamera.targetTexture);
            Shader.SetGlobalFloat(_bias, Bias);
            Shader.SetGlobalFloat(_strength, 1 - Strength);
            Shader.SetGlobalFloat(_texmapScale, 1f / TextureSize);
            Shader.SetGlobalFloat(_farplaneScale, 1 / Far);

            bInited = true;
        }
    }


}
