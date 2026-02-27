using UnityEngine;

namespace IronTide.Input
{
    /// <summary>
    /// Platform-agnostic interface for ship control input.
    /// PC and Mobile handlers implement this; ShipController consumes it.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>Normalised movement vector (X = strafe/turn, Y = forward).</summary>
        Vector2 MoveInput { get; }

        /// <summary>World-space aim direction (Y=0 plane). Zero if no aim input.</summary>
        Vector3 AimDirection { get; }

        /// <summary>True the frame the primary fire button is pressed/held.</summary>
        bool PrimaryFireHeld { get; }

        /// <summary>True the frame the secondary fire button is pressed.</summary>
        bool SecondaryFirePressed { get; }

        /// <summary>True the frame ability slot 1 is pressed.</summary>
        bool Ability1Pressed { get; }

        /// <summary>True the frame ability slot 2 is pressed.</summary>
        bool Ability2Pressed { get; }

        /// <summary>True the frame ability slot 3 is pressed.</summary>
        bool Ability3Pressed { get; }

        /// <summary>Called once per frame by InputRouter.</summary>
        void Tick();
    }
}
