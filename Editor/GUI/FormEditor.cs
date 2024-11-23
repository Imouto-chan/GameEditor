using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor.Editor;
using System.IO;
using Editor.Engine;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Configuration;

namespace Editor
{
    public partial class FormEditor : Form
    {
        public GameEditor Game { get => m_game; set { m_game = value; HookEvents(); } }

        private GameEditor m_game = null;
        private Process m_MGCBProcess = null;

        public FormEditor()
        {
            InitializeComponent();
            KeyPreview = true;
            toolStripStatusLabel1.Text = Directory.GetCurrentDirectory();
        }

        private void HookEvents()
        {
            Form gameForm = Control.FromHandle(m_game.Window.Handle) as Form;
            gameForm.MouseDown += FormEditor_MouseDown;
            gameForm.MouseUp += FormEditor_MouseUp;
            gameForm.MouseWheel += FormEditor_MouseWheel;
            gameForm.MouseMove += FormEditor_MouseMove;
            KeyDown += FormEditor_KeyDown;
            KeyUp += FormEditor_KeyUp;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void splitContainer_SizeChanged(object sender, EventArgs e)
        {
            if (Game == null) return;
            Game.AdjustAspectRatio();
        }

        private void splitContainer_Panel1_SizeChanged(object sender, EventArgs e)
        {
            if (Game == null) return;
            Game.AdjustAspectRatio();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Game.Project = new(Game.GraphicsDevice, Game.Content, sfd.FileName);
                Game.Project.OnAssetsUpdated += Project_OnAssetsUpdated;
                Text = "Our Cool Editor - " + Game.Project.Name;
                Game.AdjustAspectRatio();
            }
            saveToolStripMenuItem_Click(sender, e);
        }

        private void Project_OnAssetsUpdated()
        {
            this.Invoke(delegate
            {
                listBoxAssets.Items.Clear();
                var assets = Game.Project.AssetMonitor.Assets;
                if (!assets.ContainsKey(AssetTypes.MODEL)) return;
                foreach (string asset in assets[AssetTypes.MODEL])
                {
                    listBoxAssets.Items.Add(asset);
                }
            });
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fname = Path.Combine(Game.Project.Folder, Game.Project.Name);
            using var stream = File.Open(fname, FileMode.Create);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
            Game.Project.Serialize(writer);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "OCE Files|*.oce";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using var stream = File.Open(ofd.FileName, FileMode.Open);
                using var reader = new BinaryReader(stream, Encoding.UTF8, false);
                Game.Project = new();
                Game.Project.Deserialize(reader, Game.Content);
                Text = "Our Cool Editor - " + Game.Project.Name;
                Game.AdjustAspectRatio();
            }
        }

        private void FormEditor_MouseUp(object sender, MouseEventArgs e)
        {
            InputController.Instance.SetBuuttonUp(e.Button);
            var p = new Vector2(e.Location.X, e.Location.Y);
            InputController.Instance.DragEnd = p;
        }

        private void FormEditor_MouseDown(object sender, MouseEventArgs e)
        {
            InputController.Instance.SetButtonDown(e.Button);
            var p = new Vector2(e.Location.X, e.Location.Y);
            InputController.Instance.DragStart = p;
        }

        private void FormEditor_KeyUp(object sender, KeyEventArgs e)
        {
            InputController.Instance.SetKeyUp(e.KeyCode);
            e.Handled = true;
        }

        private void FormEditor_KeyDown(object sender, KeyEventArgs e)
        {
            InputController.Instance.SetKeyDown(e.KeyCode);
            e.Handled = true;
        }

        private void FormEditor_MouseMove(object sender, MouseEventArgs e)
        {
            var p = new Vector2(e.Location.X, e.Location.Y);
            InputController.Instance.MousePosition = p;
        }

        private void FormEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            InputController.Instance.SetWheel(e.Delta / SystemInformation.MouseWheelScrollDelta);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mgcbEditorPath = ConfigurationManager.AppSettings["MGCB_EditorPath"];
            ProcessStartInfo startInfo = new()
            {
                FileName = "\"" + Path.Combine(mgcbEditorPath, "mgcb-editor-windows.exe") + "\"",
                Arguments = "\"" + Path.Combine(Game.Project.ContentFolder, "Content.mgcb") + "\""
            };
            m_MGCBProcess = Process.Start(startInfo);
        }

        private void FormEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_MGCBProcess == null) return;
            m_MGCBProcess.Kill();
        }
    }
}
