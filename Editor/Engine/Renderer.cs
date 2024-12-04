﻿using Editor.Engine.Lights;
using System;
using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Editor.Engine
{
    internal class Renderer
    {
        private static readonly Lazy<Renderer> lazy = new(() => new Renderer());
        public static Renderer Instance { get { return lazy.Value; } }

        internal Camera Camera { get; set; }
        internal Light Light { get; set; }

        private Renderer() { }


        public void Render(IRenderable _object)
        {
            SetShaderParams(_object);
            _object.Render();
        }

        public void SetShaderParams(IRenderable _object)
        {
            Material m = _object.Material;
            Effect e = m.Effect;

            e.Parameters["World"]?.SetValue(_object.GetTransform());
            e.Parameters["WorldViewProjection"]?.SetValue(_object.GetTransform() * Camera.View * Camera.Projection);

            e.Parameters["CameraPosition"]?.SetValue(Camera.Position);
            e.Parameters["View"]?.SetValue(Camera.View);
            e.Parameters["Projection"]?.SetValue(Camera.Projection);
            e.Parameters["TextureTiling"]?.SetValue(15.0f);
            e.Parameters["LightDirection"]?.SetValue(_object.Position - Light.Position);
            e.Parameters["Texture"]?.SetValue(m.Diffuse);
            e.Parameters["LightColor"]?.SetValue(Light.Color);
            if (_object is ISelectable)
            {
                ISelectable s = _object as ISelectable;
                e.Parameters["Tint"]?.SetValue(s.Selected);
            }
            else
            {
                e.Parameters["Tint"]?.SetValue(false);
            }
        }
    }
}
