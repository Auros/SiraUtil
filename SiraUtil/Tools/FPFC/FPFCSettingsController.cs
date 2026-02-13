using BeatSaber.Init;
using BGLib.DotnetExtension.CommandLine;
using IPA.Utilities.Async;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class FPFCSettingsController : IFPFCManager, IFPFCSettings, IInitializable, IDisposable
    {
        private readonly FPFCOptions _fpfcOptions;
        private readonly SiraLog _siraLog;
        private readonly IVRPlatformHelper _vrPlatformHelper;

        private readonly Action<IVRPlatformHelper> invokeInputFocusWasCapturedEvent;
        private readonly Action<IVRPlatformHelper> invokeInputFocusWasReleasedEvent;

        private readonly InputAction _toggleAction;
        private readonly List<CameraController> _cameraControllers = [];

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

        public FPFCSettingsController(FPFCOptions fpfcOptions, SiraLog siraLog, IVRPlatformHelper vrPlatformHelper, CommandLineParserResult commandLineParserResult)
        {
            _fpfcOptions = fpfcOptions;
            _siraLog = siraLog;
            _vrPlatformHelper = vrPlatformHelper;
            _toggleAction = new InputAction("FPFC Toggle", binding: $"<Keyboard>/{fpfcOptions.ToggleKeyCode}");
            Ignore = !(Enabled = commandLineParserResult.Contains(InitArguments.kFPFCOption));

            MethodInfo invokeAction = typeof(Action).GetMethod("Invoke");
            ParameterExpression parameter = Expression.Parameter(typeof(IVRPlatformHelper), "vrPlatformHelper");
            UnaryExpression cast = Expression.Convert(parameter, vrPlatformHelper.GetType());
            ConstantExpression nullConstant = Expression.Constant(null);

            MemberExpression capturedField = Expression.Field(cast, nameof(IVRPlatformHelper.inputFocusWasCapturedEvent));
            invokeInputFocusWasCapturedEvent = Expression.Lambda<Action<IVRPlatformHelper>>(Expression.IfThen(Expression.Not(Expression.Equal(capturedField, nullConstant)), Expression.Call(capturedField, invokeAction)), parameter).Compile();

            MemberExpression releasedField = Expression.Field(cast, nameof(IVRPlatformHelper.inputFocusWasReleasedEvent));
            invokeInputFocusWasReleasedEvent = Expression.Lambda<Action<IVRPlatformHelper>>(Expression.IfThen(Expression.Not(Expression.Equal(releasedField, nullConstant)), Expression.Call(releasedField, invokeAction)), parameter).Compile();
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
                DeinitializeXRLoader();
            }
            else
            {
                InitializeXRLoader();
            }
        }

        private void ConfigUpdated(FPFCOptions options)
        {
            UnityMainThreadTaskScheduler.Factory.StartNew(() => _toggleAction.ApplyBindingOverride($"<Keyboard>/{options.ToggleKeyCode}"));

            Changed?.Invoke(this);
            NotifyPropertyChanged(null);
        }

        // we unfortunately need to fully deinitialize/initialize the XR loader since OpenXR doesn't simply stop/start properly
        private void InitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null || !manager.activeLoaders.Any(l => l != null))
            {
                return;
            }

            _siraLog.Notice("Enabling XR Loader");
            manager.InitializeLoaderSync();

            if (!manager.isInitializationComplete)
            {
                _siraLog.Error("Failed to initialize XR loader");
                return;
            }

            manager.StartSubsystems();
        }

        private void DeinitializeXRLoader()
        {
            XRManagerSettings manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader == null)
            {
                return;
            }

            _siraLog.Notice("Disabling XR Loader");
            manager.DeinitializeLoader();
        }

        private void OnToggleActionPerformed(InputAction.CallbackContext ctx)
        {
            bool hadInputFocus = _vrPlatformHelper.hasInputFocus;

            Enabled = !Enabled;

            bool hasInputFocus = _vrPlatformHelper.hasInputFocus;

            if (hadInputFocus != hasInputFocus)
            {
                if (hasInputFocus)
                {
                    invokeInputFocusWasReleasedEvent(_vrPlatformHelper);
                }
                else
                {
                    invokeInputFocusWasCapturedEvent(_vrPlatformHelper);
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
