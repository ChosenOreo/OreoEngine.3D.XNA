using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace eXtreme_Mazes
{
    class ScreenManager : IDisposable
    {
        #region Variables
        MazeGame mazeGame; // Maze XNA Game
        GameManager gameManager; // Game Manager
        FontManager fontManager; // Font Manager
        InputManager inputManager; // Input Manager
        ContentManager contentManager; // Content Manage

        List<Screen> screens; // List of Available Screens
        Screen currentScreen; // Currently Active Screen
        Screen nextScreen; // Next Screen in a transition -- Is Null if no Transition

        float fadeTime = 1.0f; // Total Fade Time in a transition
        float fade = 0.0f; // Current Fade Time in a transition
        Vector4 fadeColor = Vector4.One; // Color Fading In and Out

        RenderTarget2D colorRT; // Render Target for Main Color Buffer
        RenderTarget2D glowRT1; // Render Target for Glow Horizontal Blur
        RenderTarget2D glowRT2; // Render Target for Glow Vertical Blur

        BlurManager blurManager; // Blur Manager

        int frameRate; // Current Game Frame Rate (FPS)
        int frameRateCount; // Current frame count since last frame rate update
        float frameRateTime; // Elapsed time since last frame rate update

        Texture2D textureBackground; // The Background Texture for Menus
        float backgroundTime = 0.0f; // Time for Background Animations used on Menus
        #endregion

        // Constructor
        public ScreenManager(MazeGame mazeGame, FontManager font, GameManager game)
        {
            this.mazeGame = mazeGame;
            gameManager = game;
            fontManager = font;

            screens = new List<Screen>();
            inputManager = new InputManager();

            // Add All Screens
            screens.Add(new Screens.ScreenIntro(this, game));
            screens.Add(new Screens.ScreenCredits(this, game));
            screens.Add(new Screens.ScreenEnd(this, game));
            screens.Add(new Screens.ScreenGame(this, game));
            screens.Add(new Screens.ScreenGameList(this, game));
            screens.Add(new Screens.ScreenHelp(this, game));
            screens.Add(new Screens.ScreenMazeList(this, game));
            screens.Add(new Screens.ScreenMulti(this, game));
            screens.Add(new Screens.ScreenOptions(this, game));
            screens.Add(new Screens.ScreenSingle(this, game));

            // Fade Into Intro Screen!
            SetNextScreen(ScreenType.ScreenIntro, GameOptions.FadeColor, GameOptions.FadeTime);
            fade = fadeTime * 0.5f;
        }

        // Process Input
        public void ProcessInput(float elapsedTime)
        {
            inputManager.BeginInputProcessing(gameManager.GameMode == GameMode.SinglePlayer);

            // Process Input for Currently Active Screen
            if (currentScreen != null && nextScreen == null)
                currentScreen.ProcessInput(elapsedTime, inputManager);

            // Toggle Full Screen with f5 Key
            if (inputManager.IsKeyPressed(0, Keys.F5) || inputManager.IsKeyPressed(1, Keys.F5))
                mazeGame.ToggleFullScreen();

            inputManager.EndInputProcessing();
        }

        // Update for a Given Elapsed Time
        public void Update(float elapsedTime)
        {
            // If in a Transition...
            if (fade > 0)
            {
                // Update Transition Time...
                fade -= elapsedTime;

                // If Time to Switch to a New Screen (Fade Out Finished..)
                if (nextScreen != null && fade < 0.5f * fadeTime)
                {
                    // Tell new Screen it's getting in focus
                    nextScreen.SetFocus(contentManager, true);

                    // Tell old Screen it's lost its focus
                    if (currentScreen != null)
                        currentScreen.SetFocus(contentManager, false);

                    // Set new Screen as current Screen
                    currentScreen = nextScreen;
                    nextScreen = null;
                }
            }

            // If Current Screen is available, Update It!
            if (currentScreen != null)
                currentScreen.Update(elapsedTime);

            // Calculate Frame Rate
            frameRateTime += elapsedTime;
            if (frameRateTime > 0.5f)
            {
                frameRate = (int)((float)frameRateCount / frameRateTime);
                frameRateCount = 0;
                frameRateTime = 0;
            }

            // Accumulate Elapsed time for Background Animation
            backgroundTime += elapsedTime;
        }

        // Blur the Color Render Target using the Alpha Channel and Blur Intensity
        void BlurGlowRenderTarget(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            gd.DepthStencilState = DepthStencilState.None;

            // Blur Horizonatal with Regular Horizontal Blur Shader
            gd.SetRenderTarget(glowRT1);
            blurManager.RenderScreenQuad(gd, BlurTechnique.BlurHorizontal, colorRT, Vector4.One);

            // Blur Vertical with Regular Vertical Blur Shader
            gd.SetRenderTarget(glowRT2);
            blurManager.RenderScreenQuat(gd, BlurTechnique.BlurVertical, glowRT1, Vector4.One);

            gd.DepthStencilState = DepthStencilState.Default;

            gd.SetRenderTarget(null);
        }

        // Draw Render Target as Fullscreen Texture with given Intensity and Blend Mode
        void DrawRenderTargetTexture(GraphicsDevice gd, RenderTarget2D renderTarget, float intensity, bool additiveBlend)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            // Set up Render State and Blend Mode
            gd.DepthStencilState = DepthStencilState.None;
            if (additiveBlend)
            {
                gd.BlendState = BlendState.Additive;
            }

            // Draw Render Target as Fullscreen Texture
            blurManager.RenderScreenQuad(gd, BlurTechnique.ColorTexture, renderTarget, new Vector4(intensity));

            // Restore Render State and Blend Mode
            gd.DepthStencilState = DepthStencilState.Default;
        }

        public void DrawTexture(Texture2D texture, Rectangle rect, Color color, BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, color, blend);
        }

        public void DrawTexture(Texture2D texture, Rectangle destinationRect, Rectangle sourceRect, Color color, BlendState blend)
        {
            fontManager.DrawTexture(texture, destinationRect, sourceRect, color, blend);
        }

        public void DrawTexture(Texture2D texture, Rectangle rect, float rotation, Color color, BlendState blend)
        {
            fontManager.DrawTexture(texture, rect, rotation, color, blend);
        }

        public void DrawBackground(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            const float animationTime = 3.0f;
            const float animationLength = 0.4f;
            const int numberLayers = 2;
            const float layerDistance = 1.0f / numberLayers;

            float normalizedTime = ((backgroundTime / animationTime) % 1.0f);

            DepthStencilState ds = gd.DepthStencilState;
            BlendState bs = gd.BlendState;
            gd.DepthStencilState = DepthStencilState.DepthRead;
            gd.BlendState = BlendState.AlphaBlend;

            float scale;
            Vector4 color;

            for (int i = 0; i < numberLayers; i++)
            {
                if (normalizedTime > 0.5f)
                    scale = 2 - normalizedTime * 2;
                else
                    scale = normalizedTime * 2;
                color = new Vector4(scale, scale, scale, 0);

                scale = 1 + normalizedTime * animationLength;

                blurManager.RenderScreenQuad(gd, BlurTechnique.ColorTexture, textureBackground, color, scale);

                normalizedTime = (normalizedTime + layerDistance) % 1.0f;
            }

            gd.DepthStencilState = ds;
            gd.BlendState = bs;
        }

        public void Draw(GraphicsDevice gd)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            frameRateCount++;

            if (currentScreen != null)
            {
                gd.SetRenderTarget(colorRT);
                currentScreen.Draw3D(gd);
                gd.SetRenderTarget(null);
                BlurGlowRenderTarget(gd);
                DrawRenderTargetTexture(gd, colorRT, 1.0f, false);
                DrawRenderTargetTexture(gd, glowRT2, 2.0f, true);
                fontManager.BeginText();
                currentScreen.Draw2d(gd, fontManager);
                fontManager.EndText();
            }

            if (fade > 0)
            {
                float size = fadeTime * 0.5f;
                fadeColor.W = 1.25f * (1.0f - Math.Abs(fade - size) / size);

                gd.DepthStencilState = DepthStencilState.None;
                gd.BlendState = BlendState.AlphaBlend;

                blurManager.RenderScreenQuad(gd, BlurTechnique.Color, null, fadeColor);

                gd.DepthStencilState = DepthStencilState.Default;
                gd.BlendState = BlendState.Opaque;
            }
        }

        public void LoadContent(GraphicsDevice gd, ContentManager content)
        {
            if (gd == null)
            {
                throw new ArgumentNullException("gd");
            }

            contentManager = content;
    }
}
