﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Editor
{
    public delegate void ScriptUpdate(string _script);

    internal class ScriptMonitor
    {
        public event ScriptUpdate OnScriptUpdated;

        private readonly FileSystemWatcher m_watcher = null;

        internal ScriptMonitor(string _scriptFolder)
        {
            m_watcher = new FileSystemWatcher(_scriptFolder);
            m_watcher.Changed += OnChanged;
            m_watcher.Created += OnCreated;
            m_watcher.Filter = "*.lua";
            m_watcher.IncludeSubdirectories = false;
            m_watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            OnScriptUpdated?.Invoke(e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            OnScriptUpdated?.Invoke(e.FullPath);
        }
    }
}
