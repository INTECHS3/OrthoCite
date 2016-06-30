using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace OrthoCite
{
    class WrappedTextLine
    {
        public string Text;
        public Vector2 Position;
    }

    class WrappedText
    {
        public Vector2 Bounds;
        public List<WrappedTextLine> Lines;
    }

    class TextHelper
    {
        public static WrappedText WrapString(SpriteFont font, String text, int maxWidth, int interline)
        {
            string[] words = text.Split(new char[] { ' ' });
            List<WrappedTextLine> lines = new List<WrappedTextLine>();
            string currentLine = "";
            int currentWidth = 0;
            int maxLineWidth = 0;
            int currentLineY = 0;

            int spaceWidth = (int)font.MeasureString(" ").X;
            int fontHeight = (int)font.MeasureString("I").Y;

            foreach (string word in words) // split in lines
            {
                if (word == "\n")
                {
                    WrappedTextLine line = new WrappedTextLine { Text = currentLine, Position = new Vector2(0, currentLineY) };
                    lines.Add(line);
                    currentLine = "";
                    currentWidth = 0;
                    currentLineY += fontHeight + interline;
                }

                int wordWidth = (int)font.MeasureString(word).X;

                if (currentWidth + wordWidth < maxWidth)
                {
                    currentLine += word + ' ';
                    currentWidth += wordWidth + spaceWidth;
                }
                else
                {
                    WrappedTextLine line = new WrappedTextLine { Text = currentLine, Position = new Vector2(0, currentLineY) };
                    lines.Add(line);
                    currentLine = word + ' ';
                    if (currentWidth > maxLineWidth) maxLineWidth = currentWidth;
                    currentWidth = wordWidth;
                    currentLineY += fontHeight + interline;
                }
            }

            if (currentLine != "")
            {
                WrappedTextLine line = new WrappedTextLine { Text = currentLine, Position = new Vector2(0, currentLineY) };
                lines.Add(line);
                if (currentWidth > maxLineWidth) maxLineWidth = currentWidth;
            }

            foreach (WrappedTextLine line in lines) // center lines
            {
                line.Text = line.Text.Trim();
                int lineWidth = (int)font.MeasureString(line.Text).X;
                line.Position.X = (maxLineWidth - lineWidth) / 2;
            }

            return new WrappedText { Bounds = new Vector2(maxLineWidth, currentLineY + fontHeight), Lines = lines };
        }
    }
}
