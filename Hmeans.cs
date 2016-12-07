using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Carto
{

    /// <summary>
    /// Hmeans clustering routine as defined in Numerical Recipes book
    /// 3rd edition, starting pages 848-850.  Note that the authors
    /// wrongly identify this as Kmeans.  It operates according to
    /// Spath's Hmeans principle.  This has parallel processing
    /// of the E step and part of the M step using Perry's method.
    /// When optimize switch is on, and this is run outside of debug
    /// mode, this is extremely fast.
    /// </summary>
    public class Hmeans3b
    {
        public double[][] X = null;        //Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.
        public int[] Membership = null;    //Cluster membership.  Initial and final.  Range of values is 1..NumClusters
        public double[] CaseCount = null;  //The number of cases in each cluster group.  Modified as program executes.
        public double[][] S = null;        //The cluster centroids, dimensioned [NumClusters, NumVars]
        public double[] E = null;          //The sum of squared distance of each member to its cluster centroid, dimensioned [NumClusters]
        public double D = double.MaxValue; //The sum of all E[NumClusters].  What the routine seeks to minimize
        public int NumVars = -1;           //Number of variables.  Designated as L in original Kmeans FORTRAN program
        public int NumClusters = -1;       //Number of clusters desired from the routine.  Designated as N in original Kmeans FORTRAN program
        public int NumCases = -1;          //Number of cases. designated as M in original Kmeans FORTRAN program
        public int NumProcessors = -1;     //Number of threads to use in parallel workers
        public Worker[] W = null;          //Array of parallel workers, containing an execute method, dimensioned [NumProcessors]
        public Thread[] T = null;          //The threads that run the parallel workers dimensioned [NumProcessors]

        ///// <summary>
        ///// Take the S matrix and CaseCount vector from each parallel worker and
        ///// combine the information into the common S matrix and CaseCount vector
        ///// </summary>
        //private void CombineThreads()
        //{
        //    //Get the total CaseCounts and centroid total distance for each cluster
        //    Utility.Assign(CaseCount, 0.0);
        //    Utility.Assign(S, 0.0);
        //    for (int i = 0; i < NumProcessors; i++)
        //    {
        //        Worker TT = W[i];
        //        for (int C = 0; C < NumClusters; C++)
        //        {
        //            CaseCount[C] += TT.CaseCount[C];
        //            for (int V = 0; V < NumVars; V++)
        //                S[C][V] += TT.S1[C][V];
        //        }
        //    }

        //    //Calculate the centroid using totals just calculated
        //    for (int k = 0; k < NumClusters; k++)
        //    {
        //        if (CaseCount[k] > 0)
        //        {
        //            for (int m = 0; m < NumVars; m++)
        //                S[k][m] /= CaseCount[k];
        //        }
        //    }

        //}

        /// <summary>
        /// Worker class for parallel version.  Implements portions of MStep and EStep together
        /// which iterate through all cases in the data array.  The parallelization method
        /// was divide and conquer the data array.  The results of the parallel operations are combined
        /// back together into the global S and X arrays by CombineThreads.
        /// </summary>
        public class Worker
        {
            public int StartCase;  //Start case for estep
            public int EndCase; //End case for estep
            public int NumVars; //Number of independent variables
            public int NumClusters; //Number of clusters
            public int[] Membership; //Cluster Membership, a reference to the global version.
            public double[][] S;  //A reference to the global centroids array
            public double[][] S1;  //Allocated.  The new centroids array local to the thread.  Used by CombineThreads.
            public double[][] X;  //A reference to the data array 
            public double [] CaseCount;  //Allocated. The array holding the number of cases in each cluster.  Used by CombineThreads.
            public bool Changed;  //If changes were made in cluster membership by the thread


            public Worker(int NumProcessors, int ProcessorIDX, int NumCases, int NumClusters, int NumVars, int[] Membership, double [][]S, double[][] X)
            {
                int NumCasesForEach = NumCases / NumProcessors;
                this.NumClusters = NumClusters;
                this.Membership = Membership;
                this.NumVars = NumVars;
                StartCase = ProcessorIDX * NumCasesForEach;
                EndCase = StartCase + NumCasesForEach - 1;
                if (ProcessorIDX == NumProcessors - 1)
                    EndCase = NumCases - 1;
                this.X = X;
                this.S = S;
                this.CaseCount = new double[NumClusters];
                this.S1 = new double[NumClusters][];
                for (int i = 0; i < NumClusters; i++)
                    this.S1[i] = new double[NumVars];
            }



            public static void PrepareThreads(Thread[] T, Worker[] W)
            {
                int NumProcessors = T.Length;
                for (int i = 0; i < NumProcessors; i++)
                    T[i] = new Thread(W[i].Execute);
                for (int i = 0; i < NumProcessors; i++)
                    T[i].Start();
                for (int i = 0; i < NumProcessors; i++)
                    T[i].Join();
                for (int i = 0; i < NumProcessors; i++)
                    T[i] = null;
            }

            public static bool GlobalChange(Worker[] W)
            {
                for (int i = 0; i < W.Length; i++)
                    if (W[i].Changed) return true;
                return false;
            }

            /// <summary>
            /// Combine the results from the worker threads, specifically S and CaseCount
            /// </summary>
            /// <param name="W">The array of worker objects</param>
            /// <param name="Global_S">The "global" centroid array</param>
            /// <param name="Global_CaseCount">The "global" CaseCount array</param>
            public static void CombineThreads(Worker[] W, double[][] Global_S, double[] Global_CaseCount)
            {
                //Get the total CaseCounts and centroid total distance for each cluster
                int NumProcessors = W.Length;
                int NumClusters = W[0].NumClusters;
                int NumVars = W[0].NumVars;
                Utils.Assign(Global_CaseCount, 0.0);
                Utils.Assign(Global_S, 0.0);
                for (int i = 0; i < NumProcessors; i++)
                {
                    Worker TT = W[i];
                    for (int C = 0; C < NumClusters; C++)
                    {
                        Global_CaseCount[C] += TT.CaseCount[C];
                        for (int V = 0; V < NumVars; V++)
                            Global_S[C][V] += TT.S1[C][V];
                    }
                }

                //Calculate the centroid using totals just calculated
                for (int k = 0; k < NumClusters; k++)
                {
                    if (Global_CaseCount[k] > 0)
                    {
                        for (int m = 0; m < NumVars; m++)
                            Global_S[k][m] /= Global_CaseCount[k];
                    }
                }
            }
           


            public void Execute()
            {
                int kmin = -1;
                double dmin;
                Changed = false;
                Utils.Assign(CaseCount, 0);
                for (int n = StartCase; n <= EndCase; n++)
                {
                    double[] OneCase = X[n];
                    dmin = double.MaxValue;
                    for (int k = 0; k < NumClusters; k++)
                    {
                        double[] OneRowS = S[k];
                        double d = 0.0;
                        for (int m = 0; m < NumVars; m++)
                        {
                            double Diff = OneCase[m] - OneRowS[m];
                            d += Diff * Diff;
                        }
                        if (d < dmin)
                        {
                            dmin = d;
                            kmin = k;
                        }
                    }
                    if (kmin != Membership[n]) Changed = true;
                    Membership[n] = kmin;
                    CaseCount[kmin]++;
                }

                //Do the summation for this part of the data set
                Utils.Assign(S1, 0.0);
                for (int n = StartCase; n <= EndCase; n++)
                    for (int m = 0; m < NumVars; m++)
                    {
                        int Assignment = Membership[n];
                        S1[Assignment][m] += X[n][m];
                    }
            }
        }

        /// <summary>
        /// Execute the K-means clustering algorithm
        /// </summary>
        /// <param name="DumpToConsole">When true, dumps the stepwise output to the console</param>
        /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
        /// <param name="Membership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
        /// <returns>The total distance between cluster centers and members.  This is what the routine seeks to minimize.  The sum of all E[NumClusters]</returns>
        public double Calculate(
            bool DumpToConsole,
            double[][] XX,
            ref int[] Membership,
            int NumClusters,
            int NumProcessors)
        {
            //Setup the object
            this.Membership = Membership;
            this.NumCases = XX.GetLength(0);
            this.NumVars = XX[0].GetLength(0);
            this.NumClusters = NumClusters;
            this.D = double.MaxValue;
            this.X = XX;
         

            //Dimension and initialize the necessary arrays and other vars
            //Matrices and vectors are zeroed / initialized using the Assign method
            S = new double[NumClusters][];
            for (int i = 0; i < NumClusters; i++)
                S[i] = new double[NumVars];
            Utils.Assign(S, 0.0);
            E = new double[NumClusters];
            Utils.Assign(E, 0.0);
            CaseCount = new double[NumClusters];
            Utils.Assign(CaseCount, 0.0);

            //Check cluster membership ID for legitimacy
            //Calculate the number of starting iCases per starting cluster
            for (int iCase = 0; iCase < NumCases; iCase++) //Loops 1 and 2
            {
                /* Logically, a cluster ID in the cells of the Membership array should range from 1 to NumClusters. 
                 * Indeed, that is what the program expects the input to be.  The problem with that is that it makes 
                 * programming with zero based arrays messy, and potentially buggy,especially when the original FORTRAN
                 * code arrays were one-based to begin with.  Hence, in this loop, we adjust all the cluster IDs
                 * to be zero-based too, i.e. Cluster ID one is reassigned an ID of zero.
                 * Late in the program in the output phase, we will readjust the IDs in the Membership array back to being
                 * one based. You will also see constructs in the program such as (ClusterID + 1).ToString() where we need 
                 * to convert them back for the sake of error messages, console printout, etc.
                 */
                Membership[iCase] -= 1;  //Here is where we adjust the ID
                int ClusterID = Membership[iCase];
                if ((ClusterID < 0) || (ClusterID >= NumClusters))
                    throw new Exception("Cluster membership index of " + (ClusterID + 1).ToString() +
                                        " in case " + (iCase).ToString() +
                                        " is out of correct range 1 to " + NumClusters.ToString());
                CaseCount[ClusterID] += 1.0;
            }

            //Calculate the centroids of the clusters 1:  Do the summation of X
            for (int iCase = 0; iCase < NumCases; iCase++)  //Loops 3 and 4
            {
                int ClusterID = Membership[iCase];
                for (int Variable = 0; Variable < NumVars; Variable++)
                    S[ClusterID][Variable] = S[ClusterID][Variable] + X[iCase][Variable];
            }

            //Calculate the centroids of the clusters 2:  Do the division to get the mean
            for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++) //Loops 5 and 6
            {
                double Count = CaseCount[ClusterID];
                if (Count == 0.0)
                    throw new Exception("Cluster with index of " + (ClusterID + 1).ToString() + " has no cases within it.  Kmeans cannot continue.");
                for (int Variable = 0; Variable < NumVars; Variable++)
                    S[ClusterID][Variable] = S[ClusterID][Variable] / Count;
            }

            //Setup for the threads and iterations
            this.NumProcessors = NumProcessors;
            W = new Worker[NumProcessors];
            T = new Thread[NumProcessors];
            for (int i = 0; i < NumProcessors; i++)
                W[i] = new Worker(NumProcessors, i, NumCases, NumClusters, NumVars, Membership, S, X);
              
            //Do the iterations until there is no change between clusters
            for (; ; )
            {
                Worker.PrepareThreads(T, W);
                bool Changed = Worker.GlobalChange(W);
                if (!Changed) break;
                Worker.CombineThreads(W, S, CaseCount);
            }

            //Accumulate the sum of squared distances for each cluster
            for (int iCase = 0; iCase < NumCases; iCase++)
            {
                int ClusMemIdx = Membership[iCase];
                double F = 0.0;
                for (int k = 0; k < NumVars; k++)
                {
                    double Diff = S[ClusMemIdx][k] - X[iCase][k];
                    F = F + Diff * Diff;
                }
                E[ClusMemIdx] = E[ClusMemIdx] + F;
            }

            //Calculate total error
            D = 0.0;
            for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
                D = D + E[ClusterID];

            //Return cluster member numbers to original indexes
            for (int i = 0; i < NumCases; i++)
                Membership[i] += 1;

            return D;
        }
    }






    /// <summary>
    /// Hmeans clustering routine as defined in Numerical Recipes book
    /// 3rd edition, starting pages 848-850.  Note that the authors
    /// wrongly identify this as Kmeans.  It operates according to
    /// Spath's Hmeans principle.  This has parallel processing
    /// of the E step using Andrew's method of thread coordination.
    /// </summary>
    //public class Hmeans3a
    //{
    //    public double[][] X = null;         //Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.
    //    public int[] Membership = null;    //Cluster membership.  Initial and final.  Range of values is 1..NumClusters
    //    public double[] CaseCount = null;  //The number of cases in each cluster group.  Modified as program executes.
    //    public double[][] S = null;         //The cluster centroids, dimensioned [NumClusters, NumVars]
    //    public double[] E = null;          //The sum of squared distance of each member to its cluster centroid, dimensioned [NumClusters]
    //    public double D = double.MaxValue; //The sum of all E[NumClusters].  What the routine seeks to minimize
    //    public int NumVars = -1;           //Number of variables.  Designated as L in original Kmeans FORTRAN program
    //    public int NumClusters = -1;       //Number of clusters desired from the routine.  Designated as N in original Kmeans FORTRAN program
    //    public int NumCases = -1;          //Number of cases. designated as M in original Kmeans FORTRAN program
    //   // public Stack Swaps = null;         //Records the swap of cases in the Estep, for use in the MStep optimization


    //    private static int EStep(int StartCase, int EndCase, int NumVars, int NumClusters,
    //                             double[] CaseCount,
    //                             int[] Membership,
    //                             double[][] S,
    //                             double[][] X)
    //    {
    //        int k, n, m;
    //        int kmin = -1;
    //        double dmin, d;
    //        int nChange = 0;
    //        Utility.Assign(CaseCount, 0);
    //        for (n = StartCase; n <= EndCase; n++)
    //        {
    //            double[] OneCase = X[n];
    //            dmin = double.MaxValue;
    //            for (k = 0; k < NumClusters; k++)
    //            {
    //                double[] OneRowS = S[k];
    //                for (d = 0.0, m = 0; m < NumVars; m++)
    //                {
    //                    double Diff = OneCase[m] - OneRowS[m];
    //                    d += Diff * Diff;
    //                }
    //                if (d < dmin)
    //                {
    //                    dmin = d;
    //                    kmin = k;
    //                }
    //            }
    //            if (kmin != Membership[n]) nChange++;
    //            Membership[n] = kmin;
    //            CaseCount[kmin]++;
    //        }
    //        return nChange;
    //    }


    //    //Worker class for parallel version of EStep
    //    private class EStepThread
    //    {
    //        public int StartCase;  //Start case for estep
    //        public int EndCase; //End case for estep
    //        public int NumVars; //Number of independent variables
    //        public int NumClusters; //Number of clusters
    //        public int[] Membership; //Locally modified Membership, a reference
    //        public double[][] S;  //A reference to the centroids array
    //        public double[][] X;  //A reference to the data array 
    //        public int ChangeCount;  //The number of changes made cluster membership by the thread
    //                                 //Unless you want it for diagnostics, Changecount could be changed to a boolean
    //                                 //just to detect if a change has been made or not
    //        public ManualResetEventSlim ExecuteDone;
    //        public ManualResetEventSlim ContinueExecute;
    //        public bool KeepExecuting;  //Should the EStepThread keep executing

    //        public void Execute()
    //        {
    //            while (KeepExecuting)
    //            {
    //                int k, n, m;
    //                int kmin = -1;
    //                double dmin, d;
    //                ChangeCount = 0;
    //                for (n = StartCase; n <= EndCase; n++)
    //                {
    //                    double[] OneCase = X[n];
    //                    dmin = double.MaxValue;
    //                    for (k = 0; k < NumClusters; k++)
    //                    {
    //                        double[] OneRowS = S[k];
    //                        for (d = 0.0, m = 0; m < NumVars; m++)
    //                        {
    //                            double Diff = OneCase[m] - OneRowS[m];
    //                            d += Diff * Diff;
    //                        }
    //                        if (d < dmin)
    //                        {
    //                            dmin = d;
    //                            kmin = k;
    //                        }
    //                    }
    //                    if (kmin != Membership[n])
    //                    {
    //                        //Console.Write(n.ToString() + " ");
    //                        ChangeCount++;
    //                    }
    //                    Membership[n] = kmin;
    //                }

    //                ExecuteDone.Set();          //Say I'm done, then wait for the repeate execute to return
    //                ContinueExecute.Wait();    //Wait for the go ahead to repeat the execute routine again.
    //                ContinueExecute.Reset();
    //            }
    //        }
    //    }


    //    private static void MStep(int StartCase, 
    //                              int EndCase, 
    //                              int NumVars, 
    //                              int NumClusters,
    //                              double[] CaseCount,
    //                              int[] Membership,
    //                              double[][] S,
    //                              double[][] X)
    //    {
    //        int n, k, m;
            
    //        //Update the CaseCount;
    //        Utility.Assign(CaseCount, 0.0);
    //        for (n = StartCase; n <= EndCase; n++)
    //        {
    //            int ClusterIDX = Membership[n];
    //            CaseCount[ClusterIDX] += 1.0;
    //        }

    //        //Update the centroids in two steps
    //        //#1 do the summation
    //        Utility.Assign(S, 0.0);
    //        for (n = StartCase; n <= EndCase; n++)
    //            for (m = 0; m < NumVars; m++)
    //            {
    //                int Assignment = Membership[n];
    //                S[Assignment][m] += X[n][m];
    //            }
    //        //#2 do the division to get the mean for each cluster
    //        for (k = 0; k < NumClusters; k++)
    //        {
    //            if (CaseCount[k] > 0)
    //            {
    //                for (m = 0; m < NumVars; m++)
    //                    S[k][m] /= CaseCount[k];
    //            }
    //        }
    //    }


        

    //    /// <summary>
    //    /// Execute the K-means clustering algorithm
    //    /// </summary>
    //    /// <param name="DumpToConsole">When true, dumps the stepwise output to the console</param>
    //    /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
    //    /// <param name="Membership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
    //    /// <returns>The total distance between cluster centers and members.  This is what the routine seeks to minimize.  The sum of all E[NumClusters]</returns>
    //    public double Calculate(
    //        bool DumpToConsole,
    //        double[][] XX,
    //        ref int[] Membership,
    //        int NumClusters,
    //        int NumProcessors)
    //    {
    //        //Setup the object
    //        this.Membership = Membership;
    //        this.NumCases = XX.GetLength(0);
    //        this.NumVars = XX[0].GetLength(0);
    //        this.NumClusters = NumClusters;
    //        this.D = double.MaxValue;

    //        this.X = XX;
    //        //Console.WriteLine("Start copy");
    //        //this.X = new double[NumCases][];
    //        //for (int i = 0; i < NumCases; i++)
    //        //{
    //        //    this.X[i] = new double[NumVars];
    //        //    for (int j = 0; j < NumVars; j++)
    //        //        this.X[i][j] = XX[i, j];
    //        //}
    //        //Console.WriteLine("End copy");

    //        //Dimension and initialize the necessary arrays and other vars
    //        //Matrices and vectors are zeroed / initialized using the Assign method
    //        S = new double[NumClusters][];
    //        for (int i = 0; i < NumClusters; i++)
    //            S[i] = new double[NumVars];
    //        Utility.Assign(S, 0.0);
    //        E = new double[NumClusters];
    //        Utility.Assign(E, 0.0);
    //        CaseCount = new double[NumClusters];
    //        Utility.Assign(CaseCount, 0.0);

    //        //Check cluster membership ID for legitimacy
    //        //Calculate the number of starting iCases per starting cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++) //Loops 1 and 2
    //        {
    //            /* Logically, a cluster ID in the cells of the Membership array should range from 1 to NumClusters. 
    //             * Indeed, that is what the program expects the input to be.  The problem with that is that it makes 
    //             * programming with zero based arrays messy, and potentially buggy,especially when the original FORTRAN
    //             * code arrays were one-based to begin with.  Hence, in this loop, we adjust all the cluster IDs
    //             * to be zero-based too, i.e. Cluster ID one is reassigned an ID of zero.
    //             * Late in the program in the output phase, we will readjust the IDs in the Membership array back to being
    //             * one based. You will also see constructs in the program such as (ClusterID + 1).ToString() where we need 
    //             * to convert them back for the sake of error messages, console printout, etc.
    //             */
    //            Membership[iCase] -= 1;  //Here is where we adjust the ID
    //            int ClusterID = Membership[iCase];
    //            if ((ClusterID < 0) || (ClusterID >= NumClusters))
    //                throw new Exception("Cluster membership index of " + (ClusterID + 1).ToString() +
    //                                    " in case " + (iCase).ToString() +
    //                                    " is out of correct range 1 to " + NumClusters.ToString());
    //            CaseCount[ClusterID] += 1.0;
    //        }

    //        //Calculate the centroids of the clusters 1:  Do the summation of X
    //        for (int iCase = 0; iCase < NumCases; iCase++)  //Loops 3 and 4
    //        {
    //            int ClusterID = Membership[iCase];
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID][Variable] = S[ClusterID][Variable] + X[iCase][Variable];
    //        }

    //        //Calculate the centroids of the clusters 2:  Do the division to get the mean
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++) //Loops 5 and 6
    //        {
    //            double Count = CaseCount[ClusterID];
    //            if (Count == 0.0)
    //                throw new Exception("Cluster with index of " + (ClusterID + 1).ToString() + " has no cases within it.  Kmeans cannot continue.");
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID][Variable] = S[ClusterID][Variable] / Count;
    //        }

    //        //Setup for the two threads
    //        //this.NumProcessors = NumProcessors;
    //        int NumCasesForEach = NumCases / NumProcessors;
    //        //int StartFirstHalf = 0;
    //        //int EndFirstHalf = (NumCases / 2) - 1;
    //        //int StartSecondHalf = EndFirstHalf + 1;
    //        //int EndSecondHalf = NumCases - 1;

    //        int Processor = 0;
    //        EStepThread E1 = new EStepThread();
    //        E1.NumClusters = NumClusters;
    //        E1.Membership = Membership;
    //        E1.NumVars = NumVars;
    //        E1.StartCase = Processor * NumCasesForEach;
    //        E1.EndCase = E1.StartCase + NumCasesForEach - 1;
    //        E1.X = X;
    //        E1.S = S;
    //        E1.ExecuteDone = new ManualResetEventSlim(false);
    //        E1.ContinueExecute = new ManualResetEventSlim(false);
    //        E1.KeepExecuting = true;

    //        Processor++;
    //        EStepThread E2 = new EStepThread();
    //        E2.NumClusters = NumClusters;
    //        E2.Membership = Membership;
    //        E2.NumVars = NumVars;
    //        E2.StartCase = Processor * NumCasesForEach;
    //        E2.EndCase = E2.StartCase + NumCasesForEach - 1;
    //        E2.X = X;
    //        E2.S = S;
    //        E2.ExecuteDone = new ManualResetEventSlim(false);
    //        E2.ContinueExecute = new ManualResetEventSlim(false);
    //        E2.KeepExecuting = true;

    //        Processor++;
    //        EStepThread E3 = new EStepThread();
    //        E3.NumClusters = NumClusters;
    //        E3.Membership = Membership;
    //        E3.NumVars = NumVars;
    //        E3.StartCase = Processor * NumCasesForEach;
    //        E3.EndCase = NumCases - 1;
    //        E3.X = X;
    //        E3.S = S;
    //        E3.ExecuteDone = new ManualResetEventSlim(false);
    //        E3.ContinueExecute = new ManualResetEventSlim(false);
    //        E3.KeepExecuting = true;
            

    //        //Now setup all three
    //        Thread T1 = new Thread(E1.Execute);
    //        T1.Name = "Cases " + E1.StartCase + "-> " + E1.EndCase;
    //        Thread T2 = new Thread(E2.Execute);
    //        T2.Name = "Cases " + E2.StartCase + "-> " + E2.EndCase;
    //        Thread T3 = new Thread(E3.Execute);
    //        T3.Name = "Cases " + E3.StartCase + "-> " + E3.EndCase;

    //        T1.Start();
    //        T2.Start();
    //        T3.Start();

    //        for (; ; )
    //        {
    //            //Wait for every thread on the ExecuteDone handle
    //            E1.ExecuteDone.Wait();    //wait until the completion event is signaled
    //            E2.ExecuteDone.Wait();
    //            E3.ExecuteDone.Wait();

    //            E1.ExecuteDone.Reset();     //Reset the completion event
    //            E2.ExecuteDone.Reset();
    //            E3.ExecuteDone.Reset();
                
                
    //            int TotalChange = E1.ChangeCount + E2.ChangeCount + E3.ChangeCount;
    //            //Console.WriteLine(E1.ChangeCount.ToString() + "  " + E2.ChangeCount.ToString());
    //            if ((TotalChange == 0))
    //            {
    //                E1.KeepExecuting = false;
    //                E2.KeepExecuting = false;
    //                E3.KeepExecuting = false;


    //                E1.ContinueExecute.Set(); //release the thread from waiting
    //                E2.ContinueExecute.Set();
    //                E3.ContinueExecute.Set();
    //                break;
    //            }
    //            MStep(0, NumCases - 1, NumVars, NumClusters, CaseCount, Membership, S, X);

    //            E1.ContinueExecute.Set(); //release the thread from waiting
    //            E2.ContinueExecute.Set();
    //            E3.ContinueExecute.Set();
    //        }

    //        //Accumulate the sum of squared distances for each cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++)
    //        {
    //            int ClusMemIdx = Membership[iCase];
    //            double F = 0.0;
    //            for (int k = 0; k < NumVars; k++)
    //            {
    //                double Diff = S[ClusMemIdx][k] - X[iCase][k];
    //                F = F + Diff * Diff;
    //            }
    //            E[ClusMemIdx] = E[ClusMemIdx] + F;
    //        }

    //        //Calculate total error
    //        D = 0.0;
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
    //            D = D + E[ClusterID];

    //        //Return cluster member numbers to original indexes
    //        for (int i = 0; i < NumCases; i++)
    //            Membership[i] += 1;

    //        return D;
    //    }
    //}





    /// <summary>
    /// Hmeans clustering routine as defined in Numerical Recipes book
    /// 3rd edition, starting pages 848-850.  Note that the authors
    /// wrongly identify this as Kmeans.  It operates according to
    /// Spath's Hmeans principle.  This has some optimizations in the
    /// EStep which double the speed over HMeans1.
    /// </summary>
    //public class Hmeans2
    //{
    //    public double[][] X = null;         //Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.
    //    public int[] Membership = null;    //Cluster membership.  Initial and final.  Range of values is 1..NumClusters
    //    public double[] CaseCount = null;  //The number of cases in each cluster group.  Modified as program executes.
    //    public double[][] S = null;         //The cluster centroids, dimensioned [NumClusters, NumVars]
    //    public double[] E = null;          //The sum of squared distance of each member to its cluster centroid, dimensioned [NumClusters]
    //    public double D = double.MaxValue; //The sum of all E[NumClusters].  What the routine seeks to minimize
    //    public int NumVars = -1;           //Number of variables.  Designated as L in original Kmeans FORTRAN program
    //    public int NumClusters = -1;       //Number of clusters desired from the routine.  Designated as N in original Kmeans FORTRAN program
    //    public int NumCases = -1;          //Number of cases. designated as M in original Kmeans FORTRAN program
    //    public Stack Swaps = null;         //Records the swap of cases in the Estep, for use in the MStep optimization

    //    //This works fine, but is not optimized in any way
    //    private static void MStep(int StartCase, int EndCase, int NumVars, int NumClusters,
    //                              double[] CaseCount,
    //                              int[] Membership,
    //                              double[][] S,
    //                              double[][] X,
    //                              Stack Swaps)
    //    {
    //        int n, k, m;
    //        //for (k = 0; k < NumClusters; k++)
    //        //    for (m = 0; m < NumVars; m++)
    //        //        S[k][m] = 0.0;

    //        Utility.Assign(S, 0.0);

    //        for (n = StartCase; n <= EndCase; n++)
    //            for (m = 0; m < NumVars; m++)
    //            {
    //                int Assignment = Membership[n];
    //                S[Assignment] [m] += X[n][m];
    //            }
    //        for (k = 0; k < NumClusters; k++)
    //        {
    //            if (CaseCount[k] > 0)
    //            {
    //                for (m = 0; m < NumVars; m++)
    //                    S[k][m] /= CaseCount[k];
    //            }
    //        }
    //    }


    //    private static int EStep(int StartCase, int EndCase, int NumVars, int NumClusters,
    //                             double[] CaseCount,
    //                             int[] Membership,
    //                             double[][] S,
    //                             double[][] X,
    //                             Stack Swaps)
    //    {
    //        int k, n, m;
    //        int kmin = -1;
    //        double dmin, d;
    //        int nChange = 0;
    //        Utility.Assign(CaseCount, 0);
    //        for (n = StartCase; n <= EndCase; n++)
    //        {
    //            double[] OneCase = X[n];
    //            dmin = double.MaxValue;
    //            for (k = 0; k < NumClusters; k++)
    //            {
    //                double[] OneRowS = S[k];
    //                for (d = 0.0, m = 0; m < NumVars; m++)
    //                {
    //                    double Diff = OneCase[m] - OneRowS[m]; 
    //                    d += Diff * Diff;
    //                }
    //                if (d < dmin)
    //                {
    //                    dmin = d;
    //                    kmin = k;
    //                }
    //            }
    //            if (kmin != Membership[n]) nChange++;
    //            Membership[n] = kmin;
    //            CaseCount[kmin]++; 
    //        }
    //        return nChange;
    //    }

    //    /// <summary>
    //    /// Execute the K-means clustering algorithm
    //    /// </summary>
    //    /// <param name="DumpToConsole">When true, dumps the stepwise output to the console</param>
    //    /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
    //    /// <param name="Membership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
    //    /// <returns>The total distance between cluster centers and members.  This is what the routine seeks to minimize.  The sum of all E[NumClusters]</returns>
    //    public double Calculate(
    //        bool DumpToConsole,
    //        double[,] XX,
    //        ref int[] Membership,
    //        int NumClusters)
    //    {
    //        //Setup the object
    //        this.Membership = Membership;
    //        this.NumCases = XX.GetLength(0);
    //        this.NumVars = XX.GetLength(1);
    //        this.NumClusters = NumClusters;
    //        this.D = double.MaxValue;

    //        //this.X = X;
    //        Console.WriteLine("Start copy");
    //        this.X = new double[NumCases][];
    //        for (int i = 0; i < NumCases; i++)
    //        {
    //            this.X[i] = new double[NumVars];
    //            for (int j = 0; j < NumVars; j++)
    //                this.X[i][j] = XX[i, j];
    //        }
    //        Console.WriteLine("End copy");

    //        //Dimension and initialize the necessary arrays and other vars
    //        //Matrices and vectors are zeroed / initialized using the Assign method
    //        S = new double[NumClusters][];
    //        for (int i = 0; i < NumClusters; i++) 
    //            S[i] = new double[NumVars];
    //        Utility.Assign(S, 0.0);
    //        E = new double[NumClusters];
    //        Utility.Assign(E, 0.0);
    //        CaseCount = new double[NumClusters];
    //        Utility.Assign(CaseCount, 0.0);

    //        //Check cluster membership ID for legitimacy
    //        //Calculate the number of starting iCases per starting cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++) //Loops 1 and 2
    //        {
    //            /* Logically, a cluster ID in the cells of the Membership array should range from 1 to NumClusters. 
    //             * Indeed, that is what the program expects the input to be.  The problem with that is that it makes 
    //             * programming with zero based arrays messy, and potentially buggy,especially when the original FORTRAN
    //             * code arrays were one-based to begin with.  Hence, in this loop, we adjust all the cluster IDs
    //             * to be zero-based too, i.e. Cluster ID one is reassigned an ID of zero.
    //             * Late in the program in the output phase, we will readjust the IDs in the Membership array back to being
    //             * one based. You will also see constructs in the program such as (ClusterID + 1).ToString() where we need 
    //             * to convert them back for the sake of error messages, console printout, etc.
    //             */
    //            Membership[iCase] -= 1;  //Here is where we adjust the ID
    //            int ClusterID = Membership[iCase];
    //            if ((ClusterID < 0) || (ClusterID >= NumClusters))
    //                throw new Exception("Cluster membership index of " + (ClusterID + 1).ToString() +
    //                                    " in case " + (iCase).ToString() +
    //                                    " is out of correct range 1 to " + NumClusters.ToString());
    //            CaseCount[ClusterID] += 1.0;
    //        }

    //        //Calculate the centroids of the clusters 1:  Do the summation of X
    //        for (int iCase = 0; iCase < NumCases; iCase++)  //Loops 3 and 4
    //        {
    //            int ClusterID = Membership[iCase];
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID][Variable] = S[ClusterID][Variable] + X[iCase][Variable];
    //        }

    //        //Calculate the centroids of the clusters 2:  Do the division to get the mean
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++) //Loops 5 and 6
    //        {
    //            double Count = CaseCount[ClusterID];
    //            if (Count == 0.0)
    //                throw new Exception("Cluster with index of " + (ClusterID + 1).ToString() + " has no cases within it.  Kmeans cannot continue.");
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID][Variable] = S[ClusterID][Variable] / Count;
    //        }

    //        int StartFirstHalf = 0;
    //        int EndFirstHalf = (NumCases / 2) - 1;
    //        int StartSecondHalf = EndFirstHalf + 1;
    //        int EndSecondHalf = NumCases - 1;
    //        Swaps = new Stack(NumCases);

    //        for (; ; )
    //        {
    //            int nChange = EStep(StartFirstHalf, EndSecondHalf, NumVars, NumClusters, CaseCount, Membership, S, X, Swaps);
    //            //Console.WriteLine(nChange);
    //            if (nChange == 0) break;
    //            MStep(StartFirstHalf, EndSecondHalf, NumVars, NumClusters, CaseCount, Membership, S, X, Swaps);
    //        }

    //        //Accumulate the sum of squared distances for each cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++)
    //        {
    //            int ClusMemIdx = Membership[iCase];
    //            double F = 0.0;
    //            for (int k = 0; k < NumVars; k++)
    //            {
    //                double Diff = S[ClusMemIdx][k] - X[iCase][k];
    //                F = F + Diff * Diff;
    //            }
    //            E[ClusMemIdx] = E[ClusMemIdx] + F;
    //        }

    //        //Calculate total error
    //        D = 0.0;
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
    //            D = D + E[ClusterID];

    //        //Return cluster member numbers to original indexes
    //        for (int i = 0; i < NumCases; i++)
    //            Membership[i] += 1;

    //        return D;
    //    }




    //}






    /// <summary>
    /// Hmeans clustering routine as defined in Numerical Recipes book
    /// 3rd edition, starting pages 848-850.  Note that the authors
    /// wrongly identify this as Kmeans.  It operates according to
    /// Spath's Hmeans principle.  Although I may not understand it perfectly
    /// the steps identified as the MStep and EStep appear to be quite brute
    /// force with some opportunities for optimizing, particularly just to 
    /// deal with points that have been moved since last iteration instead
    /// of dealing with all points all the time.
    /// </summary>
    //public class Hmeans1
    //{
    //    public double[,] X = null;         //Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.
    //    public int[] Membership = null;    //Cluster membership.  Initial and final.  Range of values is 1..NumClusters
    //    public double[] CaseCount = null;  //The number of cases in each cluster group.  Modified as program executes.
    //    public double[,] S = null;         //The cluster centroids, dimensioned [NumClusters, NumVars]
    //    public double[] E = null;          //The sum of squared distance of each member to its cluster centroid, dimensioned [NumClusters]
    //    public double D = double.MaxValue; //The sum of all E[NumClusters].  What the routine seeks to minimize
    //    public int NumVars = -1;           //Number of variables.  Designated as L in original Kmeans FORTRAN program
    //    public int NumClusters = -1;       //Number of clusters desired from the routine.  Designated as N in original Kmeans FORTRAN program
    //    public int NumCases = -1;          //Number of cases. designated as M in original Kmeans FORTRAN program

    //    private static void MStep(int StartCase, int EndCase, int NumVars, int NumClusters, 
    //                              double[] CaseCount, 
    //                              int[] Membership, 
    //                              double[,] S, 
    //                              double[,] X)
    //    {
    //        int n, k, m;
    //        for (k = 0; k < NumClusters; k++) 
    //            for (m = 0; m < NumVars; m++) 
    //                S[k, m] = 0.0;
    //        for (n = StartCase; n <= EndCase; n++)
    //            for (m = 0; m < NumVars; m++)
    //            {
    //                int Assignment = Membership[n];
    //                S[Assignment, m] += X[n, m];
    //            }
    //        for (k = 0; k < NumClusters; k++)
    //        {
    //            if (CaseCount[k] > 0)
    //            {
    //                for (m = 0; m < NumVars; m++)
    //                    S[k, m] /= CaseCount[k];
    //            }
    //        }
    //    }


    //    private static int EStep(int StartCase, int EndCase, int NumVars, int NumClusters, 
    //                             double [] CaseCount, 
    //                             int[] Membership, 
    //                             double[,] S, 
    //                             double[,] X)
    //    {
    //        int k, n, m;
    //        int kmin = -1;
    //        double dmin, d;
    //        int nChange = 0;
    //        for (k = 0; k < NumClusters; k++) CaseCount[k] = 0;
    //        for (n = StartCase; n <= EndCase; n++)
    //        {
    //            dmin = double.MaxValue;
    //            for (k = 0; k < NumClusters; k++)
    //            {
    //                for (d = 0.0, m = 0; m < NumVars; m++)
    //                {
    //                    double Diff = X[n, m] - S[k, m];
    //                    d += Diff * Diff;
    //                }
    //                if (d < dmin)
    //                {
    //                    dmin = d;
    //                    kmin = k;
    //                }
    //            }
    //            if (kmin != Membership[n]) nChange++;
    //            Membership[n] = kmin;
    //            CaseCount[kmin]++;
    //        }
    //        return nChange;
    //    }

    //    /// <summary>
    //    /// Execute the K-means clustering algorithm
    //    /// </summary>
    //    /// <param name="DumpToConsole">When true, dumps the stepwise output to the console</param>
    //    /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
    //    /// <param name="Membership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
    //    /// <returns>The total distance between cluster centers and members.  This is what the routine seeks to minimize.  The sum of all E[NumClusters]</returns>
    //    public double Calculate(
    //        bool DumpToConsole,
    //        double[,] X,
    //        ref int[] Membership,
    //        int NumClusters)
    //    {
    //        //Setup the object
    //        this.Membership = Membership;
    //        this.X = X;
    //        this.NumCases = X.GetLength(0);
    //        this.NumVars = X.GetLength(1);
    //        this.NumClusters = NumClusters;
    //        this.D = double.MaxValue;

    //        //Dimension and initialize the necessary arrays and other vars
    //        //Matrices and vectors are zeroed / initialized using the Assign method
    //        S = new double[NumClusters, NumVars]; 
    //        Utility.Assign(S, 0.0);
    //        E = new double[NumClusters]; 
    //        Utility.Assign(E, 0.0);
    //        CaseCount = new double[NumClusters]; 
    //        Utility.Assign(CaseCount, 0.0);

    //        //Check cluster membership ID for legitimacy
    //        //Calculate the number of starting iCases per starting cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++) //Loops 1 and 2
    //        {
    //            /* Logically, a cluster ID in the cells of the Membership array should range from 1 to NumClusters. 
    //             * Indeed, that is what the program expects the input to be.  The problem with that is that it makes 
    //             * programming with zero based arrays messy, and potentially buggy,especially when the original FORTRAN
    //             * code arrays were one-based to begin with.  Hence, in this loop, we adjust all the cluster IDs
    //             * to be zero-based too, i.e. Cluster ID one is reassigned an ID of zero.
    //             * Late in the program in the output phase, we will readjust the IDs in the Membership array back to being
    //             * one based. You will also see constructs in the program such as (ClusterID + 1).ToString() where we need 
    //             * to convert them back for the sake of error messages, console printout, etc.
    //             */
    //            Membership[iCase] -= 1;  //Here is where we adjust the ID
    //            int ClusterID = Membership[iCase];
    //            if ((ClusterID < 0) || (ClusterID >= NumClusters))
    //                throw new Exception("Cluster membership index of " + (ClusterID + 1).ToString() +
    //                                    " in case " + (iCase).ToString() +
    //                                    " is out of correct range 1 to " + NumClusters.ToString());
    //            CaseCount[ClusterID] += 1.0;
    //        }

    //        //Calculate the centroids of the clusters 1:  Do the summation of X
    //        for (int iCase = 0; iCase < NumCases; iCase++)  //Loops 3 and 4
    //        {
    //            int ClusterID = Membership[iCase];
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID, Variable] = S[ClusterID, Variable] + X[iCase, Variable];
    //        }

    //        //Calculate the centroids of the clusters 2:  Do the division to get the mean
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++) //Loops 5 and 6
    //        {
    //            double Count = CaseCount[ClusterID];
    //            if (Count == 0.0)
    //                throw new Exception("Cluster with index of " + (ClusterID + 1).ToString() + " has no cases within it.  Kmeans cannot continue.");
    //            for (int Variable = 0; Variable < NumVars; Variable++)
    //                S[ClusterID, Variable] = S[ClusterID, Variable] / Count;
    //        }

    //        int StartFirstHalf = 0;
    //        int EndFirstHalf = (NumCases / 2) - 1;
    //        int StartSecondHalf = EndFirstHalf + 1;
    //        int EndSecondHalf = NumCases - 1;

    //        for (; ; )
    //        {
    //            int nChange = EStep(StartFirstHalf, EndSecondHalf, NumVars, NumClusters, CaseCount, Membership, S, X);
    //            if (nChange == 0) break;
    //            MStep(StartFirstHalf, EndSecondHalf, NumVars, NumClusters, CaseCount, Membership, S, X);
    //        }

    //        //Accumulate the sum of squared distances for each cluster
    //        for (int iCase = 0; iCase < NumCases; iCase++) 
    //        {
    //            int ClusMemIdx = Membership[iCase];
    //            double F = 0.0;
    //            for (int k = 0; k < NumVars; k++)
    //            {
    //                double Diff = S[ClusMemIdx, k] - X[iCase, k];
    //                F = F + Diff * Diff;
    //            }
    //            E[ClusMemIdx] = E[ClusMemIdx] + F;
    //        }

    //        //Calculate total error
    //        D = 0.0;
    //        for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
    //            D = D + E[ClusterID];

    //        //Return cluster member numbers to original indexes
    //        for (int i = 0; i < NumCases; i++)
    //            Membership[i] += 1;

    //        return D;
    //    }




    //}
}
