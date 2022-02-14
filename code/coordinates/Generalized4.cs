using System;
using System.Linq;
using Godot;

/// <summary>Set of four generalized coordinates.</summary>
public struct Generalized4
{
    /// <summary>The first generalized coordinate.</summary>
    public float a;

    /// <summary>The second generalized coordinate.</summary>
    public float b;

    /// <summary>The third generalized coordinate.</summary>
    public float c;

    /// <summary>The fourth generalized coordinate.</summary>
    public float d;

    /// <summary>Create new set of generalized coordinates.</summary>
    /// <param name="a">The first generalized coordinate.</param>
    /// <param name="b">The second generalized coordinate.</param>
    /// <param name="c">The third generalized coordinate.</param>
    /// <param name="d">The fourth generalized coordinate.</param>
    public Generalized4(float a, float b, float c, float d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    /// <summary>Scale generalized coordinates by a given value.</summary>
    public static Generalized4 operator *(float scale, Generalized4 generalized)
    {
        return new Generalized4(
            scale * generalized.a,
            scale * generalized.b,
            scale * generalized.c,
            scale * generalized.d
        );
    }

    /// <summary>Scale generalized coordinates by a given value.</summary>
    public static Generalized4 operator *(Generalized4 generalized, float scale)
    {
        return scale * generalized;
    }

    /// <summary>Divide generalized coordinates on a given value.</summary>
    public static Generalized4 operator /(
        Generalized4 generalized,
        Generalized4 divisor
    )
    {
        return new Generalized4(
            generalized.a / divisor.a,
            generalized.b / divisor.b,
            generalized.c / divisor.c,
            generalized.d / divisor.d
        );
    }

    /// <summary>Calculate element-wise sum of generalized
    /// coordinates.</summary>
    public static Generalized4 operator +(Generalized4 self, Generalized4 other)
    {
        return new Generalized4(
            self.a + other.a,
            self.b + other.b,
            self.c + other.c,
            self.d + other.d
        );
    }

    /// <summary>Calculate element-wise difference of generalized
    /// coordinates.</summary>
    public static Generalized4 operator -(Generalized4 self, Generalized4 other)
    {
        return new Generalized4(
            self.a - other.a,
            self.b - other.b,
            self.c - other.c,
            self.d - other.d
        );
    }

    /// <summary>Get generalized coordinate value by given
    /// <c>index</c>.</summary>
    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
            }
            throw new IndexOutOfRangeException();
        }
        set
        {
            switch (index)
            {
                case 0:
                    a = value;
                    break;
                case 1:
                    b = value;
                    break;
                case 2:
                    c = value;
                    break;
                case 3:
                    d = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    /// <summary>Calculate element-wise absolute values of generalized
    /// coordinates.</summary>
    /// <returns>Element-wise absolute values.</returns>
    public Generalized4 Abs()
    {
        return new Generalized4(
            Mathf.Abs(a),
            Mathf.Abs(b),
            Mathf.Abs(c),
            Mathf.Abs(d)
        );
    }

    /// <summary>Get biggest value of generalized coordinate set.</summary>
    /// <returns>Maximum element value.</returns>
    public float Max()
    {
        return new float[] { a, b, c, d }.Max();
    }
}
