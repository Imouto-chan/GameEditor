using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class Level
    {
        public Camera GetCamera() { return m_camera; }

        private List<Models> m_models = new();
        private Camera m_camera = new(new Microsoft.Xna.Framework.Vector3(0, 2, 2), 16 / 9);

        public Level()
        { 
        }

        public void LoadContent(ContentManager _content)
        {
            Models teapot = new(_content.Load<Model>("Teapot"), _content.Load<Texture>("Metal"), Vector3.Zero, 1.0f);
            teapot.SetShader(_content.Load<Effect>("MyShader"));
            AddModel(teapot);
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void Render()
        {
            foreach (Models m in m_models)
            {
                m.Render(m_camera.View, m_camera.Projection);
            }
        }
    }
}
