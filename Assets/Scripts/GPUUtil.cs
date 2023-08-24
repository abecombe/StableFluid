using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace GPUUtil
{
    public class GPUBuffer<T> : IDisposable
    {
        public GraphicsBuffer Data => _buffer;
        public int Size => _buffer.count;
        public int Bytes => _buffer.count * _buffer.stride;

        private GraphicsBuffer _buffer;
        private bool _inited = false;

        public void Init(int size)
        {
            Dispose();
            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, size, Marshal.SizeOf(typeof(T)));
            _inited = true;
        }

        public void Dispose()
        {
            if (_inited) _buffer.Release();
            _inited = false;
        }

        public void CheckSizeChanged(int size)
        {
            if (!_inited || Size != size)
            {
                Init(size);
            }
        }

        public void SetData(T[] data)
        {
            _buffer.SetData(data);
        }
        public void SetData(T[] data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
        {
            _buffer.SetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count);
        }
        public void GetData(T[] data)
        {
            _buffer.GetData(data);
        }
        public void GetData(T[] data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
        {
            _buffer.GetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count);
        }

        public static implicit operator GraphicsBuffer(GPUBuffer<T> buffer)
        {
            return buffer.Data;
        }
    }

    public class GPUDoubleBuffer<T> : IDisposable
    {
        public GPUBuffer<T> Read => _bufferRead;
        public GPUBuffer<T> Write => _bufferWrite;
        public int Size => _bufferRead.Size;

        private GPUBuffer<T> _bufferRead = new();
        private GPUBuffer<T> _bufferWrite = new();
        private bool _inited = false;

        public void Init(int size)
        {
            Dispose();
            _bufferRead.Init(size);
            _bufferWrite.Init(size);
            _inited = true;
        }

        public void Dispose()
        {
            if (_inited) _bufferRead.Dispose();
            if (_inited) _bufferWrite.Dispose();
            _inited = false;
        }

        public void CheckSizeChanged(int size)
        {
            if (!_inited || Size != size)
            {
                Init(size);
            }
        }

        public void Swap()
        {
            (_bufferRead, _bufferWrite) = (_bufferWrite, _bufferRead);
        }

        public void SetData(T[] data)
        {
            _bufferRead.SetData(data);
        }
        public void SetData(T[] data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
        {
            _bufferRead.SetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count);
        }
        public void GetReadData(T[] data)
        {
            _bufferRead.GetData(data);
        }
        public void GetReadData(T[] data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
        {
            _bufferRead.GetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count);
        }
        public void GetWriteData(T[] data)
        {
            _bufferWrite.GetData(data);
        }
        public void GetWriteData(T[] data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
        {
            _bufferWrite.GetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count);
        }
    }

    public class GPUTexture2D : IDisposable
    {
        public RenderTexture Data => _tex;
        public int Width => _tex.width;
        public int Height => _tex.height;
        public RenderTextureFormat Format => _tex.format;
        public FilterMode FilterMode => _tex.filterMode;
        public TextureWrapMode WrapMode => _tex.wrapMode;

        private RenderTexture _tex;
        private bool _inited = false;

        public void Init(int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGBFloat, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            Dispose();
            _tex = new RenderTexture(width, height, 0, format)
            {
                filterMode = filterMode,
                wrapMode = wrapMode,
                enableRandomWrite = true
            };
            _tex.Create();
            _inited = true;
        }

        public void Dispose()
        {
            if (_inited) _tex.Release();
            _inited = false;
        }

        public void CheckSizeChanged(int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGBFloat, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            if (!_inited)
            {
                Init(width, height, format, filterMode, wrapMode);
            }
            else if (Width != width || Height != height)
            {
                Init(width, height, Format, FilterMode, WrapMode);
            }
        }

        public static implicit operator RenderTexture(GPUTexture2D tex)
        {
            return tex.Data;
        }

        public static implicit operator RenderTargetIdentifier(GPUTexture2D tex)
        {
            return tex.Data;
        }
    }

    public class GPUDoubleTexture2D : IDisposable
    {
        public GPUTexture2D Read => _texRead;
        public GPUTexture2D Write => _texWrite;
        public int Width => _texRead.Width;
        public int Height => _texRead.Height;
        public RenderTextureFormat Format => _texRead.Format;
        public FilterMode FilterMode => _texRead.FilterMode;
        public TextureWrapMode WrapMode => _texRead.WrapMode;

        private GPUTexture2D _texRead = new();
        private GPUTexture2D _texWrite = new();
        private bool _inited = false;

        public void Init(int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGBFloat, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            Dispose();
            _texRead.Init(width, height, format, filterMode, wrapMode);
            _texWrite.Init(width, height, format, filterMode, wrapMode);
            _inited = true;
        }

        public void Dispose()
        {
            if (_inited) _texRead.Dispose();
            if (_inited) _texWrite.Dispose();
            _inited = false;
        }

        public void CheckSizeChanged(int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGBFloat, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            if (!_inited)
            {
                Init(width, height, format, filterMode, wrapMode);
            }
            else if (Width != width || Height != height)
            {
                Init(width, height, Format, FilterMode, WrapMode);
            }
        }

        public void Swap()
        {
            (_texRead, _texWrite) = (_texWrite, _texRead);
        }
    }

    public class GPUComputeShader
    {
        private ComputeShader _cs;
        private Dictionary<string, GPUKernel> _kernels = new();

        public GPUComputeShader(ComputeShader cs)
        {
            _cs = cs;
        }
        public GPUComputeShader(string csName)
        {
            _cs = Resources.Load<ComputeShader>(csName);
        }

        public GPUKernel FindKernel(string name)
        {
            if (_kernels.TryGetValue(name, out var kernel))
                return kernel;

            kernel = new GPUKernel(_cs, name);
            _kernels.Add(name, kernel);
            return kernel;
        }

        #region SetInt
        public void SetInt(string name, int value)
        {
            _cs.SetInt(name, value);
        }
        public void SetInt(string name, uint value)
        {
            _cs.SetInt(name, (int)value);
        }
        #endregion

        #region SetInts
        public void SetInts(string name, params int[] value)
        {
            _cs.SetInts(name, value);
        }
        #endregion

        #region SetFloat
        public void SetFloat(string name, int value)
        {
            _cs.SetFloat(name, value);
        }
        public void SetFloat(string name, float value)
        {
            _cs.SetFloat(name, value);
        }
        #endregion

        #region SetVector
        public void SetVector(string name, Vector2 value)
        {
            _cs.SetVector(name, value);
        }
        public void SetVector(string name, Vector3 value)
        {
            _cs.SetVector(name, value);
        }
        public void SetVector(string name, Vector4 value)
        {
            _cs.SetVector(name, value);
        }
        #endregion

        #region SetMatrix
        public void SetMatrix(string name, Matrix4x4 matrix)
        {
            _cs.SetMatrix(name, matrix);
        }
        #endregion

        #region SetKeyword
        public void EnableKeyword(string keyword)
        {
            _cs.EnableKeyword(keyword);
        }
        public void DisableKeyword(string keyword)
        {
            _cs.DisableKeyword(keyword);
        }
        #endregion
    }

    public class GPUKernel
    {
        private ComputeShader _cs;
        private string _name;
        private int _id;
        private uint _threadSizeX;
        private uint _threadSizeY;
        private uint _threadSizeZ;

        public GPUKernel(ComputeShader cs, string name)
        {
            _cs = cs;
            _name = name;
            _id = cs.FindKernel(name);
            cs.GetKernelThreadGroupSizes(_id, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
        }

        #region SetBuffer
        public void SetBuffer(string name, GraphicsBuffer buffer)
        {
            _cs.SetBuffer(_id, name, buffer);
        }
        public void SetBuffer(string name, ComputeBuffer buffer)
        {
            _cs.SetBuffer(_id, name, buffer);
        }
        #endregion

        #region SetTexture
        public void SetTexture(string name, Texture tex)
        {
            _cs.SetTexture(_id, name, tex);
        }
        #endregion

        #region Dispatch
        public void Dispatch(int sizeX, int sizeY = 1, int sizeZ = 1)
        {
            int groupSizeX = (int)Mathf.Ceil(sizeX / (float)_threadSizeX);
            int groupSizeY = (int)Mathf.Ceil(sizeY / (float)_threadSizeY);
            int groupSizeZ = (int)Mathf.Ceil(sizeZ / (float)_threadSizeZ);
            _cs.SetInts("_NumThreads", sizeX, sizeY, sizeZ);
            _cs.SetInts("_NumGroups", groupSizeX, groupSizeY, groupSizeZ);
            _cs.Dispatch(_id, groupSizeX, groupSizeY, groupSizeZ);
        }
        #endregion
    }
}