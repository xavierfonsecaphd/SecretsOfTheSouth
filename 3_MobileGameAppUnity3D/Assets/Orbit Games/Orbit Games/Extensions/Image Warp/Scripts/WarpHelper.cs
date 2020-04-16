using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fenderrio.ImageWarp
{
	public static class WarpHelper
	{
		private const string kUILayerName = "UI";

		private const string kStandardSpritePath       = "UI/Skin/UISprite.psd";
		private const string kBackgroundSpritePath     = "UI/Skin/Background.psd";
		private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
		private const string kKnobPath                 = "UI/Skin/Knob.psd";
		private const string kCheckmarkPath            = "UI/Skin/Checkmark.psd";
		private const string kDropdownArrowPath        = "UI/Skin/DropdownArrow.psd";
		private const string kMaskPath                 = "UI/Skin/UIMask.psd";

		static private DefaultControls.Resources s_StandardResources;

		public static bool ConvertImageToImageWarp(GameObject imageObject)
		{
			Image imageComponent = imageObject.GetComponent<Image> ();
			ImageWarp imageWarpComponent = imageObject.GetComponent<ImageWarp> ();

			if(imageWarpComponent != null)
				return false;

			GameObject tempObject = new GameObject("temp");
			imageWarpComponent = tempObject.AddComponent<ImageWarp>();

			imageComponent.CopyComponentTo(imageWarpComponent);

			MonoBehaviour.DestroyImmediate (imageComponent);

			ImageWarp newImageWarpComponent = imageObject.AddComponent<ImageWarp> ();

			imageWarpComponent.CopyComponentTo (newImageWarpComponent);

			MonoBehaviour.DestroyImmediate (tempObject);

			return true;
		}


		public static bool ConvertRawImageToRawImageWarp(GameObject imageObject)
		{
			RawImage imageComponent = imageObject.GetComponent<RawImage> ();
			RawImageWarp imageWarpComponent = imageObject.GetComponent<RawImageWarp> ();

			if(imageWarpComponent != null)
				return false;

			GameObject tempObject = new GameObject("temp");
			imageWarpComponent = tempObject.AddComponent<RawImageWarp>();

			imageComponent.CopyComponent(imageWarpComponent);

			MonoBehaviour.DestroyImmediate (imageComponent);

			RawImageWarp newImageWarpComponent = imageObject.AddComponent<RawImageWarp> ();

			imageWarpComponent.CopyComponent (newImageWarpComponent);

			MonoBehaviour.DestroyImmediate (tempObject);

			return true;
		}


		public static void CopyComponentTo(this Image imageFrom, Image imageTo)
		{
			imageTo.sprite = imageFrom.sprite;
			imageTo.color = imageFrom.color;
			imageTo.material = imageFrom.material;
			imageTo.enabled = imageFrom.enabled;
			imageTo.raycastTarget = imageFrom.raycastTarget;
			imageTo.type = imageFrom.type;
			imageTo.preserveAspect = imageFrom.preserveAspect;
			imageTo.fillCenter = imageFrom.fillCenter;
			imageTo.fillMethod = imageFrom.fillMethod;
			imageTo.fillAmount = imageFrom.fillAmount;
			imageTo.fillClockwise = imageFrom.fillClockwise;
			imageTo.fillOrigin = imageFrom.fillOrigin;
		}


		public static void CopyComponent(this RawImage imageFrom, RawImage imageTo)
		{
			imageTo.texture = imageFrom.texture;
			imageTo.color = imageFrom.color;
			imageTo.material = imageFrom.material;
			imageTo.enabled = imageFrom.enabled;
			imageTo.raycastTarget = imageFrom.raycastTarget;
			imageTo.uvRect = imageFrom.uvRect;
		}




#if UNITY_EDITOR
		[MenuItem("GameObject/UI/Image Warp", false, 2002)]
		static public void AddImage(MenuCommand menuCommand)
		{
			GameObject go = DefaultControls.CreateImage(GetStandardResources());

			go.name = "Image Warp";

			WarpHelper.ConvertImageToImageWarp (go);

			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/UI/Raw Image Warp", false, 2003)]
		static public void AddRawImage(MenuCommand menuCommand)
		{
			GameObject go = DefaultControls.CreateRawImage(GetStandardResources());

			go.name = "RawImage Warp";

			WarpHelper.ConvertRawImageToRawImageWarp (go);

			PlaceUIElementRoot(go, menuCommand);
		}

		[MenuItem("GameObject/3D Object/Native Image Warp", false, 2003)]
		static public void AddNativeImageRenderer(MenuCommand menuCommand)
		{
			GameObject go = new GameObject();
			go.AddComponent<NativeImageWarp> ();

			go.name = "Native Image Warp";

			Selection.activeGameObject = go;
		}


		static private DefaultControls.Resources GetStandardResources()
		{
			if (s_StandardResources.standard == null)
			{
				s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
				s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
				s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
				s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
				s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
				s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
				s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
			}
			return s_StandardResources;
		}

		private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			if (parent == null || parent.GetComponentInParent<Canvas>() == null)
			{
				parent = GetOrCreateCanvasGameObject();
			}

			string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
			element.name = uniqueName;
			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
			Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
			GameObjectUtility.SetParentAndAlign(element, parent);
			if (parent != menuCommand.context) // not a context click, so center in sceneview
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

			Selection.activeGameObject = element;
		}

		// Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
		static public GameObject GetOrCreateCanvasGameObject()
		{
			GameObject selectedGo = Selection.activeGameObject;

			// Try to find a gameobject that is the selected GO or one if its parents.
			Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in selection or its parents? Then use just any canvas..
			canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in the scene at all? Then create a new one.
			return CreateNewUI();
		}

		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;

			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;

			// Create world space Plane from canvas position.
			Vector2 localPlanePosition;
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}

			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}

		static public GameObject CreateNewUI()
		{
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

			// if there is no event system add one...
			CreateEventSystem(false);
			return root;
		}

		static void CreateEventSystem(MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			CreateEventSystem(true, parent);
		}

		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}

		private static void CreateEventSystem(bool select, GameObject parent)
		{
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null)
			{
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}

			if (select && esys != null)
			{
				Selection.activeGameObject = esys.gameObject;
			}
		}

		public static bool FixLegacyBezierData(GameObject imageObject)
		{
			Component iWarpComponent = imageObject.GetComponent (typeof(IWarp));

			if (iWarpComponent != null)
			{
				IWarp iWarp = iWarpComponent as IWarp;

				iWarp.topBezierHandleA -= iWarp.cornerOffsetTL;
				iWarp.topBezierHandleB -= iWarp.cornerOffsetTR;

				iWarp.bottomBezierHandleA -= iWarp.cornerOffsetBR;
				iWarp.bottomBezierHandleB -= iWarp.cornerOffsetBL;

				iWarp.leftBezierHandleA -= iWarp.cornerOffsetBL;
				iWarp.leftBezierHandleB -= iWarp.cornerOffsetTL;

				iWarp.rightBezierHandleA -= iWarp.cornerOffsetTR;
				iWarp.rightBezierHandleB -= iWarp.cornerOffsetBR;
			}
			else
			{
				Debug.Log ("No ImageWarp component found. Make sure you have the desired GameObject selected.");
				return false;
			}

			return true;
		}

#endif
	}
}