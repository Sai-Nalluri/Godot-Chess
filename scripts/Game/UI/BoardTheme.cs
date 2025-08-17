using Godot;

namespace GodotChess.UI;

public class BoardTheme
{
    public SquareColors lightSquares;
    public SquareColors darkSquares;

    public struct SquareColors
    {
        public Color normal;
        public Color legal;
        public Color selected;
        public Color moveFromHighlight;
        public Color moveToHighlight;
    }

    public BoardTheme()
    {
        lightSquares.normal = new Color(0.93333333333f, 0.84705882352f, 0.75294117647f);
        lightSquares.legal = new Color(0.86666666666f, 0.34901960784f, 0.34901960784f);
        lightSquares.selected = new Color(0.92549019607f, 0.7725490196f, 0.48235294117f);
        lightSquares.moveFromHighlight = new Color(0.86666666666f, 0.81568627451f, 0.4862745098f);
        lightSquares.moveToHighlight = new Color(0.86666666666f, 0.81568627451f, 0.4862745098f);

        darkSquares.normal = new Color(0.67058823529f, 0.47843137254f, 0.39607843137f);
        darkSquares.legal = new Color(0.7725490196f, 0.26666666666f, 0.30980392156f);
        darkSquares.selected = new Color(0.78431372549f, 0.61960784313f, 0.31372549019f);
        darkSquares.moveFromHighlight = new Color(0.7725490196f, 0.61960784313f, 0.36862745098f);
        darkSquares.moveToHighlight = new Color(0.7725490196f, 0.61960784313f, 0.36862745098f);
    }
}