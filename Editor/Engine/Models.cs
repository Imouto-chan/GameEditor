﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Editor.Engine.Interfaces;
using System.IO;
using Editor.Editor;

namespace Editor.Engine
{
    internal class Models : ISerializable, ISelectable, IRenderable
    {
        public Model Mesh { get; set; }
        public Material Material { get; private set; }
        public Vector3 Position { get => m_position; set { m_position = value; } }
        public Vector3 Rotation { get => m_rotation; set { m_rotation = value; } }
        public float Scale { get; set; }
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected != value)
                {
                    m_selected = value;
                    SelectedDirty = true;
                }
            }
        }
        public string Name { get; set; }
        public static bool SelectedDirty { get; set; } = false;

        public Texture Texture { get; set; }

        private Vector3 m_position;
        private Vector3 m_rotation;
        private bool m_selected;

        public Models()
        {
        }

        public Models(GameEditor _game,
            string _model,
            string _texture,
            string _effect,
            Vector3 _position,
            float _scale)
        {
            Create(_game, _model, _texture, _effect, _position, _scale);
        }

        public void Create(GameEditor _game,
            string _model,
            string _texture,
            string _effect,
            Vector3 _position,
            float _scale)
        {
            Mesh = _game.Content.Load<Model>(_model);
            Mesh.Tag = _model;
            Name = _model;
            Material = new Material();
            SetTexture(_game, _texture);
            SetShader(_game, _effect);
            m_position = _position;
            Scale = _scale;
        }

        public void SetShader(Effect _effect)
        {
            Material.Effect = _effect;
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Material.Effect;
                }
            }
        }

        public void SetTexture(GameEditor _game, string _texture)
        {
            if (_texture == "DefaultTexture")
            {
                Material.Diffuse = _game.DefaultTexture;
            }
            else
            {
                Material.Diffuse = _game.Content.Load<Texture>(_texture);
            }
            Material.Diffuse.Tag = _texture;
        }

        public void SetShader(GameEditor _game, string _effect)
        {
            if (_effect == "DefaultEffect")
            {
                Material.Effect = _game.DefaultEffect;
            }
            else
            {
                Material.Effect = _game.Content.Load<Effect>(_effect);
            }
            Material.Effect.Tag = _effect;
            SetShader(Material.Effect);
        }

        public void Translate(Vector3 _translate, Camera _camera)
        {
            float distance = Vector3.Distance(_camera.Target, _camera.Position);
            Vector3 forward = _camera.Target - _camera.Position;
            forward.Normalize();
            Vector3 left = Vector3.Cross(forward, Vector3.Up);
            left.Normalize();
            Vector3 up = Vector3.Cross(left, forward);
            up.Normalize();
            Position += left * _translate.X * distance;
            Position += up * _translate.Y * distance;
            Position += forward * _translate.Z * 100f;
        }

        public void Rotate(Vector3 _rotate)
        {
            Rotation += _rotate;
        }

        public Matrix GetTransform()
        {
            return Matrix.CreateScale(Scale) *
                Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                Matrix.CreateTranslation(Position);
        }

        public void Render()
        {
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                mesh.Draw();
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(Mesh.Tag.ToString());
            _stream.Write(Material.Diffuse.Tag.ToString());
            _stream.Write(Material.Effect.Tag.ToString());
            HelpSerialize.Vec3(_stream, Position);
            HelpSerialize.Vec3(_stream, Rotation);
            _stream.Write(Scale);
        }

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            string mesh = _stream.ReadString();
            string texture = _stream.ReadString();
            string shader = _stream.ReadString();
            Position = HelpDeserialize.Vec3(_stream);
            Rotation = HelpDeserialize.Vec3(_stream);
            Scale = _stream.ReadSingle();
            Material = new Material();
            Create(_game, mesh, texture, shader, Position, Scale);
        }
    }
}
