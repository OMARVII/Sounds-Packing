using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Sounds_Packing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public bool worstfit = false;
        static public bool worstfitdec = false;
        static public bool firstfit = false;
        static public bool firstfitdec = false;
        static public bool bestfit = false;
        static public bool folderfilling = false;
        static public bool worstfitpq = false;
        static public bool worstfitdecpq = false;
        static public bool multitheading = false;
        static public string infoPath;

        static public List<int> secList = new List<int>();
        static public int[] secListarray;
        List<string> l = new List<string>();
        static string sourcePath;
        static string targetPath;
        static int maxSec;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                infoPath = openFileDialog.FileName;

            }
        }
        public void ReadAudioFileInfo()
        {
            FileStream FS = new FileStream(infoPath, FileMode.Open);
            StreamReader file = new StreamReader(FS);

            while (file.Peek() != -1)
            {
                string readline = file.ReadLine();
                l.Add(readline);
            }

            FS.Close();
            file.Close();
            for (int i = 1; i < l.Count; i++)
            {
                for (int j = 0; j < l[i].Length; j++)
                {
                    if (l[i][j] == ' ' && j > 3)
                    {
                        string final = l[i].Substring(j + 1);
                        double seconds = TimeSpan.Parse(final).TotalSeconds;
                        secList.Add((int)seconds);
                        break;
                    }
                }

            }
            secListarray = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)
            {
                secListarray[i] = secList[i];
            }
        }
        public static int folder_counter = 0;
        static void Allocatingfiles(int[] allocation)
        {
            folder_counter = 0;
            for (int i = 0; i < allocation.Length; i++)
            {
                int num = i + 1;
                string fileName = num.ToString() + ".amr";
                string sPath = sourcePath;
                string tPath = targetPath;
                tPath += allocation[i] + 1;
                if (!System.IO.Directory.Exists(tPath))
                {
                    folder_counter++;
                    System.IO.Directory.CreateDirectory(tPath);
                }

                string sourceFile = System.IO.Path.Combine(sPath, fileName);
                string destFile = System.IO.Path.Combine(tPath, fileName);
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            metaData(allocation);
        }
        private void btnOpendirsource_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sourcePath = fbd.SelectedPath;
            }
        }
        private void btnOpendirtarget_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                targetPath = fbd.SelectedPath;
                targetPath += targetPath[2];
            }
            if (System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.Delete(targetPath,true);
            }
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            maxSec = int.Parse(secBox.Text);
            ReadAudioFileInfo();

            if (worstfit == true)
            {
                if (Priority_Queue.IsChecked == true)
                { 
                    Thread w = new Thread(delegate () { WorstFitPQ(); });
                    w.Start();

                }
                else
                {

                    Thread w = new Thread(delegate () { WorstFit(); });
                    w.Start();

                }
            }
            else if (firstfitdec == true)
            {
                if (multithreading.IsChecked == true)
                {
                    multitheading = true;
                }

                Thread f = new Thread(delegate () { firstFitDec(); });
                f.Start();

                firstfitdec = false;
            }
            else if (worstfitdec == true)
            {
                if (Priority_Queue.IsChecked == true)
                {

                    Thread w = new Thread(delegate () { WorstFitDecPQ(); });
                    w.Start();

                }
                else
                {
                    Thread w = new Thread(delegate () { WorstFitDec(); });
                    w.Start();
                    worstfitdec = false;
                }
            }
            else if (firstfit == true)
            {

                Thread ff = new Thread(delegate () { firstFit(); });
                ff.Start();

                firstfit = false;
            }
            else if (folderfilling == true)
            {
                
                Thread fff = new Thread(delegate () { folderFilling(); });
                fff.Start();
                folderfilling = false;
            }
            else if (bestfit == true)
            {
                Thread b = new Thread(delegate () { BestFit(); });
                b.Start();
                bestfit = false;
            }

        }
        static public List<List<int>> ll = new List<List<int>>();
        static public void metaData(int[] allocation)
        {


            //  ll[0].Add(0);
             System.Windows.MessageBox.Show(folder_counter.ToString()+ " Folder");
            for (int i = 1; i <= folder_counter; i++)
            {
                ll.Add(new List<int>());
                for (int j = 0; j < allocation.Count(); j++)
                {
                    if (allocation[j] == i - 1)
                    {
                        ll[i - 1].Add(j);
                    }
                }
            }
            // System.Windows.MessageBox.Show(ll.Count.ToString());
            for (int i = 0; i < ll.Count(); i++)
            {
                FileStream FS = new FileStream(targetPath + @"\F" + i + 1.ToString() + ".txt", FileMode.Append);
                StreamWriter file = new StreamWriter(FS);
                file.WriteLine(ll[i].Count().ToString());
                int sum = 0;
                for (int j = 0; j < ll[i].Count(); j++)
                {

                    file.WriteLine(secList[j].ToString());
                    sum += secList[j];
                }
                // sum.ToString();
                file.WriteLine(sum.ToString());
                file.Close();
                FS.Close();
            }
        }
        static void WorstFit()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<int> Folders = new List<int>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)               // Complexity=O(n) Where n=number of files
            {
                allocation[i] = -1;                               //initialize all files by -1 as they are not allocated  
            }
            for (int i = 0; i < secList.Count; i++)               // Complexity=O(n) Where n=number of files
            {
                int wstIdx = -1;                                 //initialize all files by -1 as they are not allocated  
                for (int j = 0; j < Folders.Count; j++)          // Complexity=O(m) Where m=number of folders 
                {
                    if (Folders[j] >= secList[i])                //Compare folder size by file size to find empty folder   
                    {
                        if (wstIdx == -1)                       //Compare if still not allocated in file
                            wstIdx = j;                         // Let worstindex take the index of the  folder found
                        else if (Folders[wstIdx] <= Folders[j]) //Find if there is a larger folder to allocate in 
                            wstIdx = j;                         //Let worstindex take the index of folder found    
                    }

                }
                if (wstIdx != -1)                              // There is a folder found to allocate in 
                {
                    allocation[i] = wstIdx;                   //Alocate file i to worstindex
                    Folders[wstIdx] -= secList[i];            //Subtract the file insert from the folder and the result is the size remaining in the folder 
                }
                else if (wstIdx == -1)                        //There isn't a folder found to allocate in 
                {
                    Folders.Add(maxSec);                       //Create new folder with max size in list
                    Folders[Folders.Count - 1] -= secList[i];  //Alocate file i to worstindex
                    allocation[i] = Folders.Count - 1;          //Subtract the file insert from the folder and the result is the size remaining in the folder 
                }

            }//Total Complexity= O(n*m) Where n=number of files*m=number of folders 
            sw.Stop();
            System.Windows.MessageBox.Show("Worst Fit takes "+ sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
        }
        static void WorstFitPQ()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int folderindex = 0;
            PriorityQueue<Tuple<int, int>> Folders = new PriorityQueue<Tuple<int, int>>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)         //Complexity = O(n) Where n=number of files
            {
                allocation[i] = -1;                         //Initialize all files by -1 as they are not allocated
            }

            for (int i = 0; i < secList.Count; i++)         // Complexity=O(n) Where n=number of files
            {
                if (Folders.Count > 0)                       //Check if there is elemenets in the queue or not
                {
                    Tuple<int, int> tempp = Folders.Peek();  //Make pair of size of folder and index and intializing by top element in priority queue
                    if (tempp.Item1 > secList[i])             //Compare folder size by file size to find empty folder   
                    {
                        var top = Folders.list[0];            // Initalize top by first intem in the folder
                        Folders.Dequeue();                    //Remove the top item in the queue
                        int rem = top.Item1, ind = top.Item2;  // Initalize the rem by the top file size and  ind by the top index 
                        rem -= secList[i];                      //Subtract the file insert from rem and the result is the size remaining in the folder 
                        Tuple<int, int> after = new Tuple<int, int>(rem, ind);//Initalize after as pair of folder and index by pair of rem and ind
                        Folders.Enqueue(after);                 //Insert after in queue
                        allocation[i] = ind;                     //Alocate file i by ind

                    }
                }

                if (allocation[i] == -1)                      //There isn't a folder found to allocate in 
                {
                    int rem = maxSec - secList[i], ind = folderindex++;
                    Tuple<int, int> tmp = new Tuple<int, int>(rem, ind);  //Initalize temp as pair of folder and index by pair of rem and ind
                    Folders.Enqueue(tmp);
                    allocation[i] = ind;
                }

            }

            sw.Stop();
            System.Windows.MessageBox.Show("Worst Fit PQ takes "+ sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
        }
        static void WorstFitDec()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (multitheading == true)
            {
                mergeSort.MergeSort(secListarray);
            }
            else
            {
                mergeSort.MergeSortWithout(secListarray);
            }
            multitheading = false;
            //Reverse after sorting to have the file sorded in a decreasing order
            List<int> Folders = new List<int>();             //Complexity = O(1)
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)         // Complexity=O(n) Where n=number of files
            {
                allocation[i] = -1;                         //initialize all files by -1 as they are not allocated  
            }
            for (int i = 0; i < secList.Count; i++)         // Complexity=O(n) Where n=number of files
            {
                int wstIdx = -1;                            //intalize worstindex first by -1 as there is still no index found

                for (int j = 0; j < Folders.Count; j++)    // Complexity=O(m) Where m=number of folders 
                {
                    if (Folders[j] >= secListarray[i])          //Compare folder size by file size to find empty folder   
                    {
                        if (wstIdx == -1)                //Compare if still not allocated in file
                            wstIdx = j;                   // Let worstindex take the index of the  folder found
                        else if (Folders[wstIdx] <= Folders[j])  //Find if there is a larger folder to allocate in
                            wstIdx = j;                   //Let worstindex take the index of folder found       
                    }

                }
                if (wstIdx != -1)                   // There is a folder found to allocate in 
                {
                    allocation[i] = wstIdx;         //Alocate file i to worstindex
                    Folders[wstIdx] -= secListarray[i];  //Subtract the file insert from the folder and the result is the size remaining in the folder 
                }
                else if (wstIdx == -1)             //There isn't a folder found to allocate in 
                {
                    Folders.Add(maxSec);           //Create new folder with max size in list
                    Folders[Folders.Count - 1] -= secListarray[i];  //Alocate file i to worstindex
                    allocation[i] = Folders.Count - 1;          //Subtract the file insert from the folder and the result is the size remaining in the folder 
                }

            }//Total Complexity= O(n*m) Where n=number of files*m=number of folders 

            sw.Stop();
            System.Windows.MessageBox.Show("Worst Fit dec takes "+ sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
        }
        static void WorstFitDecPQ()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (multitheading == true)
            {
                mergeSort.MergeSort(secListarray);
            }
            else
            {
                mergeSort.MergeSortWithout(secListarray);
            }
            multitheading = false;
            int folderindex = 0;
            PriorityQueue<Tuple<int, int>> Folders = new PriorityQueue<Tuple<int, int>>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)    //Complexity = O(n) Where n=number of files
            {
                allocation[i] = -1;                    //Initialize all files by -1 as they are not allocated  
            }

            for (int i = 0; i < secList.Count; i++)   // Complexity=O(n) Where n=number of files
            {
                if (Folders.Count > 0)              //Check if there is elemenets in the queue or not
                {
                    Tuple<int, int> tempp = Folders.Peek(); //Make pair of size of folder and index and intializing by top element in priority queue
                    if (tempp.Item1 > secListarray[i])            //Compare folder size by file size to find empty folder   
                    {
                        var top = Folders.list[0];         // Initalize top by first intem in the folder
                        Folders.Dequeue();                 //Remove the top item in the queue
                        int rem = top.Item1, ind = top.Item2;// Initalize the rem by the top file size and  ind by the top index 
                        rem -= secListarray[i];                     //Subtract the file insert from rem and the result is the size remaining in the folder 
                        Tuple<int, int> after = new Tuple<int, int>(rem, ind); //Initalize after as pair of folder and index by pair of rem and ind
                        Folders.Enqueue(after);                //Insert after in queue
                        allocation[i] = ind;                   //Alocate file i by ind

                    }
                }

                if (allocation[i] == -1)            //There isn't a folder found to allocate in 
                {
                    int rem = maxSec - secListarray[i], ind = folderindex++;
                    Tuple<int, int> tmp = new Tuple<int, int>(rem, ind);  //Initalize temp as pair of folder and index by pair of rem and ind
                    Folders.Enqueue(tmp);  //Insert temp in queue
                    allocation[i] = ind;   //Alocate file i by ind
                }

            }
            sw.Stop();
            System.Windows.MessageBox.Show("Worst Fit dec PQ takes "+ sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
            //Total Complexity=O(n)  Where n=number of files
        }
        static void firstFit()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<int> Folders = new List<int>();      //folder list, complexity: O(1) 
            int[] allocation = new int[secListarray.Length]; //array with the number of files, complexity: O(1) 
            for (int i = 0; i < secList.Count; i++)     //complexity: O(n), (n -> secList.Count)
            {
                allocation[i] = -1;                 //intialize all files by -1 as the file is still not allocated
            }
            for (int i = 0; i < secList.Count; i++)  //complexity: O(n), (n -> secList.Count)
            {
                int fstIdx = -1;                 // Complexity: O(1)   
                                                 // Initialize the variable of fisrt index as not found

                for (int j = 0; j < Folders.Count; j++)//complexity: O(m), (m -> Folders.Count)
                {
                    if (Folders[j] >= secList[i]) // Complexity: O(1)
                    {                             //Compare between Folders and Files to find the fisrt suitable Folder


                        fstIdx = j;               // Complexity:O(1)
                                                  //put the current folder in the fisrt fit index
                        break;
                    }

                }
                if (fstIdx != -1)                 // Compexity: O(1)
                                                  // If there is a suitable place
                {
                    allocation[i] = fstIdx;                  //Complexity: O(1)
                                                             // assign folder i to be put in fstIdx folder

                    Folders[fstIdx] -= secList[i];           // Complexity: O(1)
                                                             //Reduce memory of the files 
                }
                else if (fstIdx == -1)             // Complexity :O(1)
                                                   //If there is not a place in folders
                {
                    Folders.Add(maxSec);           //Create new folder to add the file
                    Folders[Folders.Count - 1] -= secList[i];
                    allocation[i] = Folders.Count - 1;
                }

            }

            sw.Stop();
            System.Windows.MessageBox.Show("First Fit takes "+sw.ElapsedMilliseconds.ToString());
            //Total Complexity of First-Fit : O(N*M)
            Allocatingfiles(allocation);
        }
        static void firstFitDec()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (multitheading == true)
            {
                mergeSort.MergeSort(secListarray);
            }
            else
            {
                mergeSort.MergeSortWithout(secListarray);
            }
            multitheading = false; mergeSort.MergeSort(secListarray);

            List<int> Folders = new List<int>();      //folder list, complexity: O(1) 
            int[] allocation = new int[secListarray.Length]; //array with the number of files, complexity: O(1) 
            for (int i = 0; i < secList.Count; i++)     //complexity: O(n), (n -> secList.Count)
            {
                allocation[i] = -1;                 //intialize all files by -1 as the file is still not allocated
            }
            for (int i = 0; i < secList.Count; i++)  //complexity: O(n), (n -> secList.Count)
            {
                int fstIdx = -1;                 // Complexity: O(1)   
                                                 // Initialize the variable of fisrt index as not found

                for (int j = 0; j < Folders.Count; j++)//complexity: O(m), (m -> Folders.Count)
                {
                    if (Folders[j] >= secListarray[i]) // Complexity: O(1)
                    {                             //Compare between Folders and Files to find the fisrt suitable Folder


                        fstIdx = j;               // Complexity:O(1)
                                                  //put the current folder in the fisrt fit index
                        break;
                    }

                }
                if (fstIdx != -1)                 // Compexity: O(1)
                                                  // If there is a suitable place
                {
                    allocation[i] = fstIdx;                  //Complexity: O(1)
                                                             // assign folder i to be put in fstIdx folder

                    Folders[fstIdx] -= secList[i];           // Complexity: O(1)
                                                             //Reduce memory of the files 
                }
                else if (fstIdx == -1)             // Complexity :O(1)
                                                   //If there is not a place in folders
                {
                    Folders.Add(maxSec);           //Create new folder to add the file
                    Folders[Folders.Count - 1] -= secList[i];
                    allocation[i] = Folders.Count - 1;
                }

            }

            //Total Complexity of First-Fit : O(N*M)

            sw.Stop();
            System.Windows.MessageBox.Show("first Fit dec takes " + sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
        }
        static void BestFit()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int[] allocation = new int[secList.Count];// Complexity is O(1), Array with number of files
            List<int> Folders = new List<int>();       //Complexity is O(1), Folder list


            for (int i = 0; i < secList.Count; i++) //O(N)  as N is the number of the files
            {

                allocation[i] = -1;                 //O(1), Initialize all files by -1 as they are not allocated
            }


            for (int i = 0; i < secList.Count; i++)   // Find the minimum folder to put the file 
                                                      // Complexity of the 2 loops is O(N*M)
            {                                        //Complexity of Outer loop is O(N) as N is the number of files

                int bstIdx = -1;                     //  O(1) 
                                                     //initialize the variable of bestIndex as not found (there is not a place in folders yet)

                for (int j = 0; j < Folders.Count; j++)  //Complexity of the inner loop is O(M) as M is the number of Folders
                {
                    if (Folders[j] >= secList[i]) //O(1),Compare File size and folder size to find the best folder
                    {
                        if (bstIdx == -1)         //O(1) if the bestindex still -1 so there isn't a suitable folder for it
                            bstIdx = j;           //O(1), put the  current folder in the bestfit index

                        else if (Folders[bstIdx] > Folders[j]) // O(1),if the folder of best index is larger than the current suitable folder 
                            bstIdx = j;                        //O(1) Change bestfit index to the currrent Folder
                    }

                }

                if (bstIdx != -1)                      //O(1),If the there is a suitable place
                {

                    allocation[i] = bstIdx;            //O(1),allocate process to the folder

                    Folders[bstIdx] -= secList[i];     //O(1),Reduce memory of the files
                }
                else if (allocation[i] == -1)         // O(1),if there is not a place in folders 
                {

                    int toadd = maxSec - secList[i];     //Create new Folder to add files in it
                    Folders.Add(toadd);
                    allocation[i] = Folders.Count - 1;
                }

            }
            // Total Complexity of the Best-Fit = O(N*M)

            sw.Stop();
            System.Windows.MessageBox.Show("Best Fit takes "+ sw.ElapsedMilliseconds.ToString());
            Allocatingfiles(allocation);
        }
        static List<int> FolderFilling(ref List<Tuple<int, int>> v, ref List<Tuple<int, int>> w, int n, int W, int[,] V, bool[,] keep)
        {

            List<int> l = new List<int>();

            for (int a = 0; a <= W; a++)  ///////////////////////////////////////
            {                             //initializing rows and colomns of 
                V[0, a] = 0;              //the table : row 0 and colomn 0 
            }                             // with zeros to start biulding the table 
            for (int i = 0; i < n; i++)   // Complexity : Loop 1: o(W)
            {                             //            : Loop 2: o(N)
                V[i, 0] = 0;              //
            }                             /////////////////////////////////////////

            for (int i = 1; i <= n; i++)                                                            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            {                                                                                       // building the table of results to get :
                for (int j = 0; j <= W; j++)                                                        // files that have combined size at most W ---> List 1(v)
                {                                                                                   //The total computing time of the stored files is as large as possible ---> List 2(w)
                    if ((w[i].Item1 <= j) && (v[i].Item1 + V[i - 1, j - w[i].Item1]) > V[i - 1, j]) // Similar as the knapsack logic 
                    {                                                                               //We construct an array V[N][W]..For 1<=i>=N and 0<=w>=W , the entry V[i][w] will store the maximum(combined)
                                                                                                    //computing time of any subset of files of(combined) size at most w .
                        V[i, j] = v[i].Item1 + V[i - 1, j - w[i].Item1];    //Option 1:             //the array entry V[N][W] :
                                                                            // will contain the maximum computing time of files that can fit into the storage which is the solution 
                        keep[i, j] = true;                                 //File Taken             // equation used to build table : V[i, j] = v[i].Item1 + V[i - 1, j - w[i].Item1]
                    }                                                                               ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    else
                    {                                                                               ///////////////////////////////////
                        V[i, j] = V[i - 1, j];// Option 2                                           // boolen array to mark taken files
                        keep[i, j] = false;  // File Not Taken                                      ///////////////////////////////////
                    }
                }

            }
            int K = W;
            for (int k = n; k >= 1; k--)                                 ////////////////////////////////////////////
            {                                                            // loop on boolen array to get taken files 
                if (keep[k, K] == true)                                  //Plce taken files in the list to be returned 
                {                                                        // remove the taken files from the lists
                    l.Add(w[k].Item2);                                   ////////////////////////////////////////////////
                    //Console.WriteLine(w[k].Item2.ToString());
                    K = K - w[k].Item1;
                    w.Remove(w[k]);
                    v.Remove(v[k]);
                }

            }
            return l;   //////////////////////////////////
                        // List Containing the Taken files 
                        //////////////////////////////////
        }
        /// <Complexity>
        /// O(N*W)  where W=N ----> O(N^2)
        /// </Complexity>
        static void folderFilling()
        {
            bool[,] keep = new bool[secList.Count + 1, secList.Count + 1];
            int[,] V = new int[secList.Count + 1, secList.Count + 1];
            List<Tuple<int, int>> w = new List<Tuple<int, int>>();
            List<int> indexes = new List<int>();
            w.Add(new Tuple<int, int>(0, 0));
            for (int i = 0; i < secList.Count(); i++)
            {
                w.Add(new Tuple<int, int>(secList[i], i + 1));
            }
            List<Tuple<int, int>> v = new List<Tuple<int, int>>();
            //new Tuple<int, int>(0, 0);
            v.Add(new Tuple<int, int>(0, 0));

            for (int i = 0; i < secList.Count(); i++)
            {

                v.Add(new Tuple<int, int>(secList[i], i + 1));
            }
            int countt = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (v.Count() > 1)
            {
                Console.WriteLine("file " + countt.ToString() + ' ');
                List<int> folder_filling_Allocation = FolderFilling(ref v, ref w, v.Count() - 1, maxSec, V, keep);
                string sPath = sourcePath;
                string tPath = targetPath;
                tPath += countt.ToString();
                if (!System.IO.Directory.Exists(tPath))
                {
                    System.IO.Directory.CreateDirectory(tPath);
                }
                for (int i = 0; i < folder_filling_Allocation.Count(); i++)
                {

                    string fileName = folder_filling_Allocation[i].ToString() + ".amr";
                    Console.WriteLine(folder_filling_Allocation[i].ToString());
                    string sourceFile = System.IO.Path.Combine(sPath, fileName);
                    string destFile = System.IO.Path.Combine(tPath, fileName);
                    System.IO.File.Copy(sourceFile, destFile, true);

                }

                sw.Stop();
                System.Windows.MessageBox.Show("folderfilling takes ", sw.ElapsedMilliseconds.ToString());
                FileStream FS = new FileStream(targetPath + @"\F" + countt + 1.ToString() + ".txt", FileMode.Append);
                StreamWriter file = new StreamWriter(FS);
                file.WriteLine(folder_filling_Allocation.Count().ToString());
                int sum = 0;
                for (int i = 0; i < folder_filling_Allocation.Count(); i++)
                {
                    
                    {

                        file.WriteLine(secList[i].ToString());
                        sum += secList[i];
                    }
                    // sum.ToString();
                   

                }
                file.WriteLine(sum.ToString());
                countt++;
                file.Close();
                FS.Close();
            }
        }
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            worstfitdec = true;
        }
        private void secBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            firstfit = true;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            bestfit = true;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            worstfit = true;
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            firstfitdec = true;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            folderfilling = true;
        }
        private void Priority_Queue_Checked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}