﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Editor.Engine
{
    internal class InputController
    {
        private static readonly Lazy<InputController> lazy = new(() => new InputController());
        public static InputController Instance { get { return lazy.Value; } }

        public Vector2 MousePosition { get; set; } = Vector2.Zero;
        public Vector2 LastPosition { get; private set; } = Vector2.Zero;
        public Vector2 DragStart { get; set; } = Vector2.Zero;
        public Vector2 DragEnd { get; set; } = Vector2.Zero;

        private Dictionary<Keys, bool> m_keyState = new();
        private Dictionary<MouseButtons, bool> m_buttonState = new();
        private int m_mouseWheel = 0;

        private InputController()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (m_keyState.ContainsKey(key)) continue;
                m_keyState.Add(key, false);
            }

            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            {
                if (m_buttonState.ContainsKey(button)) continue;
                m_buttonState.Add(button, false);
            }
        }

        public void SetKeyDown(Keys _key)
        {
            m_keyState[_key] = true;
        }

        public void SetKeyUp(Keys _key)
        {
            m_keyState[_key] = false;
        }

        public bool IsKeyDown(Keys _key)
        {
            return m_keyState[_key];
        }

        public void SetButtonDown(MouseButtons _button)
        {
            m_buttonState[_button] = true;
        }

        public void SetBuuttonUp(MouseButtons _button)
        {
            m_buttonState[_button] = false;
        }

        public bool IsButtonDown(MouseButtons _button)
        {
            return m_buttonState[_button];
        }

        public void SetWheel(int _delta)
        {
            m_mouseWheel += _delta;
        }

        public int GetWheel()
        {
            return m_mouseWheel;
        }

        public void Clear()
        {
            m_mouseWheel = 0;
            LastPosition = MousePosition;
        }

        public override string ToString()
        {
            string s = "Keys Down: ";
            foreach (var key in m_keyState)
            {
                if (key.Value == true)
                {
                    s += key.Key + " ";
                }
            }

            s += "\nButtons Down: ";
            foreach (var button in m_buttonState)
            {
                if (button.Value == true)
                {
                    s += button.Key + " ";
                }
            }

            return s;
        }
    }
}
