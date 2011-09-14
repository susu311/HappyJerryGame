using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Xml;
using System.Xml.Serialization;

namespace HappyJerryGame
{
    public class Level
    {
        public class character
        {
            public string name;
            public Vector2 position;
            public string greeting;
            public float scale;
            public string textureAsset;
            int correctChoice;
            [XmlArrayItem]
            public string[] choice;
            [XmlIgnore]
            public BoundingSphere bound;
            [XmlIgnore]

            public Texture2D sprite;
        }
        //other classes could be added for collectable objects / physical layouts / everything you might need to define a level

        [XmlArrayItem]
        public character[] allCharacters;
    }
    
}
