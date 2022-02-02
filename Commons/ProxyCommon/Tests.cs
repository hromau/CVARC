// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading;
// using System.Threading.Tasks;
// using Infrastructure;
// using NUnit.Framework;
//
// namespace ProxyCommon
// {
//     [TestFixture]
//     class ProxyTests
//     {
//         private const int Port = 31337;
//         private string dataToSend;
//         private TcpClient client;
//         private Proxy proxy;
//
//         [SetUp]
//         public void SetUp()
//         {
//             dataToSend = null;
//             var listener = new TcpListener(IPAddress.Loopback, Port);
//             listener.Start();
//             Task.Run(() =>
//             {
//                 var clientFrom = listener.AcceptTcpClient();
//                 var clientTo = listener.AcceptTcpClient();
//                 listener.Stop();
//                 proxy = new Proxy(clientFrom, clientTo);
//                 Task.Run(proxy.Work);
//             });
//             Task.Run(() => Worker(Port));
//             Thread.Sleep(100);
//             client = new TcpClient();
//             client.Connect(IPAddress.Loopback, Port);
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             client.Close();
//             proxy.Cancel();
//         }
//
//         [Test]
//         public void EmptySocket()
//         {
//             Assert.AreEqual(0, client.Available);
//         }
//
//         [Test]
//         public void CloseSocketCorrect()
//         {
//             Assert.IsTrue(client.IsAlive());
//             SetData("close");
//             Assert.IsFalse(client.IsAlive());
//         }
//
//         [Test]
//         public void DataPass()
//         {
//             SetData("hello!");
//             Assert.AreEqual("hello!", client.ReadJson<string>());
//         }
//
//         [Test]
//         public void HardBreak()
//         {
//             SetData("hardbreak");
//             var task = Task.Run(() => client.ReadJson<string>());
//             Thread.Sleep(100);
//             client.Close();
//             Thread.Sleep(100);
//             Assert.AreEqual(task.Exception.InnerException.GetType(), typeof(IOException));
//         }
//
//         [Test]
//         public void WaitMuch()
//         {
//             SetData("waitmuch");
//             SetData("so slow!");
//             Assert.AreEqual("so slow!", client.ReadJson<string>());
//         }
//
//         void Worker(int port)
//         {
//             var tcpClient = new TcpClient();
//             tcpClient.Connect(IPAddress.Loopback, port);
//             while (true)
//             {
//                 if (!tcpClient.IsAlive())
//                     return;
//                 if (dataToSend == "close")
//                 {
//                     tcpClient.Close();
//                     return;
//                 }
//                 if (dataToSend == "hardbreak")
//                     return;
//                 if (dataToSend == "waitmuch")
//                 {
//                     dataToSend = null;
//                     Thread.Sleep(5000);
//                     continue;
//                 }
//                 if (dataToSend == null)
//                 {
//                     Thread.Sleep(10);
//                     continue;
//                 }
//                 tcpClient.WriteJson(dataToSend);
//                 dataToSend = null;
//             }
//         }
//
//         void SetData(string data)
//         {
//             dataToSend = data;
//             Thread.Sleep(155);
//         }
//     }
// }
