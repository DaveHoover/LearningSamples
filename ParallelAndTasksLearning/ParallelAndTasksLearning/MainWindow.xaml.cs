using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.IO;
using System.Net;


namespace ParallelAndTasksLearning
{

    public class UrlAndSize
    {
        public string Url { get; set; }
        public int Size { get; set; }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Declare a System.Threading.CancellationTokenSource.
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnClickEx1(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            List<string> summary = await ParallelExamples.ParallelStart();
            resultsTextBox.Clear();
            StringBuilder b = new StringBuilder();
            foreach ( string s in summary)
            {
                b.Append(s + "\r\n");
            }
            resultsTextBox.Text = b.ToString();
            this.Cursor = Cursors.Arrow;
        }

        #region Msdn Sample 1
        private async void OnClickMsdnSample1(object sender, RoutedEventArgs e)
        {
            // Call and await separately. 
            //Task<int> getLengthTask = AccessTheWebAsync(); 
            //// You can do independent work here. 
            //int contentLength = await getLengthTask; 

            int contentLength = await AccessTheWebAsync();

            resultsTextBox.Text +=
                String.Format("\r\nLength of the downloaded string: {0}.\r\n", contentLength);

        }
        // Three things to note in the signature: 
        //  - The method has an async modifier.  
        //  - The return type is Task or Task<T>. (See "Return Types" section.)
        //    Here, it is Task<int> because the return statement returns an integer. 
        //  - The method name ends in "Async."
        async Task<int> AccessTheWebAsync()
        {
            // You need to add a reference to System.Net.Http to declare client.
            HttpClient client = new HttpClient();

            // GetStringAsync returns a Task<string>. That means that when you await the 
            // task you'll get a string (urlContents).
            Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");

            // You can do work here that doesn't rely on the string from GetStringAsync.
            DoIndependentWork();

            // The await operator suspends AccessTheWebAsync. 
            //  - AccessTheWebAsync can't continue until getStringTask is complete. 
            //  - Meanwhile, control returns to the caller of AccessTheWebAsync. 
            //  - Control resumes here when getStringTask is complete.  
            //  - The await operator then retrieves the string result from getStringTask. 
            string urlContents = await getStringTask;

            // The return statement specifies an integer result. 
            // Any methods that are awaiting AccessTheWebAsync retrieve the length value. 
            return urlContents.Length;
        }

        void DoIndependentWork()
        {
            resultsTextBox.Text += "Working . . . . . . .\r\n";
        }

        #endregion

        #region Msdn Sample 2
        private async void OnClickMsdnSample2(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();

            // Disable the button until the operation is complete.
            buttonMsdnSample2.IsEnabled = false;

            // One-step async call.
            await SumPageSizesAsync();

            //// Two-step async call. 
            //Task sumTask = SumPageSizesAsync(); 
            //await sumTask;

            resultsTextBox.Text += "\r\nControl returned to startButton_Click.\r\n";

            // Reenable the button in case you want to run the operation again.
            buttonMsdnSample2.IsEnabled = true;

        }


        private async Task SumPageSizesAsync()
        {
            // Declare an HttpClient object and increase the buffer size. The 
            // default buffer size is 65,536.
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var total = 0;

            foreach (var url in urlList)
            {
                // GetByteArrayAsync returns a task. At completion, the task 
                // produces a byte array. 
                byte[] urlContents = await client.GetByteArrayAsync(url);

                // The following two lines can replace the previous assignment statement. 
                //Task<byte[]> getContentsTask = client.GetByteArrayAsync(url); 
                //byte[] urlContents = await getContentsTask;

                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the websites.
            resultsTextBox.Text +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }


        private List<string> SetUpURLList()
        {
            List<string> urls = new List<string> 
            { 
                "http://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                "http://msdn.microsoft.com",
                "http://msdn.microsoft.com/en-us/library/hh290136.aspx",
                "http://msdn.microsoft.com/en-us/library/ee256749.aspx",
                "http://msdn.microsoft.com/en-us/library/hh290138.aspx",
                "http://msdn.microsoft.com/en-us/library/hh290140.aspx",
                "http://msdn.microsoft.com/en-us/library/dd470362.aspx",
                "http://msdn.microsoft.com/en-us/library/aa578028.aspx",
                "http://msdn.microsoft.com/en-us/library/ms404677.aspx",
                "http://msdn.microsoft.com/en-us/library/ff730837.aspx"
            };
            return urls;
        }


        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format  
            // is designed to be used with a monospaced font, such as 
            // Lucida Console or Global Monospace. 
            var bytes = content.Length;
            // Strip off the "http://".
            var displayURL = url.Replace("http://", "");
            resultsTextBox.Text += string.Format("\n{0,-58} {1,8}", displayURL, bytes);
        }


        #endregion

        #region MsdnSample3

        private async void OnClickMsdnSample3(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();

            // Two-step async call.
            Task sumTask = SumPageSizesAsync1();
            await sumTask;

            // One-step async call. 
            //await SumPageSizesAsync();

            resultsTextBox.Text += "\r\nControl returned to startButton_Click.\r\n";

        }

        private async Task SumPageSizesAsync1()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // Create a query. 
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in urlList select ProcessURLAsync(url);

            // Use ToArray to execute the query and start the download tasks.
            Debug.WriteLine("Ready to execute downloadTasks = downloadTasksQuery.ToArray()");
            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();
            Debug.WriteLine("Ready to execute Task.WhenAll(downloadTasks);");
            // You can do other work here before awaiting. 

            // Await the completion of all the running tasks. 
            int[] lengths = await Task.WhenAll(downloadTasks);

            Debug.WriteLine("All tasks completed");
            //// The previous line is equivalent to the following two statements. 
            //Task<int[]> whenAllTask = Task.WhenAll(downloadTasks); 
            //int[] lengths = await whenAllTask; 

            int total = lengths.Sum();

            //var total = 0; 
            //foreach (var url in urlList) 
            //{ 
            //    byte[] urlContents = await GetURLContentsAsync(url); 

            //    // The previous line abbreviates the following two assignment statements. 
            //    // GetURLContentsAsync returns a Task<T>. At completion, the task 
            //    // produces a byte array. 
            //    //Task<byte[]> getContentsTask = GetURLContentsAsync(url); 
            //    //byte[] urlContents = await getContentsTask; 

            //    DisplayResults(url, urlContents); 

            //    // Update the total.           
            //    total += urlContents.Length; 
            //} 

            // Display the total count for all of the websites.
            resultsTextBox.Text +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }

        // The actions from the foreach loop are moved to this async method. 
        private async Task<int> ProcessURLAsync(string url)
        {
            Debug.WriteLine("ProcessURLAsync Start for URL " + url);
            var byteArray = await GetURLContentsAsync(url);
            DisplayResults(url, byteArray);
            Debug.WriteLine("ProcessURLAsync Complete for URL " + url);
            return byteArray.Length;
        }


        private async Task<byte[]> GetURLContentsAsync(string url)
        {
            // The downloaded resource ends up in the variable named content. 
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL. 
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for 
            // the response. 
            using (WebResponse response = await webReq.GetResponseAsync())
            {
                // Get the data stream that is associated with the specified url. 
                using (Stream responseStream = response.GetResponseStream())
                {
                    await responseStream.CopyToAsync(content);
                }
            }

            // Return the result as a byte array. 
            return content.ToArray();
        }

        #endregion

        #region Msdn Sample 4
        private async void OnClickMsdnSample4(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();
            await CreateMultipleTasksAsync();
            resultsTextBox.Text += "\r\n\r\nControl returned to startButton_Click.\r\n";

        }

        async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            Debug.WriteLine("ProcessURLAsync Start URL = " + url);
            var byteArray = await client.GetByteArrayAsync(url);
            DisplayResults(url, byteArray);
            Debug.WriteLine("ProcessURLAsync Complete URL = " + url);
            return byteArray.Length;
        }

        private async Task CreateMultipleTasksAsync()
        {
            // Declare an HttpClient object, and increase the buffer size. The 
            // default buffer size is 65,536.
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

            // Create and start the tasks. As each task finishes, DisplayResults  
            // displays its length.
            Task<int> download1 =
                ProcessURLAsync("http://msdn.microsoft.com", client);
            Task<int> download2 =
                ProcessURLAsync("http://msdn.microsoft.com/en-us/library/hh156528(VS.110).aspx", client);
            Task<int> download3 =
                ProcessURLAsync("http://msdn.microsoft.com/en-us/library/67w7t67f.aspx", client);
            Debug.WriteLine("CreateMultipleTasksAsync: 3 Tasks created. Now will await ...");
            // Await each task. 
            int length1 = await download1;
            int length2 = await download2;
            int length3 = await download3;

            int total = length1 + length2 + length3;

            // Display the total count for the downloaded websites.
            resultsTextBox.Text +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }

        #endregion

        #region TryCatchExample

        private async void OnClickMsdnTryCatch(object sender, RoutedEventArgs e)
        {
            Task<string> theTask = DelayAsync();

            try
            {
                string result = await theTask;
                Debug.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception Message: " + ex.Message);
            }
            Debug.WriteLine("Task IsCanceled: " + theTask.IsCanceled);
            Debug.WriteLine("Task IsFaulted:  " + theTask.IsFaulted);
            if (theTask.Exception != null)
            {
                Debug.WriteLine("Task Exception Message: "
                    + theTask.Exception.Message);
                Debug.WriteLine("Task Inner Exception Message: "
                    + theTask.Exception.InnerException.Message);
            }

            Task theTask1 = ExcAsync(info: "First Task");
            Task theTask2 = ExcAsync(info: "Second Task");
            Task theTask3 = ExcAsync(info: "Third Task");

            Task allTasks = Task.WhenAll(theTask1, theTask2, theTask3);

            try
            {
                await allTasks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                Debug.WriteLine("Task IsFaulted: " + allTasks.IsFaulted);
                foreach (var inEx in allTasks.Exception.InnerExceptions)
                {
                    Debug.WriteLine("Task Inner Exception: " + inEx.Message);
                }
            }
        }

        private async Task<string> DelayAsync()
        {
            await Task.Delay(100);

            // Uncomment each of the following lines to 
            // demonstrate exception handling. 

            //throw new OperationCanceledException("canceled");
            //throw new Exception("Something happened.");
            return "Done";
        }

        private async Task ExcAsync(string info)
        {
            await Task.Delay(100);

            throw new Exception("Error-" + info);
        }


        #endregion

        #region MsdnReturnTypes

        private async void OnClickMsdnReturnTypes(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();

            // Start the process and await its completion. DriverAsync is a  
            // Task-returning async method.
            await DriverAsync();

            // Say goodbye.
            resultsTextBox.Text += "\r\nAll done, exiting button-click event handler.";

        }

        async Task DriverAsync()
        {
            // Task<T>  
            // Call and await the Task<T>-returning async method in the same statement. 
            int result1 = await TaskOfT_MethodAsync();

            // Call and await in separate statements.
            Task<int> integerTask = TaskOfT_MethodAsync();

            // You can do other work that does not rely on integerTask before awaiting.
            resultsTextBox.Text += String.Format("Application can continue working while the Task<T> runs. . . . \r\n");

            int result2 = await integerTask;

            // Display the values of the result1 variable, the result2 variable, and 
            // the integerTask.Result property.
            resultsTextBox.Text += String.Format("\r\nValue of result1 variable:   {0}\r\n", result1);
            resultsTextBox.Text += String.Format("Value of result2 variable:   {0}\r\n", result2);
            resultsTextBox.Text += String.Format("Value of integerTask.Result: {0}\r\n", integerTask.Result);

            // Task 
            // Call and await the Task-returning async method in the same statement.
            await Task_MethodAsync();

            // Call and await in separate statements.
            Task simpleTask = Task_MethodAsync();

            // You can do other work that does not rely on simpleTask before awaiting.
            resultsTextBox.Text += String.Format("\r\nApplication can continue working while the Task runs. . . .\r\n");

            await simpleTask;
        }

        // TASK<T> EXAMPLE
        async Task<int> TaskOfT_MethodAsync()
        {
            // The body of the method is expected to contain an awaited asynchronous 
            // call. 
            // Task.FromResult is a placeholder for actual work that returns a string. 
            var today = await Task.FromResult<string>(DateTime.Now.DayOfWeek.ToString());

            // The method then can process the result in some way. 
            int leisureHours;
            if (today.First() == 'S')
                leisureHours = 16;
            else
                leisureHours = 5;

            // Because the return statement specifies an operand of type int, the 
            // method must have a return type of Task<int>. 
            return leisureHours;
        }


        // TASK EXAMPLE
        async Task Task_MethodAsync()
        {
            // The body of an async method is expected to contain an awaited  
            // asynchronous call. 
            // Task.Delay is a placeholder for actual work.
            await Task.Delay(2000);
            // Task.Delay delays the following line by two seconds.
            resultsTextBox.Text += String.Format("\r\nSorry for the delay. . . .\r\n");

            // This method has no return statement, so its return type is Task.  
        }

        #endregion

        #region MsdnSimpleflow

        private async void OnClickMsdnSimpleFlow(object sender, RoutedEventArgs e)
        {
            // ONE
            Debug.WriteLine("Step 1");
            Task<int> getLengthTask = AccessTheWebAsyncSimpleFlow();

            Debug.WriteLine("Step 4");
            // FOUR
            int contentLength = await getLengthTask;
            Debug.WriteLine("Step 6");
            // SIX
            resultsTextBox.Text +=
                String.Format("\r\nLength of the downloaded string: {0}.\r\n", contentLength);

        }

        async Task<int> AccessTheWebAsyncSimpleFlow()
        {

            // TWO
            Debug.WriteLine("Step 2");
            HttpClient client = new HttpClient();
            Task<string> getStringTask =
                client.GetStringAsync("http://msdn.microsoft.com");

            // THREE                 
            Debug.WriteLine("Step 3");
            string urlContents = await getStringTask;

            // FIVE
            Debug.WriteLine("Step 5");
            return urlContents.Length;
        }

        #endregion

        #region MsdnAllowCancel and Cancel

        private async void OnClickMsdnAllowCancel(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            this.buttonMsdnCancel1.IsEnabled = true;
            b.IsEnabled = false;
            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            // ***Set up the CancellationTokenSource to cancel after 2.5 seconds. (You 
            // can adjust the time.)
            cts.CancelAfter(4500);

            resultsTextBox.Clear();

            try
            {
                await AccessTheWebAsync(cts.Token);
                // ***Small change in the display lines.
                resultsTextBox.Text += "\r\nDownloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownloads canceled.";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownloads failed.";
            }

            // Set the CancellationTokenSource to null when the download is complete.
            cts = null;
            b.IsEnabled = true;
            this.buttonMsdnCancel1.IsEnabled = false;
        }

        private void OnClickMsdnCancelOp(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
            this.buttonMsdnCancel1.IsEnabled = false;
        }

        // Provide a parameter for the CancellationToken. 
        // ***Change the return type to Task because the method has no return statement.
        async Task AccessTheWebAsync(CancellationToken ct)
        {
            // Declare an HttpClient object.
            HttpClient client = new HttpClient();

            // ***Call SetUpURLList to make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // ***Add a loop to process the list of web addresses. 
            foreach (var url in urlList)
            {
                // GetAsync returns a Task<HttpResponseMessage>.  
                // Argument ct carries the message if the Cancel button is chosen.  
                // ***Note that the Cancel button can cancel all remaining downloads.
                HttpResponseMessage response = await client.GetAsync(url, ct);

                // Retrieve the website contents from the HttpResponseMessage. 
                byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

                resultsTextBox.Text +=
                    String.Format("\r\nLength of the downloaded string: {0}.\r\n", urlContents.Length);
            }
        }

        #endregion

        #region CancleWhenAny

        private async void OnClickCancelWhenAny(object sender, RoutedEventArgs e)
        {
            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            resultsTextBox.Clear();

            try
            {
                await AccessTheWebAsyncWhenAny(cts.Token);
                resultsTextBox.Text += "\r\nDownload complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownload canceled.";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownload failed.";
            }

            // Set the CancellationTokenSource to null when the download is complete.
            cts = null;

        }

        // Provide a parameter for the CancellationToken.
        async Task AccessTheWebAsyncWhenAny(CancellationToken ct)
        {
            HttpClient client = new HttpClient();

            // Call SetUpURLList to make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // ***Comment out or delete the loop. 
            //foreach (var url in urlList) 
            //{ 
            //    // GetAsync returns a Task<HttpResponseMessage>.  
            //    // Argument ct carries the message if the Cancel button is chosen.  
            //    // ***Note that the Cancel button can cancel all remaining downloads. 
            //    HttpResponseMessage response = await client.GetAsync(url, ct); 

            //    // Retrieve the website contents from the HttpResponseMessage. 
            //    byte[] urlContents = await response.Content.ReadAsByteArrayAsync(); 

            //    resultsTextBox.Text += 
            //        String.Format("\r\nLength of the downloaded string: {0}.\r\n", urlContents.Length);
            //} 

            // ***Create a query that, when executed, returns a collection of tasks.
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in urlList select ProcessURLAsync(url, client, ct);

            // ***Use ToArray to execute the query and start the download tasks. 
            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            // ***Call WhenAny and then await the result. The task that finishes  
            // first is assigned to firstFinishedTask.
            Task<int> firstFinishedTask = await Task.WhenAny(downloadTasks);

            // ***Cancel the rest of the downloads. You just want the first one.
            cts.Cancel();

            // ***Await the first completed task and display the results.  
            // Run the program several times to demonstrate that different 
            // websites can finish first. 
            var length = await firstFinishedTask;
            resultsTextBox.Text += String.Format("\r\nLength of the downloaded website:  {0}\r\n", length);
        }


        // ***Bundle the processing steps for a website into one async method.
        async Task<int> ProcessURLAsync(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>. 
            HttpResponseMessage response = await client.GetAsync(url, ct);

            // Retrieve the website contents from the HttpResponseMessage. 
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return urlContents.Length;
        }



        #endregion

        #region Process As Tasks Finish

        private async void OnClickProcessAsFinish(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            this.buttonMsdnAsFinishCancel.IsEnabled = true;
            b.IsEnabled = false;

            resultsTextBox.Clear();

            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsyncAsFinish(cts.Token);
                resultsTextBox.Text += "\r\nDownloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownloads canceled.\r\n";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownloads failed.\r\n";
            }

            cts = null;
            this.buttonMsdnAsFinishCancel.IsEnabled = false;
            b.IsEnabled = true;
        }

        private void OnClickCancelAsFinish(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
            this.buttonMsdnAsFinishCancel.IsEnabled = false;
        }

        async Task AccessTheWebAsyncAsFinish(CancellationToken ct)
        {
            HttpClient client = new HttpClient();

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // ***Create a query that, when executed, returns a collection of tasks.
            IEnumerable<Task<UrlAndSize>> downloadTasksQuery =
                from url in urlList select ProcessURL(url, client, ct);

            // ***Use ToList to execute the query and start the tasks. 
            List<Task<UrlAndSize>> downloadTasks = downloadTasksQuery.ToList();

            // ***Add a loop to process the tasks one at a time until none remain. 
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.
                Task<UrlAndSize> firstFinishedTask = await Task.WhenAny(downloadTasks);

                // ***Remove the selected task from the list so that you don't 
                // process it more than once.
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task. 
                UrlAndSize info = await firstFinishedTask;
                resultsTextBox.Text += String.Format("\r\nLength of the download:  {0} {1} ", info.Size , info.Url );
            }
        }

        async Task<UrlAndSize> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>. 
            HttpResponseMessage response = await client.GetAsync(url, ct);

            // Retrieve the website contents from the HttpResponseMessage. 
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return new UrlAndSize { Size = urlContents.Length, Url = url };
        }

        #endregion

        #region File Read/Write

        private async void OnClickFileR_W(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Calling ProcessWrite");
            await this.ProcessWrite();
            Debug.WriteLine("Calling ProcessRead");
            await this.ProcessRead();
            Debug.WriteLine("Calling Process Multiple Writes");
            await this.ProcessWriteMult();

        }

        public async Task ProcessWrite()
        {
            string filePath = @"temp2.txt";
            string text = "Hello World\r\n";

            await WriteTextAsync(filePath, text);
        }

        private async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        public async Task ProcessRead()
        {
            string filePath = @"temp2.txt";

            if (File.Exists(filePath) == false)
            {
                Debug.WriteLine("file not found: " + filePath);
            }
            else
            {
                try
                {
                    string text = await ReadTextAsync(filePath);
                    Debug.WriteLine(text);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            File.Delete(filePath);
        }

        private async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }

        public async Task ProcessWriteMult()
        {
            string folder = @"tempfolder\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            List<Task> tasks = new List<Task>();
            List<FileStream> sourceStreams = new List<FileStream>();

            try
            {
                for (int index = 1; index <= 10; index++)
                {
                    string text = "In file " + index.ToString() + "\r\n";

                    string fileName = "thefile" + index.ToString("00") + ".txt";
                    string filePath = folder + fileName;

                    byte[] encodedText = Encoding.Unicode.GetBytes(text);

                    FileStream sourceStream = new FileStream(filePath,
                        FileMode.Append, FileAccess.Write, FileShare.None,
                        bufferSize: 4096, useAsync: true);

                    Task theTask = sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                    sourceStreams.Add(sourceStream);

                    tasks.Add(theTask);
                    Debug.WriteLine("Added task to write file " + filePath);
                }

                await Task.WhenAll(tasks);
                Debug.WriteLine("All files written");
            }

            finally
            {
                Debug.WriteLine("Closing file handles");
                foreach (FileStream sourceStream in sourceStreams)
                {
                    sourceStream.Close();
                }
            }
            Directory.Delete(folder, true);
        }




        #endregion


    }

}
