using System;
using System.IO;
using System.IO.Compression;
using AntDeploy.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.zip");
            var publishPath = @"E:\WorkSpace\github\Lito.mgr\Lito.mgr.core\Lito.mgr.core\bin\Debug\netcoreapp2.1\publish";

            //ZipFile.CreateFromDirectory(publishPath, zipPath, CompressionLevel.Optimal, false);

           var stream = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal,false);
           
        }
    }
}
