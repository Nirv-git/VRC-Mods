using System.IO;
using MelonLoader;
using UnityEngine;


namespace InstanceHistory
{
    class LoadAssets
    {
        private static Sprite _menuIcon, _upArrow, _downArrow, _item;
        public static Sprite MenuIcon
        {
            get
            {
                if (_menuIcon == null) _menuIcon = LoadEmbeddedImage("MenuIcon.png");
                return _menuIcon;
            }
        }
        public static Sprite UpArrow
        {
            get
            {
                if (_upArrow == null) _upArrow = LoadEmbeddedImage("UpArrow.png");
                return _upArrow;
            }
        }
        public static Sprite DownArrow
        {
            get
            {
                if (_downArrow == null) _downArrow = LoadEmbeddedImage("DownArrow.png");
                return _downArrow;
            }
        }
        public static Sprite Item
        {
            get
            {
                if (_item == null) _item = LoadEmbeddedImage("ItemIcon.png");
                return _item;
            }
        }

        private static Sprite LoadEmbeddedImage(string imageName)
        {
            try
            {
                //Load image into Texture
                using var assetStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("InstanceHistory." + imageName);
                using var tempStream = new MemoryStream((int)assetStream.Length);
                assetStream.CopyTo(tempStream);
                var Texture2 = new Texture2D(2, 2);
                ImageConversion.LoadImage(Texture2, tempStream.ToArray());
                Texture2.wrapMode = TextureWrapMode.Clamp;
                Texture2.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                //Texture to Sprite
                var rec = new Rect(0.0f, 0.0f, Texture2.width, Texture2.height);
                var piv = new Vector2(.5f, 5f);
                var border = Vector4.zero;
                var s = Sprite.CreateSprite_Injected(Texture2, ref rec, ref piv, 100.0f, 0, SpriteMeshType.Tight, ref border, false);
                s.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                return s;
            }
            catch (System.Exception ex) { MelonLogger.Error("Failed to load image from asset bundle: " + imageName + "\n" + ex.ToString()); return null; }
        }
    }
}
