using System;

public class Cell {
    public enum Item {
        None,
        Gold,
        Wompus,
        Pit
    };

    public Item item;
    public Position position;

    public Cell(Item item, Position position) {
        this.item = item;
        this.position = position;
    }

    public static bool operator == (Cell cell1, Cell cell2) {
        return (cell1.position == cell2.position);
    }

    public static bool operator != (Cell cell1, Cell cell2) {
        return (cell1.position != cell2.position);
    }
}