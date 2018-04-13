using System;

public struct Position {
    public int x;
    public int y;

    public static Position up = new Position(0, -1);
    public static Position right = new Position(1, 0);
    public static Position left = new Position(-1, 0);
    public static Position down = new Position(0, 1);
    public static Position zero = new Position(0, 0);

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static Position Parse(string input) {
        if (input.Contains("left")) {
            return left;
        } else if (input.Contains("right")) {
            return right;
        } else if (input.Contains("up")) {
            return up;
        } else if (input.Contains("down")) {
            return down;
        } else {
            return new Position(0, 0);
        }
    }

    public override string ToString() {
        return "(" + x + ", " + y + ")";
    }

    public static bool operator == (Position pos1, Position pos2) {
        return (pos1.x == pos2.x && pos1.y == pos2.y);
    }

    public static bool operator != (Position pos1, Position pos2) {
        return (pos1.x != pos2.x || pos1.y != pos2.y);
    }

    public static Position operator + (Position pos1, Position pos2) {
        return new Position(pos1.x + pos2.x, pos1.y + pos2.y);
    }

    public static Position operator - (Position pos1, Position pos2) {
        return new Position(pos1.x - pos2.x, pos1.y - pos2.y);
    }
}