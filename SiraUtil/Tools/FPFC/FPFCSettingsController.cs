using BeatSaber.Init;
using BGLib.DotnetExtension.CommandLine;
using IPA.Utilities.Async;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCSettingsController : IFPFCManager, IFPFCSettings, IInitializable, IDisposable
    {
        private readonly FPFCOptions _fpfcOptions;
        private readonly IXRSystemState _xrSystemState;
        private readonly XRHelper _xrHelper;

        private readonly InputAction _toggleAction;
        private readonly List<CameraController> _cameraControllers = [];

        private readonly Action<UnityXRSystemState, XRSystemEventType> invokeOnChangeStateEvent;

        public bool Ignore { [Obsolete] get; private set; }
        public float FOV => _fpfcOptions.CameraFOV;
        public float MoveSensitivity => _fpfcOptions.MoveSensitivity;
        public float MouseSensitivity => _fpfcOptions.MouseSensitivity;
        public bool InvertY => _fpfcOptions.InvertY;

        public bool Enabled
        {
            get;
            set
            {
                field = value;

                ApplyState();

                Changed?.Invoke(this);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Active));
            }
        }

        public bool Active => Enabled && _cameraControllers.Count > 0;

        public bool LockViewOnDisable => _fpfcOptions.LockViewOnDisable;

        public bool LimitFrameRate => _fpfcOptions.LimitFrameRate;

        public int VSyncCount => _fpfcOptions.VSyncCount;

        public event Action<IFPFCSettings>? Changed;

        public event PropertyChangedEventHandler? PropertyChanged;

        public FPFCSettingsController(FPFCOptions fpfcOptions, IXRSystemState xrSystemState, XRHelper xrHelper, CommandLineParserResult commandLineParserResult)
        {
            _fpfcOptions = fpfcOptions;
            _xrSystemState = xrSystemState;
            _xrHelper = xrHelper;
            _toggleAction = new InputAction("FPFC Toggle", binding: $"<Keyboard>/{fpfcOptions.ToggleKeyCode}");
            Ignore = !(Enabled = commandLineParserResult.Contains(InitArguments.kFPFCOption));

            MethodInfo invokeAction = typeof(Action<XRSystemEventType>).GetMethod("Invoke");
            ParameterExpression systemStateParam = Expression.Parameter(typeof(UnityXRSystemState), "xrSystemState");
            ParameterExpression eventTypeParam = Expression.Parameter(typeof(XRSystemEventType), "xrSystemEventType");
            MemberExpression eventField = Expression.Field(systemStateParam, "_onChangeStateEvent");
            invokeOnChangeStateEvent = Expression.Lambda<Action<UnityXRSystemState, XRSystemEventType>>(Expression.IfThen(Expression.Not(Expression.Equal(eventField, Expression.Constant(null))), Expression.Call(eventField, invokeAction, eventTypeParam)), systemStateParam, eventTypeParam).Compile();
        }

        public void Initialize()
        {
            _fpfcOptions.Updated += ConfigUpdated;
            _toggleAction.performed += OnToggleActionPerformed;
        }

        public void Dispose()
        {
            _fpfcOptions.Updated -= ConfigUpdated;

            _toggleAction.performed -= OnToggleActionPerformed;
            _toggleAction.Dispose();
        }

        public void Add(CameraController cameraController)
        {
            if (_cameraControllers.Contains(cameraController))
            {
                return;
            }

            _cameraControllers.Add(cameraController);

            _toggleAction.Enable();

            ApplyState();

            Changed?.Invoke(this);
            NotifyPropertyChanged(nameof(Active));
        }

        public void Remove(CameraController cameraController)
        {
            if (!_cameraControllers.Remove(cameraController))
            {
                return;
            }

            if (_cameraControllers.Count == 0)
            {
                _toggleAction.Disable();
            }

            ApplyState();

            Changed?.Invoke(this);
            NotifyPropertyChanged(nameof(Active));
        }

        private void ApplyState()
        {
            Cursor.lockState = Active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Active;

            Application.targetFrameRate = Enabled && LimitFrameRate ? (int)Math.Round(Screen.currentResolution.refreshRateRatio.value) : -1;
            QualitySettings.vSyncCount = Enabled ? VSyncCount : 0;

            if (Enabled)
            {
                _xrHelper.DeinitializeXRLoader();
            }
            else
            {
                _xrHelper.InitializeXRLoader();
            }
        }

        private void ConfigUpdated(FPFCOptions options)
        {
            UnityMainThreadTaskScheduler.Factory.StartNew(() => _toggleAction.ApplyBindingOverride($"<Keyboard>/{options.ToggleKeyCode}"));

            Changed?.Invoke(this);
            NotifyPropertyChanged(null);
        }

        private void OnToggleActionPerformed(InputAction.CallbackContext ctx)
        {
            // enabling/disabling the XR loader causes input devices to change; doing that inside an input update
            // leaves actions bound to stale devices, so defer until the current input update has finished
            UnityMainThreadTaskScheduler.Factory.StartNew(Toggle);
        }

        private void Toggle()
        {
            bool hadInputFocus = _xrSystemState.hasInputFocus;

            Enabled = !Enabled;

            bool hasInputFocus = _xrSystemState.hasInputFocus;

            if (hadInputFocus != hasInputFocus && _xrSystemState is UnityXRSystemState unityXRSystemState)
            {
                invokeOnChangeStateEvent(unityXRSystemState, hasInputFocus ? XRSystemEventType.InputFocusAcquired : XRSystemEventType.InputFocusLost);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
