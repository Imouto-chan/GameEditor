using Editor;

FormEditor editor = new();
editor.Game = new GameEditor(editor);
editor.Show();
editor.Game.Run();
