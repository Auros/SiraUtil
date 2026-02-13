namespace SiraUtil.Tools.FPFC
{
    internal interface IFPFCManager : IFPFCSettings
    {
        new float FOV { get; }

        new float MoveSensitivity { get; }

        new float MouseSensitivity { get; }

        bool InvertY { get; }

        new bool LockViewOnDisable { get; }

        new bool LimitFrameRate { get; }

        new int VSyncCount { get; }

        void Add(CameraController cameraController);

        void Remove(CameraController cameraController);
    }
}
