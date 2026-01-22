//using Networking;
//using System;

//class Program
//{
//    static void Main(string[] args)
//    {
//        TcpServer tcp = new TcpServer(7777);
//        UdpServer udp = new UdpServer(8888);

//        tcp.Start();
//        udp.Start();

//        Console.WriteLine("Server running...");
//        Console.ReadLine();
//    }
//}
using BomberServer.Tests;

ModelTest.Run();
Console.ReadKey();
return;
