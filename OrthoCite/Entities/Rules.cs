using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace OrthoCite.Entities
{
    public class Rules : IEntity
    {
        const int PADDING = 20;
        const int MAX_MARGIN_BORDERS = 20;
        const int INTERLINE = 2;

        RuntimeData _runtimeData;
        Texture2D _blackTexture;
        SpriteFont _font;
        WrappedText _currentWrappedText;

        Action _done;

        public Rules(RuntimeData runtimeData)
        {
            _runtimeData = runtimeData;
        }


        public void SetCallback(Action callback)
        {
            _done = callback;
        }

        public void SetMiniGame(GameContext miniGame)
        {
            string text = "";

            switch(miniGame)
            {
                case GameContext.MINIGAME_PLATFORMER:
                    text = String.Join(" \n ",
                        "Des plateformes vont apparaître.",
                        "Appuie sur [Espace] pour voler et atterrir sur les plateformes.",
                        "Appuie sur [E] pour casser une plateforme avec un mot mal écrit.",
                        " \n ",
                        "Si tu casses une plateforme avec un mot bien écrit, tu perds une vie.",
                        " \n ",
                        " \n ",
                        "Bon courage !"
                    );
                    break;
                case GameContext.MINIGAME_BOSS:
                    text = String.Join(" \n ",
                        "Des boules de feu vont apparaître.",
                        "Tu dois les éviter avec les touches [Gauche] et [Droite].",
                        "En même temps, tu dois taper le mot affiché pour attaquer le méchant.",
                        " \n ",
                        "Si tu te trompes en tapant le mot ou si tu prends une boule de feu, tu perds une vie.",
                        " \n ",
                        " \n ",
                        "Bon courage !"
                    );
                    break;
                case GameContext.MINIGAME_DOORGAME:
                    text = String.Join(" \n ",
                        "Il y a deux ou trois portes au fond de chaque salle.",
                        "Place toi face à un panneau pour le lire et choisis la bonne porte.",
                        " \n ",
                        "Si tu te trompes de porte, tu perds une vie.",
                        " \n ",
                        " \n ",
                        "Bon courage !"
                    );
                    break;
                case GameContext.MINIGAME_REARRANGER:
                    text = String.Join(" \n ",
                        "Il y a un mot affiché sur les tables. Les lettres sont mélangées.",
                        "Replace les lettre dans le bon ordre, en appuyant sur [E] pour déplacer les lettres sur les tables et [A] pour valider ton mot.",
                        " \n ",
                        "Si tu te trompes dans l'ordre, tu perds une vie.",
                        " \n ",
                        "Tu as une minute pour trouver chaque mot",
                        " \n ",
                        "Bon courage !"
                    );
                    break;
                case GameContext.MINIGAME_THROWGAME:
                    text = String.Join(" \n ",
                        "Quelqu'un te lance des mots !",
                        "Certains mots sont corrects, d'autres ne le sont pas. Tire sur les mauvais mots avec [E].",
                        " \n ",
                        "Si tu tires sur un mot valide ou si tu laisses tomber un mot invalide, tu perds une vie.",
                        "\n",
                        "Si tu marches sur une des cases rouge au sol, tu perd une vie",
                        " \n ",
                        " \n ",
                        "Bon courage !"
                    );
                    break;
                    case GameContext.MINIGAME_GUESSGAME:
                    text = String.Join(" \n ",
                        "Des verbes conjugués vont apparaître en haut de l'écran. Appuie sur [A] si tu penses que le verbe est valide",
                        "\n",
                        "Appuie sur [E] si tu penses que le verbe est invalide. Tu as 15 secondes pour choisir avant de perdre une vie",
                        "\n",
                        "Des cases vont apparaître au sol, si tu marches dessus, tu perds une vie",
                        "\n",
                        "Pour gagner, tu dois valider 30 mots",
                        "\n",
                        "\n",
                        "Bon courage !"
                    );
                    break;

                    default:
                    text = String.Join(" \n ",
                        "Pas de règles."
                    );
                    break;
            }

            _currentWrappedText = TextHelper.WrapString(_font, text, _runtimeData.Scene.Width - (MAX_MARGIN_BORDERS * 2), INTERLINE);
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _blackTexture = new Texture2D(graphicsDevice, 1, 1);
            _blackTexture.SetData(new Color[] { Color.Black });
            _font = content.Load<SpriteFont>("dialogbox/font");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Camera2D camera)
        {
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                _runtimeData.OrthoCite.RulesDone();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix frozenMatrix, Matrix cameraMatrix)
        {
            spriteBatch.Begin(transformMatrix: frozenMatrix);

            spriteBatch.DrawString(_font, "Règles - [Entrée] pour jouer", new Vector2(560, 30), Color.White);

            int xOffset = (_runtimeData.Scene.Width - (int)_currentWrappedText.Bounds.X - (PADDING * 2)) / 2;
            int yOffset = (_runtimeData.Scene.Height - (int)_currentWrappedText.Bounds.Y - (PADDING * 2)) / 2;

            foreach (WrappedTextLine line in _currentWrappedText.Lines)
            {
                spriteBatch.DrawString(_font, line.Text, new Vector2(xOffset + line.Position.X + PADDING, yOffset + line.Position.Y + PADDING), Color.White);
            }
            spriteBatch.End();
        }

        public void Dispose()
        {
        }

        public void Execute(params string[] param)
        {

        }
        
    }
}
