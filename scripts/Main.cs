using Godot;
using GodotChess.Core;

public partial class Main : Node2D
{
    Board board;

    public override void _Ready()
    {
        base._Ready();
        board = new();
        board.LoadStartPosition();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Q)
            {
                GetTree().Quit();
            }
        }
    }

}
