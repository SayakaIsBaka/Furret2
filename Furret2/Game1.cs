using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace Furret2
{
    public class Game1 : Game
    {
        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private System.Windows.Forms.Control _ctrl;
        private System.Windows.Forms.Form _form;

        private AnimatedSprite _characterSpriteAnimation;
        private Vector2 _spritePosition;
        private float _angle = 0.0F;
        private Random _randomSource;

        private const int heightMargin = 260;
        private const int widthMargin = 260;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = true;
            IntPtr hWnd = Window.Handle;
            _ctrl = System.Windows.Forms.Control.FromHandle(hWnd);
            _form = _ctrl.FindForm();
            _form.TransparencyKey = System.Drawing.Color.Beige;
            _form.TopMost = true;

            uint initialStyle = GetWindowLong(hWnd, -20);
            SetWindowLong(hWnd, -20, initialStyle | 0x80000 | 0x20);

            _graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            _graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Window.Position = new Point(0, 0);
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _spritePosition = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);
            _randomSource = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var characterTexture = Content.Load<Texture2D>("furret");
            var characterMap = Content.Load<Dictionary<string, Rectangle>>("furretMap");
            var characterAtlas = new TextureAtlas("furret", characterTexture, characterMap);
            var characterAnimationFactory = new SpriteSheetAnimationFactory(characterAtlas);

            characterAnimationFactory.Add("idle", new SpriteSheetAnimationData(Enumerable.Range(0, 29).ToArray(), isLooping: true));

            _characterSpriteAnimation = new AnimatedSprite(characterAnimationFactory, "idle");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _characterSpriteAnimation.Update(0.16F);

            // TODO: Add your update logic here
            if (_spritePosition.X < -widthMargin
                || _spritePosition.X > GraphicsDevice.Viewport.Bounds.Width + widthMargin
                || _spritePosition.Y < -heightMargin
                || _spritePosition.Y > GraphicsDevice.Viewport.Bounds.Height + heightMargin)
            {
                SetRandomStartPoint();
            }
            else
                UpdatePosition();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_characterSpriteAnimation, _spritePosition, _angle);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetRandomStartPoint()
        {
            _angle = (float)(_randomSource.NextDouble() * Math.PI * 2);
            _spritePosition.X = (int)((GraphicsDevice.Viewport.Bounds.Width / 2) + (Math.Cos(_angle) * ((GraphicsDevice.Viewport.Bounds.Width + 700) / 2)));
            _spritePosition.Y = (int)((GraphicsDevice.Viewport.Bounds.Height / 2) + (Math.Sin(_angle) * ((GraphicsDevice.Viewport.Bounds.Height + 550) / 2)));
        }

        private void UpdatePosition()
        {
            int speed = 4;
            _spritePosition.X += (int)(Math.Cos(_angle) * -speed);
            _spritePosition.Y += (int)(Math.Sin(_angle) * -speed);
        }
    }
}
