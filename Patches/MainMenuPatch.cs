﻿using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ProjectM.UI;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class MainMenuPatch
{
    /*
    static MainMenuCanvasBase _mainMenuCanvasBase;

    static GameObject _retroCameraLogoObject;
    static RectTransform _retroCameraLogoRect;
    static Image _retroCameraLogoImage;
    static Image _cameraFlashImage;

    static GameObject _discordButtonObject;
    static GameObject _kofiButtonObject;
    static GameObject _patreonButtonObject;

    static Image _discordButtonImage;
    static Image _kofiButtonImage;
    static Image _patreonButtonImage;
    static bool _consoleReady = false;
    */

    // WorldVFXVolumeComponent

    /*
    [HarmonyPatch(typeof(MainMenuNewView), nameof(MainMenuNewView.Update))]
    [HarmonyPrefix]
    static void UpdatePrefix(MainMenuNewView __instance)
    {
        if (_consoleReady && _retroCameraLogoObject == null)
        {
            // Find the canvas base
            _mainMenuCanvasBase = UnityEngine.Object.FindObjectOfType<MainMenuCanvasBase>();
            _mainMenuNewView = __instance;

            if (_mainMenuCanvasBase != null)
            {
                // Create a new GameObject for texture
                _retroCameraLogoObject = new("RetroCameraLogo");
                GameObject.DontDestroyOnLoad(_retroCameraLogoObject);

                // rectTransform, set parent
                _retroCameraLogoRect = _retroCameraLogoObject.AddComponent<RectTransform>();
                _retroCameraLogoObject.transform.SetParent(_mainMenuCanvasBase.MenuParent, false);
                _retroCameraLogoObject.transform.SetAsLastSibling();

                // load texture from png
                Assembly assembly = Assembly.GetExecutingAssembly();
                using Stream stream = assembly.GetManifestResourceStream(CAMERA_LOGO_PATH);

                // Make circular sprite for masking
                Texture2D texture = stream.LoadTextureFromStream();
                Sprite logoSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);

                // Add an Image component to the timer object
                _retroCameraLogoImage = _retroCameraLogoObject.AddComponent<Image>();
                _retroCameraLogoImage.sprite = logoSprite;

                // Instead of radial fill, just use a simple Image
                _retroCameraLogoImage.type = Image.Type.Simple;
                _retroCameraLogoImage.color = Color.white;

                // Set the RectTransform to properly size and position the timer in the UI
                _retroCameraLogoRect.sizeDelta = new Vector2(texture.width / 2, texture.height / 2);
                _retroCameraLogoRect.localScale = Vector3.one;

                // Center the moon logo
                _retroCameraLogoRect.anchorMin = new Vector2(0.5f, 0.75f);   
                _retroCameraLogoRect.anchorMax = new Vector2(0.5f, 0.75f);
                _retroCameraLogoRect.pivot = new Vector2(0.5f, 0.5f);        // Pivot at center
                _retroCameraLogoRect.anchoredPosition = new Vector2(0f, 0f); // Ensures it's exactly centered

                // Child object for the glow
                GameObject glowObject = new("RetroCameraGlow");
                glowObject.transform.SetParent(_retroCameraLogoObject.transform, false);

                // Get the child's RectTransform
                RectTransform glowRect = glowObject.AddComponent<RectTransform>();

                // Optional: Make the glow bigger than the main sprite
                glowRect.sizeDelta = _retroCameraLogoRect.sizeDelta * 2.25f;
                glowRect.anchoredPosition = Vector2.zero;

                // Load the glow sprite
                using Stream streamGlow = assembly.GetManifestResourceStream(CAMERA_GLOW_PATH);
                Texture2D glowTexture = streamGlow.LoadTextureFromStream();
                Sprite glowSprite = Sprite.Create(
                    glowTexture,
                    new Rect(0, 0, glowTexture.width, glowTexture.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );

                // Add Image for the glow
                _cameraFlashImage = glowObject.AddComponent<Image>();
                _cameraFlashImage.sprite = glowSprite;
                _cameraFlashImage.type = Image.Type.Simple;

                // Start transparent or visible depending on _autoContinue.Value
                _cameraFlashImage.color = new Color(1f, 1f, 1f, 0f);

                // Make sure this draws behind the main logo:
                glowObject.transform.SetAsFirstSibling();

                // Set active
                _retroCameraLogoObject.SetActive(true);

                // Core.Log.LogWarning($"[MainMenuNewView.Update()] Preparing social buttons...");
                InitializeSocialButtons(__instance);
            }
        }
    }
    */

    static MainMenuNewView _mainMenuNewView;

    [HarmonyPatch(typeof(MainMenuNewView), nameof(MainMenuNewView.SetConsoleReady))] // reset bools and object states when exiting world
    [HarmonyPostfix]
    static void SetConsoleReadyPostfix()
    {
        try
        {
            Core.Log.LogWarning($"[MainMenuNewView.SetConsoleReady]");
            _mainMenuNewView = UnityEngine.Object.FindObjectOfType<MainMenuNewView>();
            _newsPanel = _mainMenuNewView.News;

            if (_newsPanel._NewsEntries.Elements.Count != 0 || _newsPanel._NewsManagerSystem._NewsData.News.Count != 0)
            {
                Reset();
            }

            InitializeNewsEntries();

            /*
            if (!Localization.Initialized)
            {
                // not ready if menu opened quickly enough, but if the rest of the menu is fine and presumably localized what's the rub?
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization isn't ready yet! Attempting manual loading...");

                try
                {
                    Localization.LoadDefaultLanguage();
                }
                catch (Exception ex)
                {
                    Core.Log.LogError($"[OptionsPanel_Interface.Start()] Failed to load localization: {ex.Message}");
                }
            }
            else
            {
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization is ready!");
            }
            */
        }
        catch (Exception ex)
        {
            Core.Log.LogWarning($"[MainMenuNewView.SetConsoleReady] error: {ex}");
        }
    }
    public static void Reset()
    {
        // _newsPanel._NewsEntries.EndUpdate();

        foreach (var newsEntry in _newsPanel._NewsEntries._Elements_k__BackingField)
        {
            if (newsEntry != null) GameObject.Destroy(newsEntry.gameObject);
        }

        foreach (var newsEntry in _newsPanel._NewsEntries.Elements)
        {
            if (newsEntry != null) GameObject.Destroy(newsEntry.gameObject);
        }

        _newsPanel._NewsEntries._Elements_k__BackingField.Clear();
        _newsPanel._NewsEntries.Elements.Clear();
        _newsPanel._NewsManagerSystem._NewsData.News.Clear();

        // _mainMenuNewView = null;
        // _newsPanel = null;
    }

    const string DISCORD_LOGO_PATH = "RetroCamera.Resources.ModDiscordLogo.png";
    const string CAMERA_LOGO_PATH = "RetroCamera.Resources.RetroCameraLogo.png";
    // const string CAMERA_LOGO_PATH = "RetroCamera.Resources.RetroCamLogoFull.png";
    const string CAMERA_GLOW_PATH = "RetroCamera.Resources.Glow03.png";

    const string DISCORD_BUTTON = "Modding Discord";
    const string DISCORD_LINK = "https://discord.com/invite/QG2FmueAG9";

    const string DONATION_BUTTON = "Support Development";
    const string KOFI_LINK = "https://ko-fi.com/zfolmt";
    const string PATREON_LINK = "https://www.patreon.com/c/zfolmt";

    static readonly float _aspectRatio = Screen.width / Screen.height;

    static NewsPanelEntry _newsPanelEntry;
    static NewsPanel _newsPanel;
    static bool _newsPanelInitialized = false;
    static void InitializeNewsEntries()
    {
        Core.Log.LogWarning($"[InitializeNewsEntries()] Initializing NewsPanel!");

        if (_newsPanel == null)
        {
            Core.Log.LogWarning($"[InitializeNewsEntries()] NewsPanel is null!");
            return;
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        CreateNewsEntry(
            assembly,
            DISCORD_LOGO_PATH,
            DISCORD_BUTTON,
            DISCORD_LINK
        );

        CreateNewsEntry(
            assembly,
            CAMERA_LOGO_PATH,
            DONATION_BUTTON,
            KOFI_LINK
        );

        Core.Log.LogWarning($"[InitializeNewsEntries()] NewsEntries - {_newsPanel._NewsEntries.Elements.Count}");
        _newsPanel._NewsEntries.StartUpdate();
    }
    static void CreateNewsEntry(
    Assembly assembly,
    string resourcePath,
    string buttonText,
    string urlToOpen
    )
    {
        if (!TryGet2DTextureFromResource(assembly, resourcePath, out Texture2D texture)) return;
        // Core.Log.LogWarning($"[CreateNewsEntry()] Loaded texture from path - {resourcePath}");

        NewsPanelEntry newsEntry = _newsPanel._NewsEntries.InstantiateElement();
        SimpleStunButton imageButton = newsEntry.ImageButton;
        NewsPanelEntryData entryData = new();

        if (newsEntry == null || entryData == null)
        {
            Core.Log.LogWarning($"[CreateNewsEntry()] NewsPanelEntry and/or NewsPanelEntryData null!");
            return;
        }

        entryData.Duration = 15f;
        entryData._HasLink = true;
        entryData.LinkURL = urlToOpen;
        newsEntry._Data = entryData;
        newsEntry._Texture = texture;
        newsEntry.Image.m_Texture = texture;
        newsEntry.Image.m_Color = Color.white;
        newsEntry.Image.m_UVRect = new Rect(-0.475f, -0.125f, 1.95f, 1.25f);
        // newsEntry._WebRequest

        /*
        RawImage rawImage = newsEntry.Image;

        float textureWidth = texture.width;
        float textureHeight = texture.height;

        RectTransform rt = rawImage.rectTransform;
        float rectWidth = rt.rect.width;
        float rectHeight = rt.rect.height;

        float textureAspect = textureWidth / textureHeight;
        float rectAspect = rectWidth / rectHeight;

        Rect uv = new(0, 0, 1, 1);

        if (textureAspect > rectAspect)
        {
            // Texture is wider than the container: letterbox vertically
            float scale = rectAspect / textureAspect;
            uv.height = scale;
            uv.y = (1f - scale) / 2f;
        }
        else
        {
            // Texture is taller than the container: letterbox horizontally
            float scale = textureAspect / rectAspect;
            uv.width = scale;
            uv.x = (1f - scale) / 2f;
        }

        rawImage.uvRect = uv;
        */

        // Core.Log.LogWarning($"[CreateNewsEntry()] Instantiated NewsPanelEntry with NewsPanelEntryData...");

        // LocalizedText localizedText = newsEntry.ImageButton.gameObject.transform.GetChild();
        // localizedText?.ForceSet(buttonText);

        GameObject buttonTitleObject = FindTargetGameObject(imageButton.gameObject.transform, "Title");
        GameObject inputContextEntriesObject = FindTargetGameObject(imageButton.gameObject.transform, "OpenLinkInputContextEntry");

        if (buttonTitleObject == null)
        {
            Core.Log.LogWarning($"[CreateNewsEntry()] Title is null!");
        }

        if (inputContextEntriesObject == null)
        {
            Core.Log.LogWarning($"[CreateNewsEntry()] OpenLinkInputContextEntry is null!");
        }

        TextMeshProUGUI textMeshPro = buttonTitleObject?.GetComponent<TextMeshProUGUI>();
        textMeshPro?.SetText(buttonText);

        inputContextEntriesObject?.SetActive(false);

        if (textMeshPro == null)
        {
            Core.Log.LogWarning($"[CreateNewsEntry()] TextMeshProUGUI is null!");
        }

        /*
        if (urlToOpen.Any())
        {
            List<string> urls = [..urlToOpen];
            SimpleStunButton stunButton = newsEntry.ImageButton;
            stunButton.onClick.AddListener((UnityAction)(() => OpenURLs(urls)));
            // Core.Log.LogWarning($"[CreateNewsEntry()] Added urls to ImageButton...");
        }
        */

        // _newsPanel._NewsEntries.Elements.Add(newsEntry);
        _newsPanel._NewsManagerSystem._NewsData.News.Add(newsEntry._Data);
        // newsEntry.gameObject.SetActive(false);
    }
    static bool TryGetSpriteFromResource(Assembly assembly, string resourcePath, out Sprite sprite)
    {
        using Stream stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream != null)
        {
            Texture2D texture = stream.LoadTextureFromStream();

            if (texture != null)
            {
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                return true;
            }
        }
        else
        {
            Core.Log.LogError($"Failed to load resource - {resourcePath}");
        }

        sprite = default;
        return false;
    }
    static bool TryGet2DTextureFromResource(Assembly assembly, string resourcePath, out Texture2D texture)
    {
        using Stream stream = assembly?.GetManifestResourceStream(resourcePath);

        if (stream != null)
        {
            texture = stream?.LoadTextureFromStream();

            if (texture != null)
            {
                return true;
            }
        }
        else
        {
            Core.Log.LogError($"Failed to load resource - {resourcePath}");
        }

        texture = default;
        return false;
    }
    /*
    public static Texture2D LoadTextureFromStream(this Stream stream, FilterMode filterMode = FilterMode.Bilinear)
    {
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);

        Texture2D texture2D = new(2, 2, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(texture2D, array);

        texture2D.filterMode = filterMode;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        return texture2D;
    }
    */
    public static Texture2D LoadTextureFromStream(this Stream stream, FilterMode filterMode = FilterMode.Bilinear)
    {
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);

        Texture2D texture2D = new(2, 2, TextureFormat.RGBA32, mipChain: false, linear: false);
        ImageConversion.LoadImage(texture2D, array, markNonReadable: false);

        texture2D.filterMode = filterMode;               // Bilinear or Trilinear
        texture2D.wrapMode = TextureWrapMode.Clamp;      // No tiling

        // CleanTransparentPixels(texture2D);
        return texture2D;
    }
    public static void CleanTransparentPixels(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a <= 0.001f)
            {
                pixels[i].r = 0f;
                pixels[i].g = 0f;
                pixels[i].b = 0f;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
    static void OpenURLs(List<string> urls)
    {
        foreach (string url in urls)
        {
            Application.OpenURL(url);
        }
    }
    public static GameObject FindTargetGameObject(Transform root, string targetName)
    {
        // Stack to hold the transforms to be processed
        Stack<(Transform transform, int indentLevel)> transformStack = new();
        transformStack.Push((root, 0));

        // HashSet to keep track of visited transforms to avoid cyclic references
        HashSet<Transform> visited = [];

        Il2CppArrayBase<Transform> children = root.GetComponentsInChildren<Transform>(true);

        List<Transform> transforms = [..children];

        while (transformStack.Count > 0)
        {
            var (current, indentLevel) = transformStack.Pop();

            if (!visited.Add(current))
            {
                // If we have already visited this transform, skip it
                continue;
            }

            if (current.gameObject.name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
            {
                // Return the transform if the name matches
                return current.gameObject;
            }

            // Create an indentation string based on the indent level
            //string indent = new('|', indentLevel);

            // Print the current GameObject's name and some basic info
            //Core.Log.LogInfo($"{indent}{current.gameObject.name} ({current.gameObject.scene.name})");

            // Add all children to the stack
            foreach (Transform child in transforms)
            {
                if (child.parent == current)
                {
                    transformStack.Push((child, indentLevel + 1));
                }
            }
        }

        Core.Log.LogWarning($"GameObject with name '{targetName}' not found!");
        return null;
    }
    public static void FindGameObjects(Transform root, string filePath = "", bool includeInactive = false)
    {
        // Stack to hold the transforms to be processed
        Stack<(Transform transform, int indentLevel)> transformStack = new();
        transformStack.Push((root, 0));

        // HashSet to keep track of visited transforms to avoid cyclic references
        HashSet<Transform> visited = [];

        Il2CppArrayBase<Transform> children = root.GetComponentsInChildren<Transform>(includeInactive);
        List<Transform> transforms = [.. children];

        Core.Log.LogWarning($"Found {transforms.Count} GameObjects!");

        if (string.IsNullOrEmpty(filePath))
        {
            while (transformStack.Count > 0)
            {
                var (current, indentLevel) = transformStack.Pop();

                if (!visited.Add(current))
                {
                    // If we have already visited this transform, skip it
                    continue;
                }

                List<string> objectComponents = FindGameObjectComponents(current.gameObject);

                // Create an indentation string based on the indent level
                string indent = new('|', indentLevel);

                // Write the current GameObject's name and some basic info to the file
                Core.Log.LogInfo($"{indent}{current.gameObject.name} | {string.Join(",", objectComponents)} | [{current.gameObject.scene.name}]");

                // Add all children to the stack
                foreach (Transform child in transforms)
                {
                    if (child.parent == current)
                    {
                        transformStack.Push((child, indentLevel + 1));
                    }
                }
            }
            return;
        }

        if (!File.Exists(filePath)) File.Create(filePath).Dispose();

        using StreamWriter writer = new(filePath, false);
        while (transformStack.Count > 0)
        {
            var (current, indentLevel) = transformStack.Pop();

            if (!visited.Add(current))
            {
                // If we have already visited this transform, skip it
                continue;
            }

            List<string> objectComponents = FindGameObjectComponents(current.gameObject);

            // Create an indentation string based on the indent level
            string indent = new('|', indentLevel);

            // Write the current GameObject's name and some basic info to the file
            writer.WriteLine($"{indent}{current.gameObject.name} | {string.Join(",", objectComponents)} | [{current.gameObject.scene.name}]");

            // Add all children to the stack
            foreach (Transform child in transforms)
            {
                if (child.parent == current)
                {
                    transformStack.Push((child, indentLevel + 1));
                }
            }
        }
    }
    public static List<string> FindGameObjectComponents(GameObject parentObject)
    {
        List<string> components = [];

        int componentCount = parentObject.GetComponentCount();
        for (int i = 0; i < componentCount; i++)
        {
            components.Add($"{parentObject.GetComponentAtIndex(i).GetIl2CppType().FullName}({i})");
        }

        return components;
    }
}
