using UnityEngine;
using IronTide.Core;

namespace IronTide.Input
{
    /// <summary>
    /// Detects platform at runtime and instantiates the appropriate IInputHandler.
    /// Registers itself with the ServiceLocator so ShipController can retrieve it.
    /// </summary>
    public class InputRouter : MonoBehaviour
    {
        [Header("Force Override (leave None in production)")]
        [SerializeField] private InputPlatformOverride forceOverride = InputPlatformOverride.None;

        public IInputHandler ActiveHandler { get; private set; }

        private PCInputHandler     _pcHandler;
        private MobileInputHandler _mobileHandler;

        private void Awake()
        {
            _pcHandler     = new PCInputHandler();
            _mobileHandler = new MobileInputHandler();

            ActiveHandler = ResolveHandler();
            ServiceLocator.Register<InputRouter>(this);
        }

        private void OnDestroy() => ServiceLocator.Unregister<InputRouter>();

        private void Update() => ActiveHandler?.Tick();

        private IInputHandler ResolveHandler()
        {
            return forceOverride switch
            {
                InputPlatformOverride.PC     => _pcHandler,
                InputPlatformOverride.Mobile => _mobileHandler,
                _                            => IsMobilePlatform() ? (IInputHandler)_mobileHandler : _pcHandler
            };
        }

        private static bool IsMobilePlatform() =>
            Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer;
    }

    public enum InputPlatformOverride { None, PC, Mobile }
}
