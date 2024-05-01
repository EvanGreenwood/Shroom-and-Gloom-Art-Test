#region
using System.Collections;
using Mainframe;
using TMPro;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
#endregion

namespace Mainframe_Editor
{
// Heirarchy Menu
//----------------------------------------------------------------------------------------------------
[InitializeOnLoad]
public static class HeirarchyMenu
{
    static HeirarchyMenu()
    {
        QuickMenu menu = new QuickMenu("Heirarchy Menu",
                                       OnInput,
                                       typeof(HeirarchyMenuItems));
    }

    public static bool OnInput()
    {
        if(EDITOR.IsMouseOverEditorWindow(UnityEditorWindowType.SceneHierarchyWindow))
        {
            if(Application.isPlaying)
                return false;

            Event e = Event.current;
            if(e != null)
                return e.type == EventType.KeyDown && e.shift && e.keyCode == KeyCode.A;
        }
        return false;
    }
}

// Menu Items
//----------------------------------------------------------------------------------------------------
public static class HeirarchyMenuItems
{
    static IEnumerator RenameRoutine(GameObject target)
    {
        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        yield return new WaitForSecondsRealtime(0.1f);

        var evnt = new Event {keyCode = KeyCode.F2, type = EventType.KeyDown}; // or Event.KeyboardEvent("f2");
        EditorWindow.focusedWindow.SendEvent(evnt);
    }

    static GameObject CreateGameObject(string name)
    {
        Transform parent = null;
        GameObject selected = Selection.activeGameObject;
        if(selected != null) { parent = selected.transform; }

        GameObject child = new GameObject(name);

        child.transform.parent = parent;

        if(parent != null && parent.transform is RectTransform)
        {
            RectTransform rt = child.AddComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            child.layer = parent.gameObject.layer;
        }
        Undo.RegisterCreatedObjectUndo(child, $"Created {name}");
        return child;
    }

    [MenuInvoke("Empty")]
    static void CreateGameObject()
    {
        GameObject child = CreateGameObject("Empty");

        Selection.activeGameObject = child;
        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

    [MenuInvoke("Sprite")]
    static void CreateSpriteRenderer()
    {
        GameObject child = CreateGameObject("Sprite");

        child.AddComponent<SpriteRenderer>();

        Selection.activeGameObject = child;
        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

    [MenuInvoke("Mesh")]
    static void CreateMeshRenderer()
    {
        GameObject child = CreateGameObject("Mesh");

        child.AddComponent<MeshFilter>();
        child.AddComponent<MeshRenderer>();

        Selection.activeGameObject = child;
        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

    [MenuInvoke("Camera", "")]
    static void CreateCamera()
    {
        GameObject child = CreateGameObject("Camera");

        Camera camera = child.AddComponent<Camera>();

        AudioListener listener = Object.FindObjectOfType<AudioListener>();
        if(listener == null)
            child.AddComponent<AudioListener>();

        if(Camera.main == null)
        {
            child.tag = "MainCamera";
            child.name = "MainCamera";
        }

        camera.transform.position = new Vector3(0f, 0f, -10f);

        if(EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
            camera.orthographic = true;
        else camera.orthographic = false;

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = RGB.darkGrey.WithA(0.0f);

        Selection.activeGameObject = child;

        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

#region UI
    [MenuInvoke("UI/Image")]
    static void CreateImage()
    {
        GameObject child = CreateGameObject("Image");
        Image image = child.AddComponent<Image>();
        image.raycastTarget = false;

        Selection.activeGameObject = child;
        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

    [MenuInvoke("UI/Text")]
    static void CreateText()
    {
        GameObject child = CreateGameObject("Text");
        TextMeshProUGUI textMesh = child.AddComponent<TextMeshProUGUI>();
        textMesh.raycastTarget = false;

        Selection.activeGameObject = child;
        EditorCoroutineUtility.StartCoroutine(RenameRoutine(child), child);
    }

    [MenuInvoke("UI/Canvas")]
    static void CreateCanvas()
    {
        GameObject canvasObject = CreateGameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        GraphicRaycaster graphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CAMERA.main;
        canvas.planeDistance = 10;
        canvas.vertexColorAlwaysGammaSpace = true;
        canvas.sortingLayerName = "UI";
        canvas.gameObject.layer = LayerMask.NameToLayer("UI");

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        if(!GameObject.FindObjectOfType<EventSystem>())
        {
            GameObject eventSystemObject = CreateGameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            InputSystemUIInputModule inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
    }
#endregion
}
}
