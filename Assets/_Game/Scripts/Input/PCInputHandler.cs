using UnityEngine;
using UnityEngine.InputSystem;

namespace IronTide.Input
{
    /// <summary>
    /// PC input: WASD movement, mouse-aim on water plane, scroll zoom, middle-drag camera rotate.
    /// Uses Unity's new Input System via Keyboard/Mouse static accessors (no InputActionAsset required).
    /// </summary>
    public class PCInputHandler : IInputHandler
    {
        public Vector2 MoveInput           { get; private set; }
        public Vector3 AimDirection        { get; private set; }
        public bool    PrimaryFireHeld     { get; private set; }
        public bool    SecondaryFirePressed { get; private set; }
        public bool    Ability1Pressed     { get; private set; }
        public bool    Ability2Pressed     { get; private set; }
        public bool    Ability3Pressed     { get; private set; }

        // Cached camera reference (resolved once in Tick)
        private Camera _mainCam;

        public void Tick()
        {
            if (_mainCam == null)
                _mainCam = Camera.main;

            ReadMovement();
            ReadAim();
            ReadAbilities();
        }

        private void ReadMovement()
        {
            var kb = Keyboard.current;
            if (kb == null) { MoveInput = Vector2.zero; return; }

            float x = 0f, y = 0f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  y -= 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;

            MoveInput = new Vector2(x, y).normalized;
        }

        private void ReadAim()
        {
            if (_mainCam == null) { AimDirection = Vector3.forward; return; }

            var mouse = Mouse.current;
            if (mouse == null) { AimDirection = Vector3.forward; return; }

            // Raycast mouse position onto Y=0 water plane
            var ray   = _mainCam.ScreenPointToRay(mouse.position.ReadValue());
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter))
            {
                AimDirection = ray.GetPoint(enter);   // world position stored; ShipController computes direction
            }
        }

        private void ReadAbilities()
        {
            var kb  = Keyboard.current;
            var m   = Mouse.current;
            if (kb == null || m == null) return;

            PrimaryFireHeld      = m.leftButton.isPressed;
            SecondaryFirePressed = m.rightButton.wasPressedThisFrame;
            Ability1Pressed      = kb.qKey.wasPressedThisFrame;
            Ability2Pressed      = kb.eKey.wasPressedThisFrame;
            Ability3Pressed      = kb.rKey.wasPressedThisFrame;
        }
    }
}
