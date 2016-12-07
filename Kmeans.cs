using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Carto
{
    /// <summary>
    /// Kmeans clustering algorithm as writin in Spath's clustering book. Adapted slightly
    /// where minor adjustment was warranted while remaining faithful
    /// to Spath's algorithm.
    /// </summary>
    public class Kmeans1
    {
        public int NumCases = -1;     //Total number of cases in the dataset
        public int NumVars = -1;      //Total number of independent variables used for the clustering
        public int NumClusters = -1;  //The number of clusters desired.  Does not change.
        public double[][] X = null;   //The data matrix with dimension [NumCases, NumVars]
        public int[] Membership = null; //The membership matrix dimensioned [NumCases] with cell values ranging from 1..NumClusters
        public double[] CaseCount = null; //The number of cases in each cluster, dimensioned [NumClusters]
        public double[][] S = null;  //The cluster centroids dimensioned [NumClusters, NumVars]
        public double[] E = null;    //The error associated with each cluster, dimensioned [NumClusters]
        public int NumProcessors = -1;    //The number of processors to use in parallel processing portions of the algorithm
        
        private enum ExitCondition { NOSWAP, SWAP };

        /// <summary>
        /// See if the case presented needs to be moved to a different cluster.  If so, do it.
        /// </summary>
        /// <param name="Case">The case under consideration</param>
        /// <param name="D">The sum of all E[NumClusters].  What the entire Kmeans algorithm seeks to minimize</param>
        /// <returns>An exit condition representing whether a swap between clusters has taken place</returns>
        private ExitCondition Iterate(int Case, ref double D)
        {
            double[] XX = X[Case];
            int OldMemID = Membership[Case];
            double[] S_Old = S[OldMemID];
            double OldClusterCaseCount = CaseCount[OldMemID];
            if (OldClusterCaseCount <= 1.0) return ExitCondition.NOSWAP;

            int CandidateMemID = -1;
            double CandidateDist = double.MaxValue;
            for (int j = 0; j < NumClusters; j++)  //Loops 12 and 13
            {
                if (j == OldMemID) continue;
                double Count = CaseCount[j];
                double Weight = Count / (Count + 1.0);
                
                //Start embedded SquaredDist function
                //double Dist = SquaredDist(S, X, Case, j, NumVars);
                double Dist = 0.0;
                for (int k = 0; k < NumVars; k++)
                {
                    double Diff = S[j][k] - XX[k];
                    Dist = Dist + Diff * Diff;
                }    
                //End embedded SquaredDist function

                Dist = Dist * Weight;
                if (Dist <= CandidateDist)
                {
                    CandidateDist = Dist;
                    CandidateMemID = j;
                }
            }

            //Check to see how potential new assignment of the case 
            // compares to the present cluster assignement for the case
            OldClusterCaseCount = CaseCount[OldMemID];
            double OldClusterWeight = OldClusterCaseCount / (OldClusterCaseCount - 1.0);

            //Begin embedded SquaredDist function
            //double OldClusterF = SquaredDist(S, X, Case, OldMemID, NumVars);
            double OldClusterF = 0.0;
            for (int k = 0; k < NumVars; k++)
            {
                double Diff = S[OldMemID][k] - XX[k];
                OldClusterF = OldClusterF + Diff * Diff;
            } 
            //End embedded SquaredDist function
            
            double OldClusterDist = OldClusterWeight * OldClusterF;
            if (CandidateDist >= OldClusterDist)
                return ExitCondition.NOSWAP;

            //We have a case to move to another cluster, i.e. it gets a 
            //  new membership ID, and cluster gets another member
            int NewMemID = CandidateMemID;
            double[] S_New = S[NewMemID];
            double NewClusterDist = CandidateDist;
            double NewClusterCaseCount = CaseCount[NewMemID];

            //Move the case from old cluster to new one, updating the 
            //  error values and centroids efficiently
            E[OldMemID] = E[OldMemID] - OldClusterDist;
            E[NewMemID] = E[NewMemID] + NewClusterDist;
            D = D - OldClusterDist + NewClusterDist;
            double OldClusterShift = 1.0 / (OldClusterCaseCount - 1.0);
            double NewClusterShift = 1.0 / (NewClusterCaseCount + 1.0);
            for (int Variable = 0; Variable < NumVars; Variable++)  //Do loop 15
            {
                double F = XX[Variable];
                S_Old[Variable] = (OldClusterCaseCount * S_Old[Variable] - F) * OldClusterShift;
                S_New[Variable] = (NewClusterCaseCount * S_New[Variable] + F) * NewClusterShift;
            }
            Membership[Case] = NewMemID;
            CaseCount[OldMemID] = CaseCount[OldMemID] - 1;
            CaseCount[NewMemID] = CaseCount[NewMemID] + 1;
            return ExitCondition.SWAP;
        }

        /// <summary>
        /// Dump the cluster membership to the console
        /// </summary>
        /// <param name="Membership">The array containing the membership</param>
        /// <param name="NumCases">The number of cases in the array</param>
        public static void DumpMembership(int[] Membership, int NumCases)
        {
            Console.WriteLine("***************************************");
            for (int i = 0; i < NumCases; i++) Console.Write((Membership[i] + 1).ToString() + " ");
            Console.WriteLine();
        }

        /// <summary>
        /// Get a frequency count for each cluster
        /// </summary>
        /// <param name="MembershipIndex">The variable containing the cluster indices</param>
        /// <param name="MemberCount">The frequency listing</param>
        public static void CountClusterMembers(int[] MembershipIndex, out int[] MemberCount)
        {
            int MaxCluster = MembershipIndex.Max();
            MemberCount = new int[MaxCluster];
            for (int i = 0; i < MemberCount.Length; i++) MemberCount[i] = 0;
            for (int i = 0; i < MembershipIndex.Length; i++)
            {
                int MemberIndex = MembershipIndex[i] - 1;
                MemberCount[MemberIndex]++;
            }
        }

        /// <summary>
        /// Produce an index containing the record numbers for each cluster index
        /// </summary>
        /// <param name="MembershipIndex">The integer variable containing the cluster index numbers for each record</param>
        /// <param name="Index">The index itself of dimension Index[NumClusters][] where the second array contains the record index numbers</param>
        public static void CollectClusterRecordIndexes(int[] MembershipIndex, out int[][] Index)
        {
            int MaxClusIdx = MembershipIndex.Max();
            int NumRecs = MembershipIndex.Length;

            //Allocate the indexer as an array of arrays,
            //permitting different number of cluster members
            //for each possible cluster index
            Index = new int[MaxClusIdx][];

            //Now find out how many cases are in each cluster
            int[] MemberCount = new int[MaxClusIdx];
            for (int i = 0; i < MaxClusIdx; i++) MemberCount[i] = 0;
            for (int i = 0; i < NumRecs; i++)
            {
                int ClusIdx = MembershipIndex[i];
                MemberCount[ClusIdx - 1]++;
            }

            //Allocate the right number of cells for each cluster
            //Next fill the cells for each respective cluster
            //with the record numbers of their members
            for (int i = 0; i < MaxClusIdx; i++) Index[i] = new int[MemberCount[i]];
            int[] CellIdx = new int[MaxClusIdx];
            for (int i = 0; i < MaxClusIdx; i++) CellIdx[i] = -1;
            for (int i = 0; i < NumRecs; i++)
            {
                int ClusIdx = MembershipIndex[i] - 1;
                CellIdx[ClusIdx] += 1;
                Index[ClusIdx][CellIdx[ClusIdx]] = i;
            }
        }

      
         
        /// <summary>
        /// Execute the K-means clustering algorithm
        /// </summary>
        /// <param name="DumpToConsole">When true, dumps intermediate results to console.  Not recommended for large data sets.</param>
        /// <param name="XX">The data array with dimension [NumCases, NumVariables]</param>
        /// <param name="InOutMembership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
        /// <returns>D, the sum of the error across all the clusters</returns>
        public double Calculate(
            bool DumpToConsole,
            double[][] XX,
            ref int[] ClusMembership,
            int NumClusters,
            int NumProcessors)

        {
            //Setup object to an initial state
            NumCases = XX.Length;
            NumVars = XX[0].Length;
            this.NumClusters = NumClusters;
            S = new double[NumClusters][];
            for (int i = 0; i < NumClusters; i++)
                S[i] = new double[NumVars];
            Utils.Assign(S, 0.0);
            E = new double[NumClusters];
            Utils.Assign(E, 0);
            CaseCount = new double[NumClusters];
            Utils.Assign(CaseCount, 0);
            this.X = XX;
            this.Membership = ClusMembership;
            this.NumProcessors = NumProcessors;

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
                                        " in case " + (iCase + 1).ToString() + 
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

            //Accumulate the sum of squared distances for each cluster 
            for (int iCase = 0; iCase < NumCases; iCase++) //Loops 7 and 8
            {
                int ClusMemIdx = Membership[iCase];

                //Embed SquaredDist function with inline code
                //double F = SquaredDist(S, X, iCase, ClusMemIdx, NumVars);
                double F = 0.0;
                for (int k = 0; k < NumVars; k++)
                {
                    double Diff = S[ClusMemIdx][k] - X[iCase][k];
                    F = F + Diff * Diff;
                }
                //End embedded SquaredDist function
                E[ClusMemIdx] = E[ClusMemIdx] + F;
            }

            //Loop DO 9.  Calculate total error
            double D = 0.0;
            for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
                D = D + E[ClusterID];

            //Begin iteration phase of the routine
            int Case = -1;
            int Iteration = 0;
            while (true)
            { 
                Case += 1;
                if (Case == NumCases) Case = 0;
                if (Iteration == NumCases) break;
                ExitCondition ExitCon = Iterate(Case, ref D);
                if (ExitCon == ExitCondition.SWAP)
                {
                    Iteration = 0;
                    if (DumpToConsole) DumpMembership(Membership, NumCases);
                }
                else
                    Iteration += 1;
            }

            //Return cluster member numbers to original indexes 1..NumClusters
            for (int i = 0; i < NumCases; i++) 
                Membership[i] += 1;

            return D;
        }

    }
}



//public static class KmeansOriginal
//    {
//        private enum ExitCondition { NOSWAP, SWAP };

//        /// <summary>
//        /// Multivariate Euclidean distance measure
//        /// </summary>
//        /// <param name="S">The cluster centroids, dimensioned [NumClusters, NumVars]</param>
//        /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
//        /// <param name="CaseNum">The case we are doing the measurement on</param>
//        /// <param name="ClusMemIdx">The cluster to which the measurement belongs</param>
//        /// <param name="NumVars">The number of independent variables in the measurement</param>
//        /// <returns></returns>
//        private static double SquaredDist(double[,] S, double[,] X, int CaseNum, int ClusMemIdx, int NumVars)
//        {
//            double F = 0.0;
//            for (int k = 0; k < NumVars; k++)
//            {
//                double Diff = S[ClusMemIdx, k] - X[CaseNum, k];
//                F = F + Diff * Diff;
//            }
//            return F;
//        }

//        /// <summary>
//        /// See if the case presented needs to be moved to a different cluster.  If so, do it.
//        /// </summary>
//        /// <param name="Case">The case under consideration</param>
//        /// <param name="NumCases">Total number of cases in the data set</param>
//        /// <param name="NumVars">Number of independent variables involved</param>
//        /// <param name="NumClusters">Total number of clusters requested by user</param>
//        /// <param name="X">The independent data matrix</param>
//        /// <param name="Membership">Which cluster each case currently belongs to</param>
//        /// <param name="CaseCount">The number of cases residing in each cluster</param>
//        /// <param name="S">The cluster centroids, dimensioned [NumClusters, NumVars]</param>
//        /// <param name="E">The sum of squares of each member to its cluster centroid, dimensioned [NumClusters]</param>
//        /// <param name="D">The sum of all E[NumClusters].  What the routine seeks to minimize</param>
//        /// <returns>An exit condition representing whether a swap between clusters has taken place</returns>
//        private static ExitCondition Iterate(
//            int Case,
//            int NumCases,
//            int NumVars,
//            int NumClusters,
//            double[,] X,
//            ref int[] Membership,
//            ref double[] CaseCount,
//            ref double[,] S,
//            ref double[] E,
//            ref double D)
//        {
//            int OldMemID = Membership[Case];
//            double OldClusterCaseCount = CaseCount[OldMemID];
//            if (OldClusterCaseCount <= 1.0) return ExitCondition.NOSWAP;

//            int CandidateMemID = -1;
//            double CandidateDist = double.MaxValue;
//            for (int j = 0; j < NumClusters; j++)  //Loops 12 and 13
//            {
//                if (j == OldMemID) continue;
//                double Count = CaseCount[j];
//                double Weight = Count / (Count + 1.0);
//                double Dist = SquaredDist(S, X, Case, j, NumVars) * Weight;
//                if (Dist <= CandidateDist)
//                {
//                    CandidateDist = Dist;
//                    CandidateMemID = j;
//                }
//            }

//            //Check to see how potential new assignment of the case 
//            // compares to the present cluster assignement for the case
//            OldClusterCaseCount = CaseCount[OldMemID];
//            double OldClusterWeight = OldClusterCaseCount / (OldClusterCaseCount - 1.0);
//            double OldClusterF = SquaredDist(S, X, Case, OldMemID, NumVars);
//            double OldClusterDist = OldClusterWeight * OldClusterF;
//            if (CandidateDist >= OldClusterDist)
//            {
//                //Iteration = Iteration + 1;  //No swap took place
//                return ExitCondition.NOSWAP;
//            }

//            //We have a case to move to another cluster, i.e. it gets a 
//            //  new membership ID, and cluster gets another member
//            int NewMemID = CandidateMemID;
//            double NewClusterDist = CandidateDist;
//            double NewClusterCaseCount = CaseCount[NewMemID];

//            //Move the case from old cluster to new one, updating the 
//            //  error values and centroids efficiently
//            E[OldMemID] = E[OldMemID] - OldClusterDist;
//            E[NewMemID] = E[NewMemID] + NewClusterDist;
//            D = D - OldClusterDist + NewClusterDist;
//            double OldClusterShift = 1.0 / (OldClusterCaseCount - 1.0);
//            double NewClusterShift = 1.0 / (NewClusterCaseCount + 1.0);
//            for (int Variable = 0; Variable < NumVars; Variable++)  //Do loop 15
//            {
//                double F = X[Case, Variable];
//                S[OldMemID, Variable] = (OldClusterCaseCount * S[OldMemID, Variable] - F) * OldClusterShift;
//                S[NewMemID, Variable] = (NewClusterCaseCount * S[NewMemID, Variable] + F) * NewClusterShift;
//            }
//            Membership[Case] = NewMemID;
//            CaseCount[OldMemID] = CaseCount[OldMemID] - 1;
//            CaseCount[NewMemID] = CaseCount[NewMemID] + 1;
//            return ExitCondition.SWAP;
//        }

//        /// <summary>
//        /// Dump the cluster membership to the console
//        /// </summary>
//        /// <param name="Membership">The array containing the membership</param>
//        /// <param name="NumCases">The number of cases in the array</param>
//        public static void DumpMembership(int[] Membership, int NumCases)
//        {
//            Console.WriteLine("***************************************");
//            for (int i = 0; i < NumCases; i++) Console.Write((Membership[i] + 1).ToString() + " ");
//            Console.WriteLine();
//        }

//        /// <summary>
//        /// Get a frequency count for each cluster
//        /// </summary>
//        /// <param name="MembershipIndex">The variable containing the cluster indices</param>
//        /// <param name="MemberCount">The frequency listing</param>
//        public static void CountClusterMembers(int[] MembershipIndex, out int[] MemberCount)
//        {
//            int MaxCluster = MembershipIndex.Max();
//            MemberCount = new int[MaxCluster];
//            for (int i = 0; i < MemberCount.Length; i++) MemberCount[i] = 0;
//            for (int i = 0; i < MembershipIndex.Length; i++)
//            {
//                int MemberIndex = MembershipIndex[i] - 1;
//                MemberCount[MemberIndex]++;
//            }
//        }

//        /// <summary>
//        /// Produce an index containing the record numbers for each cluster index
//        /// </summary>
//        /// <param name="MembershipIndex">The integer variable containing the cluster index numbers for each record</param>
//        /// <param name="Index">The index itself of dimension Index[NumClusters][] where the second array contains the record index numbers</param>
//        public static void CollectClusterRecordIndexes(int[] MembershipIndex, out int[][] Index)
//        {
//            int MaxClusIdx = MembershipIndex.Max();
//            int NumRecs = MembershipIndex.Length;

//            //Allocate the indexer as an array of arrays,
//            //permitting different number of cluster members
//            //for each possible cluster index
//            Index = new int[MaxClusIdx][];

//            //Now find out how many cases are in each cluster
//            int[] MemberCount = new int[MaxClusIdx];
//            for (int i = 0; i < MaxClusIdx; i++) MemberCount[i] = 0;
//            for (int i = 0; i < NumRecs; i++)
//            {
//                int ClusIdx = MembershipIndex[i];
//                MemberCount[ClusIdx - 1]++;
//            }

//            //Allocate the right number of cells for each cluster
//            //Next fill the cells for each respective cluster
//            //with the record numbers of their members
//            for (int i = 0; i < MaxClusIdx; i++) Index[i] = new int[MemberCount[i]];
//            int[] CellIdx = new int[MaxClusIdx];
//            for (int i = 0; i < MaxClusIdx; i++) CellIdx[i] = -1;
//            for (int i = 0; i < NumRecs; i++)
//            {
//                int ClusIdx = MembershipIndex[i] - 1;
//                CellIdx[ClusIdx] += 1;
//                Index[ClusIdx][CellIdx[ClusIdx]] = i;
//            }
//        }

        



//        /// <summary>
//        /// Execute the K-means clustering algorithm
//        /// </summary>
//        /// <param name="DumpToConsole">When true, dumps the stepwise output to the console</param>
//        /// <param name="NumCases">Number of cases. designated as M in original FORTRAN program</param>
//        /// <param name="NumVars">Number of variables.  Designated as L in original FORTRAN program</param>
//        /// <param name="NumClusters">Number of clusters desired from the routine.  Designated as N in original FORTRAN program</param>
//        /// <param name="X">Data array, dimensioned [NumCases, NumVars].  In other words, one case data record per row.</param>
//        /// <param name="Membership">Cluster membership.  Initial and final.  Range of values is 1..NumClusters</param>
//        /// <param name="S">The cluster centroids, dimensioned [NumClusters, NumVars]</param>
//        /// <param name="E">The sum of squares of each member to its cluster centroid, dimensioned [NumClusters]</param>
//        /// <param name="D">The sum of all E[NumClusters].  What the routine seeks to minimize</param>
//        public static void Calculate(
//            bool DumpToConsole,
//            int NumCases,
//            int NumVars,
//            int NumClusters,
//            double[,] X,
//            ref int[] Membership,
//            out double[,] S,
//            out double[] E,
//            out double D)
//        {

//            //Dimension and initialize the necessary arrays and other vars
//            //Matrices and vectors are zeroed within their respective constructors
//            S = Utility.TwoDimDouble(NumClusters, NumVars);
//            E = Utility.OneDimDouble(NumClusters); //The sum of squares
//            double[] CaseCount = Utility.OneDimDouble(NumClusters);

//            //Check cluster membership ID for legitimacy
//            //Calculate the number of starting iCases per starting cluster
//            for (int iCase = 0; iCase < NumCases; iCase++) //Loops 1 and 2
//            {
//                /* Logically, a cluster ID in the cells of the Membership array should range from 1 to NumClusters. 
//                 * Indeed, that is what the program expects the input to be.  The problem with that is that it makes 
//                 * programming with zero based arrays messy, and potentially buggy,especially when the original FORTRAN
//                 * code arrays were one-based to begin with.  Hence, in this loop, we adjust all the cluster IDs
//                 * to be zero-based too, i.e. Cluster ID one is reassigned an ID of zero.
//                 * Late in the program in the output phase, we will readjust the IDs in the Membership array back to being
//                 * one based. You will also see constructs in the program such as (ClusterID + 1).ToString() where we need 
//                 * to convert them back for the sake of error messages, console printout, etc.
//                 */
//                Membership[iCase] -= 1;  //Here is where we adjust the ID
//                int ClusterID = Membership[iCase];
//                if ((ClusterID < 0) || (ClusterID >= NumClusters))
//                    throw new Exception("Cluster membership index of " + (ClusterID + 1).ToString() + 
//                                        " in case " + (iCase + 1).ToString() + 
//                                        " is out of correct range 1 to " + NumClusters.ToString());
//                CaseCount[ClusterID] += 1.0;
//            }

//            //Calculate the centroids of the clusters 1:  Do the summation of X
//            for (int iCase = 0; iCase < NumCases; iCase++)  //Loops 3 and 4
//            {
//                int ClusterID = Membership[iCase];
//                for (int Variable = 0; Variable < NumVars; Variable++)
//                    S[ClusterID, Variable] = S[ClusterID, Variable] + X[iCase, Variable];
//            }

//            //Calculate the centroids of the clusters 2:  Do the division to get the mean
//            for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++) //Loops 5 and 6
//            {
//                double Count = CaseCount[ClusterID];
//                if (Count == 0.0)
//                    throw new Exception("Cluster with index of " + (ClusterID + 1).ToString() + " has no cases within it.  Kmeans cannot continue.");
//                for (int Variable = 0; Variable < NumVars; Variable++)
//                    S[ClusterID, Variable] = S[ClusterID, Variable] / Count;
//            }

//            //Accumulate the sum of squared distances for each cluster 
//            for (int iCase = 0; iCase < NumCases; iCase++) //Loops 7 and 8
//            {
//                int ClusMemIdx = Membership[iCase];
//                double F = SquaredDist(S, X, iCase, ClusMemIdx, NumVars);
//                E[ClusMemIdx] = E[ClusMemIdx] + F;
//            }

//            //Loop DO 9.  Calculate total error
//            D = 0.0;
//            for (int ClusterID = 0; ClusterID < NumClusters; ClusterID++)
//                D = D + E[ClusterID];

//            //Begin iteration phase of the routine
//            int Case = -1;
//            int Iteration = 0;
//            while (true)
//            { 
//                Case += 1;
//                if (Case == NumCases) Case = 0;
//                if (Iteration == NumCases) break;
//                ExitCondition ExitCon = Iterate(Case, NumCases, NumVars, NumClusters, X, ref Membership, ref CaseCount, ref S, ref E, ref D);
//                if (ExitCon == ExitCondition.SWAP)
//                {
//                    Iteration = 0;
//                    if (DumpToConsole) DumpMembership(Membership, NumCases);
//                }
//                else
//                    Iteration += 1;
//            }

//            //Return cluster member numbers to original indexes
//            for (int i = 0; i < NumCases; i++) 
//                Membership[i] += 1;
//        }

//    }
//}










