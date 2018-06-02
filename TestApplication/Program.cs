namespace TestApplication {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Hes.Log.Lib;
    using Hes.Socket;

    internal class Program {

        /// <summary>
        /// Logger for current class.
        /// </summary>
        private static readonly ILogger Logger = LoggerUtil.GetLogger(typeof(Program));

        static Program() {
            AppDomain.CurrentDomain.UnhandledException += (a, b) => CurrentDomain_UnhandledException(a, b);
        }

        private static void Main(string[] args) {
            MainAsync(args).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Workaround for async main.
        /// </summary>
        /// <param name="args">args</param>
        /// <returns></returns>
        private static async Task MainAsync(string[] args) {
            new Server().Test();
            await Task.Delay(2000);
            var tcpClients = Enumerable.Range(0, 400).Select(x => new TcpClient() { NoDelay = true }).ToList();

            tcpClients.ForEach(c => c.Connect("127.0.0.1", 5000));
            while (tcpClients.Any(c => c.Connected == false)) {
                Thread.SpinWait(1);
            }

            var myClient = tcpClients[0];
            new Thread(async () => {
                while (true) {
                    var buffer = new byte[4];
                    var got = await myClient.GetStream().ReadAsync(buffer, 0, 4);
                    var s = BitConverter.ToInt32(buffer, 0);
                    buffer = new byte[s];
                    got = await myClient.GetStream().ReadAsync(buffer, 0, s);
                    Console.WriteLine($"{Encoding.UTF8.GetString(buffer)}");
                }
            }).Start();
            var data = Encoding.UTF8.GetBytes("Hasan Emrah SUNGU");
            var payload = BitConverter.GetBytes(data.Length + 2);
            var operationType = BitConverter.GetBytes((short)Operation.Data);
            var final = payload.Concat(operationType).Concat(data).ToArray();
            while (true) {
                Stopwatch sw = Stopwatch.StartNew();

                tcpClients.ForEach(t => t.GetStream().WriteAsync(final, 0, final.Length));
                //var qwe = 0;
                //var tasks = tcpClients.Aggregate(new Task[tcpClients.Count], (list, x) => {
                //    list[qwe++] = x.GetStream().WriteAsync(final, 0, final.Length);
                //    return list;
                //});
                //Task.WaitAll(tasks.ToArray());
                //Console.WriteLine($"Time: {DateTime.Now:ss.fff}; Update time:{ sw.ElapsedMilliseconds}; ConnectedPeers:{tcpClients.Count(x=>x.Connected)}");
                await Task.Delay(100);
            }
            //var tcpClient = new TcpClient("127.0.0.1", 5000) {NoDelay = true};
            //var data = Encoding.UTF8.GetBytes("Emrah");
            //var payload = BitConverter.GetBytes(data.Length + 2);
            //var operationtype = BitConverter.GetBytes((short) Operation.Data);

            //tcpClient.GetStream().Write(payload, 0, payload.Length - 3);
            //Thread.Sleep(10000);
            //tcpClient.GetStream().Write(payload, payload.Length - 3, 3);
            //tcpClient.GetStream().Write(operationtype, 0, operationtype.Length);
            //tcpClient.GetStream().Write(data, 0, data.Length);

            //Thread.Sleep(30000);

            //tcpClient.Dispose();
            var b = new byte[60000];
            new Random().NextBytes(b);
            var counter = 1;
            var datagram = new Datagram {
                Session = 1234567,
                Type = DatagramType.Data,
                Data = Encoding.UTF8.GetBytes("Hasan Emrah SUNGU"),
                AckNo = counter
            };
            var clients = Enumerable.Range(0, 15000).Select(x => new UdpClient("127.0.0.1", 5001)).ToList();
            var client = new UdpClient("127.0.0.1", 5001);
            var client2 = new UdpClient("127.0.0.1", 5001);
            while (true) {
                //clients.ForEach(a=>a.SendAsync(b,b.Length));
                byte[] toSend = datagram;
                var x = await client.SendAsync(toSend, toSend.Length);
                datagram.AckNo++;
                await Task.Delay(500);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Every unhandled error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e,
                                                             [CallerFilePath] string callerFilePath = null,
                                                             [CallerMemberName] string callerMemberName = null,
                                                             [CallerLineNumber] int lineNumber = 0
                                                             ) {
            Logger.Error($"CalledFrom: {callerFilePath}:{callerMemberName}:{lineNumber} Unhandled exception error", (Exception)e.ExceptionObject);
        }
    }
}