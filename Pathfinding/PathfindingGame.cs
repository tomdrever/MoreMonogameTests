using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pathfinding.Maps;

namespace Pathfinding
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PathfindingGame : Game, IKeyPressListener
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly List<IDrawListener> _drawListeners;
        private readonly List<IUpdateListener> _updateListeners;

        private Texture2D texture;
        private float scale;
        private int gridWidth, gridHeight;

        private IMapBuilder mapBuilder;

        public PathfindingGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width
            };

            Content.RootDirectory = "Content";

            _drawListeners = new List<IDrawListener>();
            _updateListeners = new List<IUpdateListener>();

            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("tile");
            scale = 0.4f;

            gridWidth = (int)(_graphics.PreferredBackBufferWidth / (texture.Width * scale));
            gridHeight = (int)(_graphics.PreferredBackBufferHeight / (texture.Height * scale));

            SetUp();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            foreach (var listener in _updateListeners.ToArray()) listener.HandleUpdate();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach (var listener in _drawListeners.ToArray()) listener.HandleDraw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetUp()
        {
            _drawListeners.Clear();
            _updateListeners.Clear();

            mapBuilder = new DiamondSquareMapBuilder(5, null);

            var intGrid = mapBuilder.GetMap();

            var grid = new Grid(intGrid, texture) { NodeTextureScale = scale };

            // Add some random obstacles
            /*
            var random = new Random();
            for (int i = 0; i < random.Next(40, 75); i++)
            {
                int rWidth = random.Next(3, 15);
                int rHeight = random.Next(3, 15);

                int rX = random.Next(0, gridWidth - rWidth);
                int rY = random.Next(0, gridHeight - rHeight);

                grid.AddObstacle(rX, rY, rWidth, rHeight);
            }*/

            _drawListeners.Add(grid);

            _updateListeners.Add(new InputHandler(grid, this));
        }

        public void HandleKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.R:
                    SetUp();
                    break;
                case Keys.Escape:
                    Exit();
                    break;
            }
        }
    }
}
