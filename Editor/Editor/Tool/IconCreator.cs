#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;
using TMPro;
using Jeomseon.Imaging;

namespace Jeomseon.Editor.Tool
{
    internal sealed class IconCreator : EditorWindow
    {
        [MenuItem("Jeomseon/Icon Creator")]
        private static void Init()
        {
            GetWindow<IconCreator>().Show();
        }

        private int _size = 128;
        private int _divideCount = 4;

        private readonly List<Sprite> _iconSources = new();
        private ReorderableList _reorderableIconSources;
        private Texture2D _iconTexture = null;
        private IconCreatorPreset _preset;

        private static string LastPresetPrefsKey =>
            $"Jeomseon.EditorToolkit.IconCreator.LastPreset.{PlayerSettings.productGUID}";

        private void OnEnable()
        {
            string lastPresetPath = EditorPrefs.GetString(LastPresetPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(lastPresetPath))
            {
                _preset = AssetDatabase.LoadAssetAtPath<IconCreatorPreset>(lastPresetPath);
            }

            _reorderableIconSources = new(
                _iconSources,
                typeof(Sprite),
                true,
                true,
                true,
                true)
            {
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    _iconSources[index] = (Sprite)EditorGUI.ObjectField(
                        new(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        _iconSources[index], typeof(Sprite), false);
                },
                onAddCallback = _ =>
                {
                    if (_iconSources.Count >= _divideCount * _divideCount) return;
                    _iconSources.Add(null);
                },
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Icon Sources")
            };
        }

        public void OnGUI()
        {
            DrawPresetControls();

            _size = EditorGUILayout.IntSlider(new GUIContent("Size"), _size, 128, 2048);
            _divideCount = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Divide Count"), _divideCount), 1, 32);

            bool isCreate = GUILayout.Button("Create Icon");
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 3f);

            if (_iconTexture)
            {
                GUILayout.Label(_iconTexture);
            }

            _reorderableIconSources.DoLayoutList();

            if (isCreate && _iconTexture)
            {
                // SpriteRect 메타는 실제로 저장되는 _iconTexture의 픽셀 크기를 기준으로 계산해야 합니다.
                // Preview에서 생성한 atlas 폭은 아이콘 개수가 divideCount보다 적을 때 sizeMax보다 좁을 수 있습니다.
                int atlasWidth = _iconTexture.width;
                int atlasHeight = _iconTexture.height;

                string path = EditorUtility.SaveFilePanelInProject(
                    "Save Atlas",
                    "NewAtlas",
                    "png",
                    "Please enter a file name to save the atlas texture to");

                if (!string.IsNullOrEmpty(path))
                {
                    byte[] pngData = _iconTexture.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path, pngData);
                    AssetDatabase.Refresh();

                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer)
                    {
                        importer.textureType = _preset ? _preset.textureType : TextureImporterType.Sprite;
                        importer.spriteImportMode = _preset ? _preset.spriteImportMode : SpriteImportMode.Multiple;

                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                        // TMP_SpriteAsset 생성
                        Texture2D atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                        if (!atlasTexture)
                        {
                            Debug.LogError("Failed to load the atlas texture.");
                            return;
                        }

                        // SpriteMetaData 설정
                        List<SpriteRect> metas = new();
                        int currentX = 0, currentY = 0;
                        foreach (Sprite sprite in _iconSources)
                        {
                            if (currentX + _size > atlasWidth)
                            {
                                currentX = 0;
                                currentY += _size;
                            }

                            SpriteRect meta = new SpriteRect
                            {
                                name = sprite.name,
                                rect = new(currentX, atlasHeight - currentY - _size, _size, _size),
                                alignment = SpriteAlignment.Center,
                                pivot = new(0.5f, 0.5f)
                            };
                            metas.Add(meta);
                            currentX += _size;
                        }

                        SpriteDataProviderFactories factory = new();
                        factory.Init();
                        ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                        dataProvider.InitSpriteEditorDataProvider();
                        dataProvider.SetSpriteRects(metas.ToArray());
                        dataProvider.Apply();
                        importer.SaveAndReimport();

                        if (_preset == null || _preset.generateTmpSpriteAsset)
                        {
                            // 스프라이트 아틀라스에서 스프라이트 목록을 가져오기
                            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();
                            if (sprites.Length == 0)
                            {
                                Debug.LogError("No sprites found in the atlas.");
                                return;
                            }

                            // TMP_SpriteAsset 생성
                            TMP_SpriteAsset spriteAsset = CreateInstance<TMP_SpriteAsset>();
                            spriteAsset.spriteSheet = atlasTexture;
                            spriteAsset.spriteInfoList = new();

                            foreach (Sprite sprite in sprites)
                            {
                                TMP_Sprite tmpSprite = new TMP_Sprite
                                {
                                    id = spriteAsset.spriteInfoList.Count,
                                    name = sprite.name,
                                    x = sprite.rect.x,
                                    y = sprite.rect.y,
                                    width = sprite.rect.width,
                                    height = sprite.rect.height,
                                    pivot = sprite.pivot,
                                    sprite = sprite
                                };

                                spriteAsset.spriteInfoList.Add(tmpSprite);
                            }

                            // TMP_SpriteAsset 저장
                            string assetPath = System.IO.Path.ChangeExtension(path, ".asset");
                            AssetDatabase.CreateAsset(spriteAsset, assetPath);
                            AssetDatabase.SaveAssets();

                            Debug.Log("TMP_SpriteAsset created at: " + assetPath);
                        }
                    }
                }
            }

            if (GUILayout.Button("Preview"))
            {
                int sizeMax = _size * _divideCount;
                int rows = (_iconSources.Count + _divideCount - 1) / _divideCount;
                int atlasWidth = Mathf.Min(_size * _iconSources.Count, sizeMax);
                int atlasHeight = rows * _size;

                Texture2D atlasTexture = new(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);

                Color[][] resizeSourceColors = _iconSources
                    .Select(sprite =>
                    {
                        if (!sprite) return null;

                        // sprite.texture는 packing/sheet 원본 전체 텍스처이므로,
                        // 이 스프라이트가 차지하는 부분 영역(textureRect)만 잘라내야 합니다.
                        Rect textureRect = sprite.textureRect;
                        int rectX = Mathf.RoundToInt(textureRect.x);
                        int rectY = Mathf.RoundToInt(textureRect.y);
                        int rectWidth = Mathf.RoundToInt(textureRect.width);
                        int rectHeight = Mathf.RoundToInt(textureRect.height);

                        int width = sprite.texture.width;
                        int height = sprite.texture.height;

                        RenderTexture renderTexture = new(width, height, 0)
                        {
                            useMipMap = false,
                            autoGenerateMips = false
                        };

                        RenderTexture.active = renderTexture;
                        Graphics.Blit(sprite.texture, renderTexture);

                        Texture2D readableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                        readableTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                        readableTexture.Apply();

                        RenderTexture.active = null;
                        renderTexture.Release();

                        Color[] spritePixels = readableTexture.GetPixels(rectX, rectY, rectWidth, rectHeight);
                        DestroyImmediate(readableTexture);

                        return TexturePixelResampler.ResizeToFit(
                            spritePixels,
                            rectWidth,
                            rectHeight,
                            _size,
                            _size);
                    })
                    .Where(pixels => pixels is not null)
                    .ToArray();

                int currentX = 0, currentY = 0;
                foreach (Color[] t in resizeSourceColors)
                {
                    if (currentX + _size > atlasTexture.width)
                    {
                        currentX = 0;
                        currentY += _size;
                    }

                    if (currentY + _size > atlasTexture.height)
                    {
                        Debug.LogWarning("아틀라스 텍스처의 크기를 초과하여 더 이상 스프라이트를 추가할 수 없습니다.");
                        break;
                    }

                    atlasTexture.SetPixels(currentX, currentY, _size, _size, t);
                    currentX += _size;
                }

                if (currentX < atlasTexture.width || currentY + _size < atlasTexture.height)
                {
                    int remainingWidth = atlasTexture.width - currentX;
                    int remainingHeight = _size; // 마지막 줄의 높이만큼만 투명하게 채움

                    if (remainingWidth > 0)
                    {
                        Color[] transparentPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), remainingWidth * remainingHeight).ToArray();
                        atlasTexture.SetPixels(currentX, currentY, remainingWidth, remainingHeight, transparentPixels);
                    }

                    if (currentY + _size < atlasTexture.height)
                    {
                        remainingHeight = atlasTexture.height - (currentY + _size);
                        Color[] transparentPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), atlasTexture.width * remainingHeight).ToArray();
                        atlasTexture.SetPixels(0, currentY + _size, atlasTexture.width, remainingHeight, transparentPixels);
                    }
                }

                atlasTexture.Apply();
                _iconTexture = atlasTexture;
            }
        }

        private void DrawPresetControls()
        {
            IconCreatorPreset picked = (IconCreatorPreset)EditorGUILayout.ObjectField(
                new GUIContent("Preset"), _preset, typeof(IconCreatorPreset), false);
            if (picked != _preset)
            {
                _preset = picked;
                EditorPrefs.SetString(LastPresetPrefsKey, _preset ? AssetDatabase.GetAssetPath(_preset) : string.Empty);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!_preset))
                {
                    if (GUILayout.Button("Load Preset")) LoadPreset();
                }
                if (GUILayout.Button("Save As Preset")) SaveAsPreset();
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        private void LoadPreset()
        {
            if (!_preset) return;

            _size = _preset.size;
            _divideCount = _preset.divideCount;
            _iconSources.Clear();
            _iconSources.AddRange(_preset.defaultIconSources);
        }

        private void SaveAsPreset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Icon Creator Preset",
                "IconCreatorPreset",
                "asset",
                "프리셋으로 저장할 위치를 선택하세요");
            if (string.IsNullOrEmpty(path)) return;

            IconCreatorPreset preset = AssetDatabase.LoadAssetAtPath<IconCreatorPreset>(path);
            bool isNew = !preset;
            if (isNew) preset = CreateInstance<IconCreatorPreset>();

            preset.size = _size;
            preset.divideCount = _divideCount;
            preset.defaultIconSources = new List<Sprite>(_iconSources);

            if (isNew)
            {
                AssetDatabase.CreateAsset(preset, path);
            }
            EditorUtility.SetDirty(preset);
            AssetDatabase.SaveAssets();

            _preset = preset;
            EditorPrefs.SetString(LastPresetPrefsKey, path);
        }
    }
}
#endif
