using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace IronTide.Input
{
    /// <summary>
    /// Mobile twin-stick input.
    /// Left half of screen = move joystick.
    /// Right half of screen = aim joystick.
    /// Tap ability buttons via UI (IInputHandler ability bools set by UI buttons calling NotifyAbility()).
    /// </summary>
    public class MobileInputHandler : IInputHandler
    {
        public Vector2 MoveInput            { get; private set; }
        public Vector3 AimDirection         { get; private set; }
        public bool    PrimaryFireHeld      { get; private set; }
        public bool    SecondaryFirePressed { get; private set; }
        public bool    Ability1Pressed      { get; private set; }
        public bool    Ability2Pressed      { get; private set; }
        public bool    Ability3Pressed      { get; private set; }

        // Called by on-screen ability buttons (linked in HUD prefab)
        private bool _ability1ThisFrame, _ability2ThisFrame, _ability3ThisFrame;
        private bool _secondaryThisFrame;

        public MobileInputHandler()
        {
            EnhancedTouchSupport.Enable();
        }

        public void Tick()
        {
            ReadTwinStick();

            // Consume one-shot ability flags
            Ability1Pressed      = _ability1ThisFrame;  _ability1ThisFrame      = false;
            Ability2Pressed      = _ability2ThisFrame;  _ability2ThisFrame      = false;
            Ability3Pressed      = _ability3ThisFrame;  _ability3ThisFrame      = false;
            SecondaryFirePressed = _secondaryThisFrame; _secondaryThisFrame     = false;
        }

        private void ReadTwinStick()
        {
            float screenMidX = Screen.width * 0.5f;
            Vector2 leftInput  = Vector2.zero;
            Vector2 rightInput = Vector2.zero;

            foreach (var touch in Touch.activeTouches)
            {
                var pos    = touch.screenPosition;
                var delta  = touch.delta.normalized;

                if (pos.x < screenMidX)
                    leftInput  = delta;
                else
                    rightInput = delta;
            }

            MoveInput = leftInput;

            // Convert right stick to world-space aim direction (flat Y=0 plane)
            if (rightInput.sqrMagnitude > 0.01f)
            {
                AimDirection   = new Vector3(rightInput.x, 0f, rightInput.y).normalized;
                PrimaryFireHeld = true;
            }
            else
            {
                PrimaryFireHeld = false;
                // Keep last aim direction so ship doesn't snap back
            }
        }

        // Called by on-screen UI buttons
        public void NotifyAbility1() => _ability1ThisFrame = true;
        public void NotifyAbility2() => _ability2ThisFrame = true;
        public void NotifyAbility3() => _ability3ThisFrame = true;
        public void NotifySecondary() => _secondaryThisFrame = true;
    }
}
