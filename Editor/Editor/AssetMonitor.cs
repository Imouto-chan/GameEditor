using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Editor
{
    public delegate void AssetsUpdated();

    internal enum AssetTypes
    {
        NONE,
        MODEL,
        TEXTURE,
        FONT,
        AUDIO,
        EFFECT
    };

    internal class AssetMonitor
    {
        public event AssetsUpdated OnAssetsUpdated;

        private readonly FileSystemWatcher m_watcher = null;
        public Dictionary<AssetTypes, List<string>> Assets { get; private set; } = new();
        private readonly string m_metaInfo = string.Empty;

        internal AssetMonitor(string _object)
        {
            m_metaInfo = Path.Combine(Path.Combine(_object, "Windows"), ".mgstats");
            m_watcher = new FileSystemWatcher(_object);
            m_watcher.Changed += OnChanged;
            m_watcher.Created += OnCreated;
            m_watcher.Deleted += OnDeleted;
            m_watcher.Filter = "*.mgstats";
            m_watcher.IncludeSubdirectories = true;
            m_watcher.EnableRaisingEvents = true;
            //System.Diagnostics.Debug.WriteLine("here3");
        }

        public void UpdateAssetDB()
        {
            //System.Diagnostics.Debug.WriteLine("here1");
            bool updated = false;
            AssetTypes assetType = AssetTypes.MODEL;
            using var inStream = new FileStream(m_metaInfo, FileMode.Open,
                                                FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(inStream);
            string[] content = streamReader.ReadToEnd().Split(Environment.NewLine);
            foreach (string line in content)
            {
                //System.Diagnostics.Debug.WriteLine("here2");
                if (string.IsNullOrEmpty(line)) continue;
                string[] fields = line.Split(',');
                if (fields[0] == "Source File") continue;
                switch (fields[2])
                {
                    case "\"ModelProcessor\"":
                        assetType = AssetTypes.MODEL;
                        break;
                    case "\"TextureProcessor\"":
                        assetType = AssetTypes.TEXTURE;
                        break;
                    case "\"SongProcessor\"":
                        assetType = AssetTypes.AUDIO;
                        break;
                    case "\"SoundEffectProcessor\"":
                        assetType = AssetTypes.AUDIO;
                        break;
                    case "\"EffectProcessor\"":
                        assetType = AssetTypes.EFFECT;
                        break;
                    default:
                        //Debug.Assert(false, "Unhandled processor.");
                        break;
                }
                if (AddAsset(assetType, fields[1])) updated = true;
            }

            if (updated) OnAssetsUpdated?.Invoke();
        }

        private bool AddAsset(AssetTypes _assetType, string _assetName)
        {
            if (!Assets.ContainsKey(_assetType)) Assets.Add(_assetType, new());
            string assetName = Path.GetFileNameWithoutExtension(_assetName);
            Assets[_assetType].Add(assetName);
            return true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Assets.Clear();
            UpdateAssetDB();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Assets.Clear();
            UpdateAssetDB();
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Assets.Clear();
            OnAssetsUpdated?.Invoke();
        }
    }
}
