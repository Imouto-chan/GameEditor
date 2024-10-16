﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Editor.Engine;
using Editor.Engine.Interfaces;

namespace Editor.Editor
{
    internal class Project : ISerializable
    {
        public Level CurrentLevel { get; set; } = null;
        public List<Level> Levels { get; set; } = new();
        public string Folder { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Project()
        { 
        }

        public Project(ContentManager _content, string _name)
        {
            Folder = Path.GetDirectoryName(_name);
            Name = Path.GetFileName(_name);
            if (!Name.ToLower().EndsWith(".oce"))
            {
                Name += ".oce";
            }

            // Add a default level
            AddLevel(_content);
        }

        public void AddLevel(ContentManager _content) 
        {
            CurrentLevel = new();
            CurrentLevel.LoadContent(_content);
            Levels.Add(CurrentLevel);
        }

        public void Update(float _delta)
        {
            CurrentLevel?.Update(_delta);
        }

        public void Render()
        {
            CurrentLevel.Render();
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(Levels.Count);
            int clIndex = Levels.IndexOf(CurrentLevel);
            foreach (var level in Levels)
            {
                level.Serialize(_stream);
            }
            _stream.Write(clIndex);
            _stream.Write(Folder);
            _stream.Write(Name);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            int levelCount = _stream.ReadInt32();
            for (int count = 0; count < levelCount; count++)
            {
                Level l = new();
                l.Deserialize(_stream, _content);
                Levels.Add(l);
            }
            int clIndex = _stream.ReadInt32();
            CurrentLevel = Levels[clIndex];
            Folder = _stream.ReadString();
            Name = _stream.ReadString();
        }
    }
}
