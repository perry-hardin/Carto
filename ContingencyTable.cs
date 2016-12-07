using System;

namespace Carto
{
    public class ContingencyTable
    {
        public int [,] Cell;
        public int[] RowTotal;
        public int[] ColTotal;
        public string[] RowLabel;
        public string[] ColLabel;
        public int TableTotal;

        public ContingencyTable() { }

        public int Rows
        {
            get
            {
                return Cell.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return Cell.GetLength(1);
            }
        }

        public void Setup(int NumRows, int NumCols, string[] RowLabels, string[] ColLabels)
        {
            Cell = new int[NumRows, NumCols];
            RowTotal = new int[NumRows];
            ColTotal = new int[NumCols];
            ZeroTable();
        }

        public void ZeroTable()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    Cell[r, c] = 0;
            for (int r = 0; r < Rows; r++) RowTotal[r] = 0;
            for (int c = 0; c < Columns; c++) ColTotal[c] = 0;
            TableTotal = 0;
        }

        public void IncrementCell(int Row, int Col) 
        {
            Cell[Row, Col]++;
            ColTotal[Col]++;
            RowTotal[Row]++;
            TableTotal++;
        }

        public void AddCount(int Row, int Col, int AddAmount)
        {
            Cell[Row, Col] += AddAmount;
            ColTotal[Col] += AddAmount;
            RowTotal[Col] += AddAmount;
            TableTotal += AddAmount;
        }

        public void SubCount(int Row, int Col, int SubAmount)
        {
            Cell[Row, Col] -= SubAmount;
            ColTotal[Col] -= SubAmount;
            RowTotal[Col] -= SubAmount;
            TableTotal -= SubAmount;
        }

        public void Set(int Row, int Col, int NewCount)
        {
            int CurrentCount = Cell[Row, Col];
            SubCount(Row, Col, CurrentCount);
            AddCount(Row, Col, NewCount);
        }

        private void BalanceOneTime(ref double[,] M, ref double [] RowT, ref double [] ColT)
        {
            //Find row totals
            for (int r = 0; r < Rows; r++)
            {
                RowT[r] = 0.0;
                for (int c = 0; c < Columns; c++)
                    RowT[r] += M[r, c];
            }
            
            //Balance against the row total
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    M[r, c] = M[r, c] / RowT[r];

            //Find the column totals
            for (int c = 0; c < Columns; c++)
            {
                ColT[c] = 0.0;
                for (int r = 0; r < Rows; r++)
                    ColT[c] += M[r, c];
            }

            //Do the balancing against the column totals
            for (int c = 0; c < Columns; c++)
                for (int r = 0; r < Rows; r++)
                    M[r, c] = M[r, c] / ColT[c];


            //Find row totals one last time
            for (int r = 0; r < Rows; r++)
            {
                RowT[r] = 0.0;
                for (int c = 0; c < Columns; c++)
                    RowT[r] += M[r, c];
            }

            //Find the column totals one last time (should always be 1.0, since balanced last
            for (int c = 0; c < Columns; c++)
            {
                ColT[c] = 0.0;
                for (int r = 0; r < Rows; r++)
                    ColT[c] += M[r, c];
            }
                    
        }


        public void Balance(int NumIters, double EmptyCellValue, out double[,] OutMatrix)
        {
            OutMatrix = new double[Rows, Columns];
            double [] RowProp = new double[Rows];
            double [] ColProp = new double[Columns];
            
            //Initialize
            for (int r = 0; r < Rows; r++) {
                for (int c = 0; c < Columns; c++){
                    OutMatrix[r, c] = Cell[r, c];
                    if (OutMatrix[r, c] == 0.0) OutMatrix[r, c] = EmptyCellValue;
                }
            }
            for (int r = 0; r < Rows; r++) RowProp[r] = RowTotal[r];
            for (int c = 0; c < Columns; c++) ColProp[c] = ColTotal[c];

            //Do the balancing
            for (int iter = 1; iter <= NumIters; iter++)
            {
                BalanceOneTime(ref OutMatrix, ref RowProp, ref ColProp);
                Console.WriteLine();
                Console.WriteLine();
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        Console.Write(OutMatrix[r, c].ToString("F04") + " ");
                    }
                    Console.WriteLine("  " + RowProp[r].ToString("F04"));
                }
                for (int c = 0; c < Columns; c++) Console.Write(ColProp[c].ToString("F04") + " ");

            }
        }

    }
}
