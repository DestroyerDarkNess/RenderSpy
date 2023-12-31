﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderSpy.Graphics.d3d10
{
    public enum D3D10DeviceVTbl : short
    {
        // IUnknown
        QueryInterface = 0,
        AddRef = 1,
        Release = 2,

        // ID3D10Device
        VSSetConstantBuffers = 3,
        PSSetShaderResources = 4,
        PSSetShader = 5,
        PSSetSamplers = 6,
        VSSetShader = 7,
        DrawIndexed = 8,
        Draw = 9,
        PSSetConstantBuffers = 10,
        IASetInputLayout = 11,
        IASetVertexBuffers = 12,
        IASetIndexBuffer = 13,
        DrawIndexedInstanced = 14,
        DrawInstanced = 15,
        GSSetConstantBuffers = 16,
        GSSetShader = 17,
        IASetPrimitiveTopology = 18,
        VSSetShaderResources = 19,
        VSSetSamplers = 20,
        SetPredication = 21,
        GSSetShaderResources = 22,
        GSSetSamplers = 23,
        OMSetRenderTargets = 24,
        OMSetBlendState = 25,
        OMSetDepthStencilState = 26,
        SOSetTargets = 27,
        DrawAuto = 28,
        RSSetState = 29,
        RSSetViewports = 30,
        RSSetScissorRects = 31,
        CopySubresourceRegion = 32,
        CopyResource = 33,
        UpdateSubresource = 34,
        ClearRenderTargetView = 35,
        ClearDepthStencilView = 36,
        GenerateMips = 37,
        ResolveSubresource = 38,
        VSGetConstantBuffers = 39,
        PSGetShaderResources = 40,
        PSGetShader = 41,
        PSGetSamplers = 42,
        VSGetShader = 43,
        PSGetConstantBuffers = 44,
        IAGetInputLayout = 45,
        IAGetVertexBuffers = 46,
        IAGetIndexBuffer = 47,
        GSGetConstantBuffers = 48,
        GSGetShader = 49,
        IAGetPrimitiveTopology = 50,
        VSGetShaderResources = 51,
        VSGetSamplers = 52,
        GetPredication = 53,
        GSGetShaderResources = 54,
        GSGetSamplers = 55,
        OMGetRenderTargets = 56,
        OMGetBlendState = 57,
        OMGetDepthStencilState = 58,
        SOGetTargets = 59,
        RSGetState = 60,
        RSGetViewports = 61,
        RSGetScissorRects = 62,
        GetDeviceRemovedReason = 63,
        SetExceptionMode = 64,
        GetExceptionMode = 65,
        GetPrivateData = 66,
        SetPrivateData = 67,
        SetPrivateDataInterface = 68,
        ClearState = 69,
        Flush = 70,
        CreateBuffer = 71,
        CreateTexture1D = 72,
        CreateTexture2D = 73,
        CreateTexture3D = 74,
        CreateShaderResourceView = 75,
        CreateRenderTargetView = 76,
        CreateDepthStencilView = 77,
        CreateInputLayout = 78,
        CreateVertexShader = 79,
        CreateGeometryShader = 80,
        CreateGemoetryShaderWithStreamOutput = 81,
        CreatePixelShader = 82,
        CreateBlendState = 83,
        CreateDepthStencilState = 84,
        CreateRasterizerState = 85,
        CreateSamplerState = 86,
        CreateQuery = 87,
        CreatePredicate = 88,
        CreateCounter = 89,
        CheckFormatSupport = 90,
        CheckMultisampleQualityLevels = 91,
        CheckCounterInfo = 92,
        CheckCounter = 93,
        GetCreationFlags = 94,
        OpenSharedResource = 95,
        SetTextFilterSize = 96,
        GetTextFilterSize = 97,
    }

}
