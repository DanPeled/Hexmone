/*
  Package Name: Smart Console
  Version: 2.1.3
  Author: EdgarDev
  Unity Asset Profile: https://assetstore.unity.com/publishers/64126
  Date: 2023-03-05
  Script Name: InputHandler.cs

  Description:
  This script handles user inputs and triggers various events
  based on the type of input received. It can handle inputs through
  the Unity Input System or through traditional Unity KeyCode inputs. 
*/

using System;
using TMPro;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ED.SC.Components
{
    public class InputHandler : MonoBehaviour
    {
		[SerializeField] private TMP_InputField InputField;

#if ENABLE_INPUT_SYSTEM
		[SerializeField] private InputAction m_ToggleAction = new InputAction("ToggleAction", InputActionType.Button, "<Keyboard>/escape");
		[SerializeField] private InputAction m_AutocompleteAction = new InputAction("AutocompleteAction", InputActionType.Button, "<Keyboard>/tab");
		[SerializeField] private InputAction m_CopyNextLogMessageAction = new InputAction("CopyNextAction", InputActionType.Button, "<Keyboard>/upArrow");
        [SerializeField] private InputAction m_CopyPreviousLogMessageAction = new InputAction("CopyPreviousAction", InputActionType.Button, "<Keyboard>/downArrow");
		[SerializeField] private InputAction m_SelectParameterAction = new InputAction("SelectParameterAction", InputActionType.Button, "<Keyboard>/shift");

		private bool m_InteractionsEnabled = false;
#else
        [SerializeField] private KeyCode m_ToggleKeyCode = KeyCode.Escape;
        [SerializeField] private KeyCode m_AutocompleteKeyCode = KeyCode.Tab;
        [SerializeField] private KeyCode m_CopyNextLogMessageKeyCode = KeyCode.UpArrow;
        [SerializeField] private KeyCode m_CopyPreviousLogMessageKeyCode = KeyCode.DownArrow;
        [SerializeField] private KeyCode m_SelectParameterKeyCode = KeyCode.LeftShift;
#endif

		public event Action OnOpenCloseInput;
        public event Action OnAutocompleteInput;
        public event Action OnCopyNextLogInput;
        public event Action OnCopyPreviousLogInput;
		public event Action OnSelectParameterInput;
		public event Action<string> OnInputFieldValueChange;

#if ENABLE_INPUT_SYSTEM
		private void Awake()
		{
			m_ToggleAction.performed += OpenCloseInput;

			m_AutocompleteAction.performed += (ctx) => OnAutocompleteInput?.Invoke();
			m_CopyNextLogMessageAction.performed += (ctx) => OnCopyNextLogInput?.Invoke();
			m_CopyPreviousLogMessageAction.performed += (ctx) => OnCopyPreviousLogInput?.Invoke();
			m_SelectParameterAction.performed += (ctx) => OnSelectParameterInput?.Invoke();

			InputField.onValueChanged.AddListener(InputFieldValueChange);
			InputField.onSubmit.AddListener((string inputText) => SubmitInputDispatcher());

			SmartConsole.InputField = InputField;
		}

		private void OnEnable()
		{
			m_ToggleAction.Enable();
		}

		private void OnDisable()
		{
			m_ToggleAction.Disable();
		}

		private void OpenCloseInput(InputAction.CallbackContext ctx = default)
		{
			if (m_InteractionsEnabled)
			{
				m_AutocompleteAction.Disable();
				m_CopyNextLogMessageAction.Disable();
				m_CopyPreviousLogMessageAction.Disable();
				m_SelectParameterAction.Disable();
			}
			else
			{
				m_AutocompleteAction.Enable();
				m_CopyNextLogMessageAction.Enable();
				m_CopyPreviousLogMessageAction.Enable();
				m_SelectParameterAction.Enable();
			}

			m_InteractionsEnabled = !m_InteractionsEnabled;

			OnOpenCloseInput?.Invoke();
		}
#else
		private void Awake()
		{
			InputField.onValueChanged.AddListener(InputFieldValueChange);
			InputField.onSubmit.AddListener((string inputText) => SubmitInputDispatcher());

			SmartConsole.InputField = InputField;
		}

        private void Update()
        {
            if (Input.GetKeyDown(m_ToggleKeyCode))
            {
                OnOpenCloseInput?.Invoke();
            }
            
            if (Input.GetKeyDown(m_AutocompleteKeyCode))
            {
				OnAutocompleteInput?.Invoke();
			}
            
            if (Input.GetKeyDown(m_CopyNextLogMessageKeyCode))
            {
				OnCopyNextLogInput?.Invoke();
			}
            
            if (Input.GetKeyDown(m_CopyPreviousLogMessageKeyCode))
            {
				OnCopyPreviousLogInput?.Invoke();
			}

            if (Input.GetKeyDown(m_SelectParameterKeyCode))
            {
				OnSelectParameterInput?.Invoke();
			}
        }

		private void OpenCloseInput()
		{
			OnOpenCloseInput?.Invoke();
		}
#endif

		private void InputFieldValueChange(string str) =>
            OnInputFieldValueChange?.Invoke(str);

		/// <summary>
		/// Submit for the UnityEvent button assignation
		/// </summary>
		public void SubmitInputDispatcher() =>
			SmartConsole.SubmitInputField();

		/// <summary>
		/// Toggle for the UnityEvent button assignation
		/// </summary>
		public void ToggleInputDispatcher() =>
			OpenCloseInput();
	}
}
