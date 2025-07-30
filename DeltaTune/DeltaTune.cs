﻿using System;
using System.IO;
using DeltaTune.Display;
using DeltaTune.Media;
using DeltaTune.Settings;
using DeltaTune.Window;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using R3;

namespace DeltaTune
{
    public class DeltaTune : Game
    {
        private string relativePathRoot;
        private GraphicsDeviceManager graphicsDeviceManagerInstance;
        private IWindowService windowService;
        private IMediaInfoService mediaInfoService;
        private IDisplayService displayService;
        private ISettingsMenu settingsMenu;
        private ISettingsService settingsService;
        private ISettingsFile settingsFile;

        private SpriteBatch spriteBatch;
        private BitmapFont musicTitleFont;
        
        public DeltaTune()
        {
            graphicsDeviceManagerInstance = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 1,
                PreferredBackBufferHeight = 1
            };

            relativePathRoot = AppDomain.CurrentDomain.BaseDirectory;
            Content.RootDirectory = "Content";
            
            ObservableSystemComponent observableSystemComponent = new ObservableSystemComponent(this);
            observableSystemComponent.Initialize();
            Components.Add(observableSystemComponent);
        }

        protected override void Initialize()
        { 
            settingsService = new SettingsService();
            settingsFile = new SettingsFile(settingsService, Path.Combine(relativePathRoot, "Settings.json"));
            settingsMenu = new SettingsMenu(settingsService);
            
            mediaInfoService = new SystemMediaInfoService();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            musicTitleFont = BitmapFont.FromFile(GraphicsDevice, Path.Combine("Content", "Fonts", "MusicTitleFont.fnt"));
            musicTitleFont.FallbackCharacter = '▯';
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            base.LoadContent();
        }

        protected override void BeginRun()
        {
            windowService = new WindowService(this, graphicsDeviceManagerInstance, settingsMenu, settingsService, musicTitleFont.LineHeight);
            windowService.InitializeWindow();

            Vector2 WindowSizeProvider() => new Vector2(graphicsDeviceManagerInstance.GraphicsDevice.Viewport.Width, graphicsDeviceManagerInstance.GraphicsDevice.Viewport.Height);
            displayService = new DisplayService(mediaInfoService, settingsService, musicTitleFont, WindowSizeProvider);
            displayService.BeginRun();
            
            base.BeginRun();
        }

        protected override void Update(GameTime gameTime)
        {
            displayService.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
            displayService.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}