using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pathfinding
{
    public class InputHandler : IUpdateListener
    {
        private readonly Grid _grid;
        private readonly Pathfinder _pathfinder;

        private Node _seeker, _target;

        // List of keys that have been pressed, so we know to check for release (to detect a press)
        private readonly List<Keys> _upListeners;

        private readonly IKeyPressListener _keyPressListener;

        public InputHandler(Grid grid, IKeyPressListener keyPressListener)
        {
            _grid = grid;
            _keyPressListener = keyPressListener;

            _pathfinder = new Pathfinder(grid);
            _upListeners = new List<Keys>();
        }

        public void HandleUpdate()
        {
            HandleMouse(Mouse.GetState());

            HandleKeyboard(Keyboard.GetState());
        }

        private void HandleMouse(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // get position, translate to node, put seeker
                var node = NodeAtCurrentMousePosition(mouseState);
                if (node != null && node.Walkable) _seeker = node;
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                var node = NodeAtCurrentMousePosition(mouseState);
                if (node != null && node.Walkable) _target = node;

                if (_target != null && _seeker != null)
                    _pathfinder.FindPath(_seeker.Position, _target.Position);
            }
        }

        private void HandleKeyboard(KeyboardState keyboardState)
        {
            // Get new keys pressed
            var downKeys = keyboardState.GetPressedKeys();

            foreach (var key in downKeys)
            {
                if (!_upListeners.Contains(key)) _upListeners.Add(key);
            }

            // Check for release of pressed keys
            var originalUpListenerArray = _upListeners.ToArray();
            foreach (var key in originalUpListenerArray)
            {
                if (keyboardState.IsKeyUp(key))
                {
                    _keyPressListener.HandleKeyPress(key);

                    _upListeners.Remove(key);
                }
            }
        }

        private Node NodeAtCurrentMousePosition(MouseState mouseState)
        {
            var calcedPos = new Vector2(mouseState.X / _grid.NodeTextureSize,
                mouseState.Y / _grid.NodeTextureSize);

            return _grid.NodeAt(calcedPos);
        }
    }

    public interface IKeyPressListener
    {
        void HandleKeyPress(Keys key);
    }
}