// FileStreamTests.cs - NUnit2 Test Cases for System.IO.FileStream class
//
// Authors:
// 	Ville Palo (vi64pa@koti.soon.fi)
// 	Gert Driesen (gert.driesen@ardatis.com)
// 	Gonzalo Paniagua Javier (gonzalo@ximian.com)
// 
// (C) Ville Palo
// (c) 2003 Ximian, Inc. (http://www.ximian.com)
// 

using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace MonoTests.System.IO
{
	[TestFixture]
	public class FileStreamTest
	{
		string TempFolder = Path.Combine (Path.GetTempPath (), "MonoTests.System.IO.Tests");
		static readonly char DSC = Path.DirectorySeparatorChar;

		[TearDown]
		public void TearDown ()
		{
			if (Directory.Exists (TempFolder))
				Directory.Delete (TempFolder, true);
		}

		[SetUp]
		public void SetUp ()
		{
			if (Directory.Exists (TempFolder))
				Directory.Delete (TempFolder, true);

			Directory.CreateDirectory (TempFolder);
		}

		public void TestCtr ()
		{
			string path = TempFolder + DSC + "testfilestream.tmp.1";
			DeleteFile (path);
			FileStream stream = null;
			try {
				stream = new FileStream (path, FileMode.Create);
			} finally {

				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void CtorArgumentException1 ()
		{
			FileStream stream;
			stream = new FileStream ("", FileMode.Create);
			stream.Close ();
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void CtorArgumentNullException ()
		{
			FileStream stream = new FileStream (null, FileMode.Create);
			stream.Close ();
		}

		[Test]
		[ExpectedException (typeof (FileNotFoundException))]
		public void CtorFileNotFoundException1 ()
		{
			string path = TempFolder + DSC + "thisfileshouldnotexists.test";
			DeleteFile (path);
			FileStream stream = null;
			try {
				stream = new FileStream (TempFolder + DSC + "thisfileshouldnotexists.test", FileMode.Open);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (FileNotFoundException))]
		public void CtorFileNotFoundException2 ()
		{
			string path = TempFolder + DSC + "thisfileshouldNOTexists.test";
			DeleteFile (path);
			FileStream stream = null;

			try {
				stream = new FileStream (TempFolder + DSC + "thisfileshouldNOTexists.test", FileMode.Truncate);
			} finally {
				if (stream != null)
					stream.Close ();

				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (IOException))]
		public void CtorIOException1 ()
		{
			string path = TempFolder + DSC + "thisfileshouldexists.test";
			FileStream stream = null;
			DeleteFile (path);
			try {
				stream = new FileStream (path, FileMode.CreateNew);
				stream.Close ();
				stream = null;
				stream = new FileStream (path, FileMode.CreateNew);
			} finally {

				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}

		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorArgumentOutOfRangeException1 ()
		{
			FileStream stream = null;
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			try {
				stream = new FileStream (path, FileMode.Append | FileMode.CreateNew);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorArgumentOutOfRangeException2 ()
		{
			FileStream stream = null;
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			try {
				stream = new FileStream ("test.test.test", FileMode.Append | FileMode.Open);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		private void CtorDirectoryNotFoundException (FileMode mode)
		{
			string path = TempFolder + DSC + "thisDirectoryShouldNotExists";
			if (Directory.Exists (path))
				Directory.Delete (path, true);

			FileStream stream = null;
			try {
				stream = new FileStream (path + DSC + "eitherthisfile.test", mode);
			} finally {

				if (stream != null)
					stream.Close ();

				if (Directory.Exists (path))
					Directory.Delete (path, true);
			}
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_CreateNew ()
		{
			CtorDirectoryNotFoundException (FileMode.CreateNew);
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_Create ()
		{
			CtorDirectoryNotFoundException (FileMode.Create);
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_Open ()
		{
			CtorDirectoryNotFoundException (FileMode.Open);
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_OpenOrCreate ()
		{
			CtorDirectoryNotFoundException (FileMode.OpenOrCreate);
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_Truncate ()
		{
			CtorDirectoryNotFoundException (FileMode.Truncate);
		}

		[Test]
		[ExpectedException (typeof (DirectoryNotFoundException))]
		public void CtorDirectoryNotFoundException_Append ()
		{
			CtorDirectoryNotFoundException (FileMode.Append);
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorArgumentOutOfRangeException3 ()
		{
			string path = TempFolder + DSC + "CtorArgumentOutOfRangeException1";
			DeleteFile (path);

			FileStream stream = null;
			try {
				stream = new FileStream (path, FileMode.CreateNew, FileAccess.Read, FileShare.None | FileShare.Inheritable);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorArgumentOutOfRangeException4 ()
		{
			string path = TempFolder + DSC + "CtorArgumentOutOfRangeException4";
			DeleteFile (path);

			FileStream stream = null;
			try {
				stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite, -1);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorBufferSizeZero ()
		{
			// Buffer size can't be zero

			string path = Path.Combine (TempFolder, "CtorBufferSizeZero");
			DeleteFile (path);

			FileStream stream = null;
			try {
				stream = new FileStream (path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, 0);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void CtorArgumentException2 ()
		{
			// FileMode.CreateNew && FileAccess.Read

			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			FileStream stream = null;

			DeleteFile (path);

			try {
				stream = new FileStream (".test.test.test.2", FileMode.CreateNew, FileAccess.Read, FileShare.None | FileShare.Write);
			} finally {

				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}


		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void CtorArgumentOutOfRangeException5 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			FileStream stream = null;
			try {
				stream = new FileStream (path, FileMode.CreateNew, FileAccess.Read, FileShare.Inheritable | FileShare.ReadWrite);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}


		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void CtorArgumentException3 ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			FileStream stream = null;

			DeleteFile (path);

			try {
				stream = new FileStream (".test.test.test.2", FileMode.Truncate, FileAccess.Read);
			} finally {
				if (stream != null)
					stream.Close ();

				DeleteFile (path);
			}
		}

		[Test, ExpectedException (typeof (IOException))]
		public void CtorIOException2 ()
		{
			FileStream stream = null;
			try {
				stream = new FileStream (new IntPtr (Int32.MaxValue), FileAccess.Read);
			} finally {
				if (stream != null)
					stream.Close ();
			}
		}

		[Test, ExpectedException (typeof (IOException))]
		public void CtorIOException ()
		{
			string path = TempFolder + DSC + "CTorIOException.Test";
			FileStream stream = null;
			FileStream stream2 = null;
			DeleteFile (path);

			try {
				stream = new FileStream (path, FileMode.CreateNew);

				// used by an another process
				stream2 = new FileStream (path, FileMode.OpenOrCreate);
			} finally {
				if (stream != null)
					stream.Close ();
				if (stream2 != null)
					stream2.Close ();
				DeleteFile (path);
			}
		}

		[Test]
		public void CtorAccess1Read2Read ()
		{
			FileStream fs = null;
			FileStream fs2 = null;
			string fn = Path.Combine (TempFolder, "temp");
			try {
				if (!File.Exists (fn)) {
					TextWriter tw = File.CreateText (fn);
					tw.Write ("FOO");
					tw.Close ();
				}
				fs = new FileStream (fn, FileMode.Open, FileAccess.Read);
				fs2 = new FileStream (fn, FileMode.Open, FileAccess.Read);
			} finally {
				if (fs != null)
					fs.Close ();
				if (fs2 != null)
					fs2.Close ();
				if (File.Exists (fn))
					File.Delete (fn);
			}
		}

		[Test]
		[ExpectedException (typeof (IOException))]
		public void CtorAccess1Read2Write ()
		{
			string fn = Path.Combine (TempFolder, "temp");
			FileStream fs = null;
			try {
				if (!File.Exists (fn)) {
					using (TextWriter tw = File.CreateText (fn)) {
						tw.Write ("FOO");
					}
				}
				fs = new FileStream (fn, FileMode.Open, FileAccess.Read);
				fs = new FileStream (fn, FileMode.Create, FileAccess.Write);
			} finally {
				if (fs != null)
					fs.Close ();
				if (File.Exists (fn))
					File.Delete (fn);
			}
		}

		[Test]
		[ExpectedException (typeof (IOException))]
		public void CtorAccess1Write2Write ()
		{
			string fn = Path.Combine (TempFolder, "temp");
			FileStream fs = null;
			try {
				if (File.Exists (fn))
					File.Delete (fn);
				fs = new FileStream (fn, FileMode.Create, FileAccess.Write);
				fs = new FileStream (fn, FileMode.Create, FileAccess.Write);
			} finally {
				if (fs != null)
					fs.Close ();
				if (File.Exists (fn))
					File.Delete (fn);
			}
		}

		[Test]
		[ExpectedException (typeof (UnauthorizedAccessException))]
		public void CtorReadDirectoryAsFile ()
		{
			FileStream stream = null;
			try {
				stream = new FileStream (TempFolder, FileMode.Open, FileAccess.Read);
			} finally {
				if (stream != null)
					stream.Close ();
			}
		}

		[Test]
		public void Write ()
		{
			string path = TempFolder + DSC + "FileStreamTest.Write";

			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 8);

			byte[] outbytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			byte[] bytes = new byte[15];

			// Check that the data is flushed when we overflow the buffer
			// with a large amount of data
			stream.Write (outbytes, 0, 5);
			stream.Write (outbytes, 5, 10);
			stream.Seek (0, SeekOrigin.Begin);

			stream.Read (bytes, 0, 15);
			for (int i = 0; i < 15; ++i)
				Assert.AreEqual (i + 1, bytes[i], "#1");

			// Check that the data is flushed when we overflow the buffer
			// with a small amount of data
			stream.Write (outbytes, 0, 7);
			stream.Write (outbytes, 7, 7);
			stream.Write (outbytes, 14, 1);

			stream.Read (bytes, 0, 15);
			stream.Seek (15, SeekOrigin.Begin);
			for (int i = 0; i < 15; ++i)
				Assert.AreEqual (i + 1, bytes[i], "#2");
			stream.Close ();
		}

		[Test]
		public void Length ()
		{
			// Test that the Length property takes into account the data
			// in the buffer
			string path = TempFolder + DSC + "FileStreamTest.Length";

			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew);

			byte[] outbytes = new byte[] { 1, 2, 3, 4 };

			stream.Write (outbytes, 0, 4);
			Assert.AreEqual (4, stream.Length);
			stream.Close ();
		}

		[Test]
		public void Flush ()
		{
			string path = TempFolder + DSC + "FileStreamTest.Flush";
			FileStream stream = null;
			FileStream stream2 = null;

			DeleteFile (path);

			try {
				stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
				stream2 = new FileStream (path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

				stream.Write (new byte[] { 1, 2, 3, 4, 5 }, 0, 5);

				byte[] bytes = new byte[5];
				stream2.Read (bytes, 0, 5);
				Assert.AreEqual (0, bytes[0], "#A1");
				Assert.AreEqual (0, bytes[1], "#A2");
				Assert.AreEqual (0, bytes[2], "#A3");
				Assert.AreEqual (0, bytes[3], "#A4");

				stream.Flush ();
				stream2.Read (bytes, 0, 5);
				Assert.AreEqual (1, bytes[0], "#B1");
				Assert.AreEqual (2, bytes[1], "#B2");
				Assert.AreEqual (3, bytes[2], "#B3");
				Assert.AreEqual (4, bytes[3], "#B4");
			} finally {
				if (stream != null)
					stream.Close ();
				if (stream2 != null)
					stream2.Close ();

				DeleteFile (path);
			}
		}

		public void TestDefaultProperties ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "testfilestream.tmp.2";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.Create);

			Assert.IsTrue (stream.CanRead, "#A1");
			Assert.IsTrue (stream.CanSeek, "#A2");
			Assert.IsTrue (stream.CanWrite, "#A3");
			Assert.IsFalse (stream.IsAsync, "#A4");
			Assert.IsTrue (stream.Name.EndsWith (path), "#A5");
			Assert.AreEqual (0, stream.Position, "#A6");
			Assert.AreEqual ("System.IO.FileStream", stream.ToString (), "#A7");
			stream.Close ();
			DeleteFile (path);

			stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read);
			Assert.IsTrue (stream.CanRead, "#B1");
			Assert.IsTrue (stream.CanSeek, "#B2");
			Assert.IsFalse (stream.CanWrite, "#B3");
			Assert.IsFalse (stream.IsAsync, "#B4");
			Assert.IsTrue (stream.Name.EndsWith (path), "#B5");
			Assert.AreEqual (0, stream.Position, "#B6");
			Assert.AreEqual ("System.IO.FileStream", stream.ToString (), "#B7");
			stream.Close ();

			stream = new FileStream (path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
			Assert.IsFalse (stream.CanRead, "#C1");
			Assert.IsTrue (stream.CanSeek, "#C2");
			Assert.IsTrue (stream.CanWrite, "#C3");
			Assert.IsFalse (stream.IsAsync, "#C4");
			Assert.IsTrue (stream.Name.EndsWith ("testfilestream.tmp.2"), "#C5");
			Assert.AreEqual (0, stream.Position, "#C6");
			Assert.AreEqual ("System.IO.FileStream", stream.ToString (), "#C7");
			stream.Close ();
			DeleteFile (path);
		}

		[Category ("NotWorking")]
		// Bug: 71371 -> duplicate and WONTFIX.
		public void TestLock_FailsOnMono ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "TestLock";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite);

			stream.Write (new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);
			stream.Close ();

			stream = new FileStream (path, FileMode.Open, FileAccess.ReadWrite);

			stream.Lock (0, 5);

			FileStream stream2 = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			byte[] bytes = new byte[5];
			try {
				stream2.Read (bytes, 0, 5);
				Assert.Fail ("#1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (IOException), e.GetType (), "#2");
			}

			stream.Close ();
			stream2.Close ();

			DeleteFile (path);
		}

		public void TestLock ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "TestLock";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite);

			stream.Write (new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);
			stream.Close ();

			stream = new FileStream (path, FileMode.Open, FileAccess.ReadWrite);

			stream.Lock (0, 5);

			FileStream stream2 = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			byte[] bytes = new byte[5];
			try {
				stream2.Read (bytes, 0, 5);
				Assert.Fail ("#A1");
			} catch (Exception e) {
				// Bug #71371: on MS.NET you get an IOException detailing a lock
				// Assert.AreEqual (typeof (IOException), e.GetType (), "#A2");
			}

			stream2.Seek (5, SeekOrigin.Begin);
			stream2.Read (bytes, 0, 5);

			Assert.AreEqual (5, bytes[0], "#B1");
			Assert.AreEqual (6, bytes[1], "#B2");
			Assert.AreEqual (7, bytes[2], "#B3");
			Assert.AreEqual (8, bytes[3], "#B4");
			Assert.AreEqual (9, bytes[4], "#B5");

			stream.Unlock (0, 5);
			stream2.Seek (0, SeekOrigin.Begin);
			stream2.Read (bytes, 0, 5);

			Assert.AreEqual (0, bytes[0], "#C1");
			Assert.AreEqual (1, bytes[1], "#C2");
			Assert.AreEqual (2, bytes[2], "#C3");
			Assert.AreEqual (3, bytes[3], "#C4");
			Assert.AreEqual (4, bytes[4], "#C5");

			stream.Close ();
			stream2.Close ();

			DeleteFile (path);
		}

		[Test]
		public void Seek ()
		{
			string path = TempFolder + DSC + "FST.Seek.Test";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
			FileStream stream2 = new FileStream (path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			stream.Write (new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 10 }, 0, 9);
			Assert.AreEqual (5, stream2.Seek (5, SeekOrigin.Begin), "#1");
			Assert.AreEqual (-1, stream2.ReadByte (), "#2");

			Assert.AreEqual (2, stream2.Seek (-3, SeekOrigin.Current), "#3");
			Assert.AreEqual (-1, stream2.ReadByte (), "#4");

			Assert.AreEqual (12, stream.Seek (3, SeekOrigin.Current), "#5");
			Assert.AreEqual (-1, stream.ReadByte (), "#6");

			Assert.AreEqual (5, stream.Seek (-7, SeekOrigin.Current), "#7");
			Assert.AreEqual (6, stream.ReadByte (), "#8");

			Assert.AreEqual (5, stream2.Seek (5, SeekOrigin.Begin), "#9");
			Assert.AreEqual (6, stream2.ReadByte (), "#10");

			stream.Close ();
			stream2.Close ();

			DeleteFile (path);
		}

		public void TestSeek ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "TestSeek";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
			stream.Write (new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 10);

			stream.Seek (5, SeekOrigin.End);
			Assert.AreEqual (-1, stream.ReadByte (), "#1");

			stream.Seek (-5, SeekOrigin.End);
			Assert.AreEqual (6, stream.ReadByte (), "#2");

			try {
				stream.Seek (-11, SeekOrigin.End);
				Assert.Fail ("#3");
			} catch (Exception e) {
				Assert.AreEqual (typeof (IOException), e.GetType (), "#4");
			}

			stream.Seek (19, SeekOrigin.Begin);
			Assert.AreEqual (-1, stream.ReadByte (), "#5");

			stream.Seek (1, SeekOrigin.Begin);
			Assert.AreEqual (2, stream.ReadByte (), "#6");

			stream.Seek (3, SeekOrigin.Current);
			Assert.AreEqual (6, stream.ReadByte (), "#7");

			stream.Seek (-2, SeekOrigin.Current);
			Assert.AreEqual (5, stream.ReadByte (), "#8");

			stream.Flush ();

			// Test that seeks work correctly when seeking inside the buffer
			stream.Seek (0, SeekOrigin.Begin);
			stream.WriteByte (0);
			stream.WriteByte (1);
			stream.Seek (0, SeekOrigin.Begin);
			byte[] buf = new byte[1];
			buf[0] = 2;
			stream.Write (buf, 0, 1);
			stream.Write (buf, 0, 1);
			stream.Flush ();
			stream.Seek (0, SeekOrigin.Begin);
			Assert.AreEqual (2, stream.ReadByte (), "#9");
			Assert.AreEqual (2, stream.ReadByte (), "#10");

			stream.Close ();

			DeleteFile (path);
		}

		public void TestClose ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "TestClose";
			DeleteFile (path);

			FileStream stream = new FileStream (path, FileMode.CreateNew, FileAccess.ReadWrite);

			stream.Write (new byte[] { 1, 2, 3, 4 }, 0, 4);
			stream.ReadByte ();
			stream.Close ();

			try {
				stream.ReadByte ();
				Assert.Fail ("#A1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (ObjectDisposedException), e.GetType (), "#A2");
			}

			try {
				stream.WriteByte (64);
				Assert.Fail ("#B1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (ObjectDisposedException), e.GetType (), "#B2");
			}

			try {
				stream.Flush ();
				Assert.Fail ("#C1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (ObjectDisposedException), e.GetType (), "#C2");
			}

			try {
				long l = stream.Length;
				Assert.Fail ("#D1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (ObjectDisposedException), e.GetType (), "#D2");
			}

			try {
				long l = stream.Position;
				Assert.Fail ("#E1");
			} catch (Exception e) {
				Assert.AreEqual (typeof (ObjectDisposedException), e.GetType (), "#E2");
			}

			Assert.IsFalse (stream.CanRead, "#F1");
			Assert.IsFalse (stream.CanSeek, "#F2");
			Assert.IsFalse (stream.CanWrite, "#F3");
			Assert.IsTrue (stream.Name.EndsWith (path), "#F4");

			DeleteFile (path);
		}


		/// <summary>
		/// Checks whether the <see cref="FileStream" /> throws a <see cref="NotSupportedException" />
		/// when the stream is opened with access mode <see cref="FileAccess.Read" /> and the
		/// <see cref="FileStream.Write(byte[], int, int)" /> method is called.
		/// </summary>
		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void TestWriteVerifyAccessMode ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			FileStream stream = null;
			byte[] buffer;

			try {
				buffer = Encoding.ASCII.GetBytes ("test");
				stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read);
				stream.Write (buffer, 0, buffer.Length);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		/// <summary>
		/// Checks whether the <see cref="FileStream" /> throws a <see cref="NotSupportedException" />
		/// when the stream is opened with access mode <see cref="FileAccess.Read" /> and the
		/// <see cref="FileStream.WriteByte(byte)" /> method is called.
		/// </summary>
		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void TestWriteByteVerifyAccessMode ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			FileStream stream = null;

			try {
				stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read);
				stream.WriteByte (Byte.MinValue);
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		/// <summary>
		/// Checks whether the <see cref="FileStream" /> throws a <see cref="NotSupportedException" />
		/// when the stream is opened with access mode <see cref="FileAccess.Write" /> and the
		/// <see cref="FileStream.Read(byte[], int, int)" /> method is called.
		/// </summary>
		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void TestReadVerifyAccessMode ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			FileStream stream = null;
			byte[] buffer = new byte[100];

			try {
				stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
				stream.Read (buffer, 0, buffer.Length);
			} finally {
				if (stream != null)
					stream.Close ();
			}
		}

		/// <summary>
		/// Checks whether the <see cref="FileStream" /> throws a <see cref="NotSupportedException" />
		/// when the stream is opened with access mode <see cref="FileAccess.Write" /> and the
		/// <see cref="FileStream.ReadByte()" /> method is called.
		/// </summary>
		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void TestReadByteVerifyAccessMode ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			FileStream stream = null;

			try {
				stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
				int readByte = stream.ReadByte ();
			} finally {
				if (stream != null)
					stream.Close ();
				DeleteFile (path);
			}
		}

		// Check that the stream is flushed even when it doesn't own the
		// handle
		[Test]
		public void TestFlushNotOwningHandle ()
		{
			string path = Path.Combine (TempFolder, "TestFlushNotOwningHandle");
			DeleteFile (path);

			FileStream s = new FileStream (path, FileMode.Create);
			using (FileStream s2 = new FileStream (s.Handle, FileAccess.Write, false)) {
				byte[] buf = new byte[2];
				buf[0] = (int) '1';
				s2.Write (buf, 0, 1);
			}

			s.Position = 0;
			Assert.AreEqual ((int) '1', s.ReadByte ());
			s.Close ();
		}

		private void DeleteFile (string path)
		{
			if (File.Exists (path))
				File.Delete (path);
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Read_OffsetNegative ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Read (new byte[0], -1, 1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Read_OffsetOverflow ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Read (new byte[0], Int32.MaxValue, 1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Read_CountNegative ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Read (new byte[0], 1, -1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Read_CountOverflow ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Read (new byte[0], 1, Int32.MaxValue);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Write_OffsetNegative ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.Write (new byte[0], -1, 1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Write_OffsetOverflow ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.Write (new byte[0], Int32.MaxValue, 1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentOutOfRangeException))]
		public void Write_CountNegative ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.Write (new byte[0], 1, -1);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Write_CountOverflow ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.Write (new byte[0], 1, Int32.MaxValue);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Seek_InvalidSeekOrigin ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Seek (0, (SeekOrigin) (-1));
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Constructor_InvalidFileHandle ()
		{
			new FileStream ((IntPtr) (-1L), FileAccess.Read);
		}

		[Test]
		public void PositionAfterSetLength ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);

			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.SetLength (32);
				stream.Position = 32;
				stream.SetLength (16);
				Assert.AreEqual (16, stream.Position);
			}
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void SetLength_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
			stream.Close ();
			stream.SetLength (16);
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void Position_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read);
			stream.Close ();
			stream.Position = 0;
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void BeginRead_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read);
			stream.Close ();
			stream.EndRead (stream.BeginRead (new byte[8], 0, 8, null, null));
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void BeginWrite_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
			stream.Close ();
			stream.EndWrite (stream.BeginWrite (new byte[8], 0, 8, null, null));
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void Lock_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
			stream.Close ();
			stream.Lock (0, 1);
		}

		[Test]
		[ExpectedException (typeof (ObjectDisposedException))]
		public void Unlock_Disposed ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
			stream.Close ();
			stream.Unlock (0, 1);
		}

		[Test]
		public void ReadBytePastEndOfStream ()
		{
			string path = TempFolder + Path.DirectorySeparatorChar + "temp";
			DeleteFile (path);
			using (FileStream stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Read)) {
				stream.Seek (0, SeekOrigin.End);
				Assert.AreEqual (-1, stream.ReadByte ());
				stream.Close ();
			}
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void SetLengthWithClosedBaseStream ()
		{
			string fn = Path.Combine (TempFolder, "temp");
			try {
				FileStream fs = new FileStream (fn, FileMode.Create);
				BufferedStream bs = new BufferedStream (fs);
				fs.Close ();

				bs.SetLength (1000);
			} finally {
				File.Delete (fn);
			}
		}
	}
}
