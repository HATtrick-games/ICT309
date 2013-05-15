using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AvatarSample
{
  /// <summary>
  /// Provides useful extension methods for <see cref="SpriteBatch"/>.
  /// </summary>
  public static class SpriteBatchHelper
  {
    /// <overloads>
    /// <summary>
    /// Draws a string with a 1-pixel outline.
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Draws a string with a 1-pixel outline.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="spriteFont">The <see cref="SpriteFont"/>.</param>
    /// <param name="text">The text.</param>
    /// <param name="position">The position.</param>
    /// <param name="stroke">The stroke color.</param>
    /// <param name="fill">The fill color.</param>
    public static void DrawStringOutlined(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color stroke, Color fill)
    {
      // This is not the most efficient way of drawing an outline, but it does the trick. ;)
      for (int y = -1; y <= 1; y++)
        for (int x = -1; x <= 1; x++)
          spriteBatch.DrawString(spriteFont, text, position + new Vector2(x, y), stroke);

      spriteBatch.DrawString(spriteFont, text, position, fill);
    }


    /// <summary>
    /// Draws a string with a 1-pixel outline.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/>.</param>
    /// <param name="spriteFont">The <see cref="SpriteFont"/>.</param>
    /// <param name="text">The text.</param>
    /// <param name="position">The position.</param>
    /// <param name="stroke">The stroke color.</param>
    /// <param name="fill">The fill color.</param>
    public static void DrawStringOutlined(this SpriteBatch spriteBatch, SpriteFont spriteFont, StringBuilder text, Vector2 position, Color stroke, Color fill)
    {
      for (int y = -1; y <= 1; y++)
        for (int x = -1; x <= 1; x++)
          spriteBatch.DrawString(spriteFont, text, position + new Vector2(x, y), stroke);

      spriteBatch.DrawString(spriteFont, text, position, fill);
    }
  }
}
