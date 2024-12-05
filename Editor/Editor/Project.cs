using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Editor.Engine;
using Editor.Engine.Interfaces;

namespace Editor.Editor
{
    internal class Project : ISerializable
    {
        public event AssetsUpdated OnAssetsUpdated;
        public Level CurrentLevel { get; set; } = null;
        public List<Level> Levels { get; set; } = new();
        public string Folder { get; set; } = string.Empty;
        public string ContentFolder { get; private set; } = string.Empty;
        public string AssetFolder { get; private set;} = string.Empty;
        public string ObjectFolder { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AssetMonitor AssetMonitor { get; private set; } = null;

        public Project()
        { 
        }

        public Project(GameEditor _game, string _name)
        {
            Folder = Path.GetDirectoryName(_name);
            Name = Path.GetFileName(_name);
            if (!Name.ToLower().EndsWith(".oce"))
            {
                Name += ".oce";
            }

            // Create Content folder for assets, and copy the mgcb template
            ContentFolder = Path.Combine(Folder, "Content");
            AssetFolder = Path.Combine(ContentFolder, "bin", "Windows");
            ObjectFolder = Path.Combine(ContentFolder, "obj");
            char d = Path.DirectorySeparatorChar;
            if (!Directory.Exists(ContentFolder))
            {
                Directory.CreateDirectory(ContentFolder);
                Directory.CreateDirectory(AssetFolder);
                Directory.CreateDirectory(ObjectFolder);
                File.Copy($"ContentTemplate.mgcb", ContentFolder + $"{d}Content.mgcb");
            }
            AssetMonitor = new(ObjectFolder);
            AssetMonitor.OnAssetsUpdated += AssetMonitor_OnAssetsUpdated;

            // Add a default level
            AddLevel(_game);
        }

        private void AssetMonitor_OnAssetsUpdated()
        {
            OnAssetsUpdated?.Invoke();
        }

        public void AddLevel(GameEditor _game) 
        {
            CurrentLevel = new();
            CurrentLevel.LoadContent(_game);
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
            _stream.Write(Folder);
            _stream.Write(Name);
            _stream.Write(ContentFolder);
            _stream.Write(AssetFolder);
            _stream.Write(ObjectFolder);
            _stream.Write(Levels.Count);
            int clIndex = Levels.IndexOf(CurrentLevel);
            foreach (var level in Levels)
            {
                level.Serialize(_stream);
            }
            _stream.Write(clIndex);
        }

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            Folder = _stream.ReadString();
            Name = _stream.ReadString();
            ContentFolder = _stream.ReadString();
            AssetFolder = _stream.ReadString();
            ObjectFolder = _stream.ReadString();
            int levelCount = _stream.ReadInt32();
            for (int count = 0; count < levelCount; count++)
            {
                Level l = new();
                l.Deserialize(_stream, _game);
                Levels.Add(l);
            }
            int clIndex = _stream.ReadInt32();
            CurrentLevel = Levels[clIndex];
            AssetMonitor = new(ObjectFolder);
            AssetMonitor.OnAssetsUpdated += AssetMonitor_OnAssetsUpdated;
        }
    }
}
