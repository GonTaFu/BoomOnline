using UnityEngine;

public class BuildLevel
{
    static public char[,] GenerateRandomMap(int rows = 13, int columns = 31)
    {
        char[,] map = new char[rows, columns];

        // Khởi tạo toàn bộ bản đồ là cỏ (' ')
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                map[row, col] = ' ';
            }
        }

        // Đặt tường không thể phá hủy (#) ở biên
        for (int row = 0; row < rows; row++)
        {
            map[row, 0] = '#';
            map[row, columns - 1] = '#';
        }
        for (int col = 0; col < columns; col++)
        {
            map[0, col] = '#';
            map[rows - 1, col] = '#';
        }

        // Đặt tường không thể phá hủy (#) ở lưới 2x2
        for (int row = 2; row < rows - 1; row += 2)
        {
            for (int col = 2; col < columns - 1; col += 2)
            {
                map[row, col] = '#';
            }
        }

        // Đặt người chơi (p) ở góc trên bên trái
        map[1, 1] = 'p';
        map[1, 2] = ' ';
        map[2, 1] = ' ';
        map[2, 2] = ' ';

        map[1, 3] = '*';
        map[2, 3] = '*';
        map[3, 3] = '*';
        map[3, 1] = '*';
        map[2, 1] = '*';
        map[3, 1] = '*';

        // Đặt gạch có thể phá hủy (*) ở các vị trí ngẫu nhiên, trừ vùng an toàn quanh người chơi
        float brickChance = 0.5f;
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < columns - 1; col++)
            {
                // Chỉ đặt gạch ở các ô trống và không nằm trong vùng an toàn (1,2), (2,1), (2,2), (1,3), (2,3), (3,1), (3,2)
                if (map[row, col] == ' ' &&
                    !(row == 1 && col == 2) &&
                    !(row == 2 && col == 1) &&
                    !(row == 2 && col == 2) &&
                    !(row == 1 && col == 3) &&
                    !(row == 2 && col == 3) &&
                    !(row == 3 && col == 1) &&
                    !(row == 3 && col == 2))
                {
                    if (Random.value < brickChance)
                    {
                        map[row, col] = '*';
                    }
                }
            }
        }

        return map;
    }
}