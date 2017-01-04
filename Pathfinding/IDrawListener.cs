using Microsoft.Xna.Framework.Graphics;

namespace Pathfinding
{
    public interface IDrawListener
    {
        void HandleDraw(SpriteBatch spriteBatch);
    }
}