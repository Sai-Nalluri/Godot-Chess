using Godot;
using System;

public partial class QuitButton : Button
{
    private Button quitButton;

    public override void _Pressed()
    {
        GetTree().Quit();
    }

}
