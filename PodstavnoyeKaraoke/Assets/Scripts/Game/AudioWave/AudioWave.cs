using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using FileUtility = Utilities.Files.File;

namespace Game.AudioWave
{
    public class AudioWave : MonoBehaviour
    {
        private const int MinTextureSize = 1;
        private const string CacheVersion = "v3";

        private static readonly Dictionary<string, Sprite> CachedSprites = new Dictionary<string, Sprite>();
        private static readonly Color WaveColor = new Color(0f, 0.78f, 1f, 1f);
        private static readonly Color BackgroundColor = new Color(0f, 0f, 0f, 0f);

        [SerializeField] private Image _image;
        [SerializeField] private Slider _progressSlider;

        private Coroutine _buildCoroutine;
        private readonly Vector3[] _sliderCorners = new Vector3[4];

        private void Awake()
        {
            ResolveImage();
            HideImage();
        }

        public void SetProgressSlider(Slider progressSlider)
        {
            _progressSlider = progressSlider;
        }

        public void SetAudioClip(string audioPath)
        {
            if (_buildCoroutine != null)
                StopCoroutine(_buildCoroutine);

            ResolveImage();
            HideImage();
            _buildCoroutine = StartCoroutine(SetAudioClipAfterLayout(audioPath));
        }

        private IEnumerator SetAudioClipAfterLayout(string audioPath)
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            MatchSliderWidth();
            SetAudioClipNow(audioPath);
            _buildCoroutine = null;
        }

        private void SetAudioClipNow(string audioPath)
        {
            ResolveImage();

            if (_image == null || string.IsNullOrEmpty(audioPath))
                return;

            var width = GetImageSize(true);
            var height = GetImageSize(false);
            var cacheKey = $"{CacheVersion}_{audioPath}_{width}_{height}";

            if (!CachedSprites.TryGetValue(cacheKey, out var sprite) || sprite == null)
            {
                try
                {
                    var waveData = LoadWaveData(audioPath);
                    if (waveData.Samples == null || waveData.Samples.Length <= 0)
                        return;

                    sprite = CreateWaveSprite(waveData, width, height);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[AudioWave] Failed to create waveform for '{audioPath}'. {exception}");
                    return;
                }

                CachedSprites[cacheKey] = sprite;
            }

            _image.sprite = sprite;
            _image.type = Image.Type.Simple;
            _image.preserveAspect = false;
            _image.raycastTarget = false;
            _image.color = Color.white;
            _image.enabled = true;
        }

        public void Clear()
        {
            HideImage();
        }

        private void ResolveImage()
        {
            if (_image == null)
                _image = GetComponentInChildren<Image>(true);
        }

        private void HideImage()
        {
            if (_image == null)
                return;

            _image.enabled = false;
            _image.sprite = null;
            _image.raycastTarget = false;
        }

        private void MatchSliderWidth()
        {
            var waveRect = transform as RectTransform;
            if (_progressSlider == null || waveRect == null)
                return;

            var parent = waveRect.parent as RectTransform;
            if (parent == null)
                return;

            var sliderRect = GetSliderReferenceRect();
            if (sliderRect == null)
                return;

            sliderRect.GetWorldCorners(_sliderCorners);

            var camera = GetCanvasCamera();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(camera, _sliderCorners[0]), camera, out var minPoint) ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(camera, _sliderCorners[2]), camera, out var maxPoint))
                return;

            var anchoredPosition = waveRect.anchoredPosition;
            anchoredPosition.x = (minPoint.x + maxPoint.x) * 0.5f;
            waveRect.anchoredPosition = anchoredPosition;
            waveRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(maxPoint.x - minPoint.x));
        }

        private RectTransform GetSliderReferenceRect()
        {
            var handleArea = _progressSlider.handleRect == null ? null : _progressSlider.handleRect.parent as RectTransform;
            if (handleArea != null)
                return handleArea;

            var fillArea = _progressSlider.fillRect == null ? null : _progressSlider.fillRect.parent as RectTransform;
            if (fillArea != null)
                return fillArea;

            return _progressSlider.GetComponent<RectTransform>();
        }

        private Camera GetCanvasCamera()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }

        private Sprite CreateWaveSprite(WaveData waveData, int width, int height)
        {
            var texture = CreateWaveTexture(waveData, width, height);
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f));
        }

        private int GetImageSize(bool width)
        {
            var rectTransform = _image.rectTransform;
            var result = width ? rectTransform.rect.width : rectTransform.rect.height;

            if (result <= MinTextureSize)
                result = width ? rectTransform.sizeDelta.x : rectTransform.sizeDelta.y;

            var parent = rectTransform.parent as RectTransform;
            if (result <= MinTextureSize && parent != null)
                result = width ? parent.rect.width : parent.rect.height;

            return Mathf.Max(MinTextureSize, Mathf.RoundToInt(result));
        }

        private Texture2D CreateWaveTexture(WaveData waveData, int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            FillTexture(texture, BackgroundColor);

            var centerY = height / 2;
            var halfHeight = Mathf.Max(1, centerY);
            var samplesPerPixel = Mathf.Max(1, waveData.SamplesCount / width);
            var amplitudes = GetWaveAmplitudes(waveData, width, samplesPerPixel);
            var maxAmplitude = GetMaxAmplitude(amplitudes);
            if (maxAmplitude <= 0f)
                maxAmplitude = 1f;

            for (int x = 0; x < width; x++)
            {
                var amplitude = Mathf.Sqrt(Mathf.Clamp01(amplitudes[x] / maxAmplitude));
                var waveHeight = Mathf.RoundToInt(amplitude * halfHeight);

                for (int y = -waveHeight; y <= waveHeight; y++)
                {
                    var pixelY = centerY + y;
                    if (pixelY >= 0 && pixelY < height)
                        texture.SetPixel(x, pixelY, WaveColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private WaveData LoadWaveData(string audioPath)
        {
            var bytes = File.ReadAllBytes(FileUtility.GetGlobalPath(audioPath));
            var formatChunk = FindChunk(bytes, "fmt ");
            var dataChunk = FindChunk(bytes, "data");

            if (formatChunk < 0 || dataChunk < 0)
                return new WaveData();

            var formatOffset = formatChunk + 8;
            var channels = Mathf.Max(1, BitConverter.ToUInt16(bytes, formatOffset + 2));
            var bitDepth = BitConverter.ToUInt16(bytes, formatOffset + 14);
            var dataOffset = dataChunk + 8;
            var dataSize = BitConverter.ToInt32(bytes, dataChunk + 4);

            dataSize = Mathf.Min(dataSize, bytes.Length - dataOffset);

            return new WaveData
            {
                Samples = ConvertWaveBytesToSamples(bytes, dataOffset, dataSize, bitDepth),
                Channels = channels
            };
        }

        private int FindChunk(byte[] bytes, string chunkName)
        {
            for (int i = 12; i < bytes.Length - 8;)
            {
                var currentChunkName = System.Text.Encoding.ASCII.GetString(bytes, i, 4);
                var chunkSize = BitConverter.ToInt32(bytes, i + 4);
                if (chunkSize < 0)
                    return -1;

                if (currentChunkName == chunkName)
                    return i;

                i += 8 + chunkSize + chunkSize % 2;
            }

            return -1;
        }

        private float[] ConvertWaveBytesToSamples(byte[] bytes, int dataOffset, int dataSize, ushort bitDepth)
        {
            if (bitDepth == 16)
                return Convert16BitWaveBytesToSamples(bytes, dataOffset, dataSize);

            Debug.LogError($"[AudioWave] Unsupported wav bit depth: {bitDepth}.");
            return new float[0];
        }

        private float[] Convert16BitWaveBytesToSamples(byte[] bytes, int dataOffset, int dataSize)
        {
            var samples = new float[dataSize / sizeof(short)];
            var maxValue = (float)short.MaxValue;

            for (int i = 0; i < samples.Length; i++)
                samples[i] = BitConverter.ToInt16(bytes, dataOffset + i * sizeof(short)) / maxValue;

            return samples;
        }

        private float[] GetWaveAmplitudes(WaveData waveData, int width, int samplesPerPixel)
        {
            var amplitudes = new float[width];

            for (int x = 0; x < width; x++)
            {
                var startSample = x * samplesPerPixel;
                var endSample = Mathf.Min(waveData.SamplesCount, startSample + samplesPerPixel);
                amplitudes[x] = GetMaxAmplitude(waveData, startSample, endSample);
            }

            return amplitudes;
        }

        private float GetMaxAmplitude(float[] amplitudes)
        {
            var result = 0f;

            for (int i = 0; i < amplitudes.Length; i++)
                result = Mathf.Max(result, amplitudes[i]);

            return result;
        }

        private float GetMaxAmplitude(WaveData waveData, int startSample, int endSample)
        {
            var result = 0f;

            for (int sample = startSample; sample < endSample; sample++)
            {
                var channelSum = 0f;
                var sampleIndex = sample * waveData.Channels;

                for (int channel = 0; channel < waveData.Channels; channel++)
                {
                    if (sampleIndex + channel >= waveData.Samples.Length)
                        break;

                    channelSum += Mathf.Abs(waveData.Samples[sampleIndex + channel]);
                }

                result = Mathf.Max(result, channelSum / waveData.Channels);
            }

            return Mathf.Clamp01(result);
        }

        private void FillTexture(Texture2D texture, Color color)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, color);
            }
        }

        private struct WaveData
        {
            public float[] Samples;
            public int Channels;
            public int SamplesCount => Samples == null || Channels <= 0 ? 0 : Samples.Length / Channels;
        }
    }
}
