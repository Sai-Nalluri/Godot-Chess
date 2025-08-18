using Godot;
using System;

namespace GodotChess.UIScripts;

public partial class QuitButton : Button
{
    private Button quitButton;

    public override void _Pressed()
    {
        GetTree().Quit();
    }

}
