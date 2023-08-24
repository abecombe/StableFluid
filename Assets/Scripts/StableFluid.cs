using GPUUtil;
using UnityEngine;

public class StableFluid : MonoBehaviour
{
    private GPUDoubleTexture2D _velocityTex = new();
    private GPUTexture2D _divergenceTex = new();
    private GPUDoubleTexture2D _pressureTex = new();
    private GPUTexture2D _renderingTex = new();

    private GPUComputeShader _divergenceCs;
    private GPUComputeShader _pressureCs;
    private GPUComputeShader _velocityUpdateCs;
    private GPUComputeShader _advectionCs;
    private GPUComputeShader _renderingCs;

    private int _width = 512;
    private int _height = 512;

    private Vector2 _prevPointerUV = Vector2.zero;

    private void Start()
    {
        _velocityTex.Init(_width, _height);
        _divergenceTex.Init(_width, _height);
        _pressureTex.Init(_width, _height);
        _renderingTex.Init(_width, _height);

        _divergenceCs = new GPUComputeShader("Divergence");
        _pressureCs = new GPUComputeShader("Pressure");
        _velocityUpdateCs = new GPUComputeShader("VelocityUpdate");
        _advectionCs = new GPUComputeShader("Advection");
        _renderingCs = new GPUComputeShader("Rendering");

        gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = _renderingTex;
    }

    private void Update()
    {
        // Divergence
        var cs = _divergenceCs;
        var k = cs.FindKernel("CalcDivergence");

        cs.SetInts("_Resolution", _width, _height);
        k.SetTexture("_VelocityTex", _velocityTex.Read);
        k.SetTexture("_DivergenceTex", _divergenceTex);

        k.Dispatch(_width, _height);

        // Pressure
        cs = _pressureCs;
        k = cs.FindKernel("CalcPressure");

        cs.SetInts("_Resolution", _width, _height);
        cs.SetFloat("_Alpha", 1f);
        cs.SetFloat("_Beta", 1f);
        k.SetTexture("_DivergenceTex", _divergenceTex);

        for (int i = 0; i < 20; i++)
        {
            k.SetTexture("_PressureTexRead", _pressureTex.Read);
            k.SetTexture("_PressureTexWrite", _pressureTex.Write);

            k.Dispatch(_width, _height);

            _pressureTex.Swap();
        }

        // Velocity Update
        cs = _velocityUpdateCs;
        k = cs.FindKernel("UpdateVelocity");

        cs.SetInts("_Resolution", _width, _height);
        cs.SetFloat("_Time", Time.time);
        k.SetTexture("_VelocityTex", _velocityTex.Read);
        k.SetTexture("_PressureTex", _pressureTex.Read);
        Vector2 pointerUV = GetPointerUV();
        cs.SetVector("_PointerPos", pointerUV);
        cs.SetVector("_PrevPointerPos", _prevPointerUV);
        _prevPointerUV = pointerUV;
        cs.SetFloat("_ForceRadius", 36f);
        cs.SetFloat("_ForceCoefficient", 1f);
        cs.SetFloat("_AutoForceCoefficient", 0.06f);
        cs.SetFloat("_Viscosity", 0.99f);

        k.Dispatch(_width, _height);

        // Advection
        cs = _advectionCs;
        k = cs.FindKernel("Advect");

        cs.SetInts("_Resolution", _width, _height);
        k.SetTexture("_VelocityTexRead", _velocityTex.Read);
        k.SetTexture("_VelocityTexWrite", _velocityTex.Write);

        k.Dispatch(_width, _height);

        _velocityTex.Swap();

        // Rendering
        cs = _renderingCs;
        k = cs.FindKernel("Render");

        cs.SetInts("_Resolution", _width, _height);
        cs.SetFloat("_Time", Time.time);
        k.SetTexture("_VelocityTex", _velocityTex.Read);
        k.SetTexture("_PressureTex", _pressureTex.Read);
        k.SetTexture("_RenderingTex", _renderingTex);

        k.Dispatch(_width, _height);
    }

    private void OnDestroy()
    {
        _velocityTex.Dispose();
        _divergenceTex.Dispose();
        _pressureTex.Dispose();
        _renderingTex.Dispose();
    }

    private Vector2 GetPointerUV()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePos.x + 0.5f, mousePos.y + 0.5f);
    }
}