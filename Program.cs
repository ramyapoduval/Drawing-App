using System;
using System.Collections;
using System.Text;

namespace DrawingApp
{
    class Drawing
    {

        private static int cWidth = 0;
        private static int cHeight = 0;
        private static readonly string lineCharacter = "x";
        private static StringBuilder currentOutput = new StringBuilder();
        private static bool isCanvas { get; set; }
        private static readonly string programErrorException = "Program encountered an exception. Please try again or press 'Q' to quit the application.";
        private static readonly string notValidInputMessage = "Please enter a valid input";

        public static void Main(string[] args)
        {
            string line = string.Empty;
            try
            {
                // empty lines are skipped. the application doesnt exit unless the user enters q
                while ((line = Console.ReadLine()).ToLowerInvariant() != "q")
                {
                    Draw(line);
                }
            }
            catch(Exception)
            {
                Console.WriteLine(programErrorException);
            }
        }

        /// <summary>
        /// Checks input and determines what shape to draw
        /// </summary>
        /// <param name="line"></param>
        private static void Draw(string line)
        {
            var arr = line.Split(' ');
            // the strings are converted to lowerinvariant to accept both uppercase and lowercase characters as valid inputs

            if (line.ToLowerInvariant().StartsWith("c"))
            {
                isCanvas = true;
                if (arr.Length == 3)
                {
                    var width = Convert.ToInt32(arr[1]);
                    var height = Convert.ToInt32(arr[2]);
                    CreateCanvas(width, height);

                }
                else
                {
                    Console.WriteLine(notValidInputMessage);
                }
            }
            else if (line.ToLowerInvariant().StartsWith("l"))
            {
                if (arr.Length == 5)
                {
                    DrawLine(Convert.ToInt32(arr[1]), Convert.ToInt32(arr[2]), Convert.ToInt32(arr[3]), Convert.ToInt32(arr[4]));
                }
                else
                {
                    Console.WriteLine(notValidInputMessage);
                }
            }

            else if (line.ToLowerInvariant().StartsWith("r"))
            {
                if (arr.Length == 5)
                {
                    isCanvas = false;
                    int x2 = Convert.ToInt32(arr[3]);
                    int x1 = Convert.ToInt32(arr[1]);
                    int y2 = Convert.ToInt32(arr[4]);
                    int y1 = Convert.ToInt32(arr[2]);

                    CreateRectangle(x1, y1, x2, y2);
                }
                else
                {
                    Console.WriteLine(notValidInputMessage);
                }
            }
            else if (line.ToLowerInvariant().StartsWith("b"))
            {
                if (arr.Length == 4)
                {
                    PaintBucket(Convert.ToInt32(arr[1]), Convert.ToInt32(arr[2]), Convert.ToString(arr[3])); ; ;
                }
                else
                {
                    Console.WriteLine(notValidInputMessage);
                }
            }
            else
            {
                Console.WriteLine(notValidInputMessage);
            }
        }

        /// <summary>
        /// Creates the canvas. Returns without drawing if the width and height are not valid
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void CreateCanvas(int width, int height)
        {
            if (width <= 0 || height <= 0 || width >= int.MaxValue - 1 || height >= int.MaxValue)
            {
                Console.WriteLine("Not a valid input");
                return;
            }

            var vertical = new StringBuilder();
            vertical.Append("|");
            vertical.Insert(1, " ", width);
            vertical.Append("|");

            int n = 0;
            while (n <= height + 1)
            {
                if (n == 0 || n == height + 1)
                {
                    currentOutput.AppendLine(DrawCanvasHorizontalEdgesString(0, width + 2, "-"));
                }

                else
                {
                    currentOutput.AppendLine(vertical.ToString());
                }
                n++;
            }

            Console.Write(currentOutput.ToString().TrimEnd(' '));

            // the height and width are stored in class level variables to
            // ensure the drawings fit within the canvas
            cWidth = width;
            cHeight = height;

        }


        /// <summary>
        /// Draws rectangle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private static void CreateRectangle(int x1, int y1, int x2, int y2)
        {

            bool isValid = ValidateInput(x1, y1, x2, y2);
            if (!isValid) return;
            var lines = currentOutput.ToString().Split('\n');
            if (lines.Length >= cHeight + 2)
            {
                int startIndex = y1;
                var resultString = new StringBuilder();
                int width = x2 - x1 + 1;

                var row = new StringBuilder();
                row.Append("x");
                row.Insert(1, " ", width - 1);
                row.Append("x");

                while (startIndex <= y2)
                {
                    var sb = new StringBuilder();
                    sb.Append(lines[startIndex]);
                    if (startIndex == y1 || startIndex == y2)
                    {
                        sb.Replace(" ", "x", x1, width);

                    }
                    else
                    {
                        sb.Replace(" ", "x", x1, 1);
                        sb.Replace(" ", "x", x2, 1);
                    }
                    var line = sb.ToString();
                    lines[startIndex] = line;
                    startIndex++;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    resultString.AppendLine(lines[i]);
                    currentOutput = resultString;
                }
             
                Console.Write(resultString.ToString().TrimEnd(' '));
            }
        }


        private static void PaintBucket(int x, int y, string c)
        {
            var grid = convertToArrayMultiLine(currentOutput);
            ColourTheGrid(grid, c.ToCharArray()[0], x, y);
        }

        /// <summary>
        /// Draws the line if the given coordinates falls within the range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private static void DrawLine(int x1, int y1, int x2, int y2)
        {

            bool isValid = ValidateInput(x1, y1, x2, y2);
            if (!isValid) return;

            var lines = currentOutput.ToString().Split('\n');
            if (lines.Length >= cHeight + 2)
            {
                int startIndex = y1;
                var resultString = new StringBuilder();
                while (startIndex <= y2)
                {
                    var sb = new StringBuilder();
                    sb.Append(lines[startIndex]);
                    sb.Replace(" ", "x", x1, x2 - x1 + 1);
                    lines[startIndex] = sb.ToString();
                    startIndex++;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    resultString.AppendLine(lines[i]);
                    currentOutput = resultString;
                }
                Console.Write(resultString.ToString().TrimEnd(' '));
            }
        }


        private static char[][] convertToArrayMultiLine(StringBuilder input)
        {
            var height = cHeight + 2;
            char[][] arr = new char[height][];
            var lines = currentOutput.ToString().TrimEnd(' ').Split('\n');
            
            for (int i=0; i<height;i++)
            {
                arr[i] = lines[i].ToCharArray();
            }
            return arr;

        }

        private static string convertArrayToString(char[][] arr)
        {
            var sb = new StringBuilder();
            for(int i=0;i<arr.Length;i++)
            {
                string line = string.Empty;
                for(int j=0;j<arr[i].Length;j++)
                {
                    line += arr[i][j];
                }
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
        private static void ColourTheGrid(char[][] grid, char colour, int x, int y)
        {
            try
            {
                if (grid == null || grid.Length == 0)
                    return;

                dfs(grid, x, y, colour);
                Console.Write(convertArrayToString(grid));

            }
            catch(Exception)
            {
                // log error
            }

        }

        /// <summary>
        /// recursive function which goes through the next items around the given input and colours them if empty
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="colour"></param>
        private static void dfs(char[][] grid, int i, int j, char colour)
        {
            if (i < 0 || i >= grid[0].Length || j < 0 || j >= grid.Length)
            {
                return;
            }
            if (grid[j][i] == ' ') 
            {
                grid[j][i] = colour;
                dfs(grid, i + 1, j, colour);
                dfs(grid, i - 1, j, colour);
                dfs(grid, i, j + 1, colour);
                dfs(grid, i, j - 1, colour);
            }
            else
            {
                return;
            }
        }



        private static string DrawCanvasHorizontalEdgesString(int startIndex, int width, string character)
        {
            // adding +2 to the width to accomodate the edges
            var horizontal = new StringBuilder().Insert(startIndex, character, width);
            return horizontal.ToString();
        }

        /// <summary>
        /// Validates the inputs when co ordinates are present. Checks whether the values falls within the canvas range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private static bool ValidateInput(int x1, int y1, int x2, int y2)
        {
            if (cWidth == 0 || cHeight == 0)
            {
                Console.WriteLine("Please create a canvas to start drawing");
                return false;
            }
            if (x1 < 1 || x2 > cWidth || y1 < 1 || y2 > cHeight)
            {
                Console.WriteLine(string.Format("Not a valid input. Please add a line within the canvas range. Width {0} and Height {1} ", cWidth, cHeight));
                return false; ;
            }
            if (x1 > x2 || y1 > y2)
            {
                Console.WriteLine(notValidInputMessage);
                return false;
            }
             return true;
        }
    }

}
