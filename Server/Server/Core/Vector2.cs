using System.Linq.Expressions;

namespace WX.Core;

public struct Vector2
{
    public float x;
    public float y;

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void Set(float newX, float newY)
    {
        x = newX;
        y = newY;
    }

    public void Set(Vector2 newPos)
    {
        x = newPos.x;
        y = newPos.y;
    }
}