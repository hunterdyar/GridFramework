using System;
using System.Collections.Generic;
using Bloops.GridFramework.Managers;
using UnityEditor;
using UnityEngine;

//Figured this out by copying in this repo by ATHellboy: https://github.com/ATHellboy/ScriptableObjectDropdown/
//then just ripping out its insides and making it just a dropdown for my little thing.
[CustomPropertyDrawer(typeof(PuzzleManager))]
public class GameLoopManagerDrawer : PropertyDrawer
{
	private readonly int _controlHint = typeof(PuzzleManager).GetHashCode();
	private readonly GenericMenu.MenuFunction2 _onSelectedGameLoopManager;
	private GUIContent _popupContent = new GUIContent();
	private bool _isChanged;
	private int _selectedControlID;
	private PuzzleManager _selectedPuzzleManager;

	private List<PuzzleManager> _glms = new List<PuzzleManager>();
	
	public GameLoopManagerDrawer()
	{
		_onSelectedGameLoopManager = OnSelectedGameLoopManager;

		EditorApplication.projectChanged += ClearCache;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (_glms.Count == 0)
		{
			GetGLMS();
		}

		Draw(position, label, property);
	}
	private void Draw(Rect position, GUIContent label, SerializedProperty property)
	{
		if (label != null && label != GUIContent.none)
		{
			position = EditorGUI.PrefixLabel(position, label);
		}

		if (_glms.Count == 0)
		{
			EditorGUI.LabelField(position, "This type asset does not exist in the project");
			return;
		}

		UpdateSelectionControl(position, label, property);
	}
	
	private void UpdateSelectionControl(Rect position, GUIContent label, SerializedProperty property)
	{
		SerializedProperty value = property;//,getpropertyRelativeValue??
		
		ScriptableObject output = DrawSelectionControl(position, label,value.objectReferenceValue as PuzzleManager);

		if (_isChanged)
		{
			_isChanged = false;
			value.objectReferenceValue = output;
		}
	}
	
	private PuzzleManager DrawSelectionControl(Rect position, GUIContent label, PuzzleManager glm)
    {
        bool triggerDropDown = false;
        int controlID = GUIUtility.GetControlID(_controlHint, FocusType.Keyboard, position);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.ExecuteCommand:
                if (Event.current.commandName == "GameLoopManagerReferenceUpdated")
                {
                    if (_selectedControlID == controlID)
                    {
                        if (glm != _selectedPuzzleManager)
                        {
                            glm = _selectedPuzzleManager;
                            _isChanged = true;
                        }

                        _selectedControlID = 0;
                        _selectedPuzzleManager = null;
                    }
                }
                break;

            case EventType.MouseDown:
                if (GUI.enabled && position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.keyboardControl = controlID;
                    triggerDropDown = true;
                    Event.current.Use();
                }
                break;

            case EventType.KeyDown:
                if (GUI.enabled && GUIUtility.keyboardControl == controlID)
                {
                    if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                    {
                        triggerDropDown = true;
                        Event.current.Use();
                    }
                }
                break;

            case EventType.Repaint:
                if (glm == null)
                {
                    _popupContent.text = "Nothing";
                }
                else
                {
                    _popupContent.text = glm.name;
                }
                EditorStyles.popup.Draw(position, _popupContent, controlID);
                break;
        }

        if (_glms.Count != 0 && triggerDropDown)
        {
            _selectedControlID = controlID;
            _selectedPuzzleManager = glm;

            DisplayDropDown(position, glm);
        }

        return glm;
    }
	private void DisplayDropDown(Rect position, ScriptableObject selectedScriptableObject )
	{
		var menu = new GenericMenu();
		
		menu.AddItem(new GUIContent("Nothing"), selectedScriptableObject == null, _onSelectedGameLoopManager, null);
		menu.AddSeparator("");

		for (int i = 0; i < _glms.Count; ++i)
		{
			var glm = _glms[i];
			
			var content = new GUIContent(glm.name);
			menu.AddItem(content, glm == selectedScriptableObject, _onSelectedGameLoopManager, glm);
		}

		menu.DropDown(position);
	}
	private void ClearCache()
	{
		_glms.Clear();
	}
	private void OnSelectedGameLoopManager(object userData)
	{
		_selectedPuzzleManager = userData as PuzzleManager;
		var scriptableObjectReferenceUpdatedEvent = EditorGUIUtility.CommandEvent("GameLoopManagerReferenceUpdated");
		EditorWindow.focusedWindow.SendEvent(scriptableObjectReferenceUpdatedEvent);
	}
	private void GetGLMS()
	{
		string[] guids = AssetDatabase.FindAssets(String.Format("t:{0}", typeof(PuzzleManager)));
		for (int i = 0; i < guids.Length; i++)
		{
			_glms.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(PuzzleManager)) as PuzzleManager);
		}
	}
	
}
	