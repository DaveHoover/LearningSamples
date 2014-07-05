using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MediaFileSorter.ViewModel;
using MediaFileSorter.Model;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public const string SourceFilePath = @"C:\Tmp\Photos\Source";

        public const string TargetFilePath = @"C:\Tmp\Photos\Target";

        public const string CombinedFilePath = @"C:\Tmp\Photos\SrcTarget";

        public const Int32 NumberOfFilesToCreate = 105;

        public Int32 NumberOfOrigSourceFilesRecursive { get; set; }
        public Int32 NumberOfOrigSourceFilesNonRecursive { get; set; }
        public Int32 NumberOfTargetFilesRecursive { get; set; }
        public Int32 NumberOfTargetFilesNonRecursive { get; set; }

        private string[] origSourceFilesRecursive;
        private string[] origSourceFilesNonRecursive;

        private string[] targetFilesRecursive;
        private string[] targetFilesNonRecursive;





        private MainWindowViewModel vm;
        private AppSettings         origConfig;


        [TestInitialize]
        public void TestInit()
        {
            vm = new MainWindowViewModel();
            origConfig = vm.Config;
            vm.Config = AppSettings.Load("Config.xml");
            vm.Config.SetAutoFileSavePath("Config.xml");
            vm.Config.SourceFolder = SourceFilePath;
            vm.Config.TargetFolder = TargetFilePath;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            vm.Config = origConfig;
            vm.Config.Save("Config.xml");
        }


        private void SaveOrigSourceFileStats()
        {
            this.origSourceFilesRecursive = Directory.GetFiles(vm.Config.SourceFolder, "*.*", SearchOption.AllDirectories);
            this.NumberOfOrigSourceFilesRecursive = this.origSourceFilesRecursive.Length;

            this.origSourceFilesNonRecursive = Directory.GetFiles(vm.Config.SourceFolder, "*.*", SearchOption.TopDirectoryOnly);
            this.NumberOfOrigSourceFilesNonRecursive = this.origSourceFilesNonRecursive.Length;
        }

        private void ReadTargetFileStats()
        {
            this.targetFilesRecursive = Directory.GetFiles(vm.Config.TargetFolder, "*.*", SearchOption.AllDirectories);
            this.NumberOfTargetFilesRecursive = this.targetFilesRecursive.Length;

            this.targetFilesNonRecursive = Directory.GetFiles(vm.Config.TargetFolder, "*.*", SearchOption.TopDirectoryOnly);
            this.NumberOfTargetFilesNonRecursive = this.targetFilesNonRecursive.Length;

        }

        private void CreateFileDirectories()
        {
            if (Directory.Exists(SourceFilePath))
            {
                Directory.Delete(SourceFilePath, true);
            }
            Directory.CreateDirectory(SourceFilePath);

            if (Directory.Exists(TargetFilePath))
            {
                Directory.Delete(TargetFilePath, true);
            }
            Directory.CreateDirectory(TargetFilePath);

            if (Directory.Exists(CombinedFilePath))
            {
                Directory.Delete(CombinedFilePath, true);
            }
            Directory.CreateDirectory(CombinedFilePath);

        }


        private void CreateStandardCameraPictureFiles (string directory)
        {
            this.CreateFileDirectories();
            this.CreatePictureFiles(directory);
            string dir1 = Path.Combine(directory, "Dir1");
            Directory.CreateDirectory(dir1);
            this.CreatePictureFiles(dir1);
            string dir2 = Path.Combine(directory, "Dir2");
            Directory.CreateDirectory(dir2);
            this.CreatePictureFiles(dir2);
        }

        private void CreatePictureFiles(string directory)
        {
            string time = DateTime.Now.ToString("r");
            time = time.Replace(':', '_');
            time = time.Replace(',', '_');
            time = time.Replace(' ', '_');
            for (Int32 i = 0; i < NumberOfFilesToCreate; i++)
            {
                string file = Path.Combine(directory, "Image_" + i.ToString() + time);
                FileStream f = File.Create(file);
                f.Close();
            }
        }

        private void CreateBadNumericalSortFiles(string directory)
        {
            for (Int32 i = 0; i < NumberOfFilesToCreate; i++)
            {
                string file = Path.Combine(directory, "Image" + i.ToString());
                FileStream f = File.Create(file);
                f.Close();
            }
        }

        private void CreateSongListFilesWithBadNumericalSort(string directory)
        {
            this.CreateFileDirectories();
            this.CreateBadNumericalSortFiles(directory);
            string dir1 = Path.Combine(SourceFilePath, "Dir1");
            Directory.CreateDirectory(dir1);
            this.CreateBadNumericalSortFiles(dir1);
            string dir2 = Path.Combine(SourceFilePath, "Dir2");
            Directory.CreateDirectory(dir2);
            this.CreateBadNumericalSortFiles(dir2);
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }


        [TestMethod]
        public void TestFixupOfFilesWithBadNumericalSortRecursiveSrcTargetSeparate()
        {
            this.CreateSongListFilesWithBadNumericalSort(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = true;
            this.vm.CorrectForNumericalSort(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesRecursive, this.NumberOfTargetFilesRecursive);
            for ( Int32 i = 0; i < origSourceFilesRecursive.Length; i++)
            {
                Assert.AreNotEqual<int>(origSourceFilesRecursive[i].Count(), targetFilesRecursive.Count());
            }
        }

        [TestMethod]
        public void TestFixupOfFilesWithBadNumericalSortNonRecursiveSrcTargetSeparate()
        {
            this.CreateSongListFilesWithBadNumericalSort(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = false;
            this.vm.CorrectForNumericalSort(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesNonRecursive, this.NumberOfTargetFilesNonRecursive);
            for (Int32 i = 0; i < origSourceFilesNonRecursive.Length; i++)
            {
                Assert.AreNotEqual<int>(origSourceFilesNonRecursive[i].Count(), targetFilesNonRecursive.Count());
            }

        }

        [TestMethod]
        public void TestFixupOfFilesWithBadNumericalSortRecursiveSrcTargetCombined()
        {
            this.CreateSongListFilesWithBadNumericalSort(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = true;
            this.vm.Config.TargetFolder = this.vm.Config.SourceFolder;
            this.vm.CorrectForNumericalSort(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesRecursive, this.NumberOfTargetFilesRecursive);
            for (Int32 i = 0; i < origSourceFilesRecursive.Length; i++)
            {
                Assert.AreNotEqual<int>(origSourceFilesRecursive[i].Count(), targetFilesRecursive.Count());
            }          
        }

        [TestMethod]
        public void TestFixupOfFilesWithBadNumericalSortNonRecursiveSrcTargetCombined()
        {
            this.CreateSongListFilesWithBadNumericalSort(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.TargetFolder = this.vm.Config.SourceFolder;
            this.vm.Config.ApplyRecursively = false;
            this.vm.CorrectForNumericalSort(true);

            // Verification
            this.ReadTargetFileStats();

            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesNonRecursive, this.NumberOfTargetFilesNonRecursive);
            for (Int32 i = 0; i < origSourceFilesNonRecursive.Length; i++)
            {
                Assert.AreNotEqual<int>(origSourceFilesNonRecursive[i].Count(), targetFilesNonRecursive.Count());
            }
        }

        [TestMethod]
        public void TestFixupOfCameraFilesRecursive()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = true;
            this.vm.CreateNumericalList(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesRecursive, this.NumberOfTargetFilesRecursive);
            for (Int32 i = 0; i < origSourceFilesRecursive.Length; i++)
            {
                Assert.IsTrue( this.targetFilesRecursive[i].Contains(vm.Config.Prefix) && 
                               this.targetFilesRecursive[i].Contains(vm.Config.MainString));
            }
        }

        [TestMethod]
        public void TestFixupOfCameraFilesNonRecursive()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = false;
            this.vm.CreateNumericalList(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesNonRecursive, this.NumberOfTargetFilesNonRecursive);
            for (Int32 i = 0; i < origSourceFilesNonRecursive.Length; i++)
            {
                Assert.IsTrue( this.targetFilesNonRecursive[i].Contains(vm.Config.Prefix) && 
                               this.targetFilesNonRecursive[i].Contains(vm.Config.MainString));
            }


        }

        [TestMethod]
        public void TestFixupOfCameraFilesRecursiveSrcTargetCombined()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.TargetFolder = this.vm.Config.SourceFolder;
            this.vm.Config.ApplyRecursively = true;
            this.vm.CreateNumericalList(true);

            // Verification
            this.ReadTargetFileStats();

            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesRecursive, this.NumberOfTargetFilesRecursive);
            for (Int32 i = 0; i < origSourceFilesRecursive.Length; i++)
            {
                Assert.IsTrue(this.targetFilesRecursive[i].Contains(vm.Config.Prefix) &&
                               this.targetFilesRecursive[i].Contains(vm.Config.MainString));
            }
        }

        [TestMethod]
        public void TestFixupOfCameraFilesNonRecursiveSrcTargetCombined()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.TargetFolder = this.vm.Config.SourceFolder;
            this.vm.Config.ApplyRecursively = false;
            this.vm.CreateNumericalList(true);

            // Verification
            this.ReadTargetFileStats();

            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesNonRecursive, this.NumberOfTargetFilesNonRecursive);
            for (Int32 i = 0; i < origSourceFilesNonRecursive.Length; i++)
            {
                Assert.IsTrue(this.targetFilesNonRecursive[i].Contains(vm.Config.Prefix) &&
                               this.targetFilesNonRecursive[i].Contains(vm.Config.MainString));
            }


        }


        [TestMethod]
        public void TestSearchReplaceRecursive()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = true;
            this.vm.Config.SearchSource = "Image";
            this.vm.Config.SearchTarget = "Cannon";
            this.vm.SearchUpdate(true);

            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesRecursive, this.NumberOfTargetFilesRecursive);
            for (Int32 i = 0; i < origSourceFilesRecursive.Length; i++)
            {
                Assert.IsTrue(this.targetFilesRecursive[i].Contains(vm.Config.SearchTarget));
            }
        }

        [TestMethod]
        public void TestSearchReplaceNonRecursive()
        {
            this.CreateStandardCameraPictureFiles(SourceFilePath);
            this.SaveOrigSourceFileStats();
            this.vm.Config.ApplyRecursively = false;
            this.vm.Config.SearchSource = "Image";
            this.vm.Config.SearchTarget = "Cannon";
            this.vm.SearchUpdate(true);
            // Verification
            this.ReadTargetFileStats();
            Assert.AreEqual<Int32>(this.NumberOfOrigSourceFilesNonRecursive, this.NumberOfTargetFilesNonRecursive);

            for (Int32 i = 0; i < origSourceFilesNonRecursive.Length; i++)
            {
                Assert.IsTrue(this.targetFilesNonRecursive[i].Contains(vm.Config.SearchTarget));
            }
        }


        [TestMethod]
        public void CreateNumericalSeriesSourceFolderDoesNotExist()
        {
            this.vm.Config.SourceFolder = @"C:\NonExistantFolder";
            this.vm.CreateNumericalList(true);
        }

        [TestMethod]
        public void FixNumericalSortSourceFolderDoesNotExist()
        {
            this.vm.Config.SourceFolder = @"C:\NonExistantFolder";
            this.vm.CorrectForNumericalSort(true);

        }

        [TestMethod]
        public void SearchSourceFolderDoesNotExist()
        {
            this.vm.Config.SourceFolder = @"C:\NonExistantFolder";
            this.vm.SearchUpdate(true);
        }

        [TestMethod]
        public void RescanWhenSourceFolderDoesNotExist()
        {
            this.vm.Config.SourceFolder = @"C:\NonExistantFolder";
            this.vm.RescanButton(true);
        }


    }
}

