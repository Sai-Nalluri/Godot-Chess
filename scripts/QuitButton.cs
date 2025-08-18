using Godot;
using System;

public partial class QuitButton : Button
{
    private Button quitButton;

    public override void _Ready()
    {
        base._Ready();

        quitButton.Pressed += QuitButtonPressed;
    }

    private void QuitButtonPressed()
    {
        GetTree().Quit();
    }
}
