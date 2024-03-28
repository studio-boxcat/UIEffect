using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Coffee.UIEffects
{
    public interface IParameterInstance
    {
        int index { get; set; }
    }

    /// <summary>
    /// Parameter texture.
    /// </summary>
    public class ParameterTexture
    {
        //################################
        // Public Members.
        //################################

        /// <summary>
        /// Initializes a new instance of the <see cref="Coffee.UIEffects.ParameterTexture"/> class.
        /// </summary>
        /// <param name="channels">Channels.</param>
        /// <param name="instanceLimit">Instance limit.</param>
        public ParameterTexture(int channels, int instanceLimit)
        {
            _channels = ((channels - 1) / 4 + 1) * 4;
            _instanceLimit = ((instanceLimit - 1) / 2 + 1) * 2;
            _data = new byte[_channels * _instanceLimit];

            _availableIds = new Stack<int>(_instanceLimit);
            for (var i = 1; i < _instanceLimit + 1; i++)
                _availableIds.Push(i);
        }


        /// <summary>
        /// Register the specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Register(IParameterInstance target)
        {
            Assert.AreEqual(0, target.index, "target is already registered: " + target);

            if (_availableIds.Count is 0)
            {
                L.E("ParameterTexture: Instance limit exceeded.");
                return;
            }

            target.index = _availableIds.Pop();
        }

        /// <summary>
        /// Unregister the specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Unregister(IParameterInstance target)
        {
            var index = target.index;
            if (index is 0)
            {
                L.W("ParameterTexture: Instance is not registered.");
                return;
            }

            _availableIds.Push(index);
            target.index = 0;
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="channelId">Channel identifier.</param>
        /// <param name="value">Value.</param>
        public void SetData(IParameterInstance target, int channelId, float value)
        {
            var index = target.index;
            if (index is 0)
            {
                L.W("ParameterTexture: Instance is not registered.");
                return;
            }

            var dataIndex = (index - 1) * _channels + channelId;
            var valueByte = (byte) (Mathf.Clamp01(value) * 255);
            if (_data[dataIndex] == valueByte) return;
            _data[dataIndex] = valueByte;
            _needUpload = true;
        }

        /// <summary>
        /// Registers the material.
        /// </summary>
        public void SetTextureForMaterial(Material mat, int propertyId)
        {
            Assert.IsNotNull(mat, "Material is null.");
            Assert.IsNotNull(_texture, "Not initialized.");
            mat.SetTexture(propertyId, _texture);
        }

        /// <summary>
        /// Gets the index of the normalized.
        /// </summary>
        /// <returns>The normalized index.</returns>
        /// <param name="target">Target.</param>
        public float GetNormalizedIndex(IParameterInstance target)
        {
            return (target.index - 0.5f) / _instanceLimit;
        }


        //################################
        // Private Members.
        //################################

        Texture2D _texture;
        bool _needUpload;
        readonly int _channels;
        readonly int _instanceLimit;
        readonly byte[] _data;
        readonly Stack<int> _availableIds;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Initialize()
        {
            if (_texture is not null)
                return;

            var isLinear = QualitySettings.activeColorSpace is ColorSpace.Linear;
            _texture = new Texture2D(_channels / 4, _instanceLimit, TextureFormat.RGBA32, false, isLinear)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave, // XXX: To prevent destroying the mesh after exiting play mode.
            };

            _needUpload = true;
            Canvas.willRenderCanvases += UpdateParameterTexture;
        }

        void UpdateParameterTexture()
        {
            if (_needUpload is false) return;
            _needUpload = false;
            _texture.LoadRawTextureData(_data);
            _texture.Apply(false, false);
        }
    }
}